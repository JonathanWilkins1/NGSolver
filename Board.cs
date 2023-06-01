using System;
using System.Collections;
using System.Collections.Generic;
using State = NGSolver.Cell.State;

namespace NGSolver
{
    public class Board
    {
        // 2d array of cells that make up the board
        public Cell[,] Cells { get; private set; }
        // array of priority values for each of the cells
        //   based on the Row and Col values of each cell
        public int[,] CellPriorities { get; set; }

        // Block size requirements for each row and column
        //      Row numbers are listed left to right
        //      Column numbers are listed top to bottom
        public List<Line> RowNums { get; private set; }
        public List<Line> ColNums { get; private set; }

        // Size of the board (total of size x size cells)
        public int Size { get; private set; }

        // Constructor - Initializes the board to the given size, sets all cells to empty, and sets
        //  the block size requirements for each row and column
        public Board(int size, List<Line> rowInput, List<Line> colInput)
        {
            Size = size;
            RowNums = rowInput;
            ColNums = colInput;
            Cells = new Cell[size, size];
            for (int c = 0; c < size; c++) {
                for (int r = 0; r < size; r++) {
                    Cells[r, c] = new Cell(r, c);
                }
            }
            SetUpCellPriorities();
        }

        private void SetUpCellPriorities()
        {
            CellPriorities = new int[Size, Size];
            int defaultPriority = Size * Size;
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    CellPriorities[r, c] = defaultPriority;
                }
            }
        }

        // Given a row index (row) and a column index (col), this method returns true if the cell
        //  at position [row, column] is set to unknown
        public bool IsUnknown(int row, int col) => Cells[row, col].CurrentState == State.Unknown;

        // Given a row index (row) and a column index (col), this method returns true if the cell
        //  at position [row, column] is set to off
        public bool IsOff(int row, int col) => Cells[row, col].CurrentState == State.Off;

        // Given a row index (row) and a column index (col), this method returns true if the cell
        //  at position [row, column] is set to on
        public bool IsOn(int row, int col) => Cells[row, col].CurrentState == State.On;

        // Given a row index (row) and a column index (col), this method resets the cell at position
        //  [row, column] to unknown
        public void SetUnknown(int row, int col) => Cells[row, col].Reset();

        // Given a row index (row) and a column index (col), this method sets the cell at position
        //  [row, column] to off
        public void SetOff(int row, int col) => Cells[row, col].Off();

        // Given a row index (row) and a column index (col), this method sets the cell at position
        //  [row, column] to on
        public void SetOn(int row, int col) => Cells[row, col].On();

        // Given a row index (row) and a column index (col), this method sets the priority of the
        //  cell at [row, column] back to the default value
        public int ResetPriority(int row, int col) => CellPriorities[row, col] = Size * Size;

        // Given a line number index (lineNum) and a line type (lineType, row = true; col = false),
        //  this method generates a string representation of the current states of the requested
        //  row or column
        public string LineToStringRep(int lineNum, bool lineType)
        {
            string strRep = "";
            for (int i = 0; i < Size; i++)
            {
                if (lineType)
                {
                    if (IsUnknown(lineNum, i)) strRep += "?";
                    else if (IsOff(lineNum, i)) strRep += "-";
                    else strRep += "+";
                }
                else
                {
                    if (IsUnknown(i, lineNum)) strRep += "?";
                    else if (IsOff(i, lineNum)) strRep += "-";
                    else strRep += "+";
                }
            }
            return strRep;
        }

        public string[] BoardToStringRep()
        {
            string[] strRep = new string[Size];
            for (int i = 0; i < Size; i++)
            {
                strRep[i] = LineToStringRep(i, true);
            }
            return strRep;
        }
    }
}