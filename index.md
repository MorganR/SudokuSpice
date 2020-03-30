# SudokuSpice

SudokuSpice is an efficient sudoku solving and generating library targeting .NET Core.

This library focuses on optimizing performance without sacrificing readability. Generally speaking,
when faced with readability and flexibility improvements that have a slight performance cost, the
version with improved readability has been implemented.

However that's not to say it performs poorly! See [the numbers](articles/performance.md).

This library uses dependency injection and interfaces to enable users to easily extend its
behavior, such as by adding new heuristics, or by introducing modified rules.
