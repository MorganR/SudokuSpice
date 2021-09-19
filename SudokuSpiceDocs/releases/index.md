# Releases

Select a release from below:

## [3.2.1](/releases/v3.2.1/) - 2021-09-19

* Fix bug with Puzzle.NumSquares when constructing puzzle from an array.

## [3.2.0](/releases/v3.2.0/) - 2021-09-19

* Add rule-based support for multiples of a given value
  * Add CountPerUniqueValue to IReadOnlyPuzzle
  * Add MaxCountPer* rules 
  * Rename IReadOnlyPossibleValues.AllPossibleValues to UniquePossibleValues

## [3.1.0](/releases/v3.1.0/) - 2021-08-23

* Made PossibleValues public

## [3.0.0](/releases/v3.0.0/) - 2021-03-07

* Updated to .NET 5 (dropped support for older versions)
* The rule-based and constraint-based APIs have been unified as much as possible (note: many breaking changes).
* There is now a single PuzzleGenerator that works with any IPuzzleSolver implementation.
* Solvers and generators are now generic to reduce casting.
* Performance has been improved across the board.
* The constraint-based library has been simplified around the concepts of Objectives and Possibilities, and extended from an exact-cover matrix to a more expressive exact-cover graph. This enables more complex constraints.
* Added a rule and constraint for implementing magic squares.
* Rule-based code now uses a new puzzle type that combines IPuzzle and IPossibleValues: PuzzleWithPossibleValues!
* More tests.
* Loads of other little changes.

Warning: This release *does* introduce large-scale breaking changes.

## [2.0.0](/releases/v2.0.0/) - 2020-05-12

*  Adds ConstraintBasedSolver and ConstraintBasedGenerator.
*  Replaces the parallel implementation of 'Generate' and 'GetStatsForAllSolutions' with a
   synchronous implementation. Having benchmarked various approaches, it turns out the overhead of
   running work in parallel is always more expensive than running the whole thing synchronously.

## 1.1.0 - 2020-04-19 (not documented)

*  Adds .NET Standard 2.1 support
