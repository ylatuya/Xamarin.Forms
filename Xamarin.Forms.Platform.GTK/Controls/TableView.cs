using Gtk;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class TableView : ScrolledWindow
    {
        private VBox _root;
        private VBox _list;
        private TableRoot _source;

        public TableView()
        {
            BuildTableView();
        }

        public TableRoot Source
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

        private void BuildTableView()
        {
            CanFocus = true;
            ShadowType = ShadowType.None;
            HscrollbarPolicy = PolicyType.Never;
            VscrollbarPolicy = PolicyType.Automatic;

            _root = new VBox();

            // List
            _list = new VBox();
            _root.PackStart(_list, true, true, 0);

            Viewport viewPort = new Viewport();
            viewPort.ShadowType = ShadowType.None;
            viewPort.Add(_root);

            Add(viewPort);
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
                _list.PackStart(title, false, false, 0);

                // Add Table Section
                for (int i = 0; i < source.Count; i++)
                {
                    var tableSection = source[i] as TableSection;

                    if (tableSection != null)
                    {
                        // Table Section Title
                        Gtk.Label sectionTitle = new Gtk.Label();
                        sectionTitle.SetAlignment(0, 0);
                        sectionTitle.Text = tableSection.Title;
                        _list.PackStart(sectionTitle, false, false, 0);

                        // Table Section Separator
                        EventBox separator = new EventBox();
                        separator.HeightRequest = 1;
                        separator.ModifyBg(StateType.Normal, Color.Black.ToGtkColor());
                        _list.PackStart(sectionTitle, true, true, 0);

                        // Cells
                        for (int j = 0; j < tableSection.Count; j++)
                        {
                            var cell = tableSection[j];

                            var renderer =
                                (Cells.CellRenderer)Internals.Registrar.Registered.GetHandler<IRegisterable>(cell.GetType());
                            var nativeCell = renderer.GetCell(cell, null, null);

                            _list.PackStart(nativeCell, false, false, 0);
                        }
                    }
                }
            }
        }
    }
}