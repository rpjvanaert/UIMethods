using Automaton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automaton
{
    public class Regex
    {

        public const char OPEN = '(';
        public const char CLOSE = ')';
        public const char STAR = '*';
        public const char PLUS = '+';
        public const char OR = '|';
        public const char CONCATENATION = '?';
        public const string _CONC = "?";

        private static char[] alphabet = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

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
                case PLUS: return 2;
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
                if ((Char.IsLetter(this.Concatenated[ic]) || this.Concatenated[ic] == STAR || this.Concatenated[ic] == PLUS) 
                    && this.Concatenated[(ic + 1)] == OPEN)
                {
                    ++ic;
                    this.Concatenated = this.Concatenated.Insert(ic, Char.ToString(CONCATENATION));
                }
                else if (
                    Char.IsLetter(this.Concatenated[(ic + 1)]) && 
                    (
                        this.Concatenated[ic] == CLOSE ||
                        this.Concatenated[ic] == STAR ||
                        this.Concatenated[ic] == PLUS ||
                        Char.IsLetter(this.Concatenated[ic])
                    ))
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
                else if (GetOperatorValue(operators.Peek()) >= GetOperatorValue(c))
                {
                    this.Postfix += operators.Pop();

                    while (operators.Count > 0 && (GetOperatorValue(operators.Peek()) >= GetOperatorValue(c)))
                    {
                        this.Postfix += operators.Pop();
                    }
                    operators.Push(c);
                } else
                {
                    operators.Push(c);
                }
            }

            while (operators.Count > 0)
            {
                char top = operators.Pop();
                this.Postfix += top;
            }
        }

        public Automaton<string> ConstructThompson()
        {
            Stack<Automaton<string>> nfaStack = new Stack<Automaton<string>>();

            StateNamer stateNamer = new StateNamer('s');

            foreach (char c in this.Postfix)
            {

                if (Char.IsLetter(c))
                {
                    nfaStack.Push(this.ConstructSymbol(c));
                    
                } else if (c == OR)
                {
                    Automaton<string> nfa2 = nfaStack.Pop();
                    Automaton<string> nfa1 = nfaStack.Pop();
                    nfaStack.Push(this.ConstructOr(nfa1, nfa2));
                }
                else if (c == STAR)
                {
                    nfaStack.Push(this.ConstructStar(nfaStack.Pop()));
                }
                else if (c == PLUS)
                {
                    nfaStack.Push(this.ConstructPlus(nfaStack.Pop()));
                }
                else if (c == CONCATENATION)
                {
                    Automaton<string> nfa2 = nfaStack.Pop();
                    Automaton<string> nfa1 = nfaStack.Pop();
                    nfaStack.Push(this.ConstructConcatenation(nfa1, nfa2));
                }
            }

            SortedSet<char> alphabet = new SortedSet<char>();

            foreach (char each in this.String)
            {
                if (char.IsLetter(each)) alphabet.Add(each);
            }

            Automaton<string> automaton = nfaStack.Pop();
            automaton.SetAlphabet(alphabet.ToArray());
            return automaton;
        }

        private Automaton<string> ConstructSymbol(char symbol)
        {
            Automaton<string> nfa = new Automaton<string>();

            string state1 = this.namer.GetNew();
            string state2 = this.namer.GetNew();

            nfa.AddTransition(new Transition<string>(state1, symbol, state2));

            nfa.DefineAsStartState(state1);
            nfa.DefineAsFinalState(state2);

            return nfa;
        }

        private Automaton<string> ConstructOr(Automaton<string> nfa1, Automaton<string> nfa2)
        {
            Automaton<string> nfa0 = new Automaton<string>();

            string state1 = this.namer.GetNew();
            string state2 = this.namer.GetNew();

            nfa0.AddTransition(nfa1.GetTransitions());
            nfa0.AddTransition(nfa2.GetTransitions());

            nfa0.AddTransition(new Transition<string>(state1, nfa1.GetStartStates().First()));
            nfa0.AddTransition(new Transition<string>(state1, nfa2.GetStartStates().First()));

            nfa0.AddTransition(new Transition<string>(nfa1.GetFinalStates().First(), state2));
            nfa0.AddTransition(new Transition<string>(nfa2.GetFinalStates().First(), state2));

            nfa0.DefineAsStartState(state1);
            nfa0.DefineAsFinalState(state2);

            return nfa0;
        }

        private Automaton<string> ConstructStar(Automaton<string> nfa)
        {
            nfa = ConstructPlus(nfa);

            nfa.AddTransition(new Transition<string>(nfa.GetStartStates().First(), nfa.GetFinalStates().First()));

            return nfa;
        }

        private Automaton<string> ConstructPlus(Automaton<string> nfa)
        {
            string state1 = this.namer.GetNew();
            string state2 = this.namer.GetNew();

            nfa.AddTransition(new Transition<string>(state1, nfa.GetStartStates().First()));
            nfa.AddTransition(new Transition<string>(nfa.GetFinalStates().First(), state2));
            nfa.AddTransition(new Transition<string>(nfa.GetFinalStates().First(), nfa.GetStartStates().First()));

            nfa.ResetStartFinalStates();
            nfa.DefineAsStartState(state1);
            nfa.DefineAsFinalState(state2);

            return nfa;
        }

        private Automaton<string> ConstructConcatenation(Automaton<string> nfa1, Automaton<string> nfa2)
        {
            Automaton<string> nfa0 = new Automaton<string>();

            nfa0.AddTransition(nfa1.GetTransitions());
            nfa0.AddTransition(nfa2.GetTransitions());

            nfa0.AddTransition(new Transition<string>(nfa1.GetFinalStates().First(), nfa2.GetStartStates().First()));

            nfa0.DefineAsStartState(nfa1.GetStartStates().First());
            nfa0.DefineAsFinalState(nfa2.GetFinalStates().First());

            return nfa0;
        }

        public static Automaton<string> GetRegexAcceptor()
        {
            Automaton<string> automaton = new Automaton<string>(new SortedSet<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '(', ')', '*', '+', '|' });

            automaton.AddTransition(GetRegexPart(0));

            automaton.DefineAsStartState("S0");
            automaton.DefineAsFinalState("S1");
            
            for (int i = 1; i < 3; i++)
            {
                automaton.AddTransition(GetRegexPart(i));
                automaton.AddTransition(GetParenthesis(i));
            }

            automaton = Conversion.ConvertToDfa(automaton);

            return automaton;
        }

        private static ISet<Transition<string>> GetRegexPart(int n)
        {
            Automaton<string> transitions = new Automaton<string>();

            string n0 = "S" + (2 * n);
            string n1 = "S" + (2 * n + 1);

            transitions.AddTransition(GetTransitionsAlphabet(n0, n1));
            transitions.AddTransition(GetTransitionsAlphabet(n1, n1));

            transitions.AddTransition(new Transition<string>(n1, '*', n1));
            transitions.AddTransition(new Transition<string>(n1, '+', n1));
            transitions.AddTransition(new Transition<string>(n1, '|', n0));

            return transitions.GetTransitions();
        }

        private static ISet<Transition<string>> GetParenthesis(int to)
        {
            Automaton<string> transitions = new Automaton<string>();

            string n0 = "S" + (2 * (to - 1));
            string n1 = "S" + (2 * (to - 1) + 1);
            string n2 = "S" + (2 * to);
            string n3 = "S" + (2 * to + 1);

            transitions.AddTransition(new Transition<string>(n0, '(', n2));
            transitions.AddTransition(new Transition<string>(n1, '(', n2));
            transitions.AddTransition(new Transition<string>(n3, ')', n1));

            return transitions.GetTransitions();
        }

        private static SortedSet<Transition<string>> GetTransitionsAlphabet(string from, string to)
        {
            SortedSet<Transition<string>> transitions = new SortedSet<Transition<string>>();

            foreach (char c in alphabet)
            {
                transitions.Add(new Transition<string>(from, c, to));
            }

            return transitions;
        }
    }
}
