using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automaton
{
    /// <summary>
    /// Class that represents a transition from one state to another state resulting from the reception of a symbol
    /// </summary>
    /// <typeparam name="T">Substitute a suitable type for a state as T</typeparam>
    public class Transition<T> : IComparable<Transition<T>> where T : IComparable<T>
    {
        public static readonly char EPSILON = '$'; // Represents the empty symbol epsilon

        // Transition moves from fromState to toState when symbol is received
        private T fromState;
        private char symbol;
        private T toState;

        /// <summary>
        /// Create a transition from a given state to itself when a given symbol is received
        /// </summary>
        /// <param name="fromOrTo">Starting and ending state for the transition</param>
        /// <param name="s">Symbol that causes the transition</param>
        public Transition(T fromOrTo, char s)
            : this(fromOrTo, s, fromOrTo)
        {
        }

        /// <summary>
        /// Create a transition from one given state to another given state when a given symbol is received
        /// </summary>
        /// <param name="from">Starting state for the transition</param>
        /// <param name="s">Symbol that causes the transition</param>
        /// <param name="to">Ending state for the transition</param>
        public Transition(T from, char s, T to)
        {
            this.fromState = from;
            this.symbol = s;
            this.toState = to;
        }

        /// <summary>
        /// Create an epsilon (i.e. empty symbol) transition from one state to another state
        /// </summary>
        /// <param name="from">Starting state for the transition</param>
        /// <param name="to">Ending state for the transition</param>
        public Transition(T from, T to)
            : this(from, EPSILON, to)
        {
        }

        public int CompareTo(Transition<T> other)
        {
            if (other == null) return 1;
            int fromStateComparison = fromState.CompareTo(other.fromState);
            int symbolComparison = symbol.CompareTo(other.symbol);
            int toStateComparison = toState.CompareTo(other.toState);
            return (fromStateComparison != 0 ? fromStateComparison :
                (symbolComparison != 0 ? symbolComparison :
                toStateComparison));
        }

        public override bool Equals(object obj)
        {
            return obj is Transition<T> transition &&
                   EqualityComparer<T>.Default.Equals(fromState, transition.fromState) &&
                   symbol == transition.symbol &&
                   EqualityComparer<T>.Default.Equals(toState, transition.toState);
        }

        public override int GetHashCode()
        {
            var hashCode = 1250983160;
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(fromState);
            hashCode = hashCode * -1521134295 + symbol.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(toState);
            return hashCode;
        }

        public override string ToString()
        {
            return "(" + this.GetFromState() + ", " + this.GetSymbol() + ") --> " + this.GetToState();
        }

        public T GetFromState()
        {
            return fromState;
        }

        public T GetToState()
        {
            return toState;
        }

        public char GetSymbol()
        {
            return symbol;
        }
    }
}
