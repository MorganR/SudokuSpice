using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;

namespace SudokuSpice.Benchmark
{
    class MultithreadedJobAttribute : JobConfigBaseAttribute
    {
        public MultithreadedJobAttribute() : base(CreateJob()) { }

        private static Job CreateJob()
        {
            var job = new Job("MultithreadedJob");
            job.Environment.Affinity = new IntPtr((1 << Environment.ProcessorCount) - 1);
            return job.Freeze();
        }
    }
}
