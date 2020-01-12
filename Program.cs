using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using GnuPlotHelpers;

namespace ISK.GA
{
    class Program
    {
        //public static int m_numberOfTaks = 5;
        //public static int m_numberOfProcessors = 3;
        //public static List<Task> tasks = new List<Task>();
        public static List<Result> Results { get; set; } = new List<Result>();
        public static List<string> colors = new List<string>()
        {
"#e6194b", "#3cb44b", "#ffe119", "#4363d8", "#f58231", "#911eb4", "#46f0f0", "#f032e6", "#bcf60c", "#fabebe", "#008080", "#e6beff", "#9a6324", "#fffac8", "#800000", "#aaffc3", "#808000", "#ffd8b1", "#000075", "#808080", "#ffffff", "#000000"
        };
        private static InputFile ReadFile()
        {
            var result = new InputFile();
            string[] lines = System.IO.File.ReadAllLines(@"C:\repos\ga\Test-Data-Generator-master\TestDataGenerator\bin\Debug\netcoreapp2.2\1.txt");
            result.m_numberOfTaks = int.Parse(lines[0].Trim());
            result.m_numberOfProcessors = int.Parse(lines[1].Trim());
            for (int i = 2; i < 2 + result.m_numberOfTaks; i++)
            {
                var line = lines[i].Trim();
                var numbers = line.Split(' ');
                var task = new Task(int.Parse(numbers[0]));
                task.ProcessorCosts = new List<int>(result.m_numberOfTaks);
                for (int j = 1; j < 1 + result.m_numberOfProcessors; j++)
                {
                    task.ProcessorCosts.Add(int.Parse(numbers[j]));
                }
                result.tasks.Add(task);
            }
            return result;
        }

        public static int GetChromosomeProcessorsTime(IChromosome chromosome, Func<int[], int> getTotalTimeSum)
        {
            var c = chromosome as SchedulerChromosome;
            var chP = c.GetGenes().Select(g => int.Parse(g.Value.ToString())).ToArray();
            var procesorsTotalTime = getTotalTimeSum(chP);
            return procesorsTotalTime;
        }

        /// <summary>
        /// GeneticSharp TSP Console Application template.
        /// <see href="https://github.com/giacomelli/GeneticSharp"/>
        /// </summary>
        static void Main(string[] args)
        {
            var fileInput = ReadFile();
            var getTotalTimeSum = new Func<int[], int>(genes =>
            {

                int[] procesorSums = new int[fileInput.m_numberOfProcessors];
                for (int i = 0; i < fileInput.m_numberOfProcessors; i++)
                {
                    procesorSums[i] = 0;
                }

                for (int j = 0; j < genes.Length; j++)
                {
                    var geneValue = genes[j];
                    if (geneValue < 0 || geneValue > fileInput.m_numberOfProcessors) return 0;
                    procesorSums[geneValue] += fileInput.tasks[j].ProcessorCosts[geneValue];
                }
                var max = procesorSums.Max();
                return max;
            });

            string nazwaPliku = "eksp10";
            Console.WriteLine("GA running...");
            //GnuPlot.SetTerminalToSaveToFile(nazwaPliku);
            GnuPlot.SetTitle("Wykres czasu działania procesorów od numeru generacji algorytmu");
            GnuPlot.HoldOn();
            for (int nrProby = 0; nrProby < 10; nrProby++)
            {
                var selection = new EliteSelection();
                var crossover = new UniformCrossover();
                var mutation = new TworsMutation();

                var fitness = new SchedulerFitness(fileInput.m_numberOfTaks, fileInput.m_numberOfProcessors, fileInput.tasks);
                var chromosome = new SchedulerChromosome(fileInput.m_numberOfTaks, fileInput.m_numberOfProcessors);

                var population = new Population(20000, 25000, chromosome);

                var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
                {
                    Termination = new OrTermination(new GenerationNumberTermination(6000), new FitnessStagnationTermination(600)),
                    CrossoverProbability = 0.9f,
                    MutationProbability = 0.1f,
                };
                var bestTour = 0;
                int? bestTime = null;
                var miniResults = new List<IterationResult>();
                double totalTimeEvolving = 0;
                var stopWatch = new Stopwatch();
                int generacja = 0;
                ga.GenerationRan += (s, e) =>
                {
                    generacja++;
                    var c = ga.BestChromosome as SchedulerChromosome;
                    //Console.WriteLine($"Generation: {ga.GenerationsNumber} -> Time: {c.Time}");
                    var chP = c.GetGenes().Select(g => int.Parse(g.Value.ToString())).ToArray();
                    //Console.WriteLine("Tour: {0}", string.Join(", ", chP));
                    totalTimeEvolving += ga.TimeEvolving.TotalMilliseconds;
                    var procesorsTotalTime = getTotalTimeSum(chP);
                    miniResults.Add(new IterationResult { Result = procesorsTotalTime, Time = stopWatch.ElapsedMilliseconds, GenerationNumber = generacja });
                    if (bestTime == null || getTotalTimeSum(chP) < bestTime.Value)
                    {
                        bestTime = procesorsTotalTime;
                        bestTour = ga.GenerationsNumber;
                    }
                };
                stopWatch.Start();
                ga.Start();
                //Console.WriteLine("Best solution found has {0} fitness.", ga.BestChromosome.Fitness);
                stopWatch.Stop();
                //Console.WriteLine($"Elapsed time: {ga.TimeEvolving}, best time {bestTime} at generation {bestTour}");
                Results.Add(new Result { Id = nrProby + 1, BestResult = GetChromosomeProcessorsTime(ga.BestChromosome, getTotalTimeSum), ElapsedTime = (int)ga.TimeEvolving.TotalMilliseconds, NumberOfGenerations = ga.GenerationsNumber, TasksResult = ga.BestChromosome.GetGenes().Select(x => (int)x.Value).ToList() });
                var color = colors[nrProby];
                GnuPlot.Plot(miniResults.Select(x => (double)x.GenerationNumber).ToArray(), miniResults.Select(x => (double)x.Result).ToArray(), $"with lines lt rgb \"{color}\" title \"Proba nr {nrProby + 1}\"");
            }
            TextWriter tw = new StreamWriter($"C:\\gnuplotsdatas\\{nazwaPliku}.txt");
            Result bestResult = null;
            foreach (var r in Results)
            {
                tw.WriteLine($"{r.BestResult} {r.ElapsedTime} {r.NumberOfGenerations}");
                if (bestResult == null || bestResult.BestResult > r.BestResult)
                {
                    bestResult = r;
                }
            }
            tw.Close();
            tw = new StreamWriter($"C:\\gnuplotsdatas\\{nazwaPliku}_proc.txt");
            tw.WriteLine($"{string.Join(',', bestResult.TasksResult)}");
            tw.Close();
            GnuPlot.Flush();
            Console.WriteLine("Done.");
        }
        private static List<int> GenerateRandomListOfInt(int size)
        {
            var random = new Random();
            var list = new List<int>(size);
            for (int i = 0; i < size; i++)
            {
                list.Add(random.Next(1, 100));
            }
            return list;
        }
    }
}
