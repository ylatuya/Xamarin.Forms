using System;
using System.ComponentModel;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	public class AccessibilityThing : IDisposable // TODO Thnk of better name
	{
		const string GetFromElement = "GetValueFromElement";
		string _defaultContentDescription;
		bool? _defaultFocusable;
		string _defaultHint;
		bool _disposed;

		IVisualElementRenderer _renderer;

		public AccessibilityThing(IVisualElementRenderer renderer)
		{
			_renderer = renderer;
			_renderer.ElementPropertyChanged += OnElementPropertyChanged;
			_renderer.ElementChanged += OnElementChanged;
		}

		global::Android.Views.View Control => _renderer?.View;

		VisualElement Element => _renderer?.Element;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void SetAutomationId(string id = GetFromElement)
		{
			if (Element == null || Control == null)
			{
				return;
			}

			string value = id;
			if (value == GetFromElement)
			{
				value = Element.AutomationId;
			}

			if (!string.IsNullOrEmpty(value))
			{
				Control.ContentDescription = value;
			}
		}

		public void SetContentDescription(string contentDescription = GetFromElement)
		{
			if (Element == null || Control == null)
			{
				return;
			}

			if (SetHint())
			{
				return;
			}

			if (_defaultContentDescription == null)
			{
				_defaultContentDescription = Control.ContentDescription;
			}

			string value = contentDescription;
			if (value == GetFromElement)
			{
				value = string.Join(" ", (string)Element.GetValue(Accessibility.NameProperty),
					(string)Element.GetValue(Accessibility.HintProperty));
			}

			if (!string.IsNullOrWhiteSpace(value))
			{
				Control.ContentDescription = value;
			}
			else
			{
				Control.ContentDescription = _defaultContentDescription;
			}
		}

		public void SetFocusable(bool? value = null)
		{
			if (Element == null || Control == null)
			{
				return;
			}

			if (!_defaultFocusable.HasValue)
			{
				_defaultFocusable = Control.Focusable;
			}

			Control.Focusable =
				(bool)(value ?? (bool?)Element.GetValue(Accessibility.IsInAccessibleTreeProperty) ?? _defaultFocusable);
		}

		public bool SetHint(string hint = GetFromElement)
		{
			if (Element == null || Control == null)
			{
				return false;
			}

			var textView = Control as TextView;
			if (textView == null)
			{
				return false;
			}

			// Let the specified Title/Placeholder take precedence, but don't set the ContentDescription (won't work anyway)
			if (((Element as Picker)?.Title ?? (Element as Entry)?.Placeholder) != null)
			{
				return true;
			}

			if (_defaultHint == null)
			{
				_defaultHint = textView.Hint;
			}

			string value = hint;
			if (value == GetFromElement)
			{
				value = string.Join(". ", (string)Element.GetValue(Accessibility.NameProperty),
					(string)Element.GetValue(Accessibility.HintProperty));
			}

			if (!string.IsNullOrWhiteSpace(value))
			{
				textView.Hint = value;
			}
			else
			{
				textView.Hint = _defaultHint;
			}

			return true;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (_renderer != null)
			{
				if (_renderer.Element != null)
				{
					// TODO Should we unsub propertychanged here?
				}
				_renderer = null;
			}
		}

		void OnElementChanged(object sender, VisualElementChangedEventArgs e)
		{
			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			if (e.NewElement != null)
			{
				e.NewElement.PropertyChanged += OnElementPropertyChanged;
			}
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Accessibility.HintProperty.PropertyName)
			{
				SetContentDescription();
			}
			else if (e.PropertyName == Accessibility.NameProperty.PropertyName)
			{
				SetContentDescription();
			}
			else if (e.PropertyName == Accessibility.IsInAccessibleTreeProperty.PropertyName)
			{
				SetFocusable();
			}
		}
	}
}