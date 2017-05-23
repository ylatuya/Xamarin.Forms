using Gtk;
using System.Collections.Generic;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class TableView : ScrolledWindow
    {
        private VBox _root;
        private TableRoot _source;
        private List<Container> _cells;

        public TableView()
        {
            BuildTableView();
        }

        public TableRoot Root
        {
            get
            {
                return _source;
            }
            set
            {
                if (_source != value)
                {
                    _source = value;
                    RefreshSource(_source);
                }
            }
        }

        public void SetBackgroundColor(Gdk.Color backgroundColor)
        {
            Child?.ModifyBg(StateType.Normal, backgroundColor);
        }

        public void SetRowHeight(int rowHeight)
        {
            foreach (var cell in _cells)
            {
                cell.HeightRequest = rowHeight;
            }
        }

        private void BuildTableView()
        {
            CanFocus = true;
            ShadowType = ShadowType.None;
            HscrollbarPolicy = PolicyType.Never;
            VscrollbarPolicy = PolicyType.Automatic;
            BorderWidth = 0;

            _root = new VBox(false, 0);

            Viewport viewPort = new Viewport();
            viewPort.ShadowType = ShadowType.None;
            viewPort.Add(_root);

            Add(viewPort);

            _cells = new List<Container>();
        }

        private void RefreshSource(TableRoot source)
        {
            if (!string.IsNullOrEmpty(source.Title))
            {
                // Add Title
                var titleSpan = new Span()
                {
                    FontSize = 16,
                    Text = source.Title
                };

                Gtk.Label title = new Gtk.Label();
                title.SetAlignment(0, 0);
                title.SetTextFromSpan(titleSpan);
                _root.PackStart(title, false, false, 0);
            }

            // Add Table Section
            for (int i = 0; i < source.Count; i++)
            {
                var tableSection = source[i] as TableSection;

                if (tableSection != null)
                {
                    var tableSectionSpan = new Span()
                    {
                        FontSize = 12,
                        Text = tableSection.Title
                    };

                    // Table Section Title
                    Gtk.Label sectionTitle = new Gtk.Label();
                    sectionTitle.SetAlignment(0, 0);
                    sectionTitle.SetTextFromSpan(tableSectionSpan);
                    _root.PackStart(sectionTitle, false, false, 0);

                    // Table Section Separator
                    EventBox separator = new EventBox();
                    separator.HeightRequest = 1;
                    separator.ModifyBg(StateType.Normal, Color.Black.ToGtkColor());
                    _root.PackStart(separator, false, false, 0);

                    // Cells
                    _cells.Clear();
                    for (int j = 0; j < tableSection.Count; j++)
                    {
                        var cell = tableSection[j];

                        var renderer =
                            (Cells.CellRenderer)Internals.Registrar.Registered.GetHandler<IRegisterable>(cell.GetType());
                        var nativeCell = renderer.GetCell(cell, null, null);

                        _cells.Add(nativeCell);
                    }

                    foreach (var cell in _cells)
                    {
                        _root.PackStart(cell, false, false, 0);
                    }
                }
            }
        }
    }
}