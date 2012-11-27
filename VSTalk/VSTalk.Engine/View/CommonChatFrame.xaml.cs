using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VSTalk.Engine.Core;
using VSTalk.Engine.Core.Controls;

namespace VSTalk.Engine.View
{
    /// <summary>
    /// Interaction logic for CommonChatFrame.xaml
    /// </summary>
    [HostedControl(Name = "VsTalk Chat")]
    public partial class CommonChatFrame : UserControl
    {
        public CommonChatFrame()
        {
            InitializeComponent();
        }
    }
}
