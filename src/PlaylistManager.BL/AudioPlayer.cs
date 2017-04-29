using System;
using NAudio.Wave;
using PlaylistManager.Domain;

namespace PlaylistManager.BL
{
	public class AudioPlayer
	{
		private IWavePlayer _wavePlayer;
		private AudioFileReader _audioFileReader;

		public Song CurrentSong { get; set; }

		public AudioPlayer()
		{
			_wavePlayer = new WaveOut();
		}

		public void Play()
		{
			if (CurrentSong != null)
			{
				_audioFileReader = new AudioFileReader(CurrentSong.Path);
				_wavePlayer.Init(_audioFileReader);
				_wavePlayer.Play();
			}
		}

		public void Resume()
		{
			_wavePlayer.Play();
		}

		public void Pause()
		{
			_wavePlayer.Pause();
		}

		public void Next()
		{
			throw new NotImplementedException();
		}

		public void Prev()
		{
			throw new NotImplementedException();
		}

		public void Stop()
		{
			if (_wavePlayer != null)
			{
				_wavePlayer.Stop();
			}
			
			if (_audioFileReader != null)
			{
				_audioFileReader.Dispose();
				_audioFileReader = null;
			}

			if (_wavePlayer != null)
			{
				_wavePlayer.Dispose();
				_wavePlayer = null;
			}
		}
	}
}
