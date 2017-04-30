using System;
using NAudio.Wave;

namespace PlaylistManager.BL
{
	public static class WaveStreamExtensions
	{
		public static void SetPosition(this WaveStream stream, long position)
		{
			long adj = position % stream.WaveFormat.BlockAlign;
			long newPos = Math.Max(0, Math.Min(stream.Length, position - adj));

			stream.Position = newPos;
		}

		public static void SetPosition(this WaveStream stream, double seconds)
		{
			stream.SetPosition((long)seconds * stream.WaveFormat.AverageBytesPerSecond);
		}

		public static void SetPosition(this WaveStream stream, TimeSpan time)
		{
			stream.SetPosition(time.TotalSeconds);
		}

		public static void Seek(this WaveStream stream, double offset)
		{
			stream.SetPosition(stream.Position + (long)(offset * stream.WaveFormat.AverageBytesPerSecond));
		}
	}
}
