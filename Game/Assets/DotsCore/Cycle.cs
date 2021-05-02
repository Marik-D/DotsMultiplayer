using System.Collections.Generic;

namespace DotsCore
{
    /// <summary>
    /// A series of points forming a closed cycle. Cycles are normalized normalized to be looping in a clockwise direction and points are sorted.  
    /// </summary>
    public class Cycle
    {
        public List<CellPos> Points;

        public Cycle(List<CellPos> points)
        {
            Points = points;
        }

        public bool IsSelfIntersecting()
        {
            foreach (var (prev, curr, next) in Triples())
            {
                foreach (var other in Points)
                {
                    if (other != prev && other != curr && other != next && other.IsNeighbourOf(curr))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public CellPos GetWrapping(int index) => Points[(index + Points.Count) % Points.Count];

        public IEnumerable<(CellPos, CellPos, CellPos)> Triples()
        {
            for (int i = 0; i < Points.Count; i++)
            {
                var curr = Points[i];
                var next = GetWrapping(i + 1);
                var prev = GetWrapping(i - 1);

                yield return (prev, curr, next);
            }
        }

        public void Normalize()
        {
            int sum = 0;
            foreach (var (prev, curr, next) in Triples())
            {
                var deltaPrev = curr - prev;
                var deltaNext = next - curr;
                
                sum += CellPos.Cross(deltaPrev, deltaNext);
            }

            if (sum > 0) // Cycle is counter-clockwise
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
            int code = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                code ^= (i * 1083) ^ Points[i].GetHashCode();
            }

            return code;
        }
    }
}