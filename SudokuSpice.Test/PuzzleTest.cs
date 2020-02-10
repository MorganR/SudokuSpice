using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice
{
    public class PuzzleTest
    {
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
        [InlineData(0, 0, 0b0001)]
        [InlineData(0, 1, 0b1111)]
        [InlineData(0, 3, 0b0010)]
        [InlineData(1, 0, 0b1111)]
        public void GetPossibleValues_Succeeds(int row, int col, uint possibles)
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            Assert.Equal(new BitVector(possibles), puzzle.GetPossibleValues(row, col));
        }

        [Fact]
        public void SetPossibleValues_ValidValue_Succeeds()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            puzzle.SetPossibleValues(0, 1, new BitVector(0b1100));
            Assert.Equal(new BitVector(0b1100), puzzle.GetPossibleValues(0, 1));
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
        public void YieldUnsetCoordsForRow_ReturnsAllUnsetCoordsInRow()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            var row = 1;
            var allUnset = new List<Coordinate>(puzzle.YieldUnsetCoordsForRow(row));
            foreach (var unset in allUnset)
            {
                Assert.Equal(unset.Row, row);
                Assert.Null(puzzle[unset]);
            }
            Assert.Equal(3, allUnset.Count);
        }

        [Fact]
        public void YieldUnsetCoordsForColumn_ReturnsAllUnsetCoordsInColumn()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            var col = 1;
            var allUnset = new List<Coordinate>(puzzle.YieldUnsetCoordsForColumn(col));
            foreach (var unset in allUnset)
            {
                Assert.Equal(unset.Column, col);
                Assert.Null(puzzle[unset]);
            }
            Assert.Equal(3, allUnset.Count);
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