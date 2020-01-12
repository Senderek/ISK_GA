using System;
using System.Collections.Generic;
using System.Text;

namespace ISK.GA
{
    public class Processor
    {
        public Processor(int i)
        {
            this.Id = i;
        }

        public int Id { get; private set; }
    }
}
