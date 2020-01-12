using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISK.GA
{
    public class SchedulerChromosome : ChromosomeBase
    {
        private readonly int m_numberOfTasks;
        private readonly int m_numberOfProcessors;

        public SchedulerChromosome(int numberOfTasks, int numberOfProcessors) : base(numberOfTasks)
        {
            m_numberOfTasks = numberOfTasks;
            m_numberOfProcessors = numberOfProcessors;
            var citiesIndexes = GetRandomProcessor(numberOfTasks, 0, numberOfProcessors);

            for (int i = 0; i < numberOfTasks; i++)
            {
                ReplaceGene(i, new Gene(citiesIndexes[i]));
            }
        }

        public int Time { get; internal set; }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(RandomizationProvider.Current.GetInt(0, m_numberOfTasks));
        }

        public override IChromosome CreateNew()
        {
            return new SchedulerChromosome(m_numberOfTasks, m_numberOfProcessors);
        }

        public override IChromosome Clone()
        {
            var clone = base.Clone() as SchedulerChromosome;
            clone.Time = Time;

            return clone;
        }

        public int[] GetRandomProcessor(int numberOfTasks, int minimum, int maximum)
        {
            var list = new List<int>(numberOfTasks);
            var ran = new Random();
            for(int i=0; i<numberOfTasks;i++)
            {
                list.Add(ran.Next(minimum, maximum));
            }
            return list.ToArray();
        }
    }
}
