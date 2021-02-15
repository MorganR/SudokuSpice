﻿using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public class ExactCoverGraphTest
    {
        [Fact]
        public void GetAllPossibilitiesAt_ForUnsetCoordinate_ReturnsExpectedPossibilities()
        {
            var puzzle = new Puzzle(4);
            ExactCoverGraph graph = ExactCoverGraph.Create(puzzle);

            var possibilities = graph.GetAllPossibilitiesAt(new Coordinate());

            Assert.NotNull(possibilities);
            Assert.Equal(puzzle.Size, possibilities!.Length);
            for (int i = 0; i < puzzle.Size; ++i)
            {
                Assert.NotNull(possibilities[i]);
                Assert.Equal(i, possibilities[i]!.Index);
            }
        }

        [Fact]
        public void GetAllPossibilitiesAt_ForPresetCoordinate_ReturnsNull()
        {
            var puzzle = new Puzzle(4);
            var coord = new Coordinate();
            puzzle[in coord] = 1;
            ExactCoverGraph graph = ExactCoverGraph.Create(puzzle);

            Assert.Null(graph.GetAllPossibilitiesAt(in coord));
        }
    }
}