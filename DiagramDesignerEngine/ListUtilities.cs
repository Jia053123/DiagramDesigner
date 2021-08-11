using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
	static class ListUtilities
	{
		/// <summary>
		/// whether the two lists contain exactly the same items, regardless of the order
		/// </summary>
		internal static bool AreContentsEqual<T>(List<T> list1, List<T> list2) where T : IEquatable<T>
		{
			if (list1.Count != list2.Count)
			{
				return false;
			}

			bool allItemsFound = true;

			foreach (T item1 in list1)
			{
				bool found = false;
				foreach (T item2 in list2)
				{
					if (item1.Equals(item2))
					{
						found = true;
					}
				}
				allItemsFound = allItemsFound & found;
			}

			return allItemsFound;
		}

		/// <summary>
		/// whether the two lists contain exaclty the same items, in the same order
		/// </summary>
		internal static bool AreContentsEqualInOrder<T>(List<T> list1, List<T> list2) where T : IEquatable<T>
		{
			if (list1.Count != list2.Count)
			{
				return false;
			}

			for (int i = 0; i < list1.Count; i++)
			{
				if (! list1[i].Equals(list2[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
