using Microsoft.Win32;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace N2N_Desktop
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {

        int errMode;
        bool isLoad = false;
        bool canAdd = false;
        bool isRun = false;
        List<SaveLoader.N2NConfig> configs = new List<SaveLoader.N2NConfig>();
        SaveLoader.N2NConfig nowUsed;

        public MainWindow()
        {
            InitializeComponent();

            //判断tap-windows是否安装
            bool get = true;
            if (!Directory.Exists("Cache"))
            {
                Directory.CreateDirectory("Cache");
                get = false;
            }
            else
            {
                if(!File.Exists("Cache\\get.sscache"))
                {
                    get = false;
                }
            }
            if (!get)
            {
                try
                {
                    if (!SSUserClass.Reg.IsRegeditItemExist(Registry.LocalMachine, "SOFTWARE", "TAP-Windows"))
                    {
                        errMode = 1;
                        panErrorMsg.Visibility = Visibility.Visible;
                        errorTitle.Text = "缺少组件";
                        errorSays.Text = "未安装Tap运行环境.点击确认将尝试打开安装程序，请谨慎杀毒软件查杀！";
                    }
                    else
                    {
                        if (!Directory.Exists("Cache"))
                        {
                            Directory.CreateDirectory("Cache");
                        }
                        using (StreamWriter sw = File.AppendText("Cache\\get.sscache"))
                        {
                            sw.WriteLine("GET");
                        }
                        #if DEBUG
                        errMode = 0;
                        panErrorMsg.Visibility = Visibility.Visible;
                        errorTitle.Text = "已安装Tap";
                        errorSays.Text = "在Debug模式告诉我我已经安装了，这个检测流程没问题= =";
                        #endif
                    }
                }
                catch
                {
                    //申请管理员权限
                    Thread thread = new Thread(GetAdms);
                    thread.Start();
                }
            }
            //检测文件完整性
            if (Directory.Exists("System"))
            {
                if (!File.Exists("System\\edge.exe"))
                {
                    errMode = 3;
                    panErrorMsg.Visibility = Visibility.Visible;
                    errorTitle.Text = "缺少组件";
                    errorSays.Text = "未找到运行核心edge.exe，请确认下载的程序包完整性……";
                }
            }
            else
            {
                errMode = 3;
                panErrorMsg.Visibility = Visibility.Visible;
                errorTitle.Text = "缺少组件";
                errorSays.Text = "未找到运行核心文件夹，请确认下载的程序包完整性……";
            }

            //读取配置列表
            Thread threada = new Thread(LoadConfig);
            threada.Start();
        }

        private void GetAdms()
        {
            errMode = 2;
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                panErrorMsg.Visibility = Visibility.Visible;
                errorTitle.Text = "申请权限";
                errorSays.Text = "我们将提高权限用于检查是否安装必要组件，如果点击取消将跳过环节。";
            });
        }

        // 窗口圆角
        private void Onsizechanged(object sender, SizeChangedEventArgs e)
        {
            Rect r = new Rect(e.NewSize);
            int radius = 5;
            RectangleGeometry gm = new RectangleGeometry(r, radius, radius);
            ((UIElement)sender).Clip = gm;
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            if (isRun)
            {
                SSUserClass.Proc.KillProc(SSUserClass.Proc.GetProc("edge"));
            }
            else if (a.IsSelected)
            {
                errMode = 5;
                panErrorMsg.Visibility = Visibility.Visible;
                errorTitle.Text = "申请权限";
                errorSays.Text = "我们将提高权限用于启动连接。";
                isOpen.Text = "正在启动";
                ProgressBarHelper.SetAnimateTo(openPer, 50);
            }
            else if (b.IsSelected)
            {
                //添加
                if (configs[0].iP == "new" && !canAdd)
                {
                    canAdd = true;
                    panErrorMsg.Visibility = Visibility.Visible;
                    errMode = 0;
                    errorTitle.Text = "提醒";
                    errorSays.Text = "点击添加将将当前配置显示区的配置添加入列表（本次点击无效）。";
                    return;
                }
                bool noSave = false;
                SaveLoader.N2NConfig config = new SaveLoader.N2NConfig();
                if (!String.IsNullOrWhiteSpace(Name.Text))
                {
                    for (int i = 0; i <= configs.Count - 1; i++)
                    {
                        if ((configs[i].Name) == Name.Text)
                        {
                            errMode = 4;
                            panErrorMsg.Visibility = Visibility.Visible;
                            errorTitle.Text = "提醒";
                            errorSays.Text = "存在同名的配置将覆盖保存。";
                            return;
                        }
                    }
                }
                if (!String.IsNullOrWhiteSpace(IP.Text))
                {
                    config.iP = IP.Text;
                }
                else
                {
                    noSave = true;
                }
                if (!String.IsNullOrWhiteSpace(IPAdd.Text))
                    config.iPAdditional = IPAdd.Text;
                config.isDHCP = isDHCP.IsChecked == true ? true : false;
                if (!String.IsNullOrWhiteSpace(SeverIP.Text))
                {
                    config.severIP = SeverIP.Text;
                }
                else
                {
                    noSave = true;
                }
                if (!String.IsNullOrWhiteSpace(SeverPost.Text))
                {
                    config.severPost = Convert.ToInt32(SeverPost.Text);
                }
                else
                {
                    noSave = true;
                }
                if (!String.IsNullOrWhiteSpace(TeamName.Text))
                {
                    config.teamName = TeamName.Text;
                }
                else
                {
                    noSave = true;
                }
                if (!String.IsNullOrWhiteSpace(Password.Text))
                {
                    config.teamPassword = Password.Text;
                }
                else
                {
                    noSave = true;
                }
                if (!String.IsNullOrWhiteSpace(Name.Text))
                {
                    config.Name = Name.Text;
                }
                else
                {
                    noSave = true;
                }
                config.isUsed = false;
                config.UUID = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                if (!noSave)
                {
                    SaveLoader.AddSave(config);
                    configs.Add(config);
                    AddList(config);
                }
                else
                {
                    panErrorMsg.Visibility = Visibility.Visible;
                    errMode = 0;
                    errorTitle.Text = "提醒";
                    errorSays.Text = "请将数据填写完整。";
                }
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (a.IsSelected)
            {
                if (isRun)
                {
                    ButtonHelper.SetIcon(Run, "");
                    IconHelper.SetFontSize(Run, 15);
                }
                else
                {
                    ButtonHelper.SetIcon(Run, "");
                    IconHelper.SetFontSize(Run, 25);
                }
            }
            else if (b.IsSelected)
            {
                ButtonHelper.SetIcon(Run, "");
                IconHelper.SetFontSize(Run, 20);
            }
        }

        private void LoadConfig()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                if (!isLoad)
                {
                    isLoad = true;
                    configs = SaveLoader.LoadSave();
                    if (configs[0].error != "False")
                    {
                        errMode = 3;
                        errorTitle.Text = "读取配置错误";
                        errorSays.Text = "在读取配置的时候发现了个错误：" + configs[0].error + "，程序将退出……";
                    }
                    if (configs[0].iP == "new")
                    {
                        return;
                    }
                    for (int i = 0; i <= configs.Count - 1; i++)
                    {
                        AddList(configs[i]);
                    }
                }
            });
        }

        private void AddList(SaveLoader.N2NConfig config)
        {
            SolidColorBrush Brush = new SolidColorBrush();
            Brush.Color = Color.FromArgb(189, 230, 251, 255);


            ListViewItem item = new ListViewItem();
            WrapPanel wrap = new WrapPanel();
            RadioButton check = new RadioButton();
            TextBlock text = new TextBlock();
            item.Height = 25;
            item.Name = "UUID" + config.UUID;
            wrap.Margin = new Thickness(10, 0, 10, 0);
            wrap.VerticalAlignment = VerticalAlignment.Center;
            check.Name = "CHECK" + config.UUID;
            check.Background = Brushes.Transparent;
            check.GroupName = "Used";
            check.Checked += ChangeUsed;
            check.VerticalAlignment = VerticalAlignment.Center;
            RadioButtonHelper.SetCheckedBackground(check, Brushes.Transparent);
            RadioButtonHelper.SetGlyphBrush(check, Brushes.Transparent);
            RadioButtonHelper.SetCheckedGlyphBrush(check, Brush);
            text.Text = "| " + config.Name;
            text.Foreground = Brush;
            text.VerticalAlignment = VerticalAlignment.Center;

            if(config.isUsed)
            {
                item.IsSelected = true;
                check.IsChecked = true;
                nowUsed = config;
                nName.Text = config.Name;
                nIP.Text = config.iP;
                nAdd.Text = config.iPAdditional;
                nSever.Text = config.severIP + ":" + config.severPost;
                nTeamName.Text = config.teamName;
                nPassword.Text = config.teamPassword;
                nowConfigs.Visibility = Visibility.Visible;
            }

            wrap.Children.Add(check);
            wrap.Children.Add(text);
            item.Content = wrap;
            ConfigList.Items.Add(item);
        }

        /// <summary>
        /// 点击启用单选框触发更改启用的配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeUsed(object sender, RoutedEventArgs e)
        {
            RadioButton button = (RadioButton)sender;
            for (int i = 0; i <= configs.Count - 1; i++)
            {
                if (configs[i].isUsed == true)
                {
                    if(configs[i].UUID == button.Name.Substring(5))
                    {
                        return;
                    }
                    configs[i].isUsed = false;
                    break;
                }
            }
            for (int i=0; i <= configs.Count - 1; i++)
            {
                if(configs[i].UUID == button.Name.Substring(5))
                {
                    configs[i].isUsed = true;
                    nowUsed = configs[i];
                    nName.Text = configs[i].Name;
                    nIP.Text = configs[i].iP + " -" + configs[i].iPAdditional;
                    nAdd.Text = configs[i].iPAdditional;
                    nSever.Text = configs[i].severIP + ":" + configs[i].severPost;
                    nTeamName.Text = configs[i].teamName;
                    nPassword.Text = configs[i].teamPassword;
                    break;
                }
            }
            File.Delete("Save\\Config.ssn2n");
            for (int i = 0; i <= configs.Count - 1; i++)
            {
                SaveLoader.AddSave(configs[i]);
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            panErrorMsg.Visibility = Visibility.Collapsed;
            switch(errMode)
            {
                case 1:     //未安装Tap运行环境
                    {
                        try
                        {
                            Process.Start(Environment.CurrentDirectory + "\\System\\tap-windows.exe");
                        }
                        catch(Exception ex)
                        {
                            errMode = 3;
                            panErrorMsg.Visibility = Visibility.Visible;
                            errorTitle.Text = "打开失败";
                            errorSays.Text = "请确认下载的程序包完整性……\n" + ex;
                        }
                    }
                    break;
                case 2:
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = Process.GetCurrentProcess().MainModule.FileName;
                        psi.Verb = "runas";
                        try
                        {
                            Process.Start(psi);
                            Application.Current.Shutdown();
                        }
                        catch (Exception eE)
                        {
                            MessageBox.Show("失败:" + eE);
                            Application.Current.Shutdown();
                        }
                    }
                    break;
                case 3:
                    {
                        Application.Current.Shutdown();
                    }
                    break;
                case 4:
                    {
                        bool noSave = false;
                        int what = -1;
                        for (int i = 0; i <= configs.Count - 1; i++)
                        {
                            if ((configs[i].Name) == Name.Text)
                            {
                                what = i;
                                break;
                            }
                        }
                        if (!String.IsNullOrWhiteSpace(IP.Text))
                        {
                            configs[what].iP = IP.Text;
                        }
                        else
                        {
                            noSave = true;
                        }
                        if (!String.IsNullOrWhiteSpace(IPAdd.Text))
                            configs[what].iPAdditional = IPAdd.Text;
                        configs[what].isDHCP = isDHCP.IsChecked == true ? true : false;
                        if (!String.IsNullOrWhiteSpace(SeverIP.Text))
                        {
                            configs[what].severIP = SeverIP.Text;
                        }
                        else
                        {
                            noSave = true;
                        }
                        if (!String.IsNullOrWhiteSpace(SeverPost.Text))
                        {
                            configs[what].severPost = Convert.ToInt32(SeverPost.Text);
                        }
                        else
                        {
                            noSave = true;
                        }
                        if (!String.IsNullOrWhiteSpace(TeamName.Text))
                        {
                            configs[what].teamName = TeamName.Text;
                        }
                        else
                        {
                            noSave = true;
                        }
                        if (!String.IsNullOrWhiteSpace(Password.Text))
                        {
                            configs[what].teamPassword = Password.Text;
                        }
                        else
                        {
                            noSave = true;
                        }
                        if (!String.IsNullOrWhiteSpace(Name.Text))
                        {
                            configs[what].Name = Name.Text;
                        }
                        else
                        {
                            noSave = true;
                        }
                        configs[what].isUsed = false;
                        if (!noSave)
                        {
                            File.Delete("Save\\Config.ssn2n");
                            for (int i = 0; i <= configs.Count - 1; i++)
                            {
                                SaveLoader.AddSave(configs[i]);
                            }
                        }
                        else
                        {
                            panErrorMsg.Visibility = Visibility.Visible;
                            errMode = 0;
                            errorTitle.Text = "提醒";
                            errorSays.Text = "请将数据填写完整。";
                        }
                    }
                    break;
                case 5:
                    {
                        //基本数据
                        string IPADRESS = nowUsed.isDHCP ? "dhcp:" + nowUsed.iP : nowUsed.iP;       //自定义本机IP,DHCP请在DHCP服务器IP前加："dhcp:"
                        string DHCP = nowUsed.isDHCP ? "-r" : "";                                  //如果为DHCP请将此项改为："-r",不是DHCP请留空：""。
                        string GROUPNAME = nowUsed.teamName;                                    //填写组名
                        string PASSWORD = nowUsed.teamPassword;                                 //填写密码
                        string SUPERNODEIP = nowUsed.severIP;                                   //此项为SuperNode服务器IP(公网)
                        string SUPERNODEPORT = nowUsed.severPost.ToString();                    //此项为SuperNode服务器端口
                        string OTHERARG = nowUsed.iPAdditional;                                 //其他参数
                                                                                                //准备配置文件
                        string edgearg = DHCP + " -a " + IPADRESS + " -c " + GROUPNAME + " -k " + PASSWORD + " -l " + SUPERNODEIP + ":" + SUPERNODEPORT + OTHERARG;
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = "System\\edge.exe";
                        psi.Verb = "runas";
                        psi.Arguments = edgearg;
                        try
                        {
                            Process.Start(psi);
                            //监控进程
                            RunInfo.Visibility = Visibility.Visible;
                            Thread thread = new Thread(Seeing);
                            thread.Start();
                            isOpen.Text = "正在启动线程维护";
                            ProgressBarHelper.SetAnimateTo(openPer, 99);
                        }
                        catch
                        {
                            ProgressBarHelper.SetAnimateTo(openPer, 0);
                            isOpen.Text = "未启用";
                        }
                    }
                    break;
            }
        }

        private void Seeing()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                NamePorc.Text = "加载中";
                IDPorc.Text = "加载中";
                UsePorc.Text = "加载中";
            });
            Process p = SSUserClass.Proc.GetProc("edge");
            PerformanceCounter pf1 = new PerformanceCounter("Process", "Working Set - Private", p.ProcessName);
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                isRun = true;
                ButtonHelper.SetIcon(Run, "");
                IconHelper.SetFontSize(Run, 15);
                NamePorc.Text = p.ProcessName;
                IDPorc.Text = p.Id + "/" + p.SessionId;
                UsePorc.Text = pf1.NextValue() / 1024 + "KB";
                ProgressBarHelper.SetAnimateTo(openPer, 100);
                isOpen.Text = "启动完成";
            });
            while (SSUserClass.Proc.HasProc("edge"))
            {
                ////刷新进程信息
                //p = SSUserClass.Proc.GetProc("edge");
                //pf1 = new PerformanceCounter("Process", "Working Set - Private", p.ProcessName);
                //this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                //{
                //    NamePorc.Text = p.ProcessName;
                //    IDPorc.Text = p.Id + "/" + p.SessionId;
                //    UsePorc.Text = pf1.NextValue() / 1024 + "KB";
                //    ProgressBarHelper.SetAnimateTo(openPer, 100);
                //});
                Thread.Sleep(10);
            }
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                RunInfo.Visibility = Visibility.Collapsed;
                ProgressBarHelper.SetAnimateTo(openPer, 0);
                isOpen.Text = "未启用";
                ButtonHelper.SetIcon(Run, "");
                IconHelper.SetFontSize(Run, 25);
            });
            isRun = false;
        }

        private void NO_Click(object sender, RoutedEventArgs e)
        {
            panErrorMsg.Visibility = Visibility.Collapsed;
            switch (errMode)
            {
                case 2:
                    {
                        //检测文件完整性
                        if (Directory.Exists("System"))
                        {
                            if (!File.Exists("System\\edge.exe"))
                            {
                                errMode = 3;
                                panErrorMsg.Visibility = Visibility.Visible;
                                errorTitle.Text = "缺少组件";
                                errorSays.Text = "未找到运行核心edge.exe，请确认下载的程序包完整性……";
                            }
                        }
                        else
                        {
                            errMode = 3;
                            panErrorMsg.Visibility = Visibility.Visible;
                            errorTitle.Text = "缺少组件";
                            errorSays.Text = "未找到运行核心文件夹，请确认下载的程序包完整性……";
                        }
                    }
                    break;
                case 3:
                    {
                        Application.Current.Shutdown();
                    }
                    break;
                case 5:
                    {
                        ProgressBarHelper.SetAnimateTo(openPer, 0);
                        isOpen.Text = "未启用";
                    }
                    break;
            }
        }

        private void IsDHCP_Checked(object sender, RoutedEventArgs e)
        {
            if(isDHCP.IsChecked == true)
            {
                TextBoxHelper.SetWatermark(IP, "DHCP服务器IP地址");
            }
            else
            {
                TextBoxHelper.SetWatermark(IP, "本地IP地址");
            }
        }

        private void ConfigList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool get = false;
            ListViewItem item = (ListViewItem)ConfigList.SelectedItem;
            if(item.Name == "New")
            {
                Name.Text = "";
                IP.Text = "";
                IPAdd.Text = "";
                isDHCP.IsChecked = false;
                SeverIP.Text = "";
                SeverPost.Text = "";
                TeamName.Text = "";
                Password.Text = "";
                return;
            }
            SaveLoader.N2NConfig config = new SaveLoader.N2NConfig();
            for (int i = 0; i<= configs.Count - 1; i++)
            {
                if (("UUID" + configs[i].UUID) == item.Name)
                {
                    config = configs[i];
                    get = true;
                    break;
                }
            }
            if(get)
            {
                Name.Text = config.Name;
                IP.Text = config.iP;
                IPAdd.Text = config.iPAdditional;
                isDHCP.IsChecked = config.isDHCP;
                SeverIP.Text = config.severIP;
                SeverPost.Text = config.severPost.ToString();
                TeamName.Text = config.teamName;
                Password.Text = config.teamPassword;
            }
            else
            {
                errMode = 3;
                panErrorMsg.Visibility = Visibility.Visible;
                errorTitle.Text = "检索配置错误";
                errorSays.Text = "未知的UUID……";
            }
        }

        private void N2N_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/ntop/n2n");
        }

        private void SS_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://ssteamcommunity.wordpress.com/");
        }
    }
}
