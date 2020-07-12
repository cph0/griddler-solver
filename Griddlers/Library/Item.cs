using Griddlers.Database;

namespace Griddlers.Library
{
    public class Item
    {
        public int Index { get; private set; }
        private bool Incomplete { get; set; }
        public int Value { get; private set; }
        public bool Green { get; private set; }

        public Item() { }
        public Item(int index, string v)
        {
            Index = index;

            string[] Parts = v.Split(".");
            Value = int.Parse(Parts[0]);
            Green = Parts.Length > 1;
        }
        public Item(GriddlerItem item)
        {
            Index = item.position;
            Value = item.value;
            Green = item.green;
        }
        public Item(int v, bool g, bool i = true)
        {
            Value = v;
            Green = g;
            Incomplete = i;
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
            if (a.Green != b.Green)
                return true;

            if (a.Value < b.Value && !a.Incomplete)
                return true;

            if (a.Value > b.Value && !b.Incomplete)
                return true;

            return false;
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

        //public static bool operator <(Item a, Item b)
        //{
        //    return a.Value < b.Value || a.Green != b.Green;
        //}

        //public static bool operator >(Item a, Item b)
        //{
        //    return a.Value > b.Value || a.Green != b.Green;
        //}

        //public static bool operator <=(Item a, Item b)
        //{
        //    //return a.Value <= b.Value || a.Green != b.Green;
        //    return a < b || a == b;
        //}

        //public static bool operator >=(Item a, Item b)
        //{
        //    //return a.Value >= b.Value || a.Green != b.Green;
        //    return a > b || a == b;
        //}
    }
}
