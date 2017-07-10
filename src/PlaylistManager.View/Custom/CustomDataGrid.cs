using System.Windows;
using System.Windows.Controls;

namespace PlaylistManager.View.Custom
{
	/// <summary>
	/// Adds sorted event when sorting is complete
	/// </summary>
	public sealed class CustomDataGrid : DataGrid
	{
		/// <summary>
		/// Create a custom routed event by first registering a RoutedEventID
		/// This event uses the bubbling routing strategy
		/// </summary>
		public static readonly RoutedEvent SortedEvent = EventManager.RegisterRoutedEvent(
			"Sorted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomDataGrid));

		public event RoutedEventHandler Sorted
		{
			add => AddHandler(SortedEvent, value);
			remove => RemoveHandler(SortedEvent, value);
		}

		void RaiseSortedEvent()
		{
			RoutedEventArgs newEventArgs = new RoutedEventArgs(CustomDataGrid.SortedEvent);
			RaiseEvent(newEventArgs);
		}

		protected override void OnSorting(DataGridSortingEventArgs _eventArgs)
		{
			base.OnSorting(_eventArgs);
			RaiseSortedEvent();
		}
	}
}