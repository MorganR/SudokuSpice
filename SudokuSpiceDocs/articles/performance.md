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

|                 Method | sampleCollection |         Mean |        Error |       StdDev |       Median |  Ratio | RatioSD |     Gen 0 |  Gen 1 | Gen 2 | Allocated |
|----------------------- |----------------- |-------------:|-------------:|-------------:|-------------:|-------:|--------:|----------:|-------:|------:|----------:|
|            SudokuSpice |       Guesses: 0 |     25.09 us |     0.017 us |     0.013 us |     25.08 us |   1.00 |    0.00 |    1.8311 | 0.0305 |     - |     11 KB |
| SudokuSpiceConstraints |       Guesses: 0 |     70.79 us |     0.073 us |     0.068 us |     70.81 us |   2.82 |    0.00 |   17.9443 | 3.6621 |     - |    110 KB |
|            SudokuSharp |       Guesses: 0 |    817.78 us |    28.608 us |    84.351 us |    814.02 us |  32.56 |    2.61 |  126.9531 |      - |     - |    780 KB |
|       SudokuSolverLite |       Guesses: 0 |  1,284.38 us |    29.173 us |    85.559 us |  1,288.81 us |  49.73 |    3.88 |  312.5000 |      - |     - |  1,916 KB |
|                        |                  |              |              |              |              |        |         |           |        |       |           |
|            SudokuSpice |       Guesses: 1 |     40.25 us |     0.196 us |     0.184 us |     40.25 us |   1.00 |    0.00 |    3.2349 | 0.0610 |     - |     20 KB |
| SudokuSpiceConstraints |       Guesses: 1 |     74.51 us |     0.123 us |     0.115 us |     74.52 us |   1.85 |    0.01 |   18.4326 | 3.9063 |     - |    113 KB |
|            SudokuSharp |       Guesses: 1 |  1,678.48 us |    68.227 us |   201.169 us |  1,665.56 us |  42.01 |    4.89 |  246.0938 |      - |     - |  1,514 KB |
|       SudokuSolverLite |       Guesses: 1 |  3,981.60 us |   370.771 us | 1,081.557 us |  3,947.22 us |  95.43 |   29.51 |  847.6563 |      - |     - |  5,205 KB |
|                        |                  |              |              |              |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 2-3 |     54.02 us |     0.307 us |     0.287 us |     54.01 us |   1.00 |    0.00 |    4.5776 | 0.1221 |     - |     28 KB |
| SudokuSpiceConstraints |     Guesses: 2-3 |     77.95 us |     0.159 us |     0.141 us |     77.95 us |   1.44 |    0.01 |   18.6768 | 4.0283 |     - |    115 KB |
|            SudokuSharp |     Guesses: 2-3 |  2,745.08 us |   213.166 us |   628.524 us |  2,669.67 us |  47.40 |    7.76 |  492.1875 |      - |     - |  3,018 KB |
|       SudokuSolverLite |     Guesses: 2-3 |  3,985.38 us |   266.156 us |   776.389 us |  3,754.77 us |  73.91 |    9.81 | 1234.3750 | 3.9063 |     - |  7,579 KB |
|                        |                  |              |              |              |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 4-7 |     76.20 us |     0.477 us |     0.423 us |     76.07 us |   1.00 |    0.00 |    6.7139 | 0.1221 |     - |     42 KB |
| SudokuSpiceConstraints |     Guesses: 4-7 |     83.01 us |     0.164 us |     0.154 us |     83.05 us |   1.09 |    0.01 |   19.2871 | 4.0283 |     - |    119 KB |
|            SudokuSharp |     Guesses: 4-7 |  4,065.93 us |   324.599 us |   946.870 us |  4,000.09 us |  50.74 |    9.64 |  687.5000 |      - |     - |  4,214 KB |
|       SudokuSolverLite |     Guesses: 4-7 | 10,022.57 us |   916.338 us | 2,687.461 us |  9,624.52 us | 137.60 |   45.60 | 2960.9375 | 7.8125 |     - | 18,150 KB |
|                        |                  |              |              |              |              |        |         |           |        |       |           |
|            SudokuSpice |      Guesses: 8+ |     99.03 us |     0.677 us |     0.634 us |     99.00 us |   1.00 |    0.00 |    8.9111 | 0.2441 |     - |     55 KB |
| SudokuSpiceConstraints |      Guesses: 8+ |     88.76 us |     0.170 us |     0.151 us |     88.80 us |   0.90 |    0.01 |   19.8975 | 4.1504 |     - |    122 KB |
|            SudokuSharp |      Guesses: 8+ |  6,024.64 us |   422.583 us | 1,239.364 us |  5,926.75 us |  63.90 |   14.08 | 1320.3125 |      - |     - |  8,103 KB |
|       SudokuSolverLite |      Guesses: 8+ | 20,975.40 us | 3,077.791 us | 9,026.628 us | 20,308.59 us | 230.77 |   99.60 | 7281.2500 |      - |     - | 44,727 KB |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 |  Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|-------:|------:|-----------:|
|                SudokuSpice |   Easy |      10.376 us |     0.0159 us |     0.0149 us |     1.00 |    0.00 |      1.3428 | 0.0153 |     - |       8 KB |
|   SudokuSpiceDynamicSingle |   Easy |      16.381 us |     0.0408 us |     0.0340 us |     1.58 |    0.00 |      2.4719 | 0.0305 |     - |      15 KB |
| SudokuSpiceDynamicMultiple |   Easy |      22.440 us |     0.1022 us |     0.0906 us |     2.16 |    0.01 |      2.4719 | 0.0305 |     - |      15 KB |
|     SudokuSpiceConstraints |   Easy |      25.900 us |     0.0475 us |     0.0421 us |     2.50 |    0.01 |      8.0566 | 0.9460 |     - |      50 KB |
|                SudokuSharp |   Easy |       6.549 us |     0.0694 us |     0.0616 us |     0.63 |    0.01 |      1.1063 |      - |     - |       7 KB |
|           SudokuSolverLite |   Easy |     157.011 us |     0.1890 us |     0.1767 us |    15.13 |    0.03 |     44.4336 |      - |     - |     273 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice | Medium |      87.619 us |     0.3292 us |     0.3079 us |     1.00 |    0.00 |      8.6670 | 0.1221 |     - |      54 KB |
|   SudokuSpiceDynamicSingle | Medium |     119.519 us |     0.4813 us |     0.4502 us |     1.36 |    0.01 |     13.7939 | 0.3662 |     - |      85 KB |
| SudokuSpiceDynamicMultiple | Medium |     143.604 us |     0.4239 us |     0.3539 us |     1.64 |    0.01 |     13.6719 | 0.2441 |     - |      85 KB |
|     SudokuSpiceConstraints | Medium |      76.542 us |     0.1786 us |     0.1671 us |     0.87 |    0.00 |     20.2637 | 5.0049 |     - |     125 KB |
|                SudokuSharp | Medium |   2,970.046 us |    22.0903 us |    20.6633 us |    33.90 |    0.27 |    160.1563 |      - |     - |     993 KB |
|           SudokuSolverLite | Medium |   2,319.316 us |     1.9924 us |     1.7662 us |    26.47 |    0.10 |    664.0625 |      - |     - |   4,088 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardA |      74.495 us |     0.1471 us |     0.1376 us |     1.00 |    0.00 |      7.6904 | 0.1221 |     - |      47 KB |
|   SudokuSpiceDynamicSingle |  HardA |      99.126 us |     0.2222 us |     0.2079 us |     1.33 |    0.00 |     11.7188 | 0.3662 |     - |      72 KB |
| SudokuSpiceDynamicMultiple |  HardA |     119.802 us |     0.2394 us |     0.2122 us |     1.61 |    0.00 |     11.8408 | 0.3662 |     - |      73 KB |
|     SudokuSpiceConstraints |  HardA |      82.085 us |     0.2454 us |     0.2175 us |     1.10 |    0.00 |     19.4092 | 4.7607 |     - |     120 KB |
|                SudokuSharp |  HardA |   3,268.461 us |    64.8953 us |   101.0342 us |    43.75 |    1.29 |    199.2188 |      - |     - |   1,221 KB |
|           SudokuSolverLite |  HardA |  24,151.990 us |    31.3502 us |    27.7912 us |   324.18 |    0.45 |   6937.5000 |      - |     - |  42,554 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardB |     207.820 us |     0.5048 us |     0.4722 us |     1.00 |    0.00 |     19.7754 | 0.4883 |     - |     122 KB |
|   SudokuSpiceDynamicSingle |  HardB |     255.344 us |     1.1959 us |     1.1186 us |     1.23 |    0.01 |     28.3203 | 0.9766 |     - |     175 KB |
| SudokuSpiceDynamicMultiple |  HardB |     298.989 us |     1.1020 us |     1.0308 us |     1.44 |    0.01 |     28.3203 | 0.9766 |     - |     176 KB |
|     SudokuSpiceConstraints |  HardB |      93.313 us |     0.3395 us |     0.3176 us |     0.45 |    0.00 |     20.1416 | 4.7607 |     - |     124 KB |
|                SudokuSharp |  HardB |  23,174.874 us |   708.5986 us | 2,055.7731 us |   108.47 |   10.78 |   1218.7500 |      - |     - |   7,624 KB |
|           SudokuSolverLite |  HardB |   4,763.138 us |     4.2141 us |     3.9419 us |    22.92 |    0.06 |   1335.9375 |      - |     - |   8,220 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilA |     131.635 us |     0.3728 us |     0.3487 us |     1.00 |    0.00 |     14.1602 | 0.4883 |     - |      88 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     172.129 us |     0.5143 us |     0.4811 us |     1.31 |    0.01 |     19.0430 | 0.7324 |     - |     117 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     184.953 us |     0.2840 us |     0.2656 us |     1.41 |    0.00 |     18.7988 | 0.7324 |     - |     117 KB |
|     SudokuSpiceConstraints |  EvilA |      91.387 us |     0.3214 us |     0.2849 us |     0.69 |    0.00 |     21.2402 | 5.3711 |     - |     130 KB |
|                SudokuSharp |  EvilA |  42,498.795 us | 2,813.7604 us | 8,207.8712 us |   338.83 |   64.01 |   2000.0000 |      - |     - |  12,748 KB |
|           SudokuSolverLite |  EvilA | 403,103.036 us | 1,186.9744 us | 1,110.2966 us | 3,062.31 |   13.70 | 113000.0000 |      - |     - | 693,265 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilB |   1,264.698 us |    24.1121 us |    22.5545 us |     1.00 |    0.00 |    109.3750 | 3.9063 |     - |     672 KB |
|   SudokuSpiceDynamicSingle |  EvilB |   1,686.127 us |    32.9131 us |    43.9380 us |     1.35 |    0.05 |    187.5000 | 7.8125 |     - |   1,154 KB |
| SudokuSpiceDynamicMultiple |  EvilB |   2,105.270 us |    41.9915 us |    56.0574 us |     1.67 |    0.05 |    175.7813 | 7.8125 |     - |   1,093 KB |
|     SudokuSpiceConstraints |  EvilB |     237.618 us |     0.2844 us |     0.2374 us |     0.19 |    0.00 |     26.3672 | 7.3242 |     - |     162 KB |
|                SudokuSharp |  EvilB |  47,842.343 us |   777.6965 us |   607.1742 us |    37.71 |    0.91 |   3181.8182 |      - |     - |  19,959 KB |
|           SudokuSolverLite |  EvilB |  49,562.041 us |    70.6936 us |    66.1268 us |    39.20 |    0.73 |  14181.8182 |      - |     - |  86,909 KB |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|------:|------:|-----------:|
|                SudokuSpice |   Easy |       402.4 us |       0.40 us |       0.35 us |     1.00 |    0.00 |      1.4648 |     - |     - |       7 KB |
|   SudokuSpiceDynamicSingle |   Easy |       561.3 us |       0.71 us |       0.63 us |     1.39 |    0.00 |      2.9297 |     - |     - |      12 KB |
| SudokuSpiceDynamicMultiple |   Easy |       704.8 us |       0.49 us |       0.46 us |     1.75 |    0.00 |      2.9297 |     - |     - |      12 KB |
|     SudokuSpiceConstraints |   Easy |       476.7 us |       0.62 us |       0.58 us |     1.18 |    0.00 |      9.2773 |     - |     - |      38 KB |
|                SudokuSharp |   Easy |       103.9 us |       0.05 us |       0.04 us |     0.26 |    0.00 |      1.2207 |     - |     - |       5 KB |
|           SudokuSolverLite |   Easy |     3,547.4 us |       3.61 us |       3.01 us |     8.82 |    0.01 |     62.5000 |     - |     - |     261 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice | Medium |     4,933.3 us |     156.80 us |     146.67 us |     1.00 |    0.00 |      7.8125 |     - |     - |      45 KB |
|   SudokuSpiceDynamicSingle | Medium |     5,747.3 us |     131.23 us |     116.33 us |     1.17 |    0.03 |     15.6250 |     - |     - |      64 KB |
| SudokuSpiceDynamicMultiple | Medium |     6,242.6 us |     143.11 us |     133.86 us |     1.27 |    0.05 |     15.6250 |     - |     - |      66 KB |
|     SudokuSpiceConstraints | Medium |     1,043.2 us |       1.59 us |       1.41 us |     0.21 |    0.01 |     17.5781 |     - |     - |      75 KB |
|                SudokuSharp | Medium |    50,054.4 us |   1,585.57 us |   1,483.14 us |    10.16 |    0.48 |    100.0000 |     - |     - |     686 KB |
|           SudokuSolverLite | Medium |    52,520.5 us |      42.04 us |      37.27 us |    10.67 |    0.32 |    900.0000 |     - |     - |   3,769 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardA |     4,169.0 us |      46.05 us |      40.82 us |     1.00 |    0.00 |      7.8125 |     - |     - |      40 KB |
|   SudokuSpiceDynamicSingle |  HardA |     4,911.8 us |      68.63 us |      60.84 us |     1.18 |    0.01 |      7.8125 |     - |     - |      56 KB |
| SudokuSpiceDynamicMultiple |  HardA |     5,374.0 us |      65.19 us |      57.79 us |     1.29 |    0.02 |      7.8125 |     - |     - |      56 KB |
|     SudokuSpiceConstraints |  HardA |     1,185.5 us |       0.83 us |       0.74 us |     0.28 |    0.00 |     17.5781 |     - |     - |      73 KB |
|                SudokuSharp |  HardA |    60,522.3 us |  10,539.85 us |   9,858.98 us |    14.59 |    2.47 |    125.0000 |     - |     - |     833 KB |
|           SudokuSolverLite |  HardA |   557,090.9 us |     718.95 us |     637.33 us |   133.64 |    1.32 |  10000.0000 |     - |     - |  41,347 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardB |     9,039.9 us |     268.70 us |     251.34 us |     1.00 |    0.00 |     15.6250 |     - |     - |     104 KB |
|   SudokuSpiceDynamicSingle |  HardB |    10,566.7 us |     302.94 us |     283.37 us |     1.17 |    0.04 |     31.2500 |     - |     - |     138 KB |
| SudokuSpiceDynamicMultiple |  HardB |    11,667.3 us |     274.66 us |     256.92 us |     1.29 |    0.04 |     31.2500 |     - |     - |     132 KB |
|     SudokuSpiceConstraints |  HardB |     1,420.1 us |       1.10 us |       1.02 us |     0.16 |    0.00 |     17.5781 |     - |     - |      75 KB |
|                SudokuSharp |  HardB |   405,277.6 us | 163,620.32 us | 145,045.16 us |    44.72 |   15.74 |   1500.0000 |     - |     - |   7,134 KB |
|           SudokuSolverLite |  HardB |   110,508.0 us |      91.96 us |      86.02 us |    12.23 |    0.35 |   1800.0000 |     - |     - |   7,757 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilA |     6,256.1 us |      72.96 us |      64.68 us |     1.00 |    0.00 |     15.6250 |     - |     - |      75 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     7,107.4 us |      86.19 us |      80.62 us |     1.14 |    0.02 |     15.6250 |     - |     - |      93 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     7,645.7 us |      80.40 us |      71.27 us |     1.22 |    0.02 |     15.6250 |     - |     - |      92 KB |
|     SudokuSpiceConstraints |  EvilA |     1,261.9 us |       1.00 us |       0.93 us |     0.20 |    0.00 |     17.5781 |     - |     - |      78 KB |
|                SudokuSharp |  EvilA |   701,188.9 us | 418,183.63 us | 391,169.22 us |   109.34 |   64.07 |           - |     - |     - |   2,592 KB |
|           SudokuSolverLite |  EvilA | 9,159,970.4 us |  15,908.92 us |  14,102.84 us | 1,464.32 |   14.54 | 166000.0000 |     - |     - | 674,507 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilB |    57,560.2 us |   8,251.51 us |   7,718.47 us |     1.00 |    0.00 |    125.0000 |     - |     - |     615 KB |
|   SudokuSpiceDynamicSingle |  EvilB |    71,874.6 us |  13,859.21 us |  12,963.92 us |     1.27 |    0.27 |    166.6667 |     - |     - |     915 KB |
| SudokuSpiceDynamicMultiple |  EvilB |    85,545.9 us |  14,390.55 us |  13,460.93 us |     1.50 |    0.26 |           - |     - |     - |     480 KB |
|     SudokuSpiceConstraints |  EvilB |     4,071.9 us |       3.30 us |       2.92 us |     0.07 |    0.01 |     23.4375 |     - |     - |      99 KB |
|                SudokuSharp |  EvilB |   863,445.3 us |  42,941.11 us |  38,066.18 us |    15.24 |    2.41 |   3000.0000 |     - |     - |  13,099 KB |
|           SudokuSolverLite |  EvilB | 1,170,533.2 us |     578.02 us |     540.68 us |    20.70 |    2.99 |  20000.0000 |     - |     - |  83,411 KB |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |    Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|---------:|------:|----------:|
|            SudokuSpice |  1.203 ms | 0.0116 ms | 0.0108 ms |  1.00 |    0.00 |  150.3906 |  19.5313 |     - |    927 KB |
| SudokuSpiceConstraints |  2.274 ms | 0.0411 ms | 0.0384 ms |  1.89 |    0.03 |  574.2188 | 117.1875 |     - |  3,524 KB |
|     SudokuSharpSingles | 15.556 ms | 0.6542 ms | 1.9289 ms | 13.41 |    1.80 | 2625.0000 |  15.6250 |     - | 16,145 KB |
|       SudokuSharpMixed |  7.097 ms | 0.2178 ms | 0.6250 ms |  5.82 |    0.44 | 1820.3125 |  15.6250 |     - | 11,197 KB |

### WASM

|                 Method |      Mean |      Error |     StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|-----------:|-----------:|------:|--------:|----------:|------:|------:|----------:|
|            SudokuSpice |  54.61 ms |   3.746 ms |   3.504 ms |  1.00 |    0.00 |  111.1111 |     - |     - |    866 KB |
| SudokuSpiceConstraints |  33.34 ms |   0.667 ms |   0.624 ms |  0.61 |    0.04 |  600.0000 |     - |     - |  2,636 KB |
|     SudokuSharpSingles | 279.85 ms | 128.779 ms | 120.460 ms |  5.16 |    2.28 | 1500.0000 |     - |     - |  6,270 KB |
|       SudokuSharpMixed | 120.05 ms |  61.780 ms |  57.789 ms |  2.19 |    1.03 | 1333.3333 |     - |     - |  6,510 KB |

