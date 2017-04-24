using System;
using System.Linq;
using Gtk;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class DateEventArgs : EventArgs
    {
        private DateTime _date;

        public DateTime Date
        {
            get
            {
                return _date;
            }
        }

        public DateEventArgs(DateTime date)
        {
            _date = date;
        }
    }

    public partial class DatePickerWindow : Window
    {
        private VBox _datebox;
        private Calendar _calendar;
        private HBox _timebox;
        private Gtk.Label _labelHour;
        private SpinButton _txtHour;
        private Gtk.Label _labelMin;
        private SpinButton _txtMin;
        private Gtk.Label _labelSec;
        private SpinButton _txtSec;

        public delegate void DateEventHandler(object sender, DateEventArgs args);

        public event DateEventHandler OnChange = null;

        public DatePickerWindow(int x, int y, DateTime defDate, DateEventHandler handler) : base(WindowType.Popup)
        {
            BuildDatePickerWindow();
            Move(x, y);
            OnChange = handler;
            Helpers.GrabHelper.GrabWindow(this);

            _txtHour.Value = defDate.Hour;
            _txtMin.Value = defDate.Minute;
            _txtSec.Value = defDate.Second;
            _calendar.Date = defDate;

            RefreshDate();
        }

        public static void ShowMe(int x, int y, DateTime defDate, DateEventHandler handler)
        {
            new DatePickerWindow(x, y, defDate, handler);
        }

        protected override bool OnExposeEvent(Gdk.EventExpose args)
        {
            base.OnExposeEvent(args);

            int winWidth, winHeight;
            GetSize(out winWidth, out winHeight);
            GdkWindow.DrawRectangle(
                Style.ForegroundGC(StateType.Insensitive), false, 0, 0, winWidth - 1, winHeight - 1);

            return false;
        }

        protected virtual void OnButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
        {
            Close();
        }

        private void Close()
        {
            Helpers.GrabHelper.RemoveGrab(this);
            this.Destroy();
        }

        protected virtual void OnCalendar3ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            args.RetVal = true;
        }

        private void RefreshDate()
        {
            OnChange?.Invoke(this, new DateEventArgs(CurrentDate));
        }

        protected virtual void OnTxtHourValueChanged(object sender, EventArgs e)
        {
            if (_txtHour.Value == 24) _txtHour.Value = 0;
            RefreshDate();
        }

        protected virtual void OnTxtMinValueChanged(object sender, EventArgs e)
        {
            if (_txtMin.Value == 60) _txtMin.Value = 0;
            RefreshDate();
        }

        protected virtual void OnTxtSecValueChanged(object sender, EventArgs e)
        {
            if (_txtSec.Value == 60) _txtSec.Value = 0;
            RefreshDate();
        }

        protected virtual void OnTxtHourButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            args.RetVal = true;
        }

        protected virtual void OnTxtMinButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            args.RetVal = true;
        }

        protected virtual void OnTxtSecButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            args.RetVal = true;
        }

        protected virtual void OnCalendar4ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            args.RetVal = true;
        }


        public DateTime CurrentDate
        {
            get
            {
                DateTime d = _calendar.Date;
                return new DateTime(d.Year, d.Month, d.Day, (int)_txtHour.Value, (int)_txtMin.Value, (int)_txtSec.Value);
            }
        }

        protected virtual void OnCCalendarDaySelected(object sender, EventArgs e)
        {
            OnChange?.Invoke(this, new DateEventArgs(CurrentDate));
        }

        protected virtual void OnCCalendarDaySelectedDoubleClick(object sender, EventArgs e)
        {
            OnChange?.Invoke(this, new DateEventArgs(CurrentDate));
            Close();
        }

        private void BuildDatePickerWindow()
        {
            Title = "DatePicker";
            TypeHint = Gdk.WindowTypeHint.Desktop;
            WindowPosition = WindowPosition.Mouse;
            BorderWidth = 1;
            Resizable = false;
            AllowGrow = false;
            Decorated = false;
            DestroyWithParent = true;
            SkipPagerHint = true;
            SkipTaskbarHint = true;

            _datebox = new VBox();
            _datebox.Spacing = 6;
            _datebox.BorderWidth = 3;

            _calendar = new Calendar();
            _calendar.CanFocus = true;
            _calendar.DisplayOptions = CalendarDisplayOptions.ShowHeading;
            _datebox.Add(_calendar);
            Box.BoxChild dateBoxChild = ((Box.BoxChild)(_datebox[_calendar]));
            dateBoxChild.Position = 0;

            _timebox = new HBox();
            _timebox.Spacing = 6;

            _labelHour = new Gtk.Label();
            _labelHour.LabelProp = "H:";
            _timebox.Add(_labelHour);
            Box.BoxChild labelHourBoxChild = ((Box.BoxChild)(_timebox[_labelHour]));
            labelHourBoxChild.Position = 0;
            labelHourBoxChild.Expand = false;
            labelHourBoxChild.Fill = false;

            _txtHour = new SpinButton(0D, 24D, 1D);
            _txtHour.CanFocus = true;
            _txtHour.Adjustment.PageIncrement = 1D;
            _txtHour.ClimbRate = 1D;
            _txtHour.Numeric = true;
            _timebox.Add(_txtHour);
            Box.BoxChild txtHourBoxChild = ((Box.BoxChild)(_timebox[_txtHour]));
            txtHourBoxChild.Position = 1;
            txtHourBoxChild.Expand = false;
            txtHourBoxChild.Fill = false;

            _labelMin = new Gtk.Label();
            _labelMin.LabelProp = "M:";
            _timebox.Add(_labelMin);
            Box.BoxChild labelMinBoxChild = ((Box.BoxChild)(_timebox[_labelMin]));
            labelMinBoxChild.Position = 2;
            labelMinBoxChild.Expand = false;
            labelMinBoxChild.Fill = false;

            _txtMin = new SpinButton(0D, 60D, 1D);
            _txtMin.CanFocus = true;
            _txtMin.Adjustment.PageIncrement = 10D;
            _txtMin.ClimbRate = 1D;
            _txtMin.Numeric = true;
            _timebox.Add(_txtMin);
            Box.BoxChild txtMinBoxChild = ((Box.BoxChild)(_timebox[_txtMin]));
            txtMinBoxChild.Position = 3;
            txtMinBoxChild.Expand = false;
            txtMinBoxChild.Fill = false;

            _labelSec = new Gtk.Label();
            _labelSec.LabelProp = "S:";
            _timebox.Add(_labelSec);
            Box.BoxChild labelSecBoxChild = ((Box.BoxChild)(_timebox[_labelSec]));
            labelSecBoxChild.Position = 4;
            labelSecBoxChild.Expand = false;
            labelSecBoxChild.Fill = false;

            _txtSec = new SpinButton(0D, 60D, 1D);
            _txtSec.CanFocus = true;
            _txtSec.Adjustment.PageIncrement = 10D;
            _txtSec.ClimbRate = 1D;
            _txtSec.Numeric = true;
            _timebox.Add(_txtSec);
            Box.BoxChild timeboxBoxChild = ((Box.BoxChild)(_timebox[_txtSec]));
            timeboxBoxChild.Position = 5;
            timeboxBoxChild.Expand = false;
            timeboxBoxChild.Fill = false;

            _datebox.Add(_timebox);

            Add(_datebox);

            if ((Child != null))
            {
                Child.ShowAll();
            }

            Show();

            ButtonPressEvent += new ButtonPressEventHandler(OnButtonPressEvent);
            _calendar.ButtonPressEvent += new ButtonPressEventHandler(OnCalendar4ButtonPressEvent);
            _calendar.DaySelected += new EventHandler(OnCCalendarDaySelected);
            _calendar.DaySelectedDoubleClick += new EventHandler(OnCCalendarDaySelectedDoubleClick);
            _txtHour.ValueChanged += new EventHandler(OnTxtHourValueChanged);
            _txtHour.ButtonPressEvent += new ButtonPressEventHandler(OnTxtHourButtonPressEvent);
            _txtMin.ValueChanged += new EventHandler(OnTxtMinValueChanged);
            _txtMin.ButtonPressEvent += new ButtonPressEventHandler(OnTxtMinButtonPressEvent);
            _txtSec.ValueChanged += new EventHandler(OnTxtSecValueChanged);
            _txtSec.ButtonPressEvent += new ButtonPressEventHandler(OnTxtSecButtonPressEvent);
        }
    }

    public partial class DatePicker : EventBox
    {
        private CustomComboBox _comboBox;
        private Gdk.Color _color;
        private DateTime _currentDate;
        private DateTime _minDate;
        private DateTime _maxDate;
        private string _dateFormat;

        public event EventHandler DateChanged;

        public DatePicker()
        {
            BuildDatePicker();

            CurrentDate = DateTime.Now;

            TextColor = _comboBox.Entry.Style.Text(StateType.Normal);

            _comboBox.Entry.Changed += new EventHandler(OnTxtDateChanged);
            _comboBox.PopupButton.Clicked += new EventHandler(OnBtnShowCalendarClicked);
        }

        public DateTime CurrentDate
        {
            get
            {
                return _currentDate;
            }
            set
            {
                _currentDate = value;
                _comboBox.Entry.Text = value.ToLongDateString();
            }
        }

        public DateTime MinDate
        {
            get
            {
                return _minDate;
            }
            set
            {
                _minDate = value;
            }
        }

        public DateTime MaxDate
        {
            get
            {
                return _maxDate;
            }
            set
            {
                _maxDate = value;
            }
        }

        public Gdk.Color TextColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                _comboBox.Color = _color;
            }
        }

        public string Text
        {
            get
            {
                return _comboBox.Entry.Text;
            }
        }

        public string DateFormat
        {
            get
            {
                return _dateFormat;
            }
            set
            {
                _dateFormat = value;
            }
        }

        public bool IsDateValid()
        {
            bool is_date_correct = true;

            try
            {
                IFormatProvider culture = System.Globalization.CultureInfo.CurrentCulture;
                CurrentDate = DateTime.Parse(_comboBox.Entry.Text, culture);
            }
            catch
            {
                is_date_correct = false;
            }

            return is_date_correct;
        }

        protected virtual void OnTxtDateChanged(object sender, EventArgs e)
        {
            _comboBox.Entry.ModifyText(StateType.Normal, TextColor);

            DateChanged?.Invoke(this, e);
        }

        protected virtual void OnBtnShowCalendarClicked(object sender, EventArgs e)
        {
            int x = 0;
            int y = 0;

            var window = RootWindow.Children.FirstOrDefault(c => c.IsVisible);
            if (window != null)
            {
                window.GetPosition(out x, out y);
            }

            x += Allocation.Left;
            y += Allocation.Top + Allocation.Height;
            DatePickerWindow.ShowMe(x, y, CurrentDate, OnPopupDateChanged);
        }

        private void OnPopupDateChanged(object sender, DateEventArgs args)
        {
            var date = args.Date;

            if (date < MinDate)
            {
                CurrentDate = MinDate;
                return;
            }

            if (date > MaxDate)
            {
                CurrentDate = MaxDate;
                return;
            }

            CurrentDate = args.Date;
        }

        private void BuildDatePicker()
        {
            _comboBox = new CustomComboBox();
            Add(_comboBox);

            if ((Child != null))
            {
                Child.ShowAll();
            }

            Show();
        }
    }
}