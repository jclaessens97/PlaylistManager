using NAudio.Wave;
using PlaylistManager.Model.Extensions;


namespace PlaylistManager.Model
{
	/// <summary>
	/// Backend class that interacts with NAudio classes and handles the media player methods
	/// </summary>
	public class AudioPlayer
	{
		#region Attributes

		private AudioFileReader audioFileReader;
		private IWavePlayer wavePlayer;

		private float volume;
		private float lastVolumeLevel = -1f;
		private bool isMuted;

		#endregion

		#region Navigation actions

		public void Play(Song _song)
		{
			if (_song == null) return;

			if (wavePlayer == null)
			{
				wavePlayer = new WaveOut();
			}

		    audioFileReader = new AudioFileReader(_song.Path);
		    wavePlayer.Init(audioFileReader);
			audioFileReader.Volume = volume;
			wavePlayer.Play();
		}

		public void Resume()
		{
			wavePlayer.Play();
		}

		public void Pause()
		{
			wavePlayer.Pause();
		}

		public void Next(Song _song)
		{
			Stop();
			Play(_song);
		}

		public void Prev(Song _song)
		{
			Stop();
			Play(_song);
		}

		public void Stop()
		{
			wavePlayer?.Stop();

			if (audioFileReader != null)
			{
				audioFileReader.Dispose();
				audioFileReader = null;
			}

			if (wavePlayer != null)
			{
				wavePlayer.Dispose();
				wavePlayer = null;
			}
		}

		#endregion

		#region Volume actions

		public void SetVolume(float _newVolume, bool _songPlaying)
		{
			volume = (_newVolume / 100);

			if (_songPlaying)
			{
				audioFileReader.Volume = volume;

				if (audioFileReader.Volume > 0)
					isMuted = false;
			}
		}

		public float ToggleMute()
		{
			if (isMuted)
			{
				//unmute
				audioFileReader.Volume = lastVolumeLevel;
				lastVolumeLevel = -1f;
				isMuted = false;
			}
			else
			{
				//mute
				lastVolumeLevel = audioFileReader.Volume;
				audioFileReader.Volume = 0f;
				isMuted = true;
			}

			return audioFileReader.Volume * 100;
		}

		#endregion

		#region Other actions

		public double GetLengthInSeconds()
		{
			return audioFileReader?.TotalTime.TotalSeconds ?? 0;
		}

		public double GetPositionInSeconds()
		{
			return audioFileReader?.CurrentTime.TotalSeconds ?? 0;
		}

		public void Seek(double _position)
		{
			audioFileReader?.SetPosition(_position);
		}

		#endregion
	}
}