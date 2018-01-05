using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32Helper;
namespace FHWinConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "TestConsoleLikeWin32";
            TrayHelper.BindHandle(Console.Title);
            TrayHelper.DisableCloseButton();
            TrayHelper.SetNotifyIconText(Console.Title);
            TrayHelper.ShowNotifyIcon(3000, Console.Title, "我是托盘图标，用右键点击我试试，还可以双击看看。");
            TrayHelper.ShowWindow(0);
            Console.WriteLine("test");
            while (true)
            {
                Application.DoEvents();//很重要，没有这个的话可能隐藏的图标点击没有效果
            }
        }

    }
}
