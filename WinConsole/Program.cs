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
            FormHelper.BindHandle(Console.Title);
            FormHelper.DisableCloseButton();
            FormHelper.ShowNotifyIcon();
            FormHelper.ShowWindow(0);
            Console.WriteLine("test");
            while (true)
            {
                Application.DoEvents();//很重要，没有这个的话可能隐藏的图标点击没有效果
            }
        }

    }
}
