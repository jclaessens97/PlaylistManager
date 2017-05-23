using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistManager.BL
{
	public static class Extensions
	{
		private static Random rnd = new Random();

		public static void ShuffleAll<T>(this List<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				T tmp = list[i];
				T a = list[i + rnd.Next(list.Count - i)];

				list[i] = a;
				list[i + rnd.Next(list.Count - i)] = tmp;
			}
		}

		public static void ShuffleFromCurrent<T>(this List<T> list, T song)
		{
			int curIndex = list.IndexOf(song);

			T tmp = list[0];
			list[0] = list[curIndex];
			list[curIndex] = tmp;

			for (int i = 1; i < list.Count; i++)
			{
				tmp = list[i];
				T a = list[i + rnd.Next(list.Count - i)];

				list[i] = a;
				list[i + rnd.Next(list.Count - i)] = tmp;
			}
		}
	}
}
