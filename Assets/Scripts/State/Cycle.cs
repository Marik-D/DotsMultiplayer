using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DefaultNamespace
{
    public class Cycle
    {
        public List<CellPos> Points;

        public Cycle(List<CellPos> points)
        {
            Points = points;
            
            Normalize();
        }

        public void Normalize()
        {
            int sum = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                var next = i == Points.Count - 1 ? 0 : i + 1;
                sum += Points[i].Col * Points[next].Row - Points[i].Row * Points[next].Col;
            }

            if (sum < 0) // Cycle is counter-clockwise
            {
                Points.Reverse();
            }

            int minIdx = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].CompareTo(Points[minIdx]) < 0)
                {
                    minIdx = i;
                }
            }

            if (minIdx != 0)
            {
                var newPoints = new List<CellPos>(Points.Count);
                for (int i = 0; i < Points.Count; i++)
                {
                    newPoints.Add(Points[(i + minIdx) % Points.Count]);
                }

                Points = newPoints;
            }
        }

        protected bool Equals(Cycle other)
        {
            if (Points.Count != other.Points.Count)
            {
                return false;
            }

            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i] != other.Points[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Cycle) obj);
        }

        public override int GetHashCode()
        {
            return (Points != null ? Points.GetHashCode() : 0);
        }
    }
}