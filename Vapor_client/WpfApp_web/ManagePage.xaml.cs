using Newtonsoft.Json;
using System;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.Integration;
using System.Windows.Media.Animation;
using System.Net;
using System.Collections.ObjectModel;
using System.Globalization;

namespace WpfApp_web
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    /// 
    public class Setting : INotifyPropertyChanged
    {
        //[JsonProperty("Paths")]
        //public List<String> Paths = new List<String>();
        [JsonProperty("Apps")]
        public Dictionary<String, GameApp> Apps = new Dictionary<String, GameApp>();
        [JsonProperty("currentPath")]
        public String currentPath
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                if(this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("currentPath"));
                }
            }
        }

        public String path;

        public event PropertyChangedEventHandler PropertyChanged;

        public Setting()
        {

        }
        /*
        public void AddDownloadPath(String path)
        {
            Paths.Add(path);
        }
        */
        public void AddApp(String name, GameApp gapp)
        {
            if (this.Apps.ContainsKey(name))
            {
                this.Apps[name] = gapp;
            }
            else
            {
                this.Apps.Add(name, gapp);
            }
        }
        public static void Save(Setting setting, String savepath)
        {
            String json = JsonConvert.SerializeObject(setting);
            Console.WriteLine(json);
            try
            {
                using (StreamWriter sw = new StreamWriter(savepath))
                {
                    sw.Write(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void Load(ref Setting setting, String loadpath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(loadpath))
                {
                    String json = sr.ReadToEnd();
                    setting = JsonConvert.DeserializeObject<Setting>(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class DownloadControl
    {
        Page1 page;
        MainWindow mainwd;
        public DownloadControl(MainWindow mwd)
        {
            this.mainwd = mwd;
            this.page = mainwd.ManagePage;
        }
        public async void NewAppDownloadedAsync(Object argv) //(GameApp app, String path)
        {
            Object[] paras = (Object[])argv;
            GameApp app = paras[0] as GameApp;
            String path = paras[1] as String;
            MessagePopup messagePopup;
            //需要获悉本次下载是新下载还是更新下载
            int old_id;
            if (mainwd.userSetting.Apps.ContainsKey(app.AppName))
            {
                old_id = mainwd.userSetting.Apps[app.AppName].AppID;
            }
            else
            {
                old_id = -1;
            }
            /**** 解压缩方面的工作 ****/
            mainwd.Dispatcher.Invoke(new Action(() => {
                messagePopup = new MessagePopup(app.AppName + "正在解压缩", 1500) { Owner = mainwd, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                messagePopup.Show();
            })); //这里需要 dispatcher
            String rtPath = System.IO.Path.GetDirectoryName(path);
            if(InstallUtil.Uncompress.UncompressFile(path, rtPath))
            {
                app.LocalPath = String.Format(@"{0}\{1}", rtPath, app.AppName);
                mainwd.Dispatcher.Invoke(new Action(() =>
                {
                    mainwd.userSetting.AddApp(app.AppName, app); //向setting中添加,如果存在则覆盖
                    Setting.Save(mainwd.userSetting, MyUriUtil.RelativePathProcessor() + @"\settings\" + mainwd.user.UserName + @"\settings.json");
                    messagePopup = new MessagePopup(app.AppName + "解压缩完成", 1500) { Owner = mainwd, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                    messagePopup.Show();
                })); //这里需要 dispatcher
                /**** UI 方面的工作 ****/
                TileItem tmp = new TileItem(app.AppSeriesID, app.AppName, "SteelBlue");
                mainwd.AddTask(
                    tmp,
                    @"http://127.0.0.1:8000" + app.ImgSrc,
                    MyUriUtil.RelativePathProcessor() + @"\settings\" + mainwd.user.UserName + @"\"
                );
                page.Dispatcher.Invoke(new Action(() => {
                    if (page.Int2GameApp.ContainsKey(app.AppSeriesID))
                    {
                        page.Int2GameApp[app.AppSeriesID] = app;
                    }
                    else
                    {
                        page.Int2GameApp.Add(app.AppSeriesID, app);
                        page.itemList.Add(tmp);
                    }
                }));
                //tell the server
                if (old_id >= 0)
                {
                    String url_ = String.Format(@"http://127.0.0.1:8000/api/remove_download/?user={0}&appid={1}", mainwd.user.UserName, old_id);
                    String tmpstr_ = await MyWebUtil.ReadStringByUrlAsync(url_);
                } // 如果以前下载过，则请求服务器删除原来的记录
                String url = String.Format(@"http://127.0.0.1:8000/api/add_download/?username={0}&appid={1}", mainwd.user.UserName, app.AppID);
                String tmpstr = await MyWebUtil.ReadStringByUrlAsync(url);
                mainwd.Dispatcher.Invoke(new Action(() => {
                    mainwd.Browsers[0].Load(mainwd.Browsers[0].Address);
                }));
            }
            else
            {
                messagePopup = new MessagePopup("解压缩时出错", 1500) { Owner = mainwd, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                messagePopup.Show();
            }
        }
    }

    public class TileItem : INotifyPropertyChanged
    {
        private String color;
        public String tileColor
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("tileColor"));
                }
            }
        }
        private String title;
        public String tileTitle
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("tileTitle"));
                }
            }
        }
        private int id;
        public int tileId
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                if(this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("tileId"));
                }
            }
        }
        private BitmapImage imgsrc;
        public BitmapImage tileImgSrc
        {
            get
            {
                return imgsrc;
            }
            set
            {
                imgsrc = value;
                if(this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("tileImgSrc"));
                }
            }
        }
        private bool onhover;
        public bool onHover
        {
            get
            {
                return onhover;
            }
            set
            {
                onhover = value;
                if(this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("onHover"));
                }
            }
        }
        private bool available;
        public bool Available
        {
            get
            {
                return available;
            }
            set
            {
                available = value;
                if(this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Available"));
                }
            }
        }

        public TileItem(int _id, String _title, String _color)
        {
            tileId = _id;
            tileTitle = _title;
            tileColor = _color;
            tileImgSrc = null;
            onHover = false;
            Available = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ExTile : Tile
    {
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(int), typeof(ExTile));
        public static readonly DependencyProperty RealTitleProperty = DependencyProperty.Register("RealTitle", typeof(String), typeof(ExTile));
        public static readonly DependencyProperty OnHoverProperty = DependencyProperty.Register("OnHover", typeof(bool), typeof(ExTile));
        public int Id
        {
            get
            {
                return (int)GetValue(IdProperty);
            }
            set
            {
                SetValue(IdProperty, value);
            }
        }
        public String RealTitle
        {
            get
            {
                return (String)GetValue(RealTitleProperty);
            }
            set
            {
                SetValue(RealTitleProperty, value);
            }
        }
        public bool OnHover
        {
            get
            {
                return (bool)GetValue(OnHoverProperty);
            }
            set
            {
                SetValue(OnHoverProperty, value);
                //Console.WriteLine("ExTile SetValue {0}", value);
            }
        }
        public bool showImg;
        public ExTile() : base()
        {
            showImg = true;
            OnHover = false;
            //Content = "Image";
        }
    }

    public class OnHoverToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            if (b)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class OnHoverToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            if (b)
            {
                return (double)0.5;
            }
            else
            {
                return (double)0.0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class AppInfoPanel : StackPanel
    {
        private Page1 page;

        public Button gotoAppPage;
        public Button checkFile;
        public Button uninstallButton;
        public Button startGame;

        public VersionBlock versionInfo;
        public TextBlock statusInfo;

        public GameApp gapp; // 这个是服务器给的信息
        public GameApp localInfo; // 这个是本地给的信息，依据服务器的信息在本地的setting中查找出本地的gapp info
        public GameStatus status;
        public int type;
        public Setting setting;

        public enum GameStatus
        {
            complete, incomplete, missing
        };

        public AppInfoPanel(GameApp _gapp, Page1 _page, int ty = 1) : base()
        {
            this.page = _page;
            this.setting = (Application.Current.MainWindow as MainWindow).userSetting;
            this.type = ty;
            this.gapp = _gapp;
            if (this.type == 1)
            {
                this.GetLocalInfo();
            }

            this.Orientation = Orientation.Vertical;  //垂直布局

            this.Children.Add(new TextBlock() { Text = gapp.AppName, Margin = new Thickness(0, 5, 0, 25), FontSize = 24});

            this.Children.Add(new TextBlock() { Text = "发布者: " + gapp.Uploader, Margin = new Thickness(0, 1, 0, 1), FontSize = 14 });

            if (localInfo != null)
            {
                this.Children.Add(this.versionInfo = new VersionBlock(this, localInfo.Version, localInfo.Version == gapp.Version));
            }

            this.Children.Add(this.statusInfo = new TextBlock() { Text = "应用状态: " + gapp.Status, Margin = new Thickness(0, 1, 0, 1), FontSize = 14 });

            if (this.type == 1)
            {
                this.Children.Add(startGame = new Button()
                {
                    Content = new TextBlock() { Text = "启动游戏" },
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0, 2, 0, 2),
                });
                startGame.Click += StartGame_Click;
                this.Children.Add(checkFile = new Button()
                {
                    Content = new TextBlock() { Text = "检查游戏完整性" },
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0, 2, 0, 2),
                });
                checkFile.Click += CheckFile_Click;
            }
            else if(this.type == 2)
            {
                this.Children.Add(checkFile = new Button()
                {
                    Content = new TextBlock() { Text = "发布新版本" },
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0, 2, 0, 2),
                });
                checkFile.Click += ReleaseNewVersion;
            }

            this.Children.Add(gotoAppPage = new Button() {
                Content = new TextBlock() { Text = "前往APP页面" },
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 2, 0, 2),
            });
            gotoAppPage.Click += GotoAppPage_Click;

            if(this.type == 1)
            {
                this.Children.Add(uninstallButton = new Button()
                {
                    Content = new TextBlock() { Text = "卸载游戏" },
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0, 2, 0, 2),
                });
                uninstallButton.Click += UninstallButton_Click;
            }
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (localInfo != null && localInfo.Starter != null)
            {
                try
                {
                    String json;
                    //读取Starter信息
                    using (StreamReader sr = new StreamReader(localInfo.LocalPath + @"\VaporCheck.json"))
                    {
                        json = sr.ReadToEnd();
                    }
                    if(json == null || json == "")
                    {
                        MessageBox.Show("游戏验证文件缺失");
                        return ;
                    }
                    LocalGameFile lgf = JsonConvert.DeserializeObject<LocalGameFile>(json);
                    Console.WriteLine(localInfo.LocalPath + lgf.game.Starter);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = localInfo.LocalPath + lgf.game.Starter,
                        WorkingDirectory = System.IO.Path.GetDirectoryName(localInfo.LocalPath + lgf.game.Starter)
                    });
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                (btn.Content as TextBlock).Text = "正在卸载中...";
                btn.IsEnabled = false;
                Thread th = new Thread(new ThreadStart(DeleteDirectory));
                th.Start();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }
        }

        private void DeleteDirectory()
        {
            try
            {
                if (!Directory.Exists(localInfo.LocalPath))
                {
                    this.page.Dispatcher.Invoke(new Action(() => {
                        MessageBox.Show("游戏目录缺失，游戏已移除");
                        this.page.RemoveAppAsync(this.gapp);
                    }));
                }
                else
                {
                    Directory.Delete(this.localInfo.LocalPath, true);
                    this.page.Dispatcher.Invoke(new Action(() => {
                        MessageBox.Show("卸载成功");
                        this.page.RemoveAppAsync(this.gapp);
                    }));
                } 
            }
            catch (Exception err)
            {
                this.Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show("卸载异常" + err.ToString());
                    //(this.uninstallButton.Content as TextBlock).Text = "卸载游戏";
                    //this.uninstallButton.IsEnabled = true;
                    this.page.RemoveAppAsync(this.gapp);
                }));
            }
        }

        private void ReleaseNewVersion(object sender, RoutedEventArgs e)
        {
            MainWindow mainwd = Application.Current.MainWindow as MainWindow;
            mainwd.Browsers[1].Load(String.Format("http://127.0.0.1:8000/updateapp/?appsid={0}", gapp.AppSeriesID));
            Button btn = VisualTreeHelper.GetChild(mainwd.NavigationStackPanel, 2) as Button;
            btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void CheckFile_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("检查游戏完整性");
            Thread th = new Thread(new ThreadStart(CheckStarter));
            th.Start();
            this.checkFile.IsEnabled = false;
            this.checkFile.Content = "正在检查完整性...";
        }

        private GameStatus Check()
        {
            if (localInfo == null || localInfo.LocalPath == null)
            {
                return GameStatus.missing;
            } //只要不为空，则可以访问，如果串是""，或者其他非法的串，会在下面的try中触发异常
            try
            {
                String json = null;
                using (StreamReader sr = new StreamReader(localInfo.LocalPath + @"\VaporCheck.json"))
                {
                    json = sr.ReadToEnd();
                }
                LocalGameFile lgf = JsonConvert.DeserializeObject<LocalGameFile>(json);
                if (lgf == null)
                {
                    return GameStatus.missing;
                }
                String checkRes = lgf.Check(localInfo.LocalPath);
                if (checkRes == "")
                {
                    return GameStatus.complete; //返回空串(不是null)表示完整
                }
                else
                {
                    gapp.Status = checkRes;
                    return GameStatus.incomplete;
                }
            }
            catch
            {
                return GameStatus.missing;
            }
        }

        private void CheckStarter()
        {
            GameStatus tmp = this.Check(); //这一步可能耗时很久，为避免主线程UI卡死，该函数是在新建的线程中进行的
            //后续的操作耗时可以忽略
            this.Dispatcher.Invoke(new Action(() => {
                status = tmp;
                if (status == GameStatus.complete)
                {
                    gapp.Status = "已下载(完整)"; //gapp实际上是itemlist中app的引用，这里进行赋值会修改列表中app的status信息
                }
                else if (status == GameStatus.missing)
                {
                    gapp.Status = "游戏目录缺失";
                }
                else if (status == GameStatus.incomplete)
                {
                    
                }
                statusInfo.Text = "应用状态: " + gapp.Status;
                this.checkFile.Content = "检查游戏完整性";
                this.checkFile.IsEnabled = true;
            }));
        }

        private void GotoAppPage_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwd = Application.Current.MainWindow as MainWindow; //获取当前应用主窗口
            Button btn = VisualTreeHelper.GetChild(mainwd.NavigationStackPanel, 0) as Button; //获取导航标签的第一个子元素
            mainwd.Browsers[0].Load("http://127.0.0.1:8000/apps/?appsid=" + this.gapp.AppSeriesID);
            btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); //触发该子元素（按钮）的 Click 事件
        }

        private void GetLocalInfo()
        {
            if (setting.Apps.ContainsKey(gapp.AppName))
            {
                this.localInfo = setting.Apps[gapp.AppName];
            }
            else
            {
                this.gapp.Status = "本机未安装该应用";
            }
        }

        public void Update()
        {
            this.page.mainwd.Dispatcher.Invoke(new Action(() => {
                this.page.mainwd.Browsers[0].Load(String.Format("http://127.0.0.1:8000/apps/?appsid={0}", gapp.AppSeriesID));
                Button btn = VisualTreeHelper.GetChild(this.page.mainwd.NavigationStackPanel, 0) as Button;
                btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }));
            this.versionInfo.is_newest = true;
        }
    }

    public partial class Page1 : Page
    {
        public MainWindow mainwd;
        public int type;
        public ObservableCollection<TileItem> itemList;

        String[] TileColors = new String[3] { "SteelBlue", "SeaGreen", "OrangeRed" };
        //GameAppList gameApps;
        public Dictionary<int, GameApp> Int2GameApp = new Dictionary<int, GameApp>();

        public Storyboard infoPanelPopup1()
        {
            var doubleAnimation = new DoubleAnimation() { From = 0, To = 330, AutoReverse = false };
            doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));
            var storyBoard = new Storyboard();
            storyBoard.Children.Add(doubleAnimation);
            Storyboard.SetTargetName(doubleAnimation, AppDetailsPanel.Name);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(WrapPanel.WidthProperty));
            return storyBoard;
        }

        public Storyboard infoPanelPopup2()
        {
            var doubleAnimation = new DoubleAnimation() { From = 330, To = 0, AutoReverse = false };
            doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));
            var storyBoard = new Storyboard();
            storyBoard.Children.Add(doubleAnimation);
            Storyboard.SetTargetName(doubleAnimation, AppDetailsPanel.Name);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(WrapPanel.WidthProperty));
            return storyBoard;
        }

        Storyboard infoPanelStoryBoard1, infoPanelStoryBoard2;

        public Page1(MainWindow wd, int ty = 1)
        {
            this.itemList = new ObservableCollection<TileItem>();
            this.mainwd = wd;
            this.type = ty;

            InitializeComponent();

            this.AppsPanel.Dispatcher.Invoke(new Action(() => {
                this.AppsPanel.ItemsSource = itemList;
            }));  //为磁贴绑定数据 磁贴会显示列表中的app
            //这是项目早期的代码，数据绑定的思想运用不充分
            //AppInfoPanel没有运用数据绑定，写法不够优雅
            this.infoPanelStoryBoard1 = infoPanelPopup1(); //设置动画
            this.infoPanelStoryBoard2 = infoPanelPopup2(); //设置动画

            if(ty == 2)
            {
                this.listTitle.Text = "已发布游戏列表";
            }
        }

        public void Quit()
        {
            foreach (var item in this.itemList)
            {
                item.tileImgSrc = null;
            }
            itemList = null;
            int num = VisualTreeHelper.GetChildrenCount(this.AppsPanel);
            Console.WriteLine(num);
            for(int i = 0; i < num; ++i)
            {
                ExTile item = VisualTreeHelper.GetChild(this.AppsPanel, i) as ExTile;
                if (item == null || item.Content == null) continue;
                (item.Content as Image).Source = null;
            }
            GC.Collect();
        }

        private void Tile_MouseEnter(object sender, MouseEventArgs e)
        {
            ExTile tile = sender as ExTile;
            tile.BorderThickness = new Thickness(2);
            tile.OnHover = true;
        }

        private void Tile_MouseLeave(object sender, MouseEventArgs e)
        {
            ExTile tile = sender as ExTile;
            tile.BorderThickness = new Thickness(0);
            tile.OnHover = false;
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            ExTile extile = sender as ExTile;
            if(this.AppDetailsPanel.Width < 300)
            {
                this.infoPanelStoryBoard1.Begin(this);
            }
            this.AppInfoBorder.Child = new AppInfoPanel(Int2GameApp[extile.Id], this, this.type);
        }

        private void CloseAppDetailsPanelButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.AppDetailsPanel.Width > 300)
            {
                this.infoPanelStoryBoard2.Begin(this);
            }
        }

        public async void RemoveAppAsync(GameApp gapp)
        {
            this.infoPanelStoryBoard2.Begin(this); //收起界面
            this.Int2GameApp.Remove(gapp.AppSeriesID); //删字典
            this.mainwd.userSetting.Apps.Remove(gapp.AppName); //删本地记录
            Setting.Save(this.mainwd.userSetting, MyUriUtil.RelativePathProcessor() + @"\settings\" + this.mainwd.user.UserName + @"\settings.json");

            foreach (var item in this.itemList)
            {
                if(item.tileId == gapp.AppSeriesID)
                {
                    this.itemList.Remove(item);
                    break;
                }
            }

            String url = String.Format(@"http://127.0.0.1:8000/api/remove_download/?user={0}&appid={1}", mainwd.user.UserName, gapp.AppID);
            String tmpstr = await MyWebUtil.ReadStringByUrlAsync(url);
        }
    }
}
