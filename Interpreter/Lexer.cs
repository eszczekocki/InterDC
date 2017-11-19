using System;
using System.Linq;
using System.Text;

namespace InterpreterCore
{
    public class Lexer
    {
        private readonly string _source;
        private Cursor _sourceMarker;
        private char _lastChar;

        public Cursor TokenMarker { get; set; }

        public string Identifer { get; set; }
        public Value Value { get; set; }

        public Lexer(string input)
        {
            _source = input;
            _sourceMarker = new Cursor(0, 1, 1);
            _lastChar = _source[0];
        }

        public void MoveCursor(Cursor cursor)
        {
            _sourceMarker = cursor;
        }


        public string GetFunctionBody()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                sb.Append(GetChar());
                if (sb.ToString().Contains("endfunc"))
                {
                    GetChar();
                    return sb.ToString().Replace("endfunc", "");
                }

            }
        }

        public string GetModuleBody()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                sb.Append(GetChar());
                if (sb.ToString().Contains("endmodule"))
                {
                    GetChar();
                    return sb.ToString().Replace("endmodule", "");
                }

            }
        }




        private char GetChar()
        {
            _sourceMarker.Column++;
            _sourceMarker.Pointer++;

            if (_sourceMarker.Pointer >= _source.Length)
                return _lastChar = (char)0;

            if ((_lastChar = _source[_sourceMarker.Pointer]) == '\n')
            {
                _sourceMarker.Column = 1;
                _sourceMarker.Line++;
            }
            return _lastChar;
        }

        public Token GetToken()
        {
            while (_lastChar == ' ' || _lastChar == '\t' || _lastChar == '\r')
                GetChar();

            TokenMarker = _sourceMarker;

            if (char.IsLetter(_lastChar) || _lastChar=='$')
            {
                Identifer = _lastChar.ToString();
                while (char.IsLetterOrDigit(GetChar()))
                    Identifer += _lastChar;
                switch (Identifer.ToUpper())
                {
                    case "PRINT": return Token.Print;
                    case "IF": return Token.If;
                    case "ENDIF": return Token.EndIf;
                    case "THEN": return Token.Then;
                    case "ELSE": return Token.Else;
                    case "FOR": return Token.For;
                    case "TO": return Token.To;
                    case "NEXT": return Token.Next;
                    case "INPUT": return Token.Input;
                    case "VAR": return Token.Var;
                    case "GOSUB": return Token.Gosub;
                    case "RETURN": return Token.Return;
                    case "END": return Token.End;
                    case "OR": return Token.Or;
                    case "AND": return Token.And;
                    case "FUNC": return Token.Func;
                    case "ENDFUNC": return Token.EndFunc;
                    case "NODE": return Token.Node;

                    case "REM":
                        while (_lastChar != '\n') GetChar();
                        GetChar();
                        return GetToken();
                    default:
                        return Token.Identifer;
                }
            }

            if (char.IsDigit(_lastChar))
            {
                string num = "";
                do { num += _lastChar; } while (char.IsDigit(GetChar()) || _lastChar == '.');

                double real;
                if (!double.TryParse(num, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out real))
                    throw new Exception("ERROR while parsing number");
                Value = new Value(real);
                return Token.Value;
            }

            Token tok = Token.Unkown;
            switch (_lastChar)
            {
                case '\n': tok = Token.NewLine; break;
                case ':': tok = Token.Colon; break;
                case ';': tok = Token.Semicolon; break;
                case ',': tok = Token.Comma; break;
                case '=': tok = Token.Equal; break;
                case '+': tok = Token.Plus; break;
                case '-': tok = Token.Minus; break;
                case '/': tok = Token.Slash; break;
                case '*': tok = Token.Asterisk; break;
                case '(': tok = Token.LParen; break;
                case ')': tok = Token.RParen; break;
                case '\'':
                    while (_lastChar != '\n') GetChar();
                    GetChar();
                    return GetToken();
                case '<':
                    GetChar();
                    if (_lastChar == '>') tok = Token.NotEqual;
                    else if (_lastChar == '=') tok = Token.LessEqual;
                    else return Token.Less;
                    break;
                case '>':
                    GetChar();
                    if (_lastChar == '=') tok = Token.MoreEqual;
                    else return Token.More;
                    break;
                case '"':
                    string str = "";
                    while (GetChar() != '"')
                    {
                        if (_lastChar == '\\')
                        {
                            switch (char.ToLower(GetChar()))
                            {
                                case 'n': str += '\n'; break;
                                case 't': str += '\t'; break;
                                case '\\': str += '\\'; break;
                                case '"': str += '"'; break;
                            }
                        }
                        else
                        {
                            str += _lastChar;
                        }
                    }
                    Value = new Value(str);
                    tok = Token.Value;
                    break;
                case '|':
                    string nodeStr = "";
                    while (GetChar() != '|')
                    {
                        if (_lastChar == '\\')
                        {
                            switch (char.ToLower(GetChar()))
                            {
                                case 'n': nodeStr += '\n'; break;
                                case 't': nodeStr += '\t'; break;
                                case '\\': nodeStr += '\\'; break;
                                case '"': nodeStr += '"'; break;
                            }
                        }
                        else
                        {
                            nodeStr += _lastChar;
                        }
                    }
                    Value = new Value(nodeStr,ValueType.Node);
                    tok = Token.Value;
                    break;


                case (char)0:
                    return Token.EOF;
            }

            GetChar();
            return tok;
        }
    }
}

