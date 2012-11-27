using System;

namespace VSTalk.Engine.Core.Controls
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class HostedControlAttribute : Attribute
    {
        public string Name { get; set; }
    }
}