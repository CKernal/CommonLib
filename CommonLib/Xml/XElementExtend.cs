using System;
using System.Xml.Linq;

namespace CommonLib.Xml
{
    public static class XElementExtend
    {
        public static XElement AddElement(this XElement parent, string name, string value)
        {
            XElement element = new XElement(name);
            if (String.IsNullOrEmpty(value) == false)
            {
                element.Value = value;
            }
            parent.Add(element);
            return element;
        }

        public static string GetElementValue(this XElement element, string name)
        {
            XElement child = element.Element(name);
            return child.Value;
        }

        public static T GetElementValue<T>(this XElement element, string name, T defaultValue) where T : IConvertible
        {
            XElement child = element.Element(name);
            if (child == null)
            {
                return defaultValue;
            }
            else
            {
                if (String.IsNullOrEmpty(child.Value))
                {
                    return defaultValue;
                }
                else
                {
                    try
                    {
                        if (typeof(T) == typeof(double))
                        {
                            double value;
                            if (double.TryParse(child.Value, out value))
                            {
                                return (T)(object)value;
                            }
                            else
                            {
                                return defaultValue;
                            }
                        }
                        else
                        {
                            return (T)Convert.ChangeType(child.Value, typeof(T));
                        }
                    }
                    catch
                    {
                        return defaultValue;
                    }
                }
            }
        }

        public static T GetAttributeValue<T>(this XElement element, string name, T defaultValue) where T : IConvertible
        {
            XAttribute attribute = element.Attribute(name);
            if (attribute == null)
            {
                return defaultValue;
            }
            else
            {
                if (String.IsNullOrEmpty(attribute.Value))
                {
                    return defaultValue;
                }
                else
                {
                    try
                    {
                        return (T)Convert.ChangeType(attribute.Value, typeof(T));
                    }
                    catch
                    {
                        return defaultValue;
                    }
                }
            }
        }

        public static T GetElementAttributeValue<T>(this XElement element, string elementName,
            string attributeName, T defaultValue) where T : IConvertible
        {
            XElement child = element.Element(elementName);
            if (child == null)
            {
                return defaultValue;
            }
            else
            {
                XAttribute attribute = child.Attribute(attributeName);
                if (attribute == null)
                {
                    return defaultValue;
                }
                else
                {
                    if (String.IsNullOrEmpty(attribute.Value))
                    {
                        return defaultValue;
                    }
                    else
                    {
                        try
                        {
                            return (T)Convert.ChangeType(attribute.Value, typeof(T));
                        }
                        catch
                        {
                            return defaultValue;
                        }
                    }
                }
            }
        }
    }
}
