using GMap.NET.GTK;
using GMap.NET.MapProviders;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Timers;
using System.Windows.Forms.Markers;
using Xamarin.Forms.Platform.GTK;

namespace Xamarin.Forms.Maps.GTK
{
    public class MapRenderer : ViewRenderer<Map, GMapControl>
    {
        private const int MinZoom = 0;
        private const int MaxZoom = 24;
        private const int Zoom = 6;

        private GMapImageMarker _userPosition;
        private bool _disposed;
        private Timer _timer;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            if (e.OldElement != null)
            {
                var mapModel = e.OldElement;
                MessagingCenter.Unsubscribe<Map, MapSpan>(this, "MapMoveToRegion");
                ((System.Collections.ObjectModel.ObservableCollection<Pin>)mapModel.Pins).CollectionChanged -= OnCollectionChanged;
            }

            if (e.NewElement != null)
            {
                var mapModel = e.NewElement;

                if (Control == null)
                {
                    var gMapControl = new GMapControl();
                    GMapProviders.GoogleMap.ApiKey = FormsMaps.AuthenticationToken;
                    gMapControl.MinZoom = MinZoom;
                    gMapControl.MaxZoom = MaxZoom;
                    gMapControl.Zoom = Zoom;
                    gMapControl.Overlays.Add(new GMapOverlay());
                    SetNativeControl(gMapControl);

                    Control.OnMapZoomChanged += OnMapZoomChanged;
                }

                MessagingCenter.Subscribe<Map, MapSpan>(this, "MapMoveToRegion", (s, a) =>
                MoveToRegion(a), mapModel);

                UpdateMapType();
                UpdateHasScrollEnabled();
                UpdateHasZoomEnabled();

                ((System.Collections.ObjectModel.ObservableCollection<Pin>)mapModel.Pins).CollectionChanged += OnCollectionChanged;

                if (mapModel.Pins.Any())
                    LoadPins();

                if (Control == null)
                    return;

                MoveToRegion(mapModel.VisibleRegion);
                UpdateIsShowingUser();
            }

            base.OnElementChanged(e);
        }

        private void OnMapZoomChanged()
        {
            UpdateVisibleRegion();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Maps.Map.MapTypeProperty.PropertyName)
                UpdateMapType();
            else if (e.PropertyName == Maps.Map.IsShowingUserProperty.PropertyName)
                UpdateIsShowingUser();
            else if (e.PropertyName == Maps.Map.HasScrollEnabledProperty.PropertyName)
                UpdateHasScrollEnabled();
            else if (e.PropertyName == Maps.Map.HasZoomEnabledProperty.PropertyName)
                UpdateHasZoomEnabled();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                MessagingCenter.Unsubscribe<Map, MapSpan>(this, "MapMoveToRegion");

                if (Control != null)
                {
                    Control.OnMapZoomChanged -= OnMapZoomChanged;
                }

                if (Element != null)
                    ((System.Collections.ObjectModel.ObservableCollection<Pin>)Element.Pins).CollectionChanged -= OnCollectionChanged;
            }

            base.Dispose(disposing);
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

        private void LoadPins()
        {
            foreach (var pin in Element.Pins)
                LoadPin(pin);
        }

        private void LoadPin(Pin pin)
        {
            var overlay = Control.Overlays.FirstOrDefault();

            if (overlay != null)
            {
                overlay.Markers.Add(new GMapImageMarker(
                    new GMap.NET.PointLatLng(
                        pin.Position.Latitude,
                        pin.Position.Longitude),
                    GMapImageMarkerType.RedDot));
            }
        }

        private void RemovePin(Pin pinToRemove)
        {
            var overlay = Control.Overlays.FirstOrDefault();

            if (overlay != null)
            {
                var positionToRemove = new GMap.NET.PointLatLng(
                        pinToRemove.Position.Latitude,
                        pinToRemove.Position.Longitude);

                var pins = overlay.Markers.Where(p => p.Position == positionToRemove);

                foreach (var pin in pins)
                {
                    overlay.Markers.Remove(pin);
                }
            }
        }

        private void ClearPins()
        {
            var overlay = Control.Overlays.FirstOrDefault();

            if (overlay != null)
            {
                overlay.Markers.Clear();
            }

            UpdateIsShowingUser();
        }

        private void UpdateMapType()
        {
            switch (Element.MapType)
            {
                case MapType.Street:
                    Control.MapProvider = GMapProviders.GoogleMap;
                    break;
                case MapType.Satellite:
                    Control.MapProvider = GMapProviders.GoogleSatelliteMap;
                    break;
                case MapType.Hybrid:
                    Control.MapProvider = GMapProviders.GoogleHybridMap;
                    break;
            }
        }

        private void UpdateIsShowingUser(bool moveToLocation = true)
        {
            if (Control == null || Element == null)
                return;

            var overlay = Control.Overlays.FirstOrDefault();

            if (Element.IsShowingUser)
            {
                var userPosition = GetUserPosition();

                if (userPosition != null)
                {
                    LoadUserPosition(userPosition.Value, moveToLocation);
                }

                if (Control == null || Element == null) return;

                if (_timer == null)
                {
                    _timer = new Timer();
                    _timer.Elapsed += (s, o) => UpdateIsShowingUser();
                    _timer.Interval = 1000 / 60.0;
                }

                if (!_timer.Enabled)
                    _timer.Start();
            }
            else if (_userPosition != null && overlay.Markers.Contains(_userPosition))
            {
                _timer.Stop();
                overlay.Markers.Remove(_userPosition);
            }
        }

        private void LoadUserPosition(GMap.NET.PointLatLng userCoordinate, bool center)
        {
            if (Control == null || Element == null) return;

            if (_userPosition == null)
            {
                _userPosition = new GMapImageMarker(userCoordinate, GMapImageMarkerType.Red);
            }

            var overlay = Control.Overlays.FirstOrDefault();

            if (overlay.Markers.Contains(_userPosition))
                overlay.Markers.Remove(_userPosition);

            overlay.Markers.Add(_userPosition);

            if (center)
            {
                Control.Position = userCoordinate;
            }
        }

        private GMap.NET.PointLatLng? GetUserPosition()
        {
            try
            {
                var ipAddress = GetPublicIpAddress();
                var webClient = new WebClient();
                var uri = new Uri(string.Format("http://freegeoip.net/json/{0}", ipAddress.ToString()));
                var result = webClient.DownloadString(uri);
                var location = JsonConvert.DeserializeObject<Location>(result);

                return new GMap.NET.PointLatLng(
                    location.Latitude,
                    location.Longitude);
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

        private void UpdateHasScrollEnabled()
        {
            var hasScrollEnabled = Element.HasScrollEnabled;

            Control.CanDragMap = hasScrollEnabled;
        }

        private void UpdateHasZoomEnabled()
        {
            var hasZoomEnabled = Element.HasZoomEnabled;

            Control.MouseWheelZoomEnabled = hasZoomEnabled;
        }

        private void MoveToRegion(MapSpan span)
        {
            if (span == null)
            {
                return;
            }

            var region = new GMap.NET.PointLatLng(
                 span.Center.Latitude + span.LatitudeDegrees / 2,
                 span.Center.Longitude - span.LongitudeDegrees / 2);

            Control.Position = region;
        }

        private void UpdateVisibleRegion()
        {
            if (Control == null || Element == null)
                return;

            try
            {
                var center = new Position(Control.Position.Lat, Control.Position.Lng);
                Element.VisibleRegion = new MapSpan(center, 0, 0);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}