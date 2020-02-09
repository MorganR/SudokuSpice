using System.Collections.Generic;

namespace SudokuSpice.Benchmark
{
public static class Puzzles
{
    public static IEnumerable<PuzzleSample> NineByNinePuzzles() => new[]
    {
        new PuzzleSample(
            "Easy",
            new int?[,]
            {
                {   4, null,    2, null, null,    1,    8,    7,    6},
                {   3, null,    8, null, null,    5, null,    9,    4},
                {   6, null,    9,    4, null,    8,    3, null,    5},
                {null,    3,    1, null,    6, null, null, null, null},
                {   2,    4,    5,    9, null,    7,    1,    6,    3},
                {   9, null,    7,    2, null,    3,    5,    4,    8},
                {null,    9, null,    8, null,    2, null, null, null},
                {   1,    8,    3, null,    4,    9,    6,    5,    2},
                {   5,    2,    4,    1,    3,    6,    9, null,    7}
            }),
        new PuzzleSample(
            "Medium",
            new int?[,]
            {
                {null,    2, null,    6, null,    8, null, null, null},
                {   5,    8, null, null, null,    9,    7, null, null},
                {null, null, null, null,    4, null, null, null, null},
                {   3,    7, null, null, null, null,    5, null, null},
                {   6, null, null, null, null, null, null, null,    4},
                {null, null,    8, null, null, null, null,    1,    3},
                {null, null, null, null,    2, null, null, null, null},
                {null, null,    9,    8, null, null, null,    3,    6},
                {null, null, null,    3, null,    6, null,    9, null},
            }),
        new PuzzleSample(
            "HardA",
            new int?[,]
            {
                {   1, null, null, null,    2,    6, null, null, null},
                {   7, null,    6, null, null,    5, null, null, null},
                {null, null,    5,    8,    1, null, null, null, null},
                {null,    5, null, null,    8, null,    1, null, null},
                {null,    2, null, null, null, null, null,    8, null},
                {null, null,    1, null,    6, null, null,    3, null},
                {null, null, null, null,    5,    8,    4, null, null},
                {null, null, null,    6, null, null,    3, null,    9},
                {null, null, null,    2,    4, null, null, null,    5}
            }),
        new PuzzleSample(
            "HardB",
            new int?[,]
            {
                {null, null,    6, null,    1, null,    9, null, null},
                {   7, null, null,    3, null, null, null,    6,    5},
                {null, null, null, null,    7, null,    4, null,    8},
                {   6, null, null, null, null,    1, null, null, null},
                {null, null,    2, null, null, null,    5, null, null},
                {null, null, null,    2, null, null, null, null,    9},
                {   2, null,    8, null,    4, null, null, null, null},
                {   1,    3, null, null, null,    7, null, null,    6},
                {null, null,    4, null,    8, null,    1, null, null}
            }),
        new PuzzleSample(
            "EvilA",
            new int?[,]
            {
                {null, null, null,    6, null, null,    4, null, null},
                {   7, null, null, null, null,    3, 6, null, null},
                {null, null, null, null,    9,    1, null,    8, null},
                {null, null, null, null, null, null, null, null, null},
                {null,    5, null,    1,    8, null, null, null,    3},
                {null, null, null,    3, null,    6, null,    4,    5},
                {null,    4, null,    2, null, null, null,    6, null},
                {   9, null,    3, null, null, null, null, null, null},
                {null,    2, null, null, null, null,    1, null, null}
            }),
        new PuzzleSample(
            "EvilB",
            new int?[,]
            {
                {null,    2, null, null, null, null, null, null, null},
                {null, null, null,    6, null, null, null, null,    3},
                {null,    7,    4, null,    8, null, null, null, null},
                {null, null, null, null, null,    3, null, null,    2},
                {null,    8, null, null,    4, null, null,    1, null},
                {   6, null, null,    5, null, null, null, null, null},
                {null, null, null, null,    1, null,    7,    8, null},
                {   5, null, null, null, null,    9, null, null, null},
                {null, null, null, null, null, null, null,    4, null}
            }),
    };

    public static IEnumerable<PuzzleSample> MegaPuzzles() => new[]
    {
        new PuzzleSample(
            "MegaA",
            new int?[,]
            {
                {null, null, null, null,   10,   1,    null, 8,    null, 15,   3,    11,   null, 2,    16,   null},
                {14,   null, 2,    null, null, 4,    3,    null, null, 13,   8,    null, null, 12,   null, null},
                {null, null, null, 12,   null, null, null, 15,   null, null, null, 7,    null, null, 9,    10},
                {1,    10,   15,   null, 6,    null, null, null, null, 14,   null, null, null, null, null, 11},
                {null, 11,   14,   6,    null, null, null, 9,    13,   8,    null, null, null, null, 2,    3},
                {12,   null, null, null, 4,    null, 7,    3,    11,   6,    null, null, 16,   null, 5,    null},
                {13,   16,   null, 2,    null, null, null, 1,    null, null, 5,    null, 10,   9,    null, null},
                {null, 4,    null, null, 13,   null, 2,    null, null, null, 16,   3,    11,   null, null, null},
                {null, null, null, 10,   3,    6,    null, null, null, 9,    null, 12,   null, null, 4,    null},
                {null, null, 12,   15,   null, 9,    null, null, 7,    null, null, null, 1,    null, 3,    14},
                {null, 1,    null, 4,    null, null, 5,    12,   3,    10,   null, 8,    null, null, null, 2},
                {3,    6,    null, null, null, null, 15,   10,   4,    null, null, null, 12,   5,    7,    null},
                {2,    null, null, null, null, null, 4,    null, null, null, null, 15,   null, 16,   11,   9},
                {4,    14,   null, null, 16,   null, null, null, 2,    null, null, null, 6,    null, null, null},
                {null, null, 16,   null, null, 7,    8,    null, null, 4,    10,   null, null, 14,   null, 5},
                {null, 3,    6,    null, 9,    12,   14,   null, 8,    null, 13,   16,   null, null, null, null}
            }),
        new PuzzleSample(
            "MegaB",
            new int?[,]
            {
                {null,   15,    9, null, null,    5,   14,    2, null,    1, null, null, null, null,    8, null},
                {null, null, null,   16, null,    3,    6,    8, null, null, null,    7,    1,    5, null,    4},
                {   5, null, null,    2, null, null,   13, null,   14, null,    4, null,    3, null, null,   16},
                {null, null,    8, null, null, null, null,   15,    5, null, null,   12, null,    6, null,    2},
                {   8, null, null, null, null, null,    7, null,    1,   10, null, null, null, null,   11, null},
                {   7, 9, null,   15, null, null, null,   13, null, null, null,    5,   16, null,    2,    8},
                {null, null, null, null,   15,   10, null, null,    4, null,   16,    6, null,    7, null, null},
                {null, null,    2,   14, null,   11,   12, null, null,   13, null, null,    6, null,    1, null},
                {null,    5, null,   10, null, null,   16, null, null,   15,    8, null,    4,    3, null, null},
                {null, null,   16, null,    6,    9, null,   12, null, null,   11,    3, null, null, null, null},
                {   6,    3, null,   11,    8, null, null, null,   13, null, null, null,    7, null,   12,   10},
                {null,    8, null, null, null, null,   15,   10, null,    7, null, null, null, null, null,    9},
                {   3, null,    7, null,    1, null, null,   14,    9, null, null, null, null,   12, null, null},
                {  15, null, null,    8, null,    2, null,   16, null,   12, null, null,   10, null, null,   13},
                {2, null,   14,   12,   11, null, null, null,   15,    4,   10, null,    9, null, null, null},
                {null,    1, null, null, null, null,    5, null,   16,    6,    3, null, null,   14,    4, null}
            })
    };
}
}