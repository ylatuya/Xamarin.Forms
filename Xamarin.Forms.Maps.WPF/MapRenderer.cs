using Microsoft.Maps.MapControl.WPF;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Platform.WPF;

namespace Xamarin.Forms.Maps.WPF
{
	public class MapRenderer : ViewRenderer<Map, Microsoft.Maps.MapControl.WPF.Map>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				var mapModel = e.NewElement;

				if (Control == null)
				{
					SetNativeControl(new Microsoft.Maps.MapControl.WPF.Map());

					ApplicationIdCredentialsProvider bingKey = new ApplicationIdCredentialsProvider(FormsMaps.AuthenticationToken);
					Control.CredentialsProvider = bingKey;
				}

				UpdateMapType();
				UpdateHasScrollEnabled();
				UpdateHasZoomEnabled();

				if (mapModel.Pins.Any())
					LoadPins();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Map.MapTypeProperty.PropertyName)
				UpdateMapType();
			else if (e.PropertyName == Map.HasScrollEnabledProperty.PropertyName)
				UpdateHasScrollEnabled();
			else if (e.PropertyName == Map.HasZoomEnabledProperty.PropertyName)
				UpdateHasZoomEnabled();
		}

		private void UpdateMapType()
		{
			switch (Element.MapType)
			{
				case MapType.Street:
					Control.Mode = new RoadMode();
					break;
				case MapType.Satellite:
					Control.Mode = new AerialMode(false);
					break;
				case MapType.Hybrid:
					Control.Mode = new AerialMode(true);
					break;
			}
		}

		private void UpdateHasScrollEnabled()
		{

		}

		private void UpdateHasZoomEnabled()
		{

		}

		private void LoadPins()
		{

		}
	}
}