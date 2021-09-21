using ListOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeGrammarEngine
{
	static internal class Utilities
	{
		/// <summary>
		/// Generate all unique permutations of a list
		/// </summary>
		/// <returns> All unique permutations </returns>
		static internal List<List<T>> GenerateAllPermutations<T>(List<T> listToPermutate) where T : IEquatable<T>
		{
			if (listToPermutate.Count == 0)
			{
				throw new ArgumentException("listToPermutate must have at least one entry");
			}

			List < List < T >> helper(int beginningIndex, int endIndex)
			{
				// terminal case: if there is only one member, output that member
				if (beginningIndex == endIndex)
				{
					return new List<List<T>> { new List<T> { listToPermutate[beginningIndex] } };
				}

				// otherwise, get all permutations of all members other than the first one, 
				// then for each permutation in the result insert the first member at all possible locations, creating the new set of permutations
				var result = helper(beginningIndex + 1, endIndex);
				var output = new List<List<T>>();
				foreach (List<T> permutation in result)
				{
					for (int i = 0; i < permutation.Count + 1; i++)
					{
						var newPermutation = new List<T>(permutation);
						newPermutation.Insert(i, listToPermutate[beginningIndex]);
						output.Add(newPermutation);
					}
				}
				return output;
			}

			var permutations = helper(0, listToPermutate.Count - 1);
			var duplicateIndexes = new List<int>();
			for (int i = 0; i < permutations.Count; i++)
			{
				for (int j = i+1; j < permutations.Count; j++)
				{
					if (ListUtilities.AreContentsEqualInOrder(permutations[i], permutations[j]))
					{
						duplicateIndexes.Add(j);
					}
				}
			}

			duplicateIndexes.Sort();
			duplicateIndexes.Reverse();
			foreach (int indexToRemove in duplicateIndexes)
			{
				permutations.RemoveAt(indexToRemove);
			}

			return permutations;
		}
	}
}
