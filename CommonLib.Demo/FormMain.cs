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

            for (int i = 0; i < 96; i++)
            {
                ChannelUI channelUI = new ChannelUI(i);
                if ((i + 1) % 8 == 0)
                {
                    flowLayoutPanel1.SetFlowBreak(channelUI, true);
                }
                flowLayoutPanel1.Controls.Add(channelUI);

            }
            //flowLayoutPanel1.VerticalScroll.LargeChange = 1;
            //flowLayoutPanel1.VerticalScroll.SmallChange = 1;
        }

        private void btn_Test_Click(object sender, EventArgs e)
        {
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
