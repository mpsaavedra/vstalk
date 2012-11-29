using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSTalk.Engine.Core;
using VSTalk.Engine.Core.Context;

namespace VSTalk.Engine
{
    public class AppBootstrapperHelper
    {
        static public void StartTest(IWindowsManager _windowsManager)
        {
            windowsManager = new AppWindowManager(_windowsManager);
            new AppBootstrapper();

            var w = IoC.Get<IWindowManager>();
            w.ShowDialog(new ViewModels.MainViewModel());
        }

        static public AppWindowManager windowsManager;
    }
}
