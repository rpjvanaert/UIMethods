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
        /// <param name="nfa">The NFA to converted to a DFA</param>
        /// <returns>Returns the DFA that was converted from the given NFA</returns>
        public static Automaton<string> ConvertToDfa(Automaton<string> nfa)
        {
            // Preparation
            ISet<char> alphabet = nfa.GetAlphabet();
            Automaton<string> dfa = new Automaton<string>();
            List<string[]> states = new List<string[]>();           // (Combination) state list
            Stack<string[]> statesstack = new Stack<string[]>();    // (Combination state stack still to iterate over

            string trapstate = "TS";

            // Each letter in alphabet should stay in trapstate if there
            foreach (char c in alphabet)
            {
                dfa.AddTransition(new Transition<string>(trapstate, c, trapstate));
            }

            // Add start state(s) to list and stack.
            // Note: since current implemenation only 1 start state is generated, this is atm done for optimisation
            states.Add(nfa.GetStartStates().ToArray());
            statesstack.Push(nfa.GetStartStates().ToArray());


            // While there are states in stack iterate over them
            while (statesstack.Count > 0)
            {

                // Pop the first (combination) state
                string[] fromStates = statesstack.Pop();

                // for each letter in the alphabet iterate over each from state
                foreach (char letter in alphabet)
                {
                    SortedSet<string> letterToStates = new SortedSet<string>();


                    foreach (string fromState in fromStates)
                    {
                        // Determine where the letter in the alphabet can go to and remember the states
                        ISet<string> addingStates = nfa.GetToStates(fromState, letter);

                        foreach (string addingState in addingStates)
                        {
                            letterToStates.Add(addingState);
                        }
                    }

                    string combinedFromStates = CombineStates(new SortedSet<string>(fromStates));

                    // If letter has states to go to process, else to trapstate
                    if (letterToStates.Count > 0)
                    {
                        
                        string combinedToStates = CombineStates(letterToStates);
                        
                        // Check if combined to state already exist, if not add to lettertostates and the state stack
                        if (!Contains(states, letterToStates.ToArray())) { states.Add(letterToStates.ToArray()); statesstack.Push(letterToStates.ToArray()); }

                        // Create transition for the letter to go towards this state
                        dfa.AddTransition(new Transition<string>(combinedFromStates, letter, combinedToStates));
                    } else
                    {
                        // No letters to go to for this letter -> to trapstate
                        dfa.AddTransition(new Transition<string>(combinedFromStates, letter, trapstate));
                    }
                }
            }

            //Set start/end states for every combination state that contains the nfa end state

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

        /// <summary>
        /// Greates a combined state name
        /// </summary>
        /// <param name="states">The set of states that will be used to make a combined state name</param>
        /// <returns>Returns curly brackeds within it the states seperated with a comma</returns>
        public static  string CombineStates (SortedSet<string> states)
        {
            string combinedState = "{";

            if (states == null || states.Count == 0) return "{}";
            foreach (string state in states)
            {
                combinedState += state + ",";
            }

            combinedState = combinedState.Remove(combinedState.Length - 1);
            combinedState += "}";

            return combinedState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool Contains(List<string[]> set, string[] item)
        {
            if (set == null || set.Count == 0) return false;
            if (item == null || item.Length == 0) return false;

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
