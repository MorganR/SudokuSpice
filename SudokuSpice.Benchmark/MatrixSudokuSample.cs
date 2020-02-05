using System;

namespace SudokuSpice.Benchmark
{
class MatrixSudokuSample
{
    internal PuzzleSample Puzzle { get; set;}
    internal PuzzleSample Solution { get; set;}

    internal MatrixSudokuSample(SudokuSample sample)
    {
        Puzzle = new PuzzleSample("puzzle", NumberStringToMatrix(sample.Puzzle)); 
        Solution = new PuzzleSample("solution", NumberStringToMatrix(sample.Solution)); 
    }

    private static int?[,] NumberStringToMatrix(string nums)
    {
        var matrix = new int?[9,9];
        int idx = -1;
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                ++idx;
                var c = nums[idx];
                if (c == '0') {
                    continue;
                }
                matrix[row, col] = Int32.Parse(c.ToString());
            }
        }
        return matrix;
    }
 }
 }