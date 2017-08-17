using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.Controls.Timeline.Models
{
    public static class TimelineDataFactory
    {
        public static IList<ExerciseClass> Classes { get; private set; }

        private static DateTime TodayAt(int hour, int minute)
        {
            return new DateTime(DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                hour, minute, 0);
        }

        static TimelineDataFactory()
        {
            Classes = new ObservableCollection<ExerciseClass>
            {
                new ExerciseClass
                {
                    ClassName = "Yoga",
                    Instructor = "Maharshi Patanjali",
                    ClassTime = TodayAt(8,00),
                },
                 new ExerciseClass
                {
                    ClassName = "ABS + Stretch",
                    Instructor = "David Hasslehoff",
                    ClassTime = TodayAt(9,30),
                },
                 new ExerciseClass
                {
                    ClassName = "Cycle",
                    Instructor = "Lance Armstrong",
                    ClassTime = TodayAt(12,00),
                },
                 new ExerciseClass
                {
                    ClassName = "Aerobics",
                    Instructor = "Jacky Chan",
                    ClassTime = TodayAt(15,30),
                },
                 new ExerciseClass
                {
                    ClassName = "Weights",
                    Instructor = "Arnold Schwarzenegger",
                    ClassTime = TodayAt(18,00),
                    IsLast = true
                },
            };
        }
    }
}