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
    public partial class ChannelUI : UserControl
    {
        private Rectangle m_rectangle;

        private int m_channel;

        public ChannelUI(int channel)
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.DoubleBuffer
                | ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                , true);
            this.UpdateStyles();

            m_channel = channel;
            m_rectangle = new Rectangle(0, 0, this.Width-1, this.Height-1);
        }

        private void ChannelUI_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawRectangle(Pens.Red, m_rectangle);

            g.DrawString(m_channel.ToString(), this.Font, Brushes.Blue, m_rectangle);
            g.DrawRectangle(Pens.Red, this.ClientRectangle);
            //g.FillRectangle(Brushes.LightYellow, m_rectangle);
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var parms = base.CreateParams;
        //        parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
        //        return parms;
        //    }
        //}
    }
}
