using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticsLab
{
    static class PairWiseAlign
    {
        private static int MATCH_COST = -3;
        private static int SUBSTITUTION_COST = 1;
        private static int INDEL_COST = 5;
        /// <summary>
        /// this is the function you implement.
        /// </summary>
        /// <param name="sequenceA">the first sequence</param>
        /// <param name="sequenceB">the second sequence, may have length not equal to the length of the first seq.</param>
        /// <param name="banded">true if alignment should be band limited.</param>
        /// <returns>the alignment score and the alignment (in a Result object) for sequenceA and sequenceB.  The calling function places the result in the dispay appropriately.
        /// 
        public static ResultTable.Result Align_And_Extract(int alignLen, GeneSequence sequenceA, GeneSequence sequenceB, bool banded)
        {
            ResultTable.Result result = new ResultTable.Result();
            int score = computeOptimalAlignment(alignLen, sequenceA, sequenceB);
            string[] alignment = new string[2];                              // place your two computed alignments here


            // ********* these are placeholder assignments that you'll replace with your code  *******
            score = 0;                                                
            alignment[0] = "";
            alignment[1] = "";
            // ***************************************************************************************
            

            result.Update(score,alignment[0],alignment[1]);                  // bundling your results into the right object type 
            return(result);
        }

        public static int computeOptimalAlignment(int maxAlignLength, GeneSequence a, GeneSequence b)
        {
            // Should only compare at most maxAlignLength chars
            int m = Math.Min(a.getLength() + 1, maxAlignLength);
            int n = Math.Min(b.getLength() + 1, maxAlignLength);

            int[,] computedWeight = new int[m,n];
            Tuple<int, int>[,] parent = new Tuple<int, int>[m, n];
            parent[0, 0] = new Tuple<int, int>(-1, -1);

            // Initialize the first column
            for (int i = 1; i < m; i++)
            {
                computedWeight[i, 0] = i * INDEL_COST;
                parent[i, 0] = new Tuple<int, int>(i - 1, 0);
            }
            // Initialize the first row
            for (int j = 1; j < n; j++)
            {
                computedWeight[0, j] = j * INDEL_COST;
                parent[0, j] = new Tuple<int, int>(0, j - 1);
            }

            for(int i = 1; i < m; i++)
            {
                for(int j = 1; j < n; j++)
                {
                    // Get what the maximum costs of coming from
                    // the up, diagonal, and left
                    // Up or left will always be an insertion or deletion
                    int up = getInDel(computedWeight, i - 1, j);
                    int diagonal = getProposedDiagonalCost(a, b, computedWeight, i, j);
                    int left = getInDel(computedWeight, i, j - 1);

                    Tuple<int, Tuple<int, int>> minValAndParents = determineParent(i, j, up, diagonal, left);

                    computedWeight[i, j] = minValAndParents.Item1;
                    parent[i, j] = minValAndParents.Item2;
                }
            }

            return computedWeight[m - 1, n - 1];
        }

        private static int getProposedDiagonalCost(GeneSequence a, GeneSequence b, int[,] computedWeight, int i, int j)
        {
            int diagonal = computedWeight[i - 1, j - 1];
            char aLetter = a.getCharAt(i - 1);
            char bLetter = b.getCharAt(j - 1);

            if (aLetter.Equals(bLetter))
                return diagonal + MATCH_COST;
            else
                return diagonal + SUBSTITUTION_COST;
        }

        private static int getInDel(int[,] computedWeight, int k, int l)
        {
            return computedWeight[k, l] + INDEL_COST;
        }

        private static Tuple<int, Tuple<int,int>> determineParent(int i, int j, int up, int diagonal, int left)
        {
            int item1 = -1;
            int parentI = -1;
            int parentJ = -1;

            // Up is less than both
            if (up < diagonal && up < left) {
                item1 = up;
                parentI = i - 1;
                parentJ = j;
            }
            // Diagonal is less than both
            else if (diagonal < up && diagonal < left)
            {
                item1 = diagonal;
                parentI = i - 1;
                parentJ = i - 1;
            }
            // Left is less than both
            else if (left < up && left < diagonal)
            {
                item1 = left;
                parentI = i;
                parentJ = j - 1;
            }
            // All three are the same value
            // Return one of the three
            else if(up == diagonal && up == left && diagonal == left)
            {
                item1 = up;
                parentI = i - 1;
                parentJ = j;
            }
            // Up and Diagonal are tied for lowest
            // Return one of the two
            else if(up == diagonal)
            {
                item1 = diagonal;
                parentI = i - 1;
                parentJ = j - 1;
            }
            // Up and Left are tied for lowest
            else if(up == left)
            {
                item1 = up;
                parentI = i - 1;
                parentJ = j;
            }
            // Diagonal and Left are the same
            else if(diagonal == left)
            {
                item1 = diagonal;
                parentI = i - 1;
                parentJ = j - 1;
            }
            // This case should never happen
            else
            {
                throw new SystemException("This case should never happen: up " + up + " diagonal: " + diagonal + " left: " + left);
            }

            Tuple<int, int> item2 = new Tuple<int, int>(parentI, parentJ);
            return new Tuple<int, Tuple<int, int>>(item1, item2);
        }

        private static void print(int[,] array, int m, int n)
        {
            for(int i = 0; i < m; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    Console.Write(array[i, j]);
                    Console.Write(",");
                }
                Console.Write("\n");
            }
        }
    }
}
