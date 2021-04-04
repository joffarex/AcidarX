using System.IO;

namespace AcidarX.Core.Profiling
{
    public struct InstrumentationSession
    {
        public string Name;
    }

    public class Instrumentation
    {
        private static Instrumentation _instance;

        private static readonly object padlock = new();
        private InstrumentationSession? _currentSession;
        private StreamWriter _outputStream;
        private int _profileCount;

        private Instrumentation()
        {
            _currentSession = null;
            _profileCount = 0;
        }

        public static Instrumentation Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new Instrumentation();
                    }

                    return _instance;
                }
            }
        }

        public void BeginSession(string name, string filePath)
        {
            string fullPath = PathUtils.GetFullPath($"profile/{filePath}");
            _outputStream = new StreamWriter(fullPath);
            WriteHeader();
            _currentSession = new InstrumentationSession {Name = name};
        }

        public void EndSession()
        {
            WriteFooter();
            _outputStream.Close();
            _outputStream.Dispose();
            _currentSession = null;
            _profileCount = 0;
        }

        public void WriteProfile(ProfileResult result)
        {
            if (_profileCount++ > 0)
            {
                _outputStream.WriteLine(",");
            }

            string name = result.Name;
            _outputStream.Write(
                $"{{\"cat\":\"function\",\"dur\":{result.End - result.Start},\"name\":\"{name}\",\"ph\":\"X\",\"pid\":0,\"tid\":{result.ThreadID},\"ts\":{result.Start}}}");
        }

        public void WriteHeader()
        {
            _outputStream.WriteLine("{\"otherData\": {},\"traceEvents\":[");
        }

        public void WriteFooter()
        {
            _outputStream.WriteLine("]}");
        }
    }
}