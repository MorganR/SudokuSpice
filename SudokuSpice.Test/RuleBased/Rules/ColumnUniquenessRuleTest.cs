using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.RuleBased.Rules.Test
{
    public class ColumnUniquenessRuleTest
    {
        [Fact]
        public void Constructor_FiltersCorrectly()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */,            1, null /* 2 */, 3},
                {           3, null /* 2 */, null /* 4 */, 1}
            });
            var rule = new ColumnUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size));
            Assert.Equal(new BitVector(0b10100), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b11100), rule.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b11110), rule.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b00000), rule.GetPossibleValues(new Coordinate(0, 3)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(9)]
        [InlineData(25)]
        public void Constructor_AcceptsValidPuzzleSizes(int size)
        {
            int?[,] matrix = new int?[size, size];
            var puzzle = new Puzzle(matrix);
            var rule = new ColumnUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size));
            Assert.NotNull(rule);
        }

        [Fact]
        public void Constructor_WithDuplicateValueInColumn_Throws()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(() =>
            {
                var rule = new ColumnUniquenessRule(
                    new Puzzle(
                        new int?[,] {
                            {                1, null /* 4 */, null /* 3 */, 2},
                            {/* INCORRECT */ 1, null /* 3 */, null /* 1 */, 4},
                            {     null /* 4 */,            1, null /* 2 */, 3},
                            {                3, null /* 2 */, null /* 4 */, 1}
                        }),
                    BitVector.CreateWithSize(4));
            });
            Assert.Contains("Puzzle has duplicate value in column", ex.Message);
        }

        [Fact]
        public void CopyWithNewReference_CreatesDeepCopy()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */,            1, null /* 2 */, 3},
                {           3, null /* 2 */, null /* 4 */, 1}
            });
            var rule = new ColumnUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size));

            var puzzleCopy = new Puzzle(puzzle);
            ISudokuRule ruleCopy = rule.CopyWithNewReference(puzzleCopy);
            int val = 4;
            var coord = new Coordinate(3, 2);
            ruleCopy.Update(coord, val, new CoordinateTracker(puzzle.Size));
            Assert.NotEqual(rule.GetPossibleValues(coord), ruleCopy.GetPossibleValues(coord));

            puzzleCopy[coord] = val;
            var secondCoord = new Coordinate(2, 2);
            int secondVal = 2;
            var coordTracker = new CoordinateTracker(puzzle.Size);
            ruleCopy.Update(secondCoord, secondVal, coordTracker);
            var originalCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Update(secondCoord, secondVal, originalCoordTracker);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(0, 2), new Coordinate(1, 2) },
                new HashSet<Coordinate>(coordTracker.GetTrackedCoords().ToArray()));
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(0, 2), new Coordinate(1, 2), new Coordinate(3, 2) },
                new HashSet<Coordinate>(originalCoordTracker.GetTrackedCoords().ToArray()));
        }

        [Fact]
        public void Update_UpdatesSpecifiedColumn()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */,            1, null /* 2 */, 3},
                {           3, null /* 2 */, null /* 4 */, 1}
            });
            var rule = new ColumnUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size));
            var coordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 1);
            int val = 3;
            rule.Update(coord, val, coordTracker);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(0, 1), new Coordinate(3, 1) },
                new HashSet<Coordinate>(coordTracker.GetTrackedCoords().ToArray()));
            Assert.Equal(new BitVector(0b10100), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b10100), rule.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b11110), rule.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b00000), rule.GetPossibleValues(new Coordinate(0, 3)));
        }

        [Fact]
        public void Revert_WithoutAffectedCoordsList_RevertsSpecifiedColumn()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */,            1, null /* 2 */, 3},
                {           3, null /* 2 */, null /* 4 */, 1}
            });
            var rule = new ColumnUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size));
            IList<BitVector> initialPossibleValuesByColumn = _GetPossibleValuesByColumn(puzzle.Size, rule);
            var coord = new Coordinate(1, 1);
            int val = 3;
            rule.Update(coord, val, new CoordinateTracker(puzzle.Size));

            rule.Revert(coord, val);

            for (int column = 0; column < initialPossibleValuesByColumn.Count; column++)
            {
                Assert.Equal(
                    initialPossibleValuesByColumn[column],
                    rule.GetMissingValuesForColumn(column));
            }
        }

        [Fact]
        public void Revert_RevertsSpecifiedColumn()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */,            1, null /* 2 */, 3},
                {           3, null /* 2 */, null /* 4 */, 1}
            });
            var rule = new ColumnUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size));
            IList<BitVector> initialPossibleValuesByColumn = _GetPossibleValuesByColumn(puzzle.Size, rule);
            var updatedCoordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 1);
            int val = 3;
            rule.Update(coord, val, updatedCoordTracker);

            var revertedCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Revert(coord, val, revertedCoordTracker);

            Assert.Equal(
                updatedCoordTracker.GetTrackedCoords().ToArray(),
                revertedCoordTracker.GetTrackedCoords().ToArray());
            for (int column = 0; column < initialPossibleValuesByColumn.Count; column++)
            {
                Assert.Equal(
                    initialPossibleValuesByColumn[column],
                    rule.GetMissingValuesForColumn(column));
            }
        }

        [Fact]
        public void GetPossibleValues_MatchesGetPossibleColumnValues()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */,            1, null /* 2 */, 3},
                {           3, null /* 2 */, null /* 4 */, 1}
            });
            var rule = new ColumnUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size));
            IList<BitVector> possibleValuesByColumn = _GetPossibleValuesByColumn(puzzle.Size, rule);

            for (int column = 0; column < possibleValuesByColumn.Count; column++)
            {
                for (int row = 0; row < puzzle.Size; row++)
                {
                    Assert.Equal(
                        possibleValuesByColumn[column],
                        rule.GetPossibleValues(new Coordinate(row, column)));
                }
            }
        }

        private IList<BitVector> _GetPossibleValuesByColumn(int numColumns, ColumnUniquenessRule rule)
        {
            var possibleColumnValues = new List<BitVector>();
            for (int column = 0; column < numColumns; column++)
            {
                possibleColumnValues.Add(rule.GetMissingValuesForColumn(column));
            }
            return possibleColumnValues;
        }

        private BitVector _GetAllPossibleValues(int size)
        {
            var possibleValues = BitVector.CreateWithSize(size + 1);
            possibleValues.UnsetBit(0);
            return possibleValues;
        }
    }
}
