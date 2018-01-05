using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32Helper;
namespace FHWindowsForms
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            TrayHelper.BindHandle(this.Handle);
            TrayHelper.ShowNotifyIcon(3000, "form测试", "我是托盘图标，用右键点击我试试，还可以双击看看。");
        }

        ~Main()
        {
            Console.WriteLine("程序成功退出");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            TrayHelper.ShowWindow(0);
        }
    }
}
