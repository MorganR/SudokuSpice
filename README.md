# SudokuSpice

An efficient sudoku solving and generating library targeting .NET Core.

This code focuses on optimizing performance without sacrificing readability. Generally speaking,
when faced with readability and flexibility improvements that have a slight performance cost, the
version with improved readability has been implemented.

However that's not to say it performs poorly! See [the numbers](#Performance).

This library uses dependency injection and interfaces to enable users to easily extend its
behavior, such as by adding new heuristics, or by introducing modified rules.

## Quick start

If you need to solve a standard Sudoku puzzle, you can simply create a `Puzzle`, create a `Solver`,
and solve it.

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

## Performance

All benchmarks were run using [BenchmarkDotNet](https://benchmarkdotnet.org/articles/overview.html).

### Comparison with other libraries

The performance of this library has been compared to other .NET sudoku libraries.

**Compared libraries:**

* [SudokuSharp](https://github.com/BenjaminChambers/SudokuSharp)
* [SudokuSolverLite](https://github.com/zhiliangxu/SudokuSolver)

#### Puzzle solving performance

These were compared using a set of 1750 generated puzzles, with 24 - 30 preset squares, grouped
by the number of squares that needed to be guessed to solve the puzzle if using just the basic
Sudoku constraints and no heuristics. *SudokuSpice* demonstrates considerably more speed and
stability regardless of the number of guesses.

|           Method | sampleCollection |         Mean |        Error |        StdDev |       Median |  Ratio | RatioSD |
|----------------- |----------------- |-------------:|-------------:|--------------:|-------------:|-------:|--------:|
|      SudokuSpice |       Guesses: 0 |     25.71 us |     0.049 us |      0.046 us |     25.71 us |   1.00 |    0.00 |
|      SudokuSharp |       Guesses: 0 |    770.22 us |    24.968 us |     73.618 us |    765.65 us |  30.13 |    2.46 |
| SudokuSolverLite |       Guesses: 0 |  1,296.67 us |    25.895 us |     74.298 us |  1,290.83 us |  51.69 |    2.94 |
|                  |                  |              |              |               |              |        |         |
|      SudokuSpice |       Guesses: 1 |     39.79 us |     0.191 us |      0.179 us |     39.83 us |   1.00 |    0.00 |
|      SudokuSharp |       Guesses: 1 |  1,573.98 us |    58.151 us |    170.546 us |  1,573.62 us |  39.57 |    4.67 |
| SudokuSolverLite |       Guesses: 1 |  4,365.30 us |   527.936 us |  1,540.015 us |  4,390.89 us | 109.22 |   35.06 |
|                  |                  |              |              |               |              |        |         |
|      SudokuSpice |     Guesses: 2-3 |     52.52 us |     0.231 us |      0.216 us |     52.48 us |   1.00 |    0.00 |
|      SudokuSharp |     Guesses: 2-3 |  2,495.84 us |   144.865 us |    424.864 us |  2,433.53 us |  51.12 |    8.86 |
| SudokuSolverLite |     Guesses: 2-3 |  4,193.62 us |   288.533 us |    846.216 us |  4,002.77 us |  74.87 |   10.98 |
|                  |                  |              |              |               |              |        |         |
|      SudokuSpice |     Guesses: 4-7 |     71.20 us |     0.637 us |      0.596 us |     71.33 us |   1.00 |    0.00 |
|      SudokuSharp |     Guesses: 4-7 |  3,477.92 us |   343.273 us |    995.898 us |  3,390.79 us |  51.32 |   17.61 |
| SudokuSolverLite |     Guesses: 4-7 | 10,087.58 us | 2,285.798 us |  6,484.428 us |  7,885.69 us | 159.55 |   90.84 |
|                  |                  |              |              |               |              |        |         |
|      SudokuSpice |      Guesses: 8+ |     93.24 us |     0.748 us |      0.700 us |     93.25 us |   1.00 |    0.00 |
|      SudokuSharp |      Guesses: 8+ |  5,815.83 us |   443.409 us |  1,300.442 us |  5,505.04 us |  59.00 |   15.57 |
| SudokuSolverLite |      Guesses: 8+ | 23,609.31 us | 5,532.881 us | 16,139.673 us | 19,012.11 us | 233.74 |  171.90 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to very
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|
|                SudokuSpice |   Easy |      11.426 us |     0.0205 us |     0.0192 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      17.442 us |     0.0243 us |     0.0216 us |     1.53 |    0.00 |
| SudokuSpiceDynamicMultiple |   Easy |      22.733 us |     0.0324 us |     0.0287 us |     1.99 |    0.01 |
|                SudokuSharp |   Easy |       6.709 us |     0.0080 us |     0.0075 us |     0.59 |    0.00 |
|           SudokuSolverLite |   Easy |     162.775 us |     0.1747 us |     0.1548 us |    14.25 |    0.03 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |      84.489 us |     0.3355 us |     0.2974 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |     115.826 us |     0.4844 us |     0.4531 us |     1.37 |    0.01 |
| SudokuSpiceDynamicMultiple | Medium |     139.242 us |     0.8259 us |     0.7726 us |     1.65 |    0.01 |
|                SudokuSharp | Medium |   3,020.298 us |    15.3435 us |    14.3523 us |    35.74 |    0.19 |
|           SudokuSolverLite | Medium |   2,392.081 us |     2.6646 us |     2.3621 us |    28.31 |    0.10 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |      65.868 us |     0.1798 us |     0.1501 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |      91.909 us |     0.2541 us |     0.2253 us |     1.40 |    0.00 |
| SudokuSpiceDynamicMultiple |  HardA |     111.638 us |     0.2529 us |     0.2366 us |     1.70 |    0.01 |
|                SudokuSharp |  HardA |   3,304.073 us |    64.8753 us |   106.5920 us |    50.13 |    1.94 |
|           SudokuSolverLite |  HardA |  25,194.573 us |    25.7743 us |    24.1093 us |   382.49 |    0.84 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |     186.380 us |     1.0982 us |     1.0273 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     237.525 us |     0.9396 us |     0.8329 us |     1.27 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardB |     281.802 us |     2.0544 us |     1.9217 us |     1.51 |    0.02 |
|                SudokuSharp |  HardB |  22,955.948 us |   822.3601 us | 2,424.7473 us |   126.08 |   13.91 |
|           SudokuSolverLite |  HardB |   4,965.574 us |     7.4837 us |     7.0003 us |    26.64 |    0.16 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |     115.756 us |     0.1491 us |     0.1322 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     148.939 us |     0.3319 us |     0.3105 us |     1.29 |    0.00 |
| SudokuSpiceDynamicMultiple |  EvilA |     170.742 us |     0.5440 us |     0.5088 us |     1.48 |    0.00 |
|                SudokuSharp |  EvilA |  42,700.955 us | 2,513.2044 us | 7,410.2400 us |   364.19 |   36.87 |
|           SudokuSolverLite |  EvilA | 406,736.398 us |   891.2734 us |   833.6977 us | 3,513.35 |    7.77 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |   1,176.205 us |    23.2618 us |    22.8462 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   1,689.260 us |    33.0516 us |    36.7367 us |     1.44 |    0.05 |
| SudokuSpiceDynamicMultiple |  EvilB |   1,974.574 us |    38.7827 us |    56.8472 us |     1.69 |    0.05 |
|                SudokuSharp |  EvilB |  46,650.746 us |   904.2877 us | 1,296.9024 us |    39.67 |    1.54 |
|           SudokuSolverLite |  EvilB |  51,852.796 us |   137.4782 us |   121.8709 us |    44.16 |    0.93 |

#### Puzzle generation performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|             Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|------------------- |----------:|----------:|----------:|------:|--------:|
|        SudokuSpice |  2.013 ms | 0.0448 ms | 0.0831 ms |  1.00 |    0.00 |
| SudokuSharpSingles | 14.070 ms | 0.5364 ms | 1.5477 ms |  6.96 |    0.90 |
|   SudokuSharpMixed |  7.111 ms | 0.2062 ms | 0.5948 ms |  3.54 |    0.38 |
