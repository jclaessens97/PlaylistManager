using System;
using System.Threading;


namespace PlaylistManager.Model.Other
{
	public class TimeBetweenSongChecker
	{
		private int invokeCount;
		private readonly int maxCount = 1;

		public TimeBetweenSongChecker()
		{
			invokeCount = 0;
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
