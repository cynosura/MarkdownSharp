using System;
using System.Collections;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownSharp
{
	public class Markdown
	{
		private string InputText;
		private Hashtable LinkReferences;
		public Markdown(string input)
		{
			InputText = input;
		}

		public string Parse()
		{
			string OutputString = InputText;
			LinkReferences = new Hashtable();
			GenLinkReferenceTable();
			OutputString = ParseLinks(OutputString);
			OutputString = ParseEmphasis(OutputString);
			OutputString = ParseInlineCode(OutputString);

			return OutputString;
		}

		private void GenLinkReferenceTable()
		{
			Regex rx = new Regex(@"\[([a-zA-Z0-9_-]+)\]: (\S+)\s*([""|'].*?[""|'])?");
			MatchCollection matches = rx.Matches(InputText);
			foreach (Match match in matches)
			{
				GroupCollection groups = match.Groups;
				string title = null;
				try
				{
					title = groups[3].Value;
				}
				catch (Exception)
				{
					// keep title null
				}
				LinkReferences.Add(groups[1].Value, new LinkReference(groups[2].Value, title));
			}
		}

		private string ParseLinks(string input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] == '[' && input[i - 1] != '\\')
				{
					int start = i;
					string linktext = ReadToCharacter(']', i + 1, input);
					i += linktext.Length + 2;
					if (input[i] == '(')
					{
						string parencontents = ReadToCharacter(')', i + 1, input);
						string url;
						string title;
						if (parencontents.IndexOf(' ') > -1)
						{
							string[] spl = parencontents.Split(new char[] { ' ' }, 2);
							url = spl[0];
							title = spl[1];
						}
						else
						{
							url = parencontents;
							title = null;
						}
						int len = linktext.Length + 2 + parencontents.Length + 3;
						// to be replaced
						string tbr = input.Substring(start - 1, len);
						// to replace with
						string trw = "<a href=\"" + url + "\"";
						if (title != null)
						{
							trw += " title=" + title;
						}
						trw += ">" + linktext + "</a>";
						input = input.Replace(tbr, trw);
						i += len;
					}
					else if (input[i] == '[' || (input[i] == ' ' && input[i + 1] == '['))
					{
						bool hasspace = false;
						int refidstart = i + 1;
						if (input[i] == ' ')
						{
							hasspace = true;
							refidstart += 1;
						}
						string refid = ReadToCharacter(']', refidstart, input);
						LinkReference link = (LinkReference)LinkReferences[refid];
						int len = linktext.Length + 2 + refid.Length + 2;
						if (hasspace)
						{
							len += 1;
						}
						len += 1;
						// to be replaced
						string tbr = input.Substring(start - 1, len);
						// to replace with
						string trw = "<a href=\"" + link.Url + "\"";
						if (link.Title != null)
						{
							trw += " title=" + link.Title;
						}
						trw += ">" + linktext + "</a>";
						input = input.Replace(tbr, trw);
						i += len;
					}
					else if (input[i] == ':')
					{
						string str = ReadToEndOfLine(start, input);
						input = input.Replace(str, "");
					}
				}
			}
			return input;
		}

		private string ParseEmphasis(string input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] == '*' && input[i - 1] != '\\' && !(input[i-1] == ' ' && input[i+1] == ' '))
				{
					if (input[i + 1] == '*')
					{
						string str = ReadToCharacter('*', i + 2, input);
						while (input[i + str.Length + 2] != '*')
						{
							str += ReadToCharacter('*', i + str.Length + 3, input);
						}
						input = input.Replace("**" + str + "**", "<strong>" + str + "</strong>");
						i += str.Length + 17;
					}
					else
					{
						string str = ReadToCharacter('*', i + 1, input);
						input = input.Replace("*" + str + "*", "<em>" + str + "</em>");
						i += str.Length + 9;
					}
				}
				else if (input[i] == '_' && input[i - 1] != '\\' && !(input[i - 1] == ' ' && input[i + 1] == ' '))
				{
					if (input[i + 1] == '_')
					{
						string str = ReadToCharacter('_', i + 2, input);
						while (input[i + str.Length + 3] != '_')
						{
							str += ReadToCharacter('_', i + str.Length + 3, input);
						}
						input = input.Replace("__" + str + "__", "<strong>" + str + "</strong>");
						i += str.Length + 17;
					}
					else
					{
						string str = ReadToCharacter('_', i + 1, input);
						input = input.Replace("_" + str + "_", "<em>" + str + "</em>");
						i += str.Length + 9;
					}
				}
			}
			return input;
		}

		private string ParseInlineCode(string input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] == '`' && input[i - 1] != '\\')
				{
					if (input[i + 1] == '`')
					{
						string str = ReadToCharacter('`', i + 2, input);
						while (input[i + str.Length + 3] != '`')
						{
							str += "`" + ReadToCharacter('`', i + str.Length + 3, input);
						}
						string tbr = "``" + str + "``";
						str = str.Replace("&", "&amp;");
						str = str.Replace("<", "&lt;");
						str = str.Replace(">", "&gt;");
						input = input.Replace(tbr, "<code>" + str + "</code>");
						i += str.Length + 13;
					}
					else
					{
						string str = ReadToCharacter('`', i + 1, input);
						input = input.Replace("`" + str + "`", "<code>" + str + "</code>");
						i += str.Length + 13;
					}
				}
			}
			return input;
		}

		private string ReadToEndOfLine(int startindex, string input)
		{
			StringBuilder sb = new StringBuilder();
			int i = startindex;
			try
			{
				while (input[i] != '\r' && input[i] != '\n')
				{
					sb.Append(input[i]);
					i++;
				}
			}
			catch (Exception)
			{
				// return
			}
			return sb.ToString();
		}

		private string ReadToCharacter(char c, int startindex, string input)
		{
			StringBuilder sb = new StringBuilder();
			int i = startindex;
			try
			{
				while (input[i] != c)
				{
					sb.Append(input[i]);
					i++;
				}
			}
			catch (Exception)
			{
				// return
			}
			return sb.ToString();
		}
	}

	public class LinkReference
	{
		public string Url;
		public string Title;

		public LinkReference(string url, string title)
		{
			Url = url;
			Title = title;
		}
	}
}
