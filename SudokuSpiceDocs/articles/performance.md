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

|                 Method | sampleCollection |         Mean |        Error |        StdDev |       Median |  Ratio | RatioSD |     Gen 0 |  Gen 1 | Gen 2 | Allocated |
|----------------------- |----------------- |-------------:|-------------:|--------------:|-------------:|-------:|--------:|----------:|-------:|------:|----------:|
|            SudokuSpice |       Guesses: 0 |     26.08 us |     0.233 us |      0.218 us |     26.10 us |   1.00 |    0.00 |    1.8005 | 0.0305 |     - |     11 KB |
| SudokuSpiceConstraints |       Guesses: 0 |     76.54 us |     0.338 us |      0.316 us |     76.59 us |   2.94 |    0.03 |   17.8223 | 3.6621 |     - |    109 KB |
|            SudokuSharp |       Guesses: 0 |    827.25 us |    27.009 us |     79.211 us |    830.42 us |  32.15 |    3.08 |  129.8828 |      - |     - |    801 KB |
|       SudokuSolverLite |       Guesses: 0 |  1,340.78 us |    26.674 us |     74.797 us |  1,340.79 us |  52.20 |    2.61 |  361.3281 |      - |     - |  2,220 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |       Guesses: 1 |     42.05 us |     0.427 us |      0.400 us |     42.00 us |   1.00 |    0.00 |    3.2349 | 0.0610 |     - |     20 KB |
| SudokuSpiceConstraints |       Guesses: 1 |     79.12 us |     0.487 us |      0.455 us |     79.30 us |   1.88 |    0.02 |   18.1885 | 3.7842 |     - |    112 KB |
|            SudokuSharp |       Guesses: 1 |  1,800.40 us |    74.229 us |    218.865 us |  1,796.91 us |  44.11 |    4.35 |  296.8750 |      - |     - |  1,824 KB |
|       SudokuSolverLite |       Guesses: 1 |  3,997.02 us |   508.466 us |  1,467.039 us |  3,689.55 us |  83.45 |   31.24 | 1695.3125 |      - |     - | 10,428 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 2-3 |     56.97 us |     0.592 us |      0.554 us |     57.02 us |   1.00 |    0.00 |    4.5776 | 0.1221 |     - |     28 KB |
| SudokuSpiceConstraints |     Guesses: 2-3 |     83.09 us |     0.773 us |      0.723 us |     83.17 us |   1.46 |    0.02 |   18.5547 | 3.9063 |     - |    114 KB |
|            SudokuSharp |     Guesses: 2-3 |  2,899.29 us |   216.357 us |    631.125 us |  2,842.24 us |  55.07 |   10.68 |  660.1563 |      - |     - |  4,046 KB |
|       SudokuSolverLite |     Guesses: 2-3 |  3,996.34 us |   309.393 us |    902.514 us |  3,813.10 us |  68.56 |   15.13 |  804.6875 |      - |     - |  4,935 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 4-7 |     78.63 us |     0.993 us |      0.929 us |     78.50 us |   1.00 |    0.00 |    6.7139 | 0.1221 |     - |     41 KB |
| SudokuSpiceConstraints |     Guesses: 4-7 |     89.05 us |     0.987 us |      0.923 us |     89.01 us |   1.13 |    0.02 |   19.1650 | 4.0283 |     - |    118 KB |
|            SudokuSharp |     Guesses: 4-7 |  3,911.48 us |   475.787 us |  1,357.448 us |  3,830.34 us |  42.72 |   11.12 |  421.8750 |      - |     - |  2,646 KB |
|       SudokuSolverLite |     Guesses: 4-7 | 10,308.56 us | 1,972.455 us |  5,627.525 us |  8,472.64 us | 128.50 |   70.17 | 1375.0000 |      - |     - |  8,576 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |      Guesses: 8+ |    105.26 us |     0.956 us |      0.894 us |    105.34 us |   1.00 |    0.00 |    8.9111 | 0.2441 |     - |     55 KB |
| SudokuSpiceConstraints |      Guesses: 8+ |     95.66 us |     0.623 us |      0.583 us |     95.73 us |   0.91 |    0.01 |   19.7754 | 4.2725 |     - |    121 KB |
|            SudokuSharp |      Guesses: 8+ |  6,548.23 us |   494.007 us |  1,441.042 us |  6,571.86 us |  65.90 |    9.69 | 1148.4375 |      - |     - |  7,050 KB |
|       SudokuSolverLite |      Guesses: 8+ | 19,702.79 us | 3,922.594 us | 10,934.625 us | 18,461.86 us | 216.13 |  117.21 | 3687.5000 |      - |     - | 22,649 KB |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 |  Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|-------:|------:|-----------:|
|                SudokuSpice |   Easy |      10.174 us |     0.0151 us |     0.0126 us |     1.00 |    0.00 |      1.4343 | 0.0153 |     - |       9 KB |
|   SudokuSpiceDynamicSingle |   Easy |      16.823 us |     0.0282 us |     0.0264 us |     1.65 |    0.00 |      2.5635 | 0.0610 |     - |      16 KB |
| SudokuSpiceDynamicMultiple |   Easy |      21.525 us |     0.0187 us |     0.0165 us |     2.12 |    0.00 |      2.5635 | 0.0610 |     - |      16 KB |
|     SudokuSpiceConstraints |   Easy |      25.123 us |     0.0125 us |     0.0105 us |     2.47 |    0.00 |      7.8735 | 0.8240 |     - |      48 KB |
|                SudokuSharp |   Easy |       6.960 us |     0.1164 us |     0.1089 us |     0.68 |    0.01 |      1.1063 |      - |     - |       7 KB |
|           SudokuSolverLite |   Easy |     166.101 us |     1.8373 us |     1.7187 us |    16.34 |    0.17 |     44.4336 |      - |     - |     273 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice | Medium |      94.564 us |     0.8035 us |     0.7516 us |     1.00 |    0.00 |      8.7891 | 0.2441 |     - |      54 KB |
|   SudokuSpiceDynamicSingle | Medium |     127.006 us |     1.3512 us |     1.2639 us |     1.34 |    0.02 |     13.6719 | 0.4883 |     - |      84 KB |
| SudokuSpiceDynamicMultiple | Medium |     150.774 us |     1.8804 us |     1.7590 us |     1.59 |    0.02 |     13.9160 | 0.4883 |     - |      86 KB |
|     SudokuSpiceConstraints | Medium |      85.487 us |     0.9468 us |     0.8856 us |     0.90 |    0.01 |     20.0195 | 5.0049 |     - |     123 KB |
|                SudokuSharp | Medium |   3,178.983 us |    25.7746 us |    24.1096 us |    33.62 |    0.35 |    160.1563 |      - |     - |     994 KB |
|           SudokuSolverLite | Medium |   2,482.919 us |    21.4883 us |    20.1002 us |    26.26 |    0.35 |    664.0625 |      - |     - |   4,088 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardA |      78.528 us |     0.6566 us |     0.6141 us |     1.00 |    0.00 |      7.6904 | 0.2441 |     - |      48 KB |
|   SudokuSpiceDynamicSingle |  HardA |     109.786 us |     0.8155 us |     0.7229 us |     1.40 |    0.02 |     11.8408 | 0.3662 |     - |      73 KB |
| SudokuSpiceDynamicMultiple |  HardA |     127.551 us |     1.4795 us |     1.3840 us |     1.62 |    0.02 |     11.7188 | 0.2441 |     - |      73 KB |
|     SudokuSpiceConstraints |  HardA |      94.773 us |     1.5213 us |     1.4230 us |     1.21 |    0.02 |     19.2871 | 4.6387 |     - |     118 KB |
|                SudokuSharp |  HardA |   3,492.724 us |    69.2802 us |   126.6827 us |    44.27 |    1.35 |    191.4063 |      - |     - |   1,192 KB |
|           SudokuSolverLite |  HardA |  25,963.558 us |   173.1662 us |   153.5073 us |   330.42 |    3.35 |   6937.5000 |      - |     - |  42,554 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardB |     217.646 us |     1.9052 us |     1.7821 us |     1.00 |    0.00 |     20.0195 | 0.7324 |     - |     123 KB |
|   SudokuSpiceDynamicSingle |  HardB |     270.018 us |     2.1199 us |     1.9830 us |     1.24 |    0.01 |     28.8086 | 0.9766 |     - |     177 KB |
| SudokuSpiceDynamicMultiple |  HardB |     315.904 us |     4.8679 us |     4.0649 us |     1.45 |    0.02 |     28.3203 | 0.9766 |     - |     176 KB |
|     SudokuSpiceConstraints |  HardB |     101.044 us |     0.9498 us |     0.8884 us |     0.46 |    0.01 |     19.8975 | 4.6387 |     - |     122 KB |
|                SudokuSharp |  HardB |  25,506.308 us |   867.3402 us | 2,557.3721 us |   118.89 |   10.07 |   1437.5000 |      - |     - |   8,824 KB |
|           SudokuSolverLite |  HardB |   4,996.725 us |    42.2492 us |    39.5200 us |    22.96 |    0.30 |   1335.9375 |      - |     - |   8,220 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilA |     137.371 us |     1.0771 us |     1.0076 us |     1.00 |    0.00 |     14.4043 | 0.4883 |     - |      88 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     169.541 us |     1.7616 us |     1.4710 us |     1.24 |    0.01 |     19.0430 | 0.7324 |     - |     117 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     194.091 us |     1.6470 us |     1.4600 us |     1.41 |    0.02 |     19.0430 | 0.7324 |     - |     117 KB |
|     SudokuSpiceConstraints |  EvilA |      96.705 us |     0.9076 us |     0.8490 us |     0.70 |    0.01 |     20.9961 | 5.3711 |     - |     129 KB |
|                SudokuSharp |  EvilA |  45,336.140 us | 2,922.8918 us | 8,618.2124 us |   324.02 |   71.86 |   2545.4545 |      - |     - |  16,073 KB |
|           SudokuSolverLite |  EvilA | 420,481.329 us | 5,017.8426 us | 4,448.1867 us | 3,063.08 |   38.20 | 113000.0000 |      - |     - | 693,267 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilB |   1,295.108 us |    23.4940 us |    21.9763 us |     1.00 |    0.00 |    107.4219 | 3.9063 |     - |     658 KB |
|   SudokuSpiceDynamicSingle |  EvilB |   1,741.799 us |    33.5158 us |    38.5968 us |     1.34 |    0.04 |    183.5938 | 9.7656 |     - |   1,133 KB |
| SudokuSpiceDynamicMultiple |  EvilB |   2,119.613 us |    39.8828 us |    55.9101 us |     1.64 |    0.05 |    183.5938 | 7.8125 |     - |   1,145 KB |
|     SudokuSpiceConstraints |  EvilB |     254.160 us |     4.3056 us |     4.0275 us |     0.20 |    0.01 |     25.8789 | 6.3477 |     - |     161 KB |
|                SudokuSharp |  EvilB |  50,789.466 us |   942.7074 us |   835.6856 us |    39.13 |    0.85 |   3200.0000 |      - |     - |  20,167 KB |
|           SudokuSolverLite |  EvilB |  52,743.962 us |   420.7262 us |   351.3254 us |    40.54 |    0.54 |  14100.0000 |      - |     - |  86,910 KB |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|------:|------:|-----------:|
|                SudokuSpice |   Easy |       400.5 us |       3.57 us |       3.34 us |     1.00 |    0.00 |      1.9531 |     - |     - |       8 KB |
|   SudokuSpiceDynamicSingle |   Easy |       597.9 us |       4.93 us |       4.12 us |     1.49 |    0.02 |      2.9297 |     - |     - |      12 KB |
| SudokuSpiceDynamicMultiple |   Easy |       752.0 us |      22.38 us |      20.94 us |     1.88 |    0.05 |      2.9297 |     - |     - |      12 KB |
|     SudokuSpiceConstraints |   Easy |       498.1 us |       4.67 us |       4.36 us |     1.24 |    0.02 |      8.7891 |     - |     - |      37 KB |
|                SudokuSharp |   Easy |       109.7 us |       0.77 us |       0.72 us |     0.27 |    0.00 |      1.2207 |     - |     - |       5 KB |
|           SudokuSolverLite |   Easy |     3,844.3 us |      17.72 us |      14.80 us |     9.60 |    0.08 |     62.5000 |     - |     - |     261 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice | Medium |     5,092.4 us |     125.14 us |     117.06 us |     1.00 |    0.00 |      7.8125 |     - |     - |      48 KB |
|   SudokuSpiceDynamicSingle | Medium |     6,043.0 us |     131.43 us |     122.94 us |     1.19 |    0.04 |      7.8125 |     - |     - |      63 KB |
| SudokuSpiceDynamicMultiple | Medium |     6,599.5 us |     224.41 us |     209.92 us |     1.30 |    0.05 |     15.6250 |     - |     - |      64 KB |
|     SudokuSpiceConstraints | Medium |     1,092.3 us |       9.80 us |       9.16 us |     0.21 |    0.01 |     17.5781 |     - |     - |      74 KB |
|                SudokuSharp | Medium |    51,819.5 us |   1,655.30 us |   1,548.37 us |    10.18 |    0.40 |    100.0000 |     - |     - |     649 KB |
|           SudokuSolverLite | Medium |    55,998.6 us |     665.90 us |     622.89 us |    11.00 |    0.23 |    900.0000 |     - |     - |   3,769 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardA |     4,404.0 us |      77.89 us |      72.86 us |     1.00 |    0.00 |      7.8125 |     - |     - |      41 KB |
|   SudokuSpiceDynamicSingle |  HardA |     5,217.5 us |      88.37 us |      82.66 us |     1.19 |    0.03 |      7.8125 |     - |     - |      57 KB |
| SudokuSpiceDynamicMultiple |  HardA |     5,674.0 us |     106.84 us |      99.94 us |     1.29 |    0.04 |      7.8125 |     - |     - |      56 KB |
|     SudokuSpiceConstraints |  HardA |     1,243.3 us |       9.45 us |       8.83 us |     0.28 |    0.00 |     17.5781 |     - |     - |      71 KB |
|                SudokuSharp |  HardA |    59,487.8 us |  10,563.07 us |   9,363.88 us |    13.55 |    2.25 |    125.0000 |     - |     - |     798 KB |
|           SudokuSolverLite |  HardA |   615,139.7 us |   5,045.05 us |   4,719.15 us |   139.72 |    3.10 |  10000.0000 |     - |     - |  41,347 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardB |     9,489.8 us |     222.95 us |     208.54 us |     1.00 |    0.00 |     15.6250 |     - |     - |     101 KB |
|   SudokuSpiceDynamicSingle |  HardB |    11,101.6 us |     326.82 us |     305.70 us |     1.17 |    0.04 |     31.2500 |     - |     - |     132 KB |
| SudokuSpiceDynamicMultiple |  HardB |    12,143.6 us |     214.93 us |     201.05 us |     1.28 |    0.03 |     31.2500 |     - |     - |     136 KB |
|     SudokuSpiceConstraints |  HardB |     1,509.7 us |      11.93 us |      11.16 us |     0.16 |    0.00 |     17.5781 |     - |     - |      74 KB |
|                SudokuSharp |  HardB |   426,402.6 us | 296,219.10 us | 277,083.53 us |    45.30 |   29.76 |   2000.0000 |     - |     - |  11,028 KB |
|           SudokuSolverLite |  HardB |   114,177.2 us |   1,136.28 us |   1,062.88 us |    12.04 |    0.24 |   1800.0000 |     - |     - |   7,757 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilA |     6,591.1 us |      64.79 us |      60.60 us |     1.00 |    0.00 |     15.6250 |     - |     - |      76 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     7,529.8 us |     150.56 us |     140.83 us |     1.14 |    0.02 |     15.6250 |     - |     - |      94 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     8,054.5 us |     113.26 us |     105.94 us |     1.22 |    0.02 |     15.6250 |     - |     - |      94 KB |
|     SudokuSpiceConstraints |  EvilA |     1,332.9 us |      12.53 us |      11.72 us |     0.20 |    0.00 |     17.5781 |     - |     - |      77 KB |
|                SudokuSharp |  EvilA |   782,988.0 us | 557,682.26 us | 521,656.34 us |   118.73 |   79.08 |   3000.0000 |     - |     - |  16,098 KB |
|           SudokuSolverLite |  EvilA | 9,695,891.8 us |  29,639.89 us |  27,725.17 us | 1,471.17 |   13.30 | 166000.0000 |     - |     - | 674,507 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilB |    61,824.8 us |   9,618.93 us |   8,997.56 us |     1.00 |    0.00 |           - |     - |     - |     374 KB |
|   SudokuSpiceDynamicSingle |  EvilB |    72,677.5 us |  15,861.14 us |  14,836.52 us |     1.18 |    0.24 |    142.8571 |     - |     - |     821 KB |
| SudokuSpiceDynamicMultiple |  EvilB |    85,220.7 us |  25,905.38 us |  24,231.91 us |     1.40 |    0.39 |    166.6667 |     - |     - |     912 KB |
|     SudokuSpiceConstraints |  EvilB |     4,321.3 us |      50.32 us |      42.02 us |     0.07 |    0.01 |     23.4375 |     - |     - |      98 KB |
|                SudokuSharp |  EvilB |   907,502.4 us |  52,872.88 us |  46,870.43 us |    14.94 |    2.66 |   3000.0000 |     - |     - |  14,522 KB |
|           SudokuSolverLite |  EvilB | 1,246,943.2 us |  11,719.73 us |  10,962.65 us |    20.61 |    3.28 |  20000.0000 |     - |     - |  83,411 KB |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |   Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|--------:|------:|----------:|
|            SudokuSpice |  1.266 ms | 0.0155 ms | 0.0137 ms |  1.00 |    0.00 |  152.3438 | 17.5781 |     - |    937 KB |
| SudokuSpiceConstraints |  2.387 ms | 0.0242 ms | 0.0227 ms |  1.88 |    0.02 |  582.0313 | 11.7188 |     - |  3,566 KB |
|     SudokuSharpSingles | 16.355 ms | 0.8730 ms | 2.5327 ms | 13.39 |    2.44 | 2968.7500 |       - |     - | 18,235 KB |
|       SudokuSharpMixed |  7.690 ms | 0.2651 ms | 0.7816 ms |  5.88 |    0.68 | 1289.0625 |  7.8125 |     - |  7,935 KB |

### WASM

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|------:|------:|----------:|
|            SudokuSpice |  57.75 ms |  3.444 ms |  3.222 ms |  1.00 |    0.00 |  125.0000 |     - |     - |    930 KB |
| SudokuSpiceConstraints |  34.64 ms |  1.245 ms |  1.165 ms |  0.60 |    0.04 |  600.0000 |     - |     - |  2,477 KB |
|     SudokuSharpSingles | 216.40 ms | 86.482 ms | 80.895 ms |  3.78 |    1.50 | 2666.6667 |     - |     - | 11,801 KB |
|       SudokuSharpMixed | 120.02 ms | 40.094 ms | 37.504 ms |  2.10 |    0.69 | 1000.0000 |     - |     - |  4,714 KB |

