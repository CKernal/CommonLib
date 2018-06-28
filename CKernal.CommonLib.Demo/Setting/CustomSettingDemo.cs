using CKernal.CommonLib.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CKernal.CommonLib.Demo.Setting
{
    public class CustomSettingDemo
    {
        public static void Save()
        {
            var setting = new CustomSetting();
            setting.IsOk = 1111;

            setting.Serialize();
        }

        public static void Load()
        {
            var setting = CustomSettingPersistence.Deserialize<CustomSetting>();

            MessageBox.Show(setting.IsOk.ToString());
        }

    }
}
