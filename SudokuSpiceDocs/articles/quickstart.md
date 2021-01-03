# Quick Start

For more information, see the [Framework Overview](framework.md).

## Solving

If you need to solve a standard Sudoku puzzle, you can simply create a
[`PuzzleWithPossibleValues`](xref:SudokuSpice.RuleBased.PuzzleWithPossibleValues), create a
[`PuzzleSolver`](xref:SudokuSpice.RuleBased.PuzzleSolver`1), and solve it.

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
        new List<IRule> {
            // Custom rules here
        }
    ));
```

## Generating

If you need to generate Sudoku puzzles, simply create a
[`StandardPuzzleGenerator`](xref:SudokuSpice.RuleBased.StandardPuzzleGenerator)
and call `Generate` as many times as you like.

```csharp
// Create a 9x9 puzzle generator.
var generator = new StandardPuzzleGenerator();
// Generate a 9x9 puzzle with 30 preset square values.
var puzzle = generator.Generate(/*puzzleSize=*/9, /*numSetSquares=*/30, /*timeout=*/TimeSpan.FromSeconds(1));

// For custom solvers:
var customGenerator = new PuzzleGenerator<MyPuzzle>(
    puzzleSize => new MyPuzzle(puzzleSize),
    new MyCustomSolver());
// Use TimeSpan.Zero to disable the timeout.
var puzzle = generator.Generate(puzzleSize, numSetSquares, TimeSpan.Zero);
```

**Warning:** Requesting a small number of set squares when calling `Generate` can take a very long time,
and may eventually time out. For quickly generating 9x9 puzzles, it's recommended to use a value of
23 or greater.

## Constraint-based

*SudokuSpice* also provides a constraint-based solver that can be better in some scenarios (see
[benchmarks](performance.md)). This can be used similarly: create a
[`Puzzle`](xref:SudokuSpice.Puzzle), create a
[`ConstraintBased.PuzzleSolver`](xref:SudokuSpice.ConstraintBased.PuzzleSolver`1), and solve.

```csharp
var puzzle = new Puzzle(...);

// For standard puzzles:
var solver = ConstraintBased.StandardPuzzles.CreateSolver();
var solved = solver.Solve(puzzle);

// With custom constraints
var customSolver = new ConstraintBased.PuzzleSolver<Puzzle>(
    new List<IConstraint> {
        // Custom constraints go here
    });
```
