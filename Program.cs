using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Priority_Queue;

namespace NGSolver
{
    public class Program
    {
        // Main - Initializes a new board from the sample row and column requirements fields
        //  (rowInput and colInput), calls a method to set obvious states of cells, and calls the
        //  solver with a starting index position of [0, 0]
        public static void Main()
        {
            List<Line> rowNumss = new()
            {
                new Line(1),
                new Line(3),
                new Line(1)
            };
            List<Line> colNumss = new()
            {
                new Line(1),
                new Line(3),
                new Line(1)
            };



            //string[] lines = System.IO.File.ReadAllLines("/Users/jwilkins/Documents/Millersville/Other/Thesis/puzzles/25x25_puzzles.txt");
            int puzzlesSolved = 0;
            Stopwatch timer = new();
            int size = 3;
            Board boardd = new(rowNumss.Count, rowNumss, colNumss);
            Boolean result = Solve(ref boardd);
            PrintBoard(boardd);

            //Tuple<List<Line>, List<Line>> input = Generator.GeneratePuzzle(size);
            //rowNums = input.Item1;
            //colNums = input.Item2;
            //List<Line> rowNums = new();
            //List<Line> colNums = new();
            //int currentLine = 0;
            //while (!lines[currentLine].Equals("end"))
            //{
            //    if (lines[currentLine].Equals("Row numbers:"))
            //    {
            //        timer.Reset();
            //        currentLine++;
            //        for (int i = 0; i < size; i++)
            //        {
            //            rowNums.Add(new Line(Array.ConvertAll(lines[currentLine].Split(","), s => int.Parse(s))));
            //            currentLine++;
            //        }
            //        currentLine++;
            //        for (int i = 0; i < size; i++)
            //        {
            //            colNums.Add(new Line(Array.ConvertAll(lines[currentLine].Split(","), s => int.Parse(s))));
            //            currentLine++;
            //        }

            //        Board board = new(size, rowNums, colNums);
            //        //PrintNums(rowNums, colNums);
            //        timer.Start();
            //        Boolean res = Solve(ref board);
            //        if (!res)
            //        {
            //            //Console.WriteLine("--- Failed to solve ---");
            //            //PrintNums(rowNums, colNums);
            //            //PrintBoard(board);
            //        }
            //        else if ((++puzzlesSolved) == 100)
            //        {
            //            Console.WriteLine("100 puzzles solved");
            //            return;
            //        }
            //        timer.Stop();
            //        Console.WriteLine(timer.Elapsed.TotalSeconds);
            //        rowNums.Clear();
            //        colNums.Clear();
            //    }
            //    currentLine++;
            //}
            //Console.WriteLine($"EOF - {puzzlesSolved} puzzles solved");
        }

        public static Boolean Solve(ref Board board)
        {
            // https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
            StablePriorityQueue<Cell> pqueue = new(board.Size * board.Size);
            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    pqueue.Enqueue(board.Cells[r, c], board.CellPriorities[r, c]);
                }
            }

            int lastUnsolvedCount = 0;
            int consecSolveFailAttempts = 0;
            while (pqueue.Count > 0)
            {
                if (lastUnsolvedCount == pqueue.Count)
                {
                    lastUnsolvedCount = pqueue.Count;
                    consecSolveFailAttempts = 0;
                }
                else
                {
                    if (consecSolveFailAttempts == Math.Pow(board.Size, 3))
                    {
                        //Console.WriteLine($"\nCells left: {pqueue.Count}");
                        return false;
                    }
                    consecSolveFailAttempts++;
                }
                PrintBoard(board);
                //Console.WriteLine($"Cells left: {pqueue.Count}");
                Cell curr = pqueue.Dequeue();
                Console.WriteLine($"Revising ({curr.Row}, {curr.Col})");
                Console.WriteLine($"Cell prio = {board.CellPriorities[curr.Row, curr.Col]}\n");
                if (Revise(ref board, ref curr))
                {
                    Console.WriteLine($"revised cell {curr.Row}, {curr.Col}\n");
                    // incr_prio for row
                    for (int c = 0; c < board.Size; c++)
                    {
                        if (pqueue.Contains(board.Cells[curr.Row, c]))
                        {
                            pqueue.UpdatePriority(board.Cells[curr.Row, c], --board.CellPriorities[curr.Row, c]);
                        }
                    }
                    // incr_prio for col
                    for (int r = 0; r < board.Size; r++)
                    {
                        if (pqueue.Contains(board.Cells[r, curr.Col]))
                        {
                            pqueue.UpdatePriority(board.Cells[r, curr.Col], --board.CellPriorities[r, curr.Col]);
                        }
                    }
                }
                else
                {
                    board.ResetPriority(curr.Row, curr.Col);
                    pqueue.Enqueue(curr, ++board.CellPriorities[curr.Row, curr.Col]);
                }
            }
            return true;
        }

        public static bool Revise(ref Board board, ref Cell cell)
        {
            string line = board.LineToStringRep(cell.Row, lineType: true);
            line = line.Substring(0, cell.Col) + "+" + line[(cell.Col + 1)..];
            if (TestLine(line, board.RowNums[cell.Row], ref cell, assumed_state: true))
            {
                return true;
            }
             
            line = line.Substring(0, cell.Col) + "-" + line[(cell.Col + 1)..];
            if (TestLine(line, board.RowNums[cell.Row], ref cell, assumed_state: false))
            {
                return true;
            }

            line = board.LineToStringRep(cell.Col, lineType: false);
            line = line.Substring(0, cell.Row) + "+" + line[(cell.Row + 1)..];
            if (TestLine(line, board.ColNums[cell.Col], ref cell, assumed_state: true))
            {
                return true;
            }

            line = line.Substring(0, cell.Row) + "-" + line[(cell.Row + 1)..];
            if (TestLine(line, board.ColNums[cell.Col], ref cell, assumed_state: false))
            {
                return true;
            }
            return false;
        }

        public static bool TestLine(string line, Line lineNums, ref Cell cell, bool assumed_state)
        {
            string pattern = "^[-?]*";
            for (int i = 0; i < lineNums.Length; i++)
            {
                pattern += "[+?]{" + lineNums[i] + "}";
                pattern += (i + 1 != lineNums.Length) ? "[-?]+" : "[-?]*";
            }
            pattern += "$";
            Regex rgx = new(pattern);
            if (rgx.Matches(line).Count == 0)
            {
                if (assumed_state)
                {
                    cell.Off();
                }
                else
                {
                    cell.On();
                }
                return true;
            }
            return false;
        }

        public static void PrintBoard(Board board)
        {
            string[] res = board.BoardToStringRep();
            foreach (string line in res)
            {
                Console.WriteLine(line);
            }
        }

        public static void PrintNums(List<Line> rowNums, List<Line> colNums)
        {
            Console.WriteLine("Row numbers:");
            for (int i = 0; i < rowNums.Count; i++)
            {
                Console.WriteLine(rowNums[i].ToString());
            }
            Console.WriteLine("Column numbers:");
            for (int i = 0; i < colNums.Count; i++)
            {
                Console.WriteLine(colNums[i].ToString());
            }
            Console.WriteLine();
        }
    }
}
