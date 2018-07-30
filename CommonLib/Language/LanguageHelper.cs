using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

namespace CommonLib.Language
{
    public class LanguageHelper
    {
        private static ConcurrentDictionary<Control, ResourceManager> m_controlResourceDictionary = new ConcurrentDictionary<Control, ResourceManager>();

        public static string GetControlResourcesString(Control control, string key, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            if (!m_controlResourceDictionary.ContainsKey(control))
            {
                ResourceManager resource = new ResourceManager(control.GetType());
                m_controlResourceDictionary.AddOrUpdate(control, resource, (oldkey, oldvalue) => resource);
            }

            if (culture == null)
            {
                return m_controlResourceDictionary[control].GetString(key);
            }
            return m_controlResourceDictionary[control].GetString(key, culture);
        }
    }
}
