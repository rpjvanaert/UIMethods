using Automaton;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIMethods.Core;

namespace Automaton
{
    /// <summary>
    /// Class that represents an NDFA or a DFA
    /// This is a template class to allow for different state types, e.g. tuples when two (N)DFA's are combined into one
    /// </summary>
    /// <typeparam name="T">Substitute a suitable type for a state in the (N)DFA as T (such as char, string, or a tuple)</typeparam>
    public class Automaton<T> where T : IComparable<T>
    {
        private ISet<Transition<T>> transitions;

        private SortedSet<T> states;
        private SortedSet<T> startStates;
        private SortedSet<T> finalStates;
        private SortedSet<char> symbols;

        /// <summary>
        /// Create an empty DFA (no alfabet, no states, no transitions)
        /// </summary>
        public Automaton()
            : this(new SortedSet<char>())
        {
        }

        /// <summary>
        /// Create a DFA for a given alfabet with no states or transitions
        /// </summary>
        /// <param name="s">The alfabet (i.e. the symbols that the DFA can handle)</param>
        public Automaton(char[] s)
            : this(new SortedSet<char>(s.ToList<char>()))
        {
        }

        /// <summary>
        /// Create a DFA for a given alfabet with no states or transitions
        /// </summary>
        /// <param name="symbols">The alfabet (i.e. the symbols that the DFA can handle)</param>
        public Automaton(SortedSet<char> symbols)
        {
            this.transitions = new HashSet<Transition<T>>();

            this.states = new SortedSet<T>();
            this.startStates = new SortedSet<T>();
            this.finalStates = new SortedSet<T>();
            this.symbols = symbols;
        }

        /// <summary>
        /// Returns a string of the automaton, doesn't include transitions.
        /// </summary>
        /// <returns> automaton displayed in a string</returns>
        public override string ToString()
        {
            string s = "({ ";
            // ({ ...all states... }, {...alphabet chars...}, delta, {...start state...}, {...final states...})

            // Adding states
            foreach (T state in this.states)
            {
                s += state.ToString() + ", ";
            }
            s = s.Remove(s.Length - 2);
            s += " } , { ";

            // Adding alphabet
            foreach (char symbol in this.symbols)
            {
                s += symbol + ", ";
            }
            s = s.Remove(s.Length - 2);


            s += " } , delta, { ";


            // Adding start states
            foreach (T state in this.startStates)
            {
                s += state.ToString() + ", ";
            }
            s = s.Remove(s.Length - 2);
            s += " } , { ";

            // Adding final states
            foreach (T state in this.finalStates)
            {
                s += state.ToString() + ", ";
            }
            s = s.Remove(s.Length - 2);

            s += "})";
            return s;
        }

        /// <summary>
        /// Assign an alfabet to the DFA
        /// </summary>
        /// <param name="s"></param>
        public void SetAlphabet(char[] s)
        {
            this.symbols = new SortedSet<char>(s.ToList<char>());

        }

        /// <summary>
        /// Assign an alfabet to the DFA
        /// </summary>
        /// <param name="symbols"></param>
        public void SetAlphabet(SortedSet<char> symbols)
        {
            this.symbols = new SortedSet<char>(symbols.ToList<char>());
        }

        public ISet<char> GetAlphabet()
        {
            return this.symbols;
        }

        /// <summary>
        /// Add a transition to the DFA
        /// </summary>
        /// <param name="t">The transition to be added</param>
        public void AddTransition(Transition<T> t)
        {
            if (t != null)
            {
                this.transitions.Add(t);
                if (!this.states.Contains(t.GetFromState())) this.states.Add(t.GetFromState());
                if (!this.states.Contains(t.GetToState())) this.states.Add(t.GetToState());
            }

        }

        // <summary>
        /// Add a transition to the DFA
        /// </summary>
        /// <param name="t">The transitions to be added</param>
        public void AddTransition(ISet<Transition<T>> t)
        {
           foreach (Transition<T> s in t)
            {
                this.AddTransition(s);
            }
        }

        /// <summary>
        /// Sets new SortedSets for start- and finalstates
        /// </summary>
        public void ResetStartFinalStates()
        {
            this.startStates = new SortedSet<T>();
            this.finalStates = new SortedSet<T>();
        }

        /// <summary>
        /// Set a given state to be the start state
        /// Note that multiple states can be set as a start state in an NDFA (not in a DFA though)
        /// </summary>
        /// <param name="t">The state that is to be a start state</param>
        public void DefineAsStartState(T t)
        {
            this.startStates.Add(t);

        }

        /// <summary>
        /// Set a given state to be one of the end states
        /// </summary>
        /// <param name="t">The state that is to be an end state</param>
        public void DefineAsFinalState(T t)
        {
            this.finalStates.Add(t);

        }

        private void PrintStates(ISet<T> states)
        {
            Console.Write("{");
            foreach (T state in states)
            {
                Console.Write($"{state} ");
            }
            Console.Write("}");
        }

        public void PrintAutomaton()
        {
            Console.WriteLine(this.ToString());

        }
        public void PrintAlphabet()
        {
            Console.Write("{");
            foreach (char c in symbols)
            {
                Console.Write($"{c} ");
            }
            Console.Write("}");
        }
        public void PrintTransitions()
        {
            foreach (Transition<T> t in transitions)
            {
                Console.WriteLine(t);
            }
        }
        public ISet<Transition<T>> GetTransitions()
        {
            return this.transitions;
        }

        public void PrintStartStates()
        {
            PrintStates(startStates);
        }

        public SortedSet<T> GetStartStates()
        {
            return this.startStates;
        }

        public void PrintFinalStates()
        {
            PrintStates(finalStates);
        }

        public SortedSet<T> GetFinalStates()
        {
            return this.finalStates;
        }

        public void PrintAllStates()
        {
            PrintStates(states);
        }

        public SortedSet<T> GetAllStates()
        {
            return states;
        }

        /// <summary>
        /// Determine if the automata object represents a deterministic finite automaton (DFA)
        /// </summary>
        /// <returns>True if the automata object is deterministic (DFA)</returns>
        public bool IsDFA()
        {
            // Check multiple start states
            if (this.startStates.Count != 1) return false;

            // Check each transistion if epsilon or multiple ways for same char
            foreach (Transition<T> transition in this.transitions)
            {
                if (transition.GetSymbol() == Transition<T>.EPSILON) return false;
            }

            // TODO(opt) check if states have multiple ways to go with same symbol

            return true;
        }

        /// <summary>
        /// Return the set of states that can be reached from a given state when a given symbol is received
        /// Epsilon closure path discovery included
        /// </summary>
        /// <param name="from">The state to start from</param>
        /// <param name="symbol">The symbol that is received</param>
        /// <returns>The set of destination states</returns>
        public ISet<T> GetToStates(T from, char symbol)
        {
            SortedSet<T> states = new SortedSet<T>();
            ISet<T> epsilonStates;

            // Check each transition (first char then epsilon)
            foreach (Transition<T> transition in this.transitions)
            {
                // If transition is the 'from' state and goes with symbol
                if (transition.GetFromState().Equals(from) && transition.GetSymbol().Equals(symbol))
                {
                    // Add end state of transition to states
                    states.Add(transition.GetToState());

                    // Add all epsilon possible states
                    epsilonStates = this.GetToStates(transition.GetToState());
                    foreach(T t in epsilonStates)
                    {
                        states.Add(t);
                    }
                }
            }

            // Check each transition (first epsilon then char)
            epsilonStates = this.GetToStates(from);

            // For each epsilon state, check each transition it has
            foreach(T state in epsilonStates)
            {
                foreach(Transition<T> transition in this.transitions)
                {

                    // If that transition is from the ending epsilon closure state and has the symbol
                    if (transition.GetFromState().Equals(state) && transition.GetSymbol().Equals(symbol))
                    {
                        // Add end state of transition to states
                        states.Add(transition.GetToState());

                        // Add all epsilon possible states
                        epsilonStates = this.GetToStates(transition.GetToState());
                        foreach (T t in epsilonStates)
                        {
                            states.Add(t);
                        }
                    }
                }
            }

            return states;
        }

        /// <summary>
        /// Return true if a given sequence is accepted by the automata object provided that the automata is a DFA
        /// The sequence is accepted if it puts the automata object in one of its final states
        /// </summary>
        /// <param name="sequence">The sequence to be accepted (or not)</param>
        /// <returns>True if the sequence is accepted, false if it is not accepted</returns>
        public bool AcceptDFAOnly(string sequence)
        {
            // This implementation is only for DFAs, throws an exception as soon as
            // multiple to states are encountered
            T currentState = this.startStates.First(); // Assume DFA, so only one start state
            Console.WriteLine($"Accept sequence {sequence}, start at state {currentState}");
            string seq = sequence;

            while (seq.Length > 0)
            {
                // Get states possible to go to
                ISet<T> states = GetToStates(currentState, seq[0]);

                // Check if there is a state
                if (states.Count > 0)
                {
                    // If multiple states, throw exception: not a DFA, multiple options for one symbol
                    if (states.Count != 1) throw new Exception("Not a DFA, multiple ways to go to");
                    currentState = states.First();
                }
                else
                {
                    throw new Exception("Not a DFA, nowhere to go to");
                }

                // Moved on to next state, remove first character
                seq = seq.Substring(1);
            }

            // Return if the stranded state is a final state or not
            return this.finalStates.Contains(currentState);
        }

        /// <summary>
        /// return the set of states that can be reached from a given state using an epsilon transition (i.e. no symbol received)
        /// note: this is basically the epsilon closure of the given state
        /// </summary>
        /// <param name="from"> the state to start from </param>
        /// <returns> The set of destination states (not including the from state) </returns>
        public ISet<T> GetToStates(T from)
        {
            // follow all epsilon transitions starting in the from state
            SortedSet<T> states = new SortedSet<T>();
            
            // Check each transition
            foreach (Transition<T> transition in this.transitions)
            {
                // If transition has the from state and symbol is epsilon add to states
                if (transition.GetFromState().Equals(from) && transition.GetSymbol().Equals(Transition<T>.EPSILON))
                {
                    states.Add(transition.GetToState());

                    // Check epsilon possibilities for everything and add them too (recursion)
                    ISet<T> otherStates = this.GetToStates(transition.GetToState());

                    foreach (T otherState in otherStates)
                    {
                        states.Add(otherState);
                    }
                }
            }

            return states;
        }

        /// <summary>
        /// renames all states counting from begin state
        /// </summary>
        /// <param name="automaton"> automaton to rename the states of </param>
        /// <returns> new automaton with new state names </returns>
        public static Automaton<string> RenameAll(Automaton<string> automaton, char prefix)
        {
            if (automaton == null) return null; // return null, because not created here
            if (automaton.transitions.Count == 0 || automaton.startStates.Count == 0 || automaton.finalStates.Count == 0) return automaton;

            // Create new automaton and set alphabet over from original
            Automaton<string> renamed = new Automaton<string>();
            renamed.SetAlphabet(automaton.GetAlphabet().ToArray());

            // Create the statenamer
            StateNamer stateNamer = new StateNamer(prefix);

            // Create the dictionary and add the start name with the first state name
            Dictionary<string, string> dictionaryRenames = new Dictionary<string, string>();
            dictionaryRenames.Add(automaton.GetStartStates().First(), stateNamer.GetNew());

            // Go through each state and if not already renamed, then create new name and add to dictionary
            foreach (string state in automaton.GetAllStates())
            {
                if (!dictionaryRenames.ContainsKey(state)) dictionaryRenames.Add(state, stateNamer.GetNew());
            }

            // Go through each transition and rename the states
            foreach (Transition<string> transition in automaton.GetTransitions())
            {
                renamed.AddTransition(new Transition<string>(dictionaryRenames[transition.GetFromState()], transition.GetSymbol(), dictionaryRenames[transition.GetToState()]));
            }

            // Define the start state new name
            renamed.DefineAsStartState(dictionaryRenames[automaton.GetStartStates().First()]);

            // Define the final states new names
            foreach (string finalState in automaton.GetFinalStates())
            {
                renamed.DefineAsFinalState(dictionaryRenames[finalState]);
            }

            return renamed;
        }
    }
}
