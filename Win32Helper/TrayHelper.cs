using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
namespace Win32Helper
{
    /// <summary>
    /// 可以考虑单例模式，一个程序只能开一个托盘
    /// </summary>
    public class TrayHelper
    {
        static TrayHelper()
        {
            _NotifyIcon.Icon = Win32Helper.ResourceIcon.Select;
            _NotifyIcon.Visible = false;
            //_NotifyIcon.Text = "tray";

            ContextMenu menu = new ContextMenu();
            MenuItem show = new MenuItem();
            show.Text = "显示窗体";
            show.Index = 0;
            show.Click += new EventHandler(MenuItemShow_Click);
            menu.MenuItems.Add(show);

            MenuItem hide = new MenuItem();
            hide.Text = "隐藏窗体";
            hide.Index = 1;
            hide.Click +=new EventHandler(MenuItemHide_Click);
            menu.MenuItems.Add(hide);

            MenuItem cancle = new MenuItem();
            cancle.Text = "退出程序";
            cancle.Index = 2;
            cancle.Click += new EventHandler(MenuItemCancle_Click);
            menu.MenuItems.Add(cancle);

            _NotifyIcon.ContextMenu = menu;
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
        }
        public static void BindHandle(IntPtr handle)
        {
            windowHandle = handle;
        }
        public static void BindHandle(string title)
        {
            windowHandle = FindWindow(null, title);
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
    }
}
