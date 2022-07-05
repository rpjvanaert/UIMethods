using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Automaton;

namespace AutomatonTest
{
    public class AutomatonPathfindingTest
    {
        private Automaton<string> nfa = new(new SortedSet<char>() { 'a', 'b' });
        private Automaton<string> dfa = new(new SortedSet<char>() { 'a', 'b' });

        [SetUp]
        public void SetUp()
        {
            //nfa = ab+
            this.nfa.AddTransition(new Transition<string>("S0", 'a', "S1"));
            this.nfa.AddTransition(new Transition<string>("S2", 'b', "S3"));

            this.nfa.AddTransition(new Transition<string>("S4", "S2"));
            this.nfa.AddTransition(new Transition<string>("S3", "S5"));
            this.nfa.AddTransition(new Transition<string>("S3", "S2"));

            this.nfa.AddTransition(new Transition<string>("S1", "S4"));

            this.nfa.DefineAsStartState("S0");
            this.nfa.DefineAsFinalState("S5");


            //dfa = ab*
            this.dfa.AddTransition(new Transition<string>("S3", 'a', "S3"));
            this.dfa.AddTransition(new Transition<string>("S3", 'b', "S3"));

            this.dfa.AddTransition(new Transition<string>("S0", 'a', "S1"));
            this.dfa.AddTransition(new Transition<string>("S0", 'b', "S3"));

            this.dfa.AddTransition(new Transition<string>("S1", 'a', "S3"));
            this.dfa.AddTransition(new Transition<string>("S1", 'b', "S2"));

            this.dfa.AddTransition(new Transition<string>("S2", 'a', "S3"));
            this.dfa.AddTransition(new Transition<string>("S2", 'b', "S2"));

            this.dfa.DefineAsStartState("S0");
            this.dfa.DefineAsFinalState("S1");
            this.dfa.DefineAsFinalState("S2");
        }

        [Test]
        [TestCase( "S1", new string[] { "S4", "S2" } ) ]
        [TestCase( "S3", new string[] { "S5", "S2" } ) ]
        public void GetToStatesEpsilonTest(string from, string[] expected)
        {
            SortedSet<string> expectedSet = new SortedSet<string>();
            foreach (string s in expected) expectedSet.Add(s);
            Assert.AreEqual(expectedSet, this.nfa.GetToStates(from));
        }

        [Test]
        [TestCase( "S2", 'b', new string[] { "S3", "S5", "S2" } ) ]
        [TestCase( "S0", 'a', new string[] { "S1", "S4", "S2" } ) ]
        [TestCase( "S1", 'b', new string[] { "S2", "S3", "S5" } ) ]
        public void GetToStatesCharEpsilonTest(string from, char symbol, string[] expected)
        {
            SortedSet<string> expectedSet = new SortedSet<string>();
            foreach (string s in expected) expectedSet.Add(s);
            Assert.AreEqual(expectedSet, this.nfa.GetToStates(from, symbol));
        }

        [Test]
        [TestCase("S0", 'a', "S1")]
        [TestCase("S1", 'a', "S3")]
        [TestCase("S1", 'b', "S2")]
        [TestCase("S2", 'b', "S2")]
        [TestCase("S2", 'a', "S3")]

        public void GetToStatesCharTest(string from, char symbol, string expected)
        {
            Assert.AreEqual(new SortedSet<string>() { expected }, this.dfa.GetToStates(from, symbol));
        }

        [Test]
        [TestCase("ab", true)]
        [TestCase("abbbbb", true)]
        [TestCase("abbbab", false)]
        [TestCase("a", true)]
        [TestCase("babb", false)]
        public void AcceptDfaStringTest(string text, bool expected)
        {
            Assert.AreEqual(expected, this.dfa.AcceptDFAOnly(text));
        }
    }
}
