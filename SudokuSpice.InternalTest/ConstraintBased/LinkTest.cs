using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public class LinkTest
    {
        [Fact]
        public void CreateConnectedLink_ConnectsCorrectly()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            var square = new Square(new Coordinate(0, 0), 2);
            var possibleSquare = new PossibleSquareValue(square, 1);
            var requirement = new Requirement(false, 1, matrix);

            var link = Link<PossibleSquareValue, Requirement>.CreateConnectedLink(possibleSquare, requirement);

            Assert.Same(link, link.PreviousOnObjective);
            Assert.Same(link, link.NextOnObjective);
            Assert.Same(link, link.NextOnPossibility);
            Assert.Same(link, link.PreviousOnPossibility);
            Assert.Same(possibleSquare, link.Possibility);
            Assert.Same(requirement, link.Objective);
            Assert.True(requirement.AreAllLinksRequired);
            Assert.False(requirement.AreRequiredLinksSelected);
            Assert.Same(link, requirement.FirstPossibilityLink);
            Assert.Same(link, possibleSquare.FirstLink);
        }

        [Fact]
        public void CreateConnectedLink_WithExistingLinks_ConnectsCorrectly()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            var square = new Square(new Coordinate(0, 0), 2);
            var possibleSquare = new PossibleSquareValue(square, 1);
            var requirement = new Requirement(false, 1, matrix);

            var firstLink = Link<PossibleSquareValue, Requirement>.CreateConnectedLink(possibleSquare, requirement);
            var link = Link<PossibleSquareValue, Requirement>.CreateConnectedLink(possibleSquare, requirement);

            Assert.Same(firstLink, link.PreviousOnObjective);
            Assert.Same(firstLink, link.NextOnObjective);
            Assert.Same(firstLink, link.NextOnPossibility);
            Assert.Same(firstLink, link.PreviousOnPossibility);
            Assert.Same(link, firstLink.PreviousOnObjective);
            Assert.Same(link, firstLink.NextOnObjective);
            Assert.Same(link, firstLink.NextOnPossibility);
            Assert.Same(link, firstLink.PreviousOnPossibility);
            Assert.Same(possibleSquare, link.Possibility);
            Assert.Same(requirement, link.Objective);
            Assert.False(requirement.AreAllLinksRequired);
            Assert.False(requirement.AreRequiredLinksSelected);
            Assert.Same(firstLink, requirement.FirstPossibilityLink);
            Assert.Same(firstLink, possibleSquare.FirstLink);
        }

        [Fact]
        public void PopFromObjective_OnlyUpdatesLinkReferences()
        {
            var poppedPossibility = new FakePossibility();
            var secondPossibility = new FakePossibility();
            var objective = new FakeObjective();
            var poppedLink = Link<FakePossibility, FakeObjective>.CreateConnectedLink(poppedPossibility, objective);
            var secondLink = Link<FakePossibility, FakeObjective>.CreateConnectedLink(secondPossibility, objective);

            poppedLink.PopFromObjective();

            Assert.Same(secondLink, poppedLink.NextOnObjective);
            Assert.Same(secondLink, poppedLink.PreviousOnObjective);
            Assert.Same(secondLink, secondLink.NextOnObjective);
            Assert.Same(secondLink, secondLink.PreviousOnObjective);
            Assert.Contains(poppedLink, objective.GetLinks());
            Assert.Contains(secondLink, objective.GetLinks());
        }

        [Fact]
        public void ReinsertToObjective_UndoesPop()
        {
            var poppedPossibility = new FakePossibility();
            var secondPossibility = new FakePossibility();
            var objective = new FakeObjective();
            var poppedLink = Link<FakePossibility, FakeObjective>.CreateConnectedLink(poppedPossibility, objective);
            var secondLink = Link<FakePossibility, FakeObjective>.CreateConnectedLink(secondPossibility, objective);

            poppedLink.PopFromObjective();
            poppedLink.ReinsertToObjective();

            Assert.Same(secondLink, poppedLink.NextOnObjective);
            Assert.Same(secondLink, poppedLink.PreviousOnObjective);
            Assert.Same(poppedLink, secondLink.NextOnObjective);
            Assert.Same(poppedLink, secondLink.PreviousOnObjective);
            Assert.Contains(poppedLink, objective.GetLinks());
        }

        [Fact]
        public void AppendToPossibility_AppendsImmediatelyAfterLink()
        {
            var possibility = new NoopPossibility();
            var objective = new NoopObjective();
            var firstLink = Link<NoopPossibility, NoopObjective>.CreateConnectedLink(possibility, objective);
            var secondLink = Link<NoopPossibility, NoopObjective>.CreateConnectedLink(possibility, objective);
            var thirdLink = Link<NoopPossibility, NoopObjective>.CreateConnectedLink(possibility, objective);

            firstLink.AppendToPossibility(secondLink);
            firstLink.AppendToPossibility(thirdLink);

            Assert.Same(thirdLink, firstLink.NextOnPossibility);
            Assert.Same(secondLink, thirdLink.NextOnPossibility);
            Assert.Same(firstLink, secondLink.NextOnPossibility);
            Assert.Same(secondLink, firstLink.PreviousOnPossibility);
            Assert.Same(thirdLink, secondLink.PreviousOnPossibility);
            Assert.Same(firstLink, thirdLink.PreviousOnPossibility);
        }

        [Fact]
        public void PrependToPossibility_PrependsImmediatelyBeforeLink()
        {
            var possibility = new NoopPossibility();
            var objective = new NoopObjective();
            var firstLink = Link<NoopPossibility, NoopObjective>.CreateConnectedLink(possibility, objective);
            var secondLink = Link<NoopPossibility, NoopObjective>.CreateConnectedLink(possibility, objective);
            var thirdLink = Link<NoopPossibility, NoopObjective>.CreateConnectedLink(possibility, objective);

            firstLink.PrependToPossibility(secondLink);
            firstLink.PrependToPossibility(thirdLink);

            Assert.Same(secondLink, firstLink.NextOnPossibility);
            Assert.Same(thirdLink, secondLink.NextOnPossibility);
            Assert.Same(firstLink, thirdLink.NextOnPossibility);
            Assert.Same(thirdLink, firstLink.PreviousOnPossibility);
            Assert.Same(secondLink, thirdLink.PreviousOnPossibility);
            Assert.Same(firstLink, secondLink.PreviousOnPossibility);
        }

        [Fact]
        public void AppendToObjective_AppendsImmediatelyAfterLink()
        {
            var possibility = new NoopPossibility();
            var objective = new NoopObjective();
            var firstLink = Link<NoopPossibility, NoopObjective>.CreateConnectedLink(possibility, objective);
            var secondLink = Link<NoopPossibility, NoopObjective>.CreateConnectedLink(possibility, objective);
            var thirdLink = Link<NoopPossibility, NoopObjective>.CreateConnectedLink(possibility, objective);

            firstLink.AppendToObjective(secondLink);
            firstLink.AppendToObjective(thirdLink);

            Assert.Same(thirdLink, firstLink.NextOnObjective);
            Assert.Same(secondLink, thirdLink.NextOnObjective);
            Assert.Same(firstLink, secondLink.NextOnObjective);
            Assert.Same(secondLink, firstLink.PreviousOnObjective);
            Assert.Same(thirdLink, secondLink.PreviousOnObjective);
            Assert.Same(firstLink, thirdLink.PreviousOnObjective);
        }

        [Fact]
        public void PrependToObjective_PrependsImmediatelyBeforeLink()
        {
            var possibility = new NoopPossibility();
            var objective = new NoopObjective();
            var firstLink = Link<NoopPossibility, NoopObjective>.CreateConnectedLink(possibility, objective);
            var secondLink = Link<NoopPossibility, NoopObjective>.CreateConnectedLink(possibility, objective);
            var thirdLink = Link<NoopPossibility, NoopObjective>.CreateConnectedLink(possibility, objective);

            firstLink.PrependToObjective(secondLink);
            firstLink.PrependToObjective(thirdLink);

            Assert.Same(secondLink, firstLink.NextOnObjective);
            Assert.Same(thirdLink, secondLink.NextOnObjective);
            Assert.Same(firstLink, thirdLink.NextOnObjective);
            Assert.Same(thirdLink, firstLink.PreviousOnObjective);
            Assert.Same(secondLink, thirdLink.PreviousOnObjective);
            Assert.Same(firstLink, secondLink.PreviousOnObjective);
        }



        private class FakePossibility : IPossibility<FakePossibility, FakeObjective>
        {
            public Link<FakePossibility, FakeObjective>? FirstLink;

            public IEnumerable<Link<FakePossibility, FakeObjective>> GetLinks()
            {
                if (FirstLink is null)
                {
                    yield break;
                }
                var link = FirstLink;
                do
                {
                    yield return link;
                    link = link.NextOnPossibility;
                } while (link != FirstLink);
            }

            public void Append(Link<FakePossibility, FakeObjective> link)
            {
                if (FirstLink is null)
                {
                    FirstLink = link;
                    return;
                }
                FirstLink.PrependToPossibility(link);
            }
        }

        private class FakeObjective : IObjective<FakeObjective, FakePossibility>
        {
            public Link<FakePossibility, FakeObjective>? FirstLink;

            public IEnumerable<Link<FakePossibility, FakeObjective>> GetLinks()
            {
                if (FirstLink is null)
                {
                    yield break;
                }
                var link = FirstLink;
                do
                {
                    yield return link;
                    link = link.NextOnObjective;
                } while (link != FirstLink);
            }

            public void Append(Link<FakePossibility, FakeObjective> link)
            {
                if (FirstLink is null)
                {
                    FirstLink = link;
                    return;
                }
                FirstLink.PrependToObjective(link);
            }
        }

        private class NoopPossibility : IPossibility<NoopPossibility, NoopObjective>
        {
            public void Append(Link<NoopPossibility, NoopObjective> link) { }
        }

        private class NoopObjective : IObjective<NoopObjective, NoopPossibility>
        {
            public void Append(Link<NoopPossibility, NoopObjective> link) { }
        }
    }
}