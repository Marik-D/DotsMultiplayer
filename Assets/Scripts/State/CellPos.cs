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

        protected bool Equals(CellPos other)
        {
            return Row == other.Row && Col == other.Col;
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

        public static bool operator ==(CellPos a, CellPos b) => !(a is null) && a.Equals(b);

        public static bool operator !=(CellPos a, CellPos b) => !(a is null) && !a.Equals(b);

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
            return Math.Abs(this.Row - other.Row) < 1 && Math.Abs(this.Col - other.Col) < 1;
        }
    }
}