namespace SudokuSpice
{
    public interface IRowRestrict
    {
        public BitVector GetPossibleRowValues(int row);
    }
}
