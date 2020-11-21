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
|            SudokuSpice |       Guesses: 0 |     25.04 us |     0.127 us |      0.113 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |       Guesses: 0 |     64.97 us |     0.424 us |      0.396 us |   2.60 |    0.02 |
|            SudokuSharp |       Guesses: 0 |    681.14 us |    23.825 us |     70.249 us |  26.63 |    3.30 |
|       SudokuSolverLite |       Guesses: 0 |  1,111.28 us |    24.414 us |     70.439 us |  44.74 |    2.25 |
|                        |                  |              |              |               |        |         |
|            SudokuSpice |       Guesses: 1 |     38.81 us |     0.162 us |      0.152 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |       Guesses: 1 |     68.30 us |     0.637 us |      0.596 us |   1.76 |    0.01 |
|            SudokuSharp |       Guesses: 1 |  1,338.74 us |    47.750 us |    139.288 us |  35.30 |    2.89 |
|       SudokuSolverLite |       Guesses: 1 |  3,409.05 us |   268.490 us |    783.196 us |  86.41 |   21.76 |
|                        |                  |              |              |               |        |         |
|            SudokuSpice |     Guesses: 2-3 |     51.43 us |     0.626 us |      0.555 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |     Guesses: 2-3 |     71.73 us |     0.370 us |      0.346 us |   1.39 |    0.02 |
|            SudokuSharp |     Guesses: 2-3 |  2,157.02 us |   184.473 us |    538.116 us |  41.06 |    9.49 |
|       SudokuSolverLite |     Guesses: 2-3 |  3,444.09 us |   206.460 us |    605.510 us |  66.52 |   12.67 |
|                        |                  |              |              |               |        |         |
|            SudokuSpice |     Guesses: 4-7 |     69.31 us |     0.406 us |      0.380 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |     Guesses: 4-7 |     77.77 us |     0.858 us |      0.803 us |   1.12 |    0.01 |
|            SudokuSharp |     Guesses: 4-7 |  3,143.91 us |   205.063 us |    601.414 us |  42.29 |    6.56 |
|       SudokuSolverLite |     Guesses: 4-7 |  9,612.51 us |   977.259 us |  2,866.131 us | 135.65 |   41.54 |
|                        |                  |              |              |               |        |         |
|            SudokuSpice |      Guesses: 8+ |     90.70 us |     0.978 us |      0.915 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |      Guesses: 8+ |     82.60 us |     0.924 us |      0.864 us |   0.91 |    0.01 |
|            SudokuSharp |      Guesses: 8+ |  4,857.91 us |   316.244 us |    927.490 us |  53.37 |    7.16 |
|       SudokuSolverLite |      Guesses: 8+ | 18,894.30 us | 4,341.194 us | 12,731.970 us | 167.12 |  119.39 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|
|                SudokuSpice |   Easy |       9.991 us |     0.0311 us |     0.0276 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      16.083 us |     0.0578 us |     0.0541 us |     1.61 |    0.01 |
| SudokuSpiceDynamicMultiple |   Easy |      19.975 us |     0.0991 us |     0.0927 us |     2.00 |    0.01 |
|     SudokuSpiceConstraints |   Easy |      19.511 us |     0.0380 us |     0.0337 us |     1.95 |    0.01 |
|                SudokuSharp |   Easy |       6.458 us |     0.0231 us |     0.0204 us |     0.65 |    0.00 |
|           SudokuSolverLite |   Easy |     129.364 us |     0.5537 us |     0.5179 us |    12.95 |    0.06 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |      62.753 us |     0.5545 us |     0.5186 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |      91.042 us |     0.4123 us |     0.3655 us |     1.45 |    0.02 |
| SudokuSpiceDynamicMultiple | Medium |     110.849 us |     0.5091 us |     0.4762 us |     1.77 |    0.02 |
|     SudokuSpiceConstraints | Medium |      60.653 us |     0.3009 us |     0.2667 us |     0.97 |    0.01 |
|                SudokuSharp | Medium |   2,647.528 us |    25.1095 us |    22.2589 us |    42.18 |    0.45 |
|           SudokuSolverLite | Medium |   1,981.383 us |     8.9298 us |     8.3529 us |    31.58 |    0.35 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |      49.369 us |     0.2087 us |     0.1743 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |      72.099 us |     0.1758 us |     0.1644 us |     1.46 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardA |      89.037 us |     0.3842 us |     0.3594 us |     1.80 |    0.01 |
|     SudokuSpiceConstraints |  HardA |      68.114 us |     0.5162 us |     0.4576 us |     1.38 |    0.01 |
|                SudokuSharp |  HardA |   2,847.504 us |    56.6714 us |   119.5392 us |    58.19 |    2.75 |
|           SudokuSolverLite |  HardA |  20,890.090 us |    53.2175 us |    44.4390 us |   423.15 |    1.67 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |     158.491 us |     0.5278 us |     0.4678 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     206.616 us |     1.0005 us |     0.8354 us |     1.30 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardB |     243.297 us |     1.2317 us |     1.1521 us |     1.54 |    0.01 |
|     SudokuSpiceConstraints |  HardB |      78.302 us |     0.5768 us |     0.5113 us |     0.49 |    0.00 |
|                SudokuSharp |  HardB |  19,500.330 us |   662.5145 us | 1,943.0403 us |   121.37 |   11.85 |
|           SudokuSolverLite |  HardB |   4,130.864 us |    13.3249 us |    11.8122 us |    26.06 |    0.08 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |      85.699 us |     0.3429 us |     0.3208 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     113.659 us |     0.3067 us |     0.2869 us |     1.33 |    0.01 |
| SudokuSpiceDynamicMultiple |  EvilA |     132.951 us |     2.2771 us |     1.9015 us |     1.55 |    0.02 |
|     SudokuSpiceConstraints |  EvilA |      73.884 us |     0.4000 us |     0.3546 us |     0.86 |    0.01 |
|                SudokuSharp |  EvilA |  38,778.530 us | 2,105.1332 us | 6,207.0329 us |   474.08 |   90.41 |
|           SudokuSolverLite |  EvilA | 333,729.867 us | 1,830.4236 us | 1,712.1794 us | 3,894.25 |   23.19 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |   1,057.129 us |    21.1208 us |    20.7435 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   1,492.508 us |    25.4986 us |    23.8514 us |     1.41 |    0.04 |
| SudokuSpiceDynamicMultiple |  EvilB |   1,738.625 us |    33.7519 us |    38.8687 us |     1.64 |    0.06 |
|     SudokuSpiceConstraints |  EvilB |     212.272 us |     0.5421 us |     0.5071 us |     0.20 |    0.00 |
|                SudokuSharp |  EvilB |  41,243.563 us |   810.1264 us |   994.9080 us |    38.98 |    1.30 |
|           SudokuSolverLite |  EvilB |  43,548.482 us |   233.7968 us |   218.6937 us |    41.21 |    0.91 |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |            Mean |          Error |         StdDev |    Ratio | RatioSD |
|--------------------------- |------- |----------------:|---------------:|---------------:|---------:|--------:|
|                SudokuSpice |   Easy |       353.51 us |       3.877 us |       2.565 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |       534.20 us |       8.286 us |       5.481 us |     1.51 |    0.02 |
| SudokuSpiceDynamicMultiple |   Easy |       627.48 us |       4.401 us |       2.911 us |     1.78 |    0.02 |
|     SudokuSpiceConstraints |   Easy |       427.24 us |       2.348 us |       1.553 us |     1.21 |    0.01 |
|                SudokuSharp |   Easy |        94.86 us |       0.764 us |       0.506 us |     0.27 |    0.00 |
|           SudokuSolverLite |   Easy |     3,795.17 us |       7.419 us |       4.415 us |    10.73 |    0.08 |
|                            |        |                 |                |                |          |         |
|                SudokuSpice | Medium |     4,288.34 us |     169.506 us |     112.118 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |     5,273.86 us |     181.371 us |     119.966 us |     1.23 |    0.04 |
| SudokuSpiceDynamicMultiple | Medium |     5,603.11 us |     193.468 us |     127.967 us |     1.31 |    0.04 |
|     SudokuSpiceConstraints | Medium |       961.21 us |       6.712 us |       4.439 us |     0.22 |    0.01 |
|                SudokuSharp | Medium |    45,790.04 us |   1,527.092 us |   1,010.077 us |    10.69 |    0.39 |
|           SudokuSolverLite | Medium |    58,111.42 us |     251.639 us |     149.746 us |    13.56 |    0.39 |
|                            |        |                 |                |                |          |         |
|                SudokuSpice |  HardA |     4,063.99 us |      81.394 us |      53.837 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |     4,751.29 us |      98.789 us |      65.343 us |     1.17 |    0.03 |
| SudokuSpiceDynamicMultiple |  HardA |     4,852.39 us |     153.426 us |     101.482 us |     1.19 |    0.03 |
|     SudokuSpiceConstraints |  HardA |     1,193.89 us |       7.044 us |       4.659 us |     0.29 |    0.00 |
|                SudokuSharp |  HardA |    60,010.92 us |  10,768.628 us |   7,122.781 us |    14.76 |    1.71 |
|           SudokuSolverLite |  HardA |   637,410.44 us |  17,688.875 us |  10,526.366 us |   156.59 |    3.56 |
|                            |        |                 |                |                |          |         |
|                SudokuSpice |  HardB |     7,992.06 us |     252.936 us |     167.301 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     9,552.31 us |     493.516 us |     326.430 us |     1.20 |    0.05 |
| SudokuSpiceDynamicMultiple |  HardB |     9,838.13 us |     470.949 us |     311.503 us |     1.23 |    0.05 |
|     SudokuSpiceConstraints |  HardB |     1,334.53 us |       9.509 us |       6.290 us |     0.17 |    0.00 |
|                SudokuSharp |  HardB |   374,972.55 us | 184,426.902 us | 121,986.977 us |    47.01 |   15.53 |
|           SudokuSolverLite |  HardB |   116,320.10 us |   1,111.512 us |     735.196 us |    14.56 |    0.30 |
|                            |        |                 |                |                |          |         |
|                SudokuSpice |  EvilA |     5,269.70 us |     111.297 us |      58.211 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     6,380.24 us |     142.128 us |      94.009 us |     1.21 |    0.02 |
| SudokuSpiceDynamicMultiple |  EvilA |     6,525.09 us |     152.696 us |     100.999 us |     1.24 |    0.03 |
|     SudokuSpiceConstraints |  EvilA |     1,140.16 us |       6.494 us |       3.865 us |     0.22 |    0.00 |
|                SudokuSharp |  EvilA |   569,713.11 us | 404,622.377 us | 240,784.286 us |   106.53 |   47.91 |
|           SudokuSolverLite |  EvilA | 9,917,203.38 us | 438,649.148 us | 229,421.986 us | 1,882.12 |   48.06 |
|                            |        |                 |                |                |          |         |
|                SudokuSpice |  EvilB |    52,369.23 us |  11,599.336 us |   7,672.243 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |    65,216.41 us |  10,143.237 us |   6,709.123 us |     1.27 |    0.25 |
| SudokuSpiceDynamicMultiple |  EvilB |    61,580.92 us |  21,043.088 us |  13,918.700 us |     1.20 |    0.35 |
|     SudokuSpiceConstraints |  EvilB |     4,077.54 us |     248.147 us |     129.785 us |     0.08 |    0.01 |
|                SudokuSharp |  EvilB |   875,963.20 us |  81,252.799 us |  53,743.696 us |    17.09 |    2.84 |
|           SudokuSolverLite |  EvilB | 1,329,407.25 us |  34,802.563 us |  18,202.413 us |    26.36 |    4.28 |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|----------------------- |----------:|----------:|----------:|------:|--------:|
|            SudokuSpice |  1.327 ms | 0.0205 ms | 0.0192 ms |  1.00 |    0.00 |
| SudokuSpiceConstraints |  2.039 ms | 0.0405 ms | 0.0434 ms |  1.53 |    0.04 |
|     SudokuSharpSingles | 12.757 ms | 0.5837 ms | 1.7211 ms |  9.50 |    1.10 |
|       SudokuSharpMixed |  6.206 ms | 0.1652 ms | 0.4870 ms |  4.64 |    0.49 |

### WASM

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|----------------------- |----------:|----------:|----------:|------:|--------:|
|            SudokuSpice |  48.16 ms |  5.237 ms |  3.464 ms |  1.00 |    0.00 |
| SudokuSpiceConstraints |  30.90 ms |  2.579 ms |  1.535 ms |  0.65 |    0.06 |
|     SudokuSharpSingles | 199.59 ms | 93.857 ms | 62.081 ms |  4.19 |    1.41 |
|       SudokuSharpMixed | 106.95 ms | 22.039 ms | 14.578 ms |  2.23 |    0.32 |
