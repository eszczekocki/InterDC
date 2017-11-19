using System;
using System.IO;

namespace Interpreter.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory, "*.bas"))
            {
                InterpreterCore.Interpreter interpreter = new InterpreterCore.Interpreter(File.ReadAllText(file));
                try
                {
                    interpreter.Exec();
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine(e.Message);
                    continue;
                }
                Console.WriteLine("Ended - OK");
            }
            Console.Read();
        }
    }
}
