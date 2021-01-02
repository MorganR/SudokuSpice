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
|            SudokuSpice |       Guesses: 0 |     24.25 us |     0.020 us |      0.018 us |     24.24 us |   1.00 |    0.00 |    1.8005 | 0.0305 |     - |     11 KB |
| SudokuSpiceConstraints |       Guesses: 0 |     70.80 us |     0.209 us |      0.196 us |     70.79 us |   2.92 |    0.01 |   17.8223 | 3.6621 |     - |    109 KB |
|            SudokuSharp |       Guesses: 0 |    775.44 us |    27.336 us |     80.172 us |    768.45 us |  32.33 |    3.10 |  133.7891 |      - |     - |    822 KB |
|       SudokuSolverLite |       Guesses: 0 |  1,276.20 us |    29.084 us |     85.754 us |  1,282.51 us |  52.85 |    3.21 |  386.7188 |      - |     - |  2,376 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |       Guesses: 1 |     38.82 us |     0.111 us |      0.103 us |     38.84 us |   1.00 |    0.00 |    3.2349 | 0.0610 |     - |     20 KB |
| SudokuSpiceConstraints |       Guesses: 1 |     74.41 us |     0.127 us |      0.119 us |     74.42 us |   1.92 |    0.01 |   18.1885 | 3.7842 |     - |    112 KB |
|            SudokuSharp |       Guesses: 1 |  1,707.22 us |    58.389 us |    172.161 us |  1,687.12 us |  41.79 |    5.31 |  228.5156 |      - |     - |  1,409 KB |
|       SudokuSolverLite |       Guesses: 1 |  3,950.60 us |   519.854 us |  1,524.642 us |  3,743.92 us |  92.86 |   39.60 | 1187.5000 |      - |     - |  7,318 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 2-3 |     52.75 us |     0.240 us |      0.225 us |     52.74 us |   1.00 |    0.00 |    4.5776 | 0.1221 |     - |     28 KB |
| SudokuSpiceConstraints |     Guesses: 2-3 |     77.94 us |     0.179 us |      0.167 us |     77.95 us |   1.48 |    0.01 |   18.5547 | 3.9063 |     - |    114 KB |
|            SudokuSharp |     Guesses: 2-3 |  2,626.79 us |   173.856 us |    512.619 us |  2,594.24 us |  49.65 |    7.38 |  355.4688 |      - |     - |  2,191 KB |
|       SudokuSolverLite |     Guesses: 2-3 |  4,145.32 us |   289.225 us |    843.683 us |  4,051.14 us |  74.15 |   16.51 | 1316.4063 | 3.9063 |     - |  8,083 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |     Guesses: 4-7 |     73.62 us |     0.416 us |      0.369 us |     73.62 us |   1.00 |    0.00 |    6.7139 | 0.1221 |     - |     41 KB |
| SudokuSpiceConstraints |     Guesses: 4-7 |     82.57 us |     0.155 us |      0.145 us |     82.57 us |   1.12 |    0.01 |   19.1650 | 4.2725 |     - |    118 KB |
|            SudokuSharp |     Guesses: 4-7 |  3,800.08 us |   397.258 us |  1,146.181 us |  3,727.46 us |  52.43 |   22.41 |  429.6875 |      - |     - |  2,657 KB |
|       SudokuSolverLite |     Guesses: 4-7 |  9,806.37 us | 1,716.165 us |  5,006.134 us |  8,248.49 us | 144.68 |   75.12 | 1500.0000 |      - |     - |  9,257 KB |
|                        |                  |              |              |               |              |        |         |           |        |       |           |
|            SudokuSpice |      Guesses: 8+ |     98.87 us |     0.710 us |      0.630 us |     98.85 us |   1.00 |    0.00 |    8.7891 | 0.2441 |     - |     54 KB |
| SudokuSpiceConstraints |      Guesses: 8+ |     88.42 us |     0.162 us |      0.135 us |     88.47 us |   0.89 |    0.01 |   19.7754 | 4.1504 |     - |    121 KB |
|            SudokuSharp |      Guesses: 8+ |  6,108.66 us |   400.092 us |  1,167.087 us |  5,983.35 us |  59.46 |    9.65 |  984.3750 |      - |     - |  6,041 KB |
|       SudokuSolverLite |      Guesses: 8+ | 22,264.61 us | 3,654.738 us | 10,776.077 us | 19,485.97 us | 260.88 |  133.23 | 3906.2500 |      - |     - | 23,945 KB |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 |  Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|-------:|------:|-----------:|
|                SudokuSpice |   Easy |      10.116 us |     0.0183 us |     0.0171 us |     1.00 |    0.00 |      1.4343 | 0.0153 |     - |       9 KB |
|   SudokuSpiceDynamicSingle |   Easy |      16.106 us |     0.0343 us |     0.0320 us |     1.59 |    0.00 |      2.5635 | 0.0610 |     - |      16 KB |
| SudokuSpiceDynamicMultiple |   Easy |      21.653 us |     0.0298 us |     0.0279 us |     2.14 |    0.00 |      2.5635 | 0.0610 |     - |      16 KB |
|     SudokuSpiceConstraints |   Easy |      25.309 us |     0.0637 us |     0.0565 us |     2.50 |    0.01 |      7.8735 | 0.8240 |     - |      48 KB |
|                SudokuSharp |   Easy |       6.474 us |     0.0135 us |     0.0119 us |     0.64 |    0.00 |      1.1063 |      - |     - |       7 KB |
|           SudokuSolverLite |   Easy |     153.925 us |     0.0758 us |     0.0709 us |    15.22 |    0.03 |     44.4336 |      - |     - |     273 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice | Medium |      86.595 us |     0.2633 us |     0.2199 us |     1.00 |    0.00 |      8.7891 | 0.2441 |     - |      55 KB |
|   SudokuSpiceDynamicSingle | Medium |     116.342 us |     0.4153 us |     0.3885 us |     1.34 |    0.01 |     13.7939 | 0.4883 |     - |      85 KB |
| SudokuSpiceDynamicMultiple | Medium |     140.531 us |     0.8053 us |     0.7532 us |     1.62 |    0.01 |     13.6719 | 0.4883 |     - |      85 KB |
|     SudokuSpiceConstraints | Medium |      77.995 us |     0.4537 us |     0.4244 us |     0.90 |    0.01 |     20.0195 | 5.0049 |     - |     123 KB |
|                SudokuSharp | Medium |   2,982.225 us |    16.0729 us |    15.0346 us |    34.45 |    0.20 |    160.1563 |      - |     - |     996 KB |
|           SudokuSolverLite | Medium |   2,295.745 us |     3.4564 us |     3.2331 us |    26.51 |    0.10 |    664.0625 |      - |     - |   4,088 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardA |      73.501 us |     0.1952 us |     0.1730 us |     1.00 |    0.00 |      7.6904 | 0.2441 |     - |      48 KB |
|   SudokuSpiceDynamicSingle |  HardA |      98.606 us |     0.2451 us |     0.2292 us |     1.34 |    0.00 |     11.9629 | 0.3662 |     - |      73 KB |
| SudokuSpiceDynamicMultiple |  HardA |     120.036 us |     0.2488 us |     0.2328 us |     1.63 |    0.00 |     11.8408 | 0.3662 |     - |      73 KB |
|     SudokuSpiceConstraints |  HardA |      81.997 us |     0.2031 us |     0.1900 us |     1.12 |    0.00 |     19.2871 | 4.6387 |     - |     118 KB |
|                SudokuSharp |  HardA |   3,249.953 us |    63.0602 us |    81.9961 us |    44.29 |    1.30 |    207.0313 |      - |     - |   1,274 KB |
|           SudokuSolverLite |  HardA |  24,396.134 us |    44.7450 us |    41.8545 us |   332.00 |    0.71 |   6937.5000 |      - |     - |  42,554 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardB |     202.309 us |     1.0462 us |     0.9786 us |     1.00 |    0.00 |     20.0195 | 0.7324 |     - |     123 KB |
|   SudokuSpiceDynamicSingle |  HardB |     254.625 us |     1.2749 us |     1.1925 us |     1.26 |    0.01 |     28.3203 | 0.9766 |     - |     176 KB |
| SudokuSpiceDynamicMultiple |  HardB |     297.025 us |     1.2837 us |     1.2007 us |     1.47 |    0.01 |     28.8086 | 0.9766 |     - |     177 KB |
|     SudokuSpiceConstraints |  HardB |      93.212 us |     0.1403 us |     0.1313 us |     0.46 |    0.00 |     19.8975 | 4.7607 |     - |     122 KB |
|                SudokuSharp |  HardB |  22,930.976 us |   758.8910 us | 2,237.6072 us |   119.54 |   11.31 |   1375.0000 |      - |     - |   8,490 KB |
|           SudokuSolverLite |  HardB |   4,756.807 us |     6.4531 us |     5.7205 us |    23.53 |    0.10 |   1335.9375 |      - |     - |   8,220 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilA |     130.534 us |     0.4609 us |     0.4086 us |     1.00 |    0.00 |     14.4043 | 0.4883 |     - |      88 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     157.789 us |     0.2583 us |     0.2416 us |     1.21 |    0.00 |     19.0430 | 0.7324 |     - |     117 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     185.165 us |     0.5524 us |     0.4613 us |     1.42 |    0.01 |     19.0430 | 0.7324 |     - |     117 KB |
|     SudokuSpiceConstraints |  EvilA |      90.420 us |     0.2824 us |     0.2503 us |     0.69 |    0.00 |     20.9961 | 5.3711 |     - |     129 KB |
|                SudokuSharp |  EvilA |  42,282.844 us | 2,100.5361 us | 6,127.3623 us |   327.83 |   45.72 |   2500.0000 |      - |     - |  15,509 KB |
|           SudokuSolverLite |  EvilA | 392,781.056 us |   415.4553 us |   388.6172 us | 3,008.68 |   10.70 | 113000.0000 |      - |     - | 693,269 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilB |   1,234.873 us |    23.7270 us |    25.3876 us |     1.00 |    0.00 |    111.3281 | 3.9063 |     - |     683 KB |
|   SudokuSpiceDynamicSingle |  EvilB |   1,657.735 us |    32.0099 us |    42.7322 us |     1.35 |    0.04 |    181.6406 | 9.7656 |     - |   1,115 KB |
| SudokuSpiceDynamicMultiple |  EvilB |   1,991.499 us |    38.9623 us |    40.0114 us |     1.62 |    0.05 |    175.7813 | 7.8125 |     - |   1,090 KB |
|     SudokuSpiceConstraints |  EvilB |     238.721 us |     0.5477 us |     0.5123 us |     0.19 |    0.00 |     26.1230 | 6.3477 |     - |     161 KB |
|                SudokuSharp |  EvilB |  48,261.270 us |   899.3915 us |   923.6089 us |    39.13 |    1.10 |   3090.9091 |      - |     - |  19,472 KB |
|           SudokuSolverLite |  EvilB |  50,326.188 us |   131.4096 us |   116.4912 us |    40.72 |    0.88 |  14100.0000 |      - |     - |  86,909 KB |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|------:|------:|-----------:|
|                SudokuSpice |   Easy |       379.3 us |       0.26 us |       0.22 us |     1.00 |    0.00 |      1.9531 |     - |     - |       8 KB |
|   SudokuSpiceDynamicSingle |   Easy |       547.2 us |       0.69 us |       0.61 us |     1.44 |    0.00 |      2.9297 |     - |     - |      12 KB |
| SudokuSpiceDynamicMultiple |   Easy |       691.5 us |       0.75 us |       0.66 us |     1.82 |    0.00 |      2.9297 |     - |     - |      12 KB |
|     SudokuSpiceConstraints |   Easy |       467.4 us |       0.49 us |       0.46 us |     1.23 |    0.00 |      8.7891 |     - |     - |      37 KB |
|                SudokuSharp |   Easy |       103.9 us |       0.08 us |       0.07 us |     0.27 |    0.00 |      1.2207 |     - |     - |       5 KB |
|           SudokuSolverLite |   Easy |     3,615.7 us |       2.98 us |       2.48 us |     9.53 |    0.01 |     62.5000 |     - |     - |     261 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice | Medium |     4,866.1 us |     169.42 us |     158.47 us |     1.00 |    0.00 |      7.8125 |     - |     - |      46 KB |
|   SudokuSpiceDynamicSingle | Medium |     5,678.9 us |      71.07 us |      55.49 us |     1.17 |    0.04 |     15.6250 |     - |     - |      66 KB |
| SudokuSpiceDynamicMultiple | Medium |     6,265.5 us |     173.15 us |     161.96 us |     1.29 |    0.06 |     15.6250 |     - |     - |      67 KB |
|     SudokuSpiceConstraints | Medium |     1,034.9 us |       0.59 us |       0.55 us |     0.21 |    0.01 |     17.5781 |     - |     - |      74 KB |
|                SudokuSharp | Medium |    49,539.4 us |   1,861.26 us |   1,741.02 us |    10.19 |    0.45 |    100.0000 |     - |     - |     673 KB |
|           SudokuSolverLite | Medium |    53,718.8 us |      41.86 us |      37.11 us |    11.08 |    0.35 |    900.0000 |     - |     - |   3,769 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardA |     4,111.1 us |      77.47 us |      68.68 us |     1.00 |    0.00 |      7.8125 |     - |     - |      41 KB |
|   SudokuSpiceDynamicSingle |  HardA |     4,846.4 us |      77.48 us |      72.47 us |     1.18 |    0.03 |      7.8125 |     - |     - |      58 KB |
| SudokuSpiceDynamicMultiple |  HardA |     5,327.0 us |      80.01 us |      74.85 us |     1.30 |    0.02 |      7.8125 |     - |     - |      57 KB |
|     SudokuSpiceConstraints |  HardA |     1,190.7 us |       1.09 us |       0.97 us |     0.29 |    0.00 |     17.5781 |     - |     - |      71 KB |
|                SudokuSharp |  HardA |    56,583.7 us |   7,965.48 us |   7,450.91 us |    13.83 |    1.85 |    111.1111 |     - |     - |     805 KB |
|           SudokuSolverLite |  HardA |   578,151.9 us |     386.35 us |     322.62 us |   140.86 |    2.33 |  10000.0000 |     - |     - |  41,347 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardB |     8,876.0 us |     221.90 us |     207.57 us |     1.00 |    0.00 |     15.6250 |     - |     - |     104 KB |
|   SudokuSpiceDynamicSingle |  HardB |    10,570.3 us |     260.39 us |     243.57 us |     1.19 |    0.05 |     31.2500 |     - |     - |     136 KB |
| SudokuSpiceDynamicMultiple |  HardB |    11,418.9 us |     364.38 us |     340.84 us |     1.29 |    0.04 |     31.2500 |     - |     - |     140 KB |
|     SudokuSpiceConstraints |  HardB |     1,408.4 us |       1.59 us |       1.49 us |     0.16 |    0.00 |     17.5781 |     - |     - |      74 KB |
|                SudokuSharp |  HardB |   434,011.2 us |  55,253.34 us |  51,684.01 us |    48.94 |    6.12 |   1500.0000 |     - |     - |   6,343 KB |
|           SudokuSolverLite |  HardB |   110,791.2 us |      87.10 us |      77.21 us |    12.46 |    0.28 |   1800.0000 |     - |     - |   7,757 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilA |     6,241.7 us |      83.09 us |      77.72 us |     1.00 |    0.00 |     15.6250 |     - |     - |      77 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     6,983.8 us |      85.88 us |      80.33 us |     1.12 |    0.02 |     15.6250 |     - |     - |      93 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     7,537.5 us |     103.32 us |      96.65 us |     1.21 |    0.02 |     15.6250 |     - |     - |      93 KB |
|     SudokuSpiceConstraints |  EvilA |     1,242.5 us |       0.98 us |       0.87 us |     0.20 |    0.00 |     17.5781 |     - |     - |      77 KB |
|                SudokuSharp |  EvilA |   800,264.9 us | 472,269.99 us | 441,761.64 us |   128.26 |   71.05 |   4000.0000 |     - |     - |  19,023 KB |
|           SudokuSolverLite |  EvilA | 9,189,591.2 us |  16,574.60 us |  15,503.89 us | 1,472.51 |   18.65 | 166000.0000 |     - |     - | 674,507 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilB |    56,656.6 us |  10,930.21 us |  10,224.12 us |     1.00 |    0.00 |    100.0000 |     - |     - |     563 KB |
|   SudokuSpiceDynamicSingle |  EvilB |    73,722.3 us |  11,544.51 us |  10,798.74 us |     1.34 |    0.31 |    142.8571 |     - |     - |     795 KB |
| SudokuSpiceDynamicMultiple |  EvilB |    80,151.3 us |  11,577.61 us |  10,263.25 us |     1.49 |    0.38 |    166.6667 |     - |     - |     776 KB |
|     SudokuSpiceConstraints |  EvilB |     4,047.5 us |       2.66 us |       2.35 us |     0.07 |    0.02 |     23.4375 |     - |     - |      98 KB |
|                SudokuSharp |  EvilB |   910,401.9 us |  64,283.52 us |  60,130.84 us |    16.62 |    3.47 |   3000.0000 |     - |     - |  13,358 KB |
|           SudokuSolverLite |  EvilB | 1,184,352.9 us |   1,365.28 us |   1,210.28 us |    21.91 |    4.41 |  20000.0000 |     - |     - |  83,411 KB |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |   Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|--------:|------:|----------:|
|            SudokuSpice |  1.196 ms | 0.0127 ms | 0.0113 ms |  1.00 |    0.00 |  150.3906 | 19.5313 |     - |    923 KB |
| SudokuSpiceConstraints |  2.342 ms | 0.0438 ms | 0.0410 ms |  1.96 |    0.04 |  597.6563 | 15.6250 |     - |  3,675 KB |
|     SudokuSharpSingles | 14.418 ms | 0.6116 ms | 1.8032 ms | 11.63 |    1.46 | 2671.8750 | 15.6250 |     - | 16,443 KB |
|       SudokuSharpMixed |  7.158 ms | 0.2503 ms | 0.7380 ms |  6.17 |    0.66 | 1382.8125 |  7.8125 |     - |  8,485 KB |

### WASM

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|------:|------:|----------:|
|            SudokuSpice |  55.81 ms |  2.775 ms |  2.596 ms |  1.00 |    0.00 |  125.0000 |     - |     - |    785 KB |
| SudokuSpiceConstraints |  34.35 ms |  1.545 ms |  1.369 ms |  0.61 |    0.04 |  600.0000 |     - |     - |  2,577 KB |
|     SudokuSharpSingles | 208.97 ms | 71.522 ms | 66.902 ms |  3.75 |    1.18 | 1750.0000 |     - |     - |  7,573 KB |
|       SudokuSharpMixed | 117.48 ms | 62.132 ms | 55.078 ms |  2.11 |    1.06 | 1250.0000 |     - |     - |  5,987 KB |
