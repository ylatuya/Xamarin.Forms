using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Android.Support.V4.View;
using Android.Views;
using Object = Java.Lang.Object;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	public class GestureManager : Object, global::Android.Views.View.IOnClickListener, global::Android.Views.View.IOnTouchListener
	{
		IVisualElementRenderer _renderer;
		readonly Lazy<GestureDetector> _gestureDetector;
		readonly PanGestureHandler _panGestureHandler;
		readonly PinchGestureHandler _pinchGestureHandler;
		readonly Lazy<ScaleGestureDetector> _scaleDetector;
		readonly TapGestureHandler _tapGestureHandler;
		InnerGestureListener _gestureListener;

		bool _clickable;
		bool _disposed;

		NotifyCollectionChangedEventHandler _collectionChangeHandler;

		VisualElement Element => _renderer?.Element;

		View View => _renderer?.Element as View;

		global::Android.Views.View Control => _renderer?.View;

		public GestureManager(IVisualElementRenderer renderer)
		{
			_renderer = renderer;
			_renderer.ElementChanged += OnElementChanged;

			_tapGestureHandler = new TapGestureHandler(() => View);
			_panGestureHandler = new PanGestureHandler(() => View, Control.Context.FromPixels);
			_pinchGestureHandler = new PinchGestureHandler(() => View);
			_gestureDetector =
				new Lazy<GestureDetector>(
					() =>
						new GestureDetector(
							_gestureListener =
								new InnerGestureListener(_tapGestureHandler.OnTap, _tapGestureHandler.TapGestureRecognizers,
									_panGestureHandler.OnPan, _panGestureHandler.OnPanStarted, _panGestureHandler.OnPanComplete)));

			_scaleDetector =
				new Lazy<ScaleGestureDetector>(
					() =>
						new ScaleGestureDetector(Control.Context,
							new InnerScaleListener(_pinchGestureHandler.OnPinch, _pinchGestureHandler.OnPinchStarted,
								_pinchGestureHandler.OnPinchEnded), Control.Handler));
		}

		void OnElementChanged(object sender, VisualElementChangedEventArgs e)
		{
			if (e.OldElement != null)
			{
				UnsubscribeGestureRecognizers(e.OldElement);
			}

			if (e.NewElement != null)
			{
				Control.SetOnClickListener(this);
				Control.SetOnTouchListener(this);
				UpdateGestureRecognizers(true);
				SubscribeGestureRecognizers(e.NewElement);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				Control.SetOnClickListener(null);
				Control.SetOnTouchListener(null);

				if (_gestureListener != null)
				{
					_gestureListener.Dispose();
					_gestureListener = null;
				}

				if (_renderer?.Element != null)
				{
					UnsubscribeGestureRecognizers(Element);
				}

				_renderer = null;
			}

			base.Dispose(disposing);
		}

		void global::Android.Views.View.IOnClickListener.OnClick(global::Android.Views.View v)
		{
			_tapGestureHandler.OnSingleClick();
		}

		bool global::Android.Views.View.IOnTouchListener.OnTouch(global::Android.Views.View v, MotionEvent e)
		{
			var handled = false;
			if (_pinchGestureHandler.IsPinchSupported)
			{
				if (!_scaleDetector.IsValueCreated)
				{
					ScaleGestureDetectorCompat.SetQuickScaleEnabled(_scaleDetector.Value, true);
				}
				handled = _scaleDetector.Value.OnTouchEvent(e);
			}
			return _gestureDetector.Value.OnTouchEvent(e) || handled;
		}

		void HandleGestureRecognizerCollectionChanged(object sender,
			NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			UpdateGestureRecognizers();
		}

		void SubscribeGestureRecognizers(VisualElement element)
		{
			var view = element as View;
			if (view == null)
			{
				return;
			}

			if (_collectionChangeHandler == null)
			{
				_collectionChangeHandler = HandleGestureRecognizerCollectionChanged;
			}

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
			{
				return;
			}

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
			{
				return;
			}

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
			{
				return;
			}

			UpdateClickable(forceClick);
		}
	}
}