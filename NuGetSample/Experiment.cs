using NuGetSample;
using System.Collections.Generic;

namespace NuGetParallelWnd
{
    public class Experiment
    {
        public string Name { get; set; }
        public List<Solution> Population { get; set; }
        public int Generation { get; set; }
        public int MaxGenerations { get; set; }
    }
}
