using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Channels;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

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

            if (args[0].Type != ValueType.Node)
                throw new Exception("In Function SetFunction first argument must be a node");

            var client = new RestClient($"Http://{args[0]}:1234");
            var request = new RestRequest("function", Method.POST);

            if (args.Count == 2)
            {

                request.AddJsonBody(new
                {
                    name = args[1].String,
                    body = GlobalFunctionsContainer.GlobalFunctions[args[1].String]._body,
                    paramList = GlobalFunctionsContainer.GlobalFunctions[args[1].String]._argsList
                });
            }
            else
            {
                var paramList = new List<String>();
                for (int i = 3; i < args.Count; i++)
                {
                    paramList.Add(args[i].String);

                }

                request.AddJsonBody(new
                {
                    name = args[1].String,
                    body = args[2].String,
                    paramList
                });
            }

            request.AddHeader("Content-type", "application/json");
            request.RequestFormat = DataFormat.Json;
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            Console.WriteLine(content);
            return new Value("OK");
        }



        public static Value GetFunction(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 2)
                throw new ArgumentException();
            if (args[0].Type != ValueType.Node)
                Console.WriteLine($"{args[0]}, {args[1]}, {args[2]}");

            try
            {
                var client = new RestClient($"Http://{args[0]}:1234");

                var request = new RestRequest("function/{name}", Method.GET);
                request.AddUrlSegment("name", args[1].String);
                request.AddHeader("Content-type", "application/json");
                request.RequestFormat = DataFormat.Json;
                IRestResponse response = client.Execute(request);
                GlobalFunctionsContainer.GlobalFunctions.Add(args[1].String,
                    new JsonDeserializer().Deserialize<CustomFunction>(response)
                );
                return new Value("OK");
            }
            catch (Exception ex)
            {
                return new Value("ERR");
            }
        }

        public static Value InvokeSync(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 2)
                throw new ArgumentException();
            if (args[0].Type != ValueType.Node)
                Console.WriteLine($"{args[0]}, {args[1]}, {args[2]}");

            try
            {
                var client = new RestClient($"Http://{args[0]}:1234");

                var request = new RestRequest("function/{name}/invokesync", Method.POST);
                request.AddUrlSegment("name", args[1].String);
                request.AddHeader("Content-type", "application/json");
                request.RequestFormat = DataFormat.Json;
                var _paramList = new List<Value>();
                for (int i = 2; i < args.Count; i++)
                {
                    _paramList.Add(args[i]);

                }

                request.AddJsonBody(new
                {
                    paramList = _paramList
                });



                IRestResponse response = client.Execute(request);
                return new JsonDeserializer().Deserialize<Value>(response);
            }
            catch (Exception ex)
            {
                return new Value("ERR");
            }
        }
        public static Value InvokeAsync(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 3)
                throw new ArgumentException();
            if (args[0].Type != ValueType.Node)
                throw new Exception("In Function InvokeAsync first argument must be a node");

            Console.WriteLine($"{args[0]}, {args[1]}, {args[2]}");

            return new Value(0);
        }

        public static Value GetVar(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 2)
                throw new ArgumentException();
            if (args[0].Type != ValueType.Node)
                Console.WriteLine($"ERROR 1st argument must be a node");

            try
            {
                var client = new RestClient($"Http://{args[0]}:1234");

                var request = new RestRequest("variable/{name}", Method.GET);
                request.AddUrlSegment("name", args[1].String);
                request.AddHeader("Content-type", "application/json");
                request.RequestFormat = DataFormat.Json;
                IRestResponse response = client.Execute(request);
                return new JsonDeserializer().Deserialize<Value>(response);
            }
            catch (Exception ex)
            {
                return new Value("ERR");
            }
        }
        public static Value SetVar(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 3)
                throw new ArgumentException();
            if (args[0].Type != ValueType.Node)
                Console.WriteLine($"{args[0]}, {args[1]}, {args[2]}");

            try
            {
                var client = new RestClient($"Http://{args[0]}:1234");

                var request = new RestRequest("variable", Method.POST);
                request.AddHeader("Content-type", "application/json");
                request.RequestFormat = DataFormat.Json;

                request.AddJsonBody(new
                {
                    name = args[1].String,
                    value = args[2]
                });

                IRestResponse response = client.Execute(request);
                return new Value("OK");
            }
            catch (Exception ex)
            {
                return new Value("ERR");
            }
        }
    }
}
