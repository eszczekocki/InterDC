
namespace InterpreterCore
{
    public struct Cursor
    {
        public int Pointer { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public Cursor(int pointer, int line, int column)
            : this()
        {
            Pointer = pointer;
            Line = line;
            Column = Column;
        }
    }
}
