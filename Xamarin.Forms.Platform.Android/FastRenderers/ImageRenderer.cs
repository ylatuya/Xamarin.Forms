using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using AImageView = Android.Widget.ImageView;
using AView = Android.Views.View;
using static System.String;
using Android.Views;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	public class ImageRenderer : AImageView, IVisualElementRenderer, IEffectControlProvider
	{
		bool _disposed;
		Image _element;
		int? _defaultLabelFor;
		VisualElementTracker _visualElementTracker;
		VisualElementRenderer _visualElementRenderer;

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

			if (Element != null)
				Element.PropertyChanged -= OnElementPropertyChanged;

		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			if (e.NewElement != null)
			{
				if (_visualElementTracker == null)
				{
					//	_visualElementRenderer = new VisualElementRenderer(this);
					_visualElementTracker = new VisualElementTracker(this);
				}
			}

			UpdateBitmap(e.OldElement);
			UpdateAspect();

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		protected virtual Size MinimumSize()
		{
			return new Size();
		}


		protected virtual void OnRegisterEffect(PlatformEffect effect)
		{
			effect.Container = this;
			effect.Control = this;
		}


		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			var platformEffect = effect as PlatformEffect;
			if (platformEffect != null)
				OnRegisterEffect(platformEffect);
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			Measure(widthConstraint, heightConstraint);
			return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), MinimumSize());
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			var image = element as Image;
			if (image == null)
				throw new ArgumentException("Element must be of type Image");

			Element = image;
			_visualElementRenderer?.SetAutomationId();
		}

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
				_defaultLabelFor = LabelFor;

			LabelFor = (int)(id ?? _defaultLabelFor);
		}

		void IVisualElementRenderer.UpdateLayout() => _visualElementTracker?.UpdateLayout();

		VisualElement IVisualElementRenderer.Element => Element;

		VisualElementTracker IVisualElementRenderer.Tracker => _visualElementTracker;

		AView IVisualElementRenderer.View => this;

		ViewGroup IVisualElementRenderer.ViewGroup => null;

		protected AImageView Control => this;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		public ImageRenderer() : base(Forms.Context)
		{
		}

		void UpdateAspect()
		{
			ScaleType type = Element.Aspect.ToScaleType();
			SetScaleType(type);
		}

		protected Image Element
		{
			get { return _element; }
			set
			{
				if (_element == value)
					return;

				Image oldElement = _element;
				_element = value;

				OnElementChanged(new ElementChangedEventArgs<Image>(oldElement, _element));

				_element?.SendViewInitialized(Control);
			}
		}
		protected void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{

			if (e.PropertyName == Image.SourceProperty.PropertyName)
				UpdateBitmap();
			else if (e.PropertyName == Image.AspectProperty.PropertyName)
				UpdateAspect();
			//if (e.PropertyName == Accessibility.LabeledByProperty.PropertyName)
			//	SetLabeledBy();

			ElementPropertyChanged?.Invoke(this, e);
		}


		async void UpdateBitmap(Image previous = null)
		{
			if (Device.IsInvokeRequired)
				throw new InvalidOperationException("Image Bitmap must not be updated from background thread");

			if (previous != null && Equals(previous.Source, Element.Source))
				return;

			((IImageController)Element).SetIsLoading(true);

			var formsImageView = Control as FormsImageView;
			formsImageView?.SkipInvalidate();

			Control.SetImageResource(global::Android.Resource.Color.Transparent);

			ImageSource source = Element.Source;
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

			if (Element == null || !Equals(Element.Source, source))
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

				((IImageController)Element).SetIsLoading(false);
				((IVisualElementController)Element).NativeSizeChanged();
			}
		}
	}
}
