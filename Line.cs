using System.Collections.Generic;
using System.Linq;
using System;

namespace NGSolver
{
    public class Line
    {
        // List of the required block sizes for this line (row or column)
        private List<int> Nums { get; set; }
        public int Length { get; private set; }

        // Constructor - Initializes the list of required block sizes
        public Line()
        {
            Nums = new List<int>();
            Length = 0;
        }

        public Line(params int[] arr)
        {
            Nums = arr.ToList();
            Length = Nums.Count;
        }

        // Adds the given number (num) to the list of required block sizes
        public void Add(int num)
        {
            Nums.Add(num);
            Length++;
        }

        public int this[int i]
        {
            get => Nums[i];
        }

        public override string ToString()
        {
            string res = "";
            for (int i = 0; i < Length; i++)
            {
                res += Nums[i];
                if (i < Length - 1)
                {
                    res += ", ";
                }
            }
            return res;
        }
    }
}