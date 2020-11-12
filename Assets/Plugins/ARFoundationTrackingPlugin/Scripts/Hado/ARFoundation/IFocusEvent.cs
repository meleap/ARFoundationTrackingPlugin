using System;

namespace Hado.ARFoundation
{
    public interface IFocusEvent : IDisposable
    {
        void RegisterEvent();
        new void Dispose();
    }
}