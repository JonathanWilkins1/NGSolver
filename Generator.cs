using System;
using System.Collections.Generic;

namespace NGSolver
{
    public class Generator
    {
        public static Tuple<List<Line>, List<Line>> GeneratePuzzle(int size)
        {
            double median = 0.5;
            median -= size / 81;
            // https://github.com/beta-decay/beta-decay.github.io/blob/master/nonogram/game.js
            Cell[,] board = new Cell[size, size];
            Random rand = new();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    board[i, j] = new(i, j);
                    if (rand.NextDouble() > median)
                    {
                        board[i, j].On();
                    }
                    else
                    {
                        board[i, j].Off();
                    }
                }
            }

            return new Tuple<List<Line>, List<Line>>(
                GenerateNums(board, size, lineType: true),
                GenerateNums(board, size, lineType: false)
            );
        }

        private static List<Line> GenerateNums(Cell[,] board, int size, bool lineType)
        {
            List<Line> nums = new();
            int consec = 0;
            for (int i = 0; i < size; i++)
            {
                Line line = new();
                for (int j = 0; j < size; j++)
                {
                    if ((lineType ? board[i, j].CurrentState : board[j, i].CurrentState) == Cell.State.On)
                    {
                        consec++;
                    }
                    else if (consec > 0)
                    {
                        line.Add(consec);
                        consec = 0;
                    }
                }
                if (consec != 0)
                {
                    line.Add(consec);
                    consec = 0;
                }
                nums.Add(line);
            }
            

            return nums;
        }
    }
}