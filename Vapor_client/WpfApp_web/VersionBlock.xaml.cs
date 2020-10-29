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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_web
{
    /// <summary>
    /// VersionBlock.xaml 的交互逻辑
    /// </summary>
    public partial class VersionBlock : UserControl
    {
        public AppInfoPanel fatherPanel;
        public String appversion;
        public String AppVersion
        {
            get
            {
                return appversion;
            }
            set
            {
                appversion = value;
                this.version.Text = appversion;
            }
        }
        public bool newest;
        public bool is_newest
        {
            get
            {
                return newest;
            }
            set
            {
                newest = value;
                if (newest)
                {
                    this.versionButton.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    this.versionButton.IsEnabled = false;
                    this.btnText.Text = "最新版";
                }
                else
                {
                    this.versionButton.Background = new SolidColorBrush(Color.FromRgb(250, 70, 30));
                    this.versionButton.IsEnabled = true;
                    this.btnText.Text = "可更新";
                }
            }
        }
        public VersionBlock(AppInfoPanel father, String _version, bool _new = true)
        {
            InitializeComponent();

            this.fatherPanel = father;
            AppVersion = _version;
            is_newest = _new;
        }

        private void VersionButton_Click(object sender, RoutedEventArgs e)
        {
            this.fatherPanel.Update();
        }

        private void VersionButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Opacity = 1.0;
        }

        private void VersionButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Opacity = 0.7;
        }
    }
}
