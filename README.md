# SudokuSpice

![Build and Test](https://github.com/MorganR/SudokuSpice/workflows/Build%20and%20Test/badge.svg)

**Docs:** https://www.sudokuspice.dev

SudokuSpice is a [highly efficient](https://www.sudokuspice.dev/articles/performance.html) sudoku
solving and generating library for .NET.

In addition to working with standard Sudoku puzzles, *SudokuSpice* focuses on ease-of-use and
customizability. Want to solve or generate a Sudoku puzzle that also enforces unique values on
the diagonals? Just add the `DiagonalUniquenessRule` to your solver.

Want to do something more complicated, like to enforce unique values within arbitrary regions? Key
parts of the framework are exposed as interfaces so you can solve and generate truely custom
puzzles. In this example, maybe you need a custom implementation of `IPuzzle` as well as a custom
`IRule` or `IConstraint`. See the
[framework overview](https://www.sudokuspice.dev/articles/framework.html) for more details.

## Quick start

### Solving

If you need to solve a standard Sudoku puzzle, you can simply create a
`PuzzleWithPossibleValues`, create a `PuzzleSolver`, and solve it.

```csharp
var puzzle = new PuzzleWithPossibleValues(new int?[,]
    {
        {null,    2, null,    6, null,    8, null, null, null},
        {   5,    8, null, null, null,    9,    7, null, null},
        {null, null, null, null,    4, null, null, null, null},
        {   3,    7, null, null, null, null,    5, null, null},
        {   6, null, null, null, null, null, null, null,    4},
        {null, null,    8, null, null, null, null,    1,    3},
        {null, null, null, null,    2, null, null, null, null},
        {null, null,    9,    8, null, null, null,    3,    6},
        {null, null, null,    3, null,    6, null,    9, null},
    });
var solver = RuleBased.StandardPuzzles.CreateSolver();
var solved = solver.Solve(puzzle);

// Values can be accessed individually like this:
int row = 0;
int column = 1;
var value = solved[row, column]; // Returns 2

// With custom rules
var customSolver = new RuleBased.PuzzleSolver<Puzzle>(
    new DynamicRuleKeeper(
        new IRule[] {
            // Custom rules here
        }
    ));
```

### Generating

If you need to generate Sudoku puzzles, simply create a `StandardPuzzleGenerator` and call
`Generate` as many times as you like.

```csharp
// Create a 9x9 puzzle generator.
var generator = new StandardPuzzleGenerator();
// Generate a 9x9 puzzle with 30 preset square values.
var puzzle = generator.Generate(
    /*puzzleSize=*/9, /*numSetSquares=*/30, /*timeout=*/TimeSpan.FromSeconds(1));

// For custom solvers:
var customGenerator = new PuzzleGenerator<MyPuzzle>(
    puzzleSize => new MyPuzzle(puzzleSize),
    new MyCustomSolver());
// Use TimeSpan.Zero to disable the timeout.
var puzzle = generator.Generate(puzzleSize, numSetSquares, TimeSpan.Zero);
```

**Warning:** Requesting a small number of set squares when calling `Generate` can take a very long
time, and may eventually time out. For quickly generating 9x9 puzzles, it's recommended to use a
value of 23 or greater.

### Constraint-based

*SudokuSpice* also provides a constraint-based solver that can be better in some scenarios. This
can be used similarly: create a `Puzzle`, create a `ConstraintBased.PuzzleSolver`, and solve.

```csharp
var puzzle = new Puzzle(...);

// For standard puzzles:
var solver = ConstraintBased.StandardPuzzles.CreateSolver();
var solved = solver.Solve(puzzle);

// With custom constraints
var customSolver = new ConstraintBased.PuzzleSolver<Puzzle>(
    new IConstraint[] {
        // Custom constraints go here
    });
```

See [the docs](https://www.sudokuspice.dev) for more information.

## Releases

### 3.2.X

#### 3.2.1 - 2021-09-19

* Fix bug with Puzzle.NumSquares when constructing puzzle from an array.

#### 3.2.0 - 2021-09-19

* Add rule-based support for multiples of a given value
  * Add CountPerUniqueValue to IReadOnlyPuzzle
  * Add MaxCountPer* rules 
  * Rename IReadOnlyPossibleValues.AllPossibleValues to UniquePossibleValues

### 3.1.0 - 2021-08-23

* Made PossibleValues public

### 3.0.0 - 2021-03-07

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

### 2.0.0 - 2020-05-12

*  Adds ConstraintBasedSolver and ConstraintBasedGenerator.
*  Replaces the parallel implementation of 'Generate' and 'GetStatsForAllSolutions' with a
   synchronous implementation. Having benchmarked various approaches, it turns out the overhead of
   running work in parallel is always more expensive than running the whole thing synchronously.

### 1.1.0 - 2020-04-19

*  Adds .NET Standard 2.1 support
