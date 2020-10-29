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
using System.Threading;
using System.Net;

using System.Runtime.InteropServices;

using CefSharp;
using CefSharp.Structs;
using CefSharp.Wpf;
using CefSharp.WinForms;
using System.Windows.Forms.Integration;

using MahApps.Metro.Controls;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Controls.Primitives;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Media.Animation;
using MahApps.Metro.IconPacks;

using ICSharpCode.SharpZipLib.Zip;

//生成调试用控制台
namespace DebugConsole
{
    internal class DebugConsoleControl
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool FreeConsole();
    }
}

namespace InstallUtil
{
    public class CmdProcess
    {
        public Process process;
        public CmdProcess()
        {
            process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;
            process.StartInfo.CreateNoWindow = true;
            process.Exited += Process_Exited;
            process.Start();
            Console.WriteLine("start cmd");
        }
        public void Exec(String cmd)
        {
            process.StandardInput.WriteLine(cmd);
        }
        public String GetRes()
        {
            return process.StandardOutput.ReadToEnd();
        }
        public void Exit()
        {
            process.StandardInput.WriteLine("exit");
            process.WaitForExit();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("cmd disposed");
            (sender as Process).Dispose();
        }
    }

    public class Uncompress
    {
        public static bool UncompressFile(String file, String path)
        {
            /*
            Console.WriteLine("uncompress {0}", file);
            String type = WpfApp_web.MyUriUtil.GetFileTypeFromUrl(file);
            CmdProcess cmd = new CmdProcess();
            Console.WriteLine("File Type: {0}", type);
            if (type == null) type = "zip";
            try
            {
                switch (type)
                {
                    case "zip":
                        cmd.Exec(String.Format("unzip {0} -d {1}", file, path));
                        break;
                    case "tar":
                        cmd.Exec(String.Format("tar -xvf {0} -C {1}", file, path));
                        break;
                    case "gz":
                        cmd.Exec(String.Format("tar -zxvf {0} -C {1}", file, path));
                        break;
                }
                cmd.Exit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            */
            try
            {
                FastZip zip = new FastZip();
                zip.ExtractZip(file, path, "");
                return true; //解压成功
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
                return false; //解压失败
            }
        }
    }
}

//浏览器封装
namespace Common.Control
{
    // 屏蔽 Frame 的 back 导航
    public class NoBackFrame : Frame
    {
        public NoBackFrame () : base()
        {
            Navigating += NoBackFrame_Navigating;
        }

        private void NoBackFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            //Console.Write("Navigating\n{0}\n", e.NavigationMode);
            if(e.NavigationMode == NavigationMode.Back)
            {
                e.Cancel = true;
            } //屏蔽 back 导航事件
        }
    }  
    // ----- 实现接口
    /*
    internal class MyRequestHandler : IRequestHandler
    {
        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            return false; //throw new NotImplementedException();
        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return null; //throw new NotImplementedException();
        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            //throw new NotImplementedException();
            Console.WriteLine("fuck");
            if (request.TransitionType.HasFlag(CefSharp.TransitionType.ForwardBack))
            {
                return true;
            }
            return false;
        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            return false; //throw new NotImplementedException();
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false; //throw new NotImplementedException();
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
        {
            return; //throw new NotImplementedException();
        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            return false; // throw new NotImplementedException();
        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
        {
            return; // throw new NotImplementedException();
        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            return; // throw new NotImplementedException();
        }

        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return false; // throw new NotImplementedException();
        }
    }
    */
    internal class MyDisplayHandler : IDisplayHandler
    {
        WpfApp_web.MainWindow mainwd;
        public MyDisplayHandler(WpfApp_web.MainWindow mw) : base()
        {
            mainwd = mw;
        }

        public void OnAddressChanged(IWebBrowser chromiumWebBrowser, AddressChangedEventArgs addressChangedArgs)
        {
            
        }

        public bool OnAutoResize(IWebBrowser chromiumWebBrowser, IBrowser browser, CefSharp.Structs.Size newSize)
        {
            return false;
        }

        public bool OnConsoleMessage(IWebBrowser chromiumWebBrowser, ConsoleMessageEventArgs consoleMessageArgs)
        {
            return false;
        }

        public void OnFaviconUrlChange(IWebBrowser chromiumWebBrowser, IBrowser browser, IList<string> urls)
        {
            
        }

        public void OnFullscreenModeChange(IWebBrowser chromiumWebBrowser, IBrowser browser, bool fullscreen)
        {
            
        }

        public void OnLoadingProgressChange(IWebBrowser chromiumWebBrowser, IBrowser browser, double progress)
        {
            
        }

        public void OnStatusMessage(IWebBrowser chromiumWebBrowser, StatusMessageEventArgs statusMessageArgs)
        {
            
        }

        public void OnTitleChanged(IWebBrowser chromiumWebBrowser, TitleChangedEventArgs titleChangedArgs)
        {
            
        }

        public bool OnTooltipChanged(IWebBrowser chromiumWebBrowser, ref string text)
        {
            return false;
        }
    }
    internal class CefSharpOpenPageSelf : ILifeSpanHandler
    {
        public bool DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            return false;
        }
        public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {
        }
        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {
        }
        public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;
            var chromiumWebBrowser = (CefSharp.WinForms.ChromiumWebBrowser)browserControl;
            chromiumWebBrowser.Load(targetUrl);
            return true; //Return true to cancel the popup creation 
        } //屏蔽弹窗
    }
    internal class MyDownloadHandler : IDownloadHandler
    {
        WpfApp_web.MainWindow mainwd;
        public MyDownloadHandler(WpfApp_web.MainWindow mwd) : base()
        {
            mainwd = mwd;
        }

        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    if(mainwd.Address2ProgressBar.ContainsKey(downloadItem.OriginalUrl))
                    {
                        return ;
                    }
                    //mainwd.IsDownloading.Add(downloadItem.OriginalUrl, true);
                    callback.Continue(mainwd.userSetting.currentPath + downloadItem.SuggestedFileName, showDialog: false);
                    DoSomethingBeforeDownload(downloadItem, mainwd.user.is_root && (chromiumWebBrowser.Address == mainwd.Browsers[2].Address));
                }
            }
        } //下载

        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            downloadItem.IsCancelled = false;

            Console.WriteLine("{0} ; {1} ; {2}%; {3}", downloadItem.CurrentSpeed, downloadItem.EndTime, downloadItem.PercentComplete, downloadItem.IsComplete);

            mainwd.Dispatcher.Invoke(new Action(() => {
                if (mainwd.Address2ProgressBar.ContainsKey(downloadItem.OriginalUrl.ToString()))
                {
                    WpfApp_web.DownloadingPanel panel = mainwd.Address2ProgressBar[downloadItem.OriginalUrl.ToString()] as WpfApp_web.DownloadingPanel;
                    panel.callback = callback;
                    panel.downloadItem = downloadItem;
                    panel.proBar.Value = downloadItem.PercentComplete;
                    panel.Percent.Text = String.Format("{0}%", downloadItem.PercentComplete);
                    long speed = downloadItem.CurrentSpeed;
                    if (speed >= 1048576)
                        panel.Speed.Text = String.Format("{0:f1} MB/s", 1.0 * speed / 1048576);
                    else if (speed >= 1024)
                        panel.Speed.Text = String.Format("{0:f1} kB/s", 1.0 * speed / 1024);
                    else
                        panel.Speed.Text = String.Format("{0} B/s", speed);
                }
            }));

            if (downloadItem.IsComplete || downloadItem.IsCancelled || callback.IsDisposed)
            {
                DoSomethingAfterDownload(downloadItem); //上述三种情况都要执行善后工作
            }
        }

        public void DoSomethingBeforeDownload(DownloadItem downloadItem, bool special)
        {
            mainwd.Dispatcher.Invoke(new Action(() => {
                mainwd.NavigationButtonChange(0);
                mainwd.ManagePage.DownloadingPart.Visibility = Visibility.Visible;
                WpfApp_web.DownloadingPanel panel = new WpfApp_web.DownloadingPanel(downloadItem.SuggestedFileName, this, special);
                mainwd.Address2ProgressBar.Add(downloadItem.OriginalUrl.ToString(), panel);
                mainwd.ManagePage.DownloadingPanel.Children.Add(panel);
                mainwd.DownloadStoryBoard.Begin(mainwd);
            }));
        }

        public void DoSomethingAfterDownload(DownloadItem downloadItem)
        {
            mainwd.Dispatcher.Invoke(new Action(async () => {
                WpfApp_web.DownloadingPanel panel = mainwd.Address2ProgressBar[downloadItem.OriginalUrl.ToString()] as WpfApp_web.DownloadingPanel;
                if (downloadItem.IsComplete && !(panel.specialDownload)) //如果是下载完成，则执行后续工作 (通知服务器，以及其他本地文件操作)； 如果特殊下载，则不执行
                {
                    String[] info = WpfApp_web.MyUriUtil.GetAppSIdFromUrl(downloadItem.OriginalUrl);
                    if (info != null && info.Length >= 2)
                    {
                        String url = String.Format(@"http://127.0.0.1:8000/api/appinfo/?appsid={0}&version={1}", info[0], info[1]);
                        String appJsonStr = await WpfApp_web.MyWebUtil.ReadStringByUrlAsync(url);
                        WpfApp_web.GameApp someApp = JsonConvert.DeserializeObject<WpfApp_web.GameApp>(appJsonStr);
                        Thread th = new Thread(new ParameterizedThreadStart(mainwd.DownloadCtr.NewAppDownloadedAsync));
                        th.Start(new Object[] { someApp, downloadItem.FullPath });
                    }
                }
                
                mainwd.ManagePage.DownloadingPanel.Children.Remove(panel); //不论是完成还是取消还是析构，都要修改UI
                mainwd.Address2ProgressBar.Remove(downloadItem.OriginalUrl.ToString()); //记录正在下载的字典也要修改
                if (mainwd.Address2ProgressBar.Count == 0)  //如果没有正在下载的任务，则隐藏正在下载列表
                {
                    mainwd.NavigationButtonChange(1);
                    mainwd.ManagePage.DownloadingPart.Visibility = Visibility.Collapsed;
                }
            }));
        }

        public bool OnDownloadUpdated(CefSharp.DownloadItem downloadItem)
        {
            return false;
        }
    }
    internal class MyMenuHandler : IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();
            model.AddItem(CefMenuCommand.Back, "返回");
            model.AddItem(CefMenuCommand.Reload, "刷新");
        }

        public bool OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
        {
            
        }

        public bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
    // 封装实现了接口的浏览器
    public class ExChromiumWebBrowser : CefSharp.WinForms.ChromiumWebBrowser
    {
        public WpfApp_web.MainWindow mainwd;
        
        public ExChromiumWebBrowser(WpfApp_web.MainWindow _mainwd, String url) : base(url)
        {
            this.mainwd = _mainwd;
            this.LifeSpanHandler = new CefSharpOpenPageSelf();
            this.DownloadHandler = new MyDownloadHandler(this.mainwd);
            //this.RequestHandler = new MyRequestHandler();
            this.DisplayHandler = new MyDisplayHandler(this.mainwd);
            this.MenuHandler = new MyMenuHandler();
            this.BrowserSettings = new BrowserSettings() { AcceptLanguageList = "zh_CN" };

            //this.DownloadHandler = new DownloadHandler(this);
        }
        
    }
}

// -----------------------------------------------------------------
namespace WpfApp_web
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public class CanPopupWindow
    {
        public bool canPopup { get; set; }
    }
    public partial class PopupWindow : Window
    {
        public CanPopupWindow canPopup;
        public CefSharp.WinForms.ChromiumWebBrowser browser;

        public WindowsFormsHost wformsHost;
        public PopupWindow(String url, CanPopupWindow canpop) : base()
        {
            canPopup = canpop;
            canPopup.canPopup = false;
            this.browser = new CefSharp.WinForms.ChromiumWebBrowser(url);
            wformsHost = new WindowsFormsHost() { Child = this.browser };
            this.AddChild(wformsHost);
            this.Closed += PopupWindow_Closed;
            this.Width = 500;
            this.Height = 500;
        }

        private void PopupWindow_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("触发窗口关闭事件");
            //this.browser.GetBrowser().CloseBrowser(false);
            wformsHost.Dispose();
            //标记
            canPopup.canPopup = true;
            //调试语句
        }
    }

    public partial class MainWindow : MetroWindow
    {
        internal User user = new User();  //用户
        internal UserControl userCtrl;  //登录管理
        internal Common.Control.ExChromiumWebBrowser[] Browsers = new Common.Control.ExChromiumWebBrowser [4];  //浏览器数组
        internal Dictionary<String, Object> Str2Obj = new Dictionary<String, Object>(); //字典，存储一些常量
        internal Dictionary<String, Object> Address2ProgressBar = new Dictionary<String, Object>();
        internal CanPopupWindow canPopup = new CanPopupWindow() { canPopup = true };
        internal PopupWindow popupWindow;
        internal Page1 ManagePage;
        internal Page1 ReleasedAppPage;
        internal FavorPage favorPage;
        internal Setting userSetting = null;
        internal Popup settingPopup = null;
        internal DownloadControl DownloadCtr; //在managepage初始化后初始化
        internal Storyboard DownloadStoryBoard;

        //下面是关于背景线程
        public class BackgroundTaskParams
        {
            public TileItem source;
            public String url;
            public String basepath;
            public BackgroundTaskParams(TileItem _source, String _url, String _basepath)
            {
                this.source = _source;
                this.url = _url;
                this.basepath = _basepath;
            }
        }
        public Queue<BackgroundTaskParams> BackgroundTasks = new Queue<BackgroundTaskParams>();
        public readonly object _locker = new Object();
        public EventWaitHandle WaitHandle = new AutoResetEvent(false);
        public Thread DownloadThread;
        public void AddTask(TileItem item, String url, String basepath)
        {
            lock (_locker)
            {
                this.BackgroundTasks.Enqueue(new BackgroundTaskParams(item, url, basepath));
                this.WaitHandle.Set();
            }
        }

        public void GetDownloadedAppsList()
        {
            if(userSetting != null)
            {

            }
        }

        public void MyDebug()
        {
            //Console.WriteLine("Debug ... ");
            //InstallUtil.Uncompress.UncompressFile(@"C:/Users/27931/Desktop/source.zip", @"C:/Users/27931/Desktop");
        }

        public void UpdateStr2Obj(String str, Object obj)
        {
            if (this.Str2Obj.ContainsKey(str))
            {
                this.Str2Obj[str] = obj;
            }
            else
            {
                this.Str2Obj.Add(str, obj);
            }
        }

        public async void LoginInitAsync(String userName)
        {
            //修改user信息;
            this.user.UserName = userName; //调试语句，后续改为读取数据库
            User tmpuser = await User.GetUserInfo(userName);
            this.user.Update(tmpuser);
            //注意这里会产生一个新的 user 对象，其引用会变，所以之前的绑定会有问题，这里需要重新绑定
            //this.LoginStatus.SetBinding(TextBlock.TextProperty, new Binding("LoginStatusStr") { Source = user }); 
            //binding 登录状态显示
            this.user.LoginStatus = User.loginstatus.login;   //更改登录状态
            //Console.WriteLine(this.user.LoginStatus);
            this.user.GetUserHead(this.UserHead);

            //调整UI
            this.NavigationStackPanel.Visibility = Visibility.Visible;    //修改分页导航栏显示状态
            //this.LoginButton.Visibility = Visibility.Collapsed;           //修改登录按钮、注册按钮、账号管理按钮显示状态
            //this.RegisterButton.Visibility = Visibility.Collapsed;
            //this.ManageUserInfoButton.Visibility = Visibility.Visible;
            //根据user信息重新加载网页
            this.Browsers[0].Load(this.Str2Obj["主页地址"] as String);  // 调试语句，后续合并后调整
            this.Browsers[1].Load(this.Str2Obj["发布地址"] as String);
            Console.WriteLine("is_root: {0}", this.user.is_root);
            if (this.user.is_root)
            {
                this.Browsers[2].Load(this.Str2Obj["管理员页面地址"] as String);
                (VisualTreeHelper.GetChild(this.NavigationStackPanel, 3) as Button).Content = "审核发布";
            }
            else
            {
                //this.Browsers[2].Load(this.Str2Obj["收藏地址"] as String);
                (VisualTreeHelper.GetChild(this.NavigationStackPanel, 3) as Button).Content = "我的收藏";
            }
            this.Browsers[3].Load(this.Str2Obj["消息地址"] as String);
            //读取setting文件
            String path = MyUriUtil.RelativePathProcessor() + @"settings\" + this.user.UserName.ToString() + @"\";
            if (!Directory.Exists(path))
            {
                Console.WriteLine(path);
                Directory.CreateDirectory(path);
            }
            path += @"settings.json";
            if (!File.Exists(path))
            {
                Console.WriteLine(path);
                var fileStream = File.Create(path);
                fileStream.Close();
            }
            Setting.Load(ref this.userSetting, path);
            if (this.userSetting == null)
            {
                this.userSetting = new Setting() { currentPath = MyUriUtil.RelativePathProcessor() };
                Setting.Save(this.userSetting, path);
            }
            this.settingPopup = new SettingPopup();
            Console.WriteLine(this.settingPopup == null); //初始化用户头像按钮弹窗


            //初始化 [本地管理] 和 [我的发布] 页面
            this.ManagePage = new Page1(this, 1);
            this.ReleasedAppPage = new Page1(this, 2);

            this.user.GetDownloadedAppsAsync(this.ManagePage);
            this.user.GetReleasedAppsAsync(this.ReleasedAppPage);

            this.UpdateStr2Obj("本地管理", this.ManagePage);
            this.UpdateStr2Obj("我的发布", this.ReleasedAppPage);

            this.DownloadCtr = new DownloadControl(this);
        }

        public void Logout()
        {
            this.user.LoginStatus = User.loginstatus.notlogin;
            UserHead.Source = new BitmapImage(new Uri(MyUriUtil.RelativePathProcessor() + @"Image\UserHead.png", UriKind.Absolute));
            this.Browsers[0].Load("http://127.0.0.1:8000/logout/");
            Button btn = VisualTreeHelper.GetChild(this.NavigationStackPanel, 0) as Button; //获取导航标签的第一个子元素
            btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); //触发该子元素（按钮）的 Click 事件
            this.NavigationStackPanel.Visibility = Visibility.Collapsed;    //修改分页导航栏显示状态
            //this.LoginButton.Visibility = Visibility.Visible;           //修改登录按钮、注册按钮、账号管理按钮显示状态
            //this.RegisterButton.Visibility = Visibility.Visible;
            //this.ManageUserInfoButton.Visibility = Visibility.Collapsed; //UI复原
            this.settingPopup = null;
            this.ManagePage.Quit();
            this.ManagePage = null;
            this.ReleasedAppPage.Quit();
            this.ReleasedAppPage = null;
        }

        public MainWindow()     // 主窗口初始化
        {
            //DebugConsole.DebugConsoleControl.AllocConsole();   //生成用于调试的控制台

            //Thread debugThread = new Thread(new ThreadStart(MyDebug));
            //debugThread.Start(); // 调试线程

            this.DownloadThread = new Thread(new ThreadStart(DownloadBackground));
            this.DownloadThread.Start();

            InitializeComponent();
            Cef.Initialize(new CefSharp.WinForms.CefSettings());   //初始化 Cef

            Browsers[0] = new Common.Control.ExChromiumWebBrowser(this, "http://127.0.0.1:8000/");  //实例化浏览器1---主页
            Str2Obj.Add("商店主页", Browsers[0]);
            MainFrame.Navigate(new WindowsFormsHost() { Child = Browsers[0] });

            this.LoginStatus.SetBinding(TextBlock.TextProperty, new Binding("LoginStatusStr") { Source = user }); //binding 登录状态显示
            UserHead.Source = new BitmapImage(new Uri(MyUriUtil.RelativePathProcessor() + @"Image\UserHead.png", UriKind.Absolute));  //用户头像
            userCtrl = new UserControl(this); 
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;   //
            Browsers[0].JavascriptObjectRepository.Register("csharpUserControl", userCtrl, false, 
                new BindingOptions() { CamelCaseJavascriptNames = false });

            /*
            this.TitleBackgroundBrush.ImageSource = new BitmapImage(
                new Uri(MyUriUtil.RelativePathProcessor() + @"Image\TitleBackground.jpg", UriKind.Absolute));
            */
            /*
            this.BackgroundBrush.ImageSource = new BitmapImage(
                new Uri(MyUriUtil.RelativePathProcessor() + @"Image\FrameBackground2.jpg", UriKind.Absolute));
            */

            DownloadStoryBoard = CreateDownloadStoryBoard();
            //DownloadStoryBoard.Begin(this);

            initNavigationUrl(); //导航栏以及相关的初始化
        }

        internal Storyboard CreateDownloadStoryBoard()
        {
            var dbAnim = new DoubleAnimationUsingKeyFrames() { Duration = new Duration(TimeSpan.FromMilliseconds(1500))};
            dbAnim.KeyFrames = new DoubleKeyFrameCollection();
            dbAnim.KeyFrames.Add(new LinearDoubleKeyFrame(83, KeyTime.FromPercent(0.47)));
            dbAnim.KeyFrames.Add(new LinearDoubleKeyFrame(65, KeyTime.FromPercent(0.50)));
            dbAnim.KeyFrames.Add(new LinearDoubleKeyFrame(83, KeyTime.FromPercent(0.97)));
            dbAnim.KeyFrames.Add(new LinearDoubleKeyFrame(65, KeyTime.FromPercent(1.00)));
            var dbAnim2 = new DoubleAnimationUsingKeyFrames() { Duration = new Duration(TimeSpan.FromMilliseconds(1500)) };
            dbAnim2.KeyFrames = new DoubleKeyFrameCollection();
            dbAnim2.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromPercent(0.47)));
            dbAnim2.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromPercent(0.50)));
            dbAnim2.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromPercent(0.97)));
            dbAnim2.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromPercent(1.00)));
            Storyboard.SetTargetName(dbAnim, this.downloadIcon.Name);
            Storyboard.SetTargetProperty(dbAnim, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTargetName(dbAnim2, this.downloadIcon.Name);
            Storyboard.SetTargetProperty(dbAnim2, new PropertyPath(OpacityProperty));
            var storyBoard = new Storyboard();
            storyBoard.Children.Add(dbAnim);
            storyBoard.Children.Add(dbAnim2);
            return storyBoard;
        }

        internal void initNavigationUrl()
        {
            this.NavigationStackPanel.Visibility = Visibility.Collapsed;
            //字典中加入浏览器
            Str2Obj.Add("应用发布", Browsers[1] = new Common.Control.ExChromiumWebBrowser(this, "http://127.0.0.1:8000/submit_req/"));
            Str2Obj.Add("我的收藏", "我的收藏");
            //Str2Obj.Add("我的收藏", Browsers[2] = new Common.Control.ExChromiumWebBrowser(this, "http://127.0.0.1:8000/favorapps/"));
            Str2Obj.Add("审核发布", Browsers[2] = new Common.Control.ExChromiumWebBrowser(this, "http://127.0.0.1:8000/Administrate/Reqlist/"));
            Str2Obj.Add("我的消息", Browsers[3] = new Common.Control.ExChromiumWebBrowser(this, "http://127.0.0.1:8000/dialog/"));
            //字典中加入网址
            Str2Obj.Add("主页地址", "http://127.0.0.1:8000/");
            //Str2Obj.Add("收藏地址", "http://127.0.0.1:8000/favorapps/");
            Str2Obj.Add("消息地址", "http://127.0.0.1:8000/dialog/");
            Str2Obj.Add("发布地址", "http://127.0.0.1:8000/submit_req/");
            Str2Obj.Add("管理员页面地址", "http://127.0.0.1:8000/Administrate/Reqlist/");

            int num = VisualTreeHelper.GetChildrenCount(this.NavigationStackPanel); // 设置分栏按钮的父窗口属性（字段）
            for(int i = 0; i < num; ++i)
            {
                (VisualTreeHelper.GetChild(this.NavigationStackPanel, i) as labelButton).mainwd = this;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Browsers[0].Load("http://127.0.0.1:8000/login/");
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Browsers[0].Load("http://127.0.0.1:8000/register");
        }

        public class UserControl
        {
            MainWindow mainwd;
            public String userName { get; set; }

            public UserControl(MainWindow mw) { mainwd = mw; }

            public void UserLogined()
            {
                if (mainwd.user.LoginStatus == User.loginstatus.login) return;
                mainwd.Dispatcher.Invoke(new Action(() => { mainwd.LoginInitAsync(this.userName); }));   
            }
            
            public void UserInfoUpdated()
            {

            }

            public void UserQuit()
            {
                mainwd.Dispatcher.Invoke(new Action(() => {
                    mainwd.Logout();
                }));
            }

            public int GetAppState(int AppSeriesId)
            {
                if(mainwd.ManagePage == null)
                {
                    return -1;
                }
                if (mainwd.ManagePage.Int2GameApp.ContainsKey(AppSeriesId))
                {
                    GameApp app = mainwd.ManagePage.Int2GameApp[AppSeriesId]; //这个信息来自服务器
                    if (mainwd.userSetting.Apps.ContainsKey(app.AppName))
                    {
                        GameApp lapp = mainwd.userSetting.Apps[app.AppName]; //这个信息来自本地，是下载应用的时候存储在本地的
                        if(app.Version == lapp.Version)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
        }

        public void ManageUserInfoButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("修改密码");  //调试语句，后续合并后删掉
            if (canPopup.canPopup)
            {
                (popupWindow = new PopupWindow((Str2Obj["主页地址"] as String) + "modifypassword/", canPopup)).Show();
            }
            else
            {
                popupWindow.Focus();
            }
        }

        private void UserHead_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if(this.settingPopup != null)
            {
                this.settingPopup.PlacementTarget = btn;
                this.settingPopup.IsOpen = true;
            }
        }

        public async void DownloadBackground()
        {
            BackgroundTaskParams para = null;
            Console.WriteLine("我开始默默地下载");
            while (true)
            {
                lock (_locker)
                {
                    if (this.BackgroundTasks.Count > 0)
                    {
                        para = this.BackgroundTasks.Dequeue();
                    }
                    else
                    {
                        para = null;
                    }
                }
                if(para != null)
                {
                    Console.WriteLine("一个任务开始");
                    //String filename = MyUriUtil.GetFilenameFromUrl(para.url);
                    Console.WriteLine(para.url);
                    byte[] bytes = await MyWebUtil.GetBytesByUrlAsync(para.url);
                    //await MyWebUtil.DownloadFileByUrlAsync(para.url, para.basepath + filename);
                    Console.WriteLine("下载中...");
                    
                    if(para.source != null)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        MemoryStream ms = new MemoryStream(bytes);
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        para.source.tileImgSrc = bitmap;
                        bitmap = null;
                        ms.Close();
                        ms.Dispose();
                    }
                    
                    Console.WriteLine("一个任务结束");
                }
                else
                {
                    Console.WriteLine("等待");
                    this.WaitHandle.WaitOne();
                }
            }
        }

        public void NavigationButtonChange(int type)
        {
            if(type == 0)
            {
                labelButton lbtn = VisualTreeHelper.GetChild(this.NavigationStackPanel, 1) as WpfApp_web.labelButton;
                lbtn.BorderThickness = new Thickness(0, 0, 0, 1.5);
                lbtn.BorderBrush = new SolidColorBrush(Color.FromRgb(60, 240, 60));
            }
            if(type == 1)
            {
                labelButton lbtn = VisualTreeHelper.GetChild(this.NavigationStackPanel, 1) as WpfApp_web.labelButton;
                lbtn.BorderThickness = new Thickness(0);
                lbtn.BorderBrush = new SolidColorBrush(Colors.Black);
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.BorderThickness = new Thickness(1);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.BorderThickness = new Thickness(0);
        }
    }

    public partial class labelButton : Button
    {
        public MainWindow mainwd;

        public bool selected;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                if (selected)
                {
                    this.Background.Opacity = 1;
                }
                else
                {
                    this.Background.Opacity = 0;
                }
            }
        }
        public labelButton() : base()
        {
            this.Background = new SolidColorBrush() { Opacity = 1, Color = Color.FromRgb(255, 255, 255) };
            Selected = false;
            this.Click += new RoutedEventHandler(ClickSelect);
        }
        public void ClickSelect(object sender, RoutedEventArgs e)
        {
            labelButton btn = sender as labelButton;
            if (btn.Selected)
            {
                return;
            }
            DependencyObject ctrl = VisualTreeHelper.GetParent(btn);
            Console.WriteLine(ctrl); //调试语句，后续删掉
            int num = VisualTreeHelper.GetChildrenCount(ctrl);
            for(int i = 0; i < num; ++i)
            {
                labelButton lbtn = VisualTreeHelper.GetChild(ctrl, i) as labelButton;
                lbtn.Selected = false;
            } //其余的按钮选择状态置为 false
            btn.Selected = true; //设置选择状态

            Object tmpobj = mainwd.Str2Obj[btn.Content.ToString()];
            if(tmpobj is Page1)
            {
                mainwd.MainFrame.Navigate(tmpobj);
            }
            else if(tmpobj is String)
            {
                if((String)tmpobj == "我的收藏")
                {
                    mainwd.MainFrame.Navigate(new FavorPage(mainwd));
                }
            }
            else
            {
                mainwd.MainFrame.Navigate(new WindowsFormsHost() { Child = tmpobj as System.Windows.Forms.Control });
            }
        }
    }
}
