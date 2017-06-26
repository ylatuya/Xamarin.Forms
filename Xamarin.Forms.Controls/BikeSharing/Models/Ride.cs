using System;
using Xamarin.Forms;

namespace Xamarin.Forms.Controls
{
    public enum RideType
    {
        Event,
        Suggestion,
        Custom
    }

    public class Ride 
    {
        public int Id { get; set; }

        public int? EventId { get; set; }

        public RideType RideType { get; set; }

        public string Name { get; set; }

        public DateTime Start { get; set; }

        public DateTime Stop { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public int Distance { get; set; }

        public int Duration { get; set; }
    }
}