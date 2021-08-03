using System;
using System.IO;
using System.Text;

namespace DB_Benchmark.Disposables
{
    class OutputSaver : IDisposable // Based on: https://stackoverflow.com/a/6927051
    {
        FileStream fileStream;
        StreamWriter fileWriter;
        readonly TextWriter doubleWriter;
        readonly TextWriter oldOut;

        class DoubleWriter : TextWriter
        {
            readonly TextWriter one;
            readonly TextWriter two;

            public DoubleWriter(TextWriter one, TextWriter two)
            {
                this.one = one;
                this.two = two;
            }

            public override Encoding Encoding
            {
                get { return one.Encoding; }
            }

            public override void Flush()
            {
                one.Flush();
                two.Flush();
            }

            public override void Write(char value)
            {
                one.Write(value);
                two.Write(value);
            }
        }

        public OutputSaver(string path)
        {
            oldOut = Console.Out;

            try
            {
                fileStream = File.Create(path);

                fileWriter = new StreamWriter(fileStream)
                {
                    AutoFlush = true
                };

                doubleWriter = new DoubleWriter(fileWriter, oldOut);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open file for writing");
                Console.WriteLine(e.Message);
                return;
            }
            Console.SetOut(doubleWriter);
        }

        public void Dispose()
        {
            Console.SetOut(oldOut);

            if (fileWriter != null)
            {
                fileWriter.Flush();
                fileWriter.Close();
                fileWriter = null;
            }

            if (fileStream != null)
            {
                fileStream.Close();
                fileStream = null;
            }
        }
    }
}
