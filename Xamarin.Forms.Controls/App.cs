using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace Xamarin.Forms.Controls
{

    public class App : Application
    {
        public const string AppName = "XamarinFormsControls";
        static string s_insightsKey;

        // ReSharper disable once InconsistentNaming
        public static int IOSVersion = -1;

        public static List<string> AppearingMessages = new List<string>();

        static Dictionary<string, string> s_config;
        readonly ITestCloudService _testCloudService;

        public const string DefaultMainPageId = "ControlGalleryMainPage";

        public App()
        {
            _testCloudService = DependencyService.Get<ITestCloudService>();
            InitInsights();

            SetMainPage(CreateDefaultMainPage());

            //// Uncomment to verify that there is no gray screen displayed between the blue splash and red MasterDetailPage.
            //MainPage = new Bugzilla44596SplashPage(() =>
            //{
            //	var newTabbedPage = new TabbedPage();
            //	newTabbedPage.Children.Add(new ContentPage { BackgroundColor = Color.Red, Content = new Label { Text = "yay" } });
            //	MainPage = new MasterDetailPage
            //	{
            //		Master = new ContentPage { Title = "Master", BackgroundColor = Color.Red },
            //		Detail = newTabbedPage
            //	};
            //});
        }

        public Page CreateDefaultMainPage()
        {
            int counter = 0;

            ScrollView scrollView = new ScrollView();

            var stack = new StackLayout();
            stack.BackgroundColor = Color.White;

            ActivityIndicator activityIndicator = new ActivityIndicator();
            activityIndicator.IsRunning = true;
            activityIndicator.Color = Color.OrangeRed;

            Frame frame = new Frame();
            frame.OutlineColor = Color.Gold;

            Label label = new Label { Text = "-", BackgroundColor = Color.White };
            label.FontFamily = "Jokerman";
            frame.Content = label;
            
            Button button = new Button { Text = "Click me!" };
            button.BackgroundColor = Color.Red;
            button.Image = "coffee.png";
            button.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Right, 50);
            button.Clicked += (sender, args) =>
            {
                counter++;
                label.Text = string.Format("Button clicked {0} times", counter);
            };

            BoxView boxView = new BoxView();
            boxView.Color = Color.Pink;
            boxView.HeightRequest = 100;
            boxView.WidthRequest = 300;
            boxView.HorizontalOptions = LayoutOptions.Start;

            var entry = new Entry();
            entry.BackgroundColor = Color.LightSalmon;
            entry.TextColor = Color.Green;
            entry.Placeholder = "Placeholder";
            entry.Text = "Hello, I'm an entry";
            entry.FontFamily = "Jokerman";
            entry.FontSize = 32;
            entry.FontAttributes = FontAttributes.Italic;
            entry.HorizontalTextAlignment = TextAlignment.End;

            var image = new Image();
            image.Source = ImageSource.FromFile("coffee.png");
            image.HeightRequest = 100;
            image.WidthRequest = 100;

            Slider slider = new Slider();
            slider.Minimum = 0;
            slider.Maximum = 100;
            slider.Value = 50;

            Stepper stepper = new Stepper();
            stepper.Minimum = 0;
            stepper.Maximum = 100;
            stepper.Value = 33;

            TimePicker timePicker = new TimePicker();

            DatePicker datePicker = new DatePicker();

            Picker picker = new Picker();
            picker.ItemsSource = new List<string>
            {
                "Linux",
                "MacOS",
                "Windows"
            };

            ProgressBar progress = new ProgressBar();
            progress.Progress = 0.75;

            Switch switchControl = new Switch();

            ListView listView = new ListView();
            listView.BackgroundColor = Color.Violet;
            listView.HeightRequest = 150;
            listView.WidthRequest = 300;
            
            List<string> items = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                items.Add(i.ToString());
            }

            listView.ItemsSource = items;

            listView.ItemTemplate = new DataTemplate(() =>
            new ImageCell
            {
                Text = "Text",
                Detail = "Detail",
                ImageSource = ImageSource.FromFile("coffee.png")
            });

            stack.Children.Add(activityIndicator);
            stack.Children.Add(frame);
            stack.Children.Add(button);
            stack.Children.Add(boxView);
            stack.Children.Add(entry);
            stack.Children.Add(image);
            stack.Children.Add(slider);
            stack.Children.Add(stepper);
            stack.Children.Add(timePicker);
            stack.Children.Add(datePicker);
            stack.Children.Add(picker);
            stack.Children.Add(progress);
            stack.Children.Add(switchControl);
            stack.Children.Add(listView);

            scrollView.Content = stack;

            return new ContentPage
            {
                Content = scrollView
            };

            /*
			return new MasterDetailPage
			{
				AutomationId = DefaultMainPageId,
				Master = new ContentPage { Title = "Master", BackgroundColor = Color.Red },
				Detail = CoreGallery.GetMainPage()
			};
            */
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            var appDomain = "http://" + AppName.ToLowerInvariant() + "/";

            if (!uri.ToString().ToLowerInvariant().StartsWith(appDomain))
                return;

            var url = uri.ToString().Replace(appDomain, "");

            var parts = url.Split('/');
            if (parts.Length == 2)
            {
                var isPage = parts[0].Trim().ToLower() == "gallery";
                if (isPage)
                {
                    string page = parts[1].Trim();
                    var pageForms = Activator.CreateInstance(Type.GetType(page));

                    var appLinkPageGallery = pageForms as AppLinkPageGallery;
                    if (appLinkPageGallery != null)
                    {
                        appLinkPageGallery.ShowLabel = true;
                        (MainPage as MasterDetailPage)?.Detail.Navigation.PushAsync((pageForms as Page));
                    }
                }
            }

            base.OnAppLinkRequestReceived(uri);
        }

        public static Dictionary<string, string> Config
        {
            get
            {
                if (s_config == null)
                    LoadConfig();

                return s_config;
            }
        }

        public static string InsightsApiKey
        {
            get
            {
                if (s_insightsKey == null)
                {
                    string key = Config["InsightsApiKey"];
                    s_insightsKey = string.IsNullOrEmpty(key) ? Insights.DebugModeKey : key;
                }

                return s_insightsKey;
            }
        }

        public static ContentPage MenuPage { get; set; }

        public void SetMainPage(Page rootPage)
        {
            MainPage = rootPage;
        }

        static Assembly GetAssembly(out string assemblystring)
        {
            assemblystring = typeof(App).AssemblyQualifiedName.Split(',')[1].Trim();
            var assemblyname = new AssemblyName(assemblystring);
            return Assembly.Load(assemblyname);
        }

        void InitInsights()
        {
            if (Insights.IsInitialized)
            {
                Insights.ForceDataTransmission = true;
                if (_testCloudService != null && _testCloudService.IsOnTestCloud())
                    Insights.Identify(_testCloudService.GetTestCloudDevice(), "Name", _testCloudService.GetTestCloudDeviceName());
                else
                    Insights.Identify("DemoUser", "Name", "Demo User");
            }
        }

        static void LoadConfig()
        {
            s_config = new Dictionary<string, string>();

            string keyData = LoadResource("controlgallery.config").Result;
            string[] entries = keyData.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string entry in entries)
            {
                string[] parts = entry.Split(':');
                if (parts.Length < 2)
                    continue;

                s_config.Add(parts[0].Trim(), parts[1].Trim());
            }
        }

        static async Task<string> LoadResource(string filename)
        {
            string assemblystring;
            Assembly assembly = GetAssembly(out assemblystring);

            Stream stream = assembly.GetManifestResourceStream($"{assemblystring}.{filename}");
            string text;
            using (var reader = new StreamReader(stream))
                text = await reader.ReadToEndAsync();
            return text;
        }

        public bool NavigateToTestPage(string test)
        {
            try
            {
                // Create an instance of the main page
                var root = CreateDefaultMainPage();

                // Set up a delegate to handle the navigation to the test page
                EventHandler toTestPage = null;

                toTestPage = delegate (object sender, EventArgs e)
                {
                    Current.MainPage.Navigation.PushModalAsync(TestCases.GetTestCases());
                    TestCases.TestCaseScreen.PageToAction[test]();
                    Current.MainPage.Appearing -= toTestPage;
                };

                // And set that delegate to run once the main page appears
                root.Appearing += toTestPage;

                SetMainPage(root);

                return true;
            }
            catch (Exception ex)
            {
                Log.Warning("UITests", $"Error attempting to navigate directly to {test}: {ex}");

            }

            return false;
        }

        public void Reset()
        {
            SetMainPage(CreateDefaultMainPage());
        }
    }
}