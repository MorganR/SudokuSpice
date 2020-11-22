# Performance

All benchmarks were run using [BenchmarkDotNet](https://benchmarkdotnet.org/articles/overview.html).

The performance of this library has been compared to other .NET sudoku libraries.

**Compared libraries:**

* [SudokuSharp](https://github.com/BenjaminChambers/SudokuSharp)
* [SudokuSolverLite](https://github.com/zhiliangxu/SudokuSolver)

## Puzzle solving performance

These were compared using a set of 1750 generated puzzles, with 24 - 30 preset squares, grouped
by the number of squares that needed to be guessed to solve the puzzle if using just the basic
Sudoku rules and no heuristics. *SudokuSpice* demonstrates considerably more speed and
stability regardless of the number of guesses. These benchmarks use the Core CLR on 64 bit Window
or Linux. Values are roughly consistent for each OS.

|                 Method | sampleCollection |         Mean |        Error |        StdDev |  Ratio | RatioSD |
|----------------------- |----------------- |-------------:|-------------:|--------------:|-------:|--------:|
|            SudokuSpice |       Guesses: 0 |     22.93 us |     0.046 us |      0.038 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |       Guesses: 0 |     64.88 us |     0.562 us |      0.525 us |   2.83 |    0.02 |
|            SudokuSharp |       Guesses: 0 |    682.59 us |    20.567 us |     60.320 us |  29.74 |    3.29 |
|       SudokuSolverLite |       Guesses: 0 |  1,095.11 us |    22.970 us |     67.004 us |  49.09 |    3.01 |
|                        |                  |              |              |               |        |         |
|            SudokuSpice |       Guesses: 1 |     37.58 us |     0.121 us |      0.108 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |       Guesses: 1 |     68.50 us |     0.653 us |      0.579 us |   1.82 |    0.01 |
|            SudokuSharp |       Guesses: 1 |  1,420.40 us |    54.080 us |    156.896 us |  38.64 |    4.13 |
|       SudokuSolverLite |       Guesses: 1 |  3,323.63 us |   265.882 us |    779.785 us |  92.91 |   16.57 |
|                        |                  |              |              |               |        |         |
|            SudokuSpice |     Guesses: 2-3 |     49.44 us |     0.386 us |      0.361 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |     Guesses: 2-3 |     71.93 us |     0.707 us |      0.662 us |   1.46 |    0.02 |
|            SudokuSharp |     Guesses: 2-3 |  2,328.28 us |   126.016 us |    369.583 us |  51.89 |    7.15 |
|       SudokuSolverLite |     Guesses: 2-3 |  3,473.50 us |   236.899 us |    691.046 us |  68.87 |   14.50 |
|                        |                  |              |              |               |        |         |
|            SudokuSpice |     Guesses: 4-7 |     67.75 us |     0.531 us |      0.497 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |     Guesses: 4-7 |     77.62 us |     0.699 us |      0.654 us |   1.15 |    0.01 |
|            SudokuSharp |     Guesses: 4-7 |  3,110.72 us |   225.424 us |    657.573 us |  45.36 |    9.26 |
|       SudokuSolverLite |     Guesses: 4-7 |  8,505.02 us | 1,372.313 us |  3,981.329 us | 132.72 |   72.83 |
|                        |                  |              |              |               |        |         |
|            SudokuSpice |      Guesses: 8+ |     86.28 us |     0.978 us |      0.915 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |      Guesses: 8+ |     83.52 us |     0.708 us |      0.663 us |   0.97 |    0.02 |
|            SudokuSharp |      Guesses: 8+ |  5,212.12 us |   403.991 us |  1,184.835 us |  60.15 |   17.61 |
|       SudokuSolverLite |      Guesses: 8+ | 16,995.32 us | 3,929.151 us | 11,273.469 us | 133.46 |   65.86 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|
|                SudokuSpice |   Easy |       9.020 us |     0.0602 us |     0.0563 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      15.142 us |     0.0683 us |     0.0605 us |     1.68 |    0.01 |
| SudokuSpiceDynamicMultiple |   Easy |      19.333 us |     0.0343 us |     0.0287 us |     2.14 |    0.01 |
|     SudokuSpiceConstraints |   Easy |      19.799 us |     0.0850 us |     0.0753 us |     2.19 |    0.01 |
|                SudokuSharp |   Easy |       6.465 us |     0.0189 us |     0.0168 us |     0.72 |    0.00 |
|           SudokuSolverLite |   Easy |     130.907 us |     0.4271 us |     0.3566 us |    14.50 |    0.09 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |      60.982 us |     0.3594 us |     0.3362 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |      89.200 us |     0.3517 us |     0.3290 us |     1.46 |    0.01 |
| SudokuSpiceDynamicMultiple | Medium |     108.687 us |     0.4165 us |     0.3896 us |     1.78 |    0.01 |
|     SudokuSpiceConstraints | Medium |      61.178 us |     0.5070 us |     0.4495 us |     1.00 |    0.01 |
|                SudokuSharp | Medium |   2,646.656 us |    21.8647 us |    20.4522 us |    43.40 |    0.43 |
|           SudokuSolverLite | Medium |   2,005.524 us |     7.9486 us |     6.6374 us |    32.91 |    0.23 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |      45.985 us |     0.1999 us |     0.1772 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |      69.574 us |     0.6987 us |     0.6194 us |     1.51 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardA |      86.537 us |     0.4220 us |     0.3947 us |     1.88 |    0.01 |
|     SudokuSpiceConstraints |  HardA |      67.417 us |     0.3942 us |     0.3495 us |     1.47 |    0.01 |
|                SudokuSharp |  HardA |   2,864.500 us |    57.2264 us |    92.4100 us |    62.27 |    2.22 |
|           SudokuSolverLite |  HardA |  21,204.425 us |    71.4961 us |    66.8775 us |   461.17 |    2.14 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |     152.223 us |     0.9090 us |     0.8503 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     197.472 us |     0.7385 us |     0.6908 us |     1.30 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardB |     236.125 us |     1.3259 us |     1.2403 us |     1.55 |    0.01 |
|     SudokuSpiceConstraints |  HardB |      76.047 us |     0.2739 us |     0.2562 us |     0.50 |    0.00 |
|                SudokuSharp |  HardB |  20,445.904 us |   721.9200 us | 2,094.4209 us |   133.54 |   14.07 |
|           SudokuSolverLite |  HardB |   4,046.514 us |    15.1464 us |    13.4269 us |    26.58 |    0.17 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |      84.465 us |     0.4078 us |     0.3815 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     114.434 us |     0.4251 us |     0.3769 us |     1.35 |    0.01 |
| SudokuSpiceDynamicMultiple |  EvilA |     129.775 us |     0.4761 us |     0.3976 us |     1.54 |    0.01 |
|     SudokuSpiceConstraints |  EvilA |      74.966 us |     0.4680 us |     0.4149 us |     0.89 |    0.01 |
|                SudokuSharp |  EvilA |  40,707.406 us | 2,824.8576 us | 8,329.1565 us |   495.54 |  115.28 |
|           SudokuSolverLite |  EvilA | 345,495.800 us | 1,468.8377 us | 1,302.0864 us | 4,089.63 |   28.90 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |   1,066.813 us |    20.8410 us |    30.5485 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   1,451.424 us |    27.6702 us |    28.4153 us |     1.36 |    0.04 |
| SudokuSpiceDynamicMultiple |  EvilB |   1,744.189 us |    32.3192 us |    31.7417 us |     1.63 |    0.05 |
|     SudokuSpiceConstraints |  EvilB |     213.068 us |     0.6858 us |     0.6079 us |     0.20 |    0.01 |
|                SudokuSharp |  EvilB |  41,731.454 us |   518.6276 us |   459.7498 us |    38.98 |    1.29 |
|           SudokuSolverLite |  EvilB |  43,433.755 us |   159.4681 us |   149.1665 us |    40.60 |    1.29 |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |            Mean |          Error |         StdDev |          Median |    Ratio | RatioSD |
|--------------------------- |------- |----------------:|---------------:|---------------:|----------------:|---------:|--------:|
|                SudokuSpice |   Easy |       355.34 us |       3.573 us |       3.342 us |       354.95 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |       529.23 us |       3.541 us |       3.139 us |       528.41 us |     1.49 |    0.02 |
| SudokuSpiceDynamicMultiple |   Easy |       616.99 us |       3.205 us |       2.998 us |       616.91 us |     1.74 |    0.02 |
|     SudokuSpiceConstraints |   Easy |       455.48 us |       1.060 us |       0.940 us |       455.49 us |     1.28 |    0.01 |
|                SudokuSharp |   Easy |        95.77 us |       0.325 us |       0.304 us |        95.71 us |     0.27 |    0.00 |
|           SudokuSolverLite |   Easy |     3,829.51 us |       7.137 us |       6.327 us |     3,829.26 us |    10.78 |    0.11 |
|                            |        |                 |                |                |                 |          |         |
|                SudokuSpice | Medium |     4,115.36 us |      71.754 us |      63.608 us |     4,122.62 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |     4,926.65 us |     123.571 us |     115.588 us |     4,993.80 us |     1.20 |    0.04 |
| SudokuSpiceDynamicMultiple | Medium |     5,350.64 us |     132.572 us |     124.008 us |     5,330.30 us |     1.30 |    0.03 |
|     SudokuSpiceConstraints | Medium |       936.67 us |       2.217 us |       1.966 us |       936.60 us |     0.23 |    0.00 |
|                SudokuSharp | Medium |    45,844.56 us |   1,062.698 us |     994.049 us |    45,904.64 us |    11.12 |    0.25 |
|           SudokuSolverLite | Medium |    57,721.59 us |     243.750 us |     228.004 us |    57,630.67 us |    14.03 |    0.23 |
|                            |        |                 |                |                |                 |          |         |
|                SudokuSpice |  HardA |     3,445.89 us |      37.541 us |      35.116 us |     3,441.29 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |     4,180.45 us |      71.966 us |      67.317 us |     4,195.85 us |     1.21 |    0.02 |
| SudokuSpiceDynamicMultiple |  HardA |     4,417.17 us |      92.215 us |      86.258 us |     4,400.90 us |     1.28 |    0.03 |
|     SudokuSpiceConstraints |  HardA |     1,071.91 us |       5.845 us |       4.563 us |     1,072.83 us |     0.31 |    0.00 |
|                SudokuSharp |  HardA |    55,683.24 us |   8,959.346 us |   8,380.578 us |    58,872.00 us |    16.15 |    2.39 |
|           SudokuSolverLite |  HardA |   597,343.07 us |   2,625.896 us |   2,327.789 us |   597,369.00 us |   173.33 |    2.16 |
|                            |        |                 |                |                |                 |          |         |
|                SudokuSpice |  HardB |     7,157.35 us |     121.713 us |     113.850 us |     7,139.53 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     8,672.34 us |     179.296 us |     167.714 us |     8,650.67 us |     1.21 |    0.03 |
| SudokuSpiceDynamicMultiple |  HardB |     9,569.74 us |     317.551 us |     297.037 us |     9,503.89 us |     1.34 |    0.05 |
|     SudokuSpiceConstraints |  HardB |     1,285.86 us |       6.215 us |       5.510 us |     1,284.63 us |     0.18 |    0.00 |
|                SudokuSharp |  HardB |   479,575.80 us | 220,578.783 us | 206,329.531 us |   585,049.00 us |    67.05 |   28.63 |
|           SudokuSolverLite |  HardB |   117,570.87 us |     550.412 us |     514.856 us |   117,530.00 us |    16.43 |    0.25 |
|                            |        |                 |                |                |                 |          |         |
|                SudokuSpice |  EvilA |     4,954.99 us |      63.225 us |      59.140 us |     4,938.09 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     5,931.36 us |      72.112 us |      67.454 us |     5,930.57 us |     1.20 |    0.02 |
| SudokuSpiceDynamicMultiple |  EvilA |     6,067.83 us |      83.099 us |      77.731 us |     6,080.99 us |     1.22 |    0.02 |
|     SudokuSpiceConstraints |  EvilA |     1,206.64 us |       8.205 us |       7.675 us |     1,206.48 us |     0.24 |    0.00 |
|                SudokuSharp |  EvilA |   644,834.10 us | 340,539.052 us | 318,540.441 us |   673,671.50 us |   129.88 |   63.65 |
|           SudokuSolverLite |  EvilA | 9,548,856.85 us |  31,641.321 us |  26,421.936 us | 9,555,623.00 us | 1,924.86 |   21.15 |
|                            |        |                 |                |                |                 |          |         |
|                SudokuSpice |  EvilB |    44,105.56 us |   5,939.737 us |   5,556.034 us |    44,565.91 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |    63,282.04 us |  11,257.405 us |   9,979.397 us |    63,855.00 us |     1.47 |    0.37 |
| SudokuSpiceDynamicMultiple |  EvilB |    65,653.95 us |   9,721.164 us |   8,617.559 us |    68,538.71 us |     1.52 |    0.30 |
|     SudokuSpiceConstraints |  EvilB |     3,815.27 us |      10.489 us |       9.298 us |     3,812.61 us |     0.09 |    0.01 |
|                SudokuSharp |  EvilB |   942,805.27 us |  64,062.121 us |  59,923.748 us |   929,781.00 us |    21.70 |    3.09 |
|           SudokuSolverLite |  EvilB | 1,229,685.87 us |   4,052.642 us |   3,790.844 us | 1,229,671.00 us |    28.30 |    3.63 |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|----------------------- |----------:|----------:|----------:|------:|--------:|
|            SudokuSpice |  1.140 ms | 0.0175 ms | 0.0163 ms |  1.00 |    0.00 |
| SudokuSpiceConstraints |  2.050 ms | 0.0404 ms | 0.0481 ms |  1.80 |    0.04 |
|     SudokuSharpSingles | 12.642 ms | 0.5417 ms | 1.5716 ms | 10.79 |    0.86 |
|       SudokuSharpMixed |  6.449 ms | 0.2033 ms | 0.5962 ms |  5.61 |    0.50 |

### WASM

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|----------------------- |----------:|----------:|----------:|------:|--------:|
|            SudokuSpice |  42.34 ms |  3.007 ms |  2.666 ms |  1.00 |    0.00 |
| SudokuSpiceConstraints |  33.09 ms |  1.199 ms |  1.063 ms |  0.78 |    0.06 |
|     SudokuSharpSingles | 207.64 ms | 89.244 ms | 79.112 ms |  4.87 |    1.71 |
|       SudokuSharpMixed | 105.47 ms | 33.011 ms | 29.264 ms |  2.48 |    0.61 |

