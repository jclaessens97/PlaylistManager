using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlaylistManager.Model.Other
{
	public class TimeBetweenSongChecker
	{
		private int invokeCount;
		private readonly int maxCount;

		public TimeBetweenSongChecker(int _count)
		{
			invokeCount = 0;
			maxCount = _count;
		}

		public void CheckTime(Object _stateInfo)
		{
			AutoResetEvent autoEvent = (AutoResetEvent) _stateInfo;
			invokeCount++;
			if (invokeCount == maxCount)
			{
				invokeCount = 0;
				autoEvent.Set();
			}
		}
	}
}
