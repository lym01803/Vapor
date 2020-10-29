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
    /// VerifyToolPage.xaml 的交互逻辑
    /// </summary>
    public partial class VerifyToolPage : Window
    {
        String foldPath = "";
        String filePath = "";
        String savePath = "";
        public VerifyToolPage()
        {
            InitializeComponent();
            this.Closed += VerifyToolPage_Closed;
        }

        private void VerifyToolPage_Closed(object sender, EventArgs e)
        {
            this.Owner.Focus(); //父窗口聚焦
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "选择要校验的目录";
            if (foldPath != "")
            {
                dialog.SelectedPath = foldPath;
            }
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foldPath = dialog.SelectedPath;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Title = "选择游戏启动器";
            dialog.Multiselect = false;
            if(filePath != "")
            {
                dialog.FileName = filePath;
            }
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = dialog.FileName;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if(btn.Content.ToString() == "打开文件夹")
            {
                System.Diagnostics.Process.Start(foldPath);
            }
            else //生成校验文件(或重新生成校验文件)
            {
                if (foldPath == "")
                {
                    MessageBox.Show("请选择要校验的目录");
                }
                else if (filePath == "")
                {
                    MessageBox.Show("请选择游戏启动器");
                }
                else
                {
                    if(filePath.Length < foldPath.Length)
                    {
                        MessageBox.Show("启动器不位于校验目录中");
                        return ;
                    }
                    String prefix = filePath.Substring(0, foldPath.Length);
                    if(prefix != foldPath)
                    {
                        MessageBox.Show("启动器不位于校验目录中");
                        return ;
                    }
                    savePath = System.IO.Path.Combine(foldPath, "VaporCheck.json");
                    Thread th = new Thread(new ParameterizedThreadStart(BuildStarter));
                    th.Start(new Object[] { foldPath, filePath, savePath, btn });
                    btn.Content = "正在生成校验文件...";
                    btn.IsEnabled = false;
                }
            }
        }

        //新开线程调用BuildStarter,而不是直接调用Build.这样可以降低耦合度：LocalGameFile.Build()不必知道这边是怎么实现的。
        //生成完成（或异常退出）的善后工作都由该函数完成.
        private void BuildStarter(Object argv)
        {
            Object[] paras = (Object[])argv;
            LocalGameFile lgf = new LocalGameFile();
            String b = lgf.Build(paras[0] as String, paras[1] as String, paras[2] as String);
            Button btn = paras[3] as Button;
            if (b == null)
            {
                MessageBox.Show(String.Format("校验文件 {0} 已生成", paras[2] as String));
                btn.Dispatcher.Invoke(new Action(() =>{
                    btn.Content = "打开文件夹";
                    btn.IsEnabled = true;
                }));
                Thread.Sleep(3000);
            }
            else
            {
                MessageBox.Show(b);
                btn.Dispatcher.Invoke(new Action(() => {
                    btn.Content = "重新生成校验文件";
                    btn.IsEnabled = true;
                }));
            }
        }
    }
}
