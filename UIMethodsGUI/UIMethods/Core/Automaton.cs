﻿using Automaton;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automaton
{
    /// <summary>
    /// Class that represents an NDFA or a DFA
    /// This is a template class to allow for different state types, e.g. tuples when two (N)DFA's are combined into one
    /// </summary>
    /// <typeparam name="T">Substitute a suitable type for a state in the (N)DFA as T (such as char, string, or a tuple)</typeparam>
    class Automaton<T> where T : IComparable<T>
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

        public override string ToString()
        {
            string s = "({ ";
            // ({ ...all states... }, {...alphabet chars...}, delta, {...start state...}, {...final states...})

            foreach (T state in this.states)
            {
                s += state.ToString() + ", ";
            }
            s = s.Remove(s.Length - 2);
            s += " } , { ";

            foreach (char symbol in this.symbols)
            {
                s += symbol + ", ";
            }
            s = s.Remove(s.Length - 2);


            s += " } , delta, { ";


            foreach (T state in this.startStates)
            {
                s += state.ToString() + ", ";
            }
            s = s.Remove(s.Length - 2);
            s += " } , { ";

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

        private static string CreateUuid()
        {
            return System.Guid.NewGuid().ToString();
        }

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

        public ISet<Transition<T>> getTransitions()
        {
            return this.transitions;
        }

        public void PrintStartStates()
        {
            PrintStates(startStates);
        }

        public void PrintFinalStates()
        {
            PrintStates(finalStates);
        }

        public void PrintAllStates()
        {
            PrintStates(states);
        }

        /// <summary>
        /// Determine if the automata object represents a deterministic finite automaton (DFA)
        /// </summary>
        /// <returns>True if the automata object is deterministic (DFA)</returns>
        public bool IsDFA()
        {
            // TODO Write code that returns true if the Automaton is deterministic, false if it is non-deterministic

            // Check multiple start states
            if (this.startStates.Count != 1) return false;

            // Check each transistion if epsilon or multiple ways for same char
            foreach (Transition<T> transition in this.transitions)
            {
                if (transition.GetSymbol() == Transition<T>.EPSILON) return false;
            }

            return true;
        }

        /// <summary>
        /// Return the set of states that can be reached from a given state when a given symbol is received
        /// </summary>
        /// <param name="from">The state to start from</param>
        /// <param name="symbol">The symbol that is received</param>
        /// <returns>The set of destination states</returns>
        public ISet<T> GetToStates(T from, char symbol)
        {
            SortedSet<T> states = new SortedSet<T>();

            foreach (Transition<T> transition in this.transitions)
            {
                if (transition.GetFromState().Equals(from) && transition.GetSymbol().Equals(symbol))
                {
                    states.Add(transition.GetToState());
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
                ISet<T> states = GetToStates(currentState, seq[0]);
                if (states.Count > 0)
                {
                    if (states.Count != 1) throw new Exception("Not a DFA!");
                    currentState = states.First();
                }
                else return false;

                seq = seq.Substring(1);
            }

            foreach (T final in this.finalStates)
            {
                if (final.Equals(currentState)) return true;
            }
            return false;
        }

        /// <summary>
        /// Return automaton (NFA) from regex
        /// </summary>
        /// <param name="regex">The regex as input</param>
        /// <returns>Returns a NDFA if regex is valid</returns>
        public static Automaton<string> GenerateFromRegex(string regex)
        {
            Automaton<string> automaton = new Automaton<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray());

            string[] groups = ProcessGroups(regex);

            int stateCount = 0;

            Queue<Automaton<string>> queue = new Queue<Automaton<string>>();

            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] != null && Regex.IsMatch(groups[i], @"^[a-zA-Z0-9]+$"))
                {
                    if (groups[i + 1] == "*")
                    {
                        char toStarChar = groups[i].Substring(groups[i].Length - 1).ToCharArray()[0];
                        Automaton<string> rest = null;
                        if (groups[i].Length > 1)
                        {
                            rest = CreateNfaString(groups[i].Substring(0, groups.Length - 1), stateCount);
                            stateCount += groups[i].Length;


                            i++;
                        }

                        Automaton<string> toStarNfa = CreateNfaCharacter(toStarChar, stateCount);
                        stateCount += nodesAddedChar;
                        Automaton<string> star = CreateNfaStar(toStarNfa, (stateCount + 1));
                        stateCount += nodesAddedStar;

                        if (groups[i].Length == 0)
                        {
                            queue.Enqueue(star);

                        } 
                        else if (rest != null)
                        {
                            queue.Enqueue(CreateNfaCombine(rest, star));
                        }
                    }
                    else if (groups[i + 1] == "+")
                    {
                        char toPlusChar = groups[i].Substring(groups[i].Length - 1).ToCharArray()[0];
                        Automaton<string> plus = CreateNfaPlus(CreateNfaCharacter(toPlusChar, stateCount), (stateCount + 1));
                        stateCount += nodesAddedChar + nodesAddedPlus;

                        Automaton<string> rest = CreateNfaString(groups[i], stateCount);
                        stateCount += groups[i].Length;

                        queue.Enqueue(CreateNfaCombine(rest, plus));
                        i++;
                    }
                    else
                    {
                        queue.Enqueue(CreateNfaString(groups[i], stateCount));
                        stateCount += groups[i].Length;
                    }
                } 
                else if (groups[i] == "|")
                {
                    i++;
                    Automaton<string> left = queue.Dequeue();
                    Automaton<string> right = CreateNfaString(groups[i], stateCount);
                    stateCount += groups[i].Length;

                    queue.Enqueue(CreateNfaOr(left, right, stateCount));
                    stateCount += nodesAddedOr;
                }
            }

            automaton = CreateNfaQueue(queue);
            automaton.SetAlphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray());

            return automaton;
        }

        private static string[] ProcessGroups(string regex)
        {
            string[] groups = new string[regex.Length];
            int index = 0;
            string temp = "";
            foreach (char c in regex)
            {
                if (Char.IsLetter(c) || Char.IsDigit(c)) {
                    temp += c;
                } else
                {
                    if (temp != "") groups[index++] = temp;
                    groups[index++] = c.ToString();
                    temp = "";
                }
            }
            if (temp != "") groups[index] = temp;
            return groups;
        }

        private static readonly int nodesAddedChar = 1;
        public static Automaton<string> CreateNfaCharacter(char c, int count)
        {
            Automaton<string> automaton = new Automaton<string>();
            automaton.AddTransition(new Transition<string>("S" + count, c, "S" + (count + 1)));

            automaton.DefineAsStartState("S" + count);
            automaton.DefineAsFinalState("S" + (count + 1));

            return automaton;
        }

        public static Automaton<string> CreateNfaCharacter(char c)
        {
            Automaton<string> automaton = new Automaton<string>();

            string newUuid1 = CreateUuid();
            string newUuid2 = CreateUuid();

            automaton.AddTransition(new Transition<string>(newUuid1, c, newUuid2));

            automaton.DefineAsStartState(newUuid1);
            automaton.DefineAsFinalState(newUuid2);

            return automaton;
        }

        public static Automaton<string> CreateNfaString(string s, int count)
        {
            if (s.Length == 1) return CreateNfaCharacter(s[0], count);

            Queue<char> queue = new Queue<char>();

            foreach (char c in s)
            {
                queue.Enqueue(c);
            }

            Automaton<string> automaton = new Automaton<string>();

            automaton.DefineAsStartState("S" + count);
            automaton.AddTransition(new Transition<string>("S" + count, queue.Dequeue(), "S" + (++count)));
            

            while (queue.Count > 0)
            {
                automaton.AddTransition(new Transition<string>("S" + (count), queue.Dequeue(), "S" + (++count)));
            }
            automaton.DefineAsFinalState("S" + count);

            return automaton;
        }

        public static Automaton<string> CreateNfaString(string s)
        {
            if (s.Length == 1) return CreateNfaCharacter(s[0]);

            Queue<char> queue = new Queue<char>();

            foreach(char c in s)
            {
                queue.Enqueue(c);
            }

            Automaton<string> automaton = new Automaton<string>();

            string newUuid1 = CreateUuid();
            string newUuid2 = CreateUuid();

            automaton.DefineAsStartState(newUuid1);
            automaton.AddTransition(new Transition<string>(newUuid1, queue.Dequeue(), newUuid2));

            while (queue.Count > 0)
            {
                newUuid1 = newUuid2;
                newUuid2 = CreateUuid();
                automaton.AddTransition(new Transition<string>(newUuid1, queue.Dequeue(), newUuid2));
            }
            automaton.DefineAsFinalState(newUuid2);

            return automaton;
        }

        public static Automaton<string> CreateNfaOr(Automaton<string> left, Automaton<string> right)
        {
            Automaton<string> rAutomaton = new Automaton<string>();

            string newUuid1 = CreateUuid();
            string newUuid2 = CreateUuid();

            rAutomaton.AddTransition(left.transitions);
            rAutomaton.AddTransition(right.transitions);

            rAutomaton.AddTransition(new Transition<string>(newUuid1, left.startStates.First()));
            rAutomaton.AddTransition(new Transition<string>(newUuid1, right.startStates.First()));

            rAutomaton.AddTransition(new Transition<string>(left.finalStates.First(), newUuid2));
            rAutomaton.AddTransition(new Transition<string>(right.finalStates.First(), newUuid2));

            rAutomaton.DefineAsStartState(newUuid1);
            rAutomaton.DefineAsFinalState(newUuid2);

            return rAutomaton;
        }

        private static readonly int nodesAddedOr = 2;
        public static Automaton<string> CreateNfaOr(Automaton<string> left, Automaton<string> right, int count)
        {
            Automaton<string> automaton = new Automaton<string>();

            automaton.AddTransition(left.transitions);
            automaton.AddTransition(right.transitions);

            automaton.AddTransition(new Transition<string>("S" + count, left.startStates.First()));
            automaton.AddTransition(new Transition<string>("S" + count, right.startStates.First()));

            automaton.AddTransition(new Transition<string>(left.finalStates.First(), "S" + (count + 1)));
            automaton.AddTransition(new Transition<string>(right.finalStates.First(), "S" + (count + 1)));

            automaton.DefineAsStartState("S" + count);
            automaton.DefineAsFinalState("S" + (count + 1));

            return automaton;
        }

        public static Automaton<string> CreateNfaStar(Automaton<string> automaton)
        {
            string newUuid1 = CreateUuid();
            string newUuid2 = CreateUuid();

            automaton.AddTransition(new Transition<string>(newUuid1, automaton.startStates.First()));
            automaton.AddTransition(new Transition<string>(automaton.finalStates.First(), newUuid2));
            automaton.AddTransition(new Transition<string>(automaton.finalStates.First(), automaton.startStates.First()));
            automaton.AddTransition(new Transition<string>(newUuid1, newUuid2));
            automaton.ResetStartFinalStates();
            automaton.DefineAsStartState(newUuid1);
            automaton.DefineAsFinalState(newUuid2);

            return automaton;
        }


        private static readonly int nodesAddedStar = 2;
        public static Automaton<string> CreateNfaStar(Automaton<string> automaton, int count)
        {
            //automaton = CreateNfaPlus(automaton, count);
            //automaton.AddTransition(new Transition<string>("S" + count, "S" + (count + 1)));
            //return automaton;

            Automaton <string> returnAutomaton = new Automaton<string>();

            returnAutomaton.AddTransition(new Transition<string>("S" + count, automaton.startStates.First()));
            returnAutomaton.AddTransition(new Transition<string>(automaton.finalStates.First(), "S" + (count + 1)));
            returnAutomaton.AddTransition(new Transition<string>(automaton.finalStates.First(), automaton.startStates.First()));
            returnAutomaton.AddTransition(new Transition<string>("S" + count, "S" + (count + 1)));
            returnAutomaton.AddTransition(automaton.transitions);
            returnAutomaton.DefineAsStartState("S" + count);
            returnAutomaton.DefineAsFinalState("S" + (count + 1));

            return returnAutomaton;
        }

        public static Automaton<string> CreateNfaPlus(Automaton<string> automaton)
        {
            string newUuid1 = CreateUuid();
            string newUuid2 = CreateUuid();

            automaton.AddTransition(new Transition<string>(newUuid1, automaton.startStates.First()));
            automaton.AddTransition(new Transition<string>(automaton.finalStates.First(), newUuid2));
            automaton.AddTransition(new Transition<string>(automaton.finalStates.First(), automaton.startStates.First()));
            automaton.ResetStartFinalStates();
            automaton.DefineAsStartState(newUuid1);
            automaton.DefineAsFinalState(newUuid2);

            return automaton;
        }

        private static readonly int nodesAddedPlus = 2;
        public static Automaton<string> CreateNfaPlus(Automaton<string> automaton, int count)
        {
            automaton.AddTransition(new Transition<string>("S" + count, automaton.startStates.First()));
            automaton.AddTransition(new Transition<string>(automaton.finalStates.First(), "S" + (count + 1)));
            automaton.AddTransition(new Transition<string>(automaton.finalStates.First(), automaton.startStates.First()));
            automaton.ResetStartFinalStates();
            automaton.DefineAsStartState("S" + count);
            automaton.DefineAsFinalState("S" + (count + 1));
            return automaton;
        }

        public static Automaton<string> CreateNfaCombine(Automaton<string> left, Automaton<string> right)
        {
            Automaton<string> automaton = new Automaton<string>();

            automaton.AddTransition(left.transitions);
            automaton.AddTransition(right.transitions);
            automaton.DefineAsStartState(left.startStates.First());
            automaton.DefineAsFinalState(right.finalStates.First());
            automaton.AddTransition(new Transition<string>(left.finalStates.First(), right.startStates.First()));

            return automaton;
        }

        public static Automaton<string> CreateNfaQueue(Queue<Automaton<string>> stack)
        {
            Automaton<string> automaton = new Automaton<string>();

            if (stack.Count == 1) return stack.Dequeue();

            automaton = CreateNfaCombine(stack.Dequeue(), stack.Dequeue());

            while (stack.Count > 0)
            {
                automaton = CreateNfaCombine(automaton, stack.Dequeue());
            }


            return automaton;
        }


        private static int isp(char c)
        {
            switch (c)
            {
                case '#':
                    return 0;
                case '(':
                    return 1;
                case '*':
                    return 7;
                case '+':
                    return 7;
                case '.':
                    return 5;
                case '|':
                    return 3;
                case ')':
                    return 8;
            }
            return -1;
        }

        private static int icp(char c)
        {
            switch (c)
            {
                case '#':
                    return 0;
                case '(':
                    return 8;
                case '*':
                    return 6;
                case '+':
                    return 6;
                case '.':
                    return 4;
                case '|':
                    return 2;
                case ')':
                    return 1;
            }
            return -1;
        }


        /// <summary>
        /// Return transition for this letter
        /// </summary>
        /// <param name="lastNode"> Node to connect to </param>
        /// <returns>Returns a transition for this letter</returns>
        private static Transition<string> GetLetterTransistion(int lastNode, char character)
        {
            string from, to;
            if (lastNode == 0) from = "S";
            else from = "S" + (lastNode);
            ++lastNode;
            to = "S" + (lastNode);
            return new Transition<string>(from, character, to);
        }


        ///// <summary>
        ///// return the set of states that can be reached from a given state using an epsilon transition (i.e. no symbol received)
        ///// note: this is basically the epsilon closure of the given state
        ///// </summary>
        ///// <param name="from">the state to start from</param>
        ///// <returns>the set of destination states (not including the from state)</returns>
        //public iset<t> gettostates(t from)
        //{
        //    // follow all epsilon transitions starting in the from state
        //    sortedset<t> states = new sortedset<t>();
        //    ...  
        //    return states;
        //}

        ///// <summary>
        ///// Return true if a given sequence is accepted by the automata object (even if it is an NDFA)
        ///// Multiple start states are allowed (and used) and epsilon transitions are used as well
        ///// THe sequence is accepted if one of the possible transition paths through the automaton
        ///// ends in one of the final states
        ///// </summary>
        ///// <param name="sequence">The sequence to be accepted (or not)</param>
        ///// <returns>True if the sequence is accepted, false if it is not accepted</returns>
        //public bool Accept(string sequence)
        //{
        //    bool accept = false;
        //    // Assume multiple start states, try each one in turn
        //    ...
        //    return accept;
        //}

        ///// <summary>
        ///// Return true if a given sequence is accepted by the automaton (even if it is an NDFA)
        ///// from the given state, including any epsilon transitions that might occur
        ///// The sequence is accepted if one of the possible transitions paths through the automaton
        ///// ends in one of the final states
        ///// </summary>
        ///// <param name="from">The state to start the search from</param>
        ///// <param name="sequence">The sequence to be accepted (or not)</param>
        ///// <returns>True if the sequence is accepted, false if it is not accepted</returns>
        //private bool AcceptFromState(T from, string sequence)
        //{
        //    bool accepted = false;
        //    ...
        //    return accepted;
        //}

        //// TODO Add functionality to return the epsilon closure --> This is GetToStates(T from)
        //// TODO Add functionality to return delta epsilon for a given set of state and symbol, based on the epsilon closure --> This is GetToStates(T from, char symbol)

        //// TODO Add functionality for writing an object to a file and for reading an object from a file, using a human readable
        //// TODO Maybe add a derived class for this, or a decorator class?!

        //// TODO Add functionality for constructing an object that accepts only sequences that start with a given sequence of symbols
        //// TODO Add functionality for constructing an object that accepts only sequences that end with a given sequence of symbols
        //// TODO Add functionality for constructing an object that accepts only sequences that contain a given sequence of symbols
        //// TODO Maybe construct a factory class for this?!

        //// TODO Add functionality for combining two objects into a new object using the AND operator
        //// TODO Add functionality for combining two objects into a new object using the OR operator
        //// TODO Add functionality for creating a new object from an existing object using the NOT operator (the complement)

        //// TODO Add functionality to return a sorted set of string containing all words of given length that can be formed using the given symbols

        //// TODO Add functionality for listing all accepted sequences of symbols up to a given length, sorted by length

        //// TODO Add functionality for listing all non-accepted sequences of symbols up to a given length, sorted by length

        //// TODO Add functionality to convert an NDFA into a DFA
        //// TODO Add functionality to reverse a DFA (resulting in an NDFA)
        //// TODO Add functionality to minimise a DFA (using either of the two given algorithms)
        //// TODO Add functionality to perform an equality check on two automata
    }
}
