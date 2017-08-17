using System;

namespace Xamarin.Forms.Controls.Timeline.Models
{
    public class ExerciseClass
    {
        public DateTime ClassTime { get; set; }
        public string ClassName { get; set; }
        public string Instructor { get; set; }
        public bool IsLast { get; set; } = false;
    }
}