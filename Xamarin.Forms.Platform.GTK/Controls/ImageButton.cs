using Gtk;
using static Xamarin.Forms.Button;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public sealed class ImageButton : Gtk.Button
    {
        private EventBox _container;
        private HBox _centralRowContainer;
        private Box _centralCellContainer;
        private Gtk.Image _image;
        private Gtk.Label _label;

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

        public void ApplyContent(ButtonContentLayout layout)
        {
            RecreateContainer(layout);
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

        private void RecreateContainer(ButtonContentLayout layout)
        {
            if (_centralCellContainer != null)
            {
                _centralCellContainer.Remove(_image);
                _centralCellContainer.Remove(_label);
                _centralRowContainer.Remove(_centralCellContainer);
                _centralCellContainer = null;
            }

            switch (layout.Position)
            {
                case ButtonContentLayout.ImagePosition.Left:
                    _centralCellContainer = new HBox();
                    _centralCellContainer.PackStart(_image, false, false, (uint)layout.Spacing);
                    _centralCellContainer.PackStart(_label, false, false, 0);
                    break;
                case ButtonContentLayout.ImagePosition.Top:
                    _centralCellContainer = new VBox();
                    _centralCellContainer.PackStart(_image, false, false, (uint)layout.Spacing);
                    _centralCellContainer.PackStart(_label, false, false, 0);
                    break;
                case ButtonContentLayout.ImagePosition.Right:
                    _centralCellContainer = new HBox();
                    _centralCellContainer.PackStart(_label, false, false, 0);
                    _centralCellContainer.PackStart(_image, false, false, (uint)layout.Spacing);
                    break;
                case ButtonContentLayout.ImagePosition.Bottom:
                    _centralCellContainer = new VBox();
                    _centralCellContainer.PackStart(_label, false, false, 0);
                    _centralCellContainer.PackStart(_image, false, false, (uint)layout.Spacing);
                    break;
            }

            if (_centralCellContainer != null)
            {
                _centralRowContainer.PackStart(_centralCellContainer, false, false, 0);
                _centralRowContainer.ReorderChild(_centralCellContainer, 1);
                _centralRowContainer.ShowAll();
            }
        }
    }
}
