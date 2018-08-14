using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLib.CSV;
using CommonLib.Demo.CSV;
using CommonLib.Demo.Setting;
using CommonLib.Demo.Network;
using System.Threading;

namespace CommonLib.Demo
{
    public partial class FormMain : Form
    {

        public FormMain()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.DoubleBuffer
                | ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                , true);
            this.UpdateStyles();

            TrayDetailBase tray = new TrayDetail();
            tray.Dock = DockStyle.Fill;
            tabPage1.Controls.Add(tray);


            //flowLayoutPanel1.VerticalScroll.LargeChange = 1;
            //flowLayoutPanel1.VerticalScroll.SmallChange = 1;
        }

        private void btn_Test_Click(object sender, EventArgs e)
        {
            string xxxx = Language.Language.GetControlResourcesString(this, "TestA");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            xxxx = Language.Language.GetControlResourcesString(this, "TestA");


            ComponentResourceManager resources = new ComponentResourceManager(typeof(FormMain));
            System.Reflection.FieldInfo[] fieldInfo = this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            for (int i = 0; i < fieldInfo.Length; i++)
            {
                System.Diagnostics.Trace.WriteLine(fieldInfo[i].Name);

            }

            //Type type = this.btn_Test.GetType();
            //string name = type.Name;
            //resources.ApplyResources(this.btn_Test, name);

            //foreach (Control ctl in this.Controls)
            //{
            //    resources.ApplyResources(ctl, ctl.Name);
            //}
            //resources.ApplyResources(btn_Test, "btn_Test");
            //CsvHelperDemo.CsvHelperReadWriteTest();
            //CustomSettingDemo.Save();
            //CustomSettingDemo.Load();
            //IpHelperDemo.Test();
        }


        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;
        //        return cp;
        //    }
        //}

        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == 0x0014) // 禁掉清除背景消息
        //        return;
        //    base.WndProc(ref m);
        //}


    }
}
