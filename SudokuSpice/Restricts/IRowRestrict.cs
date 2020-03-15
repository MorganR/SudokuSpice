namespace SudokuSpice
{
    public interface IRowRestrict : ISudokuRestrict
    {
        public BitVector GetPossibleRowValues(int row);
    }
}
