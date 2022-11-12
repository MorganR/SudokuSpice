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
stability regardless of the number of guesses. These benchmarks use the Core CLR on 64 bit Windows
or Linux. Values are roughly consistent for each OS.

|                 Method | sampleCollection |         Mean |        Error |       StdDev |  Ratio | RatioSD |      Gen0 |   Gen1 |   Allocated | Alloc Ratio |
|----------------------- |----------------- |-------------:|-------------:|-------------:|-------:|--------:|----------:|-------:|------------:|------------:|
|            SudokuSpice |       Guesses: 0 |     22.78 us |     0.043 us |     0.038 us |   1.00 |    0.00 |    1.5564 |      - |     9.68 KB |        1.00 |
| SudokuSpiceConstraints |       Guesses: 0 |     92.19 us |     0.212 us |     0.198 us |   4.05 |    0.01 |   23.6816 | 5.3711 |   145.47 KB |       15.02 |
|            SudokuSharp |       Guesses: 0 |    797.38 us |    29.884 us |    87.645 us |  36.05 |    3.37 |  153.3203 |      - |   941.92 KB |       97.28 |
|       SudokuSolverLite |       Guesses: 0 |  1,135.39 us |    22.641 us |    65.687 us |  49.46 |    2.82 |  339.8438 |      - |  2088.36 KB |      215.68 |
|                        |                  |              |              |              |        |         |           |        |             |             |
|            SudokuSpice |       Guesses: 1 |     37.14 us |     0.127 us |     0.119 us |   1.00 |    0.00 |    2.9907 | 0.0610 |    18.56 KB |        1.00 |
| SudokuSpiceConstraints |       Guesses: 1 |     97.40 us |     0.456 us |     0.404 us |   2.62 |    0.01 |   24.1699 | 5.4932 |   148.49 KB |        8.00 |
|            SudokuSharp |       Guesses: 1 |  1,612.08 us |    88.503 us |   259.563 us |  46.77 |    7.31 |  273.4375 |      - |  1684.22 KB |       90.76 |
|       SudokuSolverLite |       Guesses: 1 |  3,598.60 us |   455.828 us | 1,315.168 us | 114.23 |   38.89 | 1531.2500 |      - |   9392.6 KB |      506.13 |
|                        |                  |              |              |              |        |         |           |        |             |             |
|            SudokuSpice |     Guesses: 2-3 |     49.63 us |     0.353 us |     0.330 us |   1.00 |    0.00 |    4.3335 | 0.0610 |    26.55 KB |        1.00 |
| SudokuSpiceConstraints |     Guesses: 2-3 |    103.68 us |     0.310 us |     0.275 us |   2.09 |    0.02 |   24.5361 | 5.7373 |    150.8 KB |        5.68 |
|            SudokuSharp |     Guesses: 2-3 |  2,590.84 us |   196.124 us |   575.197 us |  51.37 |   12.20 |  332.0313 |      - |  2040.25 KB |       76.85 |
|       SudokuSolverLite |     Guesses: 2-3 |  3,651.09 us |   211.763 us |   614.364 us |  79.50 |   16.82 |  992.1875 | 3.9063 |  6098.83 KB |      229.72 |
|                        |                  |              |              |              |        |         |           |        |             |             |
|            SudokuSpice |     Guesses: 4-7 |     69.47 us |     0.515 us |     0.457 us |   1.00 |    0.00 |    6.4697 | 0.1221 |    40.21 KB |        1.00 |
| SudokuSpiceConstraints |     Guesses: 4-7 |    109.42 us |     0.699 us |     0.620 us |   1.58 |    0.01 |   25.2686 | 5.8594 |   155.04 KB |        3.86 |
|            SudokuSharp |     Guesses: 4-7 |  3,691.96 us |   263.327 us |   763.961 us |  56.97 |   12.45 |  808.5938 |      - |  4964.08 KB |      123.47 |
|       SudokuSolverLite |     Guesses: 4-7 |  8,993.99 us | 1,679.971 us | 4,847.102 us | 158.89 |   91.58 |  875.0000 |      - |  5460.21 KB |      135.81 |
|                        |                  |              |              |              |        |         |           |        |             |             |
|            SudokuSpice |      Guesses: 8+ |     90.92 us |     0.647 us |     0.605 us |   1.00 |    0.00 |    8.6670 | 0.2441 |    53.75 KB |        1.00 |
| SudokuSpiceConstraints |      Guesses: 8+ |    118.39 us |     0.759 us |     0.710 us |   1.30 |    0.01 |   25.8789 | 6.1035 |   159.04 KB |        2.96 |
|            SudokuSharp |      Guesses: 8+ |  6,090.91 us |   410.655 us | 1,197.899 us |  69.93 |    9.46 |  765.6250 |      - |  4701.21 KB |       87.47 |
|       SudokuSolverLite |      Guesses: 8+ | 19,707.48 us | 2,887.942 us | 8,469.835 us | 220.78 |  127.33 | 5156.2500 |      - | 31739.12 KB |      590.53 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |        Gen0 |   Gen1 |   Allocated | Alloc Ratio |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|-------:|------------:|------------:|
|                SudokuSpice |   Easy |       8.704 us |     0.0205 us |     0.0192 us |     1.00 |    0.00 |      1.5106 | 0.0153 |     9.31 KB |        1.00 |
|   SudokuSpiceDynamicSingle |   Easy |      10.830 us |     0.0560 us |     0.0496 us |     1.24 |    0.01 |      1.7395 | 0.0305 |     10.7 KB |        1.15 |
| SudokuSpiceDynamicMultiple |   Easy |      14.079 us |     0.0274 us |     0.0243 us |     1.62 |    0.00 |      1.7548 | 0.0305 |    10.77 KB |        1.16 |
|     SudokuSpiceConstraints |   Easy |      34.490 us |     0.1435 us |     0.1199 us |     3.96 |    0.02 |     11.2305 | 1.2817 |    68.92 KB |        7.40 |
|                SudokuSharp |   Easy |       5.608 us |     0.0117 us |     0.0110 us |     0.64 |    0.00 |      1.0681 |      - |     6.58 KB |        0.71 |
|           SudokuSolverLite |   Easy |     132.777 us |     0.1717 us |     0.1522 us |    15.26 |    0.04 |     42.2363 |      - |   260.02 KB |       27.92 |
|                            |        |                |               |               |          |         |             |        |             |             |
|                SudokuSpice | Medium |      78.217 us |     0.3718 us |     0.3477 us |     1.00 |    0.00 |      8.9111 | 0.2441 |    55.13 KB |        1.00 |
|   SudokuSpiceDynamicSingle | Medium |      89.697 us |     0.3583 us |     0.3351 us |     1.15 |    0.01 |      9.1553 | 0.2441 |    56.27 KB |        1.02 |
| SudokuSpiceDynamicMultiple | Medium |     102.151 us |     0.3737 us |     0.3495 us |     1.31 |    0.01 |      9.1553 | 0.2441 |    56.44 KB |        1.02 |
|     SudokuSpiceConstraints | Medium |     100.259 us |     0.1975 us |     0.1751 us |     1.28 |    0.01 |     26.6113 | 6.5918 |   163.54 KB |        2.97 |
|                SudokuSharp | Medium |   2,568.076 us |    13.3857 us |    11.8661 us |    32.84 |    0.19 |    156.2500 |      - |   974.59 KB |       17.68 |
|           SudokuSolverLite | Medium |   2,116.828 us |     4.2610 us |     3.7772 us |    27.07 |    0.12 |    660.1563 |      - |  4054.83 KB |       73.55 |
|                            |        |                |               |               |          |         |             |        |             |             |
|                SudokuSpice |  HardA |      65.764 us |     0.2064 us |     0.1931 us |     1.00 |    0.00 |      7.8125 | 0.1221 |    48.31 KB |        1.00 |
|   SudokuSpiceDynamicSingle |  HardA |      73.692 us |     0.5473 us |     0.5119 us |     1.12 |    0.01 |      8.0566 | 0.2441 |    49.71 KB |        1.03 |
| SudokuSpiceDynamicMultiple |  HardA |      85.561 us |     0.1978 us |     0.1754 us |     1.30 |    0.00 |      8.0566 | 0.2441 |    49.78 KB |        1.03 |
|     SudokuSpiceConstraints |  HardA |     110.668 us |     0.2338 us |     0.2187 us |     1.68 |    0.01 |     25.6348 | 6.5918 |   157.78 KB |        3.27 |
|                SudokuSharp |  HardA |   2,791.552 us |    54.5657 us |   101.1411 us |    42.80 |    1.64 |    195.3125 |      - |  1219.98 KB |       25.25 |
|           SudokuSolverLite |  HardA |  21,736.516 us |    49.4886 us |    46.2917 us |   330.53 |    1.21 |   6812.5000 |      - | 41778.71 KB |      864.72 |
|                            |        |                |               |               |          |         |             |        |             |             |
|                SudokuSpice |  HardB |     184.474 us |     0.6411 us |     0.5683 us |     1.00 |    0.00 |     20.0195 | 0.7324 |   123.08 KB |        1.00 |
|   SudokuSpiceDynamicSingle |  HardB |     202.115 us |     1.0397 us |     0.9216 us |     1.10 |    0.01 |     20.2637 | 0.7324 |   124.51 KB |        1.01 |
| SudokuSpiceDynamicMultiple |  HardB |     224.371 us |     1.0692 us |     1.0001 us |     1.22 |    0.01 |     20.2637 | 0.7324 |   125.21 KB |        1.02 |
|     SudokuSpiceConstraints |  HardB |     114.258 us |     0.1154 us |     0.1079 us |     0.62 |    0.00 |     25.8789 | 6.3477 |   158.88 KB |        1.29 |
|                SudokuSharp |  HardB |  20,314.644 us |   617.0289 us | 1,809.6390 us |   109.71 |    9.51 |   1656.2500 |      - | 10206.23 KB |       82.92 |
|           SudokuSolverLite |  HardB |   4,293.613 us |     8.1489 us |     7.6225 us |    23.28 |    0.07 |   1335.9375 |      - |  8205.54 KB |       66.67 |
|                            |        |                |               |               |          |         |             |        |             |             |
|                SudokuSpice |  EvilA |     118.190 us |     0.2274 us |     0.2128 us |     1.00 |    0.00 |     14.4043 | 0.4883 |    88.98 KB |        1.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     128.660 us |     0.4162 us |     0.3690 us |     1.09 |    0.00 |     14.6484 | 0.4883 |    90.36 KB |        1.02 |
| SudokuSpiceDynamicMultiple |  EvilA |     141.852 us |     0.3963 us |     0.3310 us |     1.20 |    0.00 |     14.6484 | 0.4883 |    90.58 KB |        1.02 |
|     SudokuSpiceConstraints |  EvilA |     129.050 us |     0.3101 us |     0.2749 us |     1.09 |    0.00 |     27.8320 | 6.8359 |   170.76 KB |        1.92 |
|                SudokuSharp |  EvilA |  36,987.684 us | 2,173.7779 us | 6,375.3143 us |   295.92 |   59.67 |   1833.3333 |      - | 11664.74 KB |      131.09 |
|           SudokuSolverLite |  EvilA | 349,324.885 us |   613.5078 us |   543.8587 us | 2,955.52 |    5.27 | 111000.0000 |      - | 680167.4 KB |    7,644.09 |
|                            |        |                |               |               |          |         |             |        |             |             |
|                SudokuSpice |  EvilB |   1,129.770 us |    20.7709 us |    19.4291 us |     1.00 |    0.00 |    105.4688 | 3.9063 |   652.03 KB |        1.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   1,256.478 us |    24.6562 us |    23.0634 us |     1.11 |    0.03 |    109.3750 | 3.9063 |   672.27 KB |        1.03 |
| SudokuSpiceDynamicMultiple |  EvilB |   1,458.225 us |    29.1137 us |    34.6578 us |     1.29 |    0.04 |    105.4688 | 3.9063 |   648.22 KB |        0.99 |
|     SudokuSpiceConstraints |  EvilB |     241.965 us |     0.4625 us |     0.4326 us |     0.21 |    0.00 |     31.7383 | 9.7656 |   195.65 KB |        0.30 |
|                SudokuSharp |  EvilB |  40,101.719 us |   793.4971 us |   742.2377 us |    35.51 |    0.96 |   3307.6923 |      - | 20543.13 KB |       31.51 |
|           SudokuSolverLite |  EvilB |  44,343.964 us |   116.4748 us |   103.2519 us |    39.35 |    0.61 |  13916.6667 |      - | 85621.58 KB |      131.32 |

### WASM

Note: Last updated for release 3.2.1

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|------:|------:|-----------:|
|                SudokuSpice |   Easy |       362.6 us |       0.61 us |       0.57 us |     1.00 |    0.00 |      1.9531 |     - |     - |       8 KB |
|   SudokuSpiceDynamicSingle |   Easy |       448.1 us |       0.65 us |       0.60 us |     1.24 |    0.00 |      1.9531 |     - |     - |       9 KB |
| SudokuSpiceDynamicMultiple |   Easy |       549.2 us |       0.99 us |       0.93 us |     1.51 |    0.00 |      1.9531 |     - |     - |       9 KB |
|     SudokuSpiceConstraints |   Easy |       681.8 us |       0.54 us |       0.48 us |     1.88 |    0.00 |     11.7188 |     - |     - |      49 KB |
|                SudokuSharp |   Easy |       104.7 us |       0.27 us |       0.25 us |     0.29 |    0.00 |      1.2207 |     - |     - |       5 KB |
|           SudokuSolverLite |   Easy |     3,576.2 us |       4.46 us |       3.95 us |     9.86 |    0.02 |     62.5000 |     - |     - |     261 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice | Medium |     2,879.7 us |      52.87 us |      49.45 us |     1.00 |    0.00 |     11.7188 |     - |     - |      48 KB |
|   SudokuSpiceDynamicSingle | Medium |     3,374.7 us |      64.18 us |      60.04 us |     1.17 |    0.03 |     11.7188 |     - |     - |      48 KB |
| SudokuSpiceDynamicMultiple | Medium |     3,712.9 us |      60.01 us |      56.14 us |     1.29 |    0.03 |     11.7188 |     - |     - |      48 KB |
|     SudokuSpiceConstraints | Medium |     1,548.0 us |       1.07 us |       0.95 us |     0.54 |    0.01 |     23.4375 |     - |     - |      99 KB |
|                SudokuSharp | Medium |    50,507.4 us |   1,355.42 us |   1,201.55 us |    17.51 |    0.51 |     90.9091 |     - |     - |     652 KB |
|           SudokuSolverLite | Medium |    54,663.3 us |      61.86 us |      57.87 us |    18.99 |    0.32 |    900.0000 |     - |     - |   3,769 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardA |     2,462.7 us |      21.92 us |      20.51 us |     1.00 |    0.00 |      7.8125 |     - |     - |      41 KB |
|   SudokuSpiceDynamicSingle |  HardA |     2,790.8 us |      25.45 us |      23.80 us |     1.13 |    0.02 |      7.8125 |     - |     - |      42 KB |
| SudokuSpiceDynamicMultiple |  HardA |     3,136.6 us |      31.71 us |      29.66 us |     1.27 |    0.01 |      7.8125 |     - |     - |      42 KB |
|     SudokuSpiceConstraints |  HardA |     1,736.0 us |       1.12 us |       0.99 us |     0.70 |    0.01 |     23.4375 |     - |     - |      96 KB |
|                SudokuSharp |  HardA |    56,533.7 us |  15,755.97 us |  14,738.15 us |    22.95 |    5.95 |    222.2222 |     - |     - |     973 KB |
|           SudokuSolverLite |  HardA |   577,065.8 us |     644.41 us |     571.25 us |   234.17 |    1.90 |  10000.0000 |     - |     - |  41,347 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardB |     6,179.0 us |      87.85 us |      82.18 us |     1.00 |    0.00 |     23.4375 |     - |     - |     105 KB |
|   SudokuSpiceDynamicSingle |  HardB |     6,836.2 us |      90.61 us |      84.75 us |     1.11 |    0.02 |     23.4375 |     - |     - |     107 KB |
| SudokuSpiceDynamicMultiple |  HardB |     7,452.3 us |     153.21 us |     143.31 us |     1.21 |    0.03 |     23.4375 |     - |     - |     106 KB |
|     SudokuSpiceConstraints |  HardB |     1,833.9 us |       0.70 us |       0.55 us |     0.30 |    0.00 |     23.4375 |     - |     - |      97 KB |
|                SudokuSharp |  HardB |   430,153.6 us | 267,940.63 us | 250,631.83 us |    69.81 |   40.84 |   2000.0000 |     - |     - |   9,835 KB |
|           SudokuSolverLite |  HardB |   109,950.1 us |     154.42 us |     144.45 us |    17.80 |    0.24 |   1800.0000 |     - |     - |   7,757 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilA |     4,154.6 us |      44.42 us |      41.55 us |     1.00 |    0.00 |     15.6250 |     - |     - |      77 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     4,557.1 us |      34.92 us |      32.66 us |     1.10 |    0.01 |     15.6250 |     - |     - |      77 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     4,829.1 us |      53.15 us |      49.71 us |     1.16 |    0.02 |     15.6250 |     - |     - |      78 KB |
|     SudokuSpiceConstraints |  EvilA |     1,972.9 us |       3.82 us |       3.57 us |     0.47 |    0.01 |     23.4375 |     - |     - |     103 KB |
|                SudokuSharp |  EvilA |   787,805.3 us | 515,952.38 us | 482,622.18 us |   189.98 |  116.88 |   1000.0000 |     - |     - |   5,917 KB |
|           SudokuSolverLite |  EvilA | 9,442,748.9 us |   5,964.73 us |   4,980.82 us | 2,275.29 |   23.93 | 166000.0000 |     - |     - | 674,507 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilB |    35,869.1 us |   7,415.57 us |   6,936.53 us |     1.00 |    0.00 |     90.9091 |     - |     - |     552 KB |
|   SudokuSpiceDynamicSingle |  EvilB |    39,927.1 us |   4,713.56 us |   4,409.06 us |     1.16 |    0.28 |     83.3333 |     - |     - |     639 KB |
| SudokuSpiceDynamicMultiple |  EvilB |    48,724.4 us |   6,149.82 us |   5,752.55 us |     1.39 |    0.24 |     83.3333 |     - |     - |     610 KB |
|     SudokuSpiceConstraints |  EvilB |     4,012.9 us |       4.44 us |       4.15 us |     0.12 |    0.03 |     23.4375 |     - |     - |     117 KB |
|                SudokuSharp |  EvilB |   888,724.4 us |  70,195.43 us |  65,660.85 us |    25.75 |    5.72 |   2000.0000 |     - |     - |  12,198 KB |
|           SudokuSolverLite |  EvilB | 1,152,642.1 us |     945.58 us |     884.50 us |    33.45 |    7.51 |  20000.0000 |     - |     - |  83,411 KB |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |      Gen0 |    Gen1 |   Allocated | Alloc Ratio |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|--------:|------------:|------------:|
|            SudokuSpice |  1.151 ms | 0.0100 ms | 0.0094 ms |  1.00 |    0.00 |  160.1563 | 21.4844 |   984.67 KB |        1.00 |
| SudokuSpiceConstraints |  2.954 ms | 0.0481 ms | 0.0426 ms |  2.56 |    0.04 |  800.7813 |  3.9063 |   4910.1 KB |        4.99 |
|     SudokuSharpSingles | 13.777 ms | 0.7507 ms | 2.2018 ms | 12.20 |    1.49 | 2937.5000 |       - | 18057.26 KB |       18.34 |
|       SudokuSharpMixed |  7.278 ms | 0.3357 ms | 0.9686 ms |  6.21 |    0.98 | 1562.5000 |       - |  9577.44 KB |        9.73 |

### WASM

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|------:|------:|----------:|
|            SudokuSpice |  38.25 ms |  1.647 ms |  1.460 ms |  1.00 |    0.00 |  153.8462 |     - |     - |    837 KB |
| SudokuSpiceConstraints |  50.06 ms |  2.581 ms |  2.415 ms |  1.31 |    0.10 |  800.0000 |     - |     - |  3,498 KB |
|     SudokuSharpSingles | 217.88 ms | 87.680 ms | 73.217 ms |  5.73 |    1.97 | 2500.0000 |     - |     - | 10,695 KB |
|       SudokuSharpMixed | 137.09 ms | 48.759 ms | 45.609 ms |  3.72 |    1.23 | 1600.0000 |     - |     - |  6,868 KB |
