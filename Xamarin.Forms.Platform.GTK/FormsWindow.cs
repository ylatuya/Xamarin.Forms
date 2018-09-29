using Gtk;
using System;
using System.ComponentModel;
using System.Threading;

namespace Xamarin.Forms.Platform.GTK
{
	public class FormsWindow : Window
	{
		private Application _application;
		private Gdk.Size _lastSize;
		private VBox _content;
		private MenuBar _menu;
		private AccelGroup _accelGroup;

		public FormsWindow ()
			: base (WindowType.Toplevel)
		{
			SetDefaultSize (800, 600);
			SetSizeRequest (400, 400);

			MainThreadID = Thread.CurrentThread.ManagedThreadId;
			MainWindow = this;

			if (SynchronizationContext.Current == null)
				SynchronizationContext.SetSynchronizationContext (new GtkSynchronizationContext ());

			WindowStateEvent += OnWindowStateEvent;
			_content = new VBox();
			_menu = new MenuBar();
			_content.PackStart(_menu, false, true, 0);
			_content.ShowAll();
			Add(_content);
			_accelGroup = new AccelGroup();
			AddAccelGroup(_accelGroup);
		}

		/// <summary>
		/// Gets or sets the ID of the main thread where the GTK+
		/// mainloop is running.
		/// </summary>
		/// <value>The main thread identifier.</value>
		public static int MainThreadID { get; set; }

		/// <summary>
		/// Gets or sets the main window.
		/// </summary>
		/// <value>The main window.</value>
		public static FormsWindow MainWindow { get; set; }

		/// <summary>
		/// Gets the application menu bar.
		/// </summary>
		/// <value>The menu.</value>
		public MenuBar Menu => _menu;

		/// <summary>
		/// Gets the menu accel group.
		/// </summary>
		/// <value>The menu accel group.</value>
		public AccelGroup MenuAccelGroup => _accelGroup;

		/// <summary>
		/// Gets the content of the window.
		/// </summary>
		/// <value>The content.</value>
		public VBox Content => _content;

		public virtual void LoadApplication(Application application)
		{
			if (application == null)
				throw new ArgumentNullException(nameof(application));

			Application.SetCurrentApplication(application);
			_application = application;

			application.PropertyChanged += ApplicationOnPropertyChanged;
			UpdateMainPage();

			_application.SendStart();
		}

		public void SetApplicationTitle(string title)
		{
			if (string.IsNullOrEmpty(title))
				return;

			Title = title;
		}

		public void SetApplicationIcon(string icon)
		{
			if (string.IsNullOrEmpty(icon))
				return;

			var appliccationIconPixbuf = new Gdk.Pixbuf(icon);
			Icon = appliccationIconPixbuf;
		}

		public sealed override void Dispose()
		{
			base.Dispose();

			Dispose(true);
		}

		protected override bool OnDeleteEvent(Gdk.Event evnt)
		{
			base.OnDeleteEvent(evnt);

			Gtk.Application.Quit();

			return true;
		}

		private void ApplicationOnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == nameof(Application.MainPage))
			{
				UpdateMainPage();
			}
		}

		protected override bool OnConfigureEvent(Gdk.EventConfigure evnt)
		{
			Gdk.Size newSize = new Gdk.Size(evnt.Width, evnt.Height);

			if (_lastSize != newSize)
			{
				_lastSize = newSize;
				var pageRenderer = Platform.GetRenderer(_application.MainPage);
				pageRenderer?.SetElementSize(new Size(newSize.Width, newSize.Height));
			}

			return base.OnConfigureEvent(evnt);
		}

		private void UpdateMainPage()
		{
			if (_application.MainPage == null)
				return;

			var platformRenderer = Child as PlatformRenderer;

			if (platformRenderer != null)
			{
				platformRenderer.Destroy();
				((IDisposable)platformRenderer.Platform).Dispose();
			}

			var platform = new Platform();
			platform.PlatformRenderer.SetSizeRequest(WidthRequest, HeightRequest);
			_content.PackStart(platform.PlatformRenderer, true, true, 0);
			_content.ReorderChild(platform.PlatformRenderer, 1);
			platform.SetPage(_application.MainPage);

			Child.ShowAll();
		}

		private void OnWindowStateEvent(object o, WindowStateEventArgs args)
		{
			if (args.Event.ChangedMask == Gdk.WindowState.Iconified)
			{
				var windowState = args.Event.NewWindowState;

				if (windowState == Gdk.WindowState.Iconified)
					_application.SendSleep();
				else
					_application.SendResume();
			}
		}

		private void Dispose(bool disposing)
		{
			if (disposing && _application != null)
			{
				WindowStateEvent -= OnWindowStateEvent;
				_application.PropertyChanged -= ApplicationOnPropertyChanged;
			}
		}
	}
}