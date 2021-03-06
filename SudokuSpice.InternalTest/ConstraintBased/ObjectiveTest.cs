﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.InternalTest
{
    public class ObjectiveTest
    {
        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(2, 1, true)]
        [InlineData(2, 2, true)]
        [InlineData(4, 1, true)]
        [InlineData(1, 1, false)]
        [InlineData(2, 1, false)]
        [InlineData(2, 2, false)]
        [InlineData(4, 1, false)]
        public void CreateFullyConnected_ConnectsCorrectly(int numPossibilities, int numRequired, bool asConcrete)
        {
            int size = 4;
            var puzzle = new Puzzle(size);
            var matrix = ExactCoverGraph.Create(puzzle);
            var possibilities = Possibilities.CreatePossibilities(new Coordinate(), numPossibilities);

            Objective objective;
            if (asConcrete)
            {
                objective = Objective.CreateFullyConnected(matrix,
                    new ReadOnlySpan<Possibility>(possibilities),
                    numRequired);
            } else
            {
                objective = Objective.CreateFullyConnected(matrix,
                    new ReadOnlySpan<IPossibility>(possibilities),
                    numRequired);
            }

            Assert.True(objective.AllUnknownPossibilitiesAreConcrete);
            Assert.True(((IObjective)objective).IsRequired);
            Assert.Equal(numPossibilities == numRequired, objective.AllUnknownPossibilitiesAreRequired);
            Assert.NotEqual(NodeState.SELECTED, objective.State);
            Assert.Equal(possibilities.Length, objective.CountUnknown);
            Assert.Equal(numRequired, objective.TotalCountToSatisfy);
            Assert.All(possibilities,
                p => Assert.Contains(p, ((IObjective)objective).GetUnknownDirectPossibilities()));
            Assert.All(((IObjective)objective).GetUnknownDirectPossibilities(),
                p => Assert.Contains(p, possibilities));
            Assert.Contains(objective, matrix.GetUnsatisfiedRequiredObjectivesWithConcretePossibilities());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CreateFullyConnected_NeedsMoreThanPossible_Throws(bool asConcrete)
        {
            int size = 4;
            var puzzle = new Puzzle(size);
            var matrix = ExactCoverGraph.Create(puzzle);
            var possibilities = Possibilities.CreatePossibilities(new Coordinate(), 1);
            Assert.Single(possibilities);

            Assert.Throws<ArgumentException>(
                () =>
                {
                    if (asConcrete)
                    {
                        Objective.CreateFullyConnected(matrix, new ReadOnlySpan<Possibility>(possibilities), possibilities.Length + 1);
                    } else
                    {
                        Objective.CreateFullyConnected(matrix, new ReadOnlySpan<IPossibility>(possibilities), possibilities.Length + 1);
                    }
                });
        }

        [Fact]
        public void AllUnknownPossibilitiesAreConcrete_UpdatesWhenPossibilitiesChange()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverGraph.Create(puzzle);
            var fakePossibility = new FakePossibility(isConcrete: false);
            var possibilities = new IPossibility[] {
                new Possibility(new Coordinate(), 0),
                fakePossibility,
            };

            var objective = Objective.CreateFullyConnected(matrix, possibilities, 1);

            Assert.Single(fakePossibility.AttachedObjectives);
            Assert.False(objective.AllUnknownPossibilitiesAreConcrete);

            Assert.True(((IObjective)objective).TryDropPossibility(fakePossibility.AttachedObjectives.First()));
            Assert.True(objective.AllUnknownPossibilitiesAreConcrete);
        }

        [Fact]
        public void CreateFullyConnected_WithOptionalObjectives_ConnectsCorrectly()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverGraph.Create(puzzle);
            var optionalObjectives = new OptionalObjective[] {
                OptionalObjective.CreateWithPossibilities(Possibilities.CreatePossibilities(new Coordinate(), 2), 1),
                OptionalObjective.CreateWithPossibilities(Possibilities.CreatePossibilities(new Coordinate(), 2), 1),
            };
            var concreteObjective = Objective.CreateFullyConnected(matrix, optionalObjectives, 1);
            IObjective objective = concreteObjective;

            Assert.Equal(
                new HashSet<IPossibility>(optionalObjectives),
                new HashSet<IPossibility>(objective.GetUnknownDirectPossibilities()));
        }

        [Fact]
        public void TrySelectAndDeselectPossibility_WithOneRequired_Works()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverGraph.Create(puzzle);
            var possibilities = new FakePossibility[] {
                new FakePossibility(),
                new FakePossibility(),
            };
            var selectedPossibility = possibilities[0];
            var droppedPossibility = possibilities[1];
            var objective = Objective.CreateFullyConnected(matrix, possibilities, 1);

            Assert.NotEqual(NodeState.SELECTED, objective.State);
            ((IObjective)objective).TrySelectPossibility(selectedPossibility.AttachedObjectives.First());
            Assert.Equal(NodeState.SELECTED, objective.State);
            Assert.DoesNotContain(objective, matrix.GetUnsatisfiedRequiredObjectives());
            Assert.Empty(((IObjective)objective).GetUnknownDirectPossibilities());
            Assert.Empty(selectedPossibility.DroppedFromObjectives);
            Assert.Single(droppedPossibility.DroppedFromObjectives);

            ((IObjective)objective).DeselectPossibility(selectedPossibility.AttachedObjectives.First());
            Assert.NotEqual(NodeState.SELECTED, objective.State);
            Assert.Contains(objective, matrix.GetUnsatisfiedRequiredObjectivesWithConcretePossibilities());
            Assert.Empty(selectedPossibility.DroppedFromObjectives);
            Assert.Empty(droppedPossibility.DroppedFromObjectives);
            Assert.Contains(selectedPossibility, ((IObjective)objective).GetUnknownDirectPossibilities());
            Assert.Contains(droppedPossibility, ((IObjective)objective).GetUnknownDirectPossibilities());
        }

        [Fact]
        public void TrySelectAndDeselectPossibility_WithMultipleRequired_Works()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverGraph.Create(puzzle);
            var possibilities = new FakePossibility[] {
                new FakePossibility(),
                new FakePossibility(),
            };
            var firstSelected = possibilities[0];
            var secondSelected = possibilities[1];
            var concreteObjective = Objective.CreateFullyConnected(matrix, possibilities, 2);
            IObjective objective = concreteObjective;

            Assert.True(objective.TrySelectPossibility(firstSelected.AttachedObjectives.First()));
            Assert.NotEqual(NodeState.SELECTED, concreteObjective.State);
            Assert.Equal(1, concreteObjective.CountUnknown);
            Assert.Single(objective.GetUnknownDirectPossibilities(), secondSelected);
            Assert.Contains(concreteObjective, matrix.GetUnsatisfiedRequiredObjectivesWithConcretePossibilities());

            Assert.True(objective.TrySelectPossibility(secondSelected.AttachedObjectives.First()));
            Assert.Equal(NodeState.SELECTED, concreteObjective.State);
            Assert.Equal(0, concreteObjective.CountUnknown);
            Assert.True(!matrix.GetUnsatisfiedRequiredObjectivesWithConcretePossibilities().Contains(concreteObjective) ||
                matrix.GetUnsatisfiedRequiredObjectivesWithConcretePossibilities().Count() == 1);

            objective.DeselectPossibility(secondSelected.AttachedObjectives.First());
            Assert.NotEqual(NodeState.SELECTED, concreteObjective.State);
            Assert.Equal(1, concreteObjective.CountUnknown);
            Assert.Single(objective.GetUnknownDirectPossibilities(), secondSelected);
            Assert.Contains(concreteObjective, matrix.GetUnsatisfiedRequiredObjectivesWithConcretePossibilities());

            objective.DeselectPossibility(firstSelected.AttachedObjectives.First());
            Assert.Equal(2, concreteObjective.CountUnknown);
        }

        [Fact]
        public void TrySelectPossibility_WithUndetachablePossibility_Fails()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverGraph.Create(puzzle);
            var possibilities = new FakePossibility[] {
                new FakePossibility(),
                new FakePossibility(),
                new FakePossibility(),
            };
            var toSelect = possibilities[0];
            var undetachable = possibilities[2];
            undetachable.CanBeDetached = false;
            var concreteObjective = Objective.CreateFullyConnected(matrix, possibilities, 1);
            IObjective objective = concreteObjective;

            Assert.False(objective.TrySelectPossibility(toSelect.AttachedObjectives.First()));
            Assert.Empty(possibilities[1].DroppedFromObjectives);
            Assert.Empty(undetachable.DroppedFromObjectives);
            Assert.Contains(objective, matrix.GetUnsatisfiedRequiredObjectivesWithConcretePossibilities());
            Assert.Contains(toSelect, objective.GetUnknownDirectPossibilities());
            Assert.Contains(possibilities[1], objective.GetUnknownDirectPossibilities());
            Assert.Contains(undetachable, objective.GetUnknownDirectPossibilities());
        }

        [Fact]
        public void TryDropAndReturnPossibility_Succeeds()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverGraph.Create(puzzle);
            var possibilities = new FakePossibility[] {
                new FakePossibility(),
                new FakePossibility(),
                new FakePossibility(),
            };
            var toDrop = possibilities[0];
            var concreteObjective = Objective.CreateFullyConnected(matrix, possibilities, 2);
            IObjective objective = concreteObjective;

            Assert.True(objective.TryDropPossibility(toDrop.AttachedObjectives.First()));
            Assert.Equal(2, concreteObjective.CountUnknown);
            Assert.Equal(2, objective.GetUnknownDirectPossibilities().Count());
            Assert.Contains(possibilities[1], objective.GetUnknownDirectPossibilities());
            Assert.Contains(possibilities[2], objective.GetUnknownDirectPossibilities());
            Assert.Empty(toDrop.DroppedFromObjectives);

            objective.ReturnPossibility(toDrop.AttachedObjectives.First());
            Assert.Equal(3, concreteObjective.CountUnknown);
            Assert.Equal(3, objective.GetUnknownDirectPossibilities().Count());
            Assert.Contains(toDrop, objective.GetUnknownDirectPossibilities());
            Assert.Contains(possibilities[1], objective.GetUnknownDirectPossibilities());
            Assert.Contains(possibilities[2], objective.GetUnknownDirectPossibilities());
        }

        [Fact]
        public void TryDropPossibility_WhenRequired_Fails()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverGraph.Create(puzzle);
            var possibilities = new FakePossibility[] {
                new FakePossibility(),
                new FakePossibility(),
                new FakePossibility(),
            };
            var toDrop = possibilities[0];
            var concreteObjective = Objective.CreateFullyConnected(matrix, possibilities, 3);
            IObjective objective = concreteObjective;

            Assert.False(objective.TryDropPossibility(toDrop.AttachedObjectives.First()));
            Assert.Equal(3, concreteObjective.CountUnknown);
            Assert.Equal(3, objective.GetUnknownDirectPossibilities().Count());
            Assert.Contains(toDrop, objective.GetUnknownDirectPossibilities());
            Assert.Contains(possibilities[1], objective.GetUnknownDirectPossibilities());
            Assert.Contains(possibilities[2], objective.GetUnknownDirectPossibilities());
        }
    }
}