﻿using Xamarin.Forms;

namespace Xamarin.Forms.Controls.CustomControls
{
    public partial class CustomControlsPage : ContentPage
    {
        public CustomControlsPage()
        {
            InitializeComponent();

            ColorPickerBtn.Clicked += (sender, args) => Navigation.PushAsync(new ColorPickerPage());
            ChartsBtn.Clicked += (sender, args) => Navigation.PushAsync(new ChartPage());
            DataGridBtn.Clicked += (sender, args) => Navigation.PushAsync(new DataGridPage());
            DateTimePickerBtn.Clicked += (sender, args) => Navigation.PushAsync(new DateTimePickerPage());
            ExpanderBtn.Clicked += (sender, args) => Navigation.PushAsync(new ExpanderPage());
            GridSplitterBtn.Clicked += (sender, args) => Navigation.PushAsync(new GridSplitterPage());
            HyperLinkBtn.Clicked += (sender, args) => Navigation.PushAsync(new HyperLinkPage());
            ImageCheckBoxBtn.Clicked += (sender, args) => Navigation.PushAsync(new ImageCheckBoxPage());
            SeparatorBtn.Clicked += (sender, args) => Navigation.PushAsync(new SeparatorPage());
        }
    }
}