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

|                 Method | sampleCollection |         Mean |        Error |        StdDev |       Median |  Ratio | RatioSD |     Gen 0 |   Gen 1 | Gen 2 | Allocated |
|----------------------- |----------------- |-------------:|-------------:|--------------:|-------------:|-------:|--------:|----------:|--------:|------:|----------:|
|            SudokuSpice |       Guesses: 0 |     24.62 us |     0.018 us |      0.016 us |     24.62 us |   1.00 |    0.00 |    1.8311 |  0.0305 |     - |     11 KB |
| SudokuSpiceConstraints |       Guesses: 0 |     91.09 us |     0.267 us |      0.250 us |     91.08 us |   3.70 |    0.01 |   23.8037 |  5.1270 |     - |    146 KB |
|            SudokuSharp |       Guesses: 0 |    773.20 us |    27.240 us |     80.319 us |    769.92 us |  32.84 |    3.00 |  134.7656 |       - |     - |    826 KB |
|       SudokuSolverLite |       Guesses: 0 |  1,275.40 us |    26.904 us |     78.480 us |  1,269.68 us |  52.54 |    2.88 |  369.1406 |       - |     - |  2,272 KB |
|                        |                  |              |              |               |              |        |         |           |         |       |           |
|            SudokuSpice |       Guesses: 1 |     39.66 us |     0.095 us |      0.089 us |     39.64 us |   1.00 |    0.00 |    3.2349 |  0.0610 |     - |     20 KB |
| SudokuSpiceConstraints |       Guesses: 1 |     96.54 us |     0.084 us |      0.074 us |     96.56 us |   2.44 |    0.01 |   24.1699 |  5.4932 |     - |    149 KB |
|            SudokuSharp |       Guesses: 1 |  1,679.44 us |    64.322 us |    188.647 us |  1,667.07 us |  41.01 |    4.73 |  257.8125 |       - |     - |  1,582 KB |
|       SudokuSolverLite |       Guesses: 1 |  4,128.47 us |   483.868 us |  1,419.102 us |  4,359.10 us |  96.10 |   34.44 | 1015.6250 |       - |     - |  6,250 KB |
|                        |                  |              |              |               |              |        |         |           |         |       |           |
|            SudokuSpice |     Guesses: 2-3 |     53.22 us |     0.369 us |      0.345 us |     53.17 us |   1.00 |    0.00 |    4.5166 |  0.1221 |     - |     28 KB |
| SudokuSpiceConstraints |     Guesses: 2-3 |    101.47 us |     0.182 us |      0.170 us |    101.44 us |   1.91 |    0.01 |   24.5361 |  5.4932 |     - |    151 KB |
|            SudokuSharp |     Guesses: 2-3 |  2,977.69 us |   234.770 us |    688.540 us |  2,942.15 us |  52.91 |   12.45 |  507.8125 |       - |     - |  3,126 KB |
|       SudokuSolverLite |     Guesses: 2-3 |  4,060.06 us |   353.668 us |  1,031.665 us |  3,749.84 us |  79.38 |   22.39 | 1070.3125 |       - |     - |  6,584 KB |
|                        |                  |              |              |               |              |        |         |           |         |       |           |
|            SudokuSpice |     Guesses: 4-7 |     74.20 us |     0.458 us |      0.406 us |     74.14 us |   1.00 |    0.00 |    6.7139 |  0.1221 |     - |     42 KB |
| SudokuSpiceConstraints |     Guesses: 4-7 |    109.59 us |     0.250 us |      0.234 us |    109.52 us |   1.48 |    0.01 |   25.2686 |  5.8594 |     - |    155 KB |
|            SudokuSharp |     Guesses: 4-7 |  3,689.11 us |   339.939 us |    980.802 us |  3,611.79 us |  45.00 |   10.07 |  757.8125 |       - |     - |  4,652 KB |
|       SudokuSolverLite |     Guesses: 4-7 |  9,189.61 us | 1,560.193 us |  4,476.487 us |  8,160.77 us | 116.08 |   50.04 | 5656.2500 |       - |     - | 34,721 KB |
|                        |                  |              |              |               |              |        |         |           |         |       |           |
|            SudokuSpice |      Guesses: 8+ |     96.08 us |     0.708 us |      0.627 us |     96.12 us |   1.00 |    0.00 |    9.0332 |  0.2441 |     - |     55 KB |
| SudokuSpiceConstraints |      Guesses: 8+ |    118.19 us |     0.465 us |      0.435 us |    118.10 us |   1.23 |    0.01 |   26.0010 |  5.9814 |     - |    159 KB |
|            SudokuSharp |      Guesses: 8+ |  5,765.75 us |   434.846 us |  1,275.329 us |  5,722.61 us |  60.38 |   11.01 |  914.0625 |       - |     - |  5,640 KB |
|       SudokuSolverLite |      Guesses: 8+ | 21,188.46 us | 3,797.147 us | 11,136.376 us | 20,671.38 us | 208.50 |  119.53 | 8593.7500 | 31.2500 |     - | 52,832 KB |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |         Median |    Ratio | RatioSD |       Gen 0 |  Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------------:|---------:|--------:|------------:|-------:|------:|-----------:|
|                SudokuSpice |   Easy |      10.457 us |     0.2056 us |     0.3202 us |      10.248 us |     1.00 |    0.00 |      1.3428 | 0.0153 |     - |       8 KB |
|   SudokuSpiceDynamicSingle |   Easy |      16.574 us |     0.0180 us |     0.0169 us |      16.573 us |     1.55 |    0.05 |      2.4719 | 0.0305 |     - |      15 KB |
| SudokuSpiceDynamicMultiple |   Easy |      22.312 us |     0.0487 us |     0.0407 us |      22.326 us |     2.08 |    0.06 |      2.4719 | 0.0305 |     - |      15 KB |
|     SudokuSpiceConstraints |   Easy |      35.328 us |     0.2146 us |     0.1902 us |      35.264 us |     3.30 |    0.11 |     11.0474 | 1.2817 |     - |      68 KB |
|                SudokuSharp |   Easy |       6.429 us |     0.0090 us |     0.0080 us |       6.429 us |     0.60 |    0.02 |      1.1063 |      - |     - |       7 KB |
|           SudokuSolverLite |   Easy |     155.924 us |     0.2321 us |     0.1938 us |     155.880 us |    14.54 |    0.42 |     44.4336 |      - |     - |     273 KB |
|                            |        |                |               |               |                |          |         |             |        |       |            |
|                SudokuSpice | Medium |      87.637 us |     0.2707 us |     0.2532 us |      87.630 us |     1.00 |    0.00 |      8.7891 | 0.1221 |     - |      54 KB |
|   SudokuSpiceDynamicSingle | Medium |     117.070 us |     0.3485 us |     0.3260 us |     117.100 us |     1.34 |    0.00 |     13.6719 | 0.3662 |     - |      84 KB |
| SudokuSpiceDynamicMultiple | Medium |     141.620 us |     0.8552 us |     0.7999 us |     141.388 us |     1.62 |    0.01 |     13.6719 | 0.2441 |     - |      84 KB |
|     SudokuSpiceConstraints | Medium |     101.941 us |     0.0602 us |     0.0563 us |     101.938 us |     1.16 |    0.00 |     26.4893 | 7.2021 |     - |     162 KB |
|                SudokuSharp | Medium |   3,002.384 us |    20.2430 us |    18.9353 us |   3,001.126 us |    34.26 |    0.22 |    156.2500 |      - |     - |     980 KB |
|           SudokuSolverLite | Medium |   2,325.372 us |     1.9788 us |     1.6524 us |   2,325.692 us |    26.54 |    0.07 |    664.0625 |      - |     - |   4,088 KB |
|                            |        |                |               |               |                |          |         |             |        |       |            |
|                SudokuSpice |  HardA |      73.360 us |     0.1872 us |     0.1660 us |      73.341 us |     1.00 |    0.00 |      7.6904 | 0.1221 |     - |      47 KB |
|   SudokuSpiceDynamicSingle |  HardA |      97.674 us |     0.1790 us |     0.1586 us |      97.698 us |     1.33 |    0.00 |     11.7188 | 0.2441 |     - |      72 KB |
| SudokuSpiceDynamicMultiple |  HardA |     121.551 us |     0.3210 us |     0.3003 us |     121.472 us |     1.66 |    0.00 |     11.8408 | 0.3662 |     - |      73 KB |
|     SudokuSpiceConstraints |  HardA |     112.208 us |     0.2908 us |     0.2578 us |     112.267 us |     1.53 |    0.00 |     25.5127 | 5.9814 |     - |     157 KB |
|                SudokuSharp |  HardA |   3,312.889 us |    65.2922 us |   124.2251 us |   3,316.820 us |    44.98 |    1.68 |    187.5000 |      - |     - |   1,171 KB |
|           SudokuSolverLite |  HardA |  24,351.124 us |    30.5310 us |    25.4948 us |  24,352.900 us |   331.98 |    0.61 |   6937.5000 |      - |     - |  42,554 KB |
|                            |        |                |               |               |                |          |         |             |        |       |            |
|                SudokuSpice |  HardB |     203.054 us |     0.6593 us |     0.5845 us |     203.047 us |     1.00 |    0.00 |     19.7754 | 0.4883 |     - |     122 KB |
|   SudokuSpiceDynamicSingle |  HardB |     252.102 us |     1.5836 us |     1.4813 us |     251.798 us |     1.24 |    0.01 |     28.8086 | 0.9766 |     - |     177 KB |
| SudokuSpiceDynamicMultiple |  HardB |     296.624 us |     1.6445 us |     1.5383 us |     296.733 us |     1.46 |    0.01 |     28.3203 | 0.9766 |     - |     175 KB |
|     SudokuSpiceConstraints |  HardB |     116.146 us |     0.0739 us |     0.0691 us |     116.135 us |     0.57 |    0.00 |     25.6348 | 6.3477 |     - |     158 KB |
|                SudokuSharp |  HardB |  24,235.698 us |   921.2907 us | 2,716.4465 us |  24,515.485 us |   120.56 |   10.60 |   1187.5000 |      - |     - |   7,443 KB |
|           SudokuSolverLite |  HardB |   4,769.852 us |     2.1037 us |     1.8649 us |   4,769.957 us |    23.49 |    0.07 |   1335.9375 |      - |     - |   8,220 KB |
|                            |        |                |               |               |                |          |         |             |        |       |            |
|                SudokuSpice |  EvilA |     129.035 us |     0.3143 us |     0.2940 us |     128.997 us |     1.00 |    0.00 |     14.1602 | 0.4883 |     - |      88 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     157.143 us |     0.4298 us |     0.3589 us |     157.155 us |     1.22 |    0.00 |     19.0430 | 0.7324 |     - |     117 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     182.056 us |     0.4037 us |     0.3776 us |     182.117 us |     1.41 |    0.00 |     19.0430 | 0.7324 |     - |     117 KB |
|     SudokuSpiceConstraints |  EvilA |     127.611 us |     0.1152 us |     0.1021 us |     127.628 us |     0.99 |    0.00 |     27.5879 | 7.3242 |     - |     170 KB |
|                SudokuSharp |  EvilA |  44,222.381 us | 2,374.9158 us | 7,002.4928 us |  43,466.514 us |   363.63 |   60.16 |   2000.0000 |      - |     - |  12,445 KB |
|           SudokuSolverLite |  EvilA | 389,934.224 us |   680.9262 us |   636.9388 us | 389,806.186 us | 3,021.95 |    8.58 | 113000.0000 |      - |     - | 693,269 KB |
|                            |        |                |               |               |                |          |         |             |        |       |            |
|                SudokuSpice |  EvilB |   1,242.159 us |    17.6814 us |    14.7648 us |   1,245.339 us |     1.00 |    0.00 |    105.4688 | 3.9063 |     - |     657 KB |
|   SudokuSpiceDynamicSingle |  EvilB |   1,682.427 us |    30.5766 us |    28.6013 us |   1,686.190 us |     1.35 |    0.03 |    187.5000 | 7.8125 |     - |   1,155 KB |
| SudokuSpiceDynamicMultiple |  EvilB |   2,002.124 us |    39.4302 us |    52.6382 us |   1,999.750 us |     1.61 |    0.05 |    179.6875 | 7.8125 |     - |   1,120 KB |
|     SudokuSpiceConstraints |  EvilB |     241.801 us |     0.5412 us |     0.4519 us |     241.923 us |     0.19 |    0.00 |     31.7383 | 9.7656 |     - |     195 KB |
|                SudokuSharp |  EvilB |  47,953.790 us |   904.8194 us |   888.6536 us |  48,039.616 us |    38.56 |    0.93 |   3300.0000 |      - |     - |  20,701 KB |
|           SudokuSolverLite |  EvilB |  49,730.231 us |    17.1160 us |    15.1729 us |  49,726.542 us |    40.04 |    0.48 |  14181.8182 |      - |     - |  86,909 KB |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|------:|------:|-----------:|
|                SudokuSpice |   Easy |       380.3 us |       0.54 us |       0.50 us |     1.00 |    0.00 |      1.4648 |     - |     - |       7 KB |
|   SudokuSpiceDynamicSingle |   Easy |       542.0 us |       0.35 us |       0.33 us |     1.43 |    0.00 |      2.9297 |     - |     - |      12 KB |
| SudokuSpiceDynamicMultiple |   Easy |       685.6 us |       0.54 us |       0.51 us |     1.80 |    0.00 |      2.9297 |     - |     - |      12 KB |
|     SudokuSpiceConstraints |   Easy |       682.9 us |       0.43 us |       0.40 us |     1.80 |    0.00 |     11.7188 |     - |     - |      49 KB |
|                SudokuSharp |   Easy |       104.2 us |       0.10 us |       0.09 us |     0.27 |    0.00 |      1.2207 |     - |     - |       5 KB |
|           SudokuSolverLite |   Easy |     3,569.2 us |       2.76 us |       2.58 us |     9.38 |    0.01 |     62.5000 |     - |     - |     261 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice | Medium |     2,943.6 us |      49.55 us |      46.35 us |     1.00 |    0.00 |      7.8125 |     - |     - |      47 KB |
|   SudokuSpiceDynamicSingle | Medium |     3,896.9 us |      74.80 us |      69.97 us |     1.32 |    0.03 |     15.6250 |     - |     - |      65 KB |
| SudokuSpiceDynamicMultiple | Medium |     4,292.0 us |     160.73 us |     150.35 us |     1.46 |    0.05 |     15.6250 |     - |     - |      66 KB |
|     SudokuSpiceConstraints | Medium |     1,533.4 us |       1.07 us |       1.00 us |     0.52 |    0.01 |     23.4375 |     - |     - |      98 KB |
|                SudokuSharp | Medium |    49,808.4 us |   1,632.67 us |   1,527.20 us |    16.93 |    0.59 |    100.0000 |     - |     - |     661 KB |
|           SudokuSolverLite | Medium |    52,703.0 us |      53.60 us |      44.76 us |    17.94 |    0.29 |    900.0000 |     - |     - |   3,769 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardA |     2,539.1 us |      17.32 us |      16.20 us |     1.00 |    0.00 |      7.8125 |     - |     - |      40 KB |
|   SudokuSpiceDynamicSingle |  HardA |     3,234.4 us |      20.81 us |      19.47 us |     1.27 |    0.01 |     11.7188 |     - |     - |      57 KB |
| SudokuSpiceDynamicMultiple |  HardA |     3,750.6 us |      27.86 us |      26.06 us |     1.48 |    0.01 |     11.7188 |     - |     - |      56 KB |
|     SudokuSpiceConstraints |  HardA |     1,756.9 us |       1.11 us |       1.04 us |     0.69 |    0.00 |     23.4375 |     - |     - |      95 KB |
|                SudokuSharp |  HardA |    58,777.6 us |  13,316.63 us |  12,456.38 us |    23.15 |    4.89 |    125.0000 |     - |     - |     758 KB |
|           SudokuSolverLite |  HardA |   570,260.1 us |     518.53 us |     459.67 us |   224.69 |    1.53 |  10000.0000 |     - |     - |  41,347 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardB |     6,197.9 us |     126.17 us |     118.02 us |     1.00 |    0.00 |     23.4375 |     - |     - |     102 KB |
|   SudokuSpiceDynamicSingle |  HardB |     7,705.6 us |     205.66 us |     192.38 us |     1.24 |    0.03 |     31.2500 |     - |     - |     131 KB |
| SudokuSpiceDynamicMultiple |  HardB |     8,794.9 us |     245.37 us |     229.52 us |     1.42 |    0.05 |     31.2500 |     - |     - |     131 KB |
|     SudokuSpiceConstraints |  HardB |     1,862.8 us |       1.33 us |       1.24 us |     0.30 |    0.01 |     23.4375 |     - |     - |      96 KB |
|                SudokuSharp |  HardB |   435,500.0 us |  58,365.58 us |  54,595.20 us |    70.29 |    9.00 |   1062.5000 |     - |     - |   4,535 KB |
|           SudokuSolverLite |  HardB |   111,325.3 us |     132.69 us |     124.12 us |    17.97 |    0.34 |   1800.0000 |     - |     - |   7,757 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilA |     4,119.4 us |      35.67 us |      31.62 us |     1.00 |    0.00 |     15.6250 |     - |     - |      75 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     4,995.4 us |      42.34 us |      39.60 us |     1.21 |    0.01 |     15.6250 |     - |     - |      92 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     5,453.9 us |      58.73 us |      54.93 us |     1.32 |    0.02 |     15.6250 |     - |     - |      93 KB |
|     SudokuSpiceConstraints |  EvilA |     2,029.7 us |       1.53 us |       1.36 us |     0.49 |    0.00 |     23.4375 |     - |     - |     102 KB |
|                SudokuSharp |  EvilA |   872,343.7 us | 401,228.21 us | 375,309.11 us |   210.20 |   94.29 |   3000.0000 |     - |     - |  13,438 KB |
|           SudokuSolverLite |  EvilA | 9,519,133.3 us |  11,418.07 us |  10,680.47 us | 2,310.90 |   18.59 | 166000.0000 |     - |     - | 674,507 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilB |    37,857.0 us |   6,232.24 us |   5,829.64 us |     1.00 |    0.00 |     71.4286 |     - |     - |     518 KB |
|   SudokuSpiceDynamicSingle |  EvilB |    53,678.6 us |  14,997.72 us |  14,028.87 us |     1.45 |    0.46 |    142.8571 |     - |     - |     865 KB |
| SudokuSpiceDynamicMultiple |  EvilB |    57,178.3 us |  12,071.24 us |  11,291.45 us |     1.55 |    0.44 |    222.2222 |     - |     - |   1,041 KB |
|     SudokuSpiceConstraints |  EvilB |     4,116.9 us |       2.95 us |       2.46 us |     0.11 |    0.02 |     23.4375 |     - |     - |     117 KB |
|                SudokuSharp |  EvilB |   908,950.6 us |  52,559.92 us |  49,164.58 us |    24.65 |    4.57 |   3000.0000 |     - |     - |  12,294 KB |
|           SudokuSolverLite |  EvilB | 1,168,247.3 us |   1,043.59 us |     976.17 us |    31.58 |    5.04 |  20000.0000 |     - |     - |  83,411 KB |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |   Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|--------:|------:|----------:|
|            SudokuSpice |  1.192 ms | 0.0077 ms | 0.0068 ms |  1.00 |    0.00 |  150.3906 | 17.5781 |     - |    933 KB |
| SudokuSpiceConstraints |  3.059 ms | 0.0309 ms | 0.0289 ms |  2.57 |    0.03 |  785.1563 | 15.6250 |     - |  4,832 KB |
|     SudokuSharpSingles | 15.672 ms | 0.8732 ms | 2.5334 ms | 13.01 |    2.53 | 2968.7500 |       - |     - | 18,311 KB |
|       SudokuSharpMixed |  7.331 ms | 0.2473 ms | 0.7134 ms |  6.26 |    0.63 | 1437.5000 |  7.8125 |     - |  8,826 KB |

### WASM

|                 Method |      Mean |      Error |     StdDev |    Median | Ratio | RatioSD |      Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|-----------:|-----------:|----------:|------:|--------:|-----------:|------:|------:|----------:|
|            SudokuSpice |  39.28 ms |   1.325 ms |   1.175 ms |  39.30 ms |  1.00 |    0.00 |   166.6667 |     - |     - |    815 KB |
| SudokuSpiceConstraints |  49.32 ms |   2.627 ms |   2.194 ms |  48.93 ms |  1.26 |    0.04 |   800.0000 |     - |     - |  3,245 KB |
|     SudokuSharpSingles | 245.35 ms | 111.026 ms | 103.854 ms | 186.40 ms |  6.43 |    2.70 | 10666.6667 |     - |     - | 44,361 KB |
|       SudokuSharpMixed | 116.15 ms |  50.449 ms |  47.190 ms | 107.60 ms |  2.93 |    1.23 |  1000.0000 |     - |     - |  4,731 KB |

