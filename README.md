# SudokuSpice

An efficient sudoku solving and generating library targeting .NET Core.

This code focuses on optimizing performance without sacrificing readability. Generally speaking,
when faced with readability and flexibility improvements that have a slight performance cost, the
version with improved readability has been implemented.

However that's not to say it performs poorly! See [the numbers](#Performance).

This library uses dependency injection and interfaces to enable users to easily extend its
behavior, such as by adding new heuristics, or by introducing modified rules.

## Quick start

If you need to solve a standard Sudoku puzzle, you can simply create a Puzzle, create a Solver,
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
|      SudokuSpice |       Guesses: 0 |     25.00 us |     0.085 us |      0.071 us |     25.00 us |   1.00 |    0.00 |
|      SudokuSharp |       Guesses: 0 |    709.00 us |    27.013 us |     79.647 us |    701.11 us |  27.43 |    2.66 |
| SudokuSolverLite |       Guesses: 0 |  1,300.27 us |    27.633 us |     81.475 us |  1,290.03 us |  52.21 |    3.29 |
|                  |                  |              |              |               |              |        |         |
|      SudokuSpice |       Guesses: 1 |     38.71 us |     0.148 us |      0.138 us |     38.67 us |   1.00 |    0.00 |
|      SudokuSharp |       Guesses: 1 |  1,481.86 us |    56.123 us |    165.479 us |  1,488.57 us |  38.29 |    4.89 |
| SudokuSolverLite |       Guesses: 1 |  4,294.37 us |   479.297 us |  1,390.527 us |  4,286.78 us | 105.17 |   27.35 |
|                  |                  |              |              |               |              |        |         |
|      SudokuSpice |     Guesses: 2-3 |     50.80 us |     0.368 us |      0.344 us |     50.80 us |   1.00 |    0.00 |
|      SudokuSharp |     Guesses: 2-3 |  2,454.97 us |   206.323 us |    608.348 us |  2,398.61 us |  49.75 |    9.34 |
| SudokuSolverLite |     Guesses: 2-3 |  4,180.30 us |   398.569 us |  1,162.644 us |  3,845.99 us |  79.28 |   24.93 |
|                  |                  |              |              |               |              |        |         |
|      SudokuSpice |     Guesses: 4-7 |     70.26 us |     0.377 us |      0.353 us |     70.13 us |   1.00 |    0.00 |
|      SudokuSharp |     Guesses: 4-7 |  3,296.34 us |   289.213 us |    852.750 us |  3,252.51 us |  52.05 |    9.55 |
| SudokuSolverLite |     Guesses: 4-7 | 10,084.97 us | 1,272.262 us |  3,731.324 us |  9,882.79 us | 136.87 |   52.80 |
|                  |                  |              |              |               |              |        |         |
|      SudokuSpice |      Guesses: 8+ |     90.66 us |     0.983 us |      0.920 us |     90.67 us |   1.00 |    0.00 |
|      SudokuSharp |      Guesses: 8+ |  5,327.57 us |   254.311 us |    749.842 us |  5,226.59 us |  56.97 |    7.24 |
| SudokuSolverLite |      Guesses: 8+ | 22,250.35 us | 4,609.538 us | 13,518.978 us | 19,216.59 us | 205.14 |  130.03 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to very
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|
|                SudokuSpice |   Easy |      10.800 us |     0.0617 us |     0.0547 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      17.485 us |     0.0788 us |     0.0698 us |     1.62 |    0.01 |
| SudokuSpiceDynamicMultiple |   Easy |      22.058 us |     0.1055 us |     0.0935 us |     2.04 |    0.01 |
|                SudokuSharp |   Easy |       6.463 us |     0.0132 us |     0.0123 us |     0.60 |    0.00 |
|           SudokuSolverLite |   Easy |     152.962 us |     1.0108 us |     0.9455 us |    14.15 |    0.10 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |      80.101 us |     0.2304 us |     0.1924 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |     112.880 us |     0.5757 us |     0.4808 us |     1.41 |    0.01 |
| SudokuSpiceDynamicMultiple | Medium |     132.124 us |     0.5214 us |     0.4877 us |     1.65 |    0.01 |
|                SudokuSharp | Medium |   3,096.861 us |    17.9954 us |    15.9525 us |    38.64 |    0.22 |
|           SudokuSolverLite | Medium |   2,322.156 us |     6.2815 us |     5.8758 us |    29.00 |    0.09 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |      62.694 us |     0.2599 us |     0.2432 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |      90.553 us |     0.2985 us |     0.2792 us |     1.44 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardA |     105.159 us |     0.3129 us |     0.2927 us |     1.68 |    0.01 |
|                SudokuSharp |  HardA |   3,337.659 us |    64.5223 us |    83.8972 us |    52.92 |    1.44 |
|           SudokuSolverLite |  HardA |  24,299.346 us |    48.1113 us |    42.6494 us |   387.66 |    1.41 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |     182.995 us |     1.0175 us |     0.9518 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     229.035 us |     0.6892 us |     0.6446 us |     1.25 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardB |     275.652 us |     1.6837 us |     1.4926 us |     1.51 |    0.01 |
|                SudokuSharp |  HardB |  24,130.713 us |   773.6002 us | 2,268.8353 us |   127.55 |   14.89 |
|           SudokuSolverLite |  HardB |   4,709.932 us |    24.1520 us |    20.1680 us |    25.72 |    0.20 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |     114.183 us |     0.3430 us |     0.3208 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     145.827 us |     0.9058 us |     0.8030 us |     1.28 |    0.01 |
| SudokuSpiceDynamicMultiple |  EvilA |     161.287 us |     0.5366 us |     0.5019 us |     1.41 |    0.01 |
|                SudokuSharp |  EvilA |  44,912.049 us | 2,827.0948 us | 8,335.7527 us |   382.90 |   46.80 |
|           SudokuSolverLite |  EvilA | 387,139.827 us | 1,692.3841 us | 1,583.0571 us | 3,390.54 |   17.20 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |   1,146.786 us |    22.7274 us |    27.9113 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   1,562.816 us |    31.1789 us |    29.1647 us |     1.36 |    0.05 |
| SudokuSpiceDynamicMultiple |  EvilB |   1,886.061 us |    37.3895 us |    63.4901 us |     1.65 |    0.09 |
|                SudokuSharp |  EvilB |  46,393.831 us |   645.1314 us |   603.4563 us |    40.38 |    1.02 |
|           SudokuSolverLite |  EvilB |  49,447.768 us |   225.8081 us |   211.2210 us |    43.04 |    0.97 |

#### Puzzle generation performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|             Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|------------------- |----------:|----------:|----------:|------:|--------:|
|        SudokuSpice |  1.676 ms | 0.0147 ms | 0.0137 ms |  1.00 |    0.00 |
| SudokuSharpSingles | 13.467 ms | 0.5075 ms | 1.4964 ms |  8.59 |    0.97 |
|   SudokuSharpMixed |  6.702 ms | 0.2146 ms | 0.6327 ms |  4.02 |    0.43 |
