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
stability regardless of the number of guesses, performing anywhere from 25 - 199 times faster
than the competitors.

|           Method | sampleCollection |         Mean |        Error |        StdDev |       Median |  Ratio | RatioSD |
|----------------- |----------------- |-------------:|-------------:|--------------:|-------------:|-------:|--------:|
|      SudokuSpice |       Guesses: 0 |     27.07 us |     0.056 us |      0.049 us |     27.07 us |   1.00 |    0.00 |
|      SudokuSharp |       Guesses: 0 |    688.11 us |    23.489 us |     67.394 us |    681.73 us |  26.57 |    2.87 |
| SudokuSolverLite |       Guesses: 0 |  1,300.32 us |    29.947 us |     87.830 us |  1,301.06 us |  48.39 |    3.61 |
|                  |                  |              |              |               |              |        |         |
|      SudokuSpice |       Guesses: 1 |     41.29 us |     0.199 us |      0.177 us |     41.26 us |   1.00 |    0.00 |
|      SudokuSharp |       Guesses: 1 |  1,442.09 us |    57.694 us |    166.459 us |  1,431.03 us |  34.83 |    3.61 |
| SudokuSolverLite |       Guesses: 1 |  4,122.75 us |   556.281 us |  1,622.698 us |  3,749.92 us | 100.25 |   32.97 |
|                  |                  |              |              |               |              |        |         |
|      SudokuSpice |     Guesses: 2-3 |     54.21 us |     0.283 us |      0.264 us |     54.18 us |   1.00 |    0.00 |
|      SudokuSharp |     Guesses: 2-3 |  2,489.81 us |   195.404 us |    573.086 us |  2,395.93 us |  46.70 |    7.06 |
| SudokuSolverLite |     Guesses: 2-3 |  3,751.72 us |   296.050 us |    844.646 us |  3,587.26 us |  71.27 |   17.99 |
|                  |                  |              |              |               |              |        |         |
|      SudokuSpice |     Guesses: 4-7 |     72.82 us |     0.478 us |      0.424 us |     72.92 us |   1.00 |    0.00 |
|      SudokuSharp |     Guesses: 4-7 |  3,300.24 us |   299.908 us |    850.790 us |  3,246.29 us |  46.64 |   11.19 |
| SudokuSolverLite |     Guesses: 4-7 |  9,620.54 us | 1,248.111 us |  3,581.063 us |  8,516.25 us | 121.73 |   43.24 |
|                  |                  |              |              |               |              |        |         |
|      SudokuSpice |      Guesses: 8+ |     93.79 us |     1.104 us |      1.032 us |     93.80 us |   1.00 |    0.00 |
|      SudokuSharp |      Guesses: 8+ |  5,359.31 us |   404.401 us |  1,192.387 us |  5,265.53 us |  51.93 |   13.21 |
| SudokuSolverLite |      Guesses: 8+ | 17,494.69 us | 3,710.118 us | 10,763.726 us | 15,306.29 us | 185.85 |  114.48 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to very
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|
|                SudokuSpice |   Easy |      12.241 us |     0.0699 us |     0.0584 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      17.682 us |     0.0857 us |     0.0801 us |     1.45 |    0.01 |
| SudokuSpiceDynamicMultiple |   Easy |      23.383 us |     0.0891 us |     0.0834 us |     1.91 |    0.01 |
|                SudokuSharp |   Easy |       6.445 us |     0.0189 us |     0.0177 us |     0.53 |    0.00 |
|           SudokuSolverLite |   Easy |     151.161 us |     0.7009 us |     0.6214 us |    12.35 |    0.08 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |      83.181 us |     0.4363 us |     0.3868 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |     113.542 us |     0.6884 us |     0.6103 us |     1.37 |    0.01 |
| SudokuSpiceDynamicMultiple | Medium |     139.514 us |     0.5547 us |     0.4917 us |     1.68 |    0.01 |
|                SudokuSharp | Medium |   3,085.895 us |    20.0697 us |    18.7733 us |    37.08 |    0.27 |
|           SudokuSolverLite | Medium |   2,271.038 us |     9.6696 us |     9.0450 us |    27.32 |    0.17 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |      65.638 us |     0.6956 us |     0.5809 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |      89.893 us |     0.3974 us |     0.3717 us |     1.37 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardA |     108.276 us |     0.4426 us |     0.4140 us |     1.65 |    0.01 |
|                SudokuSharp |  HardA |   3,403.981 us |    65.5058 us |    87.4484 us |    51.92 |    1.70 |
|           SudokuSolverLite |  HardA |  24,268.766 us |    87.6632 us |    82.0002 us |   369.99 |    3.49 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |     185.614 us |     1.4341 us |     1.3414 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     236.502 us |     1.4611 us |     1.3667 us |     1.27 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardB |     275.781 us |     1.3934 us |     1.2353 us |     1.49 |    0.01 |
|                SudokuSharp |  HardB |  23,505.518 us |   863.4225 us | 2,545.8207 us |   120.91 |   12.65 |
|           SudokuSolverLite |  HardB |   4,772.955 us |    21.8253 us |    18.2251 us |    25.73 |    0.20 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |     114.067 us |     0.3034 us |     0.2838 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     144.105 us |     0.6960 us |     0.6170 us |     1.26 |    0.01 |
| SudokuSpiceDynamicMultiple |  EvilA |     164.121 us |     0.4804 us |     0.4494 us |     1.44 |    0.01 |
|                SudokuSharp |  EvilA |  43,299.694 us | 2,618.8570 us | 7,639.3289 us |   360.48 |   58.09 |
|           SudokuSolverLite |  EvilA | 390,516.679 us | 2,023.0951 us | 1,793.4211 us | 3,423.01 |   14.19 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |   1,201.947 us |    23.3610 us |    30.3759 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   1,585.761 us |    31.3820 us |    36.1395 us |     1.32 |    0.04 |
| SudokuSpiceDynamicMultiple |  EvilB |   1,931.311 us |    37.5828 us |    57.3930 us |     1.61 |    0.07 |
|                SudokuSharp |  EvilB |  48,075.114 us |   630.8894 us |   559.2670 us |    39.96 |    1.19 |
|           SudokuSolverLite |  EvilB |  50,967.612 us |   217.6399 us |   192.9321 us |    42.36 |    1.25 |



#### Puzzle generation performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|             Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|------------------- |----------:|----------:|----------:|------:|--------:|
|        SudokuSpice |  1.959 ms | 0.0217 ms | 0.0203 ms |  1.00 |    0.00 |
| SudokuSharpSingles | 13.421 ms | 0.5775 ms | 1.6936 ms |  6.91 |    1.03 |
|   SudokuSharpMixed |  6.775 ms | 0.1951 ms | 0.5660 ms |  3.55 |    0.27 |