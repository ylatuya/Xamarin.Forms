using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;


#if UITEST
using NUnit.Framework;
using Xamarin.UITest.Queries;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	// This is similar to the test for 35477, but tests all of the basic controls to make sure that they all exhibit
	// the same behavior across all the platforms. The question is whether tapping a control inside of a frame
	// will trigger the frame's tap gesture; for most controls it will not (the control itself absorbs the tap),
	// but for non-interactive controls (box, frame, image, label) the gesture bubbles up to the container.

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 00100100, "Verify that the tap gesture bubbling behavior is consistent across the platforms", PlatformAffected.All)]
	public class GestureBubblingTests : TestNavigationPage
	{
		const string TargetAutomationId = "controlinsideofframe";
		ContentPage _menu;

#if UITEST
		[Test, TestCaseSource(nameof(TestCases))]
		public void VerifyTapBubbling(string menuItem, bool frameShouldRegisterTap)
		{
			// TODO hartez 2017/03/22 18:08:35 results are the same without basing in on the inputtransparent tests	
			// The stepper failure is because of the broken renderer, just account for the position in the test and it will work
			// Then fix Frame. Also, the instruction labels are wrong; adjust them based on the value of frameShouldRegisterTap

			var results = RunningApp.WaitForElement(q => q.Marked(menuItem));

			if (results.Length > 1)
			{
				var rect = results.First(r => r.Class.Contains("Button")).Rect;

				RunningApp.TapCoordinates(rect.CenterX, rect.CenterY);
			}
			else
			{
				RunningApp.Tap(q => q.Marked(menuItem));
			}

			// Find the start label
			RunningApp.WaitForElement(q => q.Marked("Start"));

			// Find the control we're testing
			var result = RunningApp.WaitForElement(q => q.Marked(TargetAutomationId));
			var target = result.First().Rect;

			// Tap the control
			var y = target.CenterY;
			var x = target.CenterX;

			// In theory we want to tap the center of the control. But Stepper lays out differently than the other controls,
			// so we need to adjust for it until someone fixes it
			if (menuItem == "Stepper")
			{
				y = target.Y + 5;
				x = target.X + 5;
			}

			RunningApp.TapCoordinates(x, y);

			if (menuItem == nameof(DatePicker) || menuItem == nameof(TimePicker))
			{
				// These controls show a pop-up which we have to cancel/done out of before we can continue
#if __ANDROID__
				var cancelButtonText = "Cancel";
#elif __IOS__
				var cancelButtonText = "Done";
#else
				var cancelButtonText = "X";
#endif
				RunningApp.WaitForElement(q => q.Marked(cancelButtonText));
				RunningApp.Tap(q => q.Marked(cancelButtonText));
			}

			if (frameShouldRegisterTap)
			{
				RunningApp.WaitForElement(q => q.Marked("Frame was tapped"));
			}
			else
			{
				RunningApp.WaitForElement(q => q.Marked("Start"));
			}
		}
#endif

		ContentPage CreateTestPage(View view)
		{
			var instructions = new Label
			{
				Text = "Tap the frame below. The label with the text 'No taps yet' should change its text to 'Frame was tapped'."
			};

			var label = new Label { Text = "Start" };

			var frame = new Frame();
			frame.Content = new StackLayout { Children = { view } };

			var rec = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
			rec.Tapped += (s, e) => { label.Text = "Frame was tapped"; };
			frame.GestureRecognizers.Add(rec);
			
			var layout = new StackLayout();
		
			layout.Children.Add(instructions);
			layout.Children.Add(label);
			layout.Children.Add(frame);

			return new ContentPage { Content = layout };
		}

		Button MenuButton(string label, Func<View> view)
		{
			var button = new Button { Text = label };

			var testView = view();
			testView.AutomationId = TargetAutomationId;

			button.Clicked += (sender, args) => PushAsync(CreateTestPage(testView));

			return button;
		}

		IEnumerable<object[]> TestCases
		{
			get
			{
				// These controls should allow the tap gesture to bubble up to their container; everything else should absorb the gesture
				List<string> controlsWhichShouldAllowTheTapToBubbleUp = new List<string> { nameof(Image), nameof(Label), nameof(BoxView), nameof(Frame) };

				return (BuildMenu().Content as Layout).InternalChildren.SelectMany(
					element => (element as Layout).InternalChildren.Select(view => new object[]
					{
						(view as Button).Text,
						controlsWhichShouldAllowTheTapToBubbleUp.Contains((view as Button).Text)
					}));
			}
		}

		ContentPage BuildMenu()
		{
			if (_menu != null)
			{
				return _menu;
			}

			var layout = new Grid
			{
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(), new ColumnDefinition() }
			};

			var col1 = new StackLayout();
			layout.Children.Add(col1);
			Grid.SetColumn(col1, 0);

			var col2 = new StackLayout();
			layout.Children.Add(col2);
			Grid.SetColumn(col2, 1);

			col1.Children.Add(MenuButton(nameof(Image), () => new Image { Source = ImageSource.FromFile("oasis.jpg") }));
			col1.Children.Add(MenuButton(nameof(Frame), () => new Frame { BackgroundColor = Color.DarkGoldenrod }));
			col1.Children.Add(MenuButton(nameof(Entry), () => new Entry()));
			col1.Children.Add(MenuButton(nameof(Editor), () => new Editor()));
			col1.Children.Add(MenuButton(nameof(Button), () => new Button { Text = "Test" }));
			col1.Children.Add(MenuButton(nameof(Label), () => new Label
			{
				LineBreakMode = LineBreakMode.WordWrap,
				Text = "Lorem ipsum dolor sit amet"
			}));
			col1.Children.Add(MenuButton(nameof(SearchBar), () => new SearchBar()));

			col2.Children.Add(MenuButton(nameof(DatePicker), () => new DatePicker()));
			col2.Children.Add(MenuButton(nameof(TimePicker), () => new TimePicker()));
			col2.Children.Add(MenuButton(nameof(Slider), () => new Slider()));
			col2.Children.Add(MenuButton(nameof(Switch), () => new Switch()));
			col2.Children.Add(MenuButton(nameof(Stepper), () => new Stepper()));
			col2.Children.Add(MenuButton(nameof(BoxView), () => new BoxView { BackgroundColor = Color.DarkMagenta, WidthRequest = 100, HeightRequest = 100 }));

			return new ContentPage { Content = layout };
		}

		protected override void Init()
		{
			PushAsync(BuildMenu());
		}
	}
}