using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlaylistManager.View.Behaviours
{
	public sealed class HandleDoubleClickBehavior
	{
		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
			"Command", typeof(ICommand), typeof(HandleDoubleClickBehavior), new PropertyMetadata(default(ICommand), OnComandChanged));

		public static void SetCommand(DependencyObject _element, ICommand _value)
		{
			_element.SetValue(CommandProperty, _value);
		}

		public static ICommand GetCommand(DependencyObject _element)
		{
			return (ICommand)_element.GetValue(CommandProperty);
		}

		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
			"CommandParameter", typeof(object), typeof(HandleDoubleClickBehavior), new PropertyMetadata(default(object)));

		public static void SetCommandParameter(DependencyObject _element, object _value)
		{
			_element.SetValue(CommandParameterProperty, _value);
		}

		public static object GetCommandParameter(DependencyObject _element)
		{
			return (object)_element.GetValue(CommandParameterProperty);
		}

		private static void OnComandChanged(DependencyObject _d, DependencyPropertyChangedEventArgs _e)
		{
			var c = _d as Control;
			if (c == null)
				throw new InvalidOperationException($"can only be attached to {nameof(Control)}");
			c.MouseDoubleClick -= OnDoubleClick;
			if (GetCommand(c) != null)
				c.MouseDoubleClick += OnDoubleClick;
		}

		private static void OnDoubleClick(object _sender, MouseButtonEventArgs _e)
		{
			var d = _sender as DependencyObject;
			if (d == null)
				return;
			var command = GetCommand(d);
			if (command == null)
				return;
			var parameter = GetCommandParameter(d);
			if (!command.CanExecute(parameter))
				return;
			command.Execute(parameter);
		}
	}
}