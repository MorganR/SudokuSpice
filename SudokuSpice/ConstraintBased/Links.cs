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
        internal static bool TryUpdateOnPossibility<TPossibility, TObjective>(
            Link<TPossibility, TObjective> sourceLink, Func<Link<TPossibility, TObjective>, bool> tryFn, Action<Link<TPossibility, TObjective>> undoFn)
            where TPossibility : class, IPossibility<TPossibility, TObjective>
            where TObjective : class, IObjective<TObjective, TPossibility>
        {
            Link<TPossibility, TObjective> link = sourceLink;
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
        internal static void RevertOnPossibility<TPossibility, TObjective>(
            Link<TPossibility, TObjective> sourceLink,
            Action<Link<TPossibility, TObjective>> fn)
            where TPossibility : class, IPossibility<TPossibility, TObjective>
            where TObjective : class, IObjective<TObjective, TPossibility>
        {
            Link<TPossibility, TObjective> lastLink = sourceLink.PreviousOnPossibility;
            Link<TPossibility, TObjective> link = lastLink;
            do
            {
                fn(link);
                link = link.PreviousOnPossibility;
            } while (link != lastLink);
        }
    }
}
