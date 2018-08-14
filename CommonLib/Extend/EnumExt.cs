using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Extend
{
    public static class EnumExt
    {
        public static string GetResourceString(this Enum enumValue)
        {
            Type enumType = enumValue.GetType();
            string name = string.Format("{0}.{1}", enumType.FullName, Enum.GetName(enumType, enumValue));
            name = name.Replace('.', '_').Replace('+', '_');

            try
            {
                string value = string.Empty;//Resources.ResourceManager.GetString(name);
                return value;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(String.Format("缺少对应资源项目 {0}", name));
            }
            return String.Empty;
        }
    }
}
