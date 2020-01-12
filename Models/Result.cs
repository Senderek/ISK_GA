using System;
using System.Collections.Generic;
using System.Text;

namespace ISK.GA
{
    public class Result
    {
        public int Id { get; set; }
        public int ElapsedTime { get; set; }
        public int NumberOfGenerations { get; set; }
        public int BestResult { get; set; }
        public List<int> TasksResult { get; set; } = new List<int>();
        public override string ToString()
        {
            return $"{Id}: {BestResult} {ElapsedTime} {NumberOfGenerations}";
        }
    }

    public class IterationResult
    {
        public double Time { get; set; }
        public int Result { get; set; }
        public int GenerationNumber { get; set; }
    }
}
