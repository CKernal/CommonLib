using CommonLib.Xml;
using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace CommonLib.Setting
{
    public class CustomSettingPersistence
    {
        private static readonly string m_directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string m_directoryPath = Path.Combine(m_directory, "Settings");

        private void CreateDirectory()
        {
            if (!Directory.Exists(m_directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(m_directoryPath);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public bool Serialize()
        {
            CreateDirectory();

            try
            {
                XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"));

                Type type = this.GetType();
                string filePath = Path.Combine(m_directoryPath, type.Name);
                XElement root = new XElement(type.Name);
                doc.Add(root);

                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    object value = property.GetValue(this, null);
                    root.AddElement(property.Name, value.ToString());
                }

                doc.Save(filePath);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T Deserialize<T>() where T : CustomSettingPersistence, new()
        {
            Type type = typeof(T);
            string filePath = Path.Combine(m_directoryPath, type.Name);
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                XElement root = XElement.Load(filePath);
                if (root != null)
                {
                    T obj = (T)Activator.CreateInstance(type);
                    PropertyInfo[] properties = type.GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        string value = root.GetElementValue(property.Name);
                        Type propertyType = property.PropertyType;
                        object objValue = Convert.ChangeType(value, propertyType);
                        property.SetValue(obj, objValue, null);
                    }
                    return obj;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
