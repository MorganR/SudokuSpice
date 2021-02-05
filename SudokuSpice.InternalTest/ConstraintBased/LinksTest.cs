using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.ConstraintBased.InternalTest
{
    public class LinksTest
    {
        [Fact]
        public void TryUpdateOnPossibility_UpdatesAll()
        {
            var links = CreateThreeLinksWithSamePossibility();

            var indicesSeen = new int[links.Count];
            indicesSeen.AsSpan().Fill(-1);
            int counter = 0;

            Assert.True(
                Links.TryUpdateOnPossibility(
                    links[0],
                    link =>
                    {
                        indicesSeen[counter++] = links.IndexOf(link);
                        return true;
                    },
                    _ => Assert.False(true, "Should not try to undo.")));
            Assert.Equal(new int[] { 0, 1, 2 }, indicesSeen);
        }

        [Fact]
        public void TryUpdateOnPossibility_WithFailure_RevertsAll()
        {
            var links = CreateThreeLinksWithSamePossibility();

            var indicesUpdated = new int[links.Count];
            indicesUpdated.AsSpan().Fill(-1);
            var indicesReverted = new int[links.Count];
            indicesReverted.AsSpan().Fill(-1);
            int updateCounter = 0;
            int revertCounter = 0;

            Assert.False(
                Links.TryUpdateOnPossibility(
                    links[0],
                    link =>
                    {
                        indicesUpdated[updateCounter++] = links.IndexOf(link);
                        return updateCounter < links.Count;
                    },
                    link =>
                    {
                        indicesReverted[revertCounter++] = links.IndexOf(link);
                    }));
            Assert.Equal(new int[] { 0, 1, 2 }, indicesUpdated);
            Assert.Equal(new int[] { 1, 0, -1 }, indicesReverted);
        }

        [Fact]
        public void RevertOnPossibility_RevertsAll()
        {
            var links = CreateThreeLinksWithSamePossibility();

            var indicesReverted = new int[links.Count];
            indicesReverted.AsSpan().Fill(-1);
            int revertCounter = 0;

            Links.RevertOnPossibility(
                links[0],
                link =>
                {
                    indicesReverted[revertCounter++] = links.IndexOf(link);
                });
            Assert.Equal(new int[] { 2, 1, 0 }, indicesReverted);
        }

        [Fact]
        public void TryUpdateOthersOnPossibility_UpdatesAll()
        {
            var links = CreateThreeLinksWithSamePossibility();

            var indicesSeen = new int[links.Count - 1];
            indicesSeen.AsSpan().Fill(-1);
            int counter = 0;

            Assert.True(
                Links.TryUpdateOthersOnPossibility(
                    links[0],
                    link =>
                    {
                        indicesSeen[counter++] = links.IndexOf(link);
                        return true;
                    },
                    _ => Assert.False(true, "Should not try to undo.")));
            Assert.Equal(new int[] { 1, 2 }, indicesSeen);
        }

        [Fact]
        public void ModifyOthers_WithOneLink_UpdatesNone()
        {
            var link = Link.CreateConnectedLink(new FakePossibility(), new FakeObjective());

            Assert.True(
                Links.TryUpdateOthersOnPossibility(
                    link,
                    _ =>
                    {
                        Assert.False(true, "Should not perform any operations");
                        return true;
                    },
                    _ => Assert.False(true, "Should not try to undo.")));
            Assert.True(
                Links.TryUpdateOthersOnObjective(
                    link,
                    _ =>
                    {
                        Assert.False(true, "Should not perform any operations");
                        return true;
                    },
                    _ => Assert.False(true, "Should not try to undo.")));
            Links.RevertOthersOnPossibility(
                link,
                _ => Assert.False(true, "Should not perform any operations."));
            Links.RevertOthersOnObjective(
                link,
                _ => Assert.False(true, "Should not perform any operations."));
        }

        [Fact]
        public void TryUpdateOthersOnPossibility_WithFailure_RevertsAll()
        {
            var links = CreateThreeLinksWithSamePossibility();

            var indicesUpdated = new int[links.Count - 1];
            indicesUpdated.AsSpan().Fill(-1);
            var indicesReverted = new int[links.Count - 1];
            indicesReverted.AsSpan().Fill(-1);
            int updateCounter = 0;
            int revertCounter = 0;

            Assert.False(
                Links.TryUpdateOthersOnPossibility(
                    links[0],
                    link =>
                    {
                        indicesUpdated[updateCounter++] = links.IndexOf(link);
                        return updateCounter < links.Count - 1;
                    },
                    link =>
                    {
                        indicesReverted[revertCounter++] = links.IndexOf(link);
                    }));
            Assert.Equal(new int[] { 1, 2 }, indicesUpdated);
            Assert.Equal(new int[] { 1, -1 }, indicesReverted);
        }

        [Fact]
        public void RevertOthersOnPossibility_RevertsAll()
        {
            var links = CreateThreeLinksWithSamePossibility();

            var indicesReverted = new int[links.Count - 1];
            indicesReverted.AsSpan().Fill(-1);
            int revertCounter = 0;

            Links.RevertOthersOnPossibility(
                links[0],
                link =>
                {
                    indicesReverted[revertCounter++] = links.IndexOf(link);
                });
            Assert.Equal(new int[] { 2, 1 }, indicesReverted);
        }

        [Fact]
        public void TryUpdateOnObjective_UpdatesAll()
        {
            var links = CreateThreeLinksWithSameObjective();

            var indicesSeen = new int[links.Count];
            indicesSeen.AsSpan().Fill(-1);
            int counter = 0;

            Assert.True(
                Links.TryUpdateOnObjective(
                    links[0],
                    link =>
                    {
                        indicesSeen[counter++] = links.IndexOf(link);
                        return true;
                    },
                    _ => Assert.False(true, "Should not try to undo.")));
            Assert.Equal(new int[] { 0, 1, 2 }, indicesSeen);
        }

        [Fact]
        public void TryUpdateOnObjective_WithFailure_RevertsAll()
        {
            var links = CreateThreeLinksWithSameObjective();

            var indicesUpdated = new int[links.Count];
            indicesUpdated.AsSpan().Fill(-1);
            var indicesReverted = new int[links.Count];
            indicesReverted.AsSpan().Fill(-1);
            int updateCounter = 0;
            int revertCounter = 0;

            Assert.False(
                Links.TryUpdateOnObjective(
                    links[0],
                    link =>
                    {
                        indicesUpdated[updateCounter++] = links.IndexOf(link);
                        return updateCounter < links.Count;
                    },
                    link =>
                    {
                        indicesReverted[revertCounter++] = links.IndexOf(link);
                    }));
            Assert.Equal(new int[] { 0, 1, 2 }, indicesUpdated);
            Assert.Equal(new int[] { 1, 0, -1 }, indicesReverted);
        }

        [Fact]
        public void RevertOnObjective_RevertsAll()
        {
            var links = CreateThreeLinksWithSameObjective();

            var indicesReverted = new int[links.Count];
            indicesReverted.AsSpan().Fill(-1);
            int revertCounter = 0;

            Links.RevertOnObjective(
                links[0],
                link =>
                {
                    indicesReverted[revertCounter++] = links.IndexOf(link);
                });
            Assert.Equal(new int[] { 2, 1, 0 }, indicesReverted);
        }

        [Fact]
        public void TryUpdateOthersOnObjective_UpdatesAll()
        {
            var links = CreateThreeLinksWithSameObjective();

            var indicesSeen = new int[links.Count - 1];
            indicesSeen.AsSpan().Fill(-1);
            int counter = 0;

            Assert.True(
                Links.TryUpdateOthersOnObjective(
                    links[0],
                    link =>
                    {
                        indicesSeen[counter++] = links.IndexOf(link);
                        return true;
                    },
                    _ => Assert.False(true, "Should not try to undo.")));
            Assert.Equal(new int[] { 1, 2 }, indicesSeen);
        }

        [Fact]
        public void TryUpdateOthersOnObjective_WithFailure_RevertsAll()
        {
            var links = CreateThreeLinksWithSameObjective();

            var indicesUpdated = new int[links.Count - 1];
            indicesUpdated.AsSpan().Fill(-1);
            var indicesReverted = new int[links.Count - 1];
            indicesReverted.AsSpan().Fill(-1);
            int updateCounter = 0;
            int revertCounter = 0;

            Assert.False(
                Links.TryUpdateOthersOnObjective(
                    links[0],
                    link =>
                    {
                        indicesUpdated[updateCounter++] = links.IndexOf(link);
                        return updateCounter < links.Count - 1;
                    },
                    link =>
                    {
                        indicesReverted[revertCounter++] = links.IndexOf(link);
                    }));
            Assert.Equal(new int[] { 1, 2 }, indicesUpdated);
            Assert.Equal(new int[] { 1, -1 }, indicesReverted);
        }

        [Fact]
        public void RevertOthersOnObjective_RevertsAll()
        {
            var links = CreateThreeLinksWithSameObjective();

            var indicesReverted = new int[links.Count - 1];
            indicesReverted.AsSpan().Fill(-1);
            int revertCounter = 0;

            Links.RevertOthersOnObjective(
                links[0],
                link =>
                {
                    indicesReverted[revertCounter++] = links.IndexOf(link);
                });
            Assert.Equal(new int[] { 2, 1 }, indicesReverted);
        }



        private List<Link> CreateThreeLinksWithSamePossibility()
        {
            var possibility = new FakePossibility();
            return new List<Link> {
                Link.CreateConnectedLink(possibility, new FakeObjective()),
                Link.CreateConnectedLink(possibility, new FakeObjective()),
                Link.CreateConnectedLink(possibility, new FakeObjective()),
            };
        }

        private List<Link> CreateThreeLinksWithSameObjective()
        {
            var objective = new FakeObjective();
            return new List<Link> {
                Link.CreateConnectedLink(new FakePossibility(), objective),
                Link.CreateConnectedLink(new FakePossibility(), objective), 
                Link.CreateConnectedLink(new FakePossibility(), objective),
            };
        }
    }
}
