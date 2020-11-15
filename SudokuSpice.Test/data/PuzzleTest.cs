using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.Test
{
    public class PuzzleTest
    {
        [Fact]
        public void CopyConstructor_CreatesDeepCopy()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            var puzzleCopy = new Puzzle(puzzle);

            Assert.Equal(puzzle.Size, puzzleCopy.Size);
            Assert.Equal(puzzle.NumEmptySquares, puzzleCopy.NumEmptySquares);
            Assert.Equal(puzzle.BoxSize, puzzleCopy.BoxSize);
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
        public void Constructor_WithValidSize_Works(int size)
        {
            var puzzle = new Puzzle(size);
            Assert.Equal(size, puzzle.Size);
            Assert.False(puzzle[0, 0].HasValue);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(36)]
        public void Constructor_WithInValidSize_Throws(int size)
        {
            Assert.Throws<ArgumentException>(() => new Puzzle(size));
        }

        [Fact]
        public void Size_ReturnsPuzzleSize()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            Assert.Equal(4, puzzle.Size);
        }

        [Fact]
        public void BoxSize_ReturnsSquareRootOfSize()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            Assert.Equal(2, puzzle.BoxSize);
        }

        [Fact]
        public void NumEmptySquares_MatchesNumNullInputs()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            Assert.Equal(10, puzzle.NumEmptySquares);
        }

        [Fact]
        public void Get_ReturnsCurrentKnownValue()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            Assert.Equal(1, puzzle[0, 0]);
            Assert.Null(puzzle[1, 1]);
            Assert.Equal(4, puzzle[3, 2]);
        }

        [Fact]
        public void Set_ModifiesValue()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            puzzle[0, 1] = 3;
            Assert.Equal(3, puzzle[0, 1]);
        }

        [Fact]
        public void Set_DecrementsNumEmptySquares()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            var initialNumUnset = puzzle.NumEmptySquares;
            puzzle[0, 1] = 3;
            Assert.Equal(initialNumUnset - 1, puzzle.NumEmptySquares);
        }

        [Fact]
        public void Unset_PreviouslySetValue_Succeeds()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            var initialNumUnset = puzzle.NumEmptySquares;
            puzzle[0, 1] = 3;
            puzzle[0, 1] = null;
            Assert.Null(puzzle[0, 1]);
            Assert.Equal(initialNumUnset, puzzle.NumEmptySquares);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 1, 0)]
        [InlineData(0, 2, 1)]
        [InlineData(1, 3, 1)]
        [InlineData(2, 1, 2)]
        [InlineData(3, 0, 2)]
        [InlineData(2, 2, 3)]
        [InlineData(3, 3, 3)]
        public void GetBoxIndex_SucceedsForValidValues(int row, int col, int box)
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            Assert.Equal(box, puzzle.GetBoxIndex(row, col));
        }


        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 0, 2)]
        [InlineData(2, 2, 0)]
        [InlineData(3, 2, 2)]
        public void GetStartingBoxCoordinate_SucceedsForValidValues(int box, int row, int col)
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            Assert.Equal(new Coordinate(row, col), puzzle.GetStartingBoxCoordinate(box));
        }

        [Fact]
        public void GetUnsetCoords_ReturnsAllUnsetCoords()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            var allUnset = puzzle.GetUnsetCoords().ToArray();
            foreach (var unset in allUnset)
            {
                Assert.Null(puzzle[unset]);
            }
            Assert.Equal(10, allUnset.Length);
        }

        [Fact]
        public void YieldUnsetForBox_ReturnsAllUnsetCoordsInBox()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            var box = 1;
            var allUnset = new List<Coordinate>(puzzle.YieldUnsetCoordsForBox(box));
            foreach (var unset in allUnset)
            {
                Assert.Equal(box, puzzle.GetBoxIndex(unset.Row, unset.Column));
                Assert.Null(puzzle[unset]);
            }
            Assert.Equal(2, allUnset.Count);
        }

        [Fact]
        public void ToString_ReturnsPrettyPuzzle()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            Assert.Equal("+---+---+\n"
                + "|1, | ,2|\n"
                + "| , |1, |\n"
                + "+---+---+\n"
                + "| ,1| , |\n"
                + "|3, |4, |\n"
                + "+---+---+\n", puzzle.ToString());
        }
    }
}