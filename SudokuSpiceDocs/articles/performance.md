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

|                 Method | sampleCollection |         Mean |        Error |        StdDev |       Median |  Ratio | RatioSD |
|----------------------- |----------------- |-------------:|-------------:|--------------:|-------------:|-------:|--------:|
|            SudokuSpice |       Guesses: 0 |     23.25 us |     0.028 us |      0.026 us |     23.25 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |       Guesses: 0 |     68.47 us |     0.096 us |      0.085 us |     68.50 us |   2.94 |    0.00 |
|            SudokuSharp |       Guesses: 0 |    771.99 us |    34.656 us |    101.093 us |    771.36 us |  33.11 |    3.51 |
|       SudokuSolverLite |       Guesses: 0 |  1,249.14 us |    28.469 us |     83.495 us |  1,244.73 us |  54.31 |    3.78 |
|                        |                  |              |              |               |              |        |         |
|            SudokuSpice |       Guesses: 1 |     42.52 us |     0.181 us |      0.169 us |     42.52 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |       Guesses: 1 |     72.39 us |     0.101 us |      0.095 us |     72.40 us |   1.70 |    0.01 |
|            SudokuSharp |       Guesses: 1 |  1,596.24 us |    62.533 us |    181.419 us |  1,586.84 us |  36.91 |    3.62 |
|       SudokuSolverLite |       Guesses: 1 |  4,062.56 us |   388.657 us |  1,139.863 us |  4,033.73 us |  92.85 |   25.25 |
|                        |                  |              |              |               |              |        |         |
|            SudokuSpice |     Guesses: 2-3 |     59.63 us |     0.385 us |      0.360 us |     59.57 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |     Guesses: 2-3 |     76.26 us |     0.150 us |      0.140 us |     76.30 us |   1.28 |    0.01 |
|            SudokuSharp |     Guesses: 2-3 |  2,575.15 us |   196.271 us |    578.711 us |  2,598.56 us |  42.67 |   10.79 |
|       SudokuSolverLite |     Guesses: 2-3 |  3,924.51 us |   365.106 us |  1,076.525 us |  3,578.97 us |  58.75 |   18.94 |
|                        |                  |              |              |               |              |        |         |
|            SudokuSpice |     Guesses: 4-7 |     87.14 us |     0.591 us |      0.553 us |     87.07 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |     Guesses: 4-7 |     80.60 us |     0.335 us |      0.297 us |     80.64 us |   0.93 |    0.01 |
|            SudokuSharp |     Guesses: 4-7 |  3,830.27 us |   261.527 us |    767.013 us |  3,750.82 us |  42.13 |    6.44 |
|       SudokuSolverLite |     Guesses: 4-7 |  9,875.07 us | 1,190.382 us |  3,453.513 us |  9,368.89 us | 119.77 |   43.88 |
|                        |                  |              |              |               |              |        |         |
|            SudokuSpice |      Guesses: 8+ |    117.20 us |     0.956 us |      0.847 us |    117.19 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |      Guesses: 8+ |     85.82 us |     0.281 us |      0.263 us |     85.77 us |   0.73 |    0.01 |
|            SudokuSharp |      Guesses: 8+ |  6,220.00 us |   445.927 us |  1,314.825 us |  6,218.49 us |  59.04 |   14.16 |
|       SudokuSolverLite |      Guesses: 8+ | 23,332.16 us | 3,846.482 us | 11,281.066 us | 23,359.23 us | 217.03 |  112.68 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|
|                SudokuSpice |   Easy |       9.738 us |     0.0157 us |     0.0147 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      15.645 us |     0.0515 us |     0.0430 us |     1.61 |    0.00 |
| SudokuSpiceDynamicMultiple |   Easy |      21.309 us |     0.0292 us |     0.0273 us |     2.19 |    0.00 |
|     SudokuSpiceConstraints |   Easy |      24.737 us |     0.0376 us |     0.0314 us |     2.54 |    0.00 |
|                SudokuSharp |   Easy |       6.525 us |     0.0262 us |     0.0233 us |     0.67 |    0.00 |
|           SudokuSolverLite |   Easy |     154.804 us |     0.2326 us |     0.2176 us |    15.90 |    0.03 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |     174.951 us |     0.3573 us |     0.3342 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     204.572 us |     0.4628 us |     0.4103 us |     1.17 |    0.00 |
| SudokuSpiceDynamicMultiple |  EvilA |     225.805 us |     0.4514 us |     0.4002 us |     1.29 |    0.00 |
|     SudokuSpiceConstraints |  EvilA |      89.962 us |     0.2205 us |     0.2063 us |     0.51 |    0.00 |
|                SudokuSharp |  EvilA |  44,732.794 us | 2,633.8616 us | 7,766.0004 us |   255.09 |   55.34 |
|           SudokuSolverLite |  EvilA | 397,042.040 us |   497.2516 us |   415.2276 us | 2,269.22 |    5.65 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |   1,562.112 us |    31.0545 us |    41.4569 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   2,024.354 us |    39.6313 us |    58.0911 us |     1.29 |    0.05 |
| SudokuSpiceDynamicMultiple |  EvilB |   2,358.166 us |    46.1520 us |    90.0160 us |     1.51 |    0.08 |
|     SudokuSpiceConstraints |  EvilB |     230.307 us |     0.5521 us |     0.5165 us |     0.15 |    0.00 |
|                SudokuSharp |  EvilB |  47,739.047 us |   824.5815 us |   730.9700 us |    30.54 |    0.70 |
|           SudokuSolverLite |  EvilB |  49,896.945 us |    39.5962 us |    33.0646 us |    31.93 |    0.76 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |      92.035 us |     0.1796 us |     0.1680 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |     115.486 us |     0.2381 us |     0.2227 us |     1.25 |    0.00 |
| SudokuSpiceDynamicMultiple |  HardA |     133.753 us |     0.3009 us |     0.2815 us |     1.45 |    0.00 |
|     SudokuSpiceConstraints |  HardA |      78.637 us |     0.0713 us |     0.0632 us |     0.85 |    0.00 |
|                SudokuSharp |  HardA |   3,277.716 us |    63.4024 us |    90.9298 us |    35.42 |    0.69 |
|           SudokuSolverLite |  HardA |  24,858.170 us |    21.4029 us |    18.9731 us |   270.09 |    0.52 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |     249.422 us |     1.0243 us |     0.9080 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     296.551 us |     1.3921 us |     1.3021 us |     1.19 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardB |     338.985 us |     1.1642 us |     1.0320 us |     1.36 |    0.01 |
|     SudokuSpiceConstraints |  HardB |      90.328 us |     0.1294 us |     0.1147 us |     0.36 |    0.00 |
|                SudokuSharp |  HardB |  23,074.344 us |   832.1748 us | 2,453.6863 us |    90.11 |   11.27 |
|           SudokuSolverLite |  HardB |   4,699.454 us |     4.9523 us |     4.6324 us |    18.84 |    0.08 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |     116.475 us |     0.4609 us |     0.4311 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |     145.478 us |     0.6171 us |     0.5773 us |     1.25 |    0.01 |
| SudokuSpiceDynamicMultiple | Medium |     172.492 us |     0.7044 us |     0.6589 us |     1.48 |    0.01 |
|     SudokuSpiceConstraints | Medium |      75.000 us |     0.2579 us |     0.2286 us |     0.64 |    0.00 |
|                SudokuSharp | Medium |   2,998.381 us |    16.0284 us |    14.9930 us |    25.74 |    0.14 |
|           SudokuSolverLite | Medium |   2,300.428 us |     1.8287 us |     1.7106 us |    19.75 |    0.07 |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |           Mean |         Error |        StdDev |         Median |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------------:|---------:|--------:|
|                SudokuSpice |   Easy |       369.2 us |       6.90 us |       6.45 us |       365.3 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |       546.0 us |       0.47 us |       0.42 us |       545.9 us |     1.48 |    0.03 |
| SudokuSpiceDynamicMultiple |   Easy |       670.1 us |       0.80 us |       0.67 us |       669.9 us |     1.81 |    0.03 |
|     SudokuSpiceConstraints |   Easy |       469.1 us |       0.72 us |       0.56 us |       469.0 us |     1.27 |    0.02 |
|                SudokuSharp |   Easy |       104.5 us |       0.08 us |       0.07 us |       104.5 us |     0.28 |    0.00 |
|           SudokuSolverLite |   Easy |     3,620.3 us |       2.52 us |       2.36 us |     3,619.9 us |     9.81 |    0.17 |
|                            |        |                |               |               |                |          |         |
|                SudokuSpice |  EvilA |     6,939.4 us |      72.90 us |      68.19 us |     6,939.1 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     7,713.9 us |     112.73 us |     105.45 us |     7,719.0 us |     1.11 |    0.02 |
| SudokuSpiceDynamicMultiple |  EvilA |     8,248.0 us |     145.08 us |     135.70 us |     8,211.9 us |     1.19 |    0.02 |
|     SudokuSpiceConstraints |  EvilA |     1,245.1 us |       1.10 us |       0.92 us |     1,245.0 us |     0.18 |    0.00 |
|                SudokuSharp |  EvilA |   746,066.9 us | 412,939.70 us | 386,264.05 us |   771,445.5 us |   107.59 |   55.68 |
|           SudokuSolverLite |  EvilA | 9,265,339.0 us |  14,037.94 us |  12,444.27 us | 9,261,697.5 us | 1,335.31 |   13.63 |
|                            |        |                |               |               |                |          |         |
|                SudokuSpice |  EvilB |    62,365.9 us |   9,799.40 us |   8,686.92 us |    63,030.3 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |    68,877.0 us |  16,259.51 us |  15,209.15 us |    73,634.2 us |     1.12 |    0.32 |
| SudokuSpiceDynamicMultiple |  EvilB |    78,072.4 us |   8,805.23 us |   7,805.61 us |    78,640.7 us |     1.27 |    0.22 |
|     SudokuSpiceConstraints |  EvilB |     4,006.6 us |       3.71 us |       3.29 us |     4,007.1 us |     0.07 |    0.01 |
|                SudokuSharp |  EvilB |   910,343.2 us |  72,916.34 us |  68,205.99 us |   892,070.0 us |    14.79 |    2.33 |
|           SudokuSolverLite |  EvilB | 1,155,378.9 us |     859.16 us |     761.62 us | 1,155,628.5 us |    18.86 |    2.63 |
|                            |        |                |               |               |                |          |         |
|                SudokuSpice |  HardA |     4,376.9 us |      34.59 us |      32.36 us |     4,377.4 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |     5,033.1 us |      78.73 us |      65.74 us |     5,018.5 us |     1.15 |    0.02 |
| SudokuSpiceDynamicMultiple |  HardA |     5,527.3 us |     101.70 us |      95.13 us |     5,483.2 us |     1.26 |    0.02 |
|     SudokuSpiceConstraints |  HardA |     1,173.1 us |       6.58 us |       5.84 us |     1,170.7 us |     0.27 |    0.00 |
|                SudokuSharp |  HardA |    59,207.3 us |  10,501.21 us |   9,822.84 us |    62,909.4 us |    13.53 |    2.24 |
|           SudokuSolverLite |  HardA |   568,887.5 us |     455.87 us |     426.42 us |   568,919.0 us |   129.98 |    0.99 |
|                            |        |                |               |               |                |          |         |
|                SudokuSpice |  HardB |     9,534.3 us |     291.35 us |     272.53 us |     9,489.6 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |    10,997.8 us |     233.63 us |     218.54 us |    11,002.1 us |     1.15 |    0.04 |
| SudokuSpiceDynamicMultiple |  HardB |    12,188.5 us |     293.74 us |     274.76 us |    12,162.7 us |     1.28 |    0.05 |
|     SudokuSpiceConstraints |  HardB |     1,395.6 us |       0.66 us |       0.55 us |     1,395.8 us |     0.15 |    0.00 |
|                SudokuSharp |  HardB |   448,291.9 us | 236,477.41 us | 221,201.11 us |   545,884.5 us |    46.93 |   23.29 |
|           SudokuSolverLite |  HardB |   107,924.6 us |      79.85 us |      70.79 us |   107,922.2 us |    11.37 |    0.29 |
|                            |        |                |               |               |                |          |         |
|                SudokuSpice | Medium |     5,196.0 us |     119.93 us |     106.31 us |     5,205.0 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |     6,154.2 us |     144.55 us |     135.21 us |     6,188.1 us |     1.19 |    0.04 |
| SudokuSpiceDynamicMultiple | Medium |     6,606.2 us |     144.09 us |     134.78 us |     6,649.7 us |     1.27 |    0.04 |
|     SudokuSpiceConstraints | Medium |     1,025.7 us |       1.25 us |       1.10 us |     1,025.7 us |     0.20 |    0.00 |
|                SudokuSharp | Medium |    49,233.5 us |   1,025.02 us |     958.80 us |    49,027.6 us |     9.49 |    0.22 |
|           SudokuSolverLite | Medium |    53,074.8 us |      52.19 us |      48.82 us |    53,072.6 us |    10.22 |    0.21 |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|----------------------- |----------:|----------:|----------:|------:|--------:|
|            SudokuSpice |  1.414 ms | 0.0093 ms | 0.0087 ms |  1.00 |    0.00 |
| SudokuSpiceConstraints |  2.311 ms | 0.0431 ms | 0.0442 ms |  1.63 |    0.04 |
|     SudokuSharpSingles | 15.104 ms | 0.5700 ms | 1.6537 ms | 10.99 |    1.02 |
|       SudokuSharpMixed |  7.331 ms | 0.3585 ms | 1.0402 ms |  5.10 |    0.73 |

### WASM

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|----------------------- |----------:|----------:|----------:|------:|--------:|
|            SudokuSpice |  54.87 ms |  3.710 ms |  3.289 ms |  1.00 |    0.00 |
| SudokuSpiceConstraints |  34.21 ms |  1.828 ms |  1.710 ms |  0.62 |    0.05 |
|     SudokuSharpSingles | 229.49 ms | 62.372 ms | 52.083 ms |  4.22 |    0.93 |
|       SudokuSharpMixed | 114.18 ms | 34.683 ms | 30.745 ms |  2.09 |    0.60 |
