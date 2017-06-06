using Gtk;
using System;
using Xamarin.Forms.Platform.GTK.Controls;
using Xamarin.Forms.Platform.GTK.Helpers;

namespace GtkToolkit.GTK.Controls
{
    public class DateTimeEventArgs : EventArgs
    {
        private DateTime _datetime;

        public DateTime DateTime
        {
            get
            {
                return _datetime;
            }
        }

        public DateTimeEventArgs(DateTime datetime)
        {
            _datetime = datetime;
        }
    }

    public partial class DateTimePickerWindow : Window
    {
        private HBox _datebox;
        private VBox _calendarBox;
        private RangeCalendar _calendar;
        private HBox _clearBox;
        private Button _btnClear;
        private VBox _clockBox;
        private AnalogClock _clock;
        private HBox _timeBox;
        private Label _labelHour;
        private SpinButton _txtHour;
        private Label _labelMin;
        private SpinButton _txtMin;
        private Label _labelSec;
        private SpinButton _txtSec;

		public DateTimePickerWindow(DateTime datetime)
            : base(WindowType.Popup)
        {
            BuildDatePickerWindow();
			GrabHelper.GrabWindow(this);

			_calendar.Date = datetime;
			_txtHour.Value = datetime.Hour;
			_txtMin.Value = datetime.Minute;
			_txtSec.Value = datetime.Second;

			RefreshClock();
			_clock.Start();
		}

		public DateTime SelectedDateTime
		{
			get
			{
				DateTime date = _calendar.Date;

				return new DateTime(date.Year, date.Month, date.Day, (int)_txtHour.Value, (int)_txtMin.Value, (int)_txtSec.Value);
			}
		}

		public DateTime MinimumDate
        {
            get
            {
                return _calendar.MinimumDate;
            }

            set
            {
                _calendar.MinimumDate = value;
            }
        }

        public DateTime MaximumDate
        {
            get
            {
                return _calendar.MaximumDate;
            }

            set
            {
                _calendar.MaximumDate = value;
            }
        }

        public delegate void DateTimeEventHandler(object sender, DateTimeEventArgs args);

        public event DateTimeEventHandler OnDateTimeChanged;

        protected override bool OnExposeEvent(Gdk.EventExpose args)
        {
            base.OnExposeEvent(args);

            int winWidth, winHeight;
            GetSize(out winWidth, out winHeight);
            GdkWindow.DrawRectangle(
                Style.ForegroundGC(StateType.Insensitive), false, 0, 0, winWidth - 1, winHeight - 1);

            return false;
        }

        protected virtual void OnButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            Close();
        }

        protected virtual void OnCalendarButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            args.RetVal = true;
        }

        protected virtual void OnCalendarDaySelected(object sender, EventArgs e)
        {
            OnDateTimeChanged?.Invoke(this, new DateTimeEventArgs(SelectedDateTime));
        }

        protected virtual void OnCalendarDaySelectedDoubleClick(object sender, EventArgs e)
        {
            OnDateTimeChanged?.Invoke(this, new DateTimeEventArgs(SelectedDateTime));
            Close();
        }

		protected virtual void OnBtnClearClicked(object sender, EventArgs e)
		{
			_calendar.Date = DateTime.Now;
			_txtHour.Value = DateTime.Now.Hour;
			_txtMin.Value = DateTime.Now.Minute;
			_txtSec.Value = DateTime.Now.Second;
			RefreshClock();
			_clock.Start();
		}

		protected virtual void OnTxtHourValueChanged(object sender, System.EventArgs e)
		{
			_clock.Stop();
			if (_txtHour.Value == 24) _txtHour.Value = 0;
			RefreshClock();
		}

		protected virtual void OnTxtMinValueChanged(object sender, System.EventArgs e)
		{
			_clock.Stop();
			if (_txtMin.Value == 60) _txtMin.Value = 0;
			RefreshClock();
		}

		protected virtual void OnTxtSecValueChanged(object sender, System.EventArgs e)
		{
			_clock.Stop();
			if (_txtSec.Value == 60) _txtSec.Value = 0;
			RefreshClock();
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

		private void RefreshClock()
		{
			var date = _calendar.Date;
			int hour = (int)_txtHour.Value;
			int min = (int)_txtMin.Value;
			int sec = (int)_txtSec.Value;
			DateTime dt = new DateTime(date.Year, date.Month, date.Day, hour, min, sec);
			_clock.Datetime = dt;
			OnDateTimeChanged?.Invoke(this, new DateTimeEventArgs(SelectedDateTime));
		}

		private void BuildDatePickerWindow()
        {
            Name = "DateTimePicker";
            TypeHint = Gdk.WindowTypeHint.Menu;
            WindowPosition = WindowPosition.CenterOnParent;
            BorderWidth = 1;
            Resizable = false;
            AllowGrow = false;
            Decorated = false;
            DestroyWithParent = true;
            SkipPagerHint = true;
            SkipTaskbarHint = true;

			_datebox = new HBox()
			{
				Spacing = 6,
				BorderWidth = 3
			};

			_calendarBox = new VBox()
			{
				Spacing = 6
			};

			_calendar = new RangeCalendar()
			{
				CanFocus = true,
				DisplayOptions = ((CalendarDisplayOptions)(3))
			};
			_calendarBox.Add(_calendar);

            Box.BoxChild calendarBoxChild = ((Box.BoxChild)(_calendarBox[_calendar]));
			calendarBoxChild.Position = 0;

			_clearBox = new HBox()
			{
				Homogeneous = true,
				Spacing = 6
			};

			_btnClear = new Button()
			{
				CanFocus = true,
				UseUnderline = true,
				Label = "Now"
			};
			_clearBox.Add(_btnClear);

            Box.BoxChild btnClearBoxChild = ((Box.BoxChild)(_clearBox[_btnClear]));
			btnClearBoxChild.Position = 0;
			_calendarBox.Add(_clearBox);

            Box.BoxChild clearBoxChild = ((Box.BoxChild)(_calendarBox[_clearBox]));
			clearBoxChild.Position = 1;
			clearBoxChild.Expand = false;
			clearBoxChild.Fill = false;
			_datebox.Add(_calendarBox);

            Box.BoxChild calendarBoxBoxChild = ((Box.BoxChild)(_datebox[_calendarBox]));
			calendarBoxBoxChild.Position = 0;
			calendarBoxBoxChild.Expand = false;
			calendarBoxBoxChild.Fill = false;

			_clockBox = new VBox()
			{
				Spacing = 6
			};

			_clock = new AnalogClock()
			{
				Datetime = new DateTime(0)
			};

			_clockBox.Add(_clock);
            Box.BoxChild clockBoxChild = ((Box.BoxChild)(_clockBox[_clock]));
			clockBoxChild.Position = 0;

			_timeBox = new HBox()
			{
				Spacing = 6
			};

			_labelHour = new Label()
			{
				LabelProp = "H:"
			};
			_timeBox.Add(_labelHour);

            Box.BoxChild labelHourBoxChild = ((Box.BoxChild)(_timeBox[_labelHour]));
			labelHourBoxChild.Position = 0;
			labelHourBoxChild.Expand = false;
			labelHourBoxChild.Fill = false;

			_txtHour = new SpinButton(0D, 24D, 1D)
			{
				CanFocus = true
			};

			_txtHour.Adjustment.PageIncrement = 1D;
            _txtHour.ClimbRate = 1D;
            _txtHour.Numeric = true;
			_timeBox.Add(_txtHour);

            Box.BoxChild txtHourBoxChild = ((Box.BoxChild)(_timeBox[_txtMin]));
			txtHourBoxChild.Position = 1;
			txtHourBoxChild.Expand = false;
			txtHourBoxChild.Fill = false;

			_labelMin = new Label()
			{
				LabelProp = "M:"
			};
			_timeBox.Add(_labelMin);

            Box.BoxChild labelMinBoxChild = ((Box.BoxChild)(_timeBox[_labelMin]));
			labelMinBoxChild.Position = 2;
			labelMinBoxChild.Expand = false;
			labelMinBoxChild.Fill = false;

			_txtMin = new SpinButton(0D, 60D, 1D)
			{
				CanFocus = true
			};

			_txtMin.Adjustment.PageIncrement = 10D;
            _txtMin.ClimbRate = 1D;
            _txtMin.Numeric = true;
			_timeBox.Add(_txtMin);

            Box.BoxChild txtMinBoxChild = ((Box.BoxChild)(_timeBox[_txtMin]));
			txtMinBoxChild.Position = 3;
			txtMinBoxChild.Expand = false;
			txtMinBoxChild.Fill = false;

			_labelSec = new Label()
			{
				LabelProp = "S:"
			};
			_timeBox.Add(_labelSec);

            Box.BoxChild labelSecBoxChild = ((Box.BoxChild)(_timeBox[_labelSec]));
			labelSecBoxChild.Position = 4;
			labelSecBoxChild.Expand = false;
			labelSecBoxChild.Fill = false;

			_txtSec = new SpinButton(0D, 60D, 1D)
			{
				CanFocus = true
			};

			_txtSec.Adjustment.PageIncrement = 10D;
            _txtSec.ClimbRate = 1D;
            _txtSec.Numeric = true;
			_timeBox.Add(_txtSec);

            Box.BoxChild txtSecBoxChild = ((Box.BoxChild)(_timeBox[_txtSec]));
			txtSecBoxChild.Position = 5;
			txtSecBoxChild.Expand = false;
			txtSecBoxChild.Fill = false;
			_clockBox.Add(_timeBox);

            Box.BoxChild timeBoxChild = ((Box.BoxChild)(_clockBox[_timeBox]));
			timeBoxChild.Position = 1;
			timeBoxChild.Expand = false;
			_datebox.Add(_clockBox);

            Box.BoxChild dateBoxChild = ((Box.BoxChild)(_datebox[_clockBox]));
			dateBoxChild.Position = 1;
			dateBoxChild.Expand = false;
			dateBoxChild.Fill = false;
            Add(_datebox);

            if ((Child != null))
            {
                Child.ShowAll();
            }

            DefaultWidth = 500;
            DefaultHeight = 300;

            Show();

			ButtonPressEvent += new ButtonPressEventHandler(OnButtonPressEvent);
			_calendar.ButtonPressEvent += new ButtonPressEventHandler(OnCalendarButtonPressEvent);
			_calendar.DaySelected += new EventHandler(OnCalendarDaySelected);
			_calendar.DaySelectedDoubleClick += new EventHandler(OnCalendarDaySelectedDoubleClick);
			_btnClear.Clicked += new EventHandler(OnBtnClearClicked);
			_txtHour.ValueChanged += new EventHandler(OnTxtHourValueChanged);
			_txtHour.ButtonPressEvent += new ButtonPressEventHandler(OnTxtHourButtonPressEvent);
			_txtMin.ValueChanged += new EventHandler(OnTxtMinValueChanged);
			_txtMin.ButtonPressEvent += new ButtonPressEventHandler(OnTxtMinButtonPressEvent);
			_txtSec.ValueChanged += new EventHandler(OnTxtSecValueChanged);
			_txtSec.ButtonPressEvent += new ButtonPressEventHandler(OnTxtSecButtonPressEvent);
		}

		private void Close()
		{
			GrabHelper.RemoveGrab(this);
			Destroy();
        }

        private void NotifyDateChanged()
        {
            OnDateTimeChanged?.Invoke(this, new DateTimeEventArgs(SelectedDateTime));
        }

        class RangeCalendar : Calendar
        {
            private DateTime _minimumDate;
            private DateTime _maximumDate;

            public RangeCalendar()
            {
                _minimumDate = new DateTime(1900, 1, 1);
                _maximumDate = new DateTime(2199, 1, 1);
            }

            public DateTime MinimumDate
            {
                get
                {
                    return _minimumDate;
                }

                set
                {
                    if (MaximumDate < value)
                    {
                        throw new InvalidOperationException($"{nameof(MinimumDate)} must be lower than {nameof(MaximumDate)}");
                    }

                    _minimumDate = value;
                }
            }

            public DateTime MaximumDate
            {
                get
                {
                    return _maximumDate;
                }

                set
                {
                    if (MinimumDate > value)
                    {
                        throw new InvalidOperationException($"{nameof(MaximumDate)} must be greater than {nameof(MinimumDate)}");
                    }

                    _maximumDate = value;
                }
            }

            protected override void OnDaySelected()
            {
                if (Date < MinimumDate)
                {
                    Date = MinimumDate;
                }

                if (Date > MaximumDate)
                {
                    Date = MaximumDate;
                }
            }
        }
    }

    public partial class DateTimePicker : EventBox
    {
        private CustomComboBox _comboBox;
        private DateTime _currentDateTime;
        private string _dateFormat;

        public event EventHandler DateChanged;

        public DateTimePicker()
        {
            BuildDateTimePicker();

            CurrentDateTime = DateTime.Now;

            _comboBox.Entry.CanDefault = false;
            _comboBox.Entry.CanFocus = false;
            _comboBox.Entry.IsEditable = false;
            _comboBox.Entry.State = StateType.Normal;
            _comboBox.Entry.FocusGrabbed += new EventHandler(OnEntryFocused);
            _comboBox.PopupButton.Clicked += new EventHandler(OnBtnShowCalendarClicked);
        }

        public DateTime CurrentDateTime
        {
            get
            {
                return _currentDateTime;
            }
            set
            {
                _currentDateTime = value;
                UpdateEntryText();
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
                UpdateEntryText();
            }
        }

        private void ShowPickerWindow()
        {
            int x = 0;
            int y = 0;

            GdkWindow.GetOrigin(out x, out y);
            y += Allocation.Height;

            var picker = new DateTimePickerWindow(CurrentDateTime);
            picker.Move(x, y);

            picker.OnDateTimeChanged += OnPopupDateTimeChanged;
            picker.Destroyed += OnPickerClosed;
        }

        private void OnPopupDateTimeChanged(object sender, DateTimeEventArgs args)
        {
            CurrentDateTime = args.DateTime;
            DateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void BuildDateTimePicker()
        {
            _comboBox = new CustomComboBox();
            Add(_comboBox);

            if ((Child != null))
            {
                Child.ShowAll();
            }

            Show();
        }

        private void UpdateEntryText()
        {
            _comboBox.Entry.Text = string.IsNullOrEmpty(_dateFormat)
                ? _currentDateTime.ToString()
                : _currentDateTime.ToString(_dateFormat);
        }

        private void OnBtnShowCalendarClicked(object sender, EventArgs e)
        {
            ShowPickerWindow();
        }

        private void OnEntryFocused(object sender, EventArgs e)
        {
            ShowPickerWindow();
        }

        private void OnPickerClosed(object sender, EventArgs e)
        {
            var window = sender as DatePickerWindow;

            if (window != null)
            {
                Remove(window);
            }
        }
    }
}