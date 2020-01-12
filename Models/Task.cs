using System;
using System.Collections.Generic;
using System.Text;

namespace ISK.GA
{
    public class Task
    {
        public Task(int i)
        {
            Id = i;
        }

        public int Id { get; set; }
        public List<int> ProcessorCosts { get; set; }

        private string ProcessorCostsToString()
        {
            var stringBuilder = new StringBuilder("[");
            foreach(var c in ProcessorCosts)
            {
                stringBuilder.Append(" ");
                stringBuilder.Append(c);
                
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            return $"{Id}: {ProcessorCostsToString()}";
        }
    }
}
