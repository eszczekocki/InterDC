using System.Collections.Generic;
using System.Linq;

namespace InterpreterCore
{
    public class CustomFunction
    {
        public readonly Dictionary<string, Value> _args;
        public readonly string _body;
        public readonly List<string> _argsList;


        public CustomFunction(List<string> argsList, string body)
        {
            _argsList = argsList;
            _body = body;
            _args = new Dictionary<string, Value>();
            _argsList.ForEach(x => _args.Add(x, Value.Zero));
        }

        public Value Invoke(List<Value> args)
        {
            for (int i = 0; i < args.Count; i++)
            {
                _args[_args.ElementAt(i).Key] = args[i];
            }
            var interpreter = new Interpreter(_body, _args);
            return interpreter.ExecFuncBody();
        }
    }
}