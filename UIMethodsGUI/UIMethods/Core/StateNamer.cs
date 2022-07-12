using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automaton
{
    public class StateNamer
    {

        private int _state;
        private char _prefix;

        /// <summary>
        /// Class that gives new state + 1 each time asked, starting at 0
        /// </summary>
        /// <param name="prefix">The prefix character before the state number</param>
        public StateNamer(char prefix)
        {
            _prefix = prefix;
            _state = -1;
        }


        /// <summary>
        /// Gives new state name back.
        /// </summary>
        public string GetNew()
        {
            // Return the prefix and state number one higher (precement)
            return Char.ToString(_prefix) + (++_state).ToString();
        }
    }
}
