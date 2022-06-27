using Automaton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIMethods.Core
{
    class Regex
    {

        public const char OPEN = '(';
        public const char CLOSE = ')';
        public const char STAR = '*';
        public const char PLUS = '+';
        public const char OR = '|';
        public const char CONCATENATION = '?';
        public const string _CONC = "?";

        public string String { get; set; } = string.Empty;
        public string Concatenated { get; set; } = string.Empty;
        public string Postfix { get; set; } = string.Empty;

        private StateNamer namer;

        public Regex(string regexstring)
        {
            this.String = regexstring;
            this.namer = new StateNamer('S');
        }

        public static bool IsOperator(char c)
        {
            return c == STAR || c == PLUS || c == OR || c == CONCATENATION;
        }

        public static int GetOperatorValue(char c)
        {
            switch (c)
            {
                case STAR:   return 2;
                case CONCATENATION:   return 1;
                case OR:   return 0;
                default:    return -1;
            }
        }

        public void ProcessConcatenated()
        {
            this.Concatenated = this.String;

            int ic = 0;
            while ((ic + 1) < this.Concatenated.Length)
            {
                if (Char.IsLetter(this.Concatenated[ic]) && this.Concatenated[(ic + 1)] == OPEN)
                {
                    ++ic;
                    this.Concatenated = this.Concatenated.Insert(ic, Char.ToString(CONCATENATION));
                }
                else if (Char.IsLetter(this.Concatenated[(ic + 1)]) && (this.Concatenated[ic] == CLOSE || this.Concatenated[ic] == STAR || this.Concatenated[ic] == PLUS))
                {
                    ++ic;
                    this.Concatenated = this.Concatenated.Insert(ic, Char.ToString(CONCATENATION));
                } else
                {
                    ++ic;
                }
            }
        }

        public void ShuntingYardPostfix()
        {
            this.Postfix = "";
            Stack<char> operators = new Stack<char>();

            foreach (char c in this.Concatenated)
            {
                if (Char.IsLetter(c))
                {
                    this.Postfix += c;
                } else if (operators.Count == 0)
                {
                    operators.Push(c);
                }
                else if (c == OPEN)
                {
                    operators.Push(c);
                }
                else if (c == CLOSE)
                {
                    while (operators.Count > 0)
                    {
                        if (operators.Peek() != OPEN)
                        {
                            this.Postfix += operators.Pop();
                        } else
                        {
                            operators.Pop();
                            break;
                        }
                    }
                }
                else if (GetOperatorValue(c) >= GetOperatorValue(operators.Peek()))
                {
                    operators.Push(c);
                } else
                {
                    this.Postfix += operators.Pop();
                    this.Postfix += c;
                }
            }

            while (operators.Count > 0)
            {
                char top = operators.Pop();
                this.Postfix += top;
            }
        }

    }
}
