using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Logging
{
    public class NoopDisposable : IDisposable
    {
        public static NoopDisposable Instance = new NoopDisposable();
        public void Dispose()
        {
        }
    }
}
