using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.IO;

using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace WpfApp_web
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        
    }

    public class MyUriUtil
    {
        public static String RelativePathProcessor()
        {
            String currentAddress = AppDomain.CurrentDomain.BaseDirectory;
            String targetUri = "";
            String[] tmpSplitList = currentAddress.Split('\\');
            for (int i = 0; i < tmpSplitList.Length; ++i)
            {
                if (tmpSplitList[i] != "")
                {
                    targetUri += tmpSplitList[i];
                    targetUri += "\\";
                }
                else
                {
                    break;
                }
            }
            return targetUri;
        }
        public static String GetFilenameFromUrl(String url)
        {
            String[] tmpSplitList = url.Split('/');
            if (tmpSplitList.Length < 1) return null;
            return tmpSplitList[tmpSplitList.Length - 1].TrimEnd(new char[] { ' ', '\n' });
        }
        public static String[] GetAppSIdFromUrl(String url)
        {
            String[] list = url.Split('/');
            if (list.Length < 7) return null;
            String[] res = new String[] { list[5], list[6] };
            return res;
        }
        public static String GetFileTypeFromUrl(String url)
        {
            String name = GetFilenameFromUrl(url);
            if (name == null) return null;
            String[] tmpSplitList = name.Split('.');
            if (tmpSplitList.Length < 2) return null;
            return tmpSplitList[tmpSplitList.Length - 1].TrimEnd(new char[] { ' ', '\n' });
        }
    }

    public class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("UserId")]
        public int UserID { get; set; }
        [JsonProperty("UserName")]
        public String UserName { get; set; }
        [JsonProperty("Password")]
        public String Password { get; set; }
        [JsonProperty("PhoneNumber")]
        public String PhoneNumber { get; set; }
        [JsonProperty("e_mail")]
        public String e_mail { get; set; }
        [JsonProperty("is_root")]
        public bool is_root { get; set; }
        [JsonProperty("headurl")]
        public String HeadUrl;

        public GameAppList DownloadApps;
        public GameAppList ReleasedApps;
        
        public enum loginstatus
        {
            login, notlogin
        };
        public loginstatus LoginStatus
        {
            get
            {
                return userloginstatus;
            }
            set
            {
                userloginstatus = value;
                switch (userloginstatus)
                {
                    case loginstatus.login: LoginStatusStr = UserName; break;
                    case loginstatus.notlogin: LoginStatusStr = "未登录"; break;
                }
            }
        } //属性 登录状态 枚举类型
        private loginstatus userloginstatus;  //字段
        private String LoginStatusStr_field;  //字段
        public String LoginStatusStr
        {
            get { return LoginStatusStr_field; }
            set
            {
                LoginStatusStr_field = value;
                if(this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LoginStatusStr"));
                }
            }
        }  //属性 登录状态描述字符串 String类型

        public User()
        {
            UserID = 0;
            UserName = "user";
            Password = "";
            PhoneNumber = "";
            e_mail = "";
            is_root = false;
            LoginStatus = loginstatus.notlogin;
        }

        public void Update(User u)
        {
            this.UserID = u.UserID;
            this.UserName = u.UserName;
            this.Password = u.Password;
            this.PhoneNumber = u.PhoneNumber;
            this.e_mail = u.e_mail;
            this.is_root = u.is_root;
            this.HeadUrl = u.HeadUrl;
        }

        //异步获取已发布应用信息
        public async void GetReleasedAppsAsync(Page1 page)
        {
            String url = @"http://127.0.0.1:8000/api/get_release/?username=" + WebUtility.UrlEncode(this.UserName);
            String jsonStr = await MyWebUtil.ReadStringByUrlAsync(url);
            this.ReleasedApps = JsonConvert.DeserializeObject<GameAppList>(jsonStr); //获取已发布应用列表
            TileItem tmp;
            foreach (var item in this.ReleasedApps.apps)
            {
                page.Dispatcher.Invoke(new Action(() => {
                    if (!page.Int2GameApp.ContainsKey(item.AppSeriesID))
                    {
                        page.Int2GameApp.Add(item.AppSeriesID, item); //修改字典
                        page.itemList.Add(tmp = new TileItem(item.AppSeriesID, item.AppName, "SteelBlue")); //修改列表 (该列表绑定至磁贴)
                        page.mainwd.AddTask(
                            tmp,
                            @"http://127.0.0.1:8000" + item.ImgSrc,
                            MyUriUtil.RelativePathProcessor() + @"\settings\" + page.mainwd.user.UserName + @"\"
                        ); //在背景线程中下载图片
                    }
                }));  
            }
        }

        //异步获取已下载应用信息
        public async void GetDownloadedAppsAsync(Page1 page)
        {
            String url = @"http://127.0.0.1:8000/api/downloadlist/?username=" + WebUtility.UrlEncode(this.UserName);
            String jsonStr = await MyWebUtil.ReadStringByUrlAsync(url);
            this.DownloadApps = JsonConvert.DeserializeObject<GameAppList>(jsonStr);
            TileItem tmp;
            foreach (var item in this.DownloadApps.apps)
            {
                page.Dispatcher.Invoke(new Action(() => {
                    if (!page.Int2GameApp.ContainsKey(item.AppSeriesID))
                    {
                        page.Int2GameApp.Add(item.AppSeriesID, item);
                        page.itemList.Add(tmp = new TileItem(item.AppSeriesID, item.AppName, "SteelBlue")); //同一个大ID只会有至多一项
                        page.mainwd.AddTask(
                            tmp,
                            @"http://127.0.0.1:8000" + item.ImgSrc,
                            MyUriUtil.RelativePathProcessor() + @"\settings\" + page.mainwd.user.UserName + @"\"
                        );
                    }
                }));
            }
        }

        public async void GetUserHead(Image img)
        {
            //MainWindow mainwd = Application.Current.MainWindow as MainWindow;
            try
            {
                String url = @"http://127.0.0.1:8000" + this.HeadUrl;
                Console.WriteLine(url);
                byte[] bytes = await MyWebUtil.GetBytesByUrlAsync(url);
                BitmapImage bitmap = new BitmapImage();
                MemoryStream ms = new MemoryStream(bytes);
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                img.Source = bitmap;
                bitmap = null;
                ms.Close();
                ms.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static async Task<User> GetUserInfo(String name)
        {
            String url = @"http://127.0.0.1:8000/api/userinfo/?username=" + WebUtility.UrlEncode(name);
            String json = await MyWebUtil.ReadStringByUrlAsync(url);
            User user = JsonConvert.DeserializeObject<User>(json);
            return user;
        }

        public override String ToString()
        {
            return String.Format("UserID {0}\nUserName {1}\nPassword {2}\nis_root {3}\n", this.UserID, this.UserName, this.Password, this.is_root);
        }
    }

    public class GameApp
    {
        //小ID
        [JsonProperty("appid")]
        public int AppID { get; set; }
        //大ID
        [JsonProperty("app_series_id")]
        public int AppSeriesID { get; set; }
        //名称
        [JsonProperty("name")]
        public String AppName = "";
        [JsonProperty("Appdescription")]
        public String AppDescription = "";
        [JsonProperty("Covurl")]
        public String ImgSrc = "";
        [JsonProperty("add_time")]
        public String AddTime = ""; //后续可能改成 date 类
        [JsonProperty("Status")]
        public String Status = "";
        [JsonProperty("Version")]
        public String Version = "";
        [JsonProperty("Label")]
        public String Label = "";
        [JsonProperty("Uploader")]
        public String Uploader = "";
        [JsonProperty("Starter")]
        public String Starter = "";
        //最新下载链接
        [JsonProperty("newurl")]
        public String Newurl;
        //本地存储目录
        [JsonProperty("LocalPath")]
        public String LocalPath = "";

        public GameApp(int id = 0, String name = "", String description="", String imgsrc = "", String addTime = "", String status = "",  String version = "", String label = "")
        {
            AppID = id;
            AppName = name;
            AppDescription = description;
            ImgSrc = imgsrc;
            AddTime = addTime;
            Status = status;
            Version = version;
            Label = label;
        }
    }

    public class GameAppList
    {
        [JsonProperty("apps")]
        public List<GameApp> apps;
    }

    public class MyWebUtil
    {
        public static async Task<String> ReadStringByUrlAsync(String url)
        {
            String res;
            HttpClient client = new HttpClient();
            HttpResponseMessage responseMessage = await client.GetAsync(url);
            res = await responseMessage.Content.ReadAsStringAsync();
            return res;
        }
        public static async Task DownloadFileByUrlAsync(String url, String path)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (compatible; Baiduspider/2.0; +http://www.baidu.com/search/spider.html)");
            byte[] bytes = await client.GetByteArrayAsync(url);
            FileStream fs = new FileStream(path, FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }
        public static async Task<byte[]> GetBytesByUrlAsync(String url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (compatible; Baiduspider/2.0; +http://www.baidu.com/search/spider.html)");
            byte[] bytes = await client.GetByteArrayAsync(url);
            return bytes;
        }
    }

    public class LocalGameFile
    {
        [JsonProperty("game")]
        public GameApp game;
        [JsonProperty("fileList")]
        public List<String> fileList = new List<String>();
        [JsonProperty("md5List")]
        public List<String> md5List = new List<String>();
        public String Build(String gamePath, String starterPath, String savePath)
        {
            try
            {
                Console.WriteLine(gamePath);
                if (!Directory.Exists(gamePath) || !File.Exists(starterPath))
                {
                    return "路径不存在";
                }
                /*
                using (StreamReader reader = new StreamReader(settingFilePath))
                {
                    String json = reader.ReadToEnd();
                    this.game = JsonConvert.DeserializeObject<GameApp>(json);
                }
                */
                this.game = new GameApp() { Starter = starterPath.Substring(gamePath.Length) };
                ComputeHash(gamePath, gamePath.Length);
                Save(savePath);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return e.ToString();
            }
        }
        public void ComputeHash(String path, int prefixLength)
        {
            String[] files = System.IO.Directory.GetFiles(path);
            String[] directories = System.IO.Directory.GetDirectories(path);
            String md5 = "";
            foreach (String file in files)
            {
                md5 = MD5Checker.GetMD5(file);
                if(md5 != null)
                {
                    this.fileList.Add(file.Substring(prefixLength));
                    this.md5List.Add(md5);
                }
            }
            foreach (String directory in directories)
            {
                ComputeHash(directory, prefixLength);
            }
        }
        public bool Save(String path)
        {           
            try
            {
                String json = JsonConvert.SerializeObject(this);
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(json);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        public String Check(String rtPath)
        {
            String md5 = null;
            int num = this.fileList.Count;
            try
            {
                for(int i = 0; i < num; ++i)
                {
                    md5 = MD5Checker.GetMD5(rtPath + this.fileList[i]);
                    if(md5 == null)
                    {
                        return String.Format("缺失文件 {0}", this.fileList[i]);
                    }
                    else if(md5 != this.md5List[i])
                    {
                        return String.Format("文件 {0} 损坏", this.fileList[i]);
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }

    public class MD5Checker
    {
        public static string GetMD5(String path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
            byte[] buf = md5Provider.ComputeHash(fs);
            String hashStr = BitConverter.ToString(buf);
            md5Provider.Clear();
            fs.Close();
            return hashStr;
        }
    }
}
