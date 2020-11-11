# Releases

Select a release from below:

## [2.0.0](/releases/v2.0.0/) - 2020-05-12

*  Adds ConstraintBasedSolver and ConstraintBasedGenerator.
*  Replaces the parallel implementation of 'Generate' and 'GetStatsForAllSolutions' with a
   synchronous implementation. Having benchmarked various approaches, it turns out the overhead of
   running work in parallel is always more expensive than running the whole thing synchronously.

## 1.1.0 - 2020-04-19 (not documented)

*  Adds .NET Standard 2.1 support
