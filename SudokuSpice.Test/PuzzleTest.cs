﻿using System;
using Xunit;

namespace SudokuSpice.Test
{
    public class PuzzleTest
    {
        [Fact]
        public void CopyConstructor_CreatesDeepCopy()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] {1, null, null, 2},
                new int?[] {null, null, 1, null},
                new int?[] {null, 1, null, null},
                new int?[] {3, null, 4, null}
            });
            var puzzleCopy = new Puzzle(puzzle);

            Assert.Equal(puzzle.Size, puzzleCopy.Size);
            Assert.Equal(puzzle.NumEmptySquares, puzzleCopy.NumEmptySquares);
            Assert.Equal(puzzle.CountPerUniqueValue, puzzleCopy.CountPerUniqueValue);
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.Equal(puzzle[row, col], puzzleCopy[row, col]);
                }
            }
            puzzleCopy[1, 1] = 3;
            Assert.False(puzzle[1, 1].HasValue);
            Assert.Equal(3, puzzleCopy[1, 1]);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(9)]
        [InlineData(16)]
        [InlineData(25)]
        [InlineData(35)]
        public void Constructor_WithValidSize_Works(int size)
        {
            var puzzle = new Puzzle(size);
            Assert.Equal(size, puzzle.Size);
            Assert.False(puzzle[0, 0].HasValue);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Constructor_WithInvalidSize_Throws(int size)
        {
            Assert.Throws<ArgumentException>(() => new Puzzle(size));
            Assert.Throws<ArgumentException>(() => new Puzzle(size, new int[1]));
        }

        [Fact]
        public void Constructor_WithValidSizeAndPossibleValues_Works()
        {
            var size = 4;
            var possibleValues = new int[] { 0, 0, 1, 2 };
            var puzzle = new Puzzle(size, possibleValues);
            Assert.Equal(size, puzzle.Size);
            Assert.Equal(2, puzzle.CountPerUniqueValue[0]);
            Assert.Equal(1, puzzle.CountPerUniqueValue[1]);
            Assert.Equal(1, puzzle.CountPerUniqueValue[2]);
            Assert.False(puzzle.CountPerUniqueValue.ContainsKey(3));
            Assert.False(puzzle[0, 0].HasValue);
        }

        [Fact]
        public void Constructor_WithMatrixAndPossibleValues_Works()
        {
            var size = 4;
            var possibleValues = new int[] { 0, 0, 1, 2 };
            var puzzle = new Puzzle(new int?[][] {
                new int?[] { 0, null, null, null },
                new int?[] { 1, null,    0, null },
                new int?[] { 2, null, null,    1 },
                new int?[] { 0, null,    2, null },
            }, possibleValues);
            Assert.Equal(size, puzzle.Size);
            Assert.Equal(size * size, puzzle.NumSquares);
            Assert.Equal(7, puzzle.NumSetSquares);
            Assert.Equal(9, puzzle.NumEmptySquares);
            Assert.Equal(2, puzzle.CountPerUniqueValue[0]);
            Assert.Equal(1, puzzle.CountPerUniqueValue[1]);
            Assert.Equal(1, puzzle.CountPerUniqueValue[2]);
            Assert.False(puzzle.CountPerUniqueValue.ContainsKey(3));
            Assert.True(puzzle[0, 0].HasValue);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(9)]
        public void CountPerUniqueValue_IsOneByDefaultWithSizeConstructor(int size)
        {
            var puzzle = new Puzzle(size);
            for (int i = 1; i <= size; ++i)
            {
                Assert.Equal(1, puzzle.CountPerUniqueValue[i]);
            }
            Assert.False(puzzle.CountPerUniqueValue.ContainsKey(0));
            Assert.False(puzzle.CountPerUniqueValue.ContainsKey(size + 1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(9)]
        public void CountPerUniqueValue_IsOneByDefaultWithMatrixConstructor(int size)
        {
            var matrix = new int?[size][];
            for (int row = 0; row < size; ++row)
            {
                matrix[row] = new int?[size];
            }
            var puzzle = new Puzzle(matrix);
            for (int i = 1; i <= size; ++i)
            {
                Assert.Equal(1, puzzle.CountPerUniqueValue[i]);
            }
            Assert.False(puzzle.CountPerUniqueValue.ContainsKey(0));
            Assert.False(puzzle.CountPerUniqueValue.ContainsKey(10));
        }

        [Fact]
        public void Size_ReturnsPuzzleSize()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] {1, null, null, 2},
                new int?[] {null, null, 1, null},
                new int?[] {null, 1, null, null},
                new int?[] {3, null, 4, null}
            });
            Assert.Equal(4, puzzle.Size);
        }

        [Fact]
        public void NumEmptySquares_MatchesNumNullInputs()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] {1, null, null, 2},
                new int?[] {null, null, 1, null},
                new int?[] {null, 1, null, null},
                new int?[] {3, null, 4, null}
            });
            Assert.Equal(10, puzzle.NumEmptySquares);
        }

        [Fact]
        public void Get_ReturnsCurrentKnownValue()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] {1, null, null, 2},
                new int?[] {null, null, 1, null},
                new int?[] {null, 1, null, null},
                new int?[] {3, null, 4, null}
            });
            Assert.Equal(1, puzzle[0, 0]);
            Assert.Null(puzzle[1, 1]);
            Assert.Equal(4, puzzle[3, 2]);
        }

        [Fact]
        public void Set_ModifiesValue()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] {1, null, null, 2},
                new int?[] {null, null, 1, null},
                new int?[] {null, 1, null, null},
                new int?[] {3, null, 4, null}
            });
            puzzle[0, 1] = 3;
            Assert.Equal(3, puzzle[0, 1]);
        }

        [Fact]
        public void Set_DecrementsNumEmptySquares()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] {1, null, null, 2},
                new int?[] {null, null, 1, null},
                new int?[] {null, 1, null, null},
                new int?[] {3, null, 4, null}
            });
            int initialNumUnset = puzzle.NumEmptySquares;
            puzzle[0, 1] = 3;
            Assert.Equal(initialNumUnset - 1, puzzle.NumEmptySquares);
        }

        [Fact]
        public void Unset_PreviouslySetValue_Succeeds()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] {1, null, null, 2},
                new int?[] {null, null, 1, null},
                new int?[] {null, 1, null, null},
                new int?[] {3, null, 4, null}
            });
            int initialNumUnset = puzzle.NumEmptySquares;
            puzzle[0, 1] = 3;
            puzzle[0, 1] = null;
            Assert.Null(puzzle[0, 1]);
            Assert.Equal(initialNumUnset, puzzle.NumEmptySquares);
        }

        [Fact]
        public void GetUnsetCoords_ReturnsAllUnsetCoords()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] {1, null, null, 2},
                new int?[] {null, null, 1, null},
                new int?[] {null, 1, null, null},
                new int?[] {3, null, 4, null}
            });
            Coordinate[] allUnset = puzzle.GetUnsetCoords().ToArray();
            foreach (Coordinate unset in allUnset)
            {
                Assert.Null(puzzle[unset]);
            }
            Assert.Equal(10, allUnset.Length);
        }

        [Fact]
        public void ToString_WithBoxes_ReturnsPrettyPuzzle()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] {1, null, null, 2},
                new int?[] {null, null, 1, null},
                new int?[] {null, 1, null, null},
                new int?[] {3, null, 4, null}
            });
            Assert.Equal(
                "╔═╤═╦═╤═╗\n" +
                "║1│ ║ │2║\n" +
                "╟─┼─╫─┼─╢\n" +
                "║ │ ║1│ ║\n" +
                "╠═╪═╬═╪═╣\n" +
                "║ │1║ │ ║\n" +
                "╟─┼─╫─┼─╢\n" +
                "║3│ ║4│ ║\n" +
                "╚═╧═╩═╧═╝\n", puzzle.ToString());
        }

        [Fact]
        public void ToString_WithNoBoxes_ReturnsPrettyPuzzle()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] {1, null, null, 2, null},
                new int?[] {null, null, 1, null, null},
                new int?[] {null, 1, null, null, null},
                new int?[] {3, null, 4, null, null},
                new int?[] {null, null, null, null, null}
            });
            Assert.Equal(
                "╔═╤═╤═╤═╤═╗\n" +
                "║1│ │ │2│ ║\n" +
                "╟─┼─┼─┼─┼─╢\n" +
                "║ │ │1│ │ ║\n" +
                "╟─┼─┼─┼─┼─╢\n" +
                "║ │1│ │ │ ║\n" +
                "╟─┼─┼─┼─┼─╢\n" +
                "║3│ │4│ │ ║\n" +
                "╟─┼─┼─┼─┼─╢\n" +
                "║ │ │ │ │ ║\n" +
                "╚═╧═╧═╧═╧═╝\n", puzzle.ToString());
        }

    }
}