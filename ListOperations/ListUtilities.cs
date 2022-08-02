using System;
using System.Collections.Generic;

namespace ListOperations
{
	static public class ListUtilities
	{
		/// <summary>
		/// Whether the container list's sublists contain the item
		/// </summary>
		public static bool DoesContainItem<T>(List<List<T>> containerList, T item) where T : IEquatable<T>
		{
			foreach (List<T> sl in containerList)
			{
				if (sl.Contains(item))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Whether the container list contains a list with equal content in the same order as the one provied
		/// </summary>
		public static bool DoesContainList<T>(List<List<T>> containerList, List<T> containedList) where T : IEquatable<T>
		{
			foreach (List<T> l in containerList)
			{
				if (ListUtilities.AreContentsEqualInOrder(l, containedList))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// whether the two lists contain exactly the same items, regardless of the order
		/// </summary>
		public static bool AreContentsEqual<T>(List<T> list1, List<T> list2) where T : IEquatable<T>
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
		/// whether the two lists contain exaclty identical sublists, in the same order
		/// </summary>
		public static bool AreContentsEqualInOrder<T>(List<List<T>> list1, List<List<T>> list2) where T : IEquatable<T>
		{
			if (list1.Count != list2.Count)
			{
				return false;
			}

			for (int i = 0; i < list1.Count; i++)
			{
				if (!ListUtilities.AreContentsEqualInOrder(list1[i], list2[i]))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// whether the two lists contain exaclty the same items, in the same order
		/// </summary>
		public static bool AreContentsEqualInOrder<T>(List<T> list1, List<T> list2) where T : IEquatable<T>
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
