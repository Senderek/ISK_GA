using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ISK.GA
{
    public class SchedulerFitness : IFitness
    {
        public List<Task> Tasks { get; set; }
        public List<Processor> Processors { get; set; }

        public SchedulerFitness(int numberOfTasks, int numberOfProcessors, List<Task> _tasks)
        {
            Tasks = new List<Task>(numberOfTasks);
            Processors = new List<Processor>(numberOfProcessors);
            Tasks = _tasks;
            for (int i = 0; i < numberOfProcessors; i++)
            {
                Processors.Add(new Processor(i));
            }
        }

        public double Evaluate(IChromosome chromosome)
        {
            var genes = chromosome.GetGenes();
            int[] procesorSums = new int[Processors.Count];
            for (int i = 0; i < Processors.Count; i++)
            {
                procesorSums[i] = 0;
            }

            for (int j = 0; j < genes.Length; j++)
            {
                var geneValue = Convert.ToInt32(genes[j].Value, CultureInfo.InvariantCulture);
                if (geneValue < 0 || geneValue > Processors.Count) return 0;
                procesorSums[geneValue] += Tasks[j].ProcessorCosts[geneValue];
            }
            var totalTime = procesorSums.Max();
            ((SchedulerChromosome)chromosome).Time = totalTime;
            var fitness = 1.0 / procesorSums.Max();
            if (fitness < 0)
            {
                fitness = 0;
            }

            return fitness;
        }



    }
}
