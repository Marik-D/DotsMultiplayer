using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public enum Player
    {
        Red,
        Blue
    }

    public struct CellState
    {
        public Player Player;
        public bool IsPlaced;
        public bool IsCaptured;
    }

    public struct Capture
    {
        public Player Player;
        public Cycle Points;
    }
    
    public class BoardState
    {
        public readonly int Cols;
        
        public readonly int Rows;

        private CellState[,] _state;

        public Player CurrentMove = Player.Red;
        
        public List<Capture> Captures = new List<Capture>();

        public BoardState(int rows, int cols)
        {
            this.Cols = cols;
            this.Rows = rows;
            this._state = new CellState[rows, cols];
        }

        public bool IsValidCell(int row, int col) => row >= 0 && row < Rows && col >= 0 && col < Cols;

        public CellState Get(int row, int col)
        {
            if (!IsValidCell(row, col))
            {
                throw new ArgumentOutOfRangeException();
            }

            return _state[row, col];
        }

        public CellState Get(CellPos pos) => Get(pos.Row, pos.Col);

        public bool CanPlace(int row, int col) => IsValidCell(row, col) && !Get(row, col).IsPlaced;

        public bool CanParticipateInCapture(CellPos pos, Player forPlayer)
        {
            var state = Get(pos);
            return state.IsPlaced && state.Player == forPlayer && !state.IsCaptured;
        }

        public void Place(int row, int col)
        {
            if (!CanPlace(row, col))
            {
                throw new ArgumentException();
            }

            _state[row, col].IsPlaced = true;
            _state[row, col].Player = CurrentMove;
            
            RecalculateCaptures(new CellPos(row, col), CurrentMove);
            
            CurrentMove = CurrentMove == Player.Red ? Player.Blue : Player.Red;
        }

        private void RecalculateCaptures(CellPos from, Player forPlayer)
        {
            var cycles = GetCycles(from, forPlayer);
            foreach (var cycle in cycles)
            {
                var captured = false;
                foreach (var inside in EnumeratePointsInCycle(cycle))
                {
                    if (Get(inside).IsPlaced && Get(inside).Player != forPlayer)
                    {
                        captured = true;
                        _state[inside.Row, inside.Col].IsCaptured = true;
                    }
                }
                
                if (captured)
                {
                    Captures.Add(new Capture
                    {
                        Player = forPlayer,
                        Points = cycle
                    });
                }
            }
        }

        private IEnumerable<CellPos> GetNeighbours(CellPos pos)
        {
            if (pos.Row > 0)
            {
                yield return new CellPos(pos.Row - 1, pos.Col);
            }
            if (pos.Row > 0 && pos.Col > 0)
            {
                yield return new CellPos(pos.Row - 1, pos.Col - 1);
            }
            if (pos.Col > 0)
            {
                yield return new CellPos(pos.Row, pos.Col - 1);
            }
            if (pos.Row < Rows - 1 && pos.Col > 0)
            {
                yield return new CellPos(pos.Row + 1, pos.Col - 1);
            }
            if (pos.Row < Rows - 1)
            {
                yield return new CellPos(pos.Row + 1, pos.Col);
            }
            if (pos.Row < Rows - 1 && pos.Col < Cols - 1)
            {
                yield return new CellPos(pos.Row + 1, pos.Col + 1);
            }
            if (pos.Col < Cols - 1)
            {
                yield return new CellPos(pos.Row, pos.Col + 1);
            }
            if (pos.Row > 0 && pos.Col < Cols - 1)
            {
                yield return new CellPos(pos.Row - 1, pos.Col + 1);
            }
        }
        
        private IEnumerable<CellPos> GetDirectNeighbours(CellPos pos)
        {
            if (pos.Row > 0)
            {
                yield return new CellPos(pos.Row - 1, pos.Col);
            }
            if (pos.Col > 0)
            {
                yield return new CellPos(pos.Row, pos.Col - 1);
            }
            if (pos.Row < Rows - 1)
            {
                yield return new CellPos(pos.Row + 1, pos.Col);
            }
            if (pos.Col < Cols - 1)
            {
                yield return new CellPos(pos.Row, pos.Col + 1);
            }
        }

        private IEnumerable<CellPos> GetActiveNeighbours(CellPos from, Player player)
        {
            foreach (var pos in GetNeighbours(from))
            {
                if (CanParticipateInCapture(pos, player))
                {
                    yield return pos;
                }
            }
        }

        private IEnumerable<Cycle> GetCycles(CellPos from, Player player)
        {
            var cycles = new HashSet<Cycle>();
            
            var stack = new Stack<CellPos>();
            stack.Push(from);

            void Rec()
            {
                foreach (var neighbour in GetActiveNeighbours(stack.Peek(), player))
                {
                    if (neighbour == from) // Found a cycle
                    {
                        if (stack.Count > 3) // Prevent short cycles
                        {
                            // TODO: check that cycle does not self-intersect
                            cycles.Add(new Cycle(new List<CellPos>(stack.ToArray())));
                        }    
                    } 
                    else if (!stack.Contains(neighbour) && stack.All(point => point == stack.Peek() || !point.IsNeighbourOf(neighbour))) // Prevent self-intersecting cycles
                    {
                        stack.Push(neighbour);
                        Rec();
                        stack.Pop();
                    }
                }
            }

            Rec();

            return cycles;
        }

        public CellPos GetPointInside(Cycle cycle)
        {
            foreach (var (prev, curr, next) in cycle.Triples())
            {
                foreach (var candidate in GetNeighbours(curr))
                {
                    if (CellPos.Cross(next, candidate) > 0 && CellPos.Cross(candidate, prev) > 0)
                    {
                        return candidate;
                    }
                }
            }

            return null;
        }

        public  IEnumerable<CellPos> EnumeratePointsInCycle(Cycle cycle)
        {
            var inside = GetPointInside(cycle);
            if (inside == null)
            {
                yield break;
            }
            
            var cyclesPlayer = Get(cycle.Points[0]).Player;
            var visited = new bool[Rows, Cols];
            var queue = new Queue<CellPos>();
            queue.Enqueue(inside);

            while (queue.Count > 0)
            {
                var next = queue.Dequeue();
                visited[next.Row, next.Col] = true;
                yield return next;
                
                foreach (var neighbour in GetDirectNeighbours(next))
                {
                    if (!visited[neighbour.Row, neighbour.Col] && !CanParticipateInCapture(neighbour, cyclesPlayer)) // Stop when reaching cycle's border. There should be a direct path to all points inside the cycle.
                    {
                        queue.Enqueue(neighbour);
                    }
                }
            }
        }
    }
}