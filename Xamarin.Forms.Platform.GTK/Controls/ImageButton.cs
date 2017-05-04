using System;
using Gtk;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public sealed class ImageButton : Gtk.Button
    {
        private EventBox _container;
        private HBox _centralRowContainer;
        private Box _centralCellContainer;
        private Gtk.Image _image;
        private Gtk.Label _label;
        private uint _imageSpacing = 0;

        public ImageButton()
        {
            _container = new EventBox();

            _image = new Gtk.Image();
            _label = new Gtk.Label();

            var cellsContainer = new VBox();
            _container.Add(cellsContainer);
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

            Add(_container);
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

        public void SetBackgroundColor(Gdk.Color color)
        {
            _container.ModifyBg(StateType.Normal, color);
            _container.ModifyBg(StateType.Selected, color);
            _container.ModifyBg(StateType.Prelight, color);
            _container.ModifyBg(StateType.Active, color);
            _container.ModifyBg(StateType.Insensitive, color);
        }

        public void SetForegroundColor(Gdk.Color color)
        {
            _label.ModifyFg(StateType.Normal, color);
            _label.ModifyFg(StateType.Prelight, color);
            _label.ModifyFg(StateType.Active, color);
        }

        public void SetBorderWidth(uint width)
        {
            _container.BorderWidth = width;
        }

        public void SetImagePosition(PositionType position)
        {
            ImagePosition = position;
            RecreateContainer();
        }

        public void SetImageFromFile(string fileName)
        {
            var iconPixBuf = new Gdk.Pixbuf(fileName);
            ImageWidget.Pixbuf = iconPixBuf;
        }

        public override void Dispose()
        {
            base.Dispose();

            _label = null;
            _image = null;
            _container = null;
            _centralCellContainer = null;
            _centralRowContainer = null;
        }

        private void RecreateContainer()
        {
            if (_centralCellContainer != null)
            {
                _centralCellContainer.Remove(_image);
                _centralCellContainer.Remove(_label);
                _centralRowContainer.Remove(_centralCellContainer);
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
