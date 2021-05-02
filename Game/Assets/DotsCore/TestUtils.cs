using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DotsCore
{
    public static class TestUtils
    {
        public static string StringifyGrid(List<CellPos> points)
        {
            var rows = points.Max(p => p.Row) + 1;
            var cols = points.Max(p => p.Col) + 1;

            var data = new int?[rows, cols];
            
            for (var i = 0; i < points.Count; i++)
            {
                var (row, col) = points[i];
                data[row, col] = i;
            }

            var buffer = "";
            for (int i = rows - 1; i >= 0; i--)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (data[i, j].HasValue)
                    {
                        buffer += data[i, j].ToString().PadLeft(2) + " ";
                    }
                    else
                    {
                        buffer += " . ";
                    }
                }

                buffer += "\n";
            }

            return buffer;
        }

        public static List<CellPos> ParseGrid(string str)
        {
            var cells = str
                .Split('\n')
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .Select(row => row
                    .Split(' ')
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0)
                    .Select(s => s == "." ? (int?) null : Int32.Parse(s))
                    .ToArray()
                )
                .ToArray();

            var rows = cells.Length;
            var cols = cells.Max(row => row.Length);
            
            var points = new List<CellPos>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (cells[i][j].HasValue)
                    {
                        var pos = new CellPos(rows - i - 1, j);
                        if (cells[i][j] >= points.Count)
                        {
                            points.AddRange(Enumerable.Repeat<CellPos>(null,(int) (cells[i][j] - points.Count)));
                            points.Add(pos);
                        }
                        else
                        {
                            points[(int) cells[i][j]] = pos;
                        }
                    }
                }
            }

            if (points.Any(p => p == null))
            {
                throw new ArgumentException("Invalid board.");
            }

            return points;
        }

        public static BoardState ParseBoardState(string str)
        {
            var cells = str
                .Split('\n')
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .Select(row => row
                    .Split(' ')
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0)
                    .ToArray()
                )
                .ToArray();
            
            var rows = cells.Length;
            var cols = cells.Max(row => row.Length);

            var state = new BoardState(rows, cols);
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    switch (cells[i][j])
                    {
                        case "R":
                            state.PlaceByPlayer(new CellPos(rows - i - 1, j), Player.Red);
                            break;
                        case "B":
                            state.PlaceByPlayer(new CellPos(rows - i - 1, j), Player.Blue);
                            break;
                        case ".":
                            break;
                        default:
                            throw new ArgumentException("Invalid board.");
                    }
                }
            }

            return state;
        }
    }
}