using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
	class TestUtilities
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
	}
}
