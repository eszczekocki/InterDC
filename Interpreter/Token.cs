
namespace InterpreterCore
{
    public enum Token
    {
        Unkown,

        Identifer,
        Value,

        //Keywords
        Print,
        If,
		EndIf,
        Then,
        Else,
        For,
        To,
        Next,
        Input,
        Var,
        Gosub,
        Return,
        Rem,
        End,
        Func,
        EndFunc,
        Module,
        EndModule,
        Node,

        NewLine,
        Colon,
        Semicolon,
        Comma,

        Plus,
        Minus,
        Slash,
        Asterisk,
        Equal,
        Less,
        More,
        NotEqual,
        LessEqual,
        MoreEqual,
        Or,
        And,
        Not,

        LParen,
        RParen,

        EOF = -1   //End Of File
    }
}
