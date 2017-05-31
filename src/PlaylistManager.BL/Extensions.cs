using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaylistManager.Domain;

namespace PlaylistManager.BL
{
	public static class Extensions
	{
		private static readonly Random rnd = new Random();

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

		public static int CompareTo(this uint? nVal1, uint? nVal2)
		{
			uint val1 = nVal1 ?? 0;
			uint val2 = nVal2 ?? 0;
			return val1.CompareTo(val2);
		}
	}
}
