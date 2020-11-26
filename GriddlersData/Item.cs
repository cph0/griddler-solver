using Griddlers.Database;

namespace Griddlers.Library
{
    public readonly struct Item
    {
        public readonly int Index { get; }
        public readonly int Value { get; }
        public readonly bool Green { get; }

        public Item(int index, string v)
        {
            Index = index;

            string[] Parts = v.Split(".");
            Value = int.Parse(Parts[0]);
            Green = Parts.Length > 1 && Parts[1] == "1";
        }
        public Item(GriddlerItem item)
        {
            Index = item.position;
            Value = item.value;
            Green = item.green;
        }
        public Item(int v, bool g)
        {
            Index = 0;
            Value = v;
            Green = g;
        }

        public static int operator +(Item a, Item b)
        {
            int Sum = a.Value + b.Value;

            if (a.Green == b.Green)
                Sum++;

            return Sum;
        }

        public static bool operator ==(Item a, Item b)
        {
            return a.Value == b.Value && a.Green == b.Green;
        }

        public static bool operator !=(Item a, Item b)
        {
            return !(a == b);
        }

        public static bool operator ==(Item a, Point b)
        {
            return a.Green == b.Green;
        }

        public static bool operator !=(Item a, Point b)
        {
            return a.Green != b.Green;
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