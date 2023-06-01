using System;
using System.Collections.Generic;
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
            //Tuple<List<Line>, List<Line>> input = Generator.GeneratePuzzle(size);
            List<Line> rowNums = new()
            {
                new Line(1, 1),
                new Line(2),
                new Line(3),
                new Line(3),
                new Line(3)
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10)
            };
            List<Line> colNums = new()
            {
                new Line(1, 2),
                new Line(2),
                new Line(3),
                new Line(2),
                new Line(3),
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10),
                //new Line(10)
            };
            int size = rowNums.Count;
            Console.WriteLine($"size = {size}");
            Board board = new(size, rowNums, colNums);
            Solve(ref board);
        }

        public static void Solve(ref Board board)
        {
            // https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
            StablePriorityQueue<Cell> pqueue = new(board.Size * board.Size);
            for (int c = 0; c < board.Size; c++)
            {
                for (int r = 0; r < board.Size; r++)
                {
                    pqueue.Enqueue(board.Cells[r, c], board.CellPriorities[r, c]);
                }
            }
            //Console.WriteLine("---------- Start Logging ----------");
            //int runs = 0;
            while (pqueue.Count > 0)
            {
                Console.Write("\nPress enter to continue");
                Console.ReadLine();
                //++runs;
                Console.WriteLine($"Cells left: {pqueue.Count}");
                Cell curr = pqueue.Dequeue();
                Console.WriteLine($"Revising ({curr.Row}, {curr.Col})");
                Console.WriteLine($"Cell prio = {board.CellPriorities[curr.Row, curr.Col]}");
                if (Revise(ref board, ref curr))
                {
                    Console.WriteLine($"revised cell {curr.Row}, {curr.Col}");
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
            PrintBoard(board);
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
            int pos = 0;
            int block_ctr = 0;
            while (block_ctr < lineNums.Length)
            {
                if (pos >= line.Length)
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
                pos = AttemptToFitBlock(line, pos, lineNums[block_ctr]);
                if (pos == -1)
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
                block_ctr++;
            }
            return false;
        }

        public static int AttemptToFitBlock(string line, int pos, int block_size)
        {
            for (int i = 0; i < block_size && (pos + i) < line.Length && (pos + block_size) <= line.Length; i++)
            {
                if (i == 0)
                {
                    if (line[pos] == '-')
                    {
                        pos++;
                        i = -1;
                        continue;
                    }
                    if (pos > 0)
                    {
                        if (line[pos - 1] == '+')
                        {
                            pos++;
                            i = -1;
                            continue;
                        }
                    }
                    if (pos + block_size < line.Length)
                    {
                        if (line[pos + block_size] == '+')
                        {
                            pos++;
                            i = -1;
                            continue;
                        }
                    }
                }
                else if (line[pos + i] == '-')
                {
                    pos += i;
                    i = -1;
                    continue;
                }
            }
            if (pos + block_size > line.Length)
            {
                return -1;
            }
            // check to make sure all KNOWN filled cells are accounted for by the given block sizes in the row/col using windows
                // use regex

            if (bool)
            return pos + block_size;
        }

        public static void PrintBoard(Board board)
        {
            string[] res = board.BoardToStringRep();
            foreach (string line in res)
            {
                Console.WriteLine(line);
            }
        }
    }
}
