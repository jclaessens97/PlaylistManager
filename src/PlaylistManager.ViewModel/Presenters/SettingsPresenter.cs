using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PlaylistManager.Model;
using PlaylistManager.Model.Other;
using PlaylistManager.ViewModel.Other;
using Settings = PlaylistManager.Model.Properties.Settings;

namespace PlaylistManager.ViewModel.Presenters
{
	public class SettingsPresenter : ObservableObject
	{
		#region Attributes

		private static volatile SettingsPresenter instance;
		private static readonly object syncRoot = new object();

		private readonly Settings settings;

		private float volume;
		private bool shuffleEnabled;
		private RepeatMode repeatMode;

		private string folder;
		private bool includeSubdirs;
		private float timeBetweenSongs;

		#endregion

		#region Properties

		public static SettingsPresenter Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
						{
							instance = new SettingsPresenter();
						}
					}
				}

				return instance;
			}
		}

		//Implicit settings
		public float Volume
		{
			get => volume;
			set
			{
				volume = value;
				RaisePropertyChangedEvent(nameof(Volume));
			}
		}
		public bool ShuffleEnabled
		{
			get => shuffleEnabled;
			set
			{
				shuffleEnabled = value;
				RaisePropertyChangedEvent(nameof(ShuffleEnabled));
			}
		}
		public RepeatMode RepeatMode
		{
			get => repeatMode;
			set
			{
				repeatMode = value;
				RaisePropertyChangedEvent(nameof(RepeatMode));
			}
		}

		//Explicit settings
		public string Folder
		{
			get => folder;
			set
			{
				folder = value;
				RaisePropertyChangedEvent(nameof(Folder));
				OnFolderChanged(EventArgs.Empty);
			}
		}
		public bool IncludeSubdirs
		{
			get => includeSubdirs;
			set
			{
				includeSubdirs = value;
				RaisePropertyChangedEvent(nameof(includeSubdirs));
			}
		}
		public float TimeBetweenSongs
		{
			get => timeBetweenSongs;
			set
			{
				timeBetweenSongs = value;
				RaisePropertyChangedEvent(nameof(TimeBetweenSongs));
			}
		}
		
		#endregion

		#region Commands

		public ICommand SaveCommand => new DelegateCommand(SaveExplicitSettings);
		public ICommand DefaultCommand => new DelegateCommand(LoadDefaultSettings);

		#endregion

		#region Events

		public EventHandler FolderChanged;

		private void OnFolderChanged(EventArgs _e)
		{
			EventHandler handler = FolderChanged;
			handler?.Invoke(Folder, _e);
		}
		
		#endregion

		private SettingsPresenter()
		{
			settings = Settings.Default;

			LoadSettings();
			//LoadAllDefaults();
		}

		#region Save

		private void SaveExplicitSettings()
		{
			settings.Folder = Folder;
			settings.IncludeSubdirs = IncludeSubdirs;
			settings.TimeBetweenSongs = TimeBetweenSongs;

			settings.Save();
			Debug.WriteLine("Settings saved!");
		}

		public void SaveImplicitSettings()
		{
			settings.Volume = Volume;
			settings.ShuffleEnabled = ShuffleEnabled;
			settings.RepeatMode = (byte)RepeatMode;

			settings.Save();
			Debug.WriteLine("Implicit Settings saved!");
		}

		#endregion

		#region Load

		private void LoadSettings()
		{
			//Implicit settings
			this.Volume = settings.Volume;
			this.ShuffleEnabled = settings.ShuffleEnabled;
			this.RepeatMode = (RepeatMode)settings.RepeatMode;

			//Explicit settings
			this.Folder = settings.Folder;
			this.IncludeSubdirs = settings.IncludeSubdirs;
			this.TimeBetweenSongs = settings.TimeBetweenSongs;
		}

		private void LoadDefaultSettings()
		{
			this.Folder = DefaultSettings.Folder;
			this.IncludeSubdirs = DefaultSettings.IncludeSubdirs;
			this.TimeBetweenSongs = DefaultSettings.TimeBetweenSongs;
		}

		#endregion

		#region Debug

		[System.Diagnostics.Conditional("DEBUG")]
		private void LoadAllDefaults()
		{
			this.Volume = DefaultSettings.Volume;
			this.ShuffleEnabled = DefaultSettings.ShuffleEnabled;
			this.RepeatMode = DefaultSettings.Repeatmode;

			LoadDefaultSettings();
		}

		#endregion	
	}
}
