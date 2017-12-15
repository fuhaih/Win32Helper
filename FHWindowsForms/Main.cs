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
            FormHelper.BindHandle(this.Handle);
            FormHelper.ShowNotifyIcon();
        }

        ~Main()
        {
            Console.WriteLine("程序成功退出");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            FormHelper.ShowWindow(0);
        }
    }
}
