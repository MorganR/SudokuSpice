namespace SudokuSpice.ConstraintBased
{
    internal interface IPossibility<T, TObjective>
        where T : class, IPossibility<T, TObjective>
        where TObjective : class, IObjective<TObjective, T>
    {
        /// <summary>
        /// Appends the given <paramref name="link"/> to this possibility, including updating the
        /// next and previous links as necessary to maintain a valid doubly linked list.
        /// </summary>
        /// <param name="link">The link to append and update.</param>
        void Append(Link<T, TObjective> link);
    }
}
