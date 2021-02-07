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
        public void PopFromPossibility_OnlyUpdatesLinkReferences()
        {
            var possibility = new FakePossibility();
            var poppedObjective = new FakeObjective();
            var secondObjective = new FakeObjective();
            var poppedLink = Link.CreateConnectedLink(possibility, poppedObjective);
            var secondLink = Link.CreateConnectedLink(possibility, secondObjective);

            poppedLink.PopFromPossibility();

            Assert.Same(secondLink, poppedLink.NextOnPossibility);
            Assert.Same(secondLink, poppedLink.PreviousOnPossibility);
            Assert.Same(secondLink, secondLink.NextOnPossibility);
            Assert.Same(secondLink, secondLink.PreviousOnPossibility);
            Assert.Same(poppedLink, possibility.AttachedObjectives.First());
        }

        [Fact]
        public void ReinsertToPossibility_UndoesPop()
        {
            var possibility = new FakePossibility();
            var poppedObjective = new FakeObjective();
            var secondObjective = new FakeObjective();
            var poppedLink = Link.CreateConnectedLink(possibility, poppedObjective);
            var secondLink = Link.CreateConnectedLink(possibility, secondObjective);

            poppedLink.PopFromPossibility();
            poppedLink.ReinsertToPossibility();

            Assert.Same(secondLink, poppedLink.NextOnPossibility);
            Assert.Same(secondLink, poppedLink.PreviousOnPossibility);
            Assert.Same(poppedLink, secondLink.NextOnPossibility);
            Assert.Same(poppedLink, secondLink.PreviousOnPossibility);
            Assert.Same(poppedLink, possibility.AttachedObjectives.First());
        }


        [Fact]
        public void AppendToPossibility_AppendsImmediatelyAfterLink()
        {
            var possibility = new NoopPossibility();
            var objective = new NoopObjective();
            var firstLink = Link.CreateConnectedLink(possibility, objective);
            var secondLink = Link.CreateConnectedLink(possibility, objective);
            var thirdLink = Link.CreateConnectedLink(possibility, objective);

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
        public void AppendToObjective_AppendsImmediatelyAfterLink()
        {
            var possibility = new NoopPossibility();
            var objective = new NoopObjective();
            var firstLink = Link.CreateConnectedLink(possibility, objective);
            var secondLink = Link.CreateConnectedLink(possibility, objective);
            var thirdLink = Link.CreateConnectedLink(possibility, objective);

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
            public NodeState State { get; set; }
            
            void IPossibility.AppendObjective(Link toNewObjective) { }
            void IPossibility.NotifyReattachedToObjective(Link toReattach) => throw new System.NotImplementedException();
            bool IPossibility.TryNotifyDroppedFromObjective(Link toDetach) => throw new System.NotImplementedException();
        }

        private class NoopObjective : IObjective
        {
            public NodeState State { get; set; }

            bool IObjective.IsRequired => throw new System.NotImplementedException();

            IReadOnlySet<IObjective> IObjective.RequiredObjectives => throw new System.NotImplementedException();

            void IObjective.AppendPossibility(Link toNewPossibility) { }
            void IObjective.DeselectPossibility(Link toDeselect) => throw new System.NotImplementedException();
            IEnumerable<IPossibility> IObjective.GetUnknownDirectPossibilities() => throw new System.NotImplementedException();
            void IObjective.ReturnPossibility(Link toReturn) => throw new System.NotImplementedException();
            bool IObjective.TryDropPossibility(Link toDrop) => throw new System.NotImplementedException();
            bool IObjective.TrySelectPossibility(Link toSelect) => throw new System.NotImplementedException();
        }
    }
}