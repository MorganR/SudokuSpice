# Quick Start

For more information, see the [Framework Overview](framework.md).

## Solving

If you need to solve a standard Sudoku puzzle, you can simply create a
[`Puzzle`](xref:SudokuSpice.Puzzle), create a [`Solver`](xref:SudokuSpice.RuleBased.Solver), and solve it.

```csharp
var puzzle = new Puzzle(new int?[,]
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
var solver = new Solver(puzzle);
solver.Solve();
// Puzzle is now solved!

// Values can be accessed individually like this:
int row = 0;
int column = 1;
var value = puzzle[row, column]; // Returns 2
```

## Generating

If you need to generate Sudoku puzzles, simply create a [`StandardPuzzleGenerator`](xref:SudokuSpice.RuleBased.StandardPuzzleGenerator)
and call `Generate` as many times as you like.

```csharp
// Create a 9x9 puzzle generator.
var generator = new StandardPuzzleGenerator(/*size=*/9);
// Generate a 9x9 puzzle with 30 preset square values.
var puzzle = generator.Generate(/*numSetSquares=*/30, /*timeout=*/TimeSpan.FromSeconds(1));
```

**Warning:** Requesting a small number of set squares when calling `Generate` can take a very long time,
and may eventually time out. For quickly generating 9x9 puzzles, it's recommended to use a value of
23 or greater.
