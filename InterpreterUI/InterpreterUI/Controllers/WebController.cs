using InterpreterCore;
using InterpreterUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InterpreterUI.Controllers
{
    public class WebController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(ServiceMessage model)
        {
            //model.response = model.request;
            Interpreter interpreter = new Interpreter(model.request);
            interpreter.Exec();
            model.response = interpreter.outputValue;

            return View(model);
        }
    }
}