using System;

namespace VSTalk.Tools
{
    internal class ActionWrapper<T1,T2> : IActionWrapper<T1,T2>
    {
        private Action<T1,T2> _action;

        public ActionWrapper(Action<T1,T2> action)
        {
            _action = action;
        }
        public void Exec(T1 o1, T2 o2)
        {
            _action(o1,o2);
        }
    }

    internal class ActionWrapper<T> : IActionWrapper<T>
    {
        private Action<T> _action;

        public ActionWrapper(Action<T> action)
        {
            _action = action;
        }
        public void Exec(T o)
        {
            _action(o);
        }
    }
}