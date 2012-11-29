using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using VSTalk.Engine.Core;
using System.Windows;

namespace VSTalk.Engine
{
    public class AppWindowManager : WindowManager
    {
        public AppWindowManager(IWindowsManager _windowManager)
        {
            windowManager = _windowManager;
        }

        bool ApplySettings(object target, IEnumerable<KeyValuePair<string, object>> settings)
        {
            if (settings != null)
            {
                var type = target.GetType();

                foreach (var pair in settings)
                {
                    var propertyInfo = type.GetProperty(pair.Key);

                    if (propertyInfo != null)
                        propertyInfo.SetValue(target, pair.Value, null);
                }

                return true;
            }

            return false;
        }

        public override bool? ShowDialog(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            var userControl = new UserControl();
            ApplySettings(userControl, settings);
            var view = ViewLocator.LocateForModel(rootModel, userControl, context);
            ViewModelBinder.Bind(rootModel, view, context);

            windowManager.OpenWindow((UserControl)view);

            return true;
        }

        IWindowsManager windowManager;
    }
}
