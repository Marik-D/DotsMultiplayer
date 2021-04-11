using System;

namespace DefaultNamespace
{
    public class CellPos : IComparable<CellPos>
    {
        public int Row;
        public int Col;

        public CellPos(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public void Deconstruct(out int row, out int col)
        {
            row = Row;
            col = Col;
        }

        protected bool Equals(CellPos other)
        {
            return other != null && Row == other.Row && Col == other.Col;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CellPos) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Row * 397) ^ Col;
            }
        }

        public static bool operator ==(CellPos a, CellPos b) => a is null && b is null || !(a is null) && a.Equals(b);

        public static bool operator !=(CellPos a, CellPos b) => !(a == b);

        public int CompareTo(CellPos other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var rowComparison = Row.CompareTo(other.Row);
            if (rowComparison != 0) return rowComparison;
            return Col.CompareTo(other.Col);
        }

        public bool IsNeighbourOf(CellPos other)
        {
            return Math.Abs(this.Row - other.Row) <= 1 && Math.Abs(this.Col - other.Col) <= 1;
        }
        
        public static CellPos operator -(CellPos a, CellPos b) => new CellPos(a.Row - b.Row, a.Col - b.Col);

        /// <summary>
        /// Сross-product modulo of vectors representing two point's positions.
        /// Will be a positive number when second vector is rotated clockwise in relation to the first one.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Cross(CellPos a, CellPos b)
        {
            return a.Col * b.Row - a.Row * b.Col;
        }

        public override string ToString()
        {
            return $"({Row}, {Col})";
        }
    }
}