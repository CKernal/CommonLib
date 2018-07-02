using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CKernal.CommonLib.CSV;
using CKernal.CommonLib.Demo.CSV;
using CKernal.CommonLib.Demo.Setting;
using CKernal.CommonLib.Demo.Network;
using System.Threading;

namespace CKernal.CommonLib.Demo
{
    public partial class FormMain : Form
    {

        public FormMain()
        {
            InitializeComponent();
        }

        private void btn_Test_Click(object sender, EventArgs e)
        {
            //CsvHelperDemo.CsvHelperReadWriteTest();
            //CustomSettingDemo.Save();
            //CustomSettingDemo.Load();
            IpHelperDemo.Test();

        }


    }
}
