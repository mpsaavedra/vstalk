namespace VSTalk.Tools
{
    public interface IActionWrapper<in T1, in T2>
    {
        void Exec(T1 o1, T2 o2);
    }

    public interface IActionWrapper<in T>
    {
        void Exec(T o);
    }
}