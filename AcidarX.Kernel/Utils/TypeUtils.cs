using System;
using System.Collections.Concurrent;
using System.Reflection.Emit;

namespace AcidarX.Kernel.Utils
{
    public static class TypeUtils
    {
        private static readonly ConcurrentDictionary<Type, int>
            Cache = new();

        public static int SizeOf<T>() where T : struct => SizeOf(typeof(T?));

        public static int SizeOf<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return SizeOf(obj.GetType());
        }

        public static int SizeOf(Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }

            return Cache.GetOrAdd(t, t2 =>
            {
                var dm = new DynamicMethod("$", typeof(int), Type.EmptyTypes);
                ILGenerator il = dm.GetILGenerator();
                il.Emit(OpCodes.Sizeof, t2);
                il.Emit(OpCodes.Ret);

                Func<int> func = (Func<int>) dm.CreateDelegate(typeof(Func<int>));
                return func();
            });
        }
    }
}