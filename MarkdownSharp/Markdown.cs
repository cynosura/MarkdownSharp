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

		}

		private string ReadToEndOfLine(int startindex, string input)
		{
			StringBuilder sb = new StringBuilder();
			int i = startindex;
			while (input[i] != '\r' && input[i] != '\n')
			{
				sb.Append(input[i]);
				i++;
			}
			return sb.ToString();
		}

		private string ReadToCharacter(char c, int startindex, string input)
		{
			StringBuilder sb = new StringBuilder();
			int i = startindex;
			while (input[i] != c)
			{
				sb.Append(input[i]);
				i++;
			}
			return sb.ToString();
		}
	}
}
