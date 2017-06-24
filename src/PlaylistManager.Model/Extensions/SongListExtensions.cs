using System;
using System.Collections.Generic;

namespace PlaylistManager.Model.Extensions
{
	public static class SongListExtensions
	{
		private static readonly Random rnd = new Random();

		public static void ShuffleAll<T>(this List<T> _list)
		{
			for (int i = 0; i < _list.Count; i++)
			{
				T tmp = _list[i];
				T a = _list[i + rnd.Next(_list.Count - i)];

				_list[i] = a;
				_list[i + rnd.Next(_list.Count - i)] = tmp;
			}
		}

		public static void ShuffleFromCurrent<T>(this List<T> _list, T _song)
		{
			int curIndex = _list.IndexOf(_song);

			T tmp = _list[0];
			_list[0] = _list[curIndex];
			_list[curIndex] = tmp;

			for (int i = 1; i < _list.Count; i++)
			{
				tmp = _list[i];
				T a = _list[i + rnd.Next(_list.Count - i)];

				_list[i] = a;
				_list[i + rnd.Next(_list.Count - i)] = tmp;
			}
		}

		public static int CompareTo(this uint? _nVal1, uint? _nVal2)
		{
			uint val1 = _nVal1 ?? 0;
			uint val2 = _nVal2 ?? 0;
			return val1.CompareTo(val2);
		}
	}
}
