using System.ComponentModel;
using AView = Android.Views.View;
using Object = Java.Lang.Object;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	public class VisualElementRenderer : Object, IEffectControlProvider
	{
		bool _disposed;
		
		IVisualElementRenderer _renderer;

		public VisualElementRenderer(IVisualElementRenderer renderer)
		{
			_renderer = renderer;
			_renderer.ElementPropertyChanged += OnElementPropertyChanged;
			_renderer.ElementChanged += OnElementChanged;
		}

		VisualElement Element => _renderer?.Element;
		
		AView Control => _renderer?.View;

		public void OnRegisterEffect(PlatformEffect effect)
		{
			effect.Control = Control;
			//TODO: is this crazy?
			effect.Container = Control;
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
				if (_renderer != null)
				{
					_renderer.ElementChanged -= OnElementChanged;
					_renderer.ElementPropertyChanged -= OnElementPropertyChanged;
					_renderer = null;
				}
			}

			base.Dispose(disposing);
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
				UpdateBackgroundColor();
			}
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackgroundColor();
			else if (e.PropertyName == VisualElement.InputTransparentProperty.PropertyName)
				UpdateInputTransparent();
		}
	}
}