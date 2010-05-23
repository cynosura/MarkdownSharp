using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownSharp
{
	public class Markdown
	{
		private string InputText;
		private int place;

		public Markdown(string input)
		{
			InputText = input;
			place = 0;
		}

		public string Parse()
		{
			string OutputString = InputText;

			OutputString = ParseLinks(OutputString);

			return OutputString;
		}

		private string ParseLinks(string input) {
			for (int i = 0; i < input.Length; i++) {
				if (input[i] == '[' && input[i-1] != '\\') {
					int start = i;
					string linktext = ReadToCharacter(']', i+1, input);
					i+= linktext.Length+2;
					if (input[i] == '(') {
						string parencontents = ReadToCharacter(')', i+1, input);
						if (parencontents.IndexOf(' ') > -1) {
							string[] spl = parencontents.Split(new char[] {' '}, 1);
							string url = spl[0];
							string title = spl[1];
						} else {
							string url = parencontents;
							string title = null;
						}
						int len = linktext.Length+2+parenscontent+2;
						// to be replaced
						string tbr = input.Substring(start, len);
						// to replace with
						string trw ="<a href=\"" + url +"\"";
						if (title != null) {
							trw += " title=" + title;
						}
						trw += ">" + linktext + "</a>";
						input.Replace(tbr, trw);
						i+=len;
					} else {
						// Link is a reference
					}
				}
			}
		}

		private string ReadToEndOfLine(int startindex, string input)
		{
			StringBuilder sb = new StringBuilder();
			int i = startindex;
			while (input[i] != '\r' && input[i] != '\n')
			{
				try {
					sb.Append(input[i]);
					i++;
				} catch (Exception) {
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
				try {
					sb.Append(input[i]);
					i++;
				} catch (Exception) {
					break;
				}
			}
			return sb.ToString();
		}
	}
}
