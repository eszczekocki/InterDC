using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterpreterCore;
using Nancy;
using Nancy.ModelBinding;

namespace Service
{
    public class VariableModule : NancyModule
    {
        private class Variable
        {
            public string name;
            public Value value;
        }
        public VariableModule()
        {

            Get["/variable/{name}"] = parameters =>
            {
                Console.WriteLine($"GOT get /variable/{parameters.name}");

                return GlobalVariablesContainer.GlobalVars[parameters.name];
            };
            Post["/variable"] = parameters =>
            {

                var model = this.Bind<Variable>();
                Console.WriteLine($"GOT post /variable/{model.name} {model.value}");

                if (GlobalVariablesContainer.GlobalVars.ContainsKey(model.name) == false)
                    GlobalVariablesContainer.GlobalVars.Add(model.name, model.value);
                else
                    GlobalVariablesContainer.GlobalVars[model.name] = model.value;

                return "OK";
            };
        }
    }
}
