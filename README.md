# SudokuSpice

**Docs:** https://morganr.github.io/SudokuSpice

An efficient sudoku solving and generating library targeting .NET Core.

This code focuses on optimizing performance without sacrificing readability. Generally speaking,
when faced with readability and flexibility improvements that have a slight performance cost, the
version with improved readability has been implemented.

However that's not to say it performs poorly! See
[the numbers](https://morganr.github.io/SudokuSpice/articles/performance.html).

This library uses dependency injection and interfaces to enable users to easily extend its
behavior, such as by adding new heuristics, or by introducing modified rules.
