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
		readonly Lazy<GestureDetector> _gestureDetector;
		readonly PanGestureHandler _panGestureHandler;
		readonly PinchGestureHandler _pinchGestureHandler;
		readonly Lazy<ScaleGestureDetector> _scaleDetector;
		readonly TapGestureHandler _tapGestureHandler;

		bool _clickable;
		NotifyCollectionChangedEventHandler _collectionChangeHandler;
		
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

			if (disposing)
			{
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
						// TODO Do we need to unsub the property changed stuff?
					}
					_renderer = null;
				}
			}

			base.Dispose(disposing);
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