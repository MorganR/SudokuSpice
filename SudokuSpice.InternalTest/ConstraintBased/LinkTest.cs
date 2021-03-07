using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.InternalTest
{
    public class LinkTest
    {
        [Fact]
        public void CreateConnectedLink_ConnectsCorrectly()
        {
            var possibility = new FakePossibility();
            var objective = new FakeObjective();

            var link = Link.CreateConnectedLink(possibility, objective);

            Assert.Single(possibility.AttachedObjectives, link);
            Assert.Single(objective.AttachedPossibilities, link);
            Assert.Same(link, link.NextOnObjective);
            Assert.Same(link, link.PreviousOnObjective);
            Assert.Same(link, link.NextOnPossibility);
            Assert.Same(link, link.PreviousOnPossibility);
            Assert.Same(possibility, link.Possibility);
            Assert.Same(objective, link.Objective);
            Assert.Single(link.GetLinksOnObjective(), link);
            Assert.Single(link.GetLinksOnPossibility(), link);
        }

        [Fact]
        public void CreateConnectedLink_WithExistingLinksOnObjective_ConnectsCorrectly()
        {
            var possibility = new FakePossibility();
            var objective = new FakeObjective();

            var firstLink = Link.CreateConnectedLink(new FakePossibility(), objective);
            var link = Link.CreateConnectedLink(possibility, objective);

            Assert.Same(firstLink, link.PreviousOnObjective);
            Assert.Same(firstLink, link.NextOnObjective);
            Assert.Same(link, link.NextOnPossibility);
            Assert.Same(link, link.PreviousOnPossibility);
            Assert.Same(link, firstLink.PreviousOnObjective);
            Assert.Same(link, firstLink.NextOnObjective);
            Assert.Same(firstLink, firstLink.NextOnPossibility);
            Assert.Same(firstLink, firstLink.PreviousOnPossibility);
            Assert.Same(possibility, link.Possibility);
            Assert.Same(objective, link.Objective);
            Assert.Equal(2, link.GetLinksOnObjective().Count());
            Assert.Contains(link, link.GetLinksOnObjective());
            Assert.Contains(firstLink, link.GetLinksOnObjective());
            Assert.Single(link.GetLinksOnPossibility(), link);
            Assert.Single(firstLink.GetLinksOnPossibility(), firstLink);
        }

        [Fact]
        public void CreateConnectedLink_WithExistingLinksOnPossibility_ConnectsCorrectly()
        {
            var possibility = new FakePossibility();
            var objective = new FakeObjective();

            var firstLink = Link.CreateConnectedLink(possibility, new FakeObjective());
            var link = Link.CreateConnectedLink(possibility, objective);

            Assert.Same(link, link.PreviousOnObjective);
            Assert.Same(link, link.NextOnObjective);
            Assert.Same(firstLink, link.NextOnPossibility);
            Assert.Same(firstLink, link.PreviousOnPossibility);
            Assert.Same(firstLink, firstLink.PreviousOnObjective);
            Assert.Same(firstLink, firstLink.NextOnObjective);
            Assert.Same(link, firstLink.NextOnPossibility);
            Assert.Same(link, firstLink.PreviousOnPossibility);
            Assert.Same(possibility, link.Possibility);
            Assert.Same(objective, link.Objective);
            Assert.Equal(2, link.GetLinksOnPossibility().Count());
            Assert.Contains(link, link.GetLinksOnPossibility());
            Assert.Contains(firstLink, link.GetLinksOnPossibility());
            Assert.Single(link.GetLinksOnObjective(), link);
            Assert.Single(firstLink.GetLinksOnObjective(), firstLink);
        }

        [Fact]
        public void PopFromObjective_OnlyUpdatesLinkReferences()
        {
            var poppedPossibility = new FakePossibility();
            var secondPossibility = new FakePossibility();
            var objective = new FakeObjective();
            var poppedLink = Link.CreateConnectedLink(poppedPossibility, objective);
            var secondLink = Link.CreateConnectedLink(secondPossibility, objective);

            poppedLink.PopFromObjective();

            Assert.Same(secondLink, poppedLink.NextOnObjective);
            Assert.Same(secondLink, poppedLink.PreviousOnObjective);
            Assert.Same(secondLink, secondLink.NextOnObjective);
            Assert.Same(secondLink, secondLink.PreviousOnObjective);
            Assert.Same(poppedLink, objective.AttachedPossibilities.First());
        }

        [Fact]
        public void ReinsertToObjective_UndoesPop()
        {
            var poppedPossibility = new FakePossibility();
            var secondPossibility = new FakePossibility();
            var objective = new FakeObjective();
            var poppedLink = Link.CreateConnectedLink(poppedPossibility, objective);
            var secondLink = Link.CreateConnectedLink(secondPossibility, objective);

            poppedLink.PopFromObjective();
            poppedLink.ReinsertToObjective();

            Assert.Same(secondLink, poppedLink.NextOnObjective);
            Assert.Same(secondLink, poppedLink.PreviousOnObjective);
            Assert.Same(poppedLink, secondLink.NextOnObjective);
            Assert.Same(poppedLink, secondLink.PreviousOnObjective);
            Assert.Same(poppedLink, objective.AttachedPossibilities.First());
        }

        [Fact]
        public void PrependToPossibility_PrependsImmediatelyBeforeLink()
        {
            var possibility = new NoopPossibility();
            var objective = new NoopObjective();
            var firstLink = Link.CreateConnectedLink(possibility, objective);
            var secondLink = Link.CreateConnectedLink(possibility, objective);
            var thirdLink = Link.CreateConnectedLink(possibility, objective);

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
        public void PrependToObjective_PrependsImmediatelyBeforeLink()
        {
            var possibility = new NoopPossibility();
            var objective = new NoopObjective();
            var firstLink = Link.CreateConnectedLink(possibility, objective);
            var secondLink = Link.CreateConnectedLink(possibility, objective);
            var thirdLink = Link.CreateConnectedLink(possibility, objective);

            firstLink.PrependToObjective(secondLink);
            firstLink.PrependToObjective(thirdLink);

            Assert.Same(secondLink, firstLink.NextOnObjective);
            Assert.Same(thirdLink, secondLink.NextOnObjective);
            Assert.Same(firstLink, thirdLink.NextOnObjective);
            Assert.Same(thirdLink, firstLink.PreviousOnObjective);
            Assert.Same(secondLink, thirdLink.PreviousOnObjective);
            Assert.Same(firstLink, secondLink.PreviousOnObjective);
        }
        private class NoopPossibility : IPossibility
        {
            public bool IsConcrete { get; set; }
            public NodeState State { get; set; }

            void IPossibility.AppendObjective(Link toNewObjective) { }
            void IPossibility.ReturnFromObjective(Link toReattach) => throw new System.NotImplementedException();
            bool IPossibility.TryDropFromObjective(Link toDetach) => throw new System.NotImplementedException();
        }

        private class NoopObjective : IObjective
        {
            public NodeState State { get; set; }

            bool IObjective.IsRequired => throw new System.NotImplementedException();

            void IObjective.AppendPossibility(Link toNewPossibility) { }
            void IObjective.DeselectPossibility(Link toDeselect) => throw new System.NotImplementedException();
            IEnumerable<IPossibility> IObjective.GetUnknownDirectPossibilities() => throw new System.NotImplementedException();
            void IObjective.ReturnPossibility(Link toReturn) => throw new System.NotImplementedException();
            bool IObjective.TryDropPossibility(Link toDrop) => throw new System.NotImplementedException();
            bool IObjective.TrySelectPossibility(Link toSelect) => throw new System.NotImplementedException();
        }
    }
}