namespace Griddlers.Library
{
    public class Range
    {
        public int Start { get; protected set; }
        public int End { get; protected set; }
        public int Size => End - Start + 1;

        public Range(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}
