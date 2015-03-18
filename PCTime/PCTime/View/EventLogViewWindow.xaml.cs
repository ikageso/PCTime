using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PCTime.View
{
    /// <summary>
    /// EventLogViewWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EventLogViewWindow : Window
    {
        public EventLogViewWindow()
        {
            //// Debug for "en-US"
            //Properties.Resources.Culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //マウスボタン押下状態でなければ何もしない
            if (e.ButtonState != MouseButtonState.Pressed) return;

            this.DragMove();
        }
    }
}
