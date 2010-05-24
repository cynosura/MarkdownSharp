using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MarkdownSharp;
namespace MarkdownSharpTester
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Reading markdown file...");
			StreamReader sr = File.OpenText("test.md");
			string txt = sr.ReadToEnd();
			Markdown md = new Markdown(txt);
			Console.WriteLine("Parsing and writing to test.html...");
			File.WriteAllText("test.html", md.Parse());
			Console.WriteLine("Done!");
		}
	}
}
