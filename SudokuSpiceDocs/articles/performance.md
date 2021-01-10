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

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|------:|------:|-----------:|
|                SudokuSpice |   Easy |       403.1 us |       0.29 us |       0.26 us |     1.00 |    0.00 |      1.4648 |     - |     - |       7 KB |
|   SudokuSpiceDynamicSingle |   Easy |       566.7 us |       0.59 us |       0.53 us |     1.41 |    0.00 |      2.9297 |     - |     - |      12 KB |
| SudokuSpiceDynamicMultiple |   Easy |       703.0 us |       1.05 us |       0.82 us |     1.74 |    0.00 |      2.9297 |     - |     - |      12 KB |
|     SudokuSpiceConstraints |   Easy |       474.9 us |       0.38 us |       0.33 us |     1.18 |    0.00 |      9.2773 |     - |     - |      38 KB |
|                SudokuSharp |   Easy |       104.1 us |       0.09 us |       0.08 us |     0.26 |    0.00 |      1.2207 |     - |     - |       5 KB |
|           SudokuSolverLite |   Easy |     3,594.0 us |       2.53 us |       2.36 us |     8.92 |    0.01 |     62.5000 |     - |     - |     261 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilA |     6,238.7 us |      50.25 us |      44.54 us |     1.00 |    0.00 |     15.6250 |     - |     - |      75 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     7,191.9 us |     112.08 us |     104.84 us |     1.15 |    0.02 |     15.6250 |     - |     - |      94 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     7,666.8 us |     134.39 us |     125.70 us |     1.23 |    0.02 |     15.6250 |     - |     - |      93 KB |
|     SudokuSpiceConstraints |  EvilA |     1,251.1 us |       0.73 us |       0.68 us |     0.20 |    0.00 |     17.5781 |     - |     - |      78 KB |
|                SudokuSharp |  EvilA |   707,824.8 us | 355,885.02 us | 332,895.07 us |   113.27 |   55.57 |   2500.0000 |     - |     - |  11,303 KB |
|           SudokuSolverLite |  EvilA | 9,140,635.7 us |  14,884.36 us |  13,922.84 us | 1,465.26 |    9.90 | 166000.0000 |     - |     - | 674,507 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilB |    63,399.6 us |  11,252.72 us |  10,525.80 us |     1.00 |    0.00 |    142.8571 |     - |     - |     754 KB |
|   SudokuSpiceDynamicSingle |  EvilB |    67,432.2 us |  13,646.50 us |  12,764.94 us |     1.08 |    0.23 |    250.0000 |     - |     - |   1,064 KB |
| SudokuSpiceDynamicMultiple |  EvilB |    80,264.6 us |  17,093.34 us |  15,989.12 us |     1.30 |    0.35 |    142.8571 |     - |     - |   1,013 KB |
|     SudokuSpiceConstraints |  EvilB |     4,053.6 us |       2.10 us |       1.97 us |     0.07 |    0.01 |     23.4375 |     - |     - |      99 KB |
|                SudokuSharp |  EvilB |   902,118.3 us |  61,800.09 us |  57,807.85 us |    14.60 |    2.60 |   3000.0000 |     - |     - |  13,834 KB |
|           SudokuSolverLite |  EvilB | 1,148,091.3 us |     712.67 us |     666.63 us |    18.62 |    3.35 |  20000.0000 |     - |     - |  83,411 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardA |     4,223.8 us |      73.90 us |      69.13 us |     1.00 |    0.00 |      7.8125 |     - |     - |      40 KB |
|   SudokuSpiceDynamicSingle |  HardA |     4,965.1 us |      65.29 us |      57.88 us |     1.18 |    0.03 |      7.8125 |     - |     - |      56 KB |
| SudokuSpiceDynamicMultiple |  HardA |     5,352.3 us |     106.53 us |      99.65 us |     1.27 |    0.04 |      7.8125 |     - |     - |      56 KB |
|     SudokuSpiceConstraints |  HardA |     1,170.7 us |       0.79 us |       0.74 us |     0.28 |    0.00 |     17.5781 |     - |     - |      73 KB |
|                SudokuSharp |  HardA |    65,928.3 us |  15,437.28 us |  14,440.04 us |    15.59 |    3.33 |    125.0000 |     - |     - |     771 KB |
|           SudokuSolverLite |  HardA |   574,708.5 us |     592.12 us |     524.90 us |   136.13 |    2.35 |  10000.0000 |     - |     - |  41,347 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardB |     9,133.0 us |     213.93 us |     200.11 us |     1.00 |    0.00 |     15.6250 |     - |     - |     104 KB |
|   SudokuSpiceDynamicSingle |  HardB |    10,573.2 us |     302.47 us |     282.93 us |     1.16 |    0.04 |     31.2500 |     - |     - |     139 KB |
| SudokuSpiceDynamicMultiple |  HardB |    11,673.0 us |     370.52 us |     346.58 us |     1.28 |    0.05 |     31.2500 |     - |     - |     133 KB |
|     SudokuSpiceConstraints |  HardB |     1,413.8 us |       0.73 us |       0.65 us |     0.15 |    0.00 |     17.5781 |     - |     - |      75 KB |
|                SudokuSharp |  HardB |   470,526.9 us | 259,964.44 us | 243,170.90 us |    51.53 |   26.62 |   2000.0000 |     - |     - |   8,291 KB |
|           SudokuSolverLite |  HardB |   107,895.7 us |     116.91 us |     109.36 us |    11.82 |    0.26 |   1800.0000 |     - |     - |   7,757 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice | Medium |     4,845.4 us |      91.10 us |      80.76 us |     1.00 |    0.00 |      7.8125 |     - |     - |      45 KB |
|   SudokuSpiceDynamicSingle | Medium |     5,795.3 us |     155.59 us |     145.54 us |     1.20 |    0.04 |     15.6250 |     - |     - |      71 KB |
| SudokuSpiceDynamicMultiple | Medium |     6,165.2 us |     116.10 us |     102.92 us |     1.27 |    0.03 |     15.6250 |     - |     - |      65 KB |
|     SudokuSpiceConstraints | Medium |     1,030.8 us |       1.21 us |       1.07 us |     0.21 |    0.00 |     17.5781 |     - |     - |      75 KB |
|                SudokuSharp | Medium |    49,365.2 us |   1,235.12 us |   1,155.33 us |    10.17 |    0.22 |     90.9091 |     - |     - |     640 KB |
|           SudokuSolverLite | Medium |    53,249.0 us |      35.68 us |      33.38 us |    10.99 |    0.18 |    900.0000 |     - |     - |   3,769 KB |

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
|            SudokuSpice |  56.74 ms |  3.508 ms |  3.281 ms |  1.00 |    0.00 |  100.0000 |     - |     - |    796 KB |
| SudokuSpiceConstraints |  34.93 ms |  1.153 ms |  1.079 ms |  0.62 |    0.04 |  571.4286 |     - |     - |  2,557 KB |
|     SudokuSharpSingles | 205.48 ms | 88.073 ms | 82.383 ms |  3.61 |    1.39 | 8500.0000 |     - |     - | 34,664 KB |
|       SudokuSharpMixed | 122.11 ms | 35.180 ms | 31.186 ms |  2.17 |    0.56 | 1600.0000 |     - |     - |  7,158 KB |

