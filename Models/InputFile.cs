using System.Collections.Generic;

namespace ISK.GA
{
    public class InputFile
    {
        public int m_numberOfTaks { get; set; }
        public int m_numberOfProcessors { get; set; }
        public List<Task> tasks { get; set; } = new List<Task>();
    }
}
