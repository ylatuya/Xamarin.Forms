﻿using System;
using Gdk;
using Gtk;
using Pango;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class EntryWrapper : EventBox
    {
        private Gtk.Table _table;
        private Gtk.Entry _entry;
        private Gtk.Label _placeholder;
        private Gtk.EventBox _placeholderContainer;
        private bool _isEnabled;

        public EntryWrapper()
        {
            _table = new Table(1, 1, true);
            _entry = new Gtk.Entry();
            _entry.FocusOutEvent += EntryFocusedOut;
            _entry.Changed += EntryChanged;
            _placeholder = new Gtk.Label();

            _placeholderContainer = new EventBox();
            _placeholderContainer.BorderWidth = 2;
            _placeholderContainer.Add(_placeholder);
            _placeholderContainer.ButtonPressEvent += PlaceHolderContainerPressed;

            SetBackgroundColor(_entry.Style.BaseColors[(int)StateType.Normal]);

            Add(_table);

            _table.Attach(_entry, 0, 1, 0, 1);
            _table.Attach(_placeholderContainer, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
        }

        public Gtk.Entry Entry => _entry;

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                UpdateIsEnabled();
            }
        }

        public string PlaceholderText
        {
            get
            {
                return _placeholder.Text;
            }
            set
            {
                _placeholder.Text = value;
            }
        }

        public void SetBackgroundColor(Gdk.Color color)
        {
            ModifyBg(StateType.Normal, color);
            _entry.ModifyBase(StateType.Normal, color);
            _placeholderContainer.ModifyBg(StateType.Normal, color);
        }

        public void SetTextColor(Gdk.Color color)
        {
            _entry.ModifyText(StateType.Normal, color);
        }

        public void SetPlaceholderTextColor(Gdk.Color color)
        {
            _placeholder.ModifyFg(StateType.Normal, color);
        }

        public void SetAlignment(float aligmentValue)
        {
            _entry.Alignment = aligmentValue;
            _placeholder.SetAlignment(aligmentValue, 0.5f);
        }

        public void SetFont(FontDescription fontDescription)
        {
            _entry.ModifyFont(fontDescription);
            _placeholder.ModifyFont(fontDescription);
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);

            ShowPlaceholderIfNeeded();
        }

        private void ShowPlaceholderIfNeeded()
        {
            if (string.IsNullOrEmpty(_entry.Text) && !string.IsNullOrEmpty(_placeholder.Text))
            {
                Entry.Sensitive = false;
                _placeholderContainer.GdkWindow?.Raise();
            }
            else
            {
                _entry.GdkWindow?.Raise();
            }
        }

        private void UpdateIsEnabled()
        {
            Entry.IsEditable = _isEnabled;
            Entry.CanFocus = _isEnabled;
            Entry.Sensitive = _isEnabled;
            _placeholderContainer.State = _isEnabled ? StateType.Normal : StateType.Insensitive;
        }

        private void PlaceHolderContainerPressed(object o, ButtonPressEventArgs args)
        {
            if (IsEnabled)
            {
                Entry.Sensitive = true;
                Entry.HasFocus = true;
                _entry.GdkWindow?.Raise();
            }
        }

        private void EntryFocusedOut(object o, FocusOutEventArgs args)
        {
            ShowPlaceholderIfNeeded();
        }

        private void EntryChanged(object sender, EventArgs e)
        {
            ShowPlaceholderIfNeeded();
        }
    }
}