using System;
using NAudio.Wave;

namespace PlaylistManager.Model.Extensions
{
	/// <summary>
	///     Extension methods to navigate trough a song in a proper way (extension on the AudioFileReader object in
	///     AudioPlayer)
	/// </summary>
	public static class WaveStreamExtensions
	{
		public static void SetPosition(this WaveStream _stream, long _position)
		{
			var adj = _position % _stream.WaveFormat.BlockAlign;
			var newPos = Math.Max(0, Math.Min(_stream.Length, _position - adj));

			_stream.Position = newPos;
		}

		public static void SetPosition(this WaveStream _stream, double _seconds)
		{
			_stream.SetPosition((long) _seconds * _stream.WaveFormat.AverageBytesPerSecond);
		}

		public static void SetPosition(this WaveStream _stream, TimeSpan _time)
		{
			_stream.SetPosition(_time.TotalSeconds);
		}

		public static void Seek(this WaveStream _stream, double _offset)
		{
			_stream.SetPosition(_stream.Position + (long) (_offset * _stream.WaveFormat.AverageBytesPerSecond));
		}
	}
}