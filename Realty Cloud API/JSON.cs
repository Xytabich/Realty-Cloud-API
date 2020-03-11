using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RealtyCloudAPI
{
	public class JSON
	{
		public static bool TryParse(string data, out object value)
		{
			int index = 0;
			StringBuilder sb = new StringBuilder();
			return TryParseBlock(data.ToCharArray(), ref index, data.Length, sb, out value);
		}

		public static string ToString(object value)
		{
			StringBuilder sb = new StringBuilder();
			AppendValue(value, sb);
			return sb.ToString();
		}

		private static bool TryParseBlock(char[] data, ref int index, int len, StringBuilder sb, out object value)
		{
			sb.Clear();
			value = null;
			while(index < len)
			{
				if(data[index] == '{')
				{
					index++;
					return TryParseObject(data, ref index, len, sb, out value);
				}
				else if(data[index] == '[')
				{
					index++;
					return TryParseArray(data, ref index, len, sb, out value);
				}
				else if(data[index] == '"')
				{
					index++;
					string str;
					if(TryParseString(data, ref index, len, sb, out str))
					{
						value = str;
						return true;
					}
					else return false;
				}
				else if(data[index] == ',' || data[index] == ']' || data[index] == '}')
				{
					value = sb.ToString().TrimEnd();
					return true;
				}
				else if(!char.IsWhiteSpace(data[index]))
				{
					sb.Append(data[index]);
					index++;
				}
			}

			return false;
		}

		private static bool TryParseObject(char[] data, ref int index, int len, StringBuilder sb, out object value)
		{
			value = null;

			Dictionary<string, object> obj = new Dictionary<string, object>();
			string key = null;
			object val;
			while(index < len)
			{
				if(data[index] == '"')
				{
					index++;
					if(!TryParseString(data, ref index, len, sb, out key)) return false;
				}
				else if(data[index] == ':')
				{
					index++;
					if(string.IsNullOrEmpty(key)) return false;

					if(TryParseBlock(data, ref index, len, sb, out val))
					{
						obj[key] = val;
						key = null;
					}
					else return false;
				}
				else if(data[index] == '}')
				{
					index++;
					value = obj;
					return true;
				}
				else index++;
			}

			return false;
		}

		private static bool TryParseArray(char[] data, ref int index, int len, StringBuilder sb, out object value)
		{
			value = null;

			List<object> list = new List<object>();
			object val;
			while(index < len)
			{
				if(data[index] == ']')
				{
					index++;
					value = list;
					return true;
				}
				else if(data[index] == ',' || char.IsWhiteSpace(data[index]))
				{
					index++;
				}
				else
				{
					if(TryParseBlock(data, ref index, len, sb, out val))
					{
						list.Add(val);
					}
					else return false;
				}
			}

			return false;
		}

		private static bool TryParseString(char[] data, ref int index, int len, StringBuilder sb, out string value)
		{
			sb.Clear();
			bool isEscape = false;
			while(index < len)
			{
				if(isEscape)
				{
					sb.Append(data[index]);
					isEscape = false;
					index++;
				}
				else
				{
					if(data[index] == '"')
					{
						if(isEscape)
						{
							switch(data[index])
							{
								case '\\':
									sb.Append('\\');
									break;
								case '"':
									sb.Append('"');
									break;
								case 'n':
									sb.Append('\n');
									break;
								case 'r':
									sb.Append('\r');
									break;
								case 't':
									sb.Append('\t');
									break;
								case 'b':
									sb.Append('\b');
									break;
								case 'f':
									sb.Append('\f');
									break;
								default:
									sb.Append('\\');
									sb.Append(data[index]);
									break;
							}
							index++;
						}
						else
						{
							value = sb.ToString();
							sb.Clear();
							index++;
							return true;
						}
					}
					else if(data[index] == '\\')
					{
						isEscape = true;
						index++;
					}
					else
					{
						sb.Append(data[index]);
						index++;
					}
				}
			}
			value = null;
			return false;
		}

		private static void AppendValue(object value, StringBuilder sb)
		{
			if(value == null) sb.Append("null");
			else if(value is IList<object>)
			{
				var list = value as IList<object>;
				sb.Append('[');
				for(int i = 0; i < list.Count; i++)
				{
					if(i > 0) sb.Append(',');
					AppendValue(list[i], sb);
				}
				sb.Append(']');
			}
			else if(value is IDictionary<string, object>)
			{
				var list = new List<KeyValuePair<string, object>>(value as IDictionary<string, object>);
				sb.Append('{');
				for(int i = 0; i < list.Count; i++)
				{
					if(i > 0) sb.Append(',');
					sb.Append('"');
					Escape(list[i].Key, sb);
					sb.Append('"');
					sb.Append(':');
					AppendValue(list[i].Value, sb);
				}
				sb.Append('}');
			}
			else if(value is bool)
			{
				sb.Append((bool)value ? "true" : "false");
			}
			else if(value.GetType().IsPrimitive)
			{
				sb.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
			}
			else
			{
				sb.Append('"');
				if(value is string) Escape((string)value, sb);
				else Escape(Convert.ToString(value, CultureInfo.InvariantCulture), sb);
				sb.Append('"');
			}
		}

		private static void Escape(string text, StringBuilder sb)
		{
			foreach(char c in text)
			{
				switch(c)
				{
					case '\\':
						sb.Append("\\\\");
						break;
					case '\"':
						sb.Append("\\\"");
						break;
					case '\n':
						sb.Append("\\n");
						break;
					case '\r':
						sb.Append("\\r");
						break;
					case '\t':
						sb.Append("\\t");
						break;
					case '\b':
						sb.Append("\\b");
						break;
					case '\f':
						sb.Append("\\f");
						break;
					default:
						sb.Append(c);
						break;
				}
			}
		}
	}
}