﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using EngineController = ST.Client.EngineController;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Baseline;
using Oakton;
using StoryTeller;
using StoryTeller.Engine;
using StoryTeller.Model;
using StoryTeller.Model.Persistence;
using StoryTeller.Model.Persistence.DSL;
using StoryTeller.Portal.ResultsAggregator;
using StoryTeller.Portal.ResultsAggregator.Client;
using StoryTeller.Remotes;
using StoryTeller.Remotes.Messaging;
using StoryTeller.ResultAggregation.Models;
using StoryTeller.ResultAggregation.Models.ClientModel;
using StoryTeller.Results;
using ST.Client;
using ST.CommandLine;

namespace StoryTeller.RemoteRunner.Api.Controllers
{
    public class RunsController : ApiController
    {
        private PortalResultsAggregatorClient _portalClient;
        public RunsController()
        {
            _portalClient = new PortalResultsAggregatorClient(ConfigurationManager.AppSettings["PortalUrl"], ConfigurationManager.AppSettings["PortalApiKey"]);
        }

        [HttpPost]
        public async Task<bool> AddRun([FromBody] RunInput runInput)
        {
            try
            {
                BatchRunRequest batchRunRequest = runInput.GetBatchRunRequest();

                if (!batchRunRequest.Specifications.Any())
                {
                    ConsoleWriter.Write(ConsoleColor.Yellow, "Warning: No specs found!");
                }
            }
            catch (SuiteNotFoundException ex)
            {
                ConsoleWriter.Write(ConsoleColor.Red, ex.Message);
                return false;
            }

            ST.Client.EngineController controller = runInput.BuildEngine();
            Task<bool> task = controller.Start()
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                        throw t.Exception;

                    var systemRecycled = t.Result;
                    return executeAgainstTheSystem(runInput, systemRecycled, controller);
                });

            bool passed = await task;
            controller.SafeDispose();

            Run latestRun = await _portalClient.GetLatestRun();

            var runResult = new RunResult
            {
                RunId = latestRun.Id,
                HtmlResults = File.ReadAllText(runInput.ResultsPathFlag),
                Passed = passed
            };

            await _portalClient.CompleteRunAsync(runResult);

            return passed;
        }

        private bool executeAgainstTheSystem(RunInput input, SystemRecycled systemRecycled, EngineController controller)
        {
            if (!systemRecycled.success)
            {
                systemRecycled.WriteSystemUsage();
                return false;
            }

            writeSystemUsage(systemRecycled);

            if (input.ValidateFlag)
            {
                return validateOnly(input, systemRecycled);
            }

            var execution = input.StartBatch(controller);

            // TODO -- put a command level timeout on this thing
            execution.Wait();

            var results = execution.Result;

            if (input.VerboseFlag)
            {
                writeVerbose(results);
            }

            var success = determineSuccess(input, results);

            writeResults(input, systemRecycled, results);

            writeData(input, results);

            openResults(input);

            writeSuccessOrFailure(success);

            return success;
        }

        private void writeVerbose(BatchRunResponse results)
        {
            foreach (var record in results.records)
            {
                if (record.WasSuccessful())
                {
                    ConsoleWriter.Write(ConsoleColor.Green,
                        $"{record.specification.path} succeeded with {record.results.Counts} ");
                }
                else
                {
                    ConsoleWriter.Write(ConsoleColor.Red, $"{record.specification.path} failed with {record.results.Counts}");
                }
            }
        }

        private bool validateOnly(RunInput input, SystemRecycled systemRecycled)
        {
            var fixtures = buildFixturesWithOverrides(input, systemRecycled);
            var library = FixtureLibrary.From(fixtures);

            var specs = HierarchyLoader.ReadHierarchy(input.SpecPath).GetAllSpecs().ToArray();

            SpecificationPostProcessor.PostProcessAll(specs, library);

            var errored = specs.Where(x => x.errors.Any()).ToArray();

            if (errored.Any())
            {
                ConsoleWriter.Write(ConsoleColor.Red, "Errors Detected!");

                foreach (var errorSpec in errored)
                {
                    ConsoleWriter.Write(ConsoleColor.Yellow, errorSpec.Filename);
                    foreach (var error in errorSpec.errors)
                    {
                        Console.WriteLine($"{error.location.Join(" / ")} -> {error.message}");
                    }
                }

                return false;
            }
            else
            {
                ConsoleWriter.Write(ConsoleColor.Green, "No validation errors or missing data detected in this project");
                return true;
            }
        }



        private static void openResults(RunInput input)
        {
            if (input.OpenFlag)
            {
                Process.Start(input.ResultsPathFlag);
            }
        }

        private static void writeSuccessOrFailure(bool success)
        {
            if (success)
            {
                ConsoleWriter.Write(ConsoleColor.Green, "Success!");
            }
            else
            {
                ConsoleWriter.Write(ConsoleColor.Red, "Failed with Regression Failures!");
            }
        }

        private static bool determineSuccess(RunInput input, BatchRunResponse results)
        {
            var regression = results.Summarize(Lifecycle.Regression);
            var acceptance = results.Summarize(Lifecycle.Acceptance);

            if (input.LifecycleFlag != Lifecycle.Regression)
                Console.WriteLine(acceptance);

            if (input.LifecycleFlag != Lifecycle.Acceptance)
                Console.WriteLine(regression);

            return regression.Failed == 0;
        }

        private static void writeResults(RunInput input, SystemRecycled systemRecycled, BatchRunResponse results)
        {
            results.suite = input.WorkspaceFlag;
            results.system = systemRecycled.system_name;
            results.time = DateTime.Now.ToString();

            results.fixtures = buildFixturesWithOverrides(input, systemRecycled);

            var document = BatchResultsWriter.BuildResults(results);
            Console.WriteLine("Writing results to " + input.ResultsPathFlag);
            document.WriteToFile(input.ResultsPathFlag);
        }

        private static void writeData(RunInput input, BatchRunResponse results)
        {
            if (input.DumpFlag.IsNotEmpty())
                dumpJson(input, results);

            if (input.CsvFlag.IsNotEmpty())
                writePerformanceData(input, results);

            if (input.JsonFlag.IsNotEmpty())
            {
                Console.WriteLine("Writing the raw result information to " + input.JsonFlag);
                PerformanceDataWriter.WriteJSON(results, input.JsonFlag);
            }
        }

        private static FixtureModel[] buildFixturesWithOverrides(RunInput input, SystemRecycled systemRecycled)
        {
            var overrides = FixtureLoader.LoadFromPath(input.FixturePath);
            var system = new FixtureLibrary();
            foreach (var fixture in systemRecycled.fixtures)
            {
                system.Models[fixture.key] = fixture;
            }

            return system.ApplyOverrides(overrides).Models.ToArray();
        }

        private static void writePerformanceData(RunInput input, BatchRunResponse results)
        {
            Console.WriteLine("Writing performance data as CSV data to " + input.CsvFlag.ToFullPath());

            PerformanceDataWriter.WriteCSV(results, input.CsvFlag);
        }

        private static void dumpJson(RunInput input, BatchRunResponse results)
        {
            Console.WriteLine("Dumping the raw JSON results to " + input.DumpFlag);
            var json = JsonSerialization.ToJson(results);
            new FileSystem().WriteStringToFile(input.DumpFlag, json);
        }


        private void writeSystemUsage(SystemRecycled systemRecycled)
        {
            Console.WriteLine("Using System: " + systemRecycled.system_name);
            systemRecycled.properties.Each(pair => { Console.WriteLine("{0}: {1}", pair.Key, pair.Value); });
            Console.WriteLine();
        }
    }
}
