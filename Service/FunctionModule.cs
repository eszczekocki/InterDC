using System;
using System.Collections.Generic;
using InterpreterCore;
using Nancy;
using Nancy.Extensions;
using Nancy.ModelBinding;

namespace Service
{
    public class FunctionModule : NancyModule
    {
        private class Function
        {
            public string name;
            public string body;
            public string[] paramList;

        }

        private class FunctionInvoke
        {
            public List<Value> paramList;
        }
        public FunctionModule()
        {
            Get["/function/{name}"] = parameters =>
                GlobalFunctionsContainer.GlobalFunctions[parameters.name];

            Post["/function"] = parameters =>
            {
                
                var model = this.Bind<Function>();
               GlobalFunctionsContainer.GlobalFunctions.Add(
                    model.name, new CustomFunction(new List<string>(model.paramList),model.body));
                return "OK";
            };

            Post["/function/{name}/invokesync"] = parameters =>
            {

                var model = this.Bind<FunctionInvoke>();
                return GlobalFunctionsContainer.GlobalFunctions[parameters.name].Invoke(model.paramList);
            };


        }
    }
}
