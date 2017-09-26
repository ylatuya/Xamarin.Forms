using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Windows;
using Xamarin.Forms.Platform.WPF;

namespace Xamarin.Forms.Maps.WPF
{
	public class MapRenderer : ViewRenderer<Map, Microsoft.Maps.MapControl.WPF.Map>
	{
		private Pushpin _userPosition;

		protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				var mapModel = e.OldElement;
				MessagingCenter.Unsubscribe<Map, MapSpan>(this, "MapMoveToRegion");
				((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged -= OnCollectionChanged;
			}

			if (e.NewElement != null)
			{
				var mapModel = e.NewElement;

				if (Control == null)
				{
					SetNativeControl(new Microsoft.Maps.MapControl.WPF.Map());

					ApplicationIdCredentialsProvider bingKey = new ApplicationIdCredentialsProvider(FormsMaps.AuthenticationToken);
					Control.CredentialsProvider = bingKey;

					Control.SizeChanged += OnSizeChanged;
				}

				MessagingCenter.Subscribe<Map, MapSpan>(this, "MapMoveToRegion", (s, a) =>
				MoveToRegion(a), mapModel);

				UpdateMapType();
				UpdateHasScrollEnabled();
				UpdateHasZoomEnabled();

				((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged += OnCollectionChanged;

				if (mapModel.Pins.Any())
					LoadPins();

				MoveToRegion(mapModel.LastMoveToRegion);
				UpdateIsShowingUser();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Map.MapTypeProperty.PropertyName)
				UpdateMapType();
			else if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
				UpdateIsShowingUser();
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
			var hasScrollEnabled = Element.HasScrollEnabled;
		}

		private void UpdateHasZoomEnabled()
		{
			var hasZoomEnabled = Element.HasZoomEnabled;
		}

		private void LoadPins()
		{
			foreach (var pin in Element.Pins)
				LoadPin(pin);
		}

		private void LoadPin(Pin pin)
		{
			Pushpin pushpin = new Pushpin
			{
				ToolTip = pin.Label,
				Location = new Location(pin.Position.Latitude, pin.Position.Longitude)
			};

			Control.Children.Add(pushpin);
		}

		private void RemovePin(Pin pinToRemove)
		{
			var positionToRemove = new Location(pinToRemove.Position.Latitude, pinToRemove.Position.Longitude);

			foreach (UIElement element in Control.Children)
			{
				if (element.GetType() == typeof(MapLayer))
				{
					MapLayer mapLayer = (MapLayer)element;

					foreach (UIElement p in mapLayer.Children)
					{
						if (p.GetType() == typeof(Pushpin))
						{
							var pushpin = (Pushpin)p;

							if (pushpin.Location == positionToRemove)
							{
								Control.Children.Remove(pushpin);
							}
						}
					}
				}
			}
		}

		private void ClearPins()
		{
			Control.Children.Clear();
			
			UpdateIsShowingUser();
		}

		private void MoveToRegion(MapSpan span)
		{
			try
			{
				if (span == null)
				{
					return;
				}

				var p1 = new Location
				{
					Latitude = span.Center.Latitude + span.LatitudeDegrees / 2,
					Longitude = span.Center.Longitude - span.LongitudeDegrees / 2
				};

				var p2 = new Location
				{
					Latitude = span.Center.Latitude - span.LatitudeDegrees / 2,
					Longitude = span.Center.Longitude + span.LongitudeDegrees / 2
				};

				double x1 = Math.Min(p1.Longitude, p2.Longitude);
				double y1 = Math.Max(p1.Latitude, p2.Latitude);
				double x2 = Math.Max(p1.Longitude, p2.Longitude);
				double y2 = Math.Min(p1.Latitude, p2.Latitude);

				var region = new LocationRect(y1, x1, x2 - x1, y1 - y2);

				Control.SetView(region);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("MoveToRegion exception: " + ex);
				Log.Warning("Xamarin.Forms MapRenderer", $"MoveToRegion exception: {ex}");
			}
		}

		private void UpdateVisibleRegion()
		{
			if (Control == null || Element == null)
				return;

			try
			{
				var region = Control.BoundingRectangle;
				var topLeft = region.Northeast;
				var center = region.Center;
				var rightBottom = region.Southwest;

				var latitudeDelta = Math.Abs(topLeft.Latitude - rightBottom.Latitude);
				var longitudeDelta = Math.Abs(topLeft.Longitude - rightBottom.Longitude);

				Element.VisibleRegion = new MapSpan(
					new Position(center.Latitude, center.Longitude),
					latitudeDelta,
					longitudeDelta);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("UpdateVisibleRegion exception: " + ex);
				Log.Warning("Xamarin.Forms MapRenderer", $"UpdateVisibleRegion exception: {ex}");

				return;
			}
		}

		private void UpdateIsShowingUser(bool moveToLocation = true)
		{
			if (Control == null || Element == null) return;

			if (Element.IsShowingUser)
			{
				if (Control == null || Element == null) return;

				var userPosition = GetUserPosition();

				if (userPosition != null)
				{
					LoadUserPosition(userPosition, moveToLocation);
				}
			}
		}
		private Location GetUserPosition()
		{
			try
			{
				var ipAddress = GetPublicIpAddress();
				var webClient = new WebClient();
				var uri = new Uri(string.Format("http://freegeoip.net/json/{0}", ipAddress.ToString()));
				var result = webClient.DownloadString(uri);
				var location = JsonConvert.DeserializeObject<Location>(result);

				return new Location(location.Latitude, location.Longitude);
			}
			catch
			{
				return null;
			}
		}

		private string GetPublicIpAddress()
		{
			string uri = "http://checkip.dyndns.org/";
			string ip = string.Empty;

			using (var client = new HttpClient())
			{
				var result = client.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;

				ip = result.Split(':')[1].Split('<')[0];
				ip = ip.Trim();
			}

			return ip;
		}

		private void LoadUserPosition(Location userCoordinate, bool center)
		{
			if (Control == null || Element == null) return;

			if (_userPosition == null)
			{
				_userPosition = new Pushpin { Location = userCoordinate };
			}

			Control.Children.Add(_userPosition);
			
			if (center)
			{
				Control.Center = userCoordinate;
			}
		}

		private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
		{
			UpdateVisibleRegion();
		}

		private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					foreach (Pin pin in e.NewItems)
						LoadPin(pin);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
					// Do nothing
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					foreach (Pin pin in e.OldItems)
						RemovePin(pin);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
					foreach (Pin pin in e.OldItems)
						RemovePin(pin);
					foreach (Pin pin in e.NewItems)
						LoadPin(pin);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
					ClearPins();
					break;
			}
		}
	}
}