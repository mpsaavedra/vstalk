using System;
using System.Linq;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using Microsoft.VisualStudio.Shell;
using VSTalk.Engine.Core;
using VSTalk.Engine.Core.Controls;

namespace Microsoft.VSTalk
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid("4aaf9d89-bf38-4ed9-b0b5-1f324bfbbdc9")]
    public class BaseToolWindow : ToolWindowPane
    {
        private readonly FrameworkElement _control;

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public BaseToolWindow() :
            base(null)
        {
            // Set the window title reading it from the resources.
            this.Caption = Resources.ToolWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            _control = new FrameControl();

            _control.Loaded += ControlLoaded;
            
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            // ignore container with existence control
            if (((FrameControl)_control).AddinContainer.Children.Count > 0)
            {
                return;
            }
            var manager = VSTalkPackage.VSTalkCore.WindowsManager as WindowsManager;
            if (manager == null)
            {
                return;
            }
            manager.SearchContent(this);
        }

        /// <summary>
        /// Called by package when creating a new tool window to set
        /// window content and pass on a Package ref
        /// </summary>
        /// <param name="userControl"></param>
        public void SetContent(UserControl userControl)
        {
            var addinContainer = ((FrameControl)_control).AddinContainer;
            //if control is alredy exist in container we do nothing
            
            if (addinContainer.Children.OfType<MetroContentControl>()
                .Select(wrapper => wrapper.Content)
                .Contains(userControl))
            {
                return;
            }
            Caption = ExtractName(userControl);
            
            var metroWrapper = new MetroContentControl();
            
            LoadResources(metroWrapper);

            metroWrapper.Content = userControl;
            
            addinContainer.Children.Add(metroWrapper);
        }

        private string ExtractName(UserControl control)
        {
            var hostedControlAttr = control.GetType().GetCustomAttributes(true)
                .OfType<HostedControlAttribute>()
                .FirstOrDefault();
            if (hostedControlAttr == null)
            {
                return "Unknown";
            }
            return hostedControlAttr.Name;
        }

        private static void LoadResources(MetroContentControl metroWrapper)
        {
            var dynamycResources = new[]
            {
                    "pack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml", 
                    "pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml",
                    "pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml",
                    "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml",
                    "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml",
                    "pack://application:,,,/MahApps.Metro;component/Styles/Controls.AnimatedTabControl.xaml",
                    "pack://application:,,,/VSTalk;component/Styles/ButtonStyle.xaml"
            };

            foreach (var resource in dynamycResources)
            {
                var rDictionary = new ResourceDictionary();
                rDictionary.Source = new Uri(resource);
                metroWrapper.Resources.MergedDictionaries.Add(rDictionary);
            }
        }

        /// <summary>
        /// This property returns the control that should be hosted in the Tool Window.
        /// It can be either a FrameworkElement (for easy creation of toolwindows hosting WPF content), 
        /// or it can be an object implementing one of the IVsUIWPFElement or IVsUIWin32Element interfaces.
        /// </summary>
        override public object Content
        {
            get
            {
                return _control;
            }
        }
    }
}
