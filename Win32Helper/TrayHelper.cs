using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
namespace Win32Helper
{
    /// <summary>
    /// 托盘助手
    /// 可以考虑单例模式，一个程序只能开一个托盘
    /// </summary>
    public class TrayHelper
    {
        private static Mutex mutex=null;
        static TrayHelper()
        {
            _NotifyIcon.Icon = Win32Helper.ResourceIcon.Select;
            _NotifyIcon.Visible = false;
            //_NotifyIcon.Text = "tray";

            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem show = new ToolStripMenuItem();
            show.Text = "显示窗体";
            show.Image = ResourceIcon.show;
            //show.Index = 0;
            show.Click += new EventHandler(MenuItemShow_Click);
            menu.Items.Add(show);

            ToolStripMenuItem hide = new ToolStripMenuItem();
            hide.Text = "隐藏窗体";
            //hide.Index = 1;
            hide.Image = ResourceIcon.hide;

            hide.Click +=new EventHandler(MenuItemHide_Click);
            menu.Items.Add(hide);

            ToolStripMenuItem cancle = new ToolStripMenuItem();
            cancle.Text = "退出程序";
            cancle.Image = ResourceIcon.exit;

            //cancle.Index = 2;
            cancle.Click += new EventHandler(MenuItemCancle_Click);
            menu.Items.Add(cancle);

            _NotifyIcon.ContextMenuStrip = menu;
            _NotifyIcon.MouseDoubleClick += new MouseEventHandler(_NotifyIcon_MouseDoubleClick);

        }

        public static IntPtr windowHandle = IntPtr.Zero;
        static NotifyIcon _NotifyIcon = new NotifyIcon();

        #region even
        static void MenuItemShow_Click(object sender,EventArgs e)
        {
            ShowWindow(1);
        }

        static void MenuItemHide_Click(object sender, EventArgs e)
        {
            ShowWindow(0);
        }
        static void MenuItemCancle_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("真的要关闭系统吗？", "提示", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Information);
            if (dr ==DialogResult.Yes)
            {
                Application.Exit();
                Environment.Exit(0);
            }
        }

        static void _NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowWindow(1);
        }

        #endregion

        #region win32
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);
        /// <summary>
        /// 隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "ShowWindow")]
        static extern bool ShowWindow(IntPtr hWnd, int type);
        #endregion

        #region 关闭控制台 快速编辑模式、插入模式
        const int STD_INPUT_HANDLE = -10;
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        const uint ENABLE_INSERT_MODE = 0x0020;
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int hConsoleHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);

        public static void DisbleQuickEditMode()
        {
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            uint mode;
            GetConsoleMode(hStdin, out mode);
            mode &= ~ENABLE_QUICK_EDIT_MODE;//移除快速编辑模式
            mode &= ~ENABLE_INSERT_MODE;      //移除插入模式
            SetConsoleMode(hStdin, mode);
        }
        #endregion

        public static void DisableCloseButton()
        {
            //线程睡眠，确保closebtn中能够正常FindWindow，否则有时会Find失败。。
            Thread.Sleep(100);
            //IntPtr windowHandle = FindWindow(null, title);
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }
        public static bool IsExistsConsole(string title)
        {
            IntPtr windowHandle = FindWindow(null, title);
            if (windowHandle.Equals(IntPtr.Zero)) return false;

            return true;
        }
        public static void ShowNotifyIcon(int timeout,string title,string body)
        {
            _NotifyIcon.Visible = true;
            _NotifyIcon.ShowBalloonTip(timeout, title, body, ToolTipIcon.None);
        }
        public static void HideNotifyIcon()
        {
            _NotifyIcon.Visible = false;
        }
        public static void ShowWindow(int type)
        {
            //IntPtr windowHandle = FindWindow(null, title);
            ShowWindow(windowHandle, type);
            Console.WriteLine(windowHandle);
        }
        public static void BindHandle(IntPtr handle)
        {
            windowHandle = handle;
        }
        public static void BindHandle(string title)
        {
            windowHandle = FindWindow(null, title);
            Console.WriteLine(windowHandle);
            Console.WriteLine(title);
        }
        public static void SetContextMenu(ContextMenu menu)
        {
            _NotifyIcon.ContextMenu = menu;

        }
        public static void SetNotifyIcon(Icon icon)
        {
            _NotifyIcon.Icon = icon;
        }
        public static void SetNotifyIconText(string text)
        {
            _NotifyIcon.Text = text;
        }
        public static bool IsSingleCase(Assembly assem)
        {
            string ptitle = assem.GetName().Name;
            foreach (Attribute attr in Attribute.GetCustomAttributes(assem))
            {
                if (attr.GetType() == typeof(AssemblyTitleAttribute))
                {
                    ptitle = ((AssemblyTitleAttribute)attr).Title;
                    break;
                }
            }
            bool canCreateNew = false;
            mutex = new Mutex(true, @"Global\"+ptitle, out canCreateNew);
            return canCreateNew;
        }
    }
}
