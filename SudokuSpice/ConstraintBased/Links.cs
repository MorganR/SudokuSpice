using System;

namespace SudokuSpice.ConstraintBased
{
    internal static class Links
    {
        /// <summary>
        /// Tries to perform <paramref name="tryFn"/> on each link while iterating forward through
        /// the links along the possibility. If any fail, performs <paramref name="undoFn"/> on all
        /// the already completed links in reverse order. Note that <paramref name="undoFn"/> is
        /// <em>not</em> called on the link that failed the <paramref name="tryFn"/> call.
        /// </summary>
        /// <param name="sourceLink">The first link to call <paramref name="tryFn"/> on.</param>
        /// <param name="tryFn">
        /// The operation to attempt. Returns true if the operation succeeds, else false. If this
        /// returns false, all state should be as if the function was never called.
        /// </param>
        /// <param name="undoFn">
        /// Undoes the operations performed by a succesful call to <paramref name="tryFn"/>.
        /// </param>
        /// <returns>True if all calls to <paramref name="tryFn"/> succeed, else false.</returns>
        internal static bool TryUpdateOnPossibility(
            Link sourceLink,
            Func<Link, bool> tryFn,
            Action<Link> undoFn)
        {
            Link link = sourceLink;
            do
            {
                if (!tryFn(link))
                {
                    while (link != sourceLink)
                    {
                        link = link.PreviousOnPossibility;
                        undoFn(link);
                    }
                    return false;
                }
                link = link.NextOnPossibility;
            } while (link != sourceLink);
            return true;
        } 

        /// <summary>
        /// Performs <paramref name="fn"/> on all the links on the possibility dimension, iterating
        /// in reverse and ending at <paramref name="sourceLink"/>.
        ///
        /// This behavior is equivalent to how
        /// <see cref="TryUpdateOnPossibility{TPossibility, TObjective}(Link{TPossibility, TObjective}, Func{Link{TPossibility, TObjective}, bool}, Action{Link{TPossibility, TObjective}})"/>
        /// performs the <c>undoFn</c>. This is useful, for example, to undo a successful call to
        /// <see cref="TryUpdateOnPossibility{TPossibility, TObjective}(Link{TPossibility, TObjective}, Func{Link{TPossibility, TObjective}, bool}, Action{Link{TPossibility, TObjective}})"/>
        /// at a later time. In this case, <paramref name="sourceLink"/> should be the same for each
        /// method.
        /// </summary>
        /// <param name="sourceLink">
        /// The source link for an operation. This will be the last link for which
        /// <paramref name="fn"/> is called.
        /// </param>
        /// <param name="fn">The operation to perform.</param>
        internal static void RevertOnPossibility(
            Link sourceLink,
            Action<Link> fn)
        {
            Link lastLink = sourceLink.PreviousOnPossibility;
            Link link = lastLink;
            do
            {
                fn(link);
                link = link.PreviousOnPossibility;
            } while (link != lastLink);
        }

        /// <summary>
        /// Tries to perform <paramref name="tryFn"/> on each link except the
        /// <paramref name="sourceLink"/> while iterating forward through the links along the
        /// possibility. If any fail, performs <paramref name="undoFn"/> on all the already
        /// completed links in reverse order. Note that <paramref name="undoFn"/> is <em>not</em>
        /// called on the link that failed the <paramref name="tryFn"/> call.
        /// </summary>
        /// <param name="sourceLink">
        /// The source to iterate from. <paramref name="tryFn"/> is not called on this link.
        /// </param>
        /// <param name="tryFn">
        /// The operation to attempt. Returns true if the operation succeeds, else false. If this
        /// returns false, all state should be as if the function was never called.
        /// </param>
        /// <param name="undoFn">
        /// Undoes the operations performed by a succesful call to <paramref name="tryFn"/>.
        /// </param>
        /// <returns>True if all calls to <paramref name="tryFn"/> succeed, else false.</returns>
        internal static bool TryUpdateOthersOnPossibility(
            Link sourceLink,
            Func<Link, bool> tryFn,
            Action<Link> undoFn) 
        {
            for (Link? toUpdate = sourceLink.NextOnPossibility;
                toUpdate != sourceLink;
                toUpdate = toUpdate.NextOnPossibility)
            {
                if (!tryFn(toUpdate))
                {
                    for (toUpdate = toUpdate.PreviousOnPossibility;
                        toUpdate != sourceLink;
                        toUpdate = toUpdate.PreviousOnPossibility)
                    {
                        undoFn(toUpdate);
                    }
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Performs <paramref name="fn"/> on all the links except the <paramref name="sourceLink"/>
        /// on the possibility dimension, iterating in reverse and ending at the iteration after
        /// <paramref name="sourceLink"/>.
        ///
        /// This behavior is equivalent to how
        /// <see cref="TryUpdateOthersOnPossibility{TPossibility, TObjective}(Link{TPossibility, TObjective}, Func{Link{TPossibility, TObjective}, bool}, Action{Link{TPossibility, TObjective}})"/>
        /// performs the <c>undoFn</c>. This is useful, for example, to undo a successful call to
        /// <see cref="TryUpdateOthersOnPossibility{TPossibility, TObjective}(Link{TPossibility, TObjective}, Func{Link{TPossibility, TObjective}, bool}, Action{Link{TPossibility, TObjective}})"/>
        /// at a later time. In this case, <paramref name="sourceLink"/> should be the same for each
        /// method.
        /// </summary>
        /// <param name="sourceLink">
        /// The source to iterate from. <paramref name="fn"/> is not called on this link.
        /// </param>
        /// <param name="fn">The operation to perform.</param>
        internal static void RevertOthersOnPossibility(
            Link sourceLink,
            Action<Link> fn) 
        {
            for (
                Link? toRevert = sourceLink.PreviousOnPossibility;
                toRevert != sourceLink;
                toRevert = toRevert.PreviousOnPossibility)
            {
                fn(toRevert);
            }
        }

        /// <summary>
        /// Tries to perform <paramref name="tryFn"/> on each link while iterating forward through
        /// the links along the objective. If any fail, performs <paramref name="undoFn"/> on all
        /// the already completed links in reverse order. Note that <paramref name="undoFn"/> is
        /// <em>not</em> called on the link that failed the <paramref name="tryFn"/> call.
        /// </summary>
        /// <param name="sourceLink">The first link to call <paramref name="tryFn"/> on.</param>
        /// <param name="tryFn">
        /// The operation to attempt. Returns true if the operation succeeds, else false. If this
        /// returns false, all state should be as if the function was never called.
        /// </param>
        /// <param name="undoFn">
        /// Undoes the operations performed by a succesful call to <paramref name="tryFn"/>.
        /// </param>
        /// <returns>True if all calls to <paramref name="tryFn"/> succeed, else false.</returns>
        internal static bool TryUpdateOnObjective(
            Link sourceLink,
            Func<Link, bool> tryFn,
            Action<Link> undoFn)
        {
            Link link = sourceLink;
            do
            {
                if (!tryFn(link))
                {
                    while (link != sourceLink)
                    {
                        link = link.PreviousOnObjective;
                        undoFn(link);
                    }
                    return false;
                }
                link = link.NextOnObjective;
            } while (link != sourceLink);
            return true;
        } 



        /// <summary>
        /// Performs <paramref name="fn"/> on each link while iterating forward through the links
        /// along the objective.
        /// </summary>
        /// <param name="sourceLink">The first link to call <paramref name="tryFn"/> on.</param>
        /// <param name="fn">
        /// The operation to perform on each link.
        /// </param>
        internal static void UpdateOnObjective(
            Link sourceLink,
            Action<Link> fn)
        {
            Link link = sourceLink;
            do
            {
                fn(link);
                link = link.NextOnObjective;
            } while (link != sourceLink);
        } 

        /// <summary>
        /// Performs <paramref name="fn"/> on all the links on the objective dimension, iterating
        /// in reverse and ending at <paramref name="sourceLink"/>.
        ///
        /// This behavior is the same as
        /// <see cref="UpdateOnObjective{TPossibility, TObjective}(Link{TPossibility, TObjective}, Action{Link{TPossibility, TObjective}})"/>,
        /// but operates in the opposite direction.
        /// </summary>
        /// <param name="sourceLink">
        /// The source link for an operation. This will be the last link for which
        /// <paramref name="fn"/> is called.
        /// </param>
        /// <param name="fn">The operation to perform.</param>
        internal static void RevertOnObjective(
            Link sourceLink,
            Action<Link> fn)
        {
            Link lastLink = sourceLink.PreviousOnObjective;
            Link link = lastLink;
            do
            {
                fn(link);
                link = link.PreviousOnObjective;
            } while (link != lastLink);
        }

        /// <summary>
        /// Tries to perform <paramref name="tryFn"/> on each link except the
        /// <paramref name="sourceLink"/> while iterating forward through the links along the
        /// objective. If any fail, performs <paramref name="undoFn"/> on all the already
        /// completed links in reverse order. Note that <paramref name="undoFn"/> is <em>not</em>
        /// called on the link that failed the <paramref name="tryFn"/> call.
        /// </summary>
        /// <param name="sourceLink">
        /// The source to iterate from. <paramref name="tryFn"/> is not called on this link.
        /// </param>
        /// <param name="tryFn">
        /// The operation to attempt. Returns true if the operation succeeds, else false. If this
        /// returns false, all state should be as if the function was never called.
        /// </param>
        /// <param name="undoFn">
        /// Undoes the operations performed by a succesful call to <paramref name="tryFn"/>.
        /// </param>
        /// <returns>True if all calls to <paramref name="tryFn"/> succeed, else false.</returns>
        internal static bool TryUpdateOthersOnObjective(
            Link sourceLink,
            Func<Link, bool> tryFn,
            Action<Link> undoFn) 
        {
            for (Link? toUpdate = sourceLink.NextOnObjective;
                toUpdate != sourceLink;
                toUpdate = toUpdate.NextOnObjective)
            {
                if (!tryFn(toUpdate))
                {
                    for (toUpdate = toUpdate.PreviousOnObjective;
                        toUpdate != sourceLink;
                        toUpdate = toUpdate.PreviousOnObjective)
                    {
                        undoFn(toUpdate);
                    }
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Performs <paramref name="fn"/> on all the links except the <paramref name="sourceLink"/>
        /// on the objective dimension.
        /// </summary>
        /// <param name="sourceLink">
        /// The source to iterate from. <paramref name="fn"/> is not called on this link.
        /// </param>
        /// <param name="fn">The operation to perform.</param>
        internal static void UpdateOthersOnObjective(
            Link sourceLink,
            Action<Link> fn) 
        {
            for (Link? toUpdate = sourceLink.NextOnObjective;
                toUpdate != sourceLink;
                toUpdate = toUpdate.NextOnObjective)
            {
                fn(toUpdate);
            }
        }

        /// <summary>
        /// Performs <paramref name="fn"/> on all the links except the <paramref name="sourceLink"/>
        /// on the objective dimension, iterating in reverse and ending at the iteration after
        /// <paramref name="sourceLink"/>.
        ///
        /// This behavior is equivalent to how
        /// <see cref="TryUpdateOthersOnObjective{TPossibility, TObjective}(Link{TPossibility, TObjective}, Func{Link{TPossibility, TObjective}, bool}, Action{Link{TPossibility, TObjective}})"/>
        /// performs the <c>undoFn</c>. This is useful, for example, to undo a successful call to
        /// <see cref="TryUpdateOthersOnObjective{TPossibility, TObjective}(Link{TPossibility, TObjective}, Func{Link{TPossibility, TObjective}, bool}, Action{Link{TPossibility, TObjective}})"/>
        /// at a later time. In this case, <paramref name="sourceLink"/> should be the same for each
        /// method.
        /// </summary>
        /// <param name="sourceLink">
        /// The source to iterate from. <paramref name="fn"/> is not called on this link.
        /// </param>
        /// <param name="fn">The operation to perform.</param>
        internal static void RevertOthersOnObjective(
            Link sourceLink,
            Action<Link> fn) 
        {
            for (
                Link? toRevert = sourceLink.PreviousOnObjective;
                toRevert != sourceLink;
                toRevert = toRevert.PreviousOnObjective)
            {
                fn(toRevert);
            }
        }
    }
}
