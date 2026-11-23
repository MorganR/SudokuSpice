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
* **WASM runtime:** .NET 10.0.15 Mono WebAssembly (`wasm-tools` `10.0.115/10.0.100`) on Node.js v22.19.0 (Interpreter and AOT modes)
* **Tool:** BenchmarkDotNet v0.15.8 (native), v0.16.0-preview.2 (WASM)

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

Performance is considerably different in a Mono WASM environment (measured in both Interpreter and
AOT modes; the CSV benchmarks are not included for WASM). WASM AOT delivers roughly a 2.6×–3.4×
speedup over the WASM interpreter for *SudokuSpice* while keeping managed memory allocations
essentially unchanged. Once again, `SudokuSharp` is faster on the trivial `Easy` puzzle, while
*SudokuSpice* is tens to thousands of times faster on `Medium` through `EvilB`. Between the two
*SudokuSpice* solvers, the rule-based solver is faster on `Easy` (and on `HardA` under AOT), whereas
the constraint-based solver is faster on `Medium`, `HardB`, `EvilA`, and especially `EvilB`
(roughly 5×–6× faster than the rule-based solver).

#### Interpreter

| Method                     | puzzle |            Mean |         Error |         StdDev |    Ratio |        Gen0 |    Allocated | Alloc Ratio |
|--------------------------- |------- |----------------:|--------------:|---------------:|---------:|------------:|-------------:|------------:|
| SudokuSpice                | Easy   |        89.17 μs |      1.778 μs |       3.426 μs |     1.00 |      1.9531 |      7.81 KB |        1.00 |
| SudokuSpiceDynamicSingle   | Easy   |        97.86 μs |      1.938 μs |       3.781 μs |     1.10 |      2.1973 |      9.06 KB |        1.16 |
| SudokuSpiceDynamicMultiple | Easy   |       157.04 μs |      3.041 μs |       8.324 μs |     1.76 |      2.1973 |      9.09 KB |        1.16 |
| SudokuSpiceConstraints     | Easy   |       210.10 μs |      4.188 μs |       8.461 μs |     2.36 |     10.2539 |     41.99 KB |        5.38 |
| SudokuSharp                | Easy   |        34.43 μs |      0.682 μs |       1.581 μs |     0.39 |      1.2207 |      5.08 KB |        0.65 |
| SudokuSolverLite           | Easy   |     1,225.91 μs |     24.477 μs |      47.159 μs |    13.75 |     48.8281 |    199.26 KB |       25.51 |
|                            |        |                 |               |                |          |             |              |             |
| SudokuSpice                | Medium |       756.09 μs |     15.034 μs |      43.137 μs |     1.00 |      7.8125 |     31.56 KB |        1.00 |
| SudokuSpiceDynamicSingle   | Medium |       841.60 μs |     16.821 μs |      44.313 μs |     1.11 |      7.8125 |     33.32 KB |        1.06 |
| SudokuSpiceDynamicMultiple | Medium |       979.93 μs |     14.518 μs |      14.259 μs |     1.30 |      7.8125 |     32.74 KB |        1.04 |
| SudokuSpiceConstraints     | Medium |       537.90 μs |     10.660 μs |      15.955 μs |     0.71 |     23.4375 |     95.45 KB |        3.02 |
| SudokuSharp                | Medium |    22,066.47 μs |    402.615 μs |     683.670 μs |    29.18 |    156.2500 |    659.48 KB |       20.90 |
| SudokuSolverLite           | Medium |    19,240.32 μs |    277.258 μs |     216.465 μs |    25.45 |    718.7500 |   2978.91 KB |       94.39 |
|                            |        |                 |               |                |          |             |              |             |
| SudokuSpice                | HardA  |       641.04 μs |     12.783 μs |      32.997 μs |     1.00 |      6.8359 |     28.73 KB |        1.00 |
| SudokuSpiceDynamicSingle   | HardA  |       704.93 μs |     14.089 μs |      37.361 μs |     1.10 |      6.8359 |     29.99 KB |        1.04 |
| SudokuSpiceDynamicMultiple | HardA  |       845.99 μs |     14.939 μs |      24.124 μs |     1.32 |      6.8359 |     30.08 KB |        1.05 |
| SudokuSpiceConstraints     | HardA  |       640.43 μs |     12.807 μs |      31.657 μs |     1.00 |     22.4609 |     92.45 KB |        3.22 |
| SudokuSharp                | HardA  |    25,962.84 μs |    938.391 μs |   2,766.868 μs |    40.50 |    187.5000 |    889.21 KB |       30.95 |
| SudokuSolverLite           | HardA  |   201,480.71 μs |  4,008.891 μs |   9,605.048 μs |   314.30 |   7333.3333 |  30867.48 KB |    1,074.40 |
|                            |        |                 |               |                |          |             |              |             |
| SudokuSpice                | HardB  |     1,567.02 μs |     31.235 μs |      75.436 μs |     1.00 |     17.5781 |      72.1 KB |        1.00 |
| SudokuSpiceDynamicSingle   | HardB  |     1,747.82 μs |     34.904 μs |     100.706 μs |     1.12 |     15.6250 |     73.14 KB |        1.01 |
| SudokuSpiceDynamicMultiple | HardB  |     2,070.98 μs |     41.137 μs |     100.133 μs |     1.32 |     15.6250 |     73.85 KB |        1.02 |
| SudokuSpiceConstraints     | HardB  |       667.23 μs |     13.225 μs |      27.606 μs |     0.43 |     22.4609 |     93.04 KB |        1.29 |
| SudokuSharp                | HardB  |   168,621.40 μs | 37,846.151 μs | 111,590.238 μs |   107.61 |   2000.0000 |   8700.19 KB |      120.67 |
| SudokuSolverLite           | HardB  |    39,962.01 μs |    794.552 μs |   1,918.933 μs |    25.50 |   1428.5714 |   6037.09 KB |       83.73 |
|                            |        |                 |               |                |          |             |              |             |
| SudokuSpice                | EvilA  |     1,003.89 μs |     19.911 μs |      26.581 μs |     1.00 |     11.7188 |     49.12 KB |        1.00 |
| SudokuSpiceDynamicSingle   | EvilA  |     1,116.52 μs |     22.221 μs |      64.112 μs |     1.11 |     11.7188 |     50.55 KB |        1.03 |
| SudokuSpiceDynamicMultiple | EvilA  |     1,273.72 μs |     25.042 μs |      51.716 μs |     1.27 |     11.7188 |     50.59 KB |        1.03 |
| SudokuSpiceConstraints     | EvilA  |       753.24 μs |     14.903 μs |      35.992 μs |     0.75 |     24.4141 |     99.56 KB |        2.03 |
| SudokuSharp                | EvilA  |   318,508.84 μs | 47,991.453 μs | 141,503.891 μs |   317.27 |   2000.0000 |   9888.29 KB |      201.31 |
| SudokuSolverLite           | EvilA  | 3,284,042.48 μs | 64,823.263 μs |  88,730.821 μs | 3,271.32 | 125000.0000 | 509269.77 KB |   10,367.87 |
|                            |        |                 |               |                |          |             |              |             |
| SudokuSpice                | EvilB  |     9,653.13 μs |    304.483 μs |     892.997 μs |     1.00 |     93.7500 |    389.15 KB |        1.00 |
| SudokuSpiceDynamicSingle   | EvilB  |    10,975.05 μs |    277.344 μs |     817.755 μs |     1.14 |     78.1250 |     339.4 KB |        0.87 |
| SudokuSpiceDynamicMultiple | EvilB  |    13,643.30 μs |    540.916 μs |   1,586.413 μs |     1.41 |     62.5000 |    372.01 KB |        0.96 |
| SudokuSpiceConstraints     | EvilB  |     1,527.25 μs |     30.111 μs |      45.983 μs |     0.16 |     27.3438 |    114.58 KB |        0.29 |
| SudokuSharp                | EvilB  |   395,547.14 μs |  9,602.515 μs |  28,162.516 μs |    40.98 |   3000.0000 |  14719.38 KB |       37.82 |
| SudokuSolverLite           | EvilB  |   417,714.28 μs |  8,342.090 μs |  21,233.284 μs |    43.27 |  15000.0000 |  63190.63 KB |      162.38 |

#### AOT

| Method                     | puzzle |            Mean |         Error |        StdDev |    Ratio |        Gen0 |    Allocated | Alloc Ratio |
|--------------------------- |------- |----------------:|--------------:|--------------:|---------:|------------:|-------------:|------------:|
| SudokuSpice                | Easy   |        32.82 μs |      0.648 μs |      1.601 μs |     1.00 |      1.9531 |      7.81 KB |        1.00 |
| SudokuSpiceDynamicSingle   | Easy   |        34.83 μs |      0.695 μs |      1.497 μs |     1.06 |      2.2583 |      9.06 KB |        1.16 |
| SudokuSpiceDynamicMultiple | Easy   |        43.00 μs |      0.860 μs |      2.026 μs |     1.31 |      2.2583 |      9.09 KB |        1.16 |
| SudokuSpiceConstraints     | Easy   |        64.09 μs |      1.272 μs |      3.351 μs |     1.95 |     10.2539 |     41.99 KB |        5.38 |
| SudokuSharp                | Easy   |        14.36 μs |      0.272 μs |      0.567 μs |     0.44 |      1.2512 |      5.08 KB |        0.65 |
| SudokuSolverLite           | Easy   |       867.87 μs |     17.321 μs |     43.134 μs |    26.44 |     48.8281 |    199.26 KB |       25.51 |
|                            |        |                 |               |               |          |             |              |             |
| SudokuSpice                | Medium |       224.07 μs |      4.417 μs |     10.750 μs |     1.00 |      7.8125 |     31.66 KB |        1.00 |
| SudokuSpiceDynamicSingle   | Medium |       246.07 μs |      4.837 μs |      9.204 μs |     1.10 |      8.0566 |     32.91 KB |        1.04 |
| SudokuSpiceDynamicMultiple | Medium |       278.90 μs |      5.528 μs |     13.350 μs |     1.24 |      7.8125 |     32.93 KB |        1.04 |
| SudokuSpiceConstraints     | Medium |       184.20 μs |      3.655 μs |      7.214 μs |     0.82 |     23.4375 |     95.45 KB |        3.01 |
| SudokuSharp                | Medium |     8,239.95 μs |    162.072 μs |    204.969 μs |    36.77 |    164.0625 |    675.52 KB |       21.34 |
| SudokuSolverLite           | Medium |    17,295.80 μs |    393.066 μs |  1,134.086 μs |    77.19 |    718.7500 |   2978.91 KB |       94.09 |
|                            |        |                 |               |               |          |             |              |             |
| SudokuSpice                | HardA  |       190.21 μs |      3.790 μs |      7.393 μs |     1.00 |      7.0801 |     28.71 KB |        1.00 |
| SudokuSpiceDynamicSingle   | HardA  |       200.85 μs |      4.099 μs |     12.021 μs |     1.06 |      7.3242 |     29.93 KB |        1.04 |
| SudokuSpiceDynamicMultiple | HardA  |       235.28 μs |      4.698 μs |     13.853 μs |     1.24 |      7.3242 |     29.98 KB |        1.04 |
| SudokuSpiceConstraints     | HardA  |       225.28 μs |      4.781 μs |     14.096 μs |     1.18 |     22.7051 |     92.45 KB |        3.22 |
| SudokuSharp                | HardA  |     9,378.92 μs |    298.619 μs |    871.087 μs |    49.31 |    171.8750 |    757.79 KB |       26.39 |
| SudokuSolverLite           | HardA  |   157,813.54 μs |  3,148.021 μs |  8,012.719 μs |   829.68 |   7500.0000 |  30867.47 KB |    1,075.15 |
|                            |        |                 |               |               |          |             |              |             |
| SudokuSpice                | HardB  |       498.96 μs |      9.923 μs |     25.436 μs |     1.00 |     17.5781 |     72.34 KB |        1.00 |
| SudokuSpiceDynamicSingle   | HardB  |       537.90 μs |     10.633 μs |     24.432 μs |     1.08 |     17.5781 |     73.21 KB |        1.01 |
| SudokuSpiceDynamicMultiple | HardB  |       601.02 μs |     11.919 μs |     31.191 μs |     1.20 |     17.5781 |     73.79 KB |        1.02 |
| SudokuSpiceConstraints     | HardB  |       228.62 μs |      4.518 μs |     10.106 μs |     0.46 |     22.9492 |     93.04 KB |        1.29 |
| SudokuSharp                | HardB  |    69,859.57 μs |  3,205.152 μs |  9,400.158 μs |   140.01 |   1125.0000 |   4577.57 KB |       63.28 |
| SudokuSolverLite           | HardB  |    33,144.88 μs |    660.859 μs |  1,863.965 μs |    66.43 |   1437.5000 |   6037.09 KB |       83.45 |
|                            |        |                 |               |               |          |             |              |             |
| SudokuSpice                | EvilA  |       308.67 μs |      6.115 μs |     15.564 μs |     1.00 |     11.7188 |     49.23 KB |        1.00 |
| SudokuSpiceDynamicSingle   | EvilA  |       321.64 μs |      6.407 μs |     13.792 μs |     1.04 |     12.2070 |     50.61 KB |        1.03 |
| SudokuSpiceDynamicMultiple | EvilA  |       360.63 μs |      7.203 μs |     19.474 μs |     1.17 |     12.2070 |     50.71 KB |        1.03 |
| SudokuSpiceConstraints     | EvilA  |       254.56 μs |      5.058 μs |     12.966 μs |     0.82 |     24.4141 |     99.56 KB |        2.02 |
| SudokuSharp                | EvilA  |   115,337.19 μs | 10,709.219 μs | 31,408.287 μs |   373.66 |   3500.0000 |  14623.22 KB |      297.04 |
| SudokuSolverLite           | EvilA  | 2,452,127.89 μs | 47,942.281 μs | 49,233.193 μs | 7,944.17 | 125000.0000 | 509269.74 KB |   10,344.70 |
|                            |        |                 |               |               |          |             |              |             |
| SudokuSpice                | EvilB  |     3,030.45 μs |     60.284 μs |    131.052 μs |     1.00 |     82.0313 |    347.05 KB |        1.00 |
| SudokuSpiceDynamicSingle   | EvilB  |     3,440.42 μs |     66.354 μs |    127.842 μs |     1.14 |     89.8438 |    367.54 KB |        1.06 |
| SudokuSpiceDynamicMultiple | EvilB  |     3,911.41 μs |     86.811 μs |    253.232 μs |     1.29 |     85.9375 |    360.95 KB |        1.04 |
| SudokuSpiceConstraints     | EvilB  |       589.22 μs |     11.659 μs |     22.182 μs |     0.19 |     27.3438 |    114.58 KB |        0.33 |
| SudokuSharp                | EvilB  |   139,025.00 μs |  2,727.197 μs |  5,570.940 μs |    45.88 |   3250.0000 |  13943.18 KB |       40.18 |
| SudokuSolverLite           | EvilB  |   347,854.31 μs |  6,926.187 μs | 13,991.240 μs |   114.79 |  15500.0000 |   63190.6 KB |      182.08 |

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

For puzzle generation, WASM AOT speeds up *SudokuSpice* by ~2.9×–3.6× over the WASM interpreter,
and rule-based `SudokuSpice` leads across both modes.

#### Interpreter

| Method                 |     Mean |    Error |    StdDev | Ratio | RatioSD |      Gen0 |   Allocated | Alloc Ratio |
|----------------------- |---------:|---------:|----------:|------:|--------:|----------:|------------:|------------:|
| SudokuSpice            | 10.23 ms | 0.204 ms |  0.585 ms |  1.00 |    0.00 |  156.2500 |   669.32 KB |        1.00 |
| SudokuSpiceConstraints | 20.25 ms | 2.673 ms |  7.882 ms |  1.99 |    0.78 |  687.5000 |  2888.36 KB |        4.32 |
| SudokuSharpSingles     | 89.36 ms | 8.427 ms | 24.313 ms |  8.76 |    2.42 | 4000.0000 | 16418.57 KB |       24.53 |
| SudokuSharpMixed       | 45.87 ms | 4.049 ms | 11.552 ms |  4.50 |    1.16 | 1750.0000 |  7189.79 KB |       10.74 |

#### AOT

| Method                 |      Mean |     Error |    StdDev | Ratio | RatioSD |      Gen0 |  Allocated | Alloc Ratio |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|-----------:|------------:|
| SudokuSpice            |  3.536 ms | 0.0698 ms | 0.1698 ms |  1.00 |    0.00 |  164.0625 |  659.31 KB |        1.00 |
| SudokuSpiceConstraints |  5.663 ms | 0.1120 ms | 0.2483 ms |  1.60 |    0.10 |  703.1250 | 2,837.09 KB |        4.30 |
| SudokuSharpSingles     | 17.443 ms | 0.6473 ms | 1.8883 ms |  4.94 |    0.58 | 2375.0000 | 9,648.85 KB |       14.63 |
| SudokuSharpMixed       |  9.670 ms | 0.3288 ms | 0.9591 ms |  2.74 |    0.30 | 1781.2500 | 7,257.79 KB |       11.01 |

## WASM binary sizes

Enabling WebAssembly AOT compilation (`RunAOTCompilation=true`) trades a larger `dotnet.native.wasm`
binary for roughly 2.6×–3.6× faster execution.

To measure *SudokuSpice*'s own footprint separately from the benchmark harness, a minimal standalone
`Microsoft.NET.Sdk.WebAssembly` (`net10.0`) app exercising `StandardPuzzleGenerator`,
`RuleBased.StandardPuzzles.CreateSolver()`, and `ConstraintBased.StandardPuzzles.CreateSolver()` was
published with `dotnet publish -c Release` (`PublishTrimmed=true` in both modes):

| Mode        | `dotnet.native.wasm` (Raw / `.br` / `.gz`) | `SudokuSpice.wasm` (Raw / `.br` / `.gz`) | Managed Assemblies (7 `.wasm`, Raw / `.br` / `.gz`) | Total `_framework` (14 files, Raw / `.br` / `.gz`) |
|------------ |-------------------------------------------:|-----------------------------------------:|----------------------------------------------------:|---------------------------------------------------:|
| Interpreter | 2.74 MiB / 901.6 KiB / 1.09 MiB            | 65.3 KiB / 24.4 KiB / 28.5 KiB          | 1.25 MiB / 414.2 KiB / 507.8 KiB                   | 6.93 MiB / 1.97 MiB / 2.59 MiB                    |
| AOT         | 7.06 MiB / 1.76 MiB / 2.46 MiB            | 65.3 KiB / 18.7 KiB / 22.3 KiB          | 1.25 MiB / 307.2 KiB / 385.5 KiB                   | 11.25 MiB / 2.75 MiB / 3.83 MiB                   |

*(Note: Under AOT, `SudokuSpice.wasm` retains the same 65.3 KiB raw Webcil size, while IL stripping
reduces its compressed `.br` / `.gz` footprint.)*

For completeness, the BenchmarkDotNet WASM runner bundles (`wwwroot/_framework`) produced during the
benchmark runs—which also include `BenchmarkDotNet`, Roslyn, and the compared libraries
(`SudokuSharp`, `Sudoku.Solver.Lite`, `TinyCsvParser`)—had the following sizes (note that
BenchmarkDotNet sets `PublishTrimmed=false` for `monowasm10.0` interpreter builds and
`PublishTrimmed=true` for `monowasmaot10.0` AOT builds):

| Mode (`BenchmarkDotNet`) | `PublishTrimmed` | `dotnet.native.wasm` (Raw / `.br` / `.gz`) | Managed Assemblies (Count, Raw / `.br` / `.gz`) | Total `_framework` (Raw / `.br` / `.gz`) |
|------------------------- |----------------- |-------------------------------------------:|------------------------------------------------:|-----------------------------------------:|
| Interpreter              | `false`          | 2.87 MiB / 937.0 KiB / 1.13 MiB            | 211 files: 40.92 MiB / 11.43 MiB / 14.85 MiB    | 46.81 MiB / 13.05 MiB / 17.01 MiB        |
| AOT                      | `true`           | 11.86 MiB / 2.61 MiB / 3.84 MiB            | 32 files: 3.24 MiB / 801.0 KiB / 1,003.3 KiB    | 18.04 MiB / 4.08 MiB / 5.83 MiB          |
