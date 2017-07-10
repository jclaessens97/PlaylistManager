using System;
using System.Windows.Controls;

namespace PlaylistManager.ViewModel.Other
{
    /// <summary>
    /// Helper class to invoke dispatcher if needed (when called from other thread)
    /// </summary>
    public static class DispatcherTools
    {
        public static T InvokeDispatcher<T>(Control _control, Func<T> _callback)
        {
            if (_control.Dispatcher.CheckAccess())
            {
                return _callback.Invoke();
            }
            else
            {
                return _control.Dispatcher.Invoke(_callback);
            }
        }
    }
}
