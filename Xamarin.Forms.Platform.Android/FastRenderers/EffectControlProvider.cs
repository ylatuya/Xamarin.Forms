using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	internal class EffectControlProvider : IEffectControlProvider
	{
		readonly AView _control; 

		public EffectControlProvider(AView control)
		{
			_control = control;
		}

		public void RegisterEffect(Effect effect)
		{
			var platformEffect = effect as PlatformEffect;
			if (platformEffect == null)
			{
				return;
			}

			platformEffect.SetControl(_control);
			platformEffect.SetContainer(_control);
		}
	}
}