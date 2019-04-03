using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MFGLib
{
	class StringHelper
	{
		public static string ReplaceBlock(string content, string prefix, string surfix, string replaceWith)
		{
			if (string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(surfix))
			{
				return content;
			}

			int pos = content.IndexOf(prefix);
			if (pos == -1)
			{
				return content;
			}

			pos += prefix.Length;

			int pos2 = content.IndexOf(surfix, pos + 1);
			if (pos2 == -1)
			{
				return content;
			}

			string str = content.Substring(pos, pos2 - pos);
			return content.Replace(str, replaceWith);
		}
	}
}
