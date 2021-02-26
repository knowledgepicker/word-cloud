using System;

namespace KnowledgePicker.WordCloud.Internal
{
    public static class Extensions
    {
        public static IDisposable AsDisposable<T>(this T obj)
        {
            if (obj is IDisposable d) return d;
            return EmptyDisposable.Instance;
        }

        private class EmptyDisposable : IDisposable
        {
            public static readonly EmptyDisposable Instance = new EmptyDisposable();

            private EmptyDisposable() { }

            public void Dispose() { }
        }
    }
}
