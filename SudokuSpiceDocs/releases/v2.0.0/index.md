# 2.0.0 - 2020-05-12

*  Adds ConstraintBasedSolver and ConstraintBasedGenerator.
*  Replaces the parallel implementation of 'Generate' and 'GetStatsForAllSolutions' with a
   synchronous implementation. Having benchmarked various approaches, it turns out the overhead of
   running work in parallel is always more expensive than running the whole thing synchronously.