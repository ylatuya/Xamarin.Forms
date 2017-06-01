using Gdk;
using Gtk;
using System;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public sealed class ImageButton : Gtk.Button
    {
        private HBox _centralRowContainer;
        private Box _centralCellContainer;

        private Gdk.Color _defaultBorderColor;
        private Gdk.Color _defaultBackgroundColor;
        private Gdk.Color? _borderColor;
        private Gdk.Color? _backgroundColor;

        private Gtk.Image _image;
        private Gtk.Label _label;
        private uint _imageSpacing = 0;
        private uint _borderWidth = 0;

        public ImageButton()
        {
            _defaultBackgroundColor = Style.Backgrounds[(int)StateType.Normal];
            _defaultBorderColor = Style.BaseColors[(int)StateType.Active];

            _image = new Gtk.Image();
            _label = new Gtk.Label();

            var cellsContainer = new VBox();
            cellsContainer.PackStart(new HBox(), true, true, 0);
            _centralRowContainer = new HBox();
            _centralCellContainer = new HBox();
            _centralRowContainer.PackStart(new HBox(), true, true, 0);
            _centralRowContainer.PackStart(_centralCellContainer);
            _centralRowContainer.PackStart(new HBox(), true, true, 0);
            cellsContainer.PackStart(_centralRowContainer);
            cellsContainer.PackStart(new HBox(), true, true, 0);

            Relief = ReliefStyle.None;
            CanFocus = false;

            Add(cellsContainer);

            RecreateContainer();
        }

        public Gtk.Label LabelWidget => _label;

        public Gtk.Image ImageWidget => _image;

        public uint ImageSpacing
        {
            get
            {
                return _imageSpacing;
            }

            set
            {
                _imageSpacing = value;
                UpdateImageSpacing();
            }
        }

        public void SetBackgroundColor(Gdk.Color? color)
        {
            _backgroundColor = color;
            QueueDraw();
        }

        public void ResetBackgroundColor()
        {
            _backgroundColor = _defaultBackgroundColor;
            QueueDraw();
        }

        public void SetForegroundColor(Gdk.Color color)
        {
            _label.ModifyFg(StateType.Normal, color);
            _label.ModifyFg(StateType.Prelight, color);
            _label.ModifyFg(StateType.Active, color);
        }

        public void SetBorderWidth(uint width)
        {
            _borderWidth = width;
            QueueDraw();
        }

        public void SetBorderColor(Gdk.Color? color)
        {
            _borderColor = color;
            QueueDraw();
        }

        public void ResetBorderColor()
        {
            _borderColor = _defaultBorderColor;
            QueueDraw();
        }

        public void SetImagePosition(PositionType position)
        {
            ImagePosition = position;
            RecreateContainer();
        }

        public void SetImageFromFile(string fileName)
        {
            try
            {
                var iconPixBuf = new Pixbuf(fileName);
                ImageWidget.Pixbuf = iconPixBuf;
            }
            catch (Exception ex)
            {
                Internals.Log.Warning("Image Loading", $"Image failed to load: {ex}");
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _label = null;
            _image = null;
            _centralCellContainer = null;
            _centralRowContainer = null;
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);
        }

        protected override bool OnExposeEvent(EventExpose evnt)
        {
            double colorMaxValue = 65535;

            using (var cr = CairoHelper.Create(GdkWindow))
            {
                cr.Rectangle(Allocation.Left, Allocation.Top, Allocation.Width, Allocation.Height);

                if (_backgroundColor.HasValue)
                {
                    var color = _backgroundColor.Value;
                    cr.SetSourceRGBA(color.Red / colorMaxValue, color.Green / colorMaxValue, color.Blue / colorMaxValue, 1.0);
                    cr.FillPreserve();
                }

                if (_borderColor.HasValue)
                {
                    cr.LineWidth = _borderWidth;

                    var color = _borderColor.Value;
                    cr.SetSourceRGB(color.Red / colorMaxValue, color.Green / colorMaxValue, color.Blue / colorMaxValue);
                    cr.Stroke();
                }
            }

            return base.OnExposeEvent(evnt);
        }

        private void RecreateContainer()
        {
            if (_centralCellContainer != null)
            {
                _centralCellContainer.RemoveFromContainer(_image);
                _centralCellContainer.RemoveFromContainer(_label);
                _centralRowContainer.RemoveFromContainer(_centralCellContainer);
                _centralCellContainer = null;
            }

            switch (ImagePosition)
            {
                case PositionType.Left:
                    _centralCellContainer = new HBox();
                    _centralCellContainer.PackStart(_image, false, false, _imageSpacing);
                    _centralCellContainer.PackStart(_label, false, false, 0);
                    break;
                case PositionType.Top:
                    _centralCellContainer = new VBox();
                    _centralCellContainer.PackStart(_image, false, false, _imageSpacing);
                    _centralCellContainer.PackStart(_label, false, false, 0);
                    break;
                case PositionType.Right:
                    _centralCellContainer = new HBox();
                    _centralCellContainer.PackStart(_label, false, false, 0);
                    _centralCellContainer.PackStart(_image, false, false, _imageSpacing);
                    break;
                case PositionType.Bottom:
                    _centralCellContainer = new VBox();
                    _centralCellContainer.PackStart(_label, false, false, 0);
                    _centralCellContainer.PackStart(_image, false, false, _imageSpacing);
                    break;
            }

            if (_centralCellContainer != null)
            {
                _centralRowContainer.PackStart(_centralCellContainer, false, false, 0);
                _centralRowContainer.ReorderChild(_centralCellContainer, 1);
                _centralRowContainer.ShowAll();
            }
        }

        private void UpdateImageSpacing()
        {
            _centralCellContainer.SetChildPacking(_image, false, false, _imageSpacing, PackType.Start);
        }
    }
}
