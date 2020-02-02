# SudokuSpice

An efficient sudoku-solving library targeting .NET Core.

This code focuses on optimizing performance without sacrificing readability. Generally speaking, when faced with readability and flexibility improvements that have a slight performance cost, the version with improved readability has been implemented.

However that's not to say it's not performant! See [the numbers](#Performance).

## Performance

Current performance benchmarks on my machine:

|               Method |         Mean |      Error |     StdDev |
|--------------------- |-------------:|-----------:|-----------:|
|     EasySudoku (9x9) |    15.250 us |  0.1700 us |  0.1590 us |
|   MediumSudoku (9x9) |    86.694 us |  0.2438 us |  0.2036 us |
|    HardSudokuA (9x9) |   912.694 us | 13.5620 us | 12.6859 us |
|    HardSudokuB (9x9) | 2,335.479 us | 37.0555 us | 34.6618 us |
|  MegaSudokuA (16x16) | 1,291.604 us | 15.5212 us | 14.5185 us |
|  MegaSudokuB (16x16) |   322.208 us |  1.8862 us |  1.7644 us |
