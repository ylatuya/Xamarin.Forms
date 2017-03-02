using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using AImageView = Android.Widget.ImageView;
using AView = Android.Views.View;
using static System.String;
using Android.Views;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	public class ImageRenderer : AImageView, IVisualElementRenderer
	{
		bool _disposed;
		Image _element;
		bool _skipInvalidate;
		int? _defaultLabelFor;
		VisualElementTracker _visualElementTracker;
		VisualElementRenderer _visualElementRenderer;
		AccessibilityThing _accessibilityThing;

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (_disposed)
				return;

			_disposed = true;

			if (!disposing)
				return;

			if (_visualElementTracker != null)
			{
				_visualElementTracker.Dispose();
				_visualElementTracker = null;
			}

			if (_visualElementRenderer != null)
			{
				_visualElementRenderer.Dispose();
				_visualElementRenderer = null;
			}

			if (_element != null)
				_element.PropertyChanged -= OnElementPropertyChanged;

		}

		public override void Invalidate()
		{
			if (_skipInvalidate)
			{
				_skipInvalidate = false;
				return;
			}

			base.Invalidate();
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			UpdateBitmap(e.OldElement);
			UpdateAspect();

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		protected virtual Size MinimumSize()
		{
			return new Size();
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			Measure(widthConstraint, heightConstraint);
			return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), MinimumSize());
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			var image = element as Image;
			if (image == null)
				throw new ArgumentException("Element is not of type " + typeof(Image), nameof(element));

			Image oldElement = _element;
			_element = image;

			Performance.Start();

			if (oldElement != null)
				oldElement.PropertyChanged -= OnElementPropertyChanged;

			element.PropertyChanged += OnElementPropertyChanged;

			if (_visualElementTracker == null)
				_visualElementTracker = new VisualElementTracker(this);

			if (_visualElementRenderer == null)
			{
				_visualElementRenderer = new VisualElementRenderer(this);
			}

			if (_accessibilityThing == null)
			{
				_accessibilityThing = new AccessibilityThing(this);
			}

			Performance.Stop();

			OnElementChanged(new ElementChangedEventArgs<Image>(oldElement, _element));

			_element?.SendViewInitialized(Control);

			_accessibilityThing.SetContentDescription();
			_accessibilityThing.SetAutomationId();

		}

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
				_defaultLabelFor = LabelFor;

			LabelFor = (int)(id ?? _defaultLabelFor);
		}

		void IVisualElementRenderer.UpdateLayout() => _visualElementTracker?.UpdateLayout();

		VisualElement IVisualElementRenderer.Element => _element;

		VisualElementTracker IVisualElementRenderer.Tracker => _visualElementTracker;

		AView IVisualElementRenderer.View => this;

		ViewGroup IVisualElementRenderer.ViewGroup => null;

		protected AImageView Control => this;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		public ImageRenderer() : base(Forms.Context)
		{
		}

		protected void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{

			if (e.PropertyName == Image.SourceProperty.PropertyName)
				UpdateBitmap();
			else if (e.PropertyName == Image.AspectProperty.PropertyName)
				UpdateAspect();

			ElementPropertyChanged?.Invoke(this, e);
		}

		async void UpdateBitmap(Image previous = null)
		{
			if (Device.IsInvokeRequired)
				throw new InvalidOperationException("Image Bitmap must not be updated from background thread");

			if (previous != null && Equals(previous.Source, _element.Source))
				return;

			((IImageController)_element).SetIsLoading(true);

			SkipInvalidate();

			Control.SetImageResource(global::Android.Resource.Color.Transparent);

			ImageSource source = _element.Source;
			Bitmap bitmap = null;
			IImageSourceHandler handler;

			if (source != null && (handler = Registrar.Registered.GetHandler<IImageSourceHandler>(source.GetType())) != null)
			{
				try
				{
					bitmap = await handler.LoadImageAsync(source, Context);
				}
				catch (TaskCanceledException)
				{
				}
				catch (IOException ex)
				{
					Log.Warning("Xamarin.Forms.Platform.Android.ImageRenderer", "Error updating bitmap: {0}", ex);
				}
			}

			if (_element == null || !Equals(_element.Source, source))
			{
				bitmap?.Dispose();
				return;
			}

			if (!_disposed)
			{
				if (bitmap == null && source is FileImageSource)
					SetImageResource(ResourceManager.GetDrawableByName(((FileImageSource)source).File));
				else
					SetImageBitmap(bitmap);

				bitmap?.Dispose();

				((IImageController)_element).SetIsLoading(false);
				((IVisualElementController)_element).NativeSizeChanged();
			}
		}

		void SkipInvalidate()
		{
			_skipInvalidate = true;
		}

		void UpdateAspect()
		{
			ScaleType type = _element.Aspect.ToScaleType();
			SetScaleType(type);
		}

	}
}
