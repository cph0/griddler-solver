using Griddlers.Database;

namespace Griddlers.Library
{
    public interface IColour
    {
        string Colour { get; }
    }

    public readonly struct Item : IColour
    {
        public readonly int Index { get; }
        public readonly int Value { get; }
        public readonly string Colour { get; }

        public Item(int index, string v)
        {
            Index = index;

            string[] Parts = v.Split(".");
            Value = int.Parse(Parts[0]);
            bool Green = Parts.Length > 1 && Parts[1] == "1";
            Colour = Green ? "green" : "black";
        }
        public Item(GriddlerItem item)
        {
            Index = item.position;
            Value = item.value;
            bool Green = item.green;
            Colour = Green ? "green" : "black";
        }

        public void Deconstruct(out int value, out string colour) 
        {
            value = Value;
            colour = Colour;
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