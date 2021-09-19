using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeGrammarEngine
{
	static internal class Utilities
	{
		/// <summary>
		/// Generate all permutations of a consecutive sequence of positive integers
		/// </summary>
		/// <param name="start"> The first member of the sequence </param>
		/// <param name="end"> The last member of the sequence </param>
		/// <returns> All permutations of the sequence </returns>
		static internal List<List<int>> GenerateAllPermutations(int start, int end)
		{
			if (start < 0 || end < 0)
			{
				throw new ArgumentException("the arguments cannot be negative");
			}

			int small, large;

			if (start < end)
			{
				small = start;
				large = end;
			}
			else
			{
				small = end;
				large = start;
			}


			// terminal case: if there is only one member, output that member
			if (small == large)
			{
				return new List<List<int>> { new List<int> { small } };
			}

			// otherwise, get all permutations of all members other than the first one, 
			// then for each permutation in the result insert the first member at all possible locations, creating the new set of permutations
			var result = Utilities.GenerateAllPermutations(small + 1, large);
			var output = new List<List<int>>();
			foreach (List<int> permutation in result)
			{
				for (int i = 0; i < permutation.Count + 1; i++)
				{
					var newPermutation = new List<int>(permutation);
					newPermutation.Insert(i, small);
					output.Add(newPermutation);
				}
			}
			return output;
		}
	}
}
