using System.Collections.ObjectModel;
using Xamarin.Forms.Controls.Monkeys.Helpers;
using Xamarin.Forms.Controls.Monkeys.Models;

namespace Xamarin.Forms.Controls.Monkeys.ViewModels
{
    public class MonkeysViewModel : BindableObject
    {
        public MonkeysViewModel()
        {
            Monkeys = MonkeyHelper.Monkeys;
        }

        public ObservableCollection<Monkey> Monkeys { get; set; }
    }
}
