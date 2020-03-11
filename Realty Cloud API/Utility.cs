using System;
using System.Collections.Generic;
using System.Globalization;

namespace RealtyCloudAPI
{
	internal static class Utility
	{
		public delegate bool TryObjectBuild<T>(IDictionary<string, object> data, out T value);

		public static bool TryGetString(this IDictionary<string, object> data, string key, out string value)
		{
			object obj;
			if(data.TryGetValue(key, out obj) && obj is string)
			{
				value = (string)obj;
				return true;
			}

			value = string.Empty;
			return false;
		}

		public static bool TryGetDecimal(this IDictionary<string, object> data, string key, out decimal value)
		{
			string str;
			if(data.TryGetString(key, out str))
			{
				str = str.Replace(',', '.');

				return decimal.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
			}

			value = decimal.Zero;
			return false;
		}

		public static bool TryGetInt(this IDictionary<string, object> data, string key, out int value)
		{
			string str;
			if(data.TryGetString(key, out str))
			{
				return int.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
			}

			value = 0;
			return false;
		}

		public static bool TryGetBool(this IDictionary<string, object> data, string key, out bool value)
		{
			string str;
			if(data.TryGetString(key, out str))
			{
				return bool.TryParse(str, out value);
			}

			value = false;
			return false;
		}

		public static bool TryGetDateTime(this IDictionary<string, object> data, string key, out DateTime value)
		{
			string str;
			if(data.TryGetString(key, out str))
			{
				return DateTime.TryParse(str, out value);
			}

			value = DateTime.MinValue;
			return false;
		}

		public static bool TryGetObj<T>(this IDictionary<string, object> data, string key, TryObjectBuild<T> builder, out T value)
		{
			object obj;
			if(data.TryGetValue(key, out obj) && obj is IDictionary<string, object>)
			{
				return builder.Invoke(obj as IDictionary<string, object>, out value);
			}

			value = default(T);
			return false;
		}

		public static bool TryGetObjArray<T>(this IDictionary<string, object> data, string key, TryObjectBuild<T> builder, out T[] value)
		{
			object obj;
			if(data.TryGetValue(key, out obj) && obj is IList<object>)
			{
				var list = (IList<object>)obj;
				value = new T[list.Count];

				for(int i = 0; i < list.Count; i++)
				{
					if(!(list[i] is IDictionary<string, object> && builder.Invoke(list[i] as IDictionary<string, object>, out value[i])))
					{
						return false;
					}
				}

				return true;
			}

			value = null;
			return false;
		}
	}
}