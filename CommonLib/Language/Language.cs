using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

namespace CommonLib.Language
{
    public enum LanguageType
    {
        Chinese = 0,
        English = 1
    }
    public class Language
    {
        private Action<LanguageType> m_languageChangedEventHandler;
        public event Action<LanguageType> OnLanguageChanged
        {
            add { m_languageChangedEventHandler += value; }
            remove { m_languageChangedEventHandler -= value; }
        }

        private ConcurrentDictionary<Control, ResourceManager> m_controlResourceDictionary = new ConcurrentDictionary<Control, ResourceManager>();

        public string GetResourcesString(Control control, string key, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            if (!m_controlResourceDictionary.ContainsKey(control))
            {
                ResourceManager resource = new ResourceManager(control.GetType());
                m_controlResourceDictionary.AddOrUpdate(control, resource, (oldkey, oldvalue) => resource);
            }

            if (culture != null)
            {
                return m_controlResourceDictionary[control].GetString(key);
            }
            return m_controlResourceDictionary[control].GetString(key, culture);
        }

        public string GetResourcesString(string key, CultureInfo culture = null)
        {
            return string.Empty;
            //if (culture != null)
            //{
            //    return Properties.Resources.ResourceManager.GetString(key, culture);
            //}
            //return Properties.Resources.ResourceManager.GetString(key);
        }

        public string GetCurrentLanguge()
        {
            System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture;
            return cultureInfo.Name;
        }

        public string TransformText(string sourceText)
        {
            string transText = string.Empty;
            string resourceText = string.Empty;

            if (sourceText.Contains("|"))
            {
                string[] detail = sourceText.Split('|');
                resourceText = GetResourcesString(detail[0]);

                if (detail[1].Contains(","))
                {
                    string[] param = sourceText.Split(',');

                    if (param.Length == 2)
                    {
                        transText = string.Format(resourceText, param[0], param[1]);
                    }
                    if (param.Length == 3)
                    {
                        transText = string.Format(resourceText, param[0], param[1], param[2]);
                    }
                }
                else
                {
                    transText = string.Format(resourceText, detail[1]);
                }
            }
            else
            {
                transText = GetResourcesString(sourceText);
            }
            return transText;
        }

        public void SetUserControlLanguage(UserControl uesrControl)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(uesrControl.GetType());
            SetControlLanguage(uesrControl, resources);
        }

        public void SetControlLanguage(Control control, System.ComponentModel.ComponentResourceManager resources)
        {
            foreach (Control ctl in control.Controls)
            {
                if (ctl is UserControl)
                {
                    SetUserControlLanguage((UserControl)ctl);
                }
                else
                {
                    ApplyControlResources(ctl, resources);

                    if (ctl.Controls != null
                        && ctl.Controls.Count > 0)
                    {
                        SetControlLanguage(ctl, resources);
                    }
                }
            }
        }

        public void ApplyControlResources(Control control, System.ComponentModel.ComponentResourceManager resources)
        {
            //System.Diagnostics.Trace.WriteLine(string.Format("{0}, {1}", resources.BaseName, control.Name));

            resources.ApplyResources(control, control.Name);

            if (control is DataGridView)
            {
                DataGridView dgv = (DataGridView)control;
                foreach (DataGridViewColumn item in dgv.Columns)
                {
                    resources.ApplyResources(item, item.Name);

                    //if (item is DataGridViewComboBoxColumn)
                    //{
                    //}
                }
            }

            if (control is ListView)
            {
                ListView listView = (ListView)control;
                foreach (ColumnHeader item in listView.Columns)
                {
                    resources.ApplyResources(item, item.Name);
                }
            }

            //if (control is DevExpress.XtraGrid.GridControl)
            //{
            //    DevExpress.XtraGrid.GridControl gridControl = (DevExpress.XtraGrid.GridControl)control;
            //    DevExpress.XtraGrid.Views.Grid.GridView gridView = (DevExpress.XtraGrid.Views.Grid.GridView)gridControl.MainView;

            //    foreach (DevExpress.XtraGrid.Columns.GridColumn item in gridView.Columns)
            //    {
            //        resources.ApplyResources(item, item.Name);
            //    }
            //}

            if (control is ComboBox)
            {
                ComboBox comboBox = (ComboBox)control;
                string[] comboBoxItems = new string[comboBox.Items.Count];
                for (int i = 0; i < comboBoxItems.Length; i++)
                {
                    comboBoxItems[i] = comboBox.Items[i].ToString();
                }
                string name = string.Format("{0}.Items", comboBox.Name);

                int itemsCount = comboBox.Items.Count;
                int selectIndex = comboBox.SelectedIndex;
                comboBox.Items.Clear();
                for (int i = 0; i < itemsCount; i++)
                {
                    if (i > 0)
                    {
                        name = string.Format("{0}.Items{1}", comboBox.Name, i);
                    }
                    string itemName = resources.GetString(name);
                    if (!string.IsNullOrEmpty(itemName))
                    {
                        comboBox.Items.Add(itemName);
                    }
                    else
                    {
                        if (i < comboBoxItems.Length)
                        {
                            comboBox.Items.Add(comboBoxItems[i]);
                        }
                    }
                }

                comboBox.SelectedIndex = selectIndex;
            }

            //if (control is Steema.TeeChart.TChart)
            //{
            //    Steema.TeeChart.TChart tChart = (Steema.TeeChart.TChart)control;
            //    foreach (Steema.TeeChart.Styles.Series item in tChart.Series)
            //    {
            //        resources.ApplyResources(item, item.ToString());
            //    }
            //}
        }

        public void SetCurrentThreadLanguage(int languageIndex)
        {
            LanguageType languageType;
            if (Enum.TryParse(languageIndex.ToString(), out languageType))
            {
                SetCurrentThreadLanguage(languageType);
            }
        }

        public void SetCurrentThreadLanguage(LanguageType languageType)
        {
            switch (languageType)
            {
                case LanguageType.Chinese:
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh");
                    break;
                case LanguageType.English:
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
                    break;
                default:
                    break;
            }

            if (m_languageChangedEventHandler != null)
            {
                m_languageChangedEventHandler.Invoke(languageType);
            }
        }


        public void CreateResourceString(Enum enumValue, LanguageType languageType)
        {
            Type enumType = enumValue.GetType();
            string enumFullName = enumType.FullName;
            enumFullName = enumFullName.Replace('.', '_').Replace('+', '_');

            string[] names = Enum.GetNames(enumType);

            foreach (var item in names)
            {
                object obj = Enum.Parse(enumType, item);

                System.Diagnostics.Trace.WriteLine(string.Format("  <data name=\"{0}_{1}\" xml:space=\"preserve\">", enumFullName, item));
                switch (languageType)
                {
                    case LanguageType.Chinese:
                        System.Diagnostics.Trace.WriteLine(string.Format("    <value>{0}</value>", obj.ToString()));
                        break;
                    case LanguageType.English:
                        System.Diagnostics.Trace.WriteLine(string.Format("    <value>{0}</value>", Enum.GetName(enumType, obj)));
                        break;
                    default:
                        break;
                }

                System.Diagnostics.Trace.WriteLine("  </data>");
            }
        }

        private static Language m_instance;
        public static Language Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new Language();
                }
                return m_instance;
            }
        }
    }
}
