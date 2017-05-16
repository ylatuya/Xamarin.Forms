using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Xamarin.Forms
{
    public class LayoutsProfiler
    {
        static readonly Dictionary<string, LayoutData> Statistics =
            new Dictionary<string, LayoutData>();

        public static void Start(string tag = null, [CallerMemberName] string member = null)
        {
            string id = string.Format("{0} - {1}", member, tag);

            LayoutData stats;
            if (!Statistics.TryGetValue(id, out stats))
                Statistics[id] = stats = new LayoutData();

            stats.CallCount++;
            stats.StartTimes.Push(Stopwatch.GetTimestamp());
        }

        public static void Stop(string tag = null, [CallerMemberName] string member = null)
        {
            string id = string.Format("{0} - {1}", member, tag);
            long stop = Stopwatch.GetTimestamp();

            LayoutData stats = Statistics[id];
            long start = stats.StartTimes.Pop();
            if (!stats.StartTimes.Any())
                stats.TotalTime += stop - start;
        }

        public static string GetStats()
        {
            var b = new StringBuilder();
            b.AppendLine();
            foreach (KeyValuePair<string, LayoutData> kvp in
                Statistics.OrderBy(kvp => kvp.Key))
            {
                string id = kvp.Key;
                int callCount = kvp.Value.CallCount;
                double time = TimeSpan.FromTicks(kvp.Value.TotalTime).TotalMilliseconds;
                b.Append(string.Format("Call ID: {0}, Call Count: {1}, Time: {2} ms",
                    id,
                    callCount,
                    time));
                b.AppendLine();
            }
            return b.ToString();
        }
    }

    internal class LayoutData
    {
        public readonly Stack<long> StartTimes = new Stack<long>();
        public int CallCount;
        public long TotalTime;
    }
}
