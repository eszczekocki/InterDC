using System;
using System.Collections.Generic;
using InterpreterCore;
using Nancy;
using Nancy.ModelBinding;

namespace Service
{
    public class FunctionModule : NancyModule
    {
        private class Function
        {
            public string name;
            public string body;

        }
        public FunctionModule()
        {
            Get["/function/{name}"] = parameters =>
                GlobalFunctionsContainer.GlobalFunctions[parameters.name];

            Post["/function"] = parameters =>
            {
                var model = this.Bind<Function>();
                GlobalFunctionsContainer.GlobalFunctions.Add(model.name, new CustomFunction(new List<string>(),model.body ));
                return "OK";
            };
        }
    }
}
