using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Extensions
{
    public static class EnumExtensions
    {
        public static List<KeyValuePair<int, string>> GetEnumByDescriptions(Type enumType)
        {
            var list = new List<KeyValuePair<int, string>>();
            foreach (Enum value in Enum.GetValues(enumType))
            {
                var description = value.ToString();
                var fieldInfo = value.GetType().GetField(description);
                var attribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();

                if (attribute != null)
                {
                    description = (attribute as DescriptionAttribute)?.Description;
                }
                list.Add(new KeyValuePair<int, string>(Convert.ToInt32(value), description));
            }
            return list;
        }

        public static List<KeyValuePair<int, string>> GetEnumByValue<T>()
        {
            var s = Enum.GetValues(typeof(T)).Cast<byte>();
            var dictionary = s.ToDictionary(t => (int)t, t => Enum.ToObject(typeof(T), t).ToString());
            return dictionary.ToList();
        }

        public static List<KeyValuePair<int, string>> GetEnumDetails(Type enumType)
        {
            var list = new List<KeyValuePair<int, string>>();
            foreach (Enum value in Enum.GetValues(enumType))
            {
                list.Add(new KeyValuePair<int, string>(Convert.ToInt32(value), Enum.GetName(enumType, value)));
            }
            return list;
        }

        public static List<KeyValuePair<string, string>> GetEnumDetailsWithValueAndDescription(Type enumType)
        {
            var list = new List<KeyValuePair<string, string>>();
            foreach (Enum value in Enum.GetValues(enumType))
            {
                var description = value.ToString();
                var fieldInfo = value.GetType().GetField(description);
                var attribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();

                if (attribute != null)
                {
                    description = (attribute as DescriptionAttribute)?.Description;
                }
                list.Add(new KeyValuePair<string, string>(Convert.ToString(value).ToUpper(), description));
            }
            return list;
        }

        public static string ToEnumDescription(this Enum value)
        {
            if (value == null)
                return string.Empty;

            var fi = value.GetType()?.GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fi?.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;

            return string.Empty;
        }

        public static string GetEnumByKey(Type enumType, byte key)
        {
            return GetEnumByDescriptions(enumType).Exists(keyValue => keyValue.Key.Equals(key)) ?
                GetEnumByDescriptions(enumType).Find(keyValue => keyValue.Key.Equals(key)).Value :
                key.ToString();
        }
    }
}
