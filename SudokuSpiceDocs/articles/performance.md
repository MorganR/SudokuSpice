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

|                 Method | sampleCollection |         Mean |        Error |       StdDev |  Ratio | RatioSD |     Gen 0 |   Gen 1 | Gen 2 | Allocated |
|----------------------- |----------------- |-------------:|-------------:|-------------:|-------:|--------:|----------:|--------:|------:|----------:|
|            SudokuSpice |       Guesses: 0 |     25.03 us |     0.041 us |     0.039 us |   1.00 |    0.00 |    1.8311 |  0.0305 |     - |     11 KB |
| SudokuSpiceConstraints |       Guesses: 0 |     71.84 us |     0.234 us |     0.219 us |   2.87 |    0.01 |   17.9443 |  3.5400 |     - |    110 KB |
|            SudokuSharp |       Guesses: 0 |    817.31 us |    28.791 us |    84.440 us |  31.64 |    2.59 |  129.8828 |       - |     - |    799 KB |
|       SudokuSolverLite |       Guesses: 0 |  1,256.10 us |    25.975 us |    76.180 us |  51.01 |    2.76 |  341.7969 |       - |     - |  2,099 KB |
|                        |                  |              |              |              |        |         |           |         |       |           |
|            SudokuSpice |       Guesses: 1 |     40.13 us |     0.182 us |     0.171 us |   1.00 |    0.00 |    3.2349 |  0.0610 |     - |     20 KB |
| SudokuSpiceConstraints |       Guesses: 1 |     74.93 us |     0.129 us |     0.120 us |   1.87 |    0.01 |   18.4326 |  3.7842 |     - |    113 KB |
|            SudokuSharp |       Guesses: 1 |  1,583.04 us |    65.179 us |   192.182 us |  38.93 |    5.07 |  261.7188 |       - |     - |  1,607 KB |
|       SudokuSolverLite |       Guesses: 1 |  4,445.44 us |   794.736 us | 2,330.822 us | 120.03 |   58.30 |  640.6250 |       - |     - |  3,992 KB |
|                        |                  |              |              |              |        |         |           |         |       |           |
|            SudokuSpice |     Guesses: 2-3 |     53.66 us |     0.201 us |     0.188 us |   1.00 |    0.00 |    4.5776 |  0.1221 |     - |     28 KB |
| SudokuSpiceConstraints |     Guesses: 2-3 |     77.67 us |     0.197 us |     0.184 us |   1.45 |    0.01 |   18.6768 |  3.9063 |     - |    115 KB |
|            SudokuSharp |     Guesses: 2-3 |  2,628.13 us |   198.191 us |   571.828 us |  48.53 |    7.79 |  546.8750 |       - |     - |  3,364 KB |
|       SudokuSolverLite |     Guesses: 2-3 |  4,038.79 us |   267.879 us |   785.643 us |  76.61 |   12.71 | 1082.0313 |  3.9063 |     - |  6,646 KB |
|                        |                  |              |              |              |        |         |           |         |       |           |
|            SudokuSpice |     Guesses: 4-7 |     75.39 us |     0.483 us |     0.428 us |   1.00 |    0.00 |    6.7139 |  0.1221 |     - |     41 KB |
| SudokuSpiceConstraints |     Guesses: 4-7 |     83.06 us |     0.109 us |     0.097 us |   1.10 |    0.01 |   19.2871 |  4.1504 |     - |    119 KB |
|            SudokuSharp |     Guesses: 4-7 |  3,630.18 us |   344.792 us | 1,000.304 us |  51.77 |   16.49 |  468.7500 |       - |     - |  2,903 KB |
|       SudokuSolverLite |     Guesses: 4-7 | 10,177.38 us | 1,340.705 us | 3,932.056 us | 132.94 |   50.50 | 3046.8750 |       - |     - | 18,703 KB |
|                        |                  |              |              |              |        |         |           |         |       |           |
|            SudokuSpice |      Guesses: 8+ |     98.17 us |     0.722 us |     0.675 us |   1.00 |    0.00 |    8.9111 |  0.2441 |     - |     55 KB |
| SudokuSpiceConstraints |      Guesses: 8+ |     88.62 us |     0.179 us |     0.158 us |   0.90 |    0.01 |   19.8975 |  4.2725 |     - |    122 KB |
|            SudokuSharp |      Guesses: 8+ |  6,039.31 us |   594.250 us | 1,733.454 us |  69.71 |   23.63 | 1500.0000 |       - |     - |  9,210 KB |
|       SudokuSolverLite |      Guesses: 8+ | 20,206.10 us | 2,331.000 us | 6,688.075 us | 193.89 |   60.70 | 9578.1250 | 31.2500 |     - | 58,743 KB |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 |  Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|-------:|------:|-----------:|
|                SudokuSpice |   Easy |      10.216 us |     0.0106 us |     0.0100 us |     1.00 |    0.00 |      1.3428 | 0.0153 |     - |       8 KB |
|   SudokuSpiceDynamicSingle |   Easy |      16.428 us |     0.0361 us |     0.0302 us |     1.61 |    0.00 |      2.4719 | 0.0305 |     - |      15 KB |
| SudokuSpiceDynamicMultiple |   Easy |      22.198 us |     0.0304 us |     0.0285 us |     2.17 |    0.00 |      2.4719 | 0.0305 |     - |      15 KB |
|     SudokuSpiceConstraints |   Easy |      25.553 us |     0.0324 us |     0.0287 us |     2.50 |    0.00 |      8.0566 | 0.9460 |     - |      50 KB |
|                SudokuSharp |   Easy |       6.484 us |     0.0114 us |     0.0095 us |     0.63 |    0.00 |      1.1063 |      - |     - |       7 KB |
|           SudokuSolverLite |   Easy |     153.717 us |     0.1755 us |     0.1642 us |    15.05 |    0.02 |     44.4336 |      - |     - |     273 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilA |     131.611 us |     0.2777 us |     0.2168 us |     1.00 |    0.00 |     14.1602 | 0.4883 |     - |      88 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     162.119 us |     0.3180 us |     0.2974 us |     1.23 |    0.00 |     18.7988 | 0.7324 |     - |     116 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     181.188 us |     0.3849 us |     0.3214 us |     1.38 |    0.00 |     18.7988 | 0.7324 |     - |     117 KB |
|     SudokuSpiceConstraints |  EvilA |      90.584 us |     0.1739 us |     0.1541 us |     0.69 |    0.00 |     21.2402 | 5.3711 |     - |     130 KB |
|                SudokuSharp |  EvilA |  42,136.014 us | 2,770.8044 us | 8,169.7794 us |   298.63 |   53.92 |   2333.3333 |      - |     - |  14,791 KB |
|           SudokuSolverLite |  EvilA | 392,983.107 us |   469.6899 us |   416.3679 us | 2,985.86 |    6.19 | 113000.0000 |      - |     - | 693,267 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilB |   1,254.524 us |    24.6943 us |    27.4476 us |     1.00 |    0.00 |    107.4219 | 3.9063 |     - |     661 KB |
|   SudokuSpiceDynamicSingle |  EvilB |   1,688.939 us |    31.7943 us |    47.5882 us |     1.34 |    0.05 |    185.5469 | 7.8125 |     - |   1,140 KB |
| SudokuSpiceDynamicMultiple |  EvilB |   2,030.478 us |    40.3301 us |    68.4835 us |     1.62 |    0.07 |    179.6875 | 7.8125 |     - |   1,104 KB |
|     SudokuSpiceConstraints |  EvilB |     239.480 us |     0.2658 us |     0.2486 us |     0.19 |    0.00 |     26.3672 | 7.3242 |     - |     162 KB |
|                SudokuSharp |  EvilB |  48,338.432 us |   775.3084 us |   725.2239 us |    38.62 |    0.99 |   3300.0000 |      - |     - |  20,296 KB |
|           SudokuSolverLite |  EvilB |  50,040.178 us |   141.9910 us |   132.8185 us |    39.98 |    0.91 |  14181.8182 |      - |     - |  86,909 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardA |      73.977 us |     0.1519 us |     0.1347 us |     1.00 |    0.00 |      7.6904 | 0.1221 |     - |      47 KB |
|   SudokuSpiceDynamicSingle |  HardA |      99.474 us |     0.2822 us |     0.2640 us |     1.35 |    0.00 |     11.8408 | 0.3662 |     - |      73 KB |
| SudokuSpiceDynamicMultiple |  HardA |     122.002 us |     0.2514 us |     0.2100 us |     1.65 |    0.00 |     11.7188 | 0.2441 |     - |      73 KB |
|     SudokuSpiceConstraints |  HardA |      84.664 us |     0.4534 us |     0.4242 us |     1.14 |    0.01 |     19.4092 | 4.7607 |     - |     120 KB |
|                SudokuSharp |  HardA |   3,300.306 us |    64.8635 us |   100.9847 us |    44.23 |    1.67 |    191.4063 |      - |     - |   1,178 KB |
|           SudokuSolverLite |  HardA |  24,561.879 us |    69.1191 us |    64.6541 us |   332.08 |    1.37 |   6937.5000 |      - |     - |  42,554 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardB |     205.755 us |     0.6493 us |     0.6074 us |     1.00 |    0.00 |     19.7754 | 0.4883 |     - |     122 KB |
|   SudokuSpiceDynamicSingle |  HardB |     256.465 us |     1.8596 us |     1.6485 us |     1.25 |    0.01 |     28.3203 | 0.9766 |     - |     176 KB |
| SudokuSpiceDynamicMultiple |  HardB |     299.625 us |     1.2178 us |     1.1392 us |     1.46 |    0.01 |     28.3203 | 0.9766 |     - |     176 KB |
|     SudokuSpiceConstraints |  HardB |      93.638 us |     0.2164 us |     0.1918 us |     0.46 |    0.00 |     20.1416 | 4.7607 |     - |     124 KB |
|                SudokuSharp |  HardB |  23,449.739 us |   755.6988 us | 2,228.1948 us |   113.75 |   12.25 |   1156.2500 |      - |     - |   7,199 KB |
|           SudokuSolverLite |  HardB |   4,684.208 us |     4.1593 us |     3.6871 us |    22.76 |    0.07 |   1335.9375 |      - |     - |   8,220 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice | Medium |      88.942 us |     0.2618 us |     0.2321 us |     1.00 |    0.00 |      8.6670 | 0.1221 |     - |      54 KB |
|   SudokuSpiceDynamicSingle | Medium |     118.816 us |     0.5251 us |     0.4911 us |     1.34 |    0.01 |     13.6719 | 0.3662 |     - |      84 KB |
| SudokuSpiceDynamicMultiple | Medium |     143.034 us |     0.4976 us |     0.4654 us |     1.61 |    0.01 |     13.6719 | 0.2441 |     - |      84 KB |
|     SudokuSpiceConstraints | Medium |      77.045 us |     0.3289 us |     0.3077 us |     0.87 |    0.00 |     20.2637 | 5.0049 |     - |     125 KB |
|                SudokuSharp | Medium |   2,994.146 us |    21.3752 us |    19.9944 us |    33.64 |    0.21 |    160.1563 |      - |     - |   1,000 KB |
|           SudokuSolverLite | Medium |   2,398.651 us |     1.2541 us |     1.0472 us |    26.96 |    0.07 |    664.0625 |      - |     - |   4,088 KB |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |           Mean |         Error |        StdDev |         Median |    Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------------:|---------:|--------:|------------:|------:|------:|-----------:|
|                SudokuSpice |   Easy |       398.8 us |       5.37 us |       5.02 us |       397.0 us |     1.00 |    0.00 |      1.4648 |     - |     - |       7 KB |
|   SudokuSpiceDynamicSingle |   Easy |       562.2 us |       0.76 us |       0.68 us |       562.2 us |     1.41 |    0.02 |      2.9297 |     - |     - |      12 KB |
| SudokuSpiceDynamicMultiple |   Easy |       720.4 us |       0.55 us |       0.52 us |       720.5 us |     1.81 |    0.02 |      2.9297 |     - |     - |      12 KB |
|     SudokuSpiceConstraints |   Easy |       472.2 us |       0.54 us |       0.48 us |       472.0 us |     1.18 |    0.01 |      9.2773 |     - |     - |      38 KB |
|                SudokuSharp |   Easy |       103.8 us |       0.09 us |       0.08 us |       103.8 us |     0.26 |    0.00 |      1.2207 |     - |     - |       5 KB |
|           SudokuSolverLite |   Easy |     3,577.5 us |       3.39 us |       3.17 us |     3,577.1 us |     8.97 |    0.11 |     62.5000 |     - |     - |     261 KB |
|                            |        |                |               |               |                |          |         |             |       |       |            |
|                SudokuSpice |  EvilA |     6,326.4 us |      82.08 us |      76.77 us |     6,316.6 us |     1.00 |    0.00 |     15.6250 |     - |     - |      75 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     7,083.3 us |      91.72 us |      85.79 us |     7,088.3 us |     1.12 |    0.02 |     15.6250 |     - |     - |      94 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     7,695.3 us |      77.84 us |      69.00 us |     7,702.4 us |     1.22 |    0.02 |     15.6250 |     - |     - |      93 KB |
|     SudokuSpiceConstraints |  EvilA |     1,262.1 us |       0.79 us |       0.70 us |     1,262.2 us |     0.20 |    0.00 |     17.5781 |     - |     - |      78 KB |
|                SudokuSharp |  EvilA |   794,320.1 us | 470,063.38 us | 439,697.58 us |   986,669.0 us |   125.41 |   69.06 |           - |     - |     - |   2,600 KB |
|           SudokuSolverLite |  EvilA | 9,343,017.4 us |   2,405.67 us |   2,008.84 us | 9,343,276.0 us | 1,479.69 |   16.43 | 166000.0000 |     - |     - | 674,507 KB |
|                            |        |                |               |               |                |          |         |             |       |       |            |
|                SudokuSpice |  EvilB |    57,596.2 us |   5,937.51 us |   5,263.44 us |    58,291.7 us |     1.00 |    0.00 |           - |     - |     - |     425 KB |
|   SudokuSpiceDynamicSingle |  EvilB |    73,321.6 us |  19,241.28 us |  17,998.30 us |    74,385.1 us |     1.25 |    0.31 |    142.8571 |     - |     - |     781 KB |
| SudokuSpiceDynamicMultiple |  EvilB |    77,712.6 us |  12,807.65 us |  11,353.65 us |    79,793.6 us |     1.37 |    0.27 |    125.0000 |     - |     - |     912 KB |
|     SudokuSpiceConstraints |  EvilB |     4,097.6 us |       2.68 us |       2.38 us |     4,098.1 us |     0.07 |    0.01 |     23.4375 |     - |     - |      99 KB |
|                SudokuSharp |  EvilB |   923,049.9 us |  68,997.65 us |  64,540.44 us |   963,117.0 us |    16.08 |    1.91 |   3000.0000 |     - |     - |  14,411 KB |
|           SudokuSolverLite |  EvilB | 1,151,852.4 us |     641.83 us |     535.96 us | 1,151,769.0 us |    20.25 |    1.92 |  20000.0000 |     - |     - |  83,411 KB |
|                            |        |                |               |               |                |          |         |             |       |       |            |
|                SudokuSpice |  HardA |     4,181.9 us |      59.25 us |      55.42 us |     4,192.4 us |     1.00 |    0.00 |      7.8125 |     - |     - |      40 KB |
|   SudokuSpiceDynamicSingle |  HardA |     4,866.1 us |      65.73 us |      61.49 us |     4,863.5 us |     1.16 |    0.02 |      7.8125 |     - |     - |      56 KB |
| SudokuSpiceDynamicMultiple |  HardA |     5,368.2 us |      70.72 us |      66.15 us |     5,384.4 us |     1.28 |    0.02 |      7.8125 |     - |     - |      55 KB |
|     SudokuSpiceConstraints |  HardA |     1,168.9 us |       0.46 us |       0.36 us |     1,169.1 us |     0.28 |    0.00 |     17.5781 |     - |     - |      73 KB |
|                SudokuSharp |  HardA |    57,519.2 us |   9,963.09 us |   9,319.48 us |    59,238.6 us |    13.75 |    2.21 |    200.0000 |     - |     - |     903 KB |
|           SudokuSolverLite |  HardA |   569,967.5 us |     464.44 us |     434.44 us |   570,083.0 us |   136.32 |    1.76 |  10000.0000 |     - |     - |  41,347 KB |
|                            |        |                |               |               |                |          |         |             |       |       |            |
|                SudokuSpice |  HardB |     9,021.0 us |     259.87 us |     243.08 us |     9,064.7 us |     1.00 |    0.00 |     15.6250 |     - |     - |     101 KB |
|   SudokuSpiceDynamicSingle |  HardB |    10,577.5 us |     269.09 us |     251.70 us |    10,542.9 us |     1.17 |    0.05 |     31.2500 |     - |     - |     138 KB |
| SudokuSpiceDynamicMultiple |  HardB |    11,639.3 us |     421.77 us |     394.53 us |    11,681.9 us |     1.29 |    0.06 |     31.2500 |     - |     - |     139 KB |
|     SudokuSpiceConstraints |  HardB |     1,413.1 us |       1.09 us |       0.96 us |     1,413.1 us |     0.16 |    0.00 |     17.5781 |     - |     - |      75 KB |
|                SudokuSharp |  HardB |   392,239.1 us | 134,938.18 us | 126,221.26 us |   420,837.3 us |    43.36 |   13.72 |   1333.3333 |     - |     - |   5,483 KB |
|           SudokuSolverLite |  HardB |   107,586.8 us |      95.55 us |      89.37 us |   107,588.2 us |    11.93 |    0.32 |   1800.0000 |     - |     - |   7,757 KB |
|                            |        |                |               |               |                |          |         |             |       |       |            |
|                SudokuSpice | Medium |     4,879.9 us |     130.94 us |     122.48 us |     4,882.8 us |     1.00 |    0.00 |      7.8125 |     - |     - |      46 KB |
|   SudokuSpiceDynamicSingle | Medium |     5,711.0 us |     133.64 us |     118.47 us |     5,728.3 us |     1.17 |    0.03 |     15.6250 |     - |     - |      65 KB |
| SudokuSpiceDynamicMultiple | Medium |     6,258.7 us |     154.88 us |     144.87 us |     6,239.8 us |     1.28 |    0.03 |     15.6250 |     - |     - |      67 KB |
|     SudokuSpiceConstraints | Medium |     1,035.9 us |       0.96 us |       0.90 us |     1,035.6 us |     0.21 |    0.01 |     17.5781 |     - |     - |      75 KB |
|                SudokuSharp | Medium |    50,828.6 us |   1,577.07 us |   1,475.20 us |    50,712.3 us |    10.42 |    0.34 |    100.0000 |     - |     - |     662 KB |
|           SudokuSolverLite | Medium |    52,531.5 us |      42.56 us |      35.54 us |    52,528.8 us |    10.82 |    0.24 |    900.0000 |     - |     - |   3,769 KB |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |   Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|--------:|------:|----------:|
|            SudokuSpice |  1.190 ms | 0.0087 ms | 0.0082 ms |  1.00 |    0.00 |  150.3906 | 19.5313 |     - |    931 KB |
| SudokuSpiceConstraints |  2.244 ms | 0.0243 ms | 0.0227 ms |  1.89 |    0.02 |  574.2188 |  7.8125 |     - |  3,533 KB |
|     SudokuSharpSingles | 14.520 ms | 0.6469 ms | 1.8870 ms | 11.84 |    2.00 | 3750.0000 | 31.2500 |     - | 22,988 KB |
|       SudokuSharpMixed |  7.163 ms | 0.2662 ms | 0.7724 ms |  6.19 |    0.76 | 1718.7500 | 15.6250 |     - | 10,542 KB |

### WASM

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|------:|------:|----------:|
|            SudokuSpice |  55.36 ms |  4.364 ms |  4.082 ms |  1.00 |    0.00 |  111.1111 |     - |     - |    818 KB |
| SudokuSpiceConstraints |  33.33 ms |  0.915 ms |  0.764 ms |  0.61 |    0.04 |  600.0000 |     - |     - |  2,584 KB |
|     SudokuSharpSingles | 197.75 ms | 96.873 ms | 90.615 ms |  3.56 |    1.59 | 3000.0000 |     - |     - | 12,712 KB |
|       SudokuSharpMixed | 135.02 ms | 59.727 ms | 55.869 ms |  2.45 |    1.05 | 1666.6667 |     - |     - |  8,118 KB |
|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |

