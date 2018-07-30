using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLib.Demo
{
    public partial class TrayDetail : TrayDetailBase
    {
        public TrayDetail()
        {
            InitializeComponent();

            for (int i = 0; i < 96; i++)
            {
                ChannelUI channelUI = new ChannelUI(i);
                if ((i + 1) % 8 == 0)
                {
                    flowLayoutPanel1.SetFlowBreak(channelUI, true);
                }
                flowLayoutPanel1.Controls.Add(channelUI);
            }
        }
    }
}
