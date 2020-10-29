using CefSharp;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
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
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfApp_web"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfApp_web;assembly=WpfApp_web"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class DownloadingPanel : WrapPanel
    {
        public MetroProgressBar proBar;
        public TextBlock Percent;
        public TextBlock Speed;
        public Button Pause;
        public Button Cancel;
        public IDownloadItemCallback callback;
        public DownloadItem downloadItem;
        internal Common.Control.MyDownloadHandler handler;
        public bool paused;
        public bool specialDownload;
        private double fontsize;

        internal DownloadingPanel(String filename, Common.Control.MyDownloadHandler _handler, bool is_spdownload = false) : base()
        {
            this.handler = _handler;
            this.specialDownload = is_spdownload;
            this.Orientation = Orientation.Horizontal;
            this.Margin = new Thickness(2, 3, 2, 3);
            this.Children.Add(new TextBlock() { Text = "文件名:" + filename, FontSize = this.fontsize = 20, Margin = new Thickness(3, 0, 3, 0) });
            this.Children.Add(new TextBlock() { Text = "下载进度:", FontSize = this.fontsize, Margin = new Thickness(3, 0, 3, 0) });
            proBar = new MetroProgressBar()
            {
                Value = 0,
                Width = 200,
                /*
                Background = new SolidColorBrush(Color.FromRgb(0xea, 0xef, 0xef)),
                Foreground = new SolidColorBrush(Color.FromRgb(0x45, 0xd2, 0x07)),
                */
                Margin = new Thickness(3, 0, 3, 0),
                Height = 18,
                VerticalAlignment = VerticalAlignment.Center,
            };
            this.Children.Add(proBar);
            this.Children.Add(Percent = new TextBlock { Text = "0%", Margin = new Thickness(3, 0, 3, 0), FontSize = this.fontsize });
            this.Children.Add(Speed = new TextBlock { Text = "0 B/s", Margin = new Thickness(3, 0, 3, 0), FontSize = this.fontsize });
            this.Children.Add(Pause = new Button() { Margin = new Thickness(3, 0, 3, 0)});
            this.Pause.Content = new PackIconModern() { Kind = PackIconModernKind.ControlPause };
            this.paused = false;
            this.Children.Add(Cancel = new Button() { Margin = new Thickness(3, 0, 3, 0)});
            this.Cancel.Content = new PackIconModern() { Kind = PackIconModernKind.ControlStop };
            this.Pause.Click += Pause_Click;
            this.Cancel.Click += Cancel_Click;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            handler.DoSomethingAfterDownload(downloadItem);
            callback.Cancel();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if(!this.paused)
            {
                btn.Content = new PackIconModern() { Kind = PackIconModernKind.ControlPlay };
                callback.Pause();
            }
            else
            {
                btn.Content = new PackIconModern() { Kind = PackIconModernKind.ControlPause };
                callback.Resume();
            }
            this.paused = !this.paused;
        }
    }
}
