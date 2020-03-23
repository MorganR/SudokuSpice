# SudokuSpice

An efficient sudoku-solving library targeting .NET Core.

This code focuses on optimizing performance without sacrificing readability. Generally speaking,
when faced with readability and flexibility improvements that have a slight performance cost, the
version with improved readability has been implemented.

However that's not to say it performs poorly! See [the numbers](#Performance).

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
stability regardless of the number of guesses, performing anywhere from 26 - 255 times faster
than the competitors.

|           Method | sampleCollection |         Mean |        Error |        StdDev |  Ratio | RatioSD |
|----------------- |----------------- |-------------:|-------------:|--------------:|-------:|--------:|
|      SudokuSpice |       Guesses: 0 |     26.46 us |     0.089 us |      0.079 us |   1.00 |    0.00 |
|      SudokuSharp |       Guesses: 0 |    712.46 us |    27.493 us |     81.064 us |  26.92 |    3.42 |
| SudokuSolverLite |       Guesses: 0 |  1,237.60 us |    24.718 us |     60.633 us |  46.69 |    2.74 |
|                  |                  |              |              |               |        |         |
|      SudokuSpice |       Guesses: 1 |     39.19 us |     0.165 us |      0.154 us |   1.00 |    0.00 |
|      SudokuSharp |       Guesses: 1 |  1,445.58 us |    58.197 us |    170.680 us |  36.48 |    4.28 |
| SudokuSolverLite |       Guesses: 1 |  4,202.62 us |   396.089 us |  1,167.877 us | 110.30 |   24.85 |
|                  |                  |              |              |               |        |         |
|      SudokuSpice |     Guesses: 2-3 |     51.82 us |     0.397 us |      0.372 us |   1.00 |    0.00 |
|      SudokuSharp |     Guesses: 2-3 |  2,358.47 us |   193.179 us |    566.561 us |  41.58 |    9.58 |
| SudokuSolverLite |     Guesses: 2-3 |  3,941.03 us |   327.397 us |    949.838 us |  73.08 |   13.98 |
|                  |                  |              |              |               |        |         |
|      SudokuSpice |     Guesses: 4-7 |     69.18 us |     0.672 us |      0.628 us |   1.00 |    0.00 |
|      SudokuSharp |     Guesses: 4-7 |  3,370.77 us |   340.695 us |    993.825 us |  50.78 |   12.78 |
| SudokuSolverLite |     Guesses: 4-7 |  9,625.57 us | 1,179.848 us |  3,478.808 us | 125.39 |   46.06 |
|                  |                  |              |              |               |        |         |
|      SudokuSpice |      Guesses: 8+ |     89.26 us |     0.683 us |      0.639 us |   1.00 |    0.00 |
|      SudokuSharp |      Guesses: 8+ |  5,127.41 us |   380.070 us |  1,114.681 us |  55.54 |   16.16 |
| SudokuSolverLite |      Guesses: 8+ | 21,832.41 us | 3,820.255 us | 11,204.147 us | 255.89 |  118.96 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to very
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|
|                SudokuSpice |   Easy |      10.832 us |     0.0309 us |     0.0274 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      16.377 us |     0.0987 us |     0.0875 us |     1.51 |    0.01 |
| SudokuSpiceDynamicMultiple |   Easy |      20.588 us |     0.0899 us |     0.0841 us |     1.90 |    0.01 |
|                SudokuSharp |   Easy |       6.442 us |     0.0262 us |     0.0245 us |     0.59 |    0.00 |
|           SudokuSolverLite |   Easy |     155.094 us |     1.3873 us |     1.1584 us |    14.32 |    0.10 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |      76.601 us |     0.3685 us |     0.3447 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |     105.848 us |     0.3278 us |     0.2906 us |     1.38 |    0.01 |
| SudokuSpiceDynamicMultiple | Medium |     124.972 us |     0.7513 us |     0.7027 us |     1.63 |    0.01 |
|                SudokuSharp | Medium |   3,099.666 us |    27.2411 us |    25.4813 us |    40.47 |    0.34 |
|           SudokuSolverLite | Medium |   2,328.245 us |    12.2567 us |    10.2349 us |    30.38 |    0.20 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |      60.340 us |     0.5748 us |     0.5095 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |      82.833 us |     0.5454 us |     0.4835 us |     1.37 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardA |     102.150 us |     0.4983 us |     0.4661 us |     1.69 |    0.01 |
|                SudokuSharp |  HardA |   3,348.357 us |    66.3592 us |   138.5164 us |    55.59 |    2.50 |
|           SudokuSolverLite |  HardA |  24,959.000 us |   247.6776 us |   206.8220 us |   413.59 |    5.26 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |     176.056 us |     1.1634 us |     1.0313 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     228.175 us |     1.4647 us |     1.3701 us |     1.30 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardB |     267.549 us |     3.3089 us |     2.9332 us |     1.52 |    0.02 |
|                SudokuSharp |  HardB |  23,531.357 us |   869.4036 us | 2,536.0911 us |   132.70 |   14.94 |
|           SudokuSolverLite |  HardB |   4,691.087 us |    15.4921 us |    14.4913 us |    26.65 |    0.15 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |     105.521 us |     0.6640 us |     0.6212 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     132.808 us |     0.8991 us |     0.8410 us |     1.26 |    0.01 |
| SudokuSpiceDynamicMultiple |  EvilA |     153.232 us |     0.4902 us |     0.4585 us |     1.45 |    0.01 |
|                SudokuSharp |  EvilA |  42,200.030 us | 2,514.4545 us | 7,374.4598 us |   422.35 |   70.67 |
|           SudokuSolverLite |  EvilA | 391,255.413 us | 3,853.1767 us | 3,604.2639 us | 3,707.97 |   42.10 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |   1,102.509 us |    21.0480 us |    19.6883 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   1,564.207 us |    30.9952 us |    39.1990 us |     1.42 |    0.04 |
| SudokuSpiceDynamicMultiple |  EvilB |   1,859.655 us |    26.9805 us |    25.2376 us |     1.69 |    0.04 |
|                SudokuSharp |  EvilB |  46,977.514 us |   896.4341 us | 1,100.9016 us |    42.82 |    1.06 |
|           SudokuSolverLite |  EvilB |  49,334.196 us |   259.4913 us |   242.7283 us |    44.76 |    0.79 |

#### Puzzle generation performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|             Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|------------------- |----------:|----------:|----------:|------:|--------:|
|        SudokuSpice |  2.023 ms | 0.0385 ms | 0.0378 ms |  1.00 |    0.00 |
| SudokuSharpSingles | 13.241 ms | 0.5405 ms | 1.5681 ms |  6.58 |    0.40 |
|   SudokuSharpMixed |  6.991 ms | 0.2217 ms | 0.6432 ms |  3.60 |    0.34 |