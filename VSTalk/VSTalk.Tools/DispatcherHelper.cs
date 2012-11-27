using System;
using System.Windows.Threading;

namespace VSTalk.Tools
{
    public static class DispatcherHelper
    {
        public static IActionWrapper<T> Wrap<T>(this Dispatcher dispatcher, Action<T> action)
        {
            return new ActionWrapper<T>(o => dispatcher.Invoke(new Action(() => action(o))));
        }

        public static IActionWrapper<T1, T2> Wrap<T1, T2>(this Dispatcher dispatcher, Action<T1, T2> action)
        {
            return new ActionWrapper<T1, T2>((o1, o2)=> dispatcher.Invoke(new Action(() => action(o1, o2))));
        }
    }
}