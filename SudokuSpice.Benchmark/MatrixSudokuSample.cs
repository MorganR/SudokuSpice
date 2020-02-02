using System;

namespace SudokuSpice.Benchmark
{
class MatrixSudokuSample
{
    internal int?[,] Puzzle { get; set;}
    internal int?[,] Solution { get; set;}

    internal MatrixSudokuSample(SudokuSample sample)
    {
        Puzzle = NumberStringToMatrix(sample.Puzzle); 
        Solution = NumberStringToMatrix(sample.Solution); 
    }

    public bool MatchesSolution(Puzzle puzzle)
    {
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                if (puzzle[row, col] != Solution[row, col])
                {
                    return false;
                }
            }
        }
        return true;
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