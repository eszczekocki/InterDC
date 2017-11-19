using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using RestSharp;

namespace InterpreterCore
{
    class BuiltInFunctions
    {
        public static void InstallAll(Interpreter interpreter)
        {
            interpreter.AddFunction("str", Str);
            interpreter.AddFunction("num", Num);
            interpreter.AddFunction("abs", Abs);
            interpreter.AddFunction("min", Min);
            interpreter.AddFunction("max", Max);
            interpreter.AddFunction("not", Not);
            interpreter.AddFunction("setFunction", SetFunction);
            interpreter.AddFunction("getFunction", GetFunction);
            interpreter.AddFunction("getVar", GetVar);
            interpreter.AddFunction("setVar", SetVar);
            interpreter.AddFunction("invokeSync", InvokeSync);
            interpreter.AddFunction("invokeAsync", InvokeAsync);



        }

        public static Value Str(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return args[0].Convert(ValueType.String);
        }

        public static Value Num(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return args[0].Convert(ValueType.Real);
        }

        public static Value Abs(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return new Value(Math.Abs(args[0].Real));
        }

        public static Value Min(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 2)
                throw new ArgumentException();

            return new Value(Math.Min(args[0].Real, args[1].Real));
        }

        public static Value Max(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return new Value(Math.Max(args[0].Real, args[1].Real));
        }

        public static Value Not(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return new Value(args[0].Real == 0 ? 1 : 0);
        }
        public static Value SetFunction(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 3)
                throw new ArgumentException();
            if(args[0].Type!=ValueType.Node)
                throw new Exception("In Function SetFunction first argument must be a node");

            Console.WriteLine($"{args[0]}, {args[1]}, {args[2]}");
            var client = new RestClient($"Http://{args[0]}:1234");
            var request = new RestRequest("function", Method.POST);
            request.AddParameter("name",  args[1]);
            request.AddParameter("body",  args[2]);
            //request.AddParameter("args",  args[3]);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            Console.WriteLine(content);
            return new Value(0);
        }
        public static Value GetFunction(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 3)
                throw new ArgumentException();
            if(args[0].Type!=ValueType.Node)
                throw new Exception("In Function GetFunction first argument must be a node");

            Console.WriteLine($"{args[0]}, {args[1]}, {args[2]}");

            return new Value(0);
        }
        public static Value GetVar(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 2)
                throw new ArgumentException();
            if(args[0].Type!=ValueType.Node)
                throw new Exception("In Function GetVar first argument must be a node");

            Console.WriteLine($"{args[0]}, {args[1]}, {args[2]}");

            return new Value(0);
        }
        public static Value SetVar(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 3)
                throw new ArgumentException();
            if(args[0].Type!=ValueType.Node)
                throw new Exception("In Function SetVar first argument must be a node");

            Console.WriteLine($"{args[0]}, {args[1]}, {args[2]}");

            return new Value(0);
        }
        public static Value InvokeSync(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 2)
                throw new ArgumentException();
            if(args[0].Type!=ValueType.Node)
                throw new Exception("In Function InvokeSync first argument must be a node");

            Console.WriteLine($"{args[0]}, {args[1]}, {args[2]}");

            return new Value(0);
        }
        public static Value InvokeAsync(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 3)
                throw new ArgumentException();
            if(args[0].Type!=ValueType.Node)
                throw new Exception("In Function InvokeAsync first argument must be a node");

            Console.WriteLine($"{args[0]}, {args[1]}, {args[2]}");

            return new Value(0);
        }
    }
}
