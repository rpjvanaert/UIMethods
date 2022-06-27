using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIMethods.Core
{
    internal class StateNamer
    {

        private int _state;
        private char _prefix;
        
        public StateNamer(char prefix)
        {
            _prefix = prefix;
            _state = -1;
        }

        public string GetNew()
        {
            return Char.ToString(_prefix) + (++_state).ToString();
        }
    }
}
