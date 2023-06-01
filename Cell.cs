using System;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;

namespace NGSolver
{
    public class Cell : StablePriorityQueueNode
    {
        // Enumeration of possible states for a cell
        public enum State
        {
            Unknown,
            Off,
            On
        }

        public int Row { get; private set; }
        public int Col { get; private set; }
        public State CurrentState { get; private set; }

        // Constructor - Initializes the list of required block sizes
        public Cell(int r, int c)
        {
            Row = r;
            Col = c;
            CurrentState = State.Unknown;
        }

        public void On()
        {
            CurrentState = State.On;
        }

        public void Off()
        {
            CurrentState = State.Off;
        }

        public void Reset()
        {
            CurrentState = State.Unknown;
        }
    }
}