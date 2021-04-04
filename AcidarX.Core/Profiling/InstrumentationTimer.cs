using System;
using System.Threading;

namespace AcidarX.Core.Profiling
{
    public class InstrumentationTimer : IDisposable
    {
        private readonly long _start;
        private bool _stopped;

        public InstrumentationTimer(string name)
        {
            Name = name;
            _start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public string Name { get; }

        public void Dispose()
        {
            if (!_stopped)
            {
                long end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                int threadID = Thread.CurrentThread.ManagedThreadId;

                Instrumentation.Instance.WriteProfile(new ProfileResult
                    {Name = Name, Start = _start, End = end, ThreadID = threadID});
                _stopped = true;
            }
        }

        ~InstrumentationTimer()
        {
            Dispose();
        }

        public static InstrumentationTimer Profile(string name) => new(name);
    }
}