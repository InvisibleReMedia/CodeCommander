using System;
using System.Collections.Generic;
using System.Text;

namespace o2Mate
{
    public class UniqueStrings
    {
        #region Private Constants
        private const string list = "abcdefghijklmnopqrstuvwxyABCDEFGHIJKLMNOPQRSTUVWXY0123456789";
        private const int maxDepth = 3;
        #endregion

        #region Private Fields
        private int counter;
        #endregion

        #region Default Constructor
        public UniqueStrings()
        {
            this.counter = 0;
        }
        #endregion

        #region Public Methods
        public string ComputeNewString()
        {
            int max = (int)Math.Pow(UniqueStrings.list.Length, UniqueStrings.maxDepth);
            if (this.counter < max)
            {
                int[] seq = new int[UniqueStrings.maxDepth];
                seq[0] = this.counter;
                ++this.counter;
                for (int b = UniqueStrings.maxDepth - 1; b > 0; --b)
                {
                    int q = (int)Math.Pow(UniqueStrings.list.Length, b);
                    int temp = seq[UniqueStrings.maxDepth - b - 1];
                    seq[UniqueStrings.maxDepth - b - 1] = temp / q;
                    seq[UniqueStrings.maxDepth - b] = temp - seq[UniqueStrings.maxDepth - b - 1] * q;
                }
                string output = String.Empty;
                for (int index = 0; index < UniqueStrings.maxDepth; ++index)
                {
                    output += UniqueStrings.list[seq[index]];
                }
                return output;
            }
            else
            {
                throw new OverflowException("Nombre maximum de processus anonyme atteint (" + max.ToString() + ")");
            }
        }
        #endregion
    }
}
