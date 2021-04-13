namespace AcidarX.Kernel.Profiling
{
    public struct ProfileResult
    {
        public string Name;
        public long Start;
        public long End;
        public double Elapsed;
        public int ThreadID;
    }
}