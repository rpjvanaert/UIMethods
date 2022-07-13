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
        // Constants in regex
        public const char OPEN = '(';
        public const char CLOSE = ')';
        public const char STAR = '*';
        public const char PLUS = '+';
        public const char OR = '|';
        public const char CONCATENATION = '?';
        public const string _CONC = "?";

        //letters in the alphabet
        private static char[] alphabet = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        public string String { get; set; } = string.Empty;
        public string Concatenated { get; set; } = string.Empty;
        public string Postfix { get; set; } = string.Empty;

        private StateNamer namer;

        /// <summary>
        /// Regex constructor
        /// </summary>
        /// <param name="regexstring"></param>
        public Regex(string regexstring)
        {
            this.String = regexstring;
            // Naming states with prefix 'S'
            this.namer = new StateNamer('S');
        }

        /// <summary>
        /// Checks if character is an accepted operator
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsOperator(char c)
        {
            return c == STAR || c == PLUS || c == OR || c == CONCATENATION;
        }

        /// <summary>
        /// Gets operator precedence value
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Processes given string to a concatenated string (concat: '?')
        /// </summary>
        public void ProcessConcatenated()
        {
            // Copy regex string to concatenated
            this.Concatenated = this.String;

            int ic = 0;
            // While there is a character behind the index, iterate
            while ((ic + 1) < this.Concatenated.Length)
            {
                // If
                // - character on index is a letter, star or plus
                // AND
                // - character on (index + 1) is a '('
                // then: next character and insert '?' in between
                if ((Char.IsLetter(this.Concatenated[ic]) || this.Concatenated[ic] == STAR || this.Concatenated[ic] == PLUS) 
                    && this.Concatenated[(ic + 1)] == OPEN)
                {
                    ++ic;
                    this.Concatenated = this.Concatenated.Insert(ic, Char.ToString(CONCATENATION));
                }

                // If
                // - character on (index + 1) is a letter
                // AND
                // - character on index is a ')', star, plus or letter
                // then: next character and insert '?' in between
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
                    // else next character
                    ++ic;
                }
            }
        }

        /// <summary>
        /// Uses Shunting-Yard algorithm to get a Reverse Polish Postfix (RPP)
        /// This eliminates the brackets '(' and ')'
        /// This also allows to iterate over each character from front to back using the thompson construction, see ConstructThompson()
        /// </summary>
        public void ShuntingYardPostfix()
        {
            this.Postfix = "";
            Stack<char> operators = new Stack<char>();

            // Iterate over each character of the concatenated regex string
            foreach (char c in this.Concatenated)
            {
                // In case of letter, add to postfix and continue
                if (Char.IsLetter(c))
                {
                    this.Postfix += c;
                    continue;

                } // In case of no operator on stack, push this character to it
                else if (operators.Count == 0)
                {
                    operators.Push(c);

                } // In case of open parenthesis, '(', still push to stack
                else if (c == OPEN)
                {
                    operators.Push(c);

                } // In case of closing parenthesis, ')', pop every operator from operator stack until empty or open parenthesis is encountered
                else if (c == CLOSE)
                {
                    while (operators.Count > 0)
                    {
                        // If operator on top isn't open parenthesis pop it and add to postfix
                        if (operators.Peek() != OPEN)
                        {
                            this.Postfix += operators.Pop();

                        } // else pop and break out of while loop, forget both open and closing parenthesis
                        else
                        {
                            operators.Pop();
                            break;
                        }
                    }
                }// While the operator on top has a precedence higher or equal to the current character (and operator stack isn't empty):
                // add top of operator stack to postfix, repeat until false and then push the character after it.
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

            //In case any concatenations leftovers are in operatorstack, push it to the postfix
            while (operators.Count > 0)
            {
                this.Postfix += operators.Pop();
            }
        }

        /// <summary>
        /// Use the Reverse Polish Postfix to make an NFA using the Thompson construction
        /// </summary>
        /// <returns> NFA </returns>
        public Automaton<string> ConstructThompson()
        {
            // Create nfa stac and statenamer with prefix 'S'
            Stack<Automaton<string>> nfaStack = new Stack<Automaton<string>>();
            StateNamer stateNamer = new StateNamer('S');

            // Iterate over each character in postfix
            foreach (char c in this.Postfix)
            {

                //If character is a letter, create nfa for it and push to the nfa stack
                if (Char.IsLetter(c))
                {
                    nfaStack.Push(this.ConstructSymbol(c));

                } // In case or, '|', pop two nfas and create OR over them. finally push to nfa stack
                else if (c == OR)
                {
                    Automaton<string> nfa2 = nfaStack.Pop();
                    Automaton<string> nfa1 = nfaStack.Pop();
                    nfaStack.Push(this.ConstructOr(nfa1, nfa2));

                } // In case star, '*', pop last nfa and create kleene star over it (0 or more). finally push to nfa stack
                else if (c == STAR)
                {
                    nfaStack.Push(this.ConstructStar(nfaStack.Pop()));

                } // In case plus, '+', pop last nfa and create plus over it (1 or more). finally push to nfa stack
                else if (c == PLUS)
                {
                    nfaStack.Push(this.ConstructPlus(nfaStack.Pop()));

                } // In case concat, '?', pop two nfas and create concatenation over them. finally push to nfa stack
                else if (c == CONCATENATION)
                {
                    Automaton<string> nfa2 = nfaStack.Pop();
                    Automaton<string> nfa1 = nfaStack.Pop();
                    nfaStack.Push(this.ConstructConcatenation(nfa1, nfa2));
                }
            }

            // Define alphabet
            SortedSet<char> alphabet = new SortedSet<char>();

            // Check for each character in original string if it's a letter, if so add to alphabet
            foreach (char each in this.String)
            {
                if (char.IsLetter(each)) alphabet.Add(each);
            }

            // Last standing nfa on nfa stack is the final nfa, set alphabet and return
            Automaton<string> automaton = nfaStack.Pop();
            automaton.SetAlphabet(alphabet.ToArray());
            return automaton;
        }

        /// <summary>
        /// Method to create an nfa based on a single letter
        /// </summary>
        /// <param name="letter"> letter to create nfa for </param>
        /// <returns> nfa with property of acceptin the letter </returns>
        private Automaton<string> ConstructSymbol(char letter)
        {
            Automaton<string> nfa = new Automaton<string>();

            // Get two new state names
            string state1 = this.namer.GetNew();
            string state2 = this.namer.GetNew();

            // Add the transition with the letter from the first to second state
            nfa.AddTransition(new Transition<string>(state1, letter, state2));

            nfa.DefineAsStartState(state1);
            nfa.DefineAsFinalState(state2);

            return nfa;
        }

        /// <summary>
        /// Method to create an nfa based on two nfas in an 'or' case
        /// </summary>
        /// <param name="nfa1"> nfa 1</param>
        /// <param name="nfa2"> nfa 2</param>
        /// <returns> nfa where either nfa1 or nfa2 is accepted </returns>
        private Automaton<string> ConstructOr(Automaton<string> nfa1, Automaton<string> nfa2)
        {
            Automaton<string> nfa0 = new Automaton<string>();

            // Get two new state names
            string state1 = this.namer.GetNew();
            string state2 = this.namer.GetNew();

            // Add all the transitions from nfa1 and nfa2
            nfa0.AddTransition(nfa1.GetTransitions());
            nfa0.AddTransition(nfa2.GetTransitions());

            // Create transition from state1 to the start state of nfa1 and nfa2
            nfa0.AddTransition(new Transition<string>(state1, nfa1.GetStartStates().First()));
            nfa0.AddTransition(new Transition<string>(state1, nfa2.GetStartStates().First()));

            // Create transition to state2 from the final state of nfa1 and nfa2
            nfa0.AddTransition(new Transition<string>(nfa1.GetFinalStates().First(), state2));
            nfa0.AddTransition(new Transition<string>(nfa2.GetFinalStates().First(), state2));

            nfa0.DefineAsStartState(state1);
            nfa0.DefineAsFinalState(state2);

            return nfa0;
        }

        /// <summary>
        /// Method to create an nfa based on another nfa with the kleene star over it
        /// </summary>
        /// <param name="nfa"> nfa </param>
        /// <returns> nfa where the given nfa is accepted 0 or more times </returns>
        private Automaton<string> ConstructStar(Automaton<string> nfa)
        {
            // Construct plus over nfa
            nfa = ConstructPlus(nfa);

            // Add transition to allow '0 or more' instead of '1 or more' by bypassing nfa from start to final state (epsilon)
            nfa.AddTransition(new Transition<string>(nfa.GetStartStates().First(), nfa.GetFinalStates().First()));

            return nfa;
        }

        /// <summary>
        /// Method to create an nfa based on another nfa with the plus over it
        /// </summary>
        /// <param name="nfa"> nfa </param>
        /// <returns> nfa where the given nfa is accepted 1 or more times </returns>
        private Automaton<string> ConstructPlus(Automaton<string> nfa)
        {
            // Get two new state names
            string state1 = this.namer.GetNew();
            string state2 = this.namer.GetNew();

            // Create transition from state1 to start state of nfa and from final state of nfa to state2
            nfa.AddTransition(new Transition<string>(state1, nfa.GetStartStates().First()));
            nfa.AddTransition(new Transition<string>(nfa.GetFinalStates().First(), state2));

            // Create transition from final state of nfa to start state of nfa (looping back)
            nfa.AddTransition(new Transition<string>(nfa.GetFinalStates().First(), nfa.GetStartStates().First()));

            // Reset start final states and redefine them to state1 and state2 accordingly
            nfa.ResetStartFinalStates();
            nfa.DefineAsStartState(state1);
            nfa.DefineAsFinalState(state2);

            return nfa;
        }

        /// <summary>
        /// Method to create an nfa based on two nfas in an concat (after each other)
        /// </summary>
        /// <param name="nfa1"> nfa 1</param>
        /// <param name="nfa2"> nfa 2</param>
        /// <returns> nfa where nfa1 and then nfa2 is accepted, consec</returns>
        private Automaton<string> ConstructConcatenation(Automaton<string> nfa1, Automaton<string> nfa2)
        {
            Automaton<string> nfa0 = new Automaton<string>();

            // Firstly add transitions from nfa1 and nfa2
            nfa0.AddTransition(nfa1.GetTransitions());
            nfa0.AddTransition(nfa2.GetTransitions());

            // Add transition from nfa1's final state to nfa2's start state, linking them together
            nfa0.AddTransition(new Transition<string>(nfa1.GetFinalStates().First(), nfa2.GetStartStates().First()));

            // Define nfa1 start state as start state and nfa2 final state as final state.
            nfa0.DefineAsStartState(nfa1.GetStartStates().First());
            nfa0.DefineAsFinalState(nfa2.GetFinalStates().First());

            return nfa0;
        }

        /// <summary>
        /// Method to create an dfa (from nfa) to accept a regex (simple, the ones working with this class)
        /// </summary>
        /// <returns> dfa that accepts regex </returns>
        public static Automaton<string> GetRegexAcceptor()
        {
            Automaton<string> automaton = new Automaton<string>(new SortedSet<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '(', ')', '*', '+', '|' });

            // Start with base regex
            automaton.AddTransition(GetRegexPart(0));
            automaton.DefineAsStartState("S0");
            automaton.DefineAsFinalState("S1");
            
            // Add layers on top each layer for a different level of parenthesis (up to 3 here)

            for (int i = 1; i < 3; i++)
            {
                automaton.AddTransition(GetRegexPart(i));
                automaton.AddTransition(GetParenthesis(i));
            }

            // Conversion to DFA
            automaton = Conversion.ConvertToDfa(automaton);

            return automaton;
        }

        /// <summary>
        /// Method to create nfa that accepts the base of an regex
        /// Regex including:
        /// - letters
        /// - or, '|'
        /// - star, '*'
        /// - plus, '+'
        /// </summary>
        /// <param name="n"> the n-th layer, for naming of states accordingly</param>
        /// <returns> Returns set of transitions of this nfa </returns>
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

        /// <summary>
        /// Method to create a bridge towards the next layer when an open parenthesis is used
        /// </summary>
        /// <param name="to"> To which layer (n-th) </param>
        /// <returns> Returns set of transitions that are part of the nfa </returns>
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

        /// <summary>
        /// Method to create a a set of transitions with every letter of the alphabet from 'from', to 'to'
        /// </summary>
        /// <param name="from"> From which state </param>
        /// <param name="to"> To which state </param>
        /// <returns> Returns set of transitions from 'from' to 'to' for every letter of the alphabet </returns>
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
