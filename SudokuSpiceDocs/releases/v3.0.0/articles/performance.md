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

|                 Method | sampleCollection |         Mean |        Error |       StdDev |       Median |  Ratio | RatioSD |     Gen 0 |   Gen 1 | Gen 2 | Allocated |
|----------------------- |----------------- |-------------:|-------------:|-------------:|-------------:|-------:|--------:|----------:|--------:|------:|----------:|
|            SudokuSpice |       Guesses: 0 |     22.67 us |     0.017 us |     0.016 us |     22.67 us |   1.00 |    0.00 |    1.4954 |       - |     - |      9 KB |
| SudokuSpiceConstraints |       Guesses: 0 |     88.76 us |     0.136 us |     0.127 us |     88.78 us |   3.91 |    0.01 |   23.6816 |  5.2490 |     - |    145 KB |
|            SudokuSharp |       Guesses: 0 |    774.11 us |    26.053 us |    76.410 us |    768.21 us |  33.48 |    4.00 |  126.9531 |       - |     - |    778 KB |
|       SudokuSolverLite |       Guesses: 0 |  1,270.73 us |    28.276 us |    82.483 us |  1,259.97 us |  55.64 |    3.93 |  320.3125 |       - |     - |  1,965 KB |
|                        |                  |              |              |              |              |        |         |           |         |       |           |
|            SudokuSpice |       Guesses: 1 |     37.27 us |     0.145 us |     0.129 us |     37.32 us |   1.00 |    0.00 |    2.9297 |       - |     - |     18 KB |
| SudokuSpiceConstraints |       Guesses: 1 |     93.94 us |     0.166 us |     0.155 us |     93.93 us |   2.52 |    0.01 |   24.0479 |  5.4932 |     - |    148 KB |
|            SudokuSharp |       Guesses: 1 |  1,574.28 us |    62.673 us |   183.810 us |  1,553.69 us |  42.77 |    5.07 |  259.7656 |       - |     - |  1,597 KB |
|       SudokuSolverLite |       Guesses: 1 |  4,342.40 us |   409.400 us | 1,200.700 us |  4,175.26 us | 109.11 |   35.51 | 1277.3438 |  3.9063 |     - |  7,846 KB |
|                        |                  |              |              |              |              |        |         |           |         |       |           |
|            SudokuSpice |     Guesses: 2-3 |     50.57 us |     0.292 us |     0.273 us |     50.55 us |   1.00 |    0.00 |    4.2725 |  0.0610 |     - |     26 KB |
| SudokuSpiceConstraints |     Guesses: 2-3 |     98.68 us |     0.257 us |     0.240 us |     98.64 us |   1.95 |    0.01 |   24.5361 |  5.4932 |     - |    150 KB |
|            SudokuSharp |     Guesses: 2-3 |  2,634.88 us |   218.501 us |   633.910 us |  2,526.71 us |  54.26 |   13.41 |  527.3438 |       - |     - |  3,240 KB |
|       SudokuSolverLite |     Guesses: 2-3 |  3,879.67 us |   313.016 us |   898.101 us |  3,578.81 us |  69.50 |   13.47 |  750.0000 |       - |     - |  4,629 KB |
|                        |                  |              |              |              |              |        |         |           |         |       |           |
|            SudokuSpice |     Guesses: 4-7 |     71.70 us |     0.698 us |     0.653 us |     71.68 us |   1.00 |    0.00 |    6.3477 |  0.1221 |     - |     40 KB |
| SudokuSpiceConstraints |     Guesses: 4-7 |    106.00 us |     0.294 us |     0.275 us |    106.04 us |   1.48 |    0.01 |   25.1465 |  5.8594 |     - |    155 KB |
|            SudokuSharp |     Guesses: 4-7 |  3,699.21 us |   298.292 us |   874.838 us |  3,553.09 us |  51.01 |   13.27 |  863.2813 |       - |     - |  5,307 KB |
|       SudokuSolverLite |     Guesses: 4-7 |  9,769.83 us | 1,275.071 us | 3,739.562 us |  9,274.07 us | 141.26 |   44.74 | 4281.2500 | 15.6250 |     - | 26,298 KB |
|                        |                  |              |              |              |              |        |         |           |         |       |           |
|            SudokuSpice |      Guesses: 8+ |     94.35 us |     1.151 us |     1.077 us |     94.45 us |   1.00 |    0.00 |    8.5449 |  0.2441 |     - |     53 KB |
| SudokuSpiceConstraints |      Guesses: 8+ |    115.13 us |     0.363 us |     0.340 us |    115.15 us |   1.22 |    0.01 |   25.8789 |  6.1035 |     - |    159 KB |
|            SudokuSharp |      Guesses: 8+ |  6,168.38 us |   359.206 us | 1,036.390 us |  6,048.65 us |  65.77 |    9.72 | 1023.4375 |       - |     - |  6,279 KB |
|       SudokuSolverLite |      Guesses: 8+ | 20,940.05 us | 2,777.844 us | 8,103.101 us | 20,235.73 us | 205.43 |   57.29 | 4468.7500 |       - |     - | 27,515 KB |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 |  Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|-------:|------:|-----------:|
|                SudokuSpice |   Easy |       9.713 us |     0.0177 us |     0.0148 us |     1.00 |    0.00 |      1.4801 | 0.0153 |     - |       9 KB |
|   SudokuSpiceDynamicSingle |   Easy |      11.994 us |     0.0136 us |     0.0120 us |     1.23 |    0.00 |      1.7090 | 0.0305 |     - |      11 KB |
| SudokuSpiceDynamicMultiple |   Easy |      15.442 us |     0.0155 us |     0.0137 us |     1.59 |    0.00 |      1.7090 | 0.0305 |     - |      11 KB |
|     SudokuSpiceConstraints |   Easy |      33.793 us |     0.1402 us |     0.1311 us |     3.48 |    0.01 |     11.1694 | 1.2817 |     - |      69 KB |
|                SudokuSharp |   Easy |       6.441 us |     0.0123 us |     0.0109 us |     0.66 |    0.00 |      1.1063 |      - |     - |       7 KB |
|           SudokuSolverLite |   Easy |     154.272 us |     0.1981 us |     0.1547 us |    15.88 |    0.03 |     44.4336 |      - |     - |     273 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice | Medium |      84.061 us |     0.2744 us |     0.2567 us |     1.00 |    0.00 |      8.7891 | 0.2441 |     - |      55 KB |
|   SudokuSpiceDynamicSingle | Medium |      97.477 us |     0.4174 us |     0.3904 us |     1.16 |    0.00 |      9.1553 | 0.2441 |     - |      56 KB |
| SudokuSpiceDynamicMultiple | Medium |     111.392 us |     0.3579 us |     0.3347 us |     1.33 |    0.01 |      9.1553 | 0.2441 |     - |      56 KB |
|     SudokuSpiceConstraints | Medium |      98.888 us |     0.0318 us |     0.0266 us |     1.18 |    0.00 |     26.6113 | 6.5918 |     - |     163 KB |
|                SudokuSharp | Medium |   3,006.681 us |    20.1332 us |    18.8326 us |    35.77 |    0.26 |    160.1563 |      - |     - |     988 KB |
|           SudokuSolverLite | Medium |   2,298.774 us |     1.1090 us |     0.9261 us |    27.35 |    0.08 |    664.0625 |      - |     - |   4,088 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardA |      71.560 us |     0.0918 us |     0.0859 us |     1.00 |    0.00 |      7.8125 | 0.1221 |     - |      48 KB |
|   SudokuSpiceDynamicSingle |  HardA |      80.064 us |     0.1294 us |     0.1210 us |     1.12 |    0.00 |      8.0566 | 0.2441 |     - |      50 KB |
| SudokuSpiceDynamicMultiple |  HardA |      92.260 us |     0.1967 us |     0.1839 us |     1.29 |    0.00 |      8.0566 | 0.2441 |     - |      50 KB |
|     SudokuSpiceConstraints |  HardA |     109.537 us |     0.1262 us |     0.1180 us |     1.53 |    0.00 |     25.6348 | 6.4697 |     - |     157 KB |
|                SudokuSharp |  HardA |   3,245.299 us |    64.1876 us |    99.9323 us |    45.47 |    1.62 |    191.4063 |      - |     - |   1,173 KB |
|           SudokuSolverLite |  HardA |  24,671.515 us |    23.3827 us |    21.8722 us |   344.77 |    0.53 |   6937.5000 |      - |     - |  42,554 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardB |     199.689 us |     0.3651 us |     0.3236 us |     1.00 |    0.00 |     20.0195 | 0.7324 |     - |     123 KB |
|   SudokuSpiceDynamicSingle |  HardB |     215.495 us |     0.8909 us |     0.7440 us |     1.08 |    0.00 |     20.2637 | 0.7324 |     - |     125 KB |
| SudokuSpiceDynamicMultiple |  HardB |     238.336 us |     1.1611 us |     1.0861 us |     1.19 |    0.01 |     20.2637 | 0.7324 |     - |     125 KB |
|     SudokuSpiceConstraints |  HardB |     113.526 us |     0.0671 us |     0.0594 us |     0.57 |    0.00 |     25.7568 | 6.2256 |     - |     159 KB |
|                SudokuSharp |  HardB |  22,920.176 us |   820.1723 us | 2,405.4232 us |   112.84 |   13.28 |   1250.0000 |      - |     - |   7,780 KB |
|           SudokuSolverLite |  HardB |   4,768.276 us |     2.0039 us |     1.6733 us |    23.88 |    0.04 |   1335.9375 |      - |     - |   8,220 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilA |     126.716 us |     0.2533 us |     0.2369 us |     1.00 |    0.00 |     14.4043 | 0.4883 |     - |      89 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     138.105 us |     0.3467 us |     0.2895 us |     1.09 |    0.00 |     14.6484 | 0.4883 |     - |      90 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     150.227 us |     0.3722 us |     0.3482 us |     1.19 |    0.00 |     14.6484 | 0.4883 |     - |      90 KB |
|     SudokuSpiceConstraints |  EvilA |     125.953 us |     0.2539 us |     0.2120 us |     0.99 |    0.00 |     27.5879 | 7.5684 |     - |     170 KB |
|                SudokuSharp |  EvilA |  44,063.923 us | 2,580.4848 us | 7,608.6178 us |   366.66 |   65.88 |   2090.9091 |      - |     - |  13,025 KB |
|           SudokuSolverLite |  EvilA | 397,462.669 us |   449.1198 us |   398.1330 us | 3,137.10 |    5.56 | 113000.0000 |      - |     - | 693,267 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilB |   1,209.084 us |    23.2604 us |    23.8867 us |     1.00 |    0.00 |    101.5625 | 3.9063 |     - |     625 KB |
|   SudokuSpiceDynamicSingle |  EvilB |   1,324.806 us |    25.6096 us |    28.4650 us |     1.09 |    0.04 |    107.4219 | 3.9063 |     - |     664 KB |
| SudokuSpiceDynamicMultiple |  EvilB |   1,565.596 us |    31.2552 us |    29.2362 us |     1.29 |    0.04 |    107.4219 | 3.9063 |     - |     662 KB |
|     SudokuSpiceConstraints |  EvilB |     237.610 us |     0.4581 us |     0.4061 us |     0.20 |    0.00 |     31.7383 | 8.0566 |     - |     195 KB |
|                SudokuSharp |  EvilB |  48,037.180 us |   883.1826 us |   826.1296 us |    39.69 |    1.29 |   3272.7273 |      - |     - |  20,315 KB |
|           SudokuSolverLite |  EvilB |  50,279.549 us |    56.9511 us |    53.2721 us |    41.53 |    0.85 |  14100.0000 |      - |     - |  86,909 KB |

### WASM

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

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |   Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|--------:|------:|----------:|
|            SudokuSpice |  1.167 ms | 0.0116 ms | 0.0108 ms |  1.00 |    0.00 |  160.1563 | 23.4375 |     - |    992 KB |
| SudokuSpiceConstraints |  2.948 ms | 0.0561 ms | 0.0524 ms |  2.53 |    0.05 |  789.0625 | 11.7188 |     - |  4,847 KB |
|     SudokuSharpSingles | 15.468 ms | 0.7212 ms | 2.1265 ms | 13.50 |    1.56 | 3375.0000 | 15.6250 |     - | 20,744 KB |
|       SudokuSharpMixed |  7.233 ms | 0.2211 ms | 0.6484 ms |  6.31 |    0.51 | 1742.1875 | 15.6250 |     - | 10,674 KB |

### WASM

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|------:|------:|----------:|
|            SudokuSpice |  38.25 ms |  1.647 ms |  1.460 ms |  1.00 |    0.00 |  153.8462 |     - |     - |    837 KB |
| SudokuSpiceConstraints |  50.06 ms |  2.581 ms |  2.415 ms |  1.31 |    0.10 |  800.0000 |     - |     - |  3,498 KB |
|     SudokuSharpSingles | 217.88 ms | 87.680 ms | 73.217 ms |  5.73 |    1.97 | 2500.0000 |     - |     - | 10,695 KB |
|       SudokuSharpMixed | 137.09 ms | 48.759 ms | 45.609 ms |  3.72 |    1.23 | 1600.0000 |     - |     - |  6,868 KB |
