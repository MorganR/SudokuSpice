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
|            SudokuSpice |       Guesses: 0 |     24.75 us |     0.025 us |      0.022 us |     24.76 us |   1.00 |    0.00 |    1.8311 |  0.0305 |     - |     11 KB |
| SudokuSpiceConstraints |       Guesses: 0 |    154.69 us |     0.456 us |      0.426 us |    154.70 us |   6.25 |    0.02 |   37.3535 |  9.5215 |     - |    229 KB |
|            SudokuSharp |       Guesses: 0 |    770.34 us |    27.576 us |     81.310 us |    766.93 us |  30.97 |    2.98 |  120.1172 |       - |     - |    738 KB |
|       SudokuSolverLite |       Guesses: 0 |  1,255.94 us |    25.052 us |     73.079 us |  1,255.68 us |  51.42 |    2.54 |  341.7969 |       - |     - |  2,094 KB |
|                        |                  |              |              |               |              |        |         |           |         |       |           |
|            SudokuSpice |       Guesses: 1 |     39.52 us |     0.175 us |      0.155 us |     39.49 us |   1.00 |    0.00 |    3.2349 |  0.0610 |     - |     20 KB |
| SudokuSpiceConstraints |       Guesses: 1 |    164.45 us |     0.403 us |      0.336 us |    164.49 us |   4.16 |    0.02 |   37.8418 | 10.2539 |     - |    233 KB |
|            SudokuSharp |       Guesses: 1 |  1,676.18 us |    86.211 us |    250.115 us |  1,668.84 us |  42.58 |    5.79 |  214.8438 |       - |     - |  1,325 KB |
|       SudokuSolverLite |       Guesses: 1 |  4,002.10 us |   401.688 us |  1,184.387 us |  3,781.42 us | 103.39 |   30.60 | 1324.2188 |  3.9063 |     - |  8,119 KB |
|                        |                  |              |              |               |              |        |         |           |         |       |           |
|            SudokuSpice |     Guesses: 2-3 |     52.99 us |     0.210 us |      0.196 us |     52.99 us |   1.00 |    0.00 |    4.5776 |  0.1221 |     - |     28 KB |
| SudokuSpiceConstraints |     Guesses: 2-3 |    173.70 us |     0.595 us |      0.557 us |    173.90 us |   3.28 |    0.02 |   38.3301 | 10.4980 |     - |    236 KB |
|            SudokuSharp |     Guesses: 2-3 |  2,806.25 us |   225.805 us |    662.247 us |  2,751.38 us |  53.44 |    7.43 |  335.9375 |       - |     - |  2,073 KB |
|       SudokuSolverLite |     Guesses: 2-3 |  3,976.41 us |   384.816 us |  1,110.282 us |  3,682.02 us |  75.65 |   16.32 |  906.2500 |       - |     - |  5,595 KB |
|                        |                  |              |              |               |              |        |         |           |         |       |           |
|            SudokuSpice |     Guesses: 4-7 |     73.73 us |     0.622 us |      0.551 us |     73.80 us |   1.00 |    0.00 |    6.7139 |  0.1221 |     - |     42 KB |
| SudokuSpiceConstraints |     Guesses: 4-7 |    186.14 us |     0.724 us |      0.677 us |    186.02 us |   2.52 |    0.02 |   39.3066 | 10.7422 |     - |    242 KB |
|            SudokuSharp |     Guesses: 4-7 |  3,813.63 us |   260.745 us |    760.606 us |  3,777.79 us |  51.13 |   10.16 |  507.8125 |       - |     - |  3,126 KB |
|       SudokuSolverLite |     Guesses: 4-7 | 10,909.14 us | 1,347.115 us |  3,950.854 us | 10,640.09 us | 161.89 |   63.31 | 3531.2500 |       - |     - | 21,727 KB |
|                        |                  |              |              |               |              |        |         |           |         |       |           |
|            SudokuSpice |      Guesses: 8+ |     96.41 us |     0.745 us |      0.697 us |     96.56 us |   1.00 |    0.00 |    9.0332 |  0.2441 |     - |     55 KB |
| SudokuSpiceConstraints |      Guesses: 8+ |    202.48 us |     0.812 us |      0.759 us |    202.50 us |   2.10 |    0.02 |   40.5273 | 10.7422 |     - |    248 KB |
|            SudokuSharp |      Guesses: 8+ |  5,891.01 us |   523.170 us |  1,517.811 us |  6,024.78 us |  57.27 |   18.65 |  671.8750 |       - |     - |  4,128 KB |
|       SudokuSolverLite |      Guesses: 8+ | 21,511.61 us | 4,838.046 us | 14,189.152 us | 17,388.55 us | 268.43 |  172.00 | 4625.0000 |       - |     - | 28,511 KB |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 |   Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|--------:|------:|-----------:|
|                SudokuSpice |   Easy |      10.158 us |     0.0270 us |     0.0240 us |     1.00 |    0.00 |      1.3428 |  0.0153 |     - |       8 KB |
|   SudokuSpiceDynamicSingle |   Easy |      16.351 us |     0.0206 us |     0.0183 us |     1.61 |    0.00 |      2.4719 |  0.0305 |     - |      15 KB |
| SudokuSpiceDynamicMultiple |   Easy |      21.453 us |     0.0178 us |     0.0149 us |     2.11 |    0.01 |      2.4719 |  0.0305 |     - |      15 KB |
|     SudokuSpiceConstraints |   Easy |      64.807 us |     0.1290 us |     0.1207 us |     6.38 |    0.02 |     17.7002 |  3.1738 |     - |     109 KB |
|                SudokuSharp |   Easy |       6.447 us |     0.0084 us |     0.0078 us |     0.63 |    0.00 |      1.1063 |       - |     - |       7 KB |
|           SudokuSolverLite |   Easy |     158.655 us |     0.1380 us |     0.1290 us |    15.62 |    0.03 |     44.4336 |       - |     - |     273 KB |
|                            |        |                |               |               |          |         |             |         |       |            |
|                SudokuSpice | Medium |      87.794 us |     0.3568 us |     0.3163 us |     1.00 |    0.00 |      8.6670 |  0.1221 |     - |      54 KB |
|   SudokuSpiceDynamicSingle | Medium |     118.504 us |     0.6143 us |     0.5746 us |     1.35 |    0.01 |     13.7939 |  0.3662 |     - |      85 KB |
| SudokuSpiceDynamicMultiple | Medium |     140.904 us |     0.7933 us |     0.7420 us |     1.61 |    0.01 |     13.6719 |  0.2441 |     - |      84 KB |
|     SudokuSpiceConstraints | Medium |     182.490 us |     0.3336 us |     0.2957 us |     2.08 |    0.01 |     41.0156 |  0.7324 |     - |     252 KB |
|                SudokuSharp | Medium |   3,007.459 us |    12.5718 us |    10.4980 us |    34.26 |    0.20 |    160.1563 |       - |     - |   1,005 KB |
|           SudokuSolverLite | Medium |   2,336.126 us |     2.0409 us |     1.8092 us |    26.61 |    0.10 |    664.0625 |       - |     - |   4,088 KB |
|                            |        |                |               |               |          |         |             |         |       |            |
|                SudokuSpice |  HardA |      73.492 us |     0.1790 us |     0.1674 us |     1.00 |    0.00 |      7.6904 |  0.1221 |     - |      47 KB |
|   SudokuSpiceDynamicSingle |  HardA |      98.522 us |     0.3184 us |     0.2978 us |     1.34 |    0.00 |     11.7188 |  0.3662 |     - |      73 KB |
| SudokuSpiceDynamicMultiple |  HardA |     119.174 us |     0.3531 us |     0.3303 us |     1.62 |    0.00 |     11.7188 |  0.3662 |     - |      73 KB |
|     SudokuSpiceConstraints |  HardA |     197.205 us |     0.2377 us |     0.2223 us |     2.68 |    0.01 |     40.0391 | 13.4277 |     - |     245 KB |
|                SudokuSharp |  HardA |   3,260.355 us |    63.6643 us |    84.9900 us |    44.49 |    1.21 |    199.2188 |       - |     - |   1,239 KB |
|           SudokuSolverLite |  HardA |  24,382.197 us |    27.4409 us |    25.6683 us |   331.77 |    0.81 |   6937.5000 |       - |     - |  42,554 KB |
|                            |        |                |               |               |          |         |             |         |       |            |
|                SudokuSpice |  HardB |     203.399 us |     0.4330 us |     0.4051 us |     1.00 |    0.00 |     20.0195 |  0.4883 |     - |     123 KB |
|   SudokuSpiceDynamicSingle |  HardB |     254.824 us |     1.3124 us |     1.2276 us |     1.25 |    0.01 |     28.3203 |  0.9766 |     - |     176 KB |
| SudokuSpiceDynamicMultiple |  HardB |     299.901 us |     1.3314 us |     1.1802 us |     1.47 |    0.00 |     28.3203 |  0.9766 |     - |     176 KB |
|     SudokuSpiceConstraints |  HardB |     210.586 us |     0.5468 us |     0.5115 us |     1.04 |    0.00 |     40.2832 |  0.7324 |     - |     247 KB |
|                SudokuSharp |  HardB |  25,480.494 us |   705.7288 us | 2,058.6441 us |   121.00 |   12.61 |   1500.0000 |       - |     - |   9,306 KB |
|           SudokuSolverLite |  HardB |   4,719.575 us |     4.3830 us |     4.0998 us |    23.20 |    0.06 |   1335.9375 |       - |     - |   8,220 KB |
|                            |        |                |               |               |          |         |             |         |       |            |
|                SudokuSpice |  EvilA |     130.907 us |     0.4101 us |     0.3635 us |     1.00 |    0.00 |     14.1602 |  0.4883 |     - |      88 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     161.453 us |     0.3014 us |     0.2820 us |     1.23 |    0.00 |     18.7988 |  0.7324 |     - |     117 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     179.417 us |     0.3978 us |     0.3526 us |     1.37 |    0.00 |     18.7988 |  0.7324 |     - |     116 KB |
|     SudokuSpiceConstraints |  EvilA |     232.063 us |     0.2914 us |     0.2726 us |     1.77 |    0.01 |     42.9688 |  0.4883 |     - |     263 KB |
|                SudokuSharp |  EvilA |  41,823.831 us | 2,550.4830 us | 7,480.1251 us |   321.35 |   46.34 |   2300.0000 |       - |     - |  14,395 KB |
|           SudokuSolverLite |  EvilA | 398,415.525 us |   573.0408 us |   478.5150 us | 3,043.73 |    8.57 | 113000.0000 |       - |     - | 693,265 KB |
|                            |        |                |               |               |          |         |             |         |       |            |
|                SudokuSpice |  EvilB |   1,237.302 us |    23.7752 us |    31.7392 us |     1.00 |    0.00 |    105.4688 |  3.9063 |     - |     652 KB |
|   SudokuSpiceDynamicSingle |  EvilB |   1,656.890 us |    26.4552 us |    24.7462 us |     1.34 |    0.03 |    181.6406 |  7.8125 |     - |   1,115 KB |
| SudokuSpiceDynamicMultiple |  EvilB |   2,207.018 us |    43.5230 us |    73.9054 us |     1.78 |    0.08 |    171.8750 |  7.8125 |     - |   1,076 KB |
|     SudokuSpiceConstraints |  EvilB |   1,153.562 us |     1.2554 us |     1.1743 us |     0.93 |    0.02 |     64.4531 |  9.7656 |     - |     407 KB |
|                SudokuSharp |  EvilB |  48,009.216 us |   955.7749 us |   981.5105 us |    38.83 |    1.19 |   3363.6364 |       - |     - |  20,877 KB |
|           SudokuSolverLite |  EvilB |  50,153.257 us |    45.3782 us |    42.4468 us |    40.43 |    0.98 |  14181.8182 |       - |     - |  86,910 KB |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|------:|------:|-----------:|
|                SudokuSpice |   Easy |       402.4 us |       0.40 us |       0.35 us |     1.00 |    0.00 |      1.4648 |     - |     - |       7 KB |
|   SudokuSpiceDynamicSingle |   Easy |       561.3 us |       0.71 us |       0.63 us |     1.39 |    0.00 |      2.9297 |     - |     - |      12 KB |
| SudokuSpiceDynamicMultiple |   Easy |       704.8 us |       0.49 us |       0.46 us |     1.75 |    0.00 |      2.9297 |     - |     - |      12 KB |
|     SudokuSpiceConstraints |   Easy |       476.7 us |       0.62 us |       0.58 us |     1.18 |    0.00 |      9.2773 |     - |     - |      38 KB |
|                SudokuSharp |   Easy |       103.9 us |       0.05 us |       0.04 us |     0.26 |    0.00 |      1.2207 |     - |     - |       5 KB |
|           SudokuSolverLite |   Easy |     3,547.4 us |       3.61 us |       3.01 us |     8.82 |    0.01 |     62.5000 |     - |     - |     261 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice | Medium |     4,933.3 us |     156.80 us |     146.67 us |     1.00 |    0.00 |      7.8125 |     - |     - |      45 KB |
|   SudokuSpiceDynamicSingle | Medium |     5,747.3 us |     131.23 us |     116.33 us |     1.17 |    0.03 |     15.6250 |     - |     - |      64 KB |
| SudokuSpiceDynamicMultiple | Medium |     6,242.6 us |     143.11 us |     133.86 us |     1.27 |    0.05 |     15.6250 |     - |     - |      66 KB |
|     SudokuSpiceConstraints | Medium |     1,043.2 us |       1.59 us |       1.41 us |     0.21 |    0.01 |     17.5781 |     - |     - |      75 KB |
|                SudokuSharp | Medium |    50,054.4 us |   1,585.57 us |   1,483.14 us |    10.16 |    0.48 |    100.0000 |     - |     - |     686 KB |
|           SudokuSolverLite | Medium |    52,520.5 us |      42.04 us |      37.27 us |    10.67 |    0.32 |    900.0000 |     - |     - |   3,769 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardA |     4,169.0 us |      46.05 us |      40.82 us |     1.00 |    0.00 |      7.8125 |     - |     - |      40 KB |
|   SudokuSpiceDynamicSingle |  HardA |     4,911.8 us |      68.63 us |      60.84 us |     1.18 |    0.01 |      7.8125 |     - |     - |      56 KB |
| SudokuSpiceDynamicMultiple |  HardA |     5,374.0 us |      65.19 us |      57.79 us |     1.29 |    0.02 |      7.8125 |     - |     - |      56 KB |
|     SudokuSpiceConstraints |  HardA |     1,185.5 us |       0.83 us |       0.74 us |     0.28 |    0.00 |     17.5781 |     - |     - |      73 KB |
|                SudokuSharp |  HardA |    60,522.3 us |  10,539.85 us |   9,858.98 us |    14.59 |    2.47 |    125.0000 |     - |     - |     833 KB |
|           SudokuSolverLite |  HardA |   557,090.9 us |     718.95 us |     637.33 us |   133.64 |    1.32 |  10000.0000 |     - |     - |  41,347 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardB |     9,039.9 us |     268.70 us |     251.34 us |     1.00 |    0.00 |     15.6250 |     - |     - |     104 KB |
|   SudokuSpiceDynamicSingle |  HardB |    10,566.7 us |     302.94 us |     283.37 us |     1.17 |    0.04 |     31.2500 |     - |     - |     138 KB |
| SudokuSpiceDynamicMultiple |  HardB |    11,667.3 us |     274.66 us |     256.92 us |     1.29 |    0.04 |     31.2500 |     - |     - |     132 KB |
|     SudokuSpiceConstraints |  HardB |     1,420.1 us |       1.10 us |       1.02 us |     0.16 |    0.00 |     17.5781 |     - |     - |      75 KB |
|                SudokuSharp |  HardB |   405,277.6 us | 163,620.32 us | 145,045.16 us |    44.72 |   15.74 |   1500.0000 |     - |     - |   7,134 KB |
|           SudokuSolverLite |  HardB |   110,508.0 us |      91.96 us |      86.02 us |    12.23 |    0.35 |   1800.0000 |     - |     - |   7,757 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilA |     6,256.1 us |      72.96 us |      64.68 us |     1.00 |    0.00 |     15.6250 |     - |     - |      75 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     7,107.4 us |      86.19 us |      80.62 us |     1.14 |    0.02 |     15.6250 |     - |     - |      93 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     7,645.7 us |      80.40 us |      71.27 us |     1.22 |    0.02 |     15.6250 |     - |     - |      92 KB |
|     SudokuSpiceConstraints |  EvilA |     1,261.9 us |       1.00 us |       0.93 us |     0.20 |    0.00 |     17.5781 |     - |     - |      78 KB |
|                SudokuSharp |  EvilA |   701,188.9 us | 418,183.63 us | 391,169.22 us |   109.34 |   64.07 |           - |     - |     - |   2,592 KB |
|           SudokuSolverLite |  EvilA | 9,159,970.4 us |  15,908.92 us |  14,102.84 us | 1,464.32 |   14.54 | 166000.0000 |     - |     - | 674,507 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilB |    57,560.2 us |   8,251.51 us |   7,718.47 us |     1.00 |    0.00 |    125.0000 |     - |     - |     615 KB |
|   SudokuSpiceDynamicSingle |  EvilB |    71,874.6 us |  13,859.21 us |  12,963.92 us |     1.27 |    0.27 |    166.6667 |     - |     - |     915 KB |
| SudokuSpiceDynamicMultiple |  EvilB |    85,545.9 us |  14,390.55 us |  13,460.93 us |     1.50 |    0.26 |           - |     - |     - |     480 KB |
|     SudokuSpiceConstraints |  EvilB |     4,071.9 us |       3.30 us |       2.92 us |     0.07 |    0.01 |     23.4375 |     - |     - |      99 KB |
|                SudokuSharp |  EvilB |   863,445.3 us |  42,941.11 us |  38,066.18 us |    15.24 |    2.41 |   3000.0000 |     - |     - |  13,099 KB |
|           SudokuSolverLite |  EvilB | 1,170,533.2 us |     578.02 us |     540.68 us |    20.70 |    2.99 |  20000.0000 |     - |     - |  83,411 KB |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |   Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|--------:|------:|----------:|
|            SudokuSpice |  1.190 ms | 0.0094 ms | 0.0088 ms |  1.00 |    0.00 |  150.3906 | 19.5313 |     - |    933 KB |
| SudokuSpiceConstraints |  5.214 ms | 0.0719 ms | 0.0672 ms |  4.38 |    0.07 | 1234.3750 | 70.3125 |     - |  7,585 KB |
|     SudokuSharpSingles | 14.765 ms | 0.8307 ms | 2.4233 ms | 12.59 |    2.03 | 3343.7500 |       - |     - | 20,652 KB |
|       SudokuSharpMixed |  7.344 ms | 0.2189 ms | 0.6454 ms |  6.29 |    0.71 | 1664.0625 | 15.6250 |     - | 10,194 KB |

### WASM

|                 Method |      Mean |      Error |     StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|-----------:|-----------:|------:|--------:|----------:|------:|------:|----------:|
|            SudokuSpice |  54.61 ms |   3.746 ms |   3.504 ms |  1.00 |    0.00 |  111.1111 |     - |     - |    866 KB |
| SudokuSpiceConstraints |  33.34 ms |   0.667 ms |   0.624 ms |  0.61 |    0.04 |  600.0000 |     - |     - |  2,636 KB |
|     SudokuSharpSingles | 279.85 ms | 128.779 ms | 120.460 ms |  5.16 |    2.28 | 1500.0000 |     - |     - |  6,270 KB |
|       SudokuSharpMixed | 120.05 ms |  61.780 ms |  57.789 ms |  2.19 |    1.03 | 1333.3333 |     - |     - |  6,510 KB |

