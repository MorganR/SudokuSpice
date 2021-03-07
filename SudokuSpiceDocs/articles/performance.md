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
|            SudokuSpice |       Guesses: 0 |     22.66 us |     0.014 us |      0.013 us |     22.66 us |   1.00 |    0.00 |    1.4954 |      - |     - |      9 KB |
| SudokuSpiceConstraints |       Guesses: 0 |     89.16 us |     0.162 us |      0.151 us |     89.21 us |   3.93 |    0.01 |   23.6816 | 5.0049 |     - |    145 KB |
|            SudokuSharp |       Guesses: 0 |    770.24 us |    26.631 us |     78.104 us |    761.06 us |  33.50 |    3.74 |  129.8828 |      - |     - |    801 KB |
|       SudokuSolverLite |       Guesses: 0 |  1,286.71 us |    25.628 us |     75.164 us |  1,288.46 us |  56.75 |    2.53 |  369.1406 |      - |     - |  2,263 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |       Guesses: 1 |     37.46 us |     0.133 us |      0.124 us |     37.46 us |   1.00 |    0.00 |    2.9297 | 0.0610 |     - |     18 KB |
| SudokuSpiceConstraints |       Guesses: 1 |     94.15 us |     0.130 us |      0.122 us |     94.16 us |   2.51 |    0.01 |   24.1699 | 5.3711 |     - |    148 KB |
|            SudokuSharp |       Guesses: 1 |  1,722.59 us |    58.679 us |    171.170 us |  1,732.64 us |  47.13 |    6.31 |  240.2344 |      - |     - |  1,480 KB |
|       SudokuSolverLite |       Guesses: 1 |  3,785.12 us |   593.743 us |  1,731.977 us |  4,082.92 us | 103.46 |   50.71 | 1250.0000 |      - |     - |  7,686 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 2-3 |     51.07 us |     0.290 us |      0.271 us |     51.12 us |   1.00 |    0.00 |    4.3335 | 0.0610 |     - |     27 KB |
| SudokuSpiceConstraints |     Guesses: 2-3 |     99.38 us |     0.149 us |      0.140 us |     99.38 us |   1.95 |    0.01 |   24.5361 | 5.6152 |     - |    151 KB |
|            SudokuSharp |     Guesses: 2-3 |  2,614.91 us |   191.305 us |    561.064 us |  2,576.64 us |  53.48 |   10.99 |  359.3750 |      - |     - |  2,210 KB |
|       SudokuSolverLite |     Guesses: 2-3 |  3,950.63 us |   245.017 us |    706.930 us |  3,879.39 us |  76.55 |   13.43 | 1292.9688 | 3.9063 |     - |  7,941 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 4-7 |     71.16 us |     0.443 us |      0.415 us |     71.20 us |   1.00 |    0.00 |    6.4697 | 0.1221 |     - |     40 KB |
| SudokuSpiceConstraints |     Guesses: 4-7 |    105.85 us |     0.258 us |      0.228 us |    105.92 us |   1.49 |    0.01 |   25.1465 | 5.7373 |     - |    155 KB |
|            SudokuSharp |     Guesses: 4-7 |  3,467.36 us |   282.250 us |    827.791 us |  3,316.85 us |  52.70 |   13.27 |  648.4375 |      - |     - |  4,013 KB |
|       SudokuSolverLite |     Guesses: 4-7 |  9,446.98 us | 1,159.626 us |  3,327.185 us |  8,495.25 us | 135.68 |   43.41 | 2921.8750 |      - |     - | 17,936 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |      Guesses: 8+ |     94.69 us |     0.662 us |      0.619 us |     94.73 us |   1.00 |    0.00 |    8.6670 | 0.2441 |     - |     54 KB |
| SudokuSpiceConstraints |      Guesses: 8+ |    114.93 us |     0.261 us |      0.244 us |    114.89 us |   1.21 |    0.01 |   25.8789 | 6.1035 |     - |    159 KB |
|            SudokuSharp |      Guesses: 8+ |  6,079.81 us |   543.924 us |  1,586.650 us |  6,009.56 us |  68.26 |   13.16 | 1093.7500 |      - |     - |  6,705 KB |
|       SudokuSolverLite |      Guesses: 8+ | 23,310.31 us | 3,755.658 us | 11,014.696 us | 22,665.04 us | 301.57 |  111.04 | 4968.7500 |      - |     - | 30,577 KB |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 |  Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|-------:|------:|-----------:|
|                SudokuSpice |   Easy |       9.824 us |     0.1945 us |     0.1725 us |     1.00 |    0.00 |      1.4801 | 0.0153 |     - |       9 KB |
|   SudokuSpiceDynamicSingle |   Easy |      12.013 us |     0.0401 us |     0.0375 us |     1.22 |    0.02 |      1.7090 | 0.0305 |     - |      11 KB |
| SudokuSpiceDynamicMultiple |   Easy |      16.237 us |     0.2777 us |     0.2462 us |     1.65 |    0.03 |      1.7090 | 0.0305 |     - |      11 KB |
|     SudokuSpiceConstraints |   Easy |      34.685 us |     0.3219 us |     0.3011 us |     3.53 |    0.07 |     11.1694 | 1.3428 |     - |      69 KB |
|                SudokuSharp |   Easy |       6.427 us |     0.0078 us |     0.0065 us |     0.65 |    0.01 |      1.1063 |      - |     - |       7 KB |
|           SudokuSolverLite |   Easy |     155.419 us |     0.1853 us |     0.1642 us |    15.82 |    0.27 |     44.4336 |      - |     - |     273 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice | Medium |      85.301 us |     0.3228 us |     0.3020 us |     1.00 |    0.00 |      8.7891 | 0.2441 |     - |      54 KB |
|   SudokuSpiceDynamicSingle | Medium |      96.903 us |     0.2223 us |     0.1971 us |     1.14 |    0.00 |      9.1553 | 0.2441 |     - |      56 KB |
| SudokuSpiceDynamicMultiple | Medium |     109.581 us |     0.2755 us |     0.2577 us |     1.28 |    0.00 |      9.1553 | 0.2441 |     - |      56 KB |
|     SudokuSpiceConstraints | Medium |      99.889 us |     0.0929 us |     0.0869 us |     1.17 |    0.00 |     26.6113 | 6.5918 |     - |     163 KB |
|                SudokuSharp | Medium |   3,002.868 us |     9.4472 us |     8.3747 us |    35.21 |    0.17 |    160.1563 |      - |     - |     991 KB |
|           SudokuSolverLite | Medium |   2,343.198 us |     1.2755 us |     1.0651 us |    27.47 |    0.10 |    664.0625 |      - |     - |   4,088 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardA |      71.609 us |     0.1207 us |     0.1129 us |     1.00 |    0.00 |      7.8125 | 0.1221 |     - |      48 KB |
|   SudokuSpiceDynamicSingle |  HardA |      79.490 us |     0.1622 us |     0.1517 us |     1.11 |    0.00 |      8.0566 | 0.2441 |     - |      50 KB |
| SudokuSpiceDynamicMultiple |  HardA |      91.255 us |     0.1921 us |     0.1604 us |     1.27 |    0.00 |      8.0566 | 0.2441 |     - |      50 KB |
|     SudokuSpiceConstraints |  HardA |     110.583 us |     0.2464 us |     0.2305 us |     1.54 |    0.00 |     25.6348 | 6.4697 |     - |     158 KB |
|                SudokuSharp |  HardA |   3,274.955 us |    63.5959 us |   106.2545 us |    45.76 |    1.81 |    203.1250 |      - |     - |   1,250 KB |
|           SudokuSolverLite |  HardA |  24,681.237 us |    27.1466 us |    25.3930 us |   344.67 |    0.60 |   6937.5000 |      - |     - |  42,554 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardB |     199.406 us |     0.5208 us |     0.4871 us |     1.00 |    0.00 |     20.0195 | 0.7324 |     - |     123 KB |
|   SudokuSpiceDynamicSingle |  HardB |     215.370 us |     0.9822 us |     0.8707 us |     1.08 |    0.00 |     20.2637 | 0.7324 |     - |     125 KB |
| SudokuSpiceDynamicMultiple |  HardB |     240.206 us |     1.2028 us |     1.0662 us |     1.20 |    0.01 |     20.2637 | 0.7324 |     - |     125 KB |
|     SudokuSpiceConstraints |  HardB |     114.090 us |     0.0730 us |     0.0647 us |     0.57 |    0.00 |     25.8789 | 6.3477 |     - |     159 KB |
|                SudokuSharp |  HardB |  23,657.309 us |   824.3865 us | 2,417.7830 us |   122.54 |   16.09 |   1406.2500 |      - |     - |   8,778 KB |
|           SudokuSolverLite |  HardB |   4,755.749 us |     6.3824 us |     5.9701 us |    23.85 |    0.07 |   1335.9375 |      - |     - |   8,220 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilA |     127.922 us |     0.3019 us |     0.2824 us |     1.00 |    0.00 |     14.4043 | 0.4883 |     - |      89 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     137.860 us |     0.3386 us |     0.3167 us |     1.08 |    0.00 |     14.6484 | 0.4883 |     - |      90 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     150.892 us |     0.3959 us |     0.3703 us |     1.18 |    0.00 |     14.6484 | 0.4883 |     - |      90 KB |
|     SudokuSpiceConstraints |  EvilA |     124.607 us |     0.0993 us |     0.0929 us |     0.97 |    0.00 |     27.5879 | 7.0801 |     - |     170 KB |
|                SudokuSharp |  EvilA |  43,209.887 us | 2,469.0041 us | 7,279.9142 us |   359.69 |   52.81 |   2384.6154 |      - |     - |  14,644 KB |
|           SudokuSolverLite |  EvilA | 393,613.660 us |   390.2178 us |   365.0100 us | 3,077.00 |    7.50 | 113000.0000 |      - |     - | 693,267 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilB |   1,208.748 us |    23.9652 us |    32.8039 us |     1.00 |    0.00 |    103.5156 | 3.9063 |     - |     641 KB |
|   SudokuSpiceDynamicSingle |  EvilB |   1,350.394 us |    21.8974 us |    19.4115 us |     1.12 |    0.04 |    109.3750 | 3.9063 |     - |     671 KB |
| SudokuSpiceDynamicMultiple |  EvilB |   1,546.575 us |    30.1595 us |    29.6207 us |     1.28 |    0.03 |    105.4688 | 3.9063 |     - |     646 KB |
|     SudokuSpiceConstraints |  EvilB |     238.573 us |     0.2760 us |     0.2447 us |     0.20 |    0.01 |     31.7383 | 9.0332 |     - |     195 KB |
|                SudokuSharp |  EvilB |  48,241.851 us |   916.3794 us | 1,018.5532 us |    39.96 |    1.44 |   3272.7273 |      - |     - |  20,055 KB |
|           SudokuSolverLite |  EvilB |  50,280.056 us |    74.4647 us |    69.6543 us |    41.68 |    1.15 |  14100.0000 |      - |     - |  86,910 KB |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|------:|------:|-----------:|
|                SudokuSpice |   Easy |       365.6 us |       0.25 us |       0.21 us |     1.00 |    0.00 |      1.9531 |     - |     - |       8 KB |
|   SudokuSpiceDynamicSingle |   Easy |       448.7 us |       0.67 us |       0.63 us |     1.23 |    0.00 |      1.9531 |     - |     - |       9 KB |
| SudokuSpiceDynamicMultiple |   Easy |       561.5 us |       0.79 us |       0.70 us |     1.54 |    0.00 |      1.9531 |     - |     - |       9 KB |
|     SudokuSpiceConstraints |   Easy |       687.7 us |       1.19 us |       1.12 us |     1.88 |    0.00 |     11.7188 |     - |     - |      49 KB |
|                SudokuSharp |   Easy |       104.2 us |       0.16 us |       0.15 us |     0.29 |    0.00 |      1.2207 |     - |     - |       5 KB |
|           SudokuSolverLite |   Easy |     3,664.2 us |       5.04 us |       4.72 us |    10.02 |    0.02 |     62.5000 |     - |     - |     261 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilA |     4,123.8 us |      23.04 us |      19.24 us |     1.00 |    0.00 |     15.6250 |     - |     - |      75 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     4,494.0 us |      33.26 us |      31.11 us |     1.09 |    0.01 |     15.6250 |     - |     - |      78 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     4,878.8 us |      62.80 us |      58.74 us |     1.18 |    0.02 |     15.6250 |     - |     - |      77 KB |
|     SudokuSpiceConstraints |  EvilA |     1,997.4 us |       5.74 us |       5.37 us |     0.48 |    0.00 |     23.4375 |     - |     - |     103 KB |
|                SudokuSharp |  EvilA |   788,101.3 us | 504,319.42 us | 471,740.70 us |   207.28 |  114.93 |   1000.0000 |     - |     - |   6,258 KB |
|           SudokuSolverLite |  EvilA | 9,419,879.0 us |   4,863.15 us |   3,796.83 us | 2,284.63 |   11.32 | 166000.0000 |     - |     - | 674,507 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilB |    37,439.6 us |   5,341.12 us |   4,996.08 us |     1.00 |    0.00 |     83.3333 |     - |     - |     484 KB |
|   SudokuSpiceDynamicSingle |  EvilB |    42,430.6 us |   7,411.05 us |   6,932.30 us |     1.15 |    0.25 |     90.9091 |     - |     - |     518 KB |
| SudokuSpiceDynamicMultiple |  EvilB |    47,348.3 us |   5,335.78 us |   4,730.03 us |     1.28 |    0.21 |     83.3333 |     - |     - |     551 KB |
|     SudokuSpiceConstraints |  EvilB |     4,027.4 us |       2.02 us |       1.79 us |     0.11 |    0.02 |     23.4375 |     - |     - |     117 KB |
|                SudokuSharp |  EvilB |   848,789.2 us |  56,049.91 us |  52,429.12 us |    23.08 |    3.56 |   3000.0000 |     - |     - |  12,975 KB |
|           SudokuSolverLite |  EvilB | 1,154,526.1 us |   1,149.29 us |   1,075.05 us |    31.43 |    4.78 |  20000.0000 |     - |     - |  83,411 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardA |     2,496.5 us |      14.56 us |      13.62 us |     1.00 |    0.00 |      7.8125 |     - |     - |      41 KB |
|   SudokuSpiceDynamicSingle |  HardA |     2,836.4 us |      13.40 us |      11.87 us |     1.14 |    0.01 |      7.8125 |     - |     - |      42 KB |
| SudokuSpiceDynamicMultiple |  HardA |     3,123.3 us |      35.49 us |      31.46 us |     1.25 |    0.01 |      7.8125 |     - |     - |      42 KB |
|     SudokuSpiceConstraints |  HardA |     1,745.4 us |       1.37 us |       1.28 us |     0.70 |    0.00 |     23.4375 |     - |     - |      96 KB |
|                SudokuSharp |  HardA |    55,783.5 us |   6,342.27 us |   5,932.56 us |    22.34 |    2.34 |    125.0000 |     - |     - |     682 KB |
|           SudokuSolverLite |  HardA |   572,795.7 us |     455.66 us |     403.93 us |   229.40 |    1.28 |  10000.0000 |     - |     - |  41,347 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardB |     6,166.9 us |     142.76 us |     133.54 us |     1.00 |    0.00 |     23.4375 |     - |     - |     104 KB |
|   SudokuSpiceDynamicSingle |  HardB |     6,830.4 us |     133.19 us |     124.58 us |     1.11 |    0.02 |     23.4375 |     - |     - |     105 KB |
| SudokuSpiceDynamicMultiple |  HardB |     7,477.3 us |     168.35 us |     157.47 us |     1.21 |    0.04 |     23.4375 |     - |     - |     104 KB |
|     SudokuSpiceConstraints |  HardB |     1,842.4 us |       1.63 us |       1.52 us |     0.30 |    0.01 |     23.4375 |     - |     - |      97 KB |
|                SudokuSharp |  HardB |   358,877.1 us | 235,283.60 us | 220,084.43 us |    57.99 |   35.22 |   1000.0000 |     - |     - |   6,572 KB |
|           SudokuSolverLite |  HardB |   109,284.7 us |     171.51 us |     143.22 us |    17.67 |    0.37 |   1800.0000 |     - |     - |   7,757 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice | Medium |     2,920.1 us |      86.95 us |      81.33 us |     1.00 |    0.00 |      7.8125 |     - |     - |      47 KB |
|   SudokuSpiceDynamicSingle | Medium |     3,217.1 us |      44.94 us |      39.84 us |     1.10 |    0.03 |     11.7188 |     - |     - |      49 KB |
| SudokuSpiceDynamicMultiple | Medium |     3,650.5 us |      69.84 us |      61.91 us |     1.25 |    0.04 |      7.8125 |     - |     - |      47 KB |
|     SudokuSpiceConstraints | Medium |     1,535.2 us |       2.42 us |       2.26 us |     0.53 |    0.01 |     23.4375 |     - |     - |      99 KB |
|                SudokuSharp | Medium |    50,501.8 us |   1,977.03 us |   1,849.31 us |    17.30 |    0.67 |    100.0000 |     - |     - |     671 KB |
|           SudokuSolverLite | Medium |    53,238.8 us |     148.87 us |     139.25 us |    18.24 |    0.49 |    900.0000 |     - |     - |   3,769 KB |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |   Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|--------:|------:|----------:|
|            SudokuSpice |  1.157 ms | 0.0114 ms | 0.0107 ms |  1.00 |    0.00 |  158.2031 | 23.4375 |     - |    979 KB |
| SudokuSpiceConstraints |  2.941 ms | 0.0450 ms | 0.0399 ms |  2.54 |    0.04 |  804.6875 | 27.3438 |     - |  4,943 KB |
|     SudokuSharpSingles | 15.149 ms | 0.5232 ms | 1.5263 ms | 12.93 |    1.07 | 2734.3750 | 15.6250 |     - | 16,832 KB |
|       SudokuSharpMixed |  7.130 ms | 0.2182 ms | 0.6401 ms |  5.91 |    0.53 | 1515.6250 |  7.8125 |     - |  9,301 KB |

### WASM

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|------:|------:|----------:|
|            SudokuSpice |  38.14 ms |  1.405 ms |  1.245 ms |  1.00 |    0.00 |  214.2857 |     - |     - |    918 KB |
| SudokuSpiceConstraints |  50.81 ms |  1.980 ms |  1.546 ms |  1.34 |    0.05 |  800.0000 |     - |     - |  3,333 KB |
|     SudokuSharpSingles | 203.46 ms | 89.236 ms | 83.471 ms |  5.05 |    1.94 | 7000.0000 |     - |     - | 29,222 KB |
|       SudokuSharpMixed | 122.08 ms | 40.915 ms | 38.272 ms |  3.17 |    0.98 | 1200.0000 |     - |     - |  5,414 KB |
