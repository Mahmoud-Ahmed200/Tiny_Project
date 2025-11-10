using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public enum Token_Class
{
    //Reserved words
    Int, Float, String, Read, Write, Repeat, Until, If, Elseif, Else, Then, Return, Endl, End,

    //Values
    Identifier, Number, Comment, tiny_String,

    //Other Characters
    Semicolon, Comma, LParanthesis, RParanthesis,EqualOp, NotEqualOp, LessThanOp, GreaterThanOp, AndOp, OrOp,
    PlusOp, MinusOp, MultiplyOp, DivideOp, AssignmentOp,LCurlyBracket, RCurlyBracket
}
namespace Tiny_Compiler
{
    

    public class Token
    {
       public string lex;
       public Token_Class token_type;
        public Token()
        {
            lex = "";
        }
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.Elseif);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("endl", Token_Class.Endl);

            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("{", Token_Class.LCurlyBracket);
            Operators.Add("}", Token_Class.RCurlyBracket);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
            Operators.Add(":=", Token_Class.AssignmentOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);

        }

    public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (isWhiteSpace(CurrentChar))
                    continue;

                if (isLetter(CurrentChar))
                {
                    if (j < SourceCode.Length - 1)
                    {
                        ++j;
                        CurrentChar = SourceCode[j];
                        while (isLetter(CurrentChar) || isDigit(CurrentChar))
                        {
                            CurrentLexeme += SourceCode[j];
                            ++j;
                            if (j < SourceCode.Length)
                                CurrentChar = SourceCode[j];
                            else
                                break;
                        }
                        i = j - 1;
                    }
                   this.FindTokenClass(CurrentLexeme);
                }

                else if (isDigit(CurrentChar))
                {
                    if (j < SourceCode.Length - 1)
                    {
                        ++j;
                        CurrentChar = SourceCode[j];
                        while (isDigit(CurrentChar) || CurrentChar == '.'||isLetter(CurrentChar))
                        {
                            CurrentLexeme += SourceCode[j];
                            ++j;
                            if (j < SourceCode.Length)
                                CurrentChar = SourceCode[j];
                            else
                                break;
                        }
                        i = j - 1;
                    }
                    this.FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar == '/')
                {
                    if (j < SourceCode.Length - 1)
                    {
                        ++j;
                    }
                    CurrentChar = SourceCode[j];

                    if (CurrentChar == '*')
                    {
                        CurrentLexeme += CurrentChar;
                        ++j;
                        int k = j + 1;
                        char nextChar = ' ';
                        while (j < SourceCode.Length)
                        {
                            CurrentChar = SourceCode[j];
                            if (k < SourceCode.Length)
                            {
                                nextChar = SourceCode[k];
                            }
                            if (CurrentChar == '*' && nextChar == '/')
                            {
                                j += 2;
                                CurrentLexeme += "*/";
                                break;
                            }
                            CurrentLexeme += CurrentChar;
                            ++j;
                            ++k;
                        }
                    i = j - 1;
                    }
                     FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar == '"')
                {
                    ++j;
                    if (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                    }
                    while (CurrentChar != '"')
                    {
                        CurrentLexeme += CurrentChar;
                        j++;

                        if (j < SourceCode.Length)
                        {
                            CurrentChar = SourceCode[j];
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (j < SourceCode.Length && CurrentChar == '"')
                    {
                        CurrentLexeme += CurrentChar;
                    }
                    this.FindTokenClass(CurrentLexeme);
                    i = j;
                }
                else
                {
                    if (j < SourceCode.Length - 1) // &&
                    {
                        ++j;
                        char nextChar = SourceCode[j];

                        if (
                            CurrentChar == '&' && nextChar == '&' ||
                            CurrentChar == '|' && nextChar == '|' ||
                            CurrentChar == '<' && nextChar == '>' ||
                            CurrentChar == ':' && nextChar == '='
                            )
                        {
                            CurrentLexeme += nextChar;
                            ++j;
                        }
                        i = j - 1;
                    }
                    this.FindTokenClass(CurrentLexeme);

                }
            }
            Tiny_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Tok.lex))
            {
                TC=ReservedWords[Tok.lex];
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            //Is it an identifier?
            else if (isIdentifier(Tok.lex))
            {
                TC = Token_Class.Identifier;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            //Is it a Constant?
            else if(IsNumber(Tok.lex))
                {
                TC = Token_Class.Number;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            //Is it an operator?
            else if(Operators.ContainsKey(Tok.lex))
            {
                TC=Operators[Tok.lex];
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            //Check if comment?
            else if (IsComment(Tok.lex))
            {
                TC=Token_Class.Comment;
                Tok.token_type = TC;
                Tokens.Add(Tok);    
            }
            //Check if string
            else if (isString(Tok.lex))
            {
                TC = Token_Class.tiny_String;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            //Is it an undefined?
            else
            {
                Errors.Error_List.Add(Tok.lex);
            }
        }



        public bool isIdentifier(string lex)
        {
            return Regex.IsMatch(lex, @"^[A-Za-z][A-Za-z0-9]*$");
        }

        public bool IsNumber(string lex)
        {
            return Regex.IsMatch(lex, @"^[0-9]+(\.[0-9]+)?$");
        }

        public bool isString(string lex)
        {
            return Regex.IsMatch(lex, "^\"[^\"]*\"$");
        }

        public bool IsComment(string lex)
        {
            return Regex.IsMatch(lex, @"^/\*.*\*/$", RegexOptions.Singleline);
        }

        public bool isLetter(char c)
        {
            return Regex.IsMatch(c.ToString(), @"^[A-Za-z]$");
        }

        public bool isDigit(char c)
        {
            return Regex.IsMatch(c.ToString(), @"^[0-9]$");
        }

        static bool isWhiteSpace(char c)
        {
            return Regex.IsMatch(c.ToString(), @"[\s\r\n\t]");
        }

    }

}
