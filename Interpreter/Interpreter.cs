using System;
using System.Collections.Generic;
namespace InterpreterCore
{
    public class Interpreter
    {
        private readonly Lexer _lex;
        private Token _lastToken;

        private readonly Dictionary<string, Value> _vars;
        private readonly Dictionary<string, Cursor> _loops = new Dictionary<string, Cursor>();

        public delegate Value BasicFunction(Interpreter interpreter, List<Value> args);
        private readonly Dictionary<string, BasicFunction> _funcs = new Dictionary<string, BasicFunction>();
        private readonly Dictionary<string, CustomFunction> _customFuncs = new Dictionary<string, CustomFunction>();

        public string outputValue { get; set; }


        private readonly int _operationIfcounter = 0;

        private Cursor _lineMarker;

        private bool _exit;

        public Token PrevToken { get; set; }

        public Interpreter(string input)
        {
            _lex = new Lexer(input);
            _vars = new Dictionary<string, Value>();
            BuiltInFunctions.InstallAll(this);
        }

        public Interpreter(string input, Dictionary<string, Value> args)
        {
            _lex = new Lexer(input);
            _vars = args ?? new Dictionary<string, Value>();
            BuiltInFunctions.InstallAll(this);
        }

        public Value GetVar(string name)
        {
            if (!name.StartsWith("$"))
            {
                if (!_vars.ContainsKey(name))
                    throw new Exception($"Local variable with name " + name + " does not exist.");
                return _vars[name];
            }
            else
            {
                if (!GlobalVariablesContainer.GlobalVars.ContainsKey(name))
                    throw new Exception($"Global variable with name " + name + " does not exist.");
                return _vars[name];
            }
        }
        public Value GetNode(string name)
        {
            
                if (!GlobalNodesContainer.Nodes.ContainsKey(name))
                    throw new Exception($"Node with name " + name + " does not exist.");
                return _vars[name];
            
        }

        public void SetVar(string name, Value val)
        {
            if (!name.StartsWith("$"))
            {
                if (!_vars.ContainsKey(name)) _vars.Add(name, val);
                else _vars[name] = val;
            }
            else
            {
                if (!GlobalVariablesContainer.GlobalVars.ContainsKey(name))
                    GlobalVariablesContainer.GlobalVars.Add(name, val);
                else GlobalVariablesContainer.GlobalVars[name] = val;
            }
        }

        public void SetNode(string name, Value val)
        {
            
                if (!GlobalNodesContainer.Nodes.ContainsKey(name))
                    GlobalNodesContainer.Nodes.Add(name, val);
                else GlobalNodesContainer.Nodes[name] = val;
            
        }

        public void AddFunction(string name, BasicFunction function)
        {
            if (!_funcs.ContainsKey(name)) _funcs.Add(name, function);
            else _funcs[name] = function;
        }

        void Error(string text)
        {
            throw new Exception(text + " at line: " + _lineMarker.Line);
        }

        void Match(Token tok)
        {
            if (_lastToken != tok)
                Error($"Expect " + tok.ToString() + " got " + _lastToken.ToString());
        }

        public void Exec()
        {
            _exit = false;
            GetNextToken();
            while (!_exit) Line();
        }

        Token GetNextToken()
        {
            PrevToken = _lastToken;
            _lastToken = _lex.GetToken();

            if (_lastToken == Token.EOF && PrevToken == Token.EOF)
                Error("Unexpected end of file");

            return _lastToken;
        }

        void Line()
        {
            while (_lastToken == Token.NewLine) GetNextToken();

            if (_lastToken == Token.EOF)
            {
                _exit = true;
                return;
            }

            _lineMarker = _lex.TokenMarker;
            Statment();

            if (_lastToken != Token.NewLine && _lastToken != Token.EOF)
                Error("Expect new line got " + _lastToken.ToString());
        }

        void Statment()
        {
            Token keyword = _lastToken;
            GetNextToken();
            switch (keyword)
            {
                case Token.Print: Print(); break;
                case Token.If: If(); break;
                case Token.Else: Else(); break;
                case Token.EndIf: break;
                case Token.For: For(); break;
                case Token.Next: Next(); break;
                case Token.Var: Variable(); break;
                case Token.End: End(); break;
                case Token.Return: Return(); break;
                case Token.Func: Function(); break;
                case Token.Node: Node(); break;
                case Token.Identifer:
                    if (_lastToken == Token.Equal) Variable();
                    else
                    {
                        _lastToken = keyword;
                        Expr();
                    }
                    break;
                case Token.EOF:
                    _exit = true;
                    break;
                default:
                    Error("Expect keyword got " + keyword.ToString());
                    break;
            }
            if (_lastToken == Token.Colon)
            {
                GetNextToken();
                Statment();
            }
        }

        private void Function()
        {
            GetNextToken();
            string functionName = _lex.Identifer;
            List<string> args = new List<string>();

            if (_lastToken == Token.LParen)
            {
                GetNextToken();

                while (_lastToken != Token.RParen)
                {
                    Match(Token.Identifer);
                    args.Add(_lex.Identifer);
                    GetNextToken();
                    if (_lastToken == Token.Comma)
                        GetNextToken();

                }

            }
            
            if (!functionName.StartsWith("$"))
                _customFuncs.Add(functionName, new CustomFunction(args, _lex.GetFunctionBody()));
            else
                GlobalFunctionsContainer.GlobalFunctions.Add(functionName, 
                    new CustomFunction(args, _lex.GetFunctionBody()));
            _lastToken = Token.NewLine;
        }


        private void Return()
        {
            SetVar("return", Expr());
            _exit = true;
        }

        void Print()
        {
            //Console.WriteLine(Expr().ToString());
            outputValue += String.Concat(Expr().ToString(), Environment.NewLine);
            
        }



        void If()
        {
            bool result = (Expr().BinOp(new Value(0), Token.Equal).Real == 1);

            Match(Token.Then);
            GetNextToken();

            if (result)
            {
                int i = _operationIfcounter;
                while (true)
                {
                    if (_lastToken == Token.If)
                    {
                        i++;
                    }
                    else if (_lastToken == Token.Else)
                    {
                        if (i == _operationIfcounter)
                        {
                            GetNextToken();
                            return;
                        }
                    }
                    else if (_lastToken == Token.EndIf)
                    {
                        if (i == _operationIfcounter)
                        {
                            GetNextToken();
                            return;
                        }
                        i--;
                    }
                    GetNextToken();
                }
            }
        }

        void Else()
        {
            int i = _operationIfcounter;
            while (true)
            {
                if (_lastToken == Token.If)
                {
                    i++;
                }
                else if (_lastToken == Token.EndIf)
                {
                    if (i == _operationIfcounter)
                    {
                        GetNextToken();
                        return;
                    }
                    i--;
                }
                GetNextToken();
            }
        }

        void End()
        {
            _exit = true;
        }

        void Variable()
        {
            if (_lastToken != Token.Equal)
            {
                Match(Token.Identifer);
                GetNextToken();
                Match(Token.Equal);
            }

            string id = _lex.Identifer;

            GetNextToken();

            SetVar(id, Expr());
        }
        void Node()
        {
            if (_lastToken != Token.Equal)
            {
                Match(Token.Identifer);
                GetNextToken();
                Match(Token.Equal);
            }

            string id = _lex.Identifer;

            GetNextToken();

            SetVar(id, Expr());
        }

        void For()
        {
            Match(Token.Identifer);
            string var = _lex.Identifer;

            GetNextToken();
            Match(Token.Equal);

            GetNextToken();
            Value v = Expr();

            if (_loops.ContainsKey(var))
            {
                _loops[var] = _lineMarker;
            }
            else
            {
                SetVar(var, v);
                _loops.Add(var, _lineMarker);
            }

            Match(Token.To);

            GetNextToken();
            v = Expr();

            if (_vars[var].BinOp(v, Token.More).Real == 1)
            {
                while (true)
                {
                    while (!(GetNextToken() == Token.Identifer && PrevToken == Token.Next)) ;
                    if (_lex.Identifer == var)
                    {
                        _loops.Remove(var);
                        GetNextToken();
                        Match(Token.NewLine);
                        break;
                    }
                }
            }

        }

        void Next()
        {
            Match(Token.Identifer);
            string var = _lex.Identifer;
            _vars[var] = _vars[var].BinOp(new Value(1), Token.Plus);
            _lex.MoveCursor(new Cursor(_loops[var].Pointer - 1, _loops[var].Line, _loops[var].Column - 1));
            _lastToken = Token.NewLine;
        }

        Value Expr()
        {
            Dictionary<Token, int> prec = new Dictionary<Token, int>()
            {
                { Token.Or, 0 }, { Token.And, 0 },
                { Token.Equal, 1 }, { Token.NotEqual, 1 },
                { Token.Less, 1 }, { Token.More, 1 }, { Token.LessEqual, 1 },  { Token.MoreEqual, 1 },
                { Token.Plus, 2 }, { Token.Minus, 2 },
                { Token.Asterisk, 3 }, {Token.Slash, 3 }
            };

            Stack<Value> stack = new Stack<Value>();
            Stack<Token> operators = new Stack<Token>();

            int i = 0;
            while (true)
            {
                if (_lastToken == Token.Value)
                {
                    stack.Push(_lex.Value);
                }
                else if (_lastToken == Token.Identifer)
                {
                    if (GlobalNodesContainer.Nodes.ContainsKey(_lex.Identifer))
                    {
                        stack.Push(GlobalNodesContainer.Nodes[_lex.Identifer]);
                    }
                    else if (_vars.ContainsKey(_lex.Identifer))
                    {
                        stack.Push(_vars[_lex.Identifer]);
                    }
                    else if (GlobalVariablesContainer.GlobalVars.ContainsKey(_lex.Identifer))
                    {
                        stack.Push(GlobalVariablesContainer.GlobalVars[_lex.Identifer]);
                    }
                    else if (_funcs.ContainsKey(_lex.Identifer))
                    {
                        string name = _lex.Identifer;
                        List<Value> args = new List<Value>();
                        GetNextToken();
                        Match(Token.LParen);

                        start:
                        if (GetNextToken() != Token.RParen)
                        {
                            args.Add(Expr());
                            if (_lastToken == Token.Comma)
                                goto start;
                        }

                        stack.Push(_funcs[name](null, args));
                    }
                    else if (_customFuncs.ContainsKey(_lex.Identifer))
                    {
                        string name = _lex.Identifer;
                        List<Value> args = new List<Value>();

                        //Match(Token.LParen);

                        GetNextToken();
                        start:
                        if (GetNextToken() != Token.RParen)
                        {
                            args.Add(_lex.Value);
                            GetNextToken();
                            if (_lastToken == Token.Comma)
                                goto start;
                        }

                        stack.Push(_customFuncs[name].Invoke(args));
                    }
                    else if (GlobalFunctionsContainer.GlobalFunctions.ContainsKey(_lex.Identifer))
                    {
                        string name = _lex.Identifer;
                        List<Value> args = new List<Value>();

                        //Match(Token.LParen);

                        GetNextToken();
                        start:
                        if (GetNextToken() != Token.RParen)
                        {
                            args.Add(_lex.Value);
                            GetNextToken();
                            if (_lastToken == Token.Comma)
                                goto start;
                        }

                        stack.Push(GlobalFunctionsContainer.GlobalFunctions[name].Invoke(args));
                    }


                    else
                    {
                        Error("Undeclared variable " + _lex.Identifer);
                    }
                }
                else if (_lastToken == Token.LParen)
                {
                    GetNextToken();
                    stack.Push(Expr());
                    Match(Token.RParen);
                }
                else if (_lastToken >= Token.Plus && _lastToken <= Token.Not)
                {
                    if ((_lastToken == Token.Minus || _lastToken == Token.Minus) && (i == 0 || PrevToken == Token.LParen))
                    {
                        stack.Push(new Value(0));
                        operators.Push(_lastToken);
                    }
                    else
                    {
                        while (operators.Count > 0 && prec[_lastToken] <= prec[operators.Peek()])
                            Operation(ref stack, operators.Pop());
                        operators.Push(_lastToken);
                    }
                }
                else
                {
                    if (i == 0)
                        Error("Empty expression");
                    break;
                }

                i++;
                GetNextToken();
            }

            while (operators.Count > 0)
                Operation(ref stack, operators.Pop());

            return stack.Pop();
        }

        void Operation(ref Stack<Value> stack, Token token)
        {
            Value b = stack.Pop();
            Value a = stack.Pop();
            Value result = a.BinOp(b, token);
            stack.Push(result);
        }

        public Value ExecFuncBody()
        {
            _exit = false;
            GetNextToken();
            while (!_exit) Line();

            return GetVar("return");
        }
    }
}
