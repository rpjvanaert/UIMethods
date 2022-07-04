using Automaton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automaton
{
    public class Conversion
    {


        /// <summary>
        /// Converts NFA to DFA
        /// </summary>
        /// <returns>Returns a transition for this letter</returns>
        public static Automaton<string> ConvertToDfa(Automaton<string> nfa)
        {
            ISet<char> alphabet = nfa.GetAlphabet();

            Automaton<string> dfa = new Automaton<string>();

            List<string[]> states = new List<string[]>();
            Stack<string[]> statesstack = new Stack<string[]>();

            string trapstate = "TS";

            foreach (char c in alphabet)
            {
                dfa.AddTransition(new Transition<string>(trapstate, c, trapstate));
            }

            states.Add(nfa.GetStartStates().ToArray());
            statesstack.Push(nfa.GetStartStates().ToArray());

            while (statesstack.Count > 0)
            {

                string[] fromStates = statesstack.Pop();

                foreach (char letter in alphabet)
                {
                    SortedSet<string> letterToStates = new SortedSet<string>();

                    foreach (string fromState in fromStates)
                    {
                        ISet<string> addingStates = nfa.GetToStates(fromState, letter);

                        foreach (string addingState in addingStates)
                        {
                            letterToStates.Add(addingState);
                        }
                    }

                    string combinedFromStates = CombineStates(new SortedSet<string>(fromStates));

                    if (letterToStates.Count > 0)
                    {
                        string combinedToStates = CombineStates(letterToStates);
                        
                        if (!Contains(states, letterToStates.ToArray())) { states.Add(letterToStates.ToArray()); statesstack.Push(letterToStates.ToArray()); }

                        dfa.AddTransition(new Transition<string>(combinedFromStates, letter, combinedToStates));
                    } else
                    {
                        dfa.AddTransition(new Transition<string>(combinedFromStates, letter, trapstate));
                    }
                }
            }

            //Set end states

            foreach (string state in dfa.GetAllStates())
            {
                if (state.Contains(nfa.GetFinalStates().First() + ",") || state.Contains(nfa.GetFinalStates().First() + "}") || state.Contains(nfa.GetFinalStates().First()))
                {
                    dfa.DefineAsFinalState(state);
                }

                if (state.Contains(nfa.GetStartStates().First() + ",") || state.Contains(nfa.GetStartStates().First() + "}") || state.Contains(nfa.GetStartStates().First()))
                {
                    dfa.DefineAsStartState(state);
                }
            }

            return dfa;
        }

        private static  string CombineStates (SortedSet<string> states)
        {
            string combinedState = "{";
            foreach (string state in states)
            {
                combinedState += state + ",";
            }

            combinedState = combinedState.Remove(combinedState.Length - 1);
            combinedState += "}";

            return combinedState;
        }

        private static bool Contains(List<string[]> set, string[] item)
        {
            foreach (string[] eachItem in set)
            {
                if (eachItem.Length == item.Length)
                {
                    bool contains = true;
                    for (int i = 0; i < eachItem.Length; i++)
                    {
                        if (!item[i].Equals(eachItem[i])) contains = false;
                    }

                    if (contains) return true;
                }

            }

            return false;
        }
    }
}
