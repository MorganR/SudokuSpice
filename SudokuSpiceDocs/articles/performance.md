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

|                 Method | sampleCollection |         Mean |        Error |        StdDev |  Ratio | RatioSD |      Gen 0 |   Gen 1 | Gen 2 |  Allocated |
|----------------------- |----------------- |-------------:|-------------:|--------------:|-------:|--------:|-----------:|--------:|------:|-----------:|
|            SudokuSpice |       Guesses: 0 |     24.28 us |     0.040 us |      0.037 us |   1.00 |    0.00 |     1.8005 |  0.0305 |     - |      11 KB |
| SudokuSpiceConstraints |       Guesses: 0 |     69.92 us |     0.168 us |      0.157 us |   2.88 |    0.01 |    17.8223 |  3.6621 |     - |     109 KB |
|            SudokuSharp |       Guesses: 0 |    798.20 us |    28.738 us |     84.285 us |  33.69 |    3.07 |   132.8125 |       - |     - |     819 KB |
|       SudokuSolverLite |       Guesses: 0 |  1,259.47 us |    31.795 us |     93.747 us |  53.68 |    3.82 |   320.3125 |       - |     - |   1,971 KB |
|                        |                  |              |              |               |        |         |            |         |       |            |
|            SudokuSpice |       Guesses: 1 |     43.82 us |     0.129 us |      0.120 us |   1.00 |    0.00 |     3.6011 |  0.0610 |     - |      22 KB |
| SudokuSpiceConstraints |       Guesses: 1 |     73.40 us |     0.119 us |      0.111 us |   1.68 |    0.01 |    18.1885 |  3.7842 |     - |     112 KB |
|            SudokuSharp |       Guesses: 1 |  1,591.48 us |    94.568 us |    278.837 us |  35.45 |    6.85 |   234.3750 |       - |     - |   1,447 KB |
|       SudokuSolverLite |       Guesses: 1 |  4,001.32 us |   535.945 us |  1,563.378 us |  84.79 |   33.92 |  1398.4375 |       - |     - |   8,581 KB |
|                        |                  |              |              |               |        |         |            |         |       |            |
|            SudokuSpice |     Guesses: 2-3 |     62.42 us |     0.450 us |      0.421 us |   1.00 |    0.00 |     5.2490 |  0.1221 |     - |      33 KB |
| SudokuSpiceConstraints |     Guesses: 2-3 |     76.18 us |     0.179 us |      0.167 us |   1.22 |    0.01 |    18.5547 |  3.9063 |     - |     114 KB |
|            SudokuSharp |     Guesses: 2-3 |  2,660.63 us |   224.426 us |    658.203 us |  38.41 |   10.52 |   289.0625 |       - |     - |   1,774 KB |
|       SudokuSolverLite |     Guesses: 2-3 |  3,951.34 us |   176.672 us |    506.906 us |  64.61 |    9.36 |  1085.9375 |  3.9063 |     - |   6,656 KB |
|                        |                  |              |              |               |        |         |            |         |       |            |
|            SudokuSpice |     Guesses: 4-7 |     90.56 us |     0.502 us |      0.445 us |   1.00 |    0.00 |     8.0566 |  0.2441 |     - |      50 KB |
| SudokuSpiceConstraints |     Guesses: 4-7 |     82.40 us |     0.058 us |      0.045 us |   0.91 |    0.00 |    19.1650 |  4.0283 |     - |     118 KB |
|            SudokuSharp |     Guesses: 4-7 |  3,698.49 us |   325.882 us |    940.244 us |  42.53 |    8.35 |   828.1250 |       - |     - |   5,112 KB |
|       SudokuSolverLite |     Guesses: 4-7 | 10,273.96 us |   850.701 us |  2,494.959 us | 111.19 |   26.97 |  3593.7500 |  7.8125 |     - |  22,017 KB |
|                        |                  |              |              |               |        |         |            |         |       |            |
|            SudokuSpice |      Guesses: 8+ |    119.13 us |     0.733 us |      0.650 us |   1.00 |    0.00 |    10.9863 |  0.3662 |     - |      68 KB |
| SudokuSpiceConstraints |      Guesses: 8+ |     87.17 us |     0.195 us |      0.182 us |   0.73 |    0.00 |    19.6533 |  4.2725 |     - |     121 KB |
|            SudokuSharp |      Guesses: 8+ |  6,131.07 us |   384.919 us |  1,128.902 us |  54.07 |   11.15 |  1195.3125 |       - |     - |   7,359 KB |
|       SudokuSolverLite |      Guesses: 8+ | 22,331.85 us | 5,328.091 us | 15,626.369 us | 187.63 |  117.26 | 23769.2308 | 76.9231 |     - | 145,928 KB |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 |  Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|-------:|------:|-----------:|
|                SudokuSpice |   Easy |      10.144 us |     0.0518 us |     0.0484 us |     1.00 |    0.00 |      1.4496 | 0.0153 |     - |       9 KB |
|   SudokuSpiceDynamicSingle |   Easy |      16.536 us |     0.0627 us |     0.0556 us |     1.63 |    0.01 |      2.5940 | 0.0305 |     - |      16 KB |
| SudokuSpiceDynamicMultiple |   Easy |      21.345 us |     0.0189 us |     0.0158 us |     2.10 |    0.01 |      2.5940 | 0.0610 |     - |      16 KB |
|     SudokuSpiceConstraints |   Easy |      26.771 us |     0.1255 us |     0.1048 us |     2.64 |    0.02 |      7.8735 | 0.8240 |     - |      48 KB |
|                SudokuSharp |   Easy |       6.558 us |     0.0166 us |     0.0138 us |     0.65 |    0.00 |      1.1063 |      - |     - |       7 KB |
|           SudokuSolverLite |   Easy |     156.196 us |     0.3448 us |     0.3057 us |    15.40 |    0.10 |     44.4336 |      - |     - |     273 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice | Medium |     119.489 us |     0.2577 us |     0.2285 us |     1.00 |    0.00 |     11.7188 | 0.3662 |     - |      72 KB |
|   SudokuSpiceDynamicSingle | Medium |     149.275 us |     0.7980 us |     0.7464 us |     1.25 |    0.01 |     16.6016 | 0.4883 |     - |     103 KB |
| SudokuSpiceDynamicMultiple | Medium |     174.992 us |     1.1995 us |     1.1220 us |     1.46 |    0.01 |     16.6016 | 0.4883 |     - |     102 KB |
|     SudokuSpiceConstraints | Medium |      76.135 us |     0.3509 us |     0.3282 us |     0.64 |    0.00 |     20.0195 | 5.0049 |     - |     123 KB |
|                SudokuSharp | Medium |   3,012.929 us |    21.9526 us |    20.5344 us |    25.23 |    0.19 |    156.2500 |      - |     - |     978 KB |
|           SudokuSolverLite | Medium |   2,357.128 us |     2.5399 us |     2.3758 us |    19.73 |    0.05 |    664.0625 |      - |     - |   4,088 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardA |      93.180 us |     0.2672 us |     0.2368 us |     1.00 |    0.00 |      9.0332 | 0.2441 |     - |      56 KB |
|   SudokuSpiceDynamicSingle |  HardA |     117.100 us |     0.2777 us |     0.2597 us |     1.26 |    0.01 |     13.1836 | 0.4883 |     - |      81 KB |
| SudokuSpiceDynamicMultiple |  HardA |     138.822 us |     0.3354 us |     0.3137 us |     1.49 |    0.01 |     13.1836 | 0.4883 |     - |      81 KB |
|     SudokuSpiceConstraints |  HardA |      79.756 us |     0.0853 us |     0.0756 us |     0.86 |    0.00 |     19.2871 | 4.6387 |     - |     118 KB |
|                SudokuSharp |  HardA |   3,316.124 us |    64.9645 us |   106.7385 us |    35.81 |    1.17 |    187.5000 |      - |     - |   1,167 KB |
|           SudokuSolverLite |  HardA |  24,410.339 us |    51.0266 us |    47.7303 us |   262.00 |    0.71 |   6937.5000 |      - |     - |  42,554 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  HardB |     250.846 us |     1.1820 us |     1.1057 us |     1.00 |    0.00 |     24.4141 | 0.9766 |     - |     151 KB |
|   SudokuSpiceDynamicSingle |  HardB |     303.386 us |     1.4885 us |     1.3924 us |     1.21 |    0.01 |     33.2031 | 1.4648 |     - |     204 KB |
| SudokuSpiceDynamicMultiple |  HardB |     342.778 us |     1.0247 us |     0.9585 us |     1.37 |    0.01 |     33.2031 | 1.4648 |     - |     205 KB |
|     SudokuSpiceConstraints |  HardB |      90.961 us |     0.1886 us |     0.1764 us |     0.36 |    0.00 |     19.8975 | 4.6387 |     - |     122 KB |
|                SudokuSharp |  HardB |  24,085.411 us |   815.6484 us | 2,392.1555 us |    95.41 |   10.53 |   1656.2500 |      - |     - |  10,213 KB |
|           SudokuSolverLite |  HardB |   4,820.488 us |     8.2582 us |     7.7247 us |    19.22 |    0.09 |   1335.9375 |      - |     - |   8,220 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilA |     178.525 us |     0.5461 us |     0.5108 us |     1.00 |    0.00 |     19.0430 | 0.7324 |     - |     117 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     206.766 us |     0.4541 us |     0.4026 us |     1.16 |    0.00 |     23.6816 | 0.9766 |     - |     146 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     229.984 us |     0.6850 us |     0.6072 us |     1.29 |    0.01 |     23.6816 | 0.9766 |     - |     145 KB |
|     SudokuSpiceConstraints |  EvilA |      91.099 us |     0.3025 us |     0.2829 us |     0.51 |    0.00 |     20.9961 | 5.3711 |     - |     129 KB |
|                SudokuSharp |  EvilA |  42,811.765 us | 2,920.4357 us | 8,610.9705 us |   240.51 |   39.32 |   2500.0000 |      - |     - |  15,632 KB |
|           SudokuSolverLite |  EvilA | 399,166.091 us | 1,605.5283 us | 1,423.2591 us | 2,236.02 |   10.20 | 113000.0000 |      - |     - | 693,267 KB |
|                            |        |                |               |               |          |         |             |        |       |            |
|                SudokuSpice |  EvilB |   1,611.554 us |    30.6582 us |    37.6510 us |     1.00 |    0.00 |    140.6250 | 5.8594 |     - |     873 KB |
|   SudokuSpiceDynamicSingle |  EvilB |   2,007.245 us |    39.7054 us |    64.1169 us |     1.25 |    0.04 |    222.6563 | 7.8125 |     - |   1,371 KB |
| SudokuSpiceDynamicMultiple |  EvilB |   2,363.939 us |    46.4601 us |    68.1006 us |     1.47 |    0.05 |    203.1250 | 7.8125 |     - |   1,265 KB |
|     SudokuSpiceConstraints |  EvilB |     231.756 us |     0.8416 us |     0.7461 us |     0.14 |    0.00 |     26.1230 | 6.3477 |     - |     161 KB |
|                SudokuSharp |  EvilB |  47,907.768 us |   901.9639 us |   843.6976 us |    29.71 |    0.97 |   3181.8182 |      - |     - |  19,736 KB |
|           SudokuSolverLite |  EvilB |  49,722.898 us |   127.0537 us |   112.6298 us |    30.92 |    0.65 |  14181.8182 |      - |     - |  86,909 KB |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|------------:|------:|------:|-----------:|
|                SudokuSpice |   Easy |       378.9 us |       0.46 us |       0.38 us |     1.00 |    0.00 |      1.9531 |     - |     - |       8 KB |
|   SudokuSpiceDynamicSingle |   Easy |       546.5 us |       0.37 us |       0.33 us |     1.44 |    0.00 |      2.9297 |     - |     - |      12 KB |
| SudokuSpiceDynamicMultiple |   Easy |       688.7 us |       0.93 us |       0.87 us |     1.82 |    0.00 |      2.9297 |     - |     - |      12 KB |
|     SudokuSpiceConstraints |   Easy |       471.6 us |       0.35 us |       0.33 us |     1.24 |    0.00 |      8.7891 |     - |     - |      37 KB |
|                SudokuSharp |   Easy |       104.0 us |       0.51 us |       0.43 us |     0.27 |    0.00 |      1.2207 |     - |     - |       5 KB |
|           SudokuSolverLite |   Easy |     3,605.1 us |       2.54 us |       2.38 us |     9.51 |    0.01 |     62.5000 |     - |     - |     261 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilA |     7,020.7 us |      54.41 us |      48.23 us |     1.00 |    0.00 |     15.6250 |     - |     - |      91 KB |
|   SudokuSpiceDynamicSingle |  EvilA |     7,731.4 us |     111.02 us |     103.85 us |     1.10 |    0.02 |     23.4375 |     - |     - |     109 KB |
| SudokuSpiceDynamicMultiple |  EvilA |     8,328.0 us |     133.80 us |     125.15 us |     1.19 |    0.02 |     15.6250 |     - |     - |     107 KB |
|     SudokuSpiceConstraints |  EvilA |     1,258.2 us |       1.65 us |       1.46 us |     0.18 |    0.00 |     17.5781 |     - |     - |      77 KB |
|                SudokuSharp |  EvilA |   668,224.6 us | 486,544.29 us | 455,113.83 us |    99.79 |   64.53 |   2000.0000 |     - |     - |  11,817 KB |
|           SudokuSolverLite |  EvilA | 9,146,246.5 us |  19,692.14 us |  18,420.04 us | 1,302.92 |   10.27 | 166000.0000 |     - |     - | 674,507 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  EvilB |    67,052.8 us |  12,364.57 us |  11,565.83 us |     1.00 |    0.00 |    142.8571 |     - |     - |     590 KB |
|   SudokuSpiceDynamicSingle |  EvilB |    75,828.5 us |  16,284.98 us |  15,232.98 us |     1.16 |    0.32 |    142.8571 |     - |     - |   1,055 KB |
| SudokuSpiceDynamicMultiple |  EvilB |    90,022.3 us |  18,858.26 us |  17,640.03 us |     1.36 |    0.27 |    250.0000 |     - |     - |   1,095 KB |
|     SudokuSpiceConstraints |  EvilB |     4,041.1 us |       4.25 us |       3.32 us |     0.06 |    0.01 |     23.4375 |     - |     - |      98 KB |
|                SudokuSharp |  EvilB |   879,457.4 us |  46,076.23 us |  43,099.73 us |    13.52 |    2.62 |   3000.0000 |     - |     - |  13,071 KB |
|           SudokuSolverLite |  EvilB | 1,171,986.7 us |   1,524.47 us |   1,190.20 us |    18.37 |    3.35 |  20000.0000 |     - |     - |  83,411 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardA |     4,389.8 us |      55.10 us |      51.54 us |     1.00 |    0.00 |      7.8125 |     - |     - |      44 KB |
|   SudokuSpiceDynamicSingle |  HardA |     5,156.1 us |      73.48 us |      65.14 us |     1.18 |    0.02 |      7.8125 |     - |     - |      59 KB |
| SudokuSpiceDynamicMultiple |  HardA |     5,571.6 us |      48.14 us |      45.03 us |     1.27 |    0.02 |      7.8125 |     - |     - |      59 KB |
|     SudokuSpiceConstraints |  HardA |     1,160.8 us |       1.15 us |       0.96 us |     0.26 |    0.00 |     17.5781 |     - |     - |      71 KB |
|                SudokuSharp |  HardA |    61,062.6 us |  11,757.50 us |  10,997.97 us |    13.91 |    2.51 |    111.1111 |     - |     - |     747 KB |
|           SudokuSolverLite |  HardA |   587,434.4 us |     504.29 us |     447.04 us |   133.93 |    1.53 |  10000.0000 |     - |     - |  41,347 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice |  HardB |     9,722.1 us |     245.50 us |     229.64 us |     1.00 |    0.00 |     15.6250 |     - |     - |     116 KB |
|   SudokuSpiceDynamicSingle |  HardB |    11,235.0 us |     252.73 us |     236.40 us |     1.16 |    0.04 |     31.2500 |     - |     - |     151 KB |
| SudokuSpiceDynamicMultiple |  HardB |    12,174.5 us |     283.77 us |     265.44 us |     1.25 |    0.05 |     31.2500 |     - |     - |     146 KB |
|     SudokuSpiceConstraints |  HardB |     1,396.9 us |       0.87 us |       0.77 us |     0.14 |    0.00 |     17.5781 |     - |     - |      74 KB |
|                SudokuSharp |  HardB |   528,057.1 us | 213,391.94 us | 199,606.96 us |    54.59 |   21.36 |           - |     - |     - |   3,868 KB |
|           SudokuSolverLite |  HardB |   108,765.9 us |     112.72 us |     105.44 us |    11.19 |    0.26 |   1800.0000 |     - |     - |   7,757 KB |
|                            |        |                |               |               |          |         |             |       |       |            |
|                SudokuSpice | Medium |     5,325.0 us |     158.89 us |     148.63 us |     1.00 |    0.00 |      7.8125 |     - |     - |      58 KB |
|   SudokuSpiceDynamicSingle | Medium |     6,210.5 us |     166.93 us |     156.14 us |     1.17 |    0.04 |     15.6250 |     - |     - |      78 KB |
| SudokuSpiceDynamicMultiple | Medium |     6,718.9 us |     140.90 us |     131.79 us |     1.26 |    0.05 |     15.6250 |     - |     - |      76 KB |
|     SudokuSpiceConstraints | Medium |     1,024.7 us |       0.87 us |       0.81 us |     0.19 |    0.01 |     17.5781 |     - |     - |      74 KB |
|                SudokuSharp | Medium |    50,638.8 us |   1,738.09 us |   1,625.81 us |     9.52 |    0.38 |     90.9091 |     - |     - |     666 KB |
|           SudokuSolverLite | Medium |    53,482.6 us |      76.75 us |      68.03 us |    10.06 |    0.28 |    900.0000 |     - |     - |   3,769 KB |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |   Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|--------:|------:|----------:|
|            SudokuSpice |  1.413 ms | 0.0094 ms | 0.0083 ms |  1.00 |    0.00 |  175.7813 | 19.5313 |     - |      1 MB |
| SudokuSpiceConstraints |  2.266 ms | 0.0235 ms | 0.0196 ms |  1.60 |    0.02 |  589.8438 | 39.0625 |     - |      4 MB |
|     SudokuSharpSingles | 15.598 ms | 0.5903 ms | 1.7218 ms | 11.43 |    1.24 | 2968.7500 | 15.6250 |     - |     18 MB |
|       SudokuSharpMixed |  7.567 ms | 0.3313 ms | 0.9717 ms |  5.39 |    0.81 | 1359.3750 |       - |     - |      8 MB |

### WASM

|                 Method |      Mean |      Error |     StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|-----------:|-----------:|------:|--------:|----------:|------:|------:|----------:|
|            SudokuSpice |  56.21 ms |   2.392 ms |   1.868 ms |  1.00 |    0.00 |  125.0000 |     - |     - |    877 KB |
| SudokuSpiceConstraints |  35.14 ms |   2.108 ms |   1.972 ms |  0.63 |    0.05 |  562.5000 |     - |     - |  2,486 KB |
|     SudokuSharpSingles | 261.84 ms | 126.667 ms | 118.484 ms |  4.84 |    2.26 | 1666.6667 |     - |     - |  7,464 KB |
|       SudokuSharpMixed |  99.28 ms |  22.246 ms |  19.720 ms |  1.85 |    0.35 | 1000.0000 |     - |     - |  4,738 KB |
