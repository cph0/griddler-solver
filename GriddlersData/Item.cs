using Griddlers.Database;

namespace Griddlers.Library
{
    public readonly struct Item
    {
        public readonly int Index { get; }
        public readonly int Value { get; }
        public readonly string Colour { get; }

        public Item(int index, string v, bool fromFile = true)
        {
            Index = index;

            string[] Parts = v.Split(".");
            Value = int.Parse(Parts[0]);
            Colour = GetColourFromFile(Parts);
        }
        public Item(GriddlerItem item)
        {
            Index = item.position;
            Value = item.value;
            Colour = item.green ? "lightgreen" : "black";
        }
        public Item(int v, string colour)
        {
            Index = 0;
            Value = v;
            Colour = colour;
        }

        private static string GetColourFromFile(string[] fileString)
        {
            var colourString = fileString.Length > 1 ? fileString[1] : string.Empty;
            return colourString switch
            {
                "1" => "lightgreen",
                _ => "black"
            };
        }

        public static int operator +(Item a, Item b)
        {
            int Sum = a.Value + b.Value;

            if (a.Colour == b.Colour)
                Sum++;

            return Sum;
        }

        public static bool operator ==(Item a, Item b)
        {
            return a.Value == b.Value && a.Colour == b.Colour;
        }

        public static bool operator !=(Item a, Item b)
        {
            return !(a == b);
        }

        public static bool operator ==(Item a, Point b)
        {
            return a.Colour == b.Colour;
        }

        public static bool operator !=(Item a, Point b)
        {
            return a.Colour != b.Colour;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }
    }
}