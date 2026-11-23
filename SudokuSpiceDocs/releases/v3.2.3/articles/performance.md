# Performance

All benchmarks were run using [BenchmarkDotNet](https://benchmarkdotnet.org/articles/overview.html).

The performance of this library has been compared to other .NET sudoku libraries.

**Compared libraries:**

* [SudokuSharp](https://github.com/BenjaminChambers/SudokuSharp)
* [SudokuSolverLite](https://github.com/zhiliangxu/SudokuSolver)

**Benchmark environment:**

* **OS:** Linux Ubuntu 26.04.1 LTS (x64)
* **CPU:** AMD Ryzen AI 9 HX 370 w/ Radeon 890M (12 physical cores / 24 logical cores)
* **Memory:** 22 GiB RAM
* **Runtime:** .NET 10.0.15 (SDK 10.0.404, X64 RyuJIT x86-64-v4)
* **Tool:** BenchmarkDotNet v0.15.8

## Puzzle solving performance

These were compared using a set of 1750 generated puzzles, with 24 - 30 preset squares, grouped
by the number of squares that needed to be guessed to solve the puzzle if using just the basic
Sudoku rules and no heuristics. *SudokuSpice* demonstrates considerably more speed and
stability regardless of the number of guesses.

| Method                 | sampleCollection | Mean        | Error      | StdDev       | Ratio  | RatioSD | Gen0      | Gen1    | Allocated   | Alloc Ratio |
|----------------------- |----------------- |------------:|-----------:|-------------:|-------:|--------:|----------:|--------:|------------:|------------:|
| SudokuSpice            | Guesses: 0       |    13.28 μs |   0.091 μs |     0.085 μs |   1.00 |    0.01 |    1.1749 |  0.0153 |     9.68 KB |        1.00 |
| SudokuSpiceConstraints | Guesses: 0       |    61.00 μs |   1.171 μs |     1.481 μs |   4.59 |    0.11 |   17.7002 |  3.9063 |   145.44 KB |       15.02 |
| SudokuSharp            | Guesses: 0       |   254.27 μs |   5.705 μs |    16.820 μs |  19.15 |    1.27 |   41.0156 |       - |   337.83 KB |       34.89 |
| SudokuSolverLite       | Guesses: 0       |   519.67 μs |  10.382 μs |    26.426 μs |  39.14 |    1.99 |  243.1641 |       - |  1987.98 KB |      205.29 |
|                        |                  |             |            |              |        |         |           |         |             |             |
| SudokuSpice            | Guesses: 1       |    22.36 μs |   0.356 μs |     0.333 μs |   1.00 |    0.02 |    1.9226 |  0.0305 |    15.87 KB |        1.00 |
| SudokuSpiceConstraints | Guesses: 1       |    63.96 μs |   1.241 μs |     1.274 μs |   2.86 |    0.07 |   18.0664 |  5.1270 |    148.5 KB |        9.36 |
| SudokuSharp            | Guesses: 1       |   524.25 μs |  13.445 μs |    39.005 μs |  23.45 |    1.77 |   89.8438 |       - |   737.01 KB |       46.45 |
| SudokuSolverLite       | Guesses: 1       | 1,567.02 μs |  74.550 μs |   215.094 μs |  70.10 |    9.63 |  835.9375 |  1.9531 |  6838.58 KB |      431.02 |
|                        |                  |             |            |              |        |         |           |         |             |             |
| SudokuSpice            | Guesses: 2-3     |    29.50 μs |   0.270 μs |     0.225 μs |   1.00 |    0.01 |    2.5635 |  0.0610 |    21.18 KB |        1.00 |
| SudokuSpiceConstraints | Guesses: 2-3     |    66.40 μs |   0.670 μs |     0.627 μs |   2.25 |    0.03 |   18.4326 |  4.1504 |   150.68 KB |        7.12 |
| SudokuSharp            | Guesses: 2-3     |   832.81 μs |  36.202 μs |   106.743 μs |  28.23 |    3.61 |  137.6953 |       - |  1130.16 KB |       53.37 |
| SudokuSolverLite       | Guesses: 2-3     | 1,622.11 μs |  70.585 μs |   207.014 μs |  54.99 |    7.00 |  722.6563 |  1.9531 |  5916.95 KB |      279.40 |
|                        |                  |             |            |              |        |         |           |         |             |             |
| SudokuSpice            | Guesses: 4-7     |    41.24 μs |   0.478 μs |     0.424 μs |   1.00 |    0.01 |    3.6011 |  0.0610 |    29.86 KB |        1.00 |
| SudokuSpiceConstraints | Guesses: 4-7     |    69.85 μs |   0.925 μs |     0.865 μs |   1.69 |    0.03 |   18.9209 |  4.2725 |   154.88 KB |        5.19 |
| SudokuSharp            | Guesses: 4-7     | 1,087.61 μs |  50.190 μs |   144.808 μs |  26.37 |    3.50 |  191.4063 |       - |  1573.38 KB |       52.70 |
| SudokuSolverLite       | Guesses: 4-7     | 3,984.54 μs | 377.460 μs | 1,089.060 μs |  96.62 |   26.30 | 2078.1250 |  7.8125 | 16993.03 KB |      569.16 |
|                        |                  |             |            |              |        |         |           |         |             |             |
| SudokuSpice            | Guesses: 8+      |    52.51 μs |   0.263 μs |     0.246 μs |   1.00 |    0.01 |    4.6997 |  0.1221 |    38.88 KB |        1.00 |
| SudokuSpiceConstraints | Guesses: 8+      |    73.50 μs |   0.692 μs |     0.578 μs |   1.40 |    0.01 |   19.4092 |  4.3945 |   158.94 KB |        4.09 |
| SudokuSharp            | Guesses: 8+      | 1,770.15 μs |  63.272 μs |   186.560 μs |  33.71 |    3.54 |  353.5156 |       - |  2888.68 KB |       74.29 |
| SudokuSolverLite       | Guesses: 8+      | 8,570.01 μs | 719.413 μs | 2,121.205 μs | 163.22 |   40.22 | 4742.1875 | 15.6250 | 38762.88 KB |      996.94 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

| Method                     | puzzle | Mean           | Error         | StdDev        | Ratio    | RatioSD | Gen0       | Gen1     | Allocated    | Alloc Ratio |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|-----------:|---------:|-------------:|------------:|
| SudokuSpice                | Easy   |       3.510 μs |     0.0610 μs |     0.0541 μs |     1.00 |    0.02 |     1.1368 |   0.0191 |      9.31 KB |        1.00 |
| SudokuSpiceDynamicSingle   | Easy   |       4.212 μs |     0.0368 μs |     0.0344 μs |     1.20 |    0.02 |     1.3046 |   0.0229 |     10.67 KB |        1.15 |
| SudokuSpiceDynamicMultiple | Easy   |       6.080 μs |     0.0487 μs |     0.0432 μs |     1.73 |    0.03 |     1.3123 |   0.0229 |     10.73 KB |        1.15 |
| SudokuSpiceConstraints     | Easy   |      19.981 μs |     0.3128 μs |     0.2773 μs |     5.69 |    0.11 |     8.4229 |   1.0376 |     68.92 KB |        7.40 |
| SudokuSharp                | Easy   |       2.397 μs |     0.0325 μs |     0.0304 μs |     0.68 |    0.01 |     0.8049 |        - |      6.58 KB |        0.71 |
| SudokuSolverLite           | Easy   |      52.751 μs |     0.8950 μs |     0.8372 μs |    15.03 |    0.32 |    27.2217 |   0.0610 |    222.66 KB |       23.91 |
|                            |        |                |               |               |          |         |            |          |              |             |
| SudokuSpice                | Medium |      27.838 μs |     0.4734 μs |     0.4196 μs |     1.00 |    0.02 |     4.6082 |   0.1221 |     37.73 KB |        1.00 |
| SudokuSpiceDynamicSingle   | Medium |      31.176 μs |     0.4021 μs |     0.3761 μs |     1.12 |    0.02 |     4.7607 |   0.1221 |     39.14 KB |        1.04 |
| SudokuSpiceDynamicMultiple | Medium |      41.469 μs |     0.3112 μs |     0.2599 μs |     1.49 |    0.02 |     4.7607 |   0.1221 |     39.21 KB |        1.04 |
| SudokuSpiceConstraints     | Medium |      57.302 μs |     0.8464 μs |     0.7917 μs |     2.06 |    0.04 |    20.0195 |   6.2866 |    163.54 KB |        4.33 |
| SudokuSharp                | Medium |   1,113.887 μs |    10.5652 μs |     8.8224 μs |    40.02 |    0.65 |   121.0938 |        - |   1000.54 KB |       26.52 |
| SudokuSolverLite           | Medium |     917.295 μs |    10.5568 μs |     9.8749 μs |    32.96 |    0.58 |   458.9844 |   1.9531 |    3754.1 KB |       99.51 |
|                            |        |                |               |               |          |         |            |          |              |             |
| SudokuSpice                | HardA  |      23.076 μs |     0.4247 μs |     0.3765 μs |     1.00 |    0.02 |     4.2114 |   0.1221 |     34.56 KB |        1.00 |
| SudokuSpiceDynamicSingle   | HardA  |      26.219 μs |     0.4682 μs |     0.4379 μs |     1.14 |    0.03 |     4.3945 |   0.1221 |     35.97 KB |        1.04 |
| SudokuSpiceDynamicMultiple | HardA  |      33.906 μs |     0.6627 μs |     0.6508 μs |     1.47 |    0.04 |     4.3945 |   0.1221 |     36.03 KB |        1.04 |
| SudokuSpiceConstraints     | HardA  |      60.794 μs |     0.7218 μs |     0.6027 μs |     2.64 |    0.05 |    19.2871 |   5.8594 |    157.78 KB |        4.57 |
| SudokuSharp                | HardA  |   1,219.420 μs |    23.5288 μs |    28.8955 μs |    52.86 |    1.48 |   140.6250 |        - |   1159.43 KB |       33.55 |
| SudokuSolverLite           | HardA  |   9,706.850 μs |   126.6694 μs |   112.2892 μs |   420.76 |    8.09 |  4546.8750 |  15.6250 |  37238.48 KB |    1,077.49 |
|                            |        |                |               |               |          |         |            |          |              |             |
| SudokuSpice                | HardB  |      69.301 μs |     0.5934 μs |     0.5260 μs |     1.00 |    0.01 |    10.7422 |   0.3662 |     87.83 KB |        1.00 |
| SudokuSpiceDynamicSingle   | HardB  |      75.382 μs |     0.6169 μs |     0.5771 μs |     1.09 |    0.01 |    10.8643 |   0.3662 |     89.57 KB |        1.02 |
| SudokuSpiceDynamicMultiple | HardB  |      97.240 μs |     1.6522 μs |     1.5455 μs |     1.40 |    0.02 |    10.9863 |   0.4883 |      89.8 KB |        1.02 |
| SudokuSpiceConstraints     | HardB  |      62.712 μs |     0.9002 μs |     0.7980 μs |     0.90 |    0.01 |    19.4092 |   5.6152 |    158.88 KB |        1.81 |
| SudokuSharp                | HardB  |   8,265.677 μs |   203.0798 μs |   595.5981 μs |   119.28 |    8.60 |   968.7500 |        - |   8034.01 KB |       91.47 |
| SudokuSolverLite           | HardB  |   1,812.050 μs |    28.0145 μs |    24.8341 μs |    26.15 |    0.40 |   914.0625 |   3.9063 |    7469.9 KB |       85.05 |
|                            |        |                |               |               |          |         |            |          |              |             |
| SudokuSpice                | EvilA  |      41.478 μs |     0.4384 μs |     0.3886 μs |     1.00 |    0.01 |     7.2021 |   0.2441 |      59.1 KB |        1.00 |
| SudokuSpiceDynamicSingle   | EvilA  |      46.881 μs |     0.2537 μs |     0.2118 μs |     1.13 |    0.01 |     7.3853 |   0.3052 |     60.56 KB |        1.02 |
| SudokuSpiceDynamicMultiple | EvilA  |      54.544 μs |     0.5988 μs |     0.5308 μs |     1.32 |    0.02 |     7.3853 |   0.3052 |     60.67 KB |        1.03 |
| SudokuSpiceConstraints     | EvilA  |      69.618 μs |     1.3266 μs |     1.1077 μs |     1.68 |    0.03 |    20.8740 |   6.7139 |    170.76 KB |        2.89 |
| SudokuSharp                | EvilA  |  15,490.435 μs |   411.0537 μs | 1,212.0011 μs |   373.50 |   29.28 |  2000.0000 |        - |  16445.28 KB |      278.27 |
| SudokuSolverLite           | EvilA  | 148,243.149 μs | 2,829.5864 μs | 2,646.7969 μs | 3,574.35 |   69.75 | 73000.0000 | 250.0000 | 596506.66 KB |   10,093.58 |
|                            |        |                |               |               |          |         |            |          |              |             |
| SudokuSpice                | EvilB  |     578.335 μs |    11.5044 μs |    13.2485 μs |     1.00 |    0.03 |    50.7813 |   1.9531 |    420.78 KB |        1.00 |
| SudokuSpiceDynamicSingle   | EvilB  |     648.091 μs |    12.7711 μs |    19.5028 μs |     1.12 |    0.04 |    52.7344 |   1.9531 |    434.83 KB |        1.03 |
| SudokuSpiceDynamicMultiple | EvilB  |     788.875 μs |    10.8656 μs |     9.6321 μs |     1.36 |    0.03 |    50.7813 |   1.9531 |    420.88 KB |        1.00 |
| SudokuSpiceConstraints     | EvilB  |     117.932 μs |     1.9578 μs |     1.8313 μs |     0.20 |    0.01 |    23.9258 |   7.6904 |    195.65 KB |        0.46 |
| SudokuSharp                | EvilB  |  16,752.195 μs |   278.0091 μs |   246.4479 μs |    28.98 |    0.77 |  2437.5000 |        - |  19978.07 KB |       47.48 |
| SudokuSolverLite           | EvilB  |  21,106.410 μs |   348.0715 μs |   325.5863 μs |    36.51 |    0.98 |  9468.7500 |  31.2500 |  77514.53 KB |      184.22 |

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

| Method                 | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0      | Gen1     | Allocated  | Alloc Ratio |
|----------------------- |-----------:|----------:|----------:|------:|--------:|----------:|---------:|-----------:|------------:|
| SudokuSpice            |   583.3 μs |   8.23 μs |   7.29 μs |  1.00 |    0.02 |   97.6563 |  14.6484 |  800.26 KB |        1.00 |
| SudokuSpiceConstraints | 1,755.8 μs |  28.55 μs |  26.70 μs |  3.01 |    0.06 |  591.7969 | 197.2656 | 4846.54 KB |        6.06 |
| SudokuSharpSingles     | 4,739.7 μs | 129.76 μs | 378.52 μs |  8.13 |    0.65 | 1078.1250 |   7.8125 |  8825.9 KB |       11.03 |
| SudokuSharpMixed       | 2,442.7 μs |  56.60 μs | 166.90 μs |  4.19 |    0.29 |  656.2500 |   3.9063 |  5380.3 KB |        6.72 |

### WASM

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|------:|------:|----------:|
|            SudokuSpice |  38.25 ms |  1.647 ms |  1.460 ms |  1.00 |    0.00 |  153.8462 |     - |     - |    837 KB |
| SudokuSpiceConstraints |  50.06 ms |  2.581 ms |  2.415 ms |  1.31 |    0.10 |  800.0000 |     - |     - |  3,498 KB |
|     SudokuSharpSingles | 217.88 ms | 87.680 ms | 73.217 ms |  5.73 |    1.97 | 2500.0000 |     - |     - | 10,695 KB |
|       SudokuSharpMixed | 137.09 ms | 48.759 ms | 45.609 ms |  3.72 |    1.23 | 1600.0000 |     - |     - |  6,868 KB |
