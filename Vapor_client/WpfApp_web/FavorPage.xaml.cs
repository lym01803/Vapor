using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
    /// FavorPage.xaml 的交互逻辑
    /// </summary>
    /// 
    public class FavorItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private String AppName;
        public String itemAppName
        {
            get
            {
                return AppName;
            }
            set
            {
                AppName = value;
                if(PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("itemAppName"));
                }
            }
        }
        private String Uploader;
        public String itemUploader
        {
            get
            {
                return Uploader;
            }
            set
            {
                Uploader = value;
                if(PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("itemUploader"));
                }
            }
        }
        private String AddTime;
        public String itemAddTime
        {
            get
            {
                return AddTime;
            }
            set
            {
                AddTime = value;
                if(PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("itemAddTime"));
                }
            }
        }
        private int AppSid;
        public int itemAppSid
        {
            get
            {
                return AppSid;
            }
            set
            {
                AppSid = value;
                if(PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("itemAppSid"));
                }
            }
        }
        public GameApp gapp;

        public FavorItem(GameApp _app)
        {
            this.gapp = _app;
            itemAppName = gapp.AppName;
            itemUploader = gapp.Uploader;
            itemAddTime = gapp.AddTime.Substring(0, 10);
            itemAppSid = gapp.AppSeriesID;
        }
    }

    public partial class ExContentControl: ContentControl
    {
        public static readonly DependencyProperty SidProperty = DependencyProperty.Register("Sid", typeof(int), typeof(ExContentControl));
        public int Sid
        {
            get
            {
                return (int)GetValue(SidProperty);
            }
            set
            {
                SetValue(SidProperty, value);
            }
        }
    }

    public partial class FavorPage : Page
    {
        public MainWindow mainwd;
        public ObservableCollection<FavorItem> itemList = new ObservableCollection<FavorItem>();
        public FavorPage(MainWindow mwd)
        {
            InitializeComponent();
            this.mainwd = mwd;
            LoadAsync();
        }
        async void LoadAsync()
        {
            String url = @"http://127.0.0.1:8000/api/favorlist/?username=" + WebUtility.UrlEncode(mainwd.user.UserName);
            String jsonStr = await MyWebUtil.ReadStringByUrlAsync(url);
            GameAppList favorApps = JsonConvert.DeserializeObject<GameAppList>(jsonStr);
            foreach(var app in favorApps.apps)
            {
                itemList.Add(new FavorItem(app));
            }
            this.FavorsPanel.Dispatcher.Invoke(new Action(() => {
                this.FavorsPanel.ItemsSource = itemList;
            }));
        }

        private void ContentControl_MouseEnter(object sender, MouseEventArgs e)
        {
            ContentControl control = sender as ContentControl;
            Grid grid = control.Content as Grid;
            grid.Background = new SolidColorBrush(Colors.WhiteSmoke);
        }

        private void ContentControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentControl control = sender as ContentControl;
            Grid grid = control.Content as Grid;
            grid.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void ContentControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ExContentControl exc = sender as ExContentControl;
            Button btn = VisualTreeHelper.GetChild(mainwd.NavigationStackPanel, 0) as Button; //获取导航标签的第一个子元素
            mainwd.Browsers[0].Load("http://127.0.0.1:8000/apps/?appsid=" + exc.Sid.ToString());
            btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); //触发该子元素（按钮）的 Click 事件
        }

        private void ContentControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
