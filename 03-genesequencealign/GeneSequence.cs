using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticsLab
{
    class GeneSequence
    {
        private string name;
        private char[] sequence;

        public GeneSequence(string name, string sequence)
        {
            this.name = name;
            this.sequence = sequence.ToCharArray();
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public char[] Sequence
        {
            get
            {
                return sequence;
            }
        }

        public int getLength()
        {
            return sequence.Length;
        }

        public char getCharAt(int index)
        {
            return sequence[index];
        }
    }
}
