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
|            SudokuSpice |       Guesses: 0 |     26.84 us |     0.131 us |      0.123 us |     26.86 us |   1.00 |    0.00 |    1.8311 | 0.0305 |     - |     11 KB |
| SudokuSpiceConstraints |       Guesses: 0 |     74.92 us |     0.275 us |      0.244 us |     74.96 us |   2.79 |    0.02 |   17.9443 | 3.7842 |     - |    110 KB |
|            SudokuSharp |       Guesses: 0 |    814.24 us |    28.382 us |     83.240 us |    811.41 us |  30.66 |    2.96 |  151.3672 |      - |     - |    930 KB |
|       SudokuSolverLite |       Guesses: 0 |  1,358.86 us |    30.407 us |     89.655 us |  1,344.54 us |  49.48 |    2.80 |  371.0938 |      - |     - |  2,279 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |       Guesses: 1 |     42.55 us |     0.339 us |      0.317 us |     42.54 us |   1.00 |    0.00 |    3.2349 | 0.0610 |     - |     20 KB |
| SudokuSpiceConstraints |       Guesses: 1 |     79.57 us |     0.785 us |      0.734 us |     79.88 us |   1.87 |    0.02 |   18.4326 | 3.9063 |     - |    113 KB |
|            SudokuSharp |       Guesses: 1 |  1,673.89 us |    62.515 us |    183.346 us |  1,670.05 us |  41.23 |    3.98 |  314.4531 |      - |     - |  1,938 KB |
|       SudokuSolverLite |       Guesses: 1 |  4,113.72 us |   532.584 us |  1,553.571 us |  3,818.52 us | 100.76 |   39.71 | 1320.3125 |      - |     - |  8,133 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 2-3 |     58.04 us |     0.665 us |      0.622 us |     58.04 us |   1.00 |    0.00 |    4.5776 | 0.1221 |     - |     28 KB |
| SudokuSpiceConstraints |     Guesses: 2-3 |     82.55 us |     0.604 us |      0.565 us |     82.52 us |   1.42 |    0.02 |   18.6768 | 3.9063 |     - |    115 KB |
|            SudokuSharp |     Guesses: 2-3 |  2,952.14 us |   163.970 us |    475.706 us |  2,953.88 us |  51.08 |    8.32 |  507.8125 | 1.9531 |     - |  3,115 KB |
|       SudokuSolverLite |     Guesses: 2-3 |  4,344.88 us |   305.090 us |    899.564 us |  4,184.28 us |  77.63 |   16.78 |  847.6563 |      - |     - |  5,193 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 4-7 |     79.71 us |     1.027 us |      0.961 us |     79.96 us |   1.00 |    0.00 |    6.8359 | 0.1221 |     - |     42 KB |
| SudokuSpiceConstraints |     Guesses: 4-7 |     88.87 us |     1.572 us |      1.470 us |     88.82 us |   1.12 |    0.02 |   19.2871 | 4.0283 |     - |    119 KB |
|            SudokuSharp |     Guesses: 4-7 |  3,919.40 us |   356.710 us |  1,040.541 us |  3,633.28 us |  47.33 |   12.23 |  539.0625 |      - |     - |  3,310 KB |
|       SudokuSolverLite |     Guesses: 4-7 | 10,661.37 us | 2,045.862 us |  5,967.876 us |  8,582.24 us | 125.92 |   84.27 | 3156.2500 |      - |     - | 19,440 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |      Guesses: 8+ |    102.97 us |     0.700 us |      0.655 us |    102.92 us |   1.00 |    0.00 |    8.9111 | 0.2441 |     - |     55 KB |
| SudokuSpiceConstraints |      Guesses: 8+ |     95.36 us |     0.861 us |      0.806 us |     95.53 us |   0.93 |    0.01 |   19.8975 | 4.2725 |     - |    122 KB |
|            SudokuSharp |      Guesses: 8+ |  6,690.20 us |   532.946 us |  1,563.038 us |  6,650.32 us |  68.67 |   17.39 |  890.6250 |      - |     - |  5,515 KB |
|       SudokuSolverLite |      Guesses: 8+ | 23,936.57 us | 3,651.514 us | 10,651.639 us | 22,533.73 us | 220.91 |   92.66 | 2218.7500 |      - |     - | 13,739 KB |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 |  Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|-------:|------:|-----------:|
|                SudokuSpice |   Easy |      11.094 us |     0.0715 us |     0.0669 us |     1.00 |    0.00 |      1.3428 | 0.0153 |     - |       8 KB |
|   SudokuSpiceDynamicSingle |   Easy |      17.391 us |     0.1877 us |     0.1664 us |     1.57 |    0.02 |      2.4719 | 0.0305 |     - |      15 KB |
| SudokuSpiceDynamicMultiple |   Easy |      23.179 us |     0.0567 us |     0.0473 us |     2.09 |    0.01 |      2.4719 | 0.0305 |     - |      15 KB |
|     SudokuSpiceConstraints |   Easy |      26.942 us |     0.2178 us |     0.2038 us |     2.43 |    0.03 |      8.0566 | 0.8545 |     - |      49 KB |
|                SudokuSharp |   Easy |       6.874 us |     0.0507 us |     0.0450 us |     0.62 |    0.01 |      1.1063 |      - |     - |       7 KB |
|           SudokuSolverLite |   Easy |     164.221 us |     1.5490 us |     1.3732 us |    14.80 |    0.17 |     44.4336 |      - |     - |     273 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilA |     145.206 us |     0.3663 us |     0.3427 us |     1.00 |    0.00 |     14.1602 | 0.4883 |     - |      88 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     170.151 us |     2.0649 us |     1.8304 us |     1.17 |    0.01 |     18.7988 | 0.7324 |     - |     117 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     199.606 us |     1.8107 us |     1.6937 us |     1.37 |    0.01 |     18.7988 | 0.7324 |     - |     117 KB |
|     SudokuSpiceConstraints |  EvilA |      96.017 us |     0.8496 us |     0.7947 us |     0.66 |    0.01 |     21.2402 | 5.2490 |     - |     130 KB |
|                SudokuSharp |  EvilA |  45,666.760 us | 2,736.3454 us | 8,068.1761 us |   309.81 |   63.46 |   2000.0000 |      - |     - |  12,696 KB |
|           SudokuSolverLite |  EvilA | 407,669.804 us | 4,244.9577 us | 3,970.7361 us | 2,807.56 |   30.16 | 113000.0000 |      - |     - | 693,267 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilB |   1,313.052 us |    24.9716 us |    31.5811 us |     1.00 |    0.00 |    105.4688 | 3.9063 |     - |     654 KB |
|   SudokuSpiceDynamicSingle |  EvilB |   1,772.182 us |    35.1850 us |    44.4978 us |     1.35 |    0.06 |    177.7344 | 7.8125 |     - |   1,095 KB |
| SudokuSpiceDynamicMultiple |  EvilB |   2,169.383 us |    42.8788 us |    70.4511 us |     1.65 |    0.08 |    179.6875 | 7.8125 |     - |   1,115 KB |
|     SudokuSpiceConstraints |  EvilB |     250.591 us |     2.2559 us |     1.9998 us |     0.19 |    0.01 |     26.3672 | 6.8359 |     - |     162 KB |
|                SudokuSharp |  EvilB |  51,329.966 us |   942.7404 us | 1,575.1074 us |    38.97 |    1.72 |   3300.0000 |      - |     - |  20,704 KB |
|           SudokuSolverLite |  EvilB |  51,859.081 us |   590.9969 us |   523.9034 us |    39.40 |    0.99 |  14100.0000 |      - |     - |  86,909 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardA |      78.703 us |     0.6746 us |     0.5980 us |     1.00 |    0.00 |      7.6904 | 0.1221 |     - |      47 KB |
|   SudokuSpiceDynamicSingle |  HardA |     104.674 us |     0.7923 us |     0.6616 us |     1.33 |    0.01 |     11.8408 | 0.3662 |     - |      73 KB |
| SudokuSpiceDynamicMultiple |  HardA |     126.180 us |     1.4945 us |     1.3979 us |     1.60 |    0.03 |     11.7188 | 0.2441 |     - |      72 KB |
|     SudokuSpiceConstraints |  HardA |      87.944 us |     1.7350 us |     2.0654 us |     1.12 |    0.03 |     19.4092 | 4.7607 |     - |     119 KB |
|                SudokuSharp |  HardA |   3,621.153 us |    67.6003 us |   105.2456 us |    45.99 |    1.62 |    183.5938 |      - |     - |   1,130 KB |
|           SudokuSolverLite |  HardA |  25,528.954 us |   190.4002 us |   178.1005 us |   324.46 |    3.40 |   6937.5000 |      - |     - |  42,554 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardB |     218.104 us |     1.4096 us |     1.1771 us |     1.00 |    0.00 |     19.7754 | 0.7324 |     - |     122 KB |
|   SudokuSpiceDynamicSingle |  HardB |     269.551 us |     5.2520 us |     5.8375 us |     1.24 |    0.03 |     28.8086 | 0.9766 |     - |     177 KB |
| SudokuSpiceDynamicMultiple |  HardB |     321.970 us |     2.0660 us |     1.8315 us |     1.48 |    0.01 |     28.3203 | 0.9766 |     - |     175 KB |
|     SudokuSpiceConstraints |  HardB |      97.843 us |     0.6425 us |     0.6010 us |     0.45 |    0.00 |     20.1416 | 4.8828 |     - |     124 KB |
|                SudokuSharp |  HardB |  25,493.330 us |   863.7710 us | 2,546.8483 us |   113.97 |   10.03 |   1281.2500 |      - |     - |   7,967 KB |
|           SudokuSolverLite |  HardB |   4,944.115 us |    33.6732 us |    31.4979 us |    22.69 |    0.18 |   1335.9375 |      - |     - |   8,220 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice | Medium |      93.770 us |     0.4170 us |     0.3900 us |     1.00 |    0.00 |      8.6670 | 0.1221 |     - |      54 KB |
|   SudokuSpiceDynamicSingle | Medium |     126.187 us |     1.0390 us |     0.9719 us |     1.35 |    0.01 |     13.6719 | 0.2441 |     - |      85 KB |
| SudokuSpiceDynamicMultiple | Medium |     150.346 us |     0.9586 us |     0.8967 us |     1.60 |    0.01 |     13.6719 | 0.2441 |     - |      84 KB |
|     SudokuSpiceConstraints | Medium |      81.621 us |     0.7625 us |     0.6759 us |     0.87 |    0.01 |     20.2637 | 4.8828 |     - |     124 KB |
|                SudokuSharp | Medium |   3,213.069 us |    38.1995 us |    35.7319 us |    34.27 |    0.42 |    160.1563 |      - |     - |     990 KB |
|           SudokuSolverLite | Medium |   2,465.907 us |    22.0120 us |    20.5900 us |    26.30 |    0.27 |    664.0625 |      - |     - |   4,088 KB |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |            Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |----------------:|--------------:|--------------:|---------:|--------:|------------:|------:|------:|-----------:|
|                SudokuSpice |   Easy |        419.0 us |       3.34 us |       3.13 us |     1.00 |    0.00 |      1.4648 |     - |     - |       7 KB |
|   SudokuSpiceDynamicSingle |   Easy |        597.7 us |       3.79 us |       3.55 us |     1.43 |    0.01 |      2.9297 |     - |     - |      12 KB |
| SudokuSpiceDynamicMultiple |   Easy |        740.8 us |       5.80 us |       5.43 us |     1.77 |    0.02 |      2.9297 |     - |     - |      12 KB |
|     SudokuSpiceConstraints |   Easy |        504.2 us |       3.77 us |       3.53 us |     1.20 |    0.01 |      8.7891 |     - |     - |      38 KB |
|                SudokuSharp |   Easy |        109.4 us |       0.85 us |       0.79 us |     0.26 |    0.00 |      1.2207 |     - |     - |       5 KB |
|           SudokuSolverLite |   Easy |      3,810.8 us |      19.71 us |      18.43 us |     9.10 |    0.08 |     62.5000 |     - |     - |     261 KB |
|                            |        |                 |               |               |          |         |             |       |       |            |
|                SudokuSpice | Medium |      4,881.5 us |      94.27 us |      88.18 us |     1.00 |    0.00 |      7.8125 |     - |     - |      46 KB |
|   SudokuSpiceDynamicSingle | Medium |      5,728.6 us |     113.09 us |     105.79 us |     1.17 |    0.03 |     15.6250 |     - |     - |      66 KB |
| SudokuSpiceDynamicMultiple | Medium |      6,287.4 us |     105.24 us |      98.44 us |     1.29 |    0.03 |     15.6250 |     - |     - |      64 KB |
|     SudokuSpiceConstraints | Medium |      1,021.9 us |       1.09 us |       1.02 us |     0.21 |    0.00 |     17.5781 |     - |     - |      75 KB |
|                SudokuSharp | Medium |     51,126.1 us |   1,103.59 us |   1,032.30 us |    10.48 |    0.29 |    100.0000 |     - |     - |     645 KB |
|           SudokuSolverLite | Medium |     53,433.0 us |      64.94 us |      60.74 us |    10.95 |    0.20 |    900.0000 |     - |     - |   3,769 KB |
|                            |        |                 |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardA |      4,470.9 us |      87.04 us |      81.42 us |     1.00 |    0.00 |      7.8125 |     - |     - |      41 KB |
|   SudokuSpiceDynamicSingle |  HardA |      5,314.0 us |      68.26 us |      63.85 us |     1.19 |    0.02 |      7.8125 |     - |     - |      56 KB |
| SudokuSpiceDynamicMultiple |  HardA |      5,658.5 us |     110.84 us |     103.68 us |     1.27 |    0.02 |      7.8125 |     - |     - |      55 KB |
|     SudokuSpiceConstraints |  HardA |      1,223.3 us |       7.38 us |       6.90 us |     0.27 |    0.00 |     17.5781 |     - |     - |      73 KB |
|                SudokuSharp |  HardA |     53,423.4 us |   9,163.37 us |   8,123.09 us |    11.96 |    1.84 |    100.0000 |     - |     - |     669 KB |
|           SudokuSolverLite |  HardA |    583,024.1 us |     443.76 us |     393.38 us |   130.53 |    2.48 |  10000.0000 |     - |     - |  41,347 KB |
|                            |        |                 |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardB |      9,049.8 us |     238.31 us |     222.91 us |     1.00 |    0.00 |     15.6250 |     - |     - |     103 KB |
|   SudokuSpiceDynamicSingle |  HardB |     10,558.7 us |     235.63 us |     208.88 us |     1.17 |    0.04 |     31.2500 |     - |     - |     139 KB |
| SudokuSpiceDynamicMultiple |  HardB |     11,375.4 us |     206.23 us |     182.82 us |     1.26 |    0.04 |     31.2500 |     - |     - |     136 KB |
|     SudokuSpiceConstraints |  HardB |      1,412.2 us |       0.75 us |       0.66 us |     0.16 |    0.00 |     17.5781 |     - |     - |      75 KB |
|                SudokuSharp |  HardB |    380,083.0 us | 282,839.42 us | 264,568.17 us |    42.10 |   29.28 |           - |     - |     - |   2,050 KB |
|           SudokuSolverLite |  HardB |    109,067.3 us |      81.12 us |      71.91 us |    12.09 |    0.28 |   1800.0000 |     - |     - |   7,757 KB |
|                            |        |                 |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilA |      6,607.4 us |     115.15 us |     107.72 us |     1.00 |    0.00 |     15.6250 |     - |     - |      76 KB |
|   SudokuSpiceDynamicSingle |  EvilA |      7,466.7 us |      78.77 us |      69.82 us |     1.13 |    0.02 |     15.6250 |     - |     - |      94 KB |
| SudokuSpiceDynamicMultiple |  EvilA |      7,960.7 us |     140.28 us |     131.22 us |     1.21 |    0.03 |     15.6250 |     - |     - |      92 KB |
|     SudokuSpiceConstraints |  EvilA |      1,315.7 us |      11.05 us |      10.34 us |     0.20 |    0.00 |     17.5781 |     - |     - |      78 KB |
|                SudokuSharp |  EvilA |    776,257.5 us | 342,901.02 us | 320,749.83 us |   117.43 |   48.39 |   1500.0000 |     - |     - |   7,957 KB |
|           SudokuSolverLite |  EvilA | 10,018,056.1 us |  42,123.14 us |  39,402.01 us | 1,516.52 |   23.12 | 166000.0000 |     - |     - | 674,507 KB |
|                            |        |                 |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilB |     59,140.5 us |  12,553.19 us |  11,742.26 us |     1.00 |    0.00 |           - |     - |     - |     473 KB |
|   SudokuSpiceDynamicSingle |  EvilB |     75,325.8 us |  12,950.08 us |  12,113.52 us |     1.30 |    0.23 |    125.0000 |     - |     - |     808 KB |
| SudokuSpiceDynamicMultiple |  EvilB |     85,161.2 us |  21,082.87 us |  19,720.93 us |     1.49 |    0.44 |    200.0000 |     - |     - |     843 KB |
|     SudokuSpiceConstraints |  EvilB |      4,295.0 us |      17.19 us |      14.35 us |     0.07 |    0.01 |     23.4375 |     - |     - |      99 KB |
|                SudokuSharp |  EvilB |    933,310.7 us |  70,019.65 us |  65,496.43 us |    16.39 |    3.54 |   3000.0000 |     - |     - |  13,807 KB |
|           SudokuSolverLite |  EvilB |  1,212,980.0 us |  13,242.46 us |  12,387.00 us |    21.30 |    4.34 |  20000.0000 |     - |     - |  83,411 KB |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |   Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|--------:|------:|----------:|
|            SudokuSpice |  1.275 ms | 0.0168 ms | 0.0157 ms |  1.00 |    0.00 |  150.3906 | 19.5313 |     - |    928 KB |
| SudokuSpiceConstraints |  2.429 ms | 0.0416 ms | 0.0389 ms |  1.90 |    0.03 |  585.9375 | 31.2500 |     - |  3,613 KB |
|     SudokuSharpSingles | 15.695 ms | 0.8991 ms | 2.6228 ms | 12.61 |    1.93 | 3062.5000 |       - |     - | 18,787 KB |
|       SudokuSharpMixed |  7.664 ms | 0.2563 ms | 0.7557 ms |  6.17 |    0.61 | 1585.9375 |  7.8125 |     - |  9,737 KB |

### WASM

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|------:|------:|----------:|
|            SudokuSpice |  56.60 ms |  2.783 ms |  2.603 ms |  1.00 |    0.00 |  222.2222 |     - |     - |    897 KB |
| SudokuSpiceConstraints |  33.19 ms |  0.763 ms |  0.676 ms |  0.59 |    0.03 |  625.0000 |     - |     - |  2,552 KB |
|     SudokuSharpSingles | 212.02 ms | 79.983 ms | 74.816 ms |  3.75 |    1.35 | 2000.0000 |     - |     - | 10,134 KB |
|       SudokuSharpMixed | 118.34 ms | 27.601 ms | 25.818 ms |  2.09 |    0.45 | 1000.0000 |     - |     - |  4,297 KB |

