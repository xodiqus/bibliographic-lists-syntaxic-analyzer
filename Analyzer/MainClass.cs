﻿using CommandLine;
using System;
using System.IO;

namespace bibliographic_lists_syntaxic_analyzer
{
    class MainClass
    {
        static void Main(string[] args)
        {
            args = new string[] { "-f", @"C:\Users\user\Documents\bibliographic-lists-syntaxic-analyzer\Tests\Test.docx" };
            //args = new string[] { "-r", @"Федощев А.Г. Муниципальное право в схемах и определениях. Федощева Н.Н. М. : Юристъ, 2007. 162 с." };

            CommandLineOptions options = null;
            CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(opt => options = opt)
                .WithNotParsed(errs =>
                {
                    foreach (var err in errs)
                    {
                        Console.WriteLine(err.ToString());
                    }
                });

            if (options != null)
            {
                StreamWriter writer = null;
                var analyzer = new Analyzer();

                if (options.Output == "console")
                {
                    analyzer.Log = s => { Console.WriteLine(s); };
                }
                else
                {
                    writer = new StreamWriter(options.Output);
                    writer.AutoFlush = true;
                    analyzer.Log = s => { writer.WriteLine(s); };
                }

                if (options.StandardName != null)
                {
                    try
                    {
                        analyzer.Standard = Standard.Get(options.StandardName);
                    }
                    catch (UnknownStandardException ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                        return;
                    }
                }

                if (options.SingleReference != null)
                {
                    analyzer.Analyze(options.SingleReference);
                }

                if (options.File != null)
                {
                    var parser = new Parser(analyzer.Standard);
                    var refs = parser.ParseRefsFromDocx(options.File);
                    foreach (var r in refs)
                    {
                        analyzer.Analyze(r);
                    }
                }

                writer?.Dispose();
            }
        }
    }
}
