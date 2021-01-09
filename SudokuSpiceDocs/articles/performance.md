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
|            SudokuSpice |       Guesses: 0 |     24.44 us |     0.197 us |      0.184 us |     24.44 us |   1.00 |    0.00 |    1.8311 | 0.0305 |     - |     11 KB |
| SudokuSpiceConstraints |       Guesses: 0 |     63.41 us |     0.520 us |      0.461 us |     63.46 us |   2.60 |    0.03 |   17.9443 | 3.5400 |     - |    110 KB |
|            SudokuSharp |       Guesses: 0 |    748.22 us |    29.216 us |     86.143 us |    728.37 us |  30.62 |    3.81 |  149.4141 |      - |     - |    920 KB |
|       SudokuSolverLite |       Guesses: 0 |  1,133.55 us |    22.751 us |     66.723 us |  1,133.56 us |  47.02 |    1.69 |  347.6563 |      - |     - |  2,132 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |       Guesses: 1 |     38.61 us |     0.174 us |      0.163 us |     38.55 us |   1.00 |    0.00 |    3.2349 | 0.0610 |     - |     20 KB |
| SudokuSpiceConstraints |       Guesses: 1 |     68.55 us |     1.243 us |      1.162 us |     68.88 us |   1.78 |    0.03 |   18.4326 | 4.0283 |     - |    113 KB |
|            SudokuSharp |       Guesses: 1 |  1,494.79 us |    67.169 us |    196.996 us |  1,487.92 us |  38.12 |    3.91 |  298.8281 |      - |     - |  1,834 KB |
|       SudokuSolverLite |       Guesses: 1 |  3,158.81 us |   751.422 us |  2,191.933 us |  2,110.94 us |  82.26 |   51.12 |  500.0000 |      - |     - |  3,228 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 2-3 |     50.90 us |     0.555 us |      0.519 us |     50.82 us |   1.00 |    0.00 |    4.5776 | 0.1221 |     - |     28 KB |
| SudokuSpiceConstraints |     Guesses: 2-3 |     71.01 us |     0.466 us |      0.436 us |     71.00 us |   1.40 |    0.02 |   18.6768 | 3.7842 |     - |    115 KB |
|            SudokuSharp |     Guesses: 2-3 |  2,308.16 us |   197.741 us |    576.818 us |  2,190.35 us |  46.53 |   10.54 |  429.6875 |      - |     - |  2,634 KB |
|       SudokuSolverLite |     Guesses: 2-3 |  3,412.73 us |   289.330 us |    834.784 us |  3,214.71 us |  68.10 |   19.89 |  804.6875 |      - |     - |  4,956 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 4-7 |     70.80 us |     0.893 us |      0.835 us |     70.85 us |   1.00 |    0.00 |    6.7139 | 0.2441 |     - |     42 KB |
| SudokuSpiceConstraints |     Guesses: 4-7 |     76.08 us |     0.652 us |      0.544 us |     75.95 us |   1.07 |    0.01 |   19.2871 | 4.1504 |     - |    119 KB |
|            SudokuSharp |     Guesses: 4-7 |  3,113.09 us |   197.284 us |    575.488 us |  3,078.12 us |  45.61 |    7.67 |  582.0313 |      - |     - |  3,586 KB |
|       SudokuSolverLite |     Guesses: 4-7 |  8,627.83 us |   761.144 us |  2,183.864 us |  8,384.90 us | 124.97 |   25.19 | 2140.6250 | 7.8125 |     - | 13,124 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |      Guesses: 8+ |     91.61 us |     1.102 us |      1.031 us |     91.89 us |   1.00 |    0.00 |    8.9111 | 0.2441 |     - |     55 KB |
| SudokuSpiceConstraints |      Guesses: 8+ |     81.47 us |     0.511 us |      0.478 us |     81.29 us |   0.89 |    0.01 |   19.8975 | 4.1504 |     - |    122 KB |
|            SudokuSharp |      Guesses: 8+ |  5,161.20 us |   342.585 us |    993.901 us |  5,157.76 us |  60.75 |   10.96 |  742.1875 |      - |     - |  4,593 KB |
|       SudokuSolverLite |      Guesses: 8+ | 17,885.82 us | 4,333.827 us | 12,573.217 us | 14,542.08 us | 232.04 |  157.75 | 3187.5000 |      - |     - | 19,728 KB |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 |  Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|-------:|------:|-----------:|
|                SudokuSpice |   Easy |       9.699 us |     0.0516 us |     0.0458 us |     1.00 |    0.00 |      1.3428 | 0.0153 |     - |       8 KB |
|   SudokuSpiceDynamicSingle |   Easy |      14.651 us |     0.1060 us |     0.0828 us |     1.51 |    0.01 |      2.4872 | 0.0458 |     - |      15 KB |
| SudokuSpiceDynamicMultiple |   Easy |      20.179 us |     0.1238 us |     0.1034 us |     2.08 |    0.02 |      2.4719 | 0.0305 |     - |      15 KB |
|     SudokuSpiceConstraints |   Easy |      23.090 us |     0.1837 us |     0.1719 us |     2.38 |    0.02 |      8.0566 | 0.9460 |     - |      50 KB |
|                SudokuSharp |   Easy |       6.009 us |     0.0459 us |     0.0407 us |     0.62 |    0.00 |      1.1063 |      - |     - |       7 KB |
|           SudokuSolverLite |   Easy |     138.790 us |     0.6463 us |     0.6045 us |    14.32 |    0.08 |     44.4336 |      - |     - |     273 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilA |     120.492 us |     0.9365 us |     0.7311 us |     1.00 |    0.00 |     14.2822 | 0.4883 |     - |      88 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     146.528 us |     0.7751 us |     0.7251 us |     1.22 |    0.01 |     18.7988 | 0.7324 |     - |     117 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     170.974 us |     0.5610 us |     0.5248 us |     1.42 |    0.01 |     18.7988 | 0.4883 |     - |     117 KB |
|     SudokuSpiceConstraints |  EvilA |      85.512 us |     1.1479 us |     1.0737 us |     0.71 |    0.01 |     21.2402 | 5.3711 |     - |     130 KB |
|                SudokuSharp |  EvilA |  41,591.240 us | 2,214.8174 us | 6,425.5868 us |   359.06 |   45.61 |   2846.1538 |      - |     - |  17,522 KB |
|           SudokuSolverLite |  EvilA | 354,815.887 us |   766.0306 us |   716.5455 us | 2,945.80 |   18.90 | 113000.0000 |      - |     - | 693,265 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilB |   1,168.362 us |    21.8172 us |    23.3442 us |     1.00 |    0.00 |    105.4688 | 3.9063 |     - |     652 KB |
|   SudokuSpiceDynamicSingle |  EvilB |   1,543.377 us |    30.3400 us |    38.3704 us |     1.31 |    0.03 |    175.7813 | 7.8125 |     - |   1,084 KB |
| SudokuSpiceDynamicMultiple |  EvilB |   2,127.953 us |    42.1221 us |    79.1156 us |     1.80 |    0.08 |    175.7813 | 7.8125 |     - |   1,101 KB |
|     SudokuSpiceConstraints |  EvilB |     231.666 us |     1.3762 us |     1.2200 us |     0.20 |    0.00 |     26.3672 | 7.3242 |     - |     162 KB |
|                SudokuSharp |  EvilB |  46,753.282 us |   931.3844 us | 1,143.8236 us |    39.85 |    1.40 |   3181.8182 |      - |     - |  19,769 KB |
|           SudokuSolverLite |  EvilB |  45,239.891 us |   321.5351 us |   285.0325 us |    38.83 |    0.80 |  14166.6667 |      - |     - |  86,909 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardA |      70.159 us |     0.3025 us |     0.2830 us |     1.00 |    0.00 |      7.6904 | 0.1221 |     - |      47 KB |
|   SudokuSpiceDynamicSingle |  HardA |      91.071 us |     0.2831 us |     0.2648 us |     1.30 |    0.01 |     11.8408 | 0.3662 |     - |      73 KB |
| SudokuSpiceDynamicMultiple |  HardA |     124.505 us |     0.8809 us |     0.8240 us |     1.77 |    0.01 |     11.7188 | 0.2441 |     - |      72 KB |
|     SudokuSpiceConstraints |  HardA |      82.517 us |     0.6816 us |     0.6376 us |     1.18 |    0.01 |     19.4092 | 4.7607 |     - |     120 KB |
|                SudokuSharp |  HardA |   3,169.644 us |    62.9143 us |   128.5173 us |    44.94 |    2.36 |    191.4063 |      - |     - |   1,192 KB |
|           SudokuSolverLite |  HardA |  21,687.483 us |    58.4863 us |    48.8387 us |   309.25 |    1.25 |   6937.5000 |      - |     - |  42,554 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardB |     193.303 us |     1.8742 us |     1.7532 us |     1.00 |    0.00 |     19.7754 | 0.4883 |     - |     122 KB |
|   SudokuSpiceDynamicSingle |  HardB |     240.438 us |     2.1585 us |     2.0190 us |     1.24 |    0.01 |     28.5645 | 0.9766 |     - |     176 KB |
| SudokuSpiceDynamicMultiple |  HardB |     281.921 us |     2.0528 us |     1.9202 us |     1.46 |    0.02 |     28.3203 | 0.9766 |     - |     176 KB |
|     SudokuSpiceConstraints |  HardB |      87.315 us |     0.8579 us |     0.7605 us |     0.45 |    0.01 |     20.1416 | 4.7607 |     - |     124 KB |
|                SudokuSharp |  HardB |  23,179.916 us |   900.6698 us | 2,655.6452 us |   126.43 |   11.44 |   1437.5000 |      - |     - |   8,969 KB |
|           SudokuSolverLite |  HardB |   4,214.285 us |    30.9276 us |    28.9297 us |    21.80 |    0.20 |   1335.9375 |      - |     - |   8,220 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice | Medium |      80.970 us |     0.6547 us |     0.6124 us |     1.00 |    0.00 |      8.7891 | 0.1221 |     - |      54 KB |
|   SudokuSpiceDynamicSingle | Medium |     108.263 us |     0.6031 us |     0.5641 us |     1.34 |    0.01 |     13.7939 | 0.3662 |     - |      85 KB |
| SudokuSpiceDynamicMultiple | Medium |     131.229 us |     1.1292 us |     1.0562 us |     1.62 |    0.01 |     13.6719 | 0.2441 |     - |      85 KB |
|     SudokuSpiceConstraints | Medium |      70.130 us |     0.7103 us |     0.6644 us |     0.87 |    0.01 |     20.2637 | 4.8828 |     - |     125 KB |
|                SudokuSharp | Medium |   2,941.161 us |    13.0548 us |    12.2114 us |    36.33 |    0.28 |    160.1563 |      - |     - |     993 KB |
|           SudokuSolverLite | Medium |   2,056.946 us |    18.8705 us |    17.6515 us |    25.41 |    0.27 |    664.0625 |      - |     - |   4,088 KB |

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
|            SudokuSpice |  1.147 ms | 0.0102 ms | 0.0096 ms |  1.00 |    0.00 |  150.3906 | 19.5313 |     - |    930 KB |
| SudokuSpiceConstraints |  2.149 ms | 0.0389 ms | 0.0364 ms |  1.87 |    0.04 |  574.2188 | 66.4063 |     - |  3,522 KB |
|     SudokuSharpSingles | 12.542 ms | 0.4905 ms | 1.4384 ms | 10.97 |    1.29 | 2640.6250 | 15.6250 |     - | 16,246 KB |
|       SudokuSharpMixed |  6.683 ms | 0.1922 ms | 0.5668 ms |  5.83 |    0.59 | 1648.4375 | 15.6250 |     - | 10,128 KB |

### WASM

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|------:|------:|----------:|
|            SudokuSpice |  55.36 ms |  4.364 ms |  4.082 ms |  1.00 |    0.00 |  111.1111 |     - |     - |    818 KB |
| SudokuSpiceConstraints |  33.33 ms |  0.915 ms |  0.764 ms |  0.61 |    0.04 |  600.0000 |     - |     - |  2,584 KB |
|     SudokuSharpSingles | 197.75 ms | 96.873 ms | 90.615 ms |  3.56 |    1.59 | 3000.0000 |     - |     - | 12,712 KB |
|       SudokuSharpMixed | 135.02 ms | 59.727 ms | 55.869 ms |  2.45 |    1.05 | 1666.6667 |     - |     - |  8,118 KB |
|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |

