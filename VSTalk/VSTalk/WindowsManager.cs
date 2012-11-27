using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Microsoft.VisualStudio.Shell.Interop;
using VSTalk.Engine.Core;
using VSTalk.Engine.Core.Controls;

namespace Microsoft.VSTalk
{
    public class WindowsManager : IWindowsManager
    {
        private readonly VSTalkPackage _package;

        public WindowsManager(VSTalkPackage package)
        {
            _package = package;
        }

        internal void SearchContent(BaseToolWindow toolWindow)
        {
            var availableControls = ExtractFromRepository(VSTalkPackage.VSTalkCore.ControlsRepository);
            foreach (var hostedControl in availableControls)
            {
                var window = _package.FindToolWindow(typeof(BaseToolWindow),
                    Math.Abs(hostedControl.GetType().ToString().GetHashCode()),
                    false);

                if (window == toolWindow)
                {
                    toolWindow.SetContent(hostedControl);
                    return;
                }
            }
        }

        public void OpenWindow(UserControl userControl)
        {
            var window = _package.FindToolWindow(typeof(BaseToolWindow), 
                                                      Math.Abs(userControl.GetType().ToString().GetHashCode()), 
                                                      true);
            
            if (window == null || window.Frame == null)
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }

            ((BaseToolWindow)window).SetContent(userControl);

            var windowFrame = (IVsWindowFrame)window.Frame;

            VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private IEnumerable<UserControl> ExtractFromRepository(IControlsRepository repository)
        {
            foreach (var property in repository.GetType().GetProperties())
            {
                var control = property
                    .GetValue(repository, null);
                var isHostedControl = control.GetType()
                    .GetCustomAttributes(true)
                    .OfType<HostedControlAttribute>()
                    .Any();
                if (!isHostedControl)
                {
                    continue;
                }
                yield return (UserControl) control;
            }
        }
    }
}
