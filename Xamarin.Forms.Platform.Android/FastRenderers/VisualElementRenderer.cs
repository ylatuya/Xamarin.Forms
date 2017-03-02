using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using AView = Android.Views.View;
using Object = Java.Lang.Object;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	public class VisualElementRenderer : Object, AView.IOnClickListener, AView.IOnTouchListener, IEffectControlProvider
	{
		const string GetFromElement = "GetValueFromElement";

		Lazy<GestureDetector> _gestureDetector;
		PanGestureHandler _panGestureHandler;
		PinchGestureHandler _pinchGestureHandler;
		Lazy<ScaleGestureDetector> _scaleDetector;
		TapGestureHandler _tapGestureHandler;

		bool _clickable;
		NotifyCollectionChangedEventHandler _collectionChangeHandler;
		string _defaultContentDescription;
		bool? _defaultFocusable;
		string _defaultHint;
		bool _disposed;
		InnerGestureListener _gestureListener;
		IVisualElementRenderer _renderer;

		public VisualElementRenderer(IVisualElementRenderer renderer)
		{
			_renderer = renderer;
			_renderer.ElementPropertyChanged += OnElementPropertyChanged;
			_renderer.ElementChanged += OnElementChanged;
			_tapGestureHandler = new TapGestureHandler(() => View);
			_panGestureHandler = new PanGestureHandler(() => View, Control.Context.FromPixels);
			_pinchGestureHandler = new PinchGestureHandler(() => View);
			_gestureDetector =
					new Lazy<GestureDetector>(
						() =>
						new GestureDetector(
							_gestureListener =
							new InnerGestureListener(_tapGestureHandler.OnTap, _tapGestureHandler.TapGestureRecognizers, _panGestureHandler.OnPan, _panGestureHandler.OnPanStarted, _panGestureHandler.OnPanComplete)));

			_scaleDetector =
				new Lazy<ScaleGestureDetector>(
					() => new ScaleGestureDetector(Control.Context, new InnerScaleListener(_pinchGestureHandler.OnPinch, _pinchGestureHandler.OnPinchStarted, _pinchGestureHandler.OnPinchEnded), Control.Handler));

		}

		VisualElement Element => _renderer?.Element;

		View View => _renderer?.Element as View;

		AView Control => _renderer?.View;

		void AView.IOnClickListener.OnClick(AView v)
		{
			_tapGestureHandler.OnSingleClick();
		}

		public void OnRegisterEffect(PlatformEffect effect)
		{
			effect.Control = Control;
			//TODO: is this crazy?
			effect.Container = Control;
		}

		bool AView.IOnTouchListener.OnTouch(AView v, MotionEvent e)
		{
			var handled = false;
			if (_pinchGestureHandler.IsPinchSupported)
			{
				if (!_scaleDetector.IsValueCreated)
					ScaleGestureDetectorCompat.SetQuickScaleEnabled(_scaleDetector.Value, true);
				handled = _scaleDetector.Value.OnTouchEvent(e);
			}
			return _gestureDetector.Value.OnTouchEvent(e) || handled;
		}

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			var platformEffect = effect as PlatformEffect;
			if (platformEffect != null)
				OnRegisterEffect(platformEffect);
		}

		public void SetAutomationId(string id = GetFromElement)
		{
			if (Element == null || Control == null)
				return;

			var value = id;
			if (value == GetFromElement)
				value = Element.AutomationId;

			if (!string.IsNullOrEmpty(value))
				Control.ContentDescription = value;
		}

		public void SetContentDescription(string contentDescription = GetFromElement)
		{
			if (Element == null || Control == null)
				return;

			if (SetHint())
				return;

			if (_defaultContentDescription == null)
				_defaultContentDescription = Control.ContentDescription;

			var value = contentDescription;
			if (value == GetFromElement)
				value = string.Join(" ", (string)Element.GetValue(Accessibility.NameProperty), (string)Element.GetValue(Accessibility.HintProperty));

			if (!string.IsNullOrWhiteSpace(value))
				Control.ContentDescription = value;
			else
				Control.ContentDescription = _defaultContentDescription;
		}

		public void SetFocusable(bool? value = null)
		{
			if (Element == null || Control == null)
				return;

			if (!_defaultFocusable.HasValue)
				_defaultFocusable = Control.Focusable;

			Control.Focusable = (bool)(value ?? (bool?)Element.GetValue(Accessibility.IsInAccessibleTreeProperty) ?? _defaultFocusable);
		}

		public bool SetHint(string hint = GetFromElement)
		{
			if (Element == null || Control == null)
				return false;

			var textView = Control as global::Android.Widget.TextView;
			if (textView == null)
				return false;

			// Let the specified Title/Placeholder take precedence, but don't set the ContentDescription (won't work anyway)
			if (((Element as Picker)?.Title ?? (Element as Entry)?.Placeholder) != null)
				return true;

			if (_defaultHint == null)
				_defaultHint = textView.Hint;

			var value = hint;
			if (value == GetFromElement)
				value = string.Join(". ", (string)Element.GetValue(Accessibility.NameProperty), (string)Element.GetValue(Accessibility.HintProperty));

			if (!string.IsNullOrWhiteSpace(value))
				textView.Hint = value;
			else
				textView.Hint = _defaultHint;

			return true;
		}

		public void UpdateBackgroundColor(Color? color = null)
		{
			if (Element == null || Control == null)
				return;

			Control.SetBackgroundColor((color ?? Element.BackgroundColor).ToAndroid());
		}

		public void UpdateInputTransparent(bool? inputTransparent = null)
		{
			if (Element == null || Control == null)
				return;

			//TODO: InputTransparent is on FormsViewGroup
			//InputTransparent = Element.InputTransparent;
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (_gestureListener != null)
			{
				_gestureListener.Dispose();
				_gestureListener = null;
			}

			if (_renderer != null)
			{
				if (_renderer.Element != null)
				{
					UnsubscribeGestureRecognizers(Element);

				}
				_renderer = null;
			}
		}

		void HandleGestureRecognizerCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			UpdateGestureRecognizers();
		}

		void OnElementChanged(object sender, VisualElementChangedEventArgs e)
		{
			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;
				UnsubscribeGestureRecognizers(e.OldElement);
			}

			if (e.NewElement != null)
			{
				Control.SetOnClickListener(this);
				Control.SetOnTouchListener(this);

				UpdateGestureRecognizers(true);

				e.NewElement.PropertyChanged += OnElementPropertyChanged;
				UpdateBackgroundColor();
				SubscribeGestureRecognizers(e.NewElement);
			}
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackgroundColor();
			else if (e.PropertyName == VisualElement.InputTransparentProperty.PropertyName)
				UpdateInputTransparent();
			else if (e.PropertyName == Accessibility.HintProperty.PropertyName)
				SetContentDescription();
			else if (e.PropertyName == Accessibility.NameProperty.PropertyName)
				SetContentDescription();
			else if (e.PropertyName == Accessibility.IsInAccessibleTreeProperty.PropertyName)
				SetFocusable();
		}

		void SubscribeGestureRecognizers(VisualElement element)
		{
			var view = element as View;
			if (view == null)
				return;

			if (_collectionChangeHandler == null)
				_collectionChangeHandler = HandleGestureRecognizerCollectionChanged;

			var observableCollection = (ObservableCollection<IGestureRecognizer>)view.GestureRecognizers;
			if (observableCollection != null)
			{
				observableCollection.CollectionChanged += _collectionChangeHandler;
			}
		}

		void UnsubscribeGestureRecognizers(VisualElement element)
		{
			var view = element as View;
			if (view == null || _collectionChangeHandler == null)
				return;

			var observableCollection = (ObservableCollection<IGestureRecognizer>)view.GestureRecognizers;
			if (observableCollection != null)
			{
				observableCollection.CollectionChanged -= _collectionChangeHandler;
			}
		}

		void UpdateClickable(bool force = false)
		{
			var view = Element as View;
			if (view == null)
				return;

			bool newValue = view.ShouldBeMadeClickable();
			if (force || _clickable != newValue)
			{
				Control.Clickable = newValue;
				_clickable = newValue;
			}
		}

		void UpdateGestureRecognizers(bool forceClick = false)
		{
			if (Element == null)
				return;

			UpdateClickable(forceClick);
		}
	}
}