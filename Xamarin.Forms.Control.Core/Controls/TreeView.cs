using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace GtkToolkit.Controls
{
    public class Node
    {
        public Node()
        {
            Children = new ObservableCollection<Node>();
        }

        public string Name { get; set; }

        public ObservableCollection<Node> Children { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class TreeView : ContentView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(ObservableCollection<Node>), typeof(TreeView), null);

        public ObservableCollection<Node> ItemsSource
        {
            get { return (ObservableCollection<Node>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly BindableProperty RowHeightProperty = 
            BindableProperty.Create(nameof(RowHeight), typeof(int), typeof(TreeView), default(int));

        public int RowHeight
        {
            get { return (int)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }
    }
}