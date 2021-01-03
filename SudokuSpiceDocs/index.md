# SudokuSpice

SudokuSpice is a [highly efficient](articles/performance.md) sudoku solving and generating library
for .NET.

In addition to working with standard Sudoku puzzles, *SudokuSpice* focuses on ease-of-use and
customizability. Want to solve or generate a Sudoku puzzle that also enforces unique values on
the diagonals? Just add the
[`DiagonalUniquenessRule`](xref:SudokuSpice.RuleBased.Rules.DiagonalUniquenessRule) to your solver.
Want to do something more complicated, like to enforce unique values within arbitrary regions? Key
parts of the framework are exposed as interfaces so you can solve and generate truely custom
puzzles. In this example, maybe you need a custom implementation of
[`IPuzzle`](xref:SudokuSpice.IPuzzle) as well as a custom
[`IRule`](xref:SudokuSpice.RuleBased.Rules.IRule) or
[`IConstraint`](xref:SudokuSpice.ConstraintBased.Constraints.IConstraint). See the
[framework overview](articles/framework.md) for more details.

Docs:

*  [Quick Start](articles/quickstart.md)
*  [Benchmarks](articles/performance.md)
*  [Framework Overview](articles/framework.md)

