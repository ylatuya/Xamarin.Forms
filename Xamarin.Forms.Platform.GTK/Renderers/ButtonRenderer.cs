using Gtk;
using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class ButtonRenderer : ViewRenderer<Button, ButtonRenderer.GtkButtonWrapper>
    {
        protected override void Dispose(bool disposing)
        {
            if (Control != null)
            {
                Control.Clicked -= OnButtonClicked;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var btn = new GtkButtonWrapper();
                    SetNativeControl(btn);

                    Control.Clicked += OnButtonClicked;
                }

                if (Element.BackgroundColor != Color.Default)
                    UpdateBackgroundColor();

                if (Element.TextColor != Color.Default)
                    UpdateTextColor();

                UpdateText();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Button.TextProperty.PropertyName)
            {
                UpdateText();
            }
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            {
                UpdateBackgroundColor();
            }
            else if (e.PropertyName == Button.TextColorProperty.PropertyName)
            {
                UpdateTextColor();
            }
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            ((IButtonController)Element)?.SendClicked();
        }

        private void UpdateText()
        {
            Control.LabelWidget.Text = Element.Text ?? string.Empty;
        }

        protected override void UpdateBackgroundColor()
        {
            var backgroundColor = Element.BackgroundColor != Color.Default ? Element.BackgroundColor : Color.Transparent;
            Container.ModifyBg(StateType.Normal, backgroundColor.ToGtkColor());
            Control?.SetBackgroundColor(backgroundColor.ToGtkColor());
        }

        private void UpdateTextColor()
        {
            var textColor = Element.TextColor != Color.Default ? Element.TextColor : Color.Black;
            Control?.SetForegroundColor(textColor.ToGtkColor());
        }

        public sealed class GtkButtonWrapper : Gtk.Button
        {
            private EventBox _labelContainer;
            private Gtk.Label _label;

            public Gtk.Label LabelWidget => _label;

            public GtkButtonWrapper()
            {
                _labelContainer = new EventBox();
                _label = new Gtk.Label();
                _labelContainer.Child = _label;

                Add(_labelContainer);
                Relief = ReliefStyle.None;
                CanFocus = false;
            }

            public void SetBackgroundColor(Gdk.Color color)
            {
                _labelContainer.ModifyBg(StateType.Normal, color);
                _labelContainer.ModifyBg(StateType.Selected, color);
                _labelContainer.ModifyBg(StateType.Prelight, color);
                _labelContainer.ModifyBg(StateType.Active, color);
                _labelContainer.ModifyBg(StateType.Insensitive, color);
            }

            public void SetForegroundColor(Gdk.Color color)
            {
                _label.ModifyFg(StateType.Normal, color);
                _label.ModifyFg(StateType.Selected, color);
                _label.ModifyFg(StateType.Prelight, color);
                _label.ModifyFg(StateType.Active, color);
                _label.ModifyFg(StateType.Insensitive, color);
            }

            public override void Dispose()
            {
                base.Dispose();

                _labelContainer = null;
                _label = null;
            }
        }
    }
}