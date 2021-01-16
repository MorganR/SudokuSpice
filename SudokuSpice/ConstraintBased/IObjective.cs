namespace SudokuSpice.ConstraintBased
{
    internal interface IObjective<T, TPossibility>
        where T : class, IObjective<T, TPossibility>
        where TPossibility : class, IPossibility<TPossibility, T>
    {
        /// <summary>
        /// Appends the given <paramref name="link"/> to this objective, including updating the next
        /// and previous links as necessary to maintain a valid doubly linked list.
        /// </summary>
        /// <param name="link">The link to append and update.</param>
        void Append(Link<TPossibility, T> link);
    }
}
