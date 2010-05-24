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

			return OutputString;
		}

		private void GenLinkReferenceTable()
		{
			Regex rx = new Regex(@"\[([a-zA-Z0-9_-]+)\]: (.*?) ("".*?"")?");
			MatchCollection matches = rx.Matches(InputText);
			foreach (Match match in matches)
			{
				GroupCollection groups = match.Groups;
				string title = null;
				if (groups.Count > 2) {
					title = groups[3].Value;
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
							string[] spl = parencontents.Split(new char[] { ' ' }, 1);
							url = spl[0];
							title = spl[1];
						}
						else
						{
							url = parencontents;
							title = null;
						}
						int len = linktext.Length + 2 + parencontents.Length + 2;
						// to be replaced
						string tbr = input.Substring(start, len);
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
							start++;
						}
						string refid = ReadToCharacter(']', refidstart, input);
						LinkReference link = (LinkReference)LinkReferences[refid];
						int len = linktext.Length + 2 + refid.Length + 2;
						if (hasspace)
						{
							len++;
						}

						// to be replaced
						string tbr = input.Substring(start, len);
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
				}
			}
			return input;
		}

		private string ReadToEndOfLine(int startindex, string input)
		{
			StringBuilder sb = new StringBuilder();
			int i = startindex;
			while (input[i] != '\r' && input[i] != '\n')
			{
				try
				{
					sb.Append(input[i]);
					i++;
				}
				catch (Exception)
				{
					break;
				}
			}
			return sb.ToString();
		}

		private string ReadToCharacter(char c, int startindex, string input)
		{
			StringBuilder sb = new StringBuilder();
			int i = startindex;
			while (input[i] != c)
			{
				try
				{
					sb.Append(input[i]);
					i++;
				}
				catch (Exception)
				{
					break;
				}
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
