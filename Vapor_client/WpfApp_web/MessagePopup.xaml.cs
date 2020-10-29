using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp_web
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class MessagePopup : Window
    {
        int delay;
        public MessagePopup(String msg, int _delay)
        {
            InitializeComponent();
            this.TextArea.Text = msg;
            this.delay = _delay;
            this.Closed += MessagePopup_Closed;
        }

        private void MessagePopup_Closed(object sender, EventArgs e)
        {
            this.Owner.Focus();
        }

        public new void Show()
        {
            base.Show();
            Thread th = new Thread(new ThreadStart(DelayClose));
            th.Start();
        }
        private void DelayClose()
        {
            Thread.Sleep(delay);
            this.Dispatcher.Invoke(new Action(() => { this.Close(); }));
        }
    }
}
