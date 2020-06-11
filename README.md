# SudokuSpice

**Docs:** https://sudokuspice.dev

An efficient sudoku solving and generating library targeting .NET Core and .NET Standard.

This code focuses on optimizing performance without sacrificing readability. Generally speaking,
when faced with readability and flexibility improvements that have a slight performance cost, the
version with improved readability has been implemented.

However that's not to say it performs poorly! See
[the numbers](https://morganr.github.io/SudokuSpice/articles/performance.html).

This library uses dependency injection and interfaces to enable users to easily extend its
behavior, such as by adding new heuristics, or by introducing modified rules.

## Releases

### 2.0.0 (beta) - 2020-05-12

*  Adds ConstraintBasedSolver and ConstraintBasedGenerator.
*  Replaces the parallel implementation of 'Generate' and 'GetStatsForAllSolutions' with a
   synchronous implementation. Having benchmarked various approaches, it turns out the overhead of
   running work in parallel is always more expensive than running the whole thing synchronously.

### 1.1.0 - 2020-04-19

*  Adds .NET Standard 2.1 support
