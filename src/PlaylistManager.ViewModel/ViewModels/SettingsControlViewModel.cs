using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Input;
using PlaylistManager.Model;
using PlaylistManager.Model.Other;
using PlaylistManager.Model.Properties;
using PlaylistManager.ViewModel.Interfaces;
using PlaylistManager.ViewModel.Other;
using Timer = System.Timers.Timer;

namespace PlaylistManager.ViewModel.ViewModels
{
    public sealed class SettingsControlViewModel : ObservableObject
    {
        #region Attributes

        private static volatile SettingsControlViewModel instance;
        private static readonly object syncRoot = new object();

        private readonly Settings settings;

        private bool settingsChanged;

        private float volume;
        private bool shuffleEnabled;
        private RepeatMode repeatMode;

        //TODO: theme setting
        //TODO: media key setting

        private string folder;
        private bool includeSubdirs;
        private double timeBetweenSongs;

        private Timer validationTimer;
        private Dictionary<string, object> settingsBeforeChange;

        #endregion

        #region Properties

        public static SettingsControlViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new SettingsControlViewModel();
                        }
                    }
                }

                return instance;
            }
        }

        public ISettingsControl SettingsControl { get; set; }

        /// <summary>
        /// True if settings are changed while not saved
        /// False if settings are not changed or settings are changed & saved
        /// </summary>
        public bool SettingsChanged
        {
            get => settingsChanged;
            set
            {
                settingsChanged = value;
                RaisePropertyChangedEvent(nameof(SettingsChanged));
            }
        }

        //implicit settings
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

        //explicit settings
        public string Folder
        {
            get => folder;
            set
            {
                folder = value;
                RaisePropertyChangedEvent(nameof(Folder));
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
        public double TimeBetweenSongs
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

        public ICommand BrowseCommand => new DelegateCommand(BrowseFolder);
        public ICommand SaveCommand => new DelegateCommand(SaveExplicitSettings);
        public ICommand DefaultCommand => new DelegateCommand(LoadDefaultSettings);

        #endregion

        #region Events

        private void BrowseFolder()
        {
            var dialog = new FolderBrowserDialog()
            {
                SelectedPath = Folder,
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Folder = dialog.SelectedPath;
                Debug.WriteLine("Selected folder: " + Folder);
            }
        }

        private void validationTimer_Elapsed(object _sender, ElapsedEventArgs _e)
        {
            SettingsChanged = CheckSettingsChanged();
        }

        #endregion

        private SettingsControlViewModel()
        {
            settings = Settings.Default;
            LoadSettings();
            //LoadAllDefaults(); //debug method
        }

        #region Validation

        /// <summary>
        /// Starts validation timer & instantiate if not existing
        /// </summary>
        public void StartValidationTimer()
        {
            //Get settings before anything changed & store them in a dictionary
            LoadSettingsDictionary();

            //Start timer
            if (validationTimer == null)
            {
                validationTimer = new Timer(500);
                validationTimer.Elapsed += validationTimer_Elapsed;
            }

            validationTimer.Enabled = true;
        }

        /// <summary>
        /// Stops & Disposes validationtimer
        /// </summary>
        public void StopValidationTimer()
        {
            validationTimer?.Dispose();
            validationTimer = null;
        }

        /// <summary>
        /// Checks if anything changed against the settings when loaded in the dictionary
        /// </summary>
        /// <returns></returns>
        private bool CheckSettingsChanged()
        {
            if (DispatcherTools.InvokeDispatcher(SettingsControl.FolderTextBox,
                () => SettingsControl.FolderTextBox.Text != settingsBeforeChange[Globals.FolderKey].ToString()))
            {
                return true;
            }

            if (DispatcherTools.InvokeDispatcher(SettingsControl.FolderTextBox,
                () => SettingsControl.SubDirsCheckbox.IsChecked != (bool) settingsBeforeChange[Globals.SubdirsKey]))
            {
                return true;
            }

            if (DispatcherTools.InvokeDispatcher(SettingsControl.TimeBetweenSongsSlider,
                () => SettingsControl.TimeBetweenSongsSlider.Value <
                      (double) settingsBeforeChange[Globals.TimeBetweenSongsKey]
                      || SettingsControl.TimeBetweenSongsSlider.Value >
                      (double) settingsBeforeChange[Globals.TimeBetweenSongsKey]))
            {
                return true;
            }
            
            return false; //nothing changed
        }

        #endregion

        #region Save

        /// <summary>
        /// Saves explicit settings (set in settings tab)
        /// </summary>
        private void SaveExplicitSettings()
        {
            settings.Folder = Folder;
            settings.IncludeSubdirs = IncludeSubdirs;
            settings.TimeBetweenSongs = TimeBetweenSongs;

            settings.Save();
            LoadSettingsDictionary(); //Update dictionary
            settingsChanged = false;
            Debug.WriteLine("Settings Saved!");
        }

        /// <summary>
        /// Saves implicit settings (set when window closes)
        /// </summary>
        internal void SaveImplicitSettings()
        {
            settings.Volume = Volume;
            settings.ShuffleEnabled = ShuffleEnabled;
            settings.RepeatMode = (byte) RepeatMode;
            
            settings.Save();
            Debug.WriteLine("Implicit settings saved!");
        }

        #endregion

        #region Load

        /// <summary>
        /// Load all settings
        /// </summary>
        private void LoadSettings()
        {
            //Implicit settings
            Volume = settings.Volume;
            ShuffleEnabled = settings.ShuffleEnabled;
            RepeatMode = (RepeatMode)settings.RepeatMode;

            //Explicit settings
            Folder = settings.Folder;
            IncludeSubdirs = settings.IncludeSubdirs;
            TimeBetweenSongs = settings.TimeBetweenSongs;
        }

        /// <summary>
        /// Loads default settings
        /// </summary>
        private void LoadDefaultSettings()
        {
            Folder = DefaultSettings.Folder;
            IncludeSubdirs = DefaultSettings.IncludeSubdirs;
            TimeBetweenSongs = DefaultSettings.TimeBetweenSongs;
        }

        /// <summary>
        /// Loads settings before starting validation timer (to compare if changes happened)
        /// </summary>
        private void LoadSettingsDictionary()
        {
            settingsBeforeChange = new Dictionary<string, object>
            {
                {Globals.FolderKey, Folder},
                {Globals.SubdirsKey, IncludeSubdirs},
                {Globals.TimeBetweenSongsKey, TimeBetweenSongs}
            };
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
