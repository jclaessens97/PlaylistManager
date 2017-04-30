using System;
using System.ComponentModel;
using NAudio.Wave;
using PlaylistManager.Domain;

namespace PlaylistManager.BL
{
	class AudioPlayer
	{
		private IWavePlayer _wavePlayer;
		private AudioFileReader _audioFileReader;

		private bool muted = false;
		private float lastVolumeLevel = -1f;

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

		public void SetVolume(double newVolume)
		{
			_audioFileReader.Volume = Convert.ToSingle(newVolume / 100);

			if (_audioFileReader.Volume > 0)
			{
				muted = false;
			}
		}

		public float GetVolume()
		{
			return _audioFileReader != null ? _audioFileReader.Volume / 100 : 1;
		}

		public double Mute()
		{
			if (muted)
			{
				//unmute
				_audioFileReader.Volume = lastVolumeLevel;
				lastVolumeLevel = -1f;
				muted = false;
			}
			else
			{
				//mute
				lastVolumeLevel = _audioFileReader.Volume;
				_audioFileReader.Volume = 0f;
				muted = true;
			}

			return Convert.ToDouble(_audioFileReader.Volume) * 100;
		}

		public double GetLengthInSeconds()
		{
			return _audioFileReader != null ? _audioFileReader.TotalTime.TotalSeconds : 0;
		}

		public double GetPositionInSeconds()
		{
			return _audioFileReader != null ? _audioFileReader.CurrentTime.TotalSeconds : 0;
		}

		public void SetPosition(double position)
		{
			if (_audioFileReader != null)
			{
				_audioFileReader.SetPosition(position);
			} 
		}
	}
}
