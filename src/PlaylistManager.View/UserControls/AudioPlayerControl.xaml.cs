﻿using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using PlaylistManager.ViewModel.Interfaces;
using PlaylistManager.ViewModel.Other;
using PlaylistManager.ViewModel.ViewModels;


namespace PlaylistManager.View.UserControls
{
	public partial class AudioPlayerControl : UserControl, IAudioPlayerControl
	{
        #region Properties

	    public PackIcon PlayPauseIcon => playPauseIcon;
	    public PackIcon RepeatIcon => repeatIcon;
        public PackIcon ShuffleIcon => shuffleIcon;
	    public PackIcon VolumeIcon => volumeIcon;

	    #endregion

        public AudioPlayerControl()
		{
			InitializeComponent();

		    DataContext = AudioPlayerControlViewModel.Instance;
		    (DataContext as AudioPlayerControlViewModel).AudioPlayerControl = this;

            ToggleEnable(false);
		    (DataContext as AudioPlayerControlViewModel).HasPrev = false;
		    (DataContext as AudioPlayerControlViewModel).HasNext = false;
		}

        #region GUI

        /// <summary>
	    /// Enables/disables all buttons according to state
	    /// </summary>
	    /// <param name="_state"></param>
	    public void ToggleEnable(bool _state)
        {
            DispatcherTools.InvokeDispatcher(btnStop, () => btnStop.IsEnabled = _state);
            DispatcherTools.InvokeDispatcher(sliderTime, () => sliderTime.IsEnabled = _state);
            DispatcherTools.InvokeDispatcher(btnVolume, () => btnVolume.IsEnabled = _state);
            DispatcherTools.InvokeDispatcher(sliderVolume, () => sliderVolume.IsEnabled = _state);
        }

        #endregion
    }
}