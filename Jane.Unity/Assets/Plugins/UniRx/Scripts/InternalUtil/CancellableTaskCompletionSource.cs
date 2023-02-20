#pragma warning disable CS1591

using System;
using System.Threading.Tasks;

namespace UniRx.InternalUtil
{
    internal interface ICancellableTaskCompletionSource
    {
        bool TrySetException(Exception exception);
        bool TrySetCanceled();
    }

    internal class CancellableTaskCompletionSource<T> : TaskCompletionSource<T>, ICancellableTaskCompletionSource
    {
       
    }
}

#pragma warning restore CS1591
