using System;

namespace InterpreterCore
{
    public enum ValueType
    {
        Real,
        String,
        Node
    }

    public struct Value
    {
        public static readonly Value Zero = new Value(0);
        public static readonly Value Empty = new Value(string.Empty);
        public ValueType Type { get; set; }

        public double Real { get; set; }
        public string String { get; set; }

        public Value(double real) : this()
        {
            Type = ValueType.Real;
            Real = real;
        }

        public Value(string str)
            : this()
        {
            if (str.StartsWith("|") && str.EndsWith("|"))
                Type = ValueType.Node;
            else
                Type = ValueType.String;
            String = str;
        }
        public Value(string str, ValueType type)
            : this()
        {
            Type=ValueType.Node;
            String = str;
        }

        public Value Convert(ValueType type)
        {
            if (Type != type)
            {
                switch (type)
                {
                    case ValueType.Real:
                        Real = double.Parse(String);
                        Type = ValueType.Real;
                        break;
                    case ValueType.String:
                        String = Real.ToString();
                        Type = ValueType.String;
                        break;
                }
            }
            return this;
        }
        
        public override string ToString()
        {
            if (this.Type == ValueType.Real)
                return this.Real.ToString();
            return this.String;
        }
    }

    public static class ValueExtensions
    {


        public static Value OnlyFor(this Value @this,ValueType vt)
        {
            return @this.Type==vt 
                ? @this
                : throw new Exception("Cannot do binop on strings(except +).");
        }

        public static Value BinOp(this Value @this, Value b, Token tok)
        {
           if (@this.Type != b.Type)
            {
                if (@this.Type > b.Type)
                    b = b.Convert(@this.Type);
                else
                    @this = @this.Convert(b.Type);
            }

            switch (tok)
            {

                case Token.Plus:
                    return @this.Type == ValueType.Real
                        ? new Value(@this.Real + b.Real)
                        : new Value(@this.String + b.String);
                case Token.Equal:
                    return @this.Type == ValueType.Real
                        ? new Value(@this.Real == b.Real ? 1 : 0)
                        : new Value(@this.String == b.String ? 1 : 0);
                case Token.NotEqual:
                    return @this.Type == ValueType.Real
                        ? new Value(@this.Real == b.Real ? 0 : 1)
                        : new Value(@this.String == b.String ? 0 : 1);

                case Token.Minus:
                    return (new Value(@this.Real - b.Real)).OnlyFor(ValueType.Real);
                case Token.Asterisk:
                    return (new Value(@this.Real * b.Real)).OnlyFor(ValueType.Real);
                case Token.Slash:
                    return (new Value(@this.Real / b.Real)).OnlyFor(ValueType.Real);


                case Token.Less:
                    return (new Value(@this.Real < b.Real ? 1 : 0)).OnlyFor(ValueType.Real);
                case Token.More:
                    return (new Value(@this.Real > b.Real ? 1 : 0)).OnlyFor(ValueType.Real);
                case Token.LessEqual:
                    return (new Value(@this.Real <= b.Real ? 1 : 0)).OnlyFor(ValueType.Real);
                case Token.MoreEqual:
                    return (new Value(@this.Real >= b.Real ? 1 : 0)).OnlyFor(ValueType.Real);
                default:
                    return @this;
            }

               
            
        }
    }
}
