using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tiny_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }

    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = Program();
            //root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Function_Statements());
            program.Children.Add(Main_Function());
            MessageBox.Show("Success");
            return program;
        }

        Node Function_Statements()
        {
            Node n = new Node("Function_Statements");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if ((TokenStream[InputPointer].token_type == Token_Class.Int ||
                 TokenStream[InputPointer].token_type == Token_Class.Float ||
                 TokenStream[InputPointer].token_type == Token_Class.String)
                &&
                (Is_validInputPointer(InputPointer + 1) &&
                 TokenStream[InputPointer + 1].token_type == Token_Class.Identifier)
                &&
                (Is_validInputPointer(InputPointer + 2) &&
                 TokenStream[InputPointer + 2].token_type == Token_Class.LParanthesis))
            {
                n.Children.Add(Function_Statement());
                n.Children.Add(Function_Statements());
            }
            return n;
        }

        Node Function_Statement()
        {
            Node n = new Node("Function_Statement");
            n.Children.Add(Function_Decleration());
            n.Children.Add(Function_Body());
            return n;
        }

        Node Function_Decleration()
        {
            Node n = new Node("Function_Decleration");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if ((TokenStream[InputPointer].token_type == Token_Class.Int ||
                 TokenStream[InputPointer].token_type == Token_Class.Float ||
                 TokenStream[InputPointer].token_type == Token_Class.String)
                &&
                (Is_validInputPointer(InputPointer + 1) &&
                 TokenStream[InputPointer + 1].token_type == Token_Class.Identifier)
                &&
                (Is_validInputPointer(InputPointer + 2) &&
                 TokenStream[InputPointer + 2].token_type == Token_Class.LParanthesis))
            {
                n.Children.Add(Datatype());
                n.Children.Add(FunctionName());
                n.Children.Add(match(Token_Class.LParanthesis));
                n.Children.Add(Parameters());
                n.Children.Add(match(Token_Class.RParanthesis));
            }
            return n;
        }

        Node Datatype()
        {
            Node n = new Node("Datatype");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.Int)
                n.Children.Add(match(Token_Class.Int));
            else if (TokenStream[InputPointer].token_type == Token_Class.Float)
                n.Children.Add(match(Token_Class.Float));
            else if (TokenStream[InputPointer].token_type == Token_Class.String)
                n.Children.Add(match(Token_Class.String));
            return n;
        }

        Node FunctionName()
        {
            Node n = new Node("FunctionName");
            if (Is_validInputPointer(InputPointer) && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                n.Children.Add(match(Token_Class.Identifier));
            }
            return n;
        }

        Node Parameters()
        {
            Node n = new Node("Parameters");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.Int ||
                TokenStream[InputPointer].token_type == Token_Class.Float ||
                TokenStream[InputPointer].token_type == Token_Class.String)
            {
                n.Children.Add(Parameter());
                n.Children.Add(ParametersDash());
            }
            return n;
        }

        Node ParametersDash()
        {
            Node n = new Node("ParameterDash");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                n.Children.Add(match(Token_Class.Comma));
                n.Children.Add(Parameter());
                n.Children.Add(ParametersDash());
            }
            return n;
        }

        Node Parameter()
        {
            Node n = new Node("Parameter");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.Int ||
                TokenStream[InputPointer].token_type == Token_Class.Float ||
                TokenStream[InputPointer].token_type == Token_Class.String)
            {
                n.Children.Add(Datatype());
                n.Children.Add(match(Token_Class.Identifier));
            }
            return n;
        }

        Node Function_Body()
        {
            Node n = new Node("Function_Body");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.LCurlyBracket)
            {
                n.Children.Add(match(Token_Class.LCurlyBracket));
                n.Children.Add(Statements());
                n.Children.Add(Return_Statement());
                n.Children.Add(match(Token_Class.RCurlyBracket));
            }
            return n;
        }


        Node Statements()
        {
            Node n = new Node("Statements");

            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.RCurlyBracket ||
                TokenStream[InputPointer].token_type == Token_Class.End ||
                TokenStream[InputPointer].token_type == Token_Class.Else ||
                TokenStream[InputPointer].token_type == Token_Class.Elseif ||
                TokenStream[InputPointer].token_type == Token_Class.Until ||
                TokenStream[InputPointer].token_type == Token_Class.Return)
                return n;
            Node stmt = Statement();
            if (stmt == null)
                return n;
            n.Children.Add(stmt);
            n.Children.Add(Statements());     
            return n;
        }

        Node Statement()
        {
            Node n = new Node("Statement");

            if (!Is_validInputPointer(InputPointer))
                return null;
            if (TokenStream[InputPointer].token_type == Token_Class.Write)
            {
                n.Children.Add(Write_Statement());
                return n;
            }
            if (TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                n.Children.Add(Read_Statement());
                return n;
            }
            if (TokenStream[InputPointer].token_type == Token_Class.Identifier &&
                Is_validInputPointer(InputPointer + 1) &&
                TokenStream[InputPointer + 1].token_type == Token_Class.AssignmentOp)
            {
                n.Children.Add(Assignment_Statement());
                return n;
            }
            if (TokenStream[InputPointer].token_type == Token_Class.Int ||
                TokenStream[InputPointer].token_type == Token_Class.Float ||
                TokenStream[InputPointer].token_type == Token_Class.String)
            {
                n.Children.Add(Decleration_Statement());
                return n;
            }
            if (TokenStream[InputPointer].token_type == Token_Class.If)
            {
                n.Children.Add(If_Statement());
                return n;
            }
            if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
            {
                n.Children.Add(Repeat_Statement());
                return n;
            }
            if (TokenStream[InputPointer].token_type == Token_Class.Identifier &&
                Is_validInputPointer(InputPointer + 1) &&
                TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
            {
                n.Children.Add(Function_Call());
                n.Children.Add(match(Token_Class.Semicolon));
                return n;
            }
            if (TokenStream[InputPointer].token_type == Token_Class.Comment)
            {
                n.Children.Add(match(Token_Class.Comment));
                return n;
            }
            Errors.Error_List.Add("Parsing Error: Invalid statement starting with " + TokenStream[InputPointer].token_type + "\n");
            return null;
        }

        Node Write_Statement()
        {
            Node n = new Node("Write_Statement");
            n.Children.Add(match(Token_Class.Write));
            n.Children.Add(Ex());
            n.Children.Add(match(Token_Class.Semicolon));
            return n;
        }

        Node Ex()
        {
            Node n = new Node("Ex");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.Endl)
                n.Children.Add(match(Token_Class.Endl));
            else
                n.Children.Add(Expression());
            return n;
        }

        Node Expression()
        {
            Node n = new Node("Expression");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.tiny_String)
                n.Children.Add(match(Token_Class.tiny_String));
            else if (Is_validInputPointer(InputPointer + 1) &&
                     (TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp ||
                     TokenStream[InputPointer + 1].token_type == Token_Class.MinusOp ||
                     TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp ||
                     TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp ||
                     TokenStream[InputPointer].token_type == Token_Class.LParanthesis))
            {
                n.Children.Add(Equation());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Number ||
                    TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                n.Children.Add(Term());
            }
            return n;
        }

        Node Term()
        {
            Node n = new Node("Term");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.Number)
                n.Children.Add(match(Token_Class.Number));
            else if (TokenStream[InputPointer].token_type == Token_Class.Identifier && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                n.Children.Add(Function_Call());
            else if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
                n.Children.Add(match(Token_Class.Identifier));
            EquationDash();
            return n;
        }

        Node Function_Call()
        {
            Node n = new Node("Function_Call");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                n.Children.Add(match(Token_Class.Identifier));
                n.Children.Add(match(Token_Class.LParanthesis));
                n.Children.Add(Args());
                n.Children.Add(match(Token_Class.RParanthesis));
            }
            //n.Children.Add(match(Token_Class.Semicolon));
            return n;
        }

        Node Args()
        {
            Node n = new Node("Args");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.Identifier || TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                n.Children.Add(Term());
                n.Children.Add(ArgsDash());
            }
            return n;
        }

        Node ArgsDash()
        {
            Node n = new Node("ArgsDash");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                n.Children.Add(match(Token_Class.Comma));
                n.Children.Add(Term());
                n.Children.Add(ArgsDash());
            }
            return n;
        }

        Node Equation()
        {
            Node n = new Node("Equation");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                n.Children.Add(Bracket());
                n.Children.Add(EquationDash());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.PlusOp ||
                     TokenStream[InputPointer].token_type == Token_Class.MinusOp ||
                     TokenStream[InputPointer].token_type == Token_Class.MultiplyOp ||
                     TokenStream[InputPointer].token_type == Token_Class.DivideOp ||
                     TokenStream[InputPointer].token_type == Token_Class.Number ||
                     TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                n.Children.Add(Op());
                n.Children.Add(EquationDash());
            }
            return n;
        }

        Node EquationDash()
        {
            Node n = new Node("EquationDash");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.PlusOp ||
                TokenStream[InputPointer].token_type == Token_Class.MinusOp ||
                TokenStream[InputPointer].token_type == Token_Class.MultiplyOp ||
                TokenStream[InputPointer].token_type == Token_Class.DivideOp ||
                TokenStream[InputPointer].token_type == Token_Class.Number ||
                TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                n.Children.Add(Op());
                n.Children.Add(EquationDash());
            }
            return n;
        }

        Node Op()
        {
            Node n = new Node("Op");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.Number ||
                    TokenStream[InputPointer].token_type == Token_Class.Identifier) //term
            {
                n.Children.Add(Term());
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                {
                    n.Children.Add(match(Token_Class.PlusOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                {
                    n.Children.Add(match(Token_Class.MinusOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                {
                    n.Children.Add(match(Token_Class.MultiplyOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                {
                    n.Children.Add(match(Token_Class.DivideOp));
                }
                n.Children.Add(Fac());
            }
            else
            {
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                {
                    n.Children.Add(match(Token_Class.PlusOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                {
                    n.Children.Add(match(Token_Class.MinusOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                {
                    n.Children.Add(match(Token_Class.MultiplyOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                {
                    n.Children.Add(match(Token_Class.DivideOp));
                }
                n.Children.Add(Fac());
            }

            return n;
        }

        Node Fac()
        {
            Node n = new Node("Fac");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                n.Children.Add(Bracket());
            }
            else
            {
                n.Children.Add(Term());
            }
            return n;
        }

        Node Bracket()
        {
            Node n = new Node("Bracket");
            n.Children.Add(match(Token_Class.LParanthesis));
            n.Children.Add(Equation());
            n.Children.Add(match(Token_Class.RParanthesis));
            return n;
        }

        Node Read_Statement()
        {
            Node n = new Node("Read_Statement");
            n.Children.Add(match(Token_Class.Read));
            n.Children.Add(match(Token_Class.Identifier));
            n.Children.Add(match(Token_Class.Semicolon));
            return n;
        }

        Node Assignment_Statement()
        {
            Node n = new Node("Assignment_Statement");
            n.Children.Add(match(Token_Class.Identifier));
            n.Children.Add(match(Token_Class.AssignmentOp));
            n.Children.Add(Expression());
            n.Children.Add(match(Token_Class.Semicolon));
            return n;
        }

        Node Decleration_Statement()
        {
            Node n = new Node("Decleration_Statement");
            n.Children.Add(Datatype());
            n.Children.Add(IdentList());
            n.Children.Add(match(Token_Class.Semicolon));
            return n;
        }

        Node IdentList()
        {
            Node n = new Node("IdentList");
            n.Children.Add(Ident());
            n.Children.Add(IdentListDash());
            return n;
        }

        Node IdentListDash()
        {
            Node n = new Node("IdentListDash");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                n.Children.Add(match(Token_Class.Comma));
                n.Children.Add(IdentList());
            }
            return n;
        }

        Node Ident()
        {
            Node n = new Node("Ident");
            n.Children.Add(match(Token_Class.Identifier));
            n.Children.Add(IdentDash());
            return n;
        }

        Node IdentDash()
        {
            Node n = new Node("IdentDash");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.AssignmentOp)
            {
                n.Children.Add(match(Token_Class.AssignmentOp));
                n.Children.Add(Expression());
            }
            return n;
        }

        Node Return_Statement()
        {
            Node n = new Node("Return_Statement");
            n.Children.Add(match(Token_Class.Return));
            n.Children.Add(Expression());
            n.Children.Add(match(Token_Class.Semicolon));
            return n;
        }

        Node Condition_Statement()
        {
            Node n = new Node("Condition_Statement");
            n.Children.Add(Condition());
            n.Children.Add(Condition_StatementDash());
            return n;
        }

        Node Condition_StatementDash()
        {
            Node n = new Node("Condition_StatementDash");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.AndOp)
            {
                n.Children.Add(match(Token_Class.AndOp));
                n.Children.Add(Condition());
                n.Children.Add(Condition_StatementDash());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.OrOp)
            {
                n.Children.Add(match(Token_Class.OrOp));
                n.Children.Add(Condition());
                n.Children.Add(Condition_StatementDash());
            }
            return n;
        }

        Node Condition()
        {
            Node n = new Node("Condition");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type != Token_Class.Identifier)
                return n;
            n.Children.Add(match(Token_Class.Identifier));
            if (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                n.Children.Add(match(Token_Class.GreaterThanOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                n.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                n.Children.Add(match(Token_Class.EqualOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                n.Children.Add(match(Token_Class.NotEqualOp));
            }
            n.Children.Add(Term());
            return n;
        }

        Node If_Statement()
        {
            Node n = new Node("If_Statement");
            n.Children.Add(match(Token_Class.If));
            if (TokenStream[InputPointer].token_type != Token_Class.Identifier)
                return n;
            n.Children.Add(Condition_Statement());
            n.Children.Add(match(Token_Class.Then));
            n.Children.Add(Statements());
            n.Children.Add(End_If());
            return n;
        }

        Node End_If()
        {
            Node n = new Node("End_If");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.Elseif)
            {
                n.Children.Add(Else_If_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Else)
            {
                n.Children.Add(Else_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.End)
            {
                n.Children.Add(match(Token_Class.End));
            }
            return n;
        }

        Node Else_If_Statement()
        {
            Node n = new Node("Else_If_Statement");
            n.Children.Add(match(Token_Class.Elseif));
            n.Children.Add(Condition_Statement());
            n.Children.Add(match(Token_Class.Then));
            n.Children.Add(Statements());
            n.Children.Add(End_If());
            return n;
        }

        Node Else_Statement()
        {
            Node n = new Node("Else_Statement");
            n.Children.Add(match(Token_Class.Else));
            n.Children.Add(Statements());
            n.Children.Add(match(Token_Class.End));
            return n;
        }

        Node Repeat_Statement()
        {
            Node n = new Node("Repeat_Statement");
            if (!Is_validInputPointer(InputPointer))
                return n;
            if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
            {
                n.Children.Add(match(Token_Class.Repeat));
                n.Children.Add(Statements());
                n.Children.Add(match(Token_Class.Until));
                n.Children.Add(Condition_Statement());
            }
            return n;
        }

        Node Main_Function()
        {
            Node n = new Node("Main_Function");
            if (!Is_validInputPointer(InputPointer + 1))
            {
                Errors.Error_List.Add("Parsing Error: Missing main function");
                return n;
            }
            if (TokenStream[InputPointer + 1].token_type == Token_Class.Main)
            {
                n.Children.Add(Datatype());
                n.Children.Add(match(Token_Class.Main));
                n.Children.Add(match(Token_Class.LParanthesis));
                n.Children.Add(match(Token_Class.RParanthesis));
                n.Children.Add(Function_Body());
            }
            else
                Errors.Error_List.Add("Parsing Error: Missing main function");
            return n;
        }

        bool Is_validInputPointer(int input_pointer)
        {
            return (input_pointer < TokenStream.Count);

        }
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}