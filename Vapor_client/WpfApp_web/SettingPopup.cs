using CefSharp;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_web
{
    public class SettingPopup : Popup
    {
        public StackPanel stackPanel; // 面板
        public Button addressButton; // 修改地址的按钮
        public StackPanel addressButtonContent; //修改地址按钮内容
        public TextBlock showAddress;
        public Button quitButton; //退出按钮
        public Button packTool; //打包工具
        public Button modifyPasswd;
        public SettingPopup() : base()
        {
            this.StaysOpen = false;
            this.Placement = PlacementMode.Bottom;
            this.MinWidth = 120;

            this.Child = this.stackPanel = new StackPanel()
            {
                Background = new SolidColorBrush(Colors.White),
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0.5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            this.addressButtonContent = new StackPanel() { Margin = new Thickness(0), Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left };
            this.addressButtonContent.Children.Add(new PackIconBoxIcons() { Kind = PackIconBoxIconsKind.SolidEditAlt });
            this.addressButtonContent.Children.Add(new TextBlock() { Text = "下载目录", FontSize = 14 });
            this.addressButton = new Button() { Content = this.addressButtonContent, Margin = new Thickness(0.5), Height = 30, BorderThickness = new Thickness(0) };
            this.addressButton.Click += AddressButton_Click;
            this.addressButton.ToolTip = this.showAddress = new TextBlock();
            this.showAddress.SetBinding(TextBlock.TextProperty, new Binding("currentPath") { Source = (Application.Current.MainWindow as MainWindow).userSetting});

            this.quitButton = new Button() { Content = new TextBlock() { Text = "退出登录", FontSize = 14, HorizontalAlignment = HorizontalAlignment.Left },
                Margin = new Thickness(0.5), Height = 30, BorderThickness = new Thickness(0) };
            this.quitButton.Click += QuitButton_Click;

            this.packTool = new Button() { Content = new TextBlock() { Text = "生成校验文件", FontSize = 14, HorizontalAlignment = HorizontalAlignment.Left },
                Margin = new Thickness(0.5), Height = 30, BorderThickness = new Thickness(0) };
            this.packTool.Click += PackTool_Click;

            this.modifyPasswd = new Button() { Content = new TextBlock() { Text = "修改密码", FontSize = 14, HorizontalAlignment = HorizontalAlignment.Left },
                Margin = new Thickness(0.5), Height = 30, BorderThickness = new Thickness(0) };
            this.modifyPasswd.Click += ModifyPasswd_Click;
            this.modifyPasswd.Click += (Application.Current.MainWindow as MainWindow).ManageUserInfoButton_Click;

            this.stackPanel.Children.Add(this.addressButton);
            this.stackPanel.Children.Add(this.modifyPasswd);
            this.stackPanel.Children.Add(this.packTool);
            this.stackPanel.Children.Add(this.quitButton);
        }

        private void ModifyPasswd_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }
        private void PackTool_Click(object sender, RoutedEventArgs e)
        {
            VerifyToolPage toolPage = new VerifyToolPage() { Owner = Application.Current.MainWindow };
            toolPage.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.IsOpen = false;
            toolPage.Show();
        }
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwd = Application.Current.MainWindow as MainWindow;
            mainwd.userCtrl.UserQuit();
            this.IsOpen = false;
        }
        private void AddressButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwd = Application.Current.MainWindow as MainWindow;
            var dialog = new System.Windows.Forms.FolderBrowserDialog()
            {
                SelectedPath = mainwd.userSetting.currentPath
            };
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mainwd.userSetting.currentPath = dialog.SelectedPath;
                if(dialog.SelectedPath[dialog.SelectedPath.Length - 1] != '\\')
                {
                    mainwd.userSetting.currentPath += @"\";
                }
                Setting.Save(mainwd.userSetting, MyUriUtil.RelativePathProcessor()+@"settings\" + mainwd.user.UserName.ToString() + @"\settings.json");
            }
        }
    }
}
