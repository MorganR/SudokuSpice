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

|                 Method | sampleCollection |         Mean |        Error |       StdDev |  Ratio | RatioSD |     Gen 0 |  Gen 1 | Gen 2 | Allocated |
|----------------------- |----------------- |-------------:|-------------:|-------------:|-------:|--------:|----------:|-------:|------:|----------:|
|            SudokuSpice |       Guesses: 0 |     24.56 us |     0.019 us |     0.018 us |   1.00 |    0.00 |    1.8311 | 0.0305 |     - |     11 KB |
| SudokuSpiceConstraints |       Guesses: 0 |    108.00 us |     0.321 us |     0.301 us |   4.40 |    0.01 |   28.4424 | 7.0801 |     - |    175 KB |
|            SudokuSharp |       Guesses: 0 |    788.89 us |    25.984 us |    76.207 us |  32.68 |    2.80 |  134.7656 |      - |     - |    828 KB |
|       SudokuSolverLite |       Guesses: 0 |  1,260.57 us |    25.118 us |    71.662 us |  51.70 |    3.14 |  386.7188 |      - |     - |  2,373 KB |
|                        |                  |              |              |              |        |         |           |        |       |           |
|            SudokuSpice |       Guesses: 1 |     39.44 us |     0.155 us |     0.145 us |   1.00 |    0.00 |    3.2349 | 0.0610 |     - |     20 KB |
| SudokuSpiceConstraints |       Guesses: 1 |    115.98 us |     0.281 us |     0.263 us |   2.94 |    0.01 |   29.0527 | 7.3242 |     - |    178 KB |
|            SudokuSharp |       Guesses: 1 |  1,696.25 us |    74.472 us |   219.582 us |  44.48 |    6.60 |  351.5625 |      - |     - |  2,165 KB |
|       SudokuSolverLite |       Guesses: 1 |  3,890.94 us |   346.855 us | 1,022.710 us | 103.46 |   28.89 | 1242.1875 | 3.9063 |     - |  7,633 KB |
|                        |                  |              |              |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 2-3 |     53.14 us |     0.206 us |     0.193 us |   1.00 |    0.00 |    4.6387 | 0.1221 |     - |     28 KB |
| SudokuSpiceConstraints |     Guesses: 2-3 |    121.97 us |     0.309 us |     0.289 us |   2.30 |    0.01 |   29.5410 | 7.5684 |     - |    181 KB |
|            SudokuSharp |     Guesses: 2-3 |  2,689.75 us |   214.103 us |   627.927 us |  50.03 |   12.76 |  488.2813 |      - |     - |  3,007 KB |
|       SudokuSolverLite |     Guesses: 2-3 |  3,950.87 us |   324.929 us |   932.284 us |  77.41 |   17.53 | 1500.0000 |      - |     - |  9,211 KB |
|                        |                  |              |              |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 4-7 |     74.84 us |     0.537 us |     0.502 us |   1.00 |    0.00 |    6.8359 | 0.1221 |     - |     42 KB |
| SudokuSpiceConstraints |     Guesses: 4-7 |    130.75 us |     0.523 us |     0.489 us |   1.75 |    0.01 |   30.5176 | 7.8125 |     - |    187 KB |
|            SudokuSharp |     Guesses: 4-7 |  3,806.05 us |   371.161 us | 1,094.376 us |  46.63 |    9.72 |  601.5625 |      - |     - |  3,702 KB |
|       SudokuSolverLite |     Guesses: 4-7 | 10,066.54 us | 1,352.659 us | 3,945.770 us | 130.16 |   46.33 | 2031.2500 |      - |     - | 12,459 KB |
|                        |                  |              |              |              |        |         |           |        |       |           |
|            SudokuSpice |      Guesses: 8+ |     97.80 us |     0.687 us |     0.642 us |   1.00 |    0.00 |    9.0332 | 0.2441 |     - |     55 KB |
| SudokuSpiceConstraints |      Guesses: 8+ |    142.82 us |     0.395 us |     0.369 us |   1.46 |    0.01 |   31.2500 | 8.0566 |     - |    192 KB |
|            SudokuSharp |      Guesses: 8+ |  6,464.46 us |   463.934 us | 1,367.919 us |  65.40 |   16.17 | 1039.0625 |      - |     - |  6,401 KB |
|       SudokuSolverLite |      Guesses: 8+ | 21,438.84 us | 3,376.196 us | 9,901.799 us | 228.65 |   82.38 | 3875.0000 |      - |     - | 23,760 KB |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 |   Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|--------:|------:|-----------:|
|                SudokuSpice |   Easy |      10.135 us |     0.0127 us |     0.0118 us |     1.00 |    0.00 |      1.3428 |  0.0153 |     - |       8 KB |
|   SudokuSpiceDynamicSingle |   Easy |      16.300 us |     0.0200 us |     0.0177 us |     1.61 |    0.00 |      2.4719 |  0.0305 |     - |      15 KB |
| SudokuSpiceDynamicMultiple |   Easy |      21.401 us |     0.0178 us |     0.0166 us |     2.11 |    0.00 |      2.4719 |  0.0305 |     - |      15 KB |
|     SudokuSpiceConstraints |   Easy |      44.396 us |     0.7500 us |     0.6649 us |     4.38 |    0.07 |     13.2446 |  2.0142 |     - |      81 KB |
|                SudokuSharp |   Easy |       6.447 us |     0.0092 us |     0.0077 us |     0.64 |    0.00 |      1.1063 |       - |     - |       7 KB |
|           SudokuSolverLite |   Easy |     156.617 us |     0.1442 us |     0.1204 us |    15.45 |    0.02 |     44.4336 |       - |     - |     273 KB |
|                            |        |                |               |               |          |         |             |         |       |            |
|                SudokuSpice | Medium |      85.988 us |     0.2949 us |     0.2758 us |     1.00 |    0.00 |      8.7891 |  0.1221 |     - |      54 KB |
|   SudokuSpiceDynamicSingle | Medium |     118.335 us |     0.2048 us |     0.1816 us |     1.38 |    0.01 |     13.6719 |  0.3662 |     - |      84 KB |
| SudokuSpiceDynamicMultiple | Medium |     141.536 us |     0.7242 us |     0.6420 us |     1.65 |    0.01 |     13.6719 |  0.2441 |     - |      84 KB |
|     SudokuSpiceConstraints | Medium |     126.016 us |     0.3586 us |     0.3354 us |     1.47 |    0.01 |     31.4941 |  9.0332 |     - |     194 KB |
|                SudokuSharp | Medium |   2,985.310 us |    21.7135 us |    20.3109 us |    34.72 |    0.19 |    160.1563 |       - |     - |   1,001 KB |
|           SudokuSolverLite | Medium |   2,358.890 us |     1.2079 us |     1.1299 us |    27.43 |    0.09 |    664.0625 |       - |     - |   4,088 KB |
|                            |        |                |               |               |          |         |             |         |       |            |
|                SudokuSpice |  HardA |      73.451 us |     0.1449 us |     0.1284 us |     1.00 |    0.00 |      7.6904 |  0.1221 |     - |      47 KB |
|   SudokuSpiceDynamicSingle |  HardA |      97.716 us |     0.2245 us |     0.1874 us |     1.33 |    0.00 |     11.8408 |  0.3662 |     - |      73 KB |
| SudokuSpiceDynamicMultiple |  HardA |     119.127 us |     0.2440 us |     0.2282 us |     1.62 |    0.00 |     11.8408 |  0.3662 |     - |      73 KB |
|     SudokuSpiceConstraints |  HardA |     139.550 us |     0.1601 us |     0.1337 us |     1.90 |    0.00 |     30.7617 |  1.7090 |     - |     190 KB |
|                SudokuSharp |  HardA |   3,286.687 us |    64.5216 us |   116.3458 us |    44.75 |    1.84 |    191.4063 |       - |     - |   1,175 KB |
|           SudokuSolverLite |  HardA |  24,523.064 us |    28.4638 us |    26.6251 us |   333.90 |    0.60 |   6937.5000 |       - |     - |  42,554 KB |
|                            |        |                |               |               |          |         |             |         |       |            |
|                SudokuSpice |  HardB |     203.997 us |     0.8564 us |     0.7151 us |     1.00 |    0.00 |     19.7754 |  0.7324 |     - |     122 KB |
|   SudokuSpiceDynamicSingle |  HardB |     256.050 us |     1.2923 us |     1.1456 us |     1.26 |    0.01 |     28.3203 |  0.9766 |     - |     176 KB |
| SudokuSpiceDynamicMultiple |  HardB |     297.826 us |     1.4242 us |     1.3322 us |     1.46 |    0.01 |     28.3203 |  0.9766 |     - |     176 KB |
|     SudokuSpiceConstraints |  HardB |     145.288 us |     0.3491 us |     0.3265 us |     0.71 |    0.00 |     31.2500 | 10.2539 |     - |     191 KB |
|                SudokuSharp |  HardB |  25,903.486 us |   868.5285 us | 2,560.8758 us |   121.93 |   11.38 |   1156.2500 |       - |     - |   7,187 KB |
|           SudokuSolverLite |  HardB |   4,769.389 us |     3.2574 us |     2.8876 us |    23.38 |    0.08 |   1335.9375 |       - |     - |   8,220 KB |
|                            |        |                |               |               |          |         |             |         |       |            |
|                SudokuSpice |  EvilA |     130.825 us |     0.3275 us |     0.2903 us |     1.00 |    0.00 |     14.1602 |  0.4883 |     - |      88 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     159.880 us |     0.3504 us |     0.3107 us |     1.22 |    0.00 |     18.7988 |  0.4883 |     - |     117 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     182.861 us |     0.9363 us |     0.8300 us |     1.40 |    0.01 |     19.0430 |  0.7324 |     - |     117 KB |
|     SudokuSpiceConstraints |  EvilA |     160.096 us |     0.3872 us |     0.3622 us |     1.22 |    0.00 |     33.4473 | 11.4746 |     - |     205 KB |
|                SudokuSharp |  EvilA |  42,069.237 us | 2,577.1332 us | 7,517.6184 us |   338.88 |   53.67 |   1800.0000 |       - |     - |  11,090 KB |
|           SudokuSolverLite |  EvilA | 389,271.685 us |   297.6705 us |   248.5684 us | 2,974.67 |    6.96 | 113000.0000 |       - |     - | 693,269 KB |
|                            |        |                |               |               |          |         |             |         |       |            |
|                SudokuSpice |  EvilB |   1,226.740 us |    21.3137 us |    25.3724 us |     1.00 |    0.00 |    109.3750 |  3.9063 |     - |     679 KB |
|   SudokuSpiceDynamicSingle |  EvilB |   1,670.491 us |    32.9738 us |    44.0191 us |     1.36 |    0.05 |    181.6406 |  7.8125 |     - |   1,122 KB |
| SudokuSpiceDynamicMultiple |  EvilB |   2,023.518 us |    40.3178 us |    59.0973 us |     1.65 |    0.06 |    175.7813 |  7.8125 |     - |   1,092 KB |
|     SudokuSpiceConstraints |  EvilB |     315.620 us |     0.8092 us |     0.6318 us |     0.26 |    0.01 |     40.0391 |  0.9766 |     - |     248 KB |
|                SudokuSharp |  EvilB |  48,279.475 us |   954.5220 us | 1,338.1065 us |    39.49 |    1.43 |   3181.8182 |       - |     - |  20,042 KB |
|           SudokuSolverLite |  EvilB |  50,211.395 us |   116.7638 us |   103.5081 us |    40.92 |    0.93 |  14100.0000 |       - |     - |  86,909 KB |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|------:|------:|-----------:|
|                SudokuSpice |   Easy |       368.6 us |       1.32 us |       1.10 us |     1.00 |    0.00 |      1.4648 |     - |     - |       7 KB |
|   SudokuSpiceDynamicSingle |   Easy |       534.7 us |       0.46 us |       0.39 us |     1.45 |    0.00 |      2.9297 |     - |     - |      12 KB |
| SudokuSpiceDynamicMultiple |   Easy |       676.3 us |       1.47 us |       1.38 us |     1.83 |    0.01 |      2.9297 |     - |     - |      12 KB |
|     SudokuSpiceConstraints |   Easy |       789.3 us |       1.38 us |       1.22 us |     2.14 |    0.00 |     13.6719 |     - |     - |      57 KB |
|                SudokuSharp |   Easy |       104.3 us |       0.14 us |       0.11 us |     0.28 |    0.00 |      1.2207 |     - |     - |       5 KB |
|           SudokuSolverLite |   Easy |     3,597.9 us |       6.39 us |       4.99 us |     9.75 |    0.01 |     62.5000 |     - |     - |     261 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice | Medium |     2,824.8 us |      32.62 us |      28.92 us |     1.00 |    0.00 |      7.8125 |     - |     - |      46 KB |
|   SudokuSpiceDynamicSingle | Medium |     3,814.8 us |      98.16 us |      91.82 us |     1.35 |    0.04 |      7.8125 |     - |     - |      61 KB |
| SudokuSpiceDynamicMultiple | Medium |     4,301.6 us |     149.33 us |     139.69 us |     1.52 |    0.04 |     15.6250 |     - |     - |      66 KB |
|     SudokuSpiceConstraints | Medium |     1,797.3 us |       1.86 us |       1.74 us |     0.64 |    0.01 |     27.3438 |     - |     - |     118 KB |
|                SudokuSharp | Medium |    51,749.1 us |   1,613.74 us |   1,509.49 us |    18.27 |    0.55 |     90.9091 |     - |     - |     638 KB |
|           SudokuSolverLite | Medium |    53,600.9 us |      77.35 us |      68.56 us |    18.98 |    0.19 |    900.0000 |     - |     - |   3,769 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardA |     2,472.9 us |      23.81 us |      21.10 us |     1.00 |    0.00 |      7.8125 |     - |     - |      40 KB |
|   SudokuSpiceDynamicSingle |  HardA |     3,177.0 us |      23.91 us |      22.36 us |     1.28 |    0.01 |     11.7188 |     - |     - |      56 KB |
| SudokuSpiceDynamicMultiple |  HardA |     3,642.2 us |      39.52 us |      33.00 us |     1.47 |    0.02 |     11.7188 |     - |     - |      57 KB |
|     SudokuSpiceConstraints |  HardA |     2,087.0 us |       5.34 us |       4.99 us |     0.84 |    0.01 |     27.3438 |     - |     - |     116 KB |
|                SudokuSharp |  HardA |    59,193.2 us |  14,015.16 us |  13,109.78 us |    24.18 |    5.51 |    142.8571 |     - |     - |     747 KB |
|           SudokuSolverLite |  HardA |   562,755.1 us |   1,061.13 us |     940.67 us |   227.59 |    2.14 |  10000.0000 |     - |     - |  41,347 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardB |     6,162.6 us |      92.72 us |      77.43 us |     1.00 |    0.00 |     23.4375 |     - |     - |     102 KB |
|   SudokuSpiceDynamicSingle |  HardB |     7,798.2 us |     123.74 us |     115.75 us |     1.27 |    0.03 |     31.2500 |     - |     - |     132 KB |
| SudokuSpiceDynamicMultiple |  HardB |     8,594.7 us |     294.05 us |     275.06 us |     1.39 |    0.04 |     31.2500 |     - |     - |     132 KB |
|     SudokuSpiceConstraints |  HardB |     2,184.5 us |       2.94 us |       2.60 us |     0.35 |    0.00 |     27.3438 |     - |     - |     117 KB |
|                SudokuSharp |  HardB |   393,667.1 us | 174,488.44 us | 163,216.59 us |    58.44 |   23.65 |   1000.0000 |     - |     - |   5,730 KB |
|           SudokuSolverLite |  HardB |   113,331.4 us |     175.05 us |     155.18 us |    18.39 |    0.25 |   1800.0000 |     - |     - |   7,757 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilA |     4,160.9 us |      57.03 us |      53.35 us |     1.00 |    0.00 |     15.6250 |     - |     - |      76 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     5,020.7 us |      63.66 us |      59.55 us |     1.21 |    0.02 |     15.6250 |     - |     - |      92 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     5,492.4 us |      71.12 us |      66.52 us |     1.32 |    0.02 |     15.6250 |     - |     - |      92 KB |
|     SudokuSpiceConstraints |  EvilA |     2,378.4 us |      14.98 us |      13.28 us |     0.57 |    0.01 |     27.3438 |     - |     - |     124 KB |
|                SudokuSharp |  EvilA |   776,069.6 us | 381,349.20 us | 356,714.28 us |   186.77 |   86.59 |   3500.0000 |     - |     - |  15,790 KB |
|           SudokuSolverLite |  EvilA | 9,265,011.5 us |  19,577.72 us |  17,355.14 us | 2,223.21 |   26.17 | 166000.0000 |     - |     - | 674,507 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilB |    35,426.2 us |   4,705.69 us |   4,401.71 us |     1.00 |    0.00 |     76.9231 |     - |     - |     557 KB |
|   SudokuSpiceDynamicSingle |  EvilB |    47,945.7 us |  12,492.10 us |  11,685.12 us |     1.36 |    0.31 |    181.8182 |     - |     - |     911 KB |
| SudokuSpiceDynamicMultiple |  EvilB |    58,734.1 us |   7,011.25 us |   6,558.32 us |     1.68 |    0.27 |    100.0000 |     - |     - |     726 KB |
|     SudokuSpiceConstraints |  EvilB |     5,055.2 us |       4.32 us |       3.83 us |     0.15 |    0.02 |     31.2500 |     - |     - |     150 KB |
|                SudokuSharp |  EvilB |   904,203.2 us |  62,550.62 us |  58,509.89 us |    25.93 |    3.90 |   3000.0000 |     - |     - |  14,613 KB |
|           SudokuSolverLite |  EvilB | 1,144,665.4 us |     979.44 us |     916.17 us |    32.80 |    4.23 |  20000.0000 |     - |     - |  83,411 KB |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |   Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|--------:|------:|----------:|
|            SudokuSpice |  1.212 ms | 0.0084 ms | 0.0079 ms |  1.00 |    0.00 |  150.3906 | 17.5781 |     - |    930 KB |
| SudokuSpiceConstraints |  3.810 ms | 0.0414 ms | 0.0387 ms |  3.14 |    0.04 |  976.5625 | 70.3125 |     - |  5,996 KB |
|     SudokuSharpSingles | 14.060 ms | 0.5983 ms | 1.7454 ms | 12.24 |    1.46 | 2781.2500 | 15.6250 |     - | 17,091 KB |
|       SudokuSharpMixed |  7.465 ms | 0.2247 ms | 0.6446 ms |  5.97 |    0.42 | 1507.8125 |  7.8125 |     - |  9,283 KB |

### WASM

|                 Method |      Mean |      Error |    StdDev |    Median | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|-----------:|----------:|----------:|------:|--------:|----------:|------:|------:|----------:|
|            SudokuSpice |  40.06 ms |   2.192 ms |  1.943 ms |  39.99 ms |  1.00 |    0.00 |  153.8462 |     - |     - |    831 KB |
| SudokuSpiceConstraints |  63.29 ms |   3.463 ms |  3.070 ms |  62.54 ms |  1.58 |    0.11 |  875.0000 |     - |     - |  3,756 KB |
|     SudokuSharpSingles | 243.63 ms | 102.895 ms | 96.248 ms | 222.53 ms |  6.09 |    2.54 | 2000.0000 |     - |     - |  8,465 KB |
|       SudokuSharpMixed | 131.58 ms |  74.236 ms | 65.809 ms |  99.07 ms |  3.28 |    1.59 |  500.0000 |     - |     - |  2,708 KB |
|                 Method |      Mean |      Error |     StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |

