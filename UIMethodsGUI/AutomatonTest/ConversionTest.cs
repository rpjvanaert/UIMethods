using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Automaton;

namespace AutomatonTest
{
    internal class ConversionTest
    {

        [Test]
        public void ConvertToDfaStringTest()
        {
            Automaton<string> nfa = new Automaton<string>(new SortedSet<char>() { 'a', 'b', 'c' });

            nfa.AddTransition(new Transition<string>("S0", 'a', "S1"));
            nfa.AddTransition(new Transition<string>("S2", 'b', "S3"));
            nfa.AddTransition(new Transition<string>("S1", "S2"));
            nfa.AddTransition(new Transition<string>("S4", 'c', "S5"));
            nfa.AddTransition(new Transition<string>("S3", "S4"));

            nfa.DefineAsStartState("S0");
            nfa.DefineAsFinalState("S5");

            Automaton<string> dfa = Conversion.ConvertToDfa(nfa);

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("TS", 'a', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("TS", 'b', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("TS", 'c', "TS")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S0}", 'a', "{S1,S2}")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S0}", 'b', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S0}", 'c', "TS")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S1,S2}", 'a', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S1,S2}", 'b', "{S3,S4}")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S1,S2}", 'c', "TS")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S3,S4}", 'a', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S3,S4}", 'b', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S3,S4}", 'c', "{S5}")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S5}", 'a', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S5}", 'b', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S5}", 'c', "TS")));

            Assert.AreEqual(new SortedSet<string>() { "{S0}" }, dfa.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "{S5}" }, dfa.GetFinalStates());
        }

        [Test]
        public void ConvertToDfaStarTest()
        {
            Automaton<string> nfa = new Automaton<string>(new SortedSet<char>() { 'a', 'b' });

            nfa.AddTransition(new Transition<string>("S0", 'a', "S1"));
            nfa.AddTransition(new Transition<string>("S2", 'b', "S3"));
            nfa.AddTransition(new Transition<string>("S4", "S2"));
            nfa.AddTransition(new Transition<string>("S3", "S5"));
            nfa.AddTransition(new Transition<string>("S3", "S2"));
            nfa.AddTransition(new Transition<string>("S4", "S5"));
            nfa.AddTransition(new Transition<string>("S1", "S4"));

            nfa.DefineAsStartState("S0");
            nfa.DefineAsFinalState("S5");

            Automaton<string> dfa = Conversion.ConvertToDfa(nfa);

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("TS", 'a', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("TS", 'b', "TS")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S0}", 'a', "{S1,S2,S4,S5}")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S0}", 'b', "TS")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S1,S2,S4,S5}", 'a', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S1,S2,S4,S5}", 'b', "{S2,S3,S5}")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S2,S3,S5}", 'a', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S2,S3,S5}", 'b', "{S2,S3,S5}")));

            Assert.AreEqual(new SortedSet<string>() { "{S0}" }, dfa.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "{S1,S2,S4,S5}", "{S2,S3,S5}" }, dfa.GetFinalStates());
        }

        [Test]
        public void ConvertToDfaPlusTest()
        {
            Automaton<string> nfa = new Automaton<string>(new SortedSet<char>() { 'a', 'b' });

            nfa.AddTransition(new Transition<string>("S0", 'a', "S1"));
            nfa.AddTransition(new Transition<string>("S2", 'b', "S3"));

            nfa.AddTransition(new Transition<string>("S4", "S2"));
            nfa.AddTransition(new Transition<string>("S3", "S5"));
            nfa.AddTransition(new Transition<string>("S3", "S2"));
            nfa.AddTransition(new Transition<string>("S1", "S4"));

            nfa.DefineAsStartState("S0");
            nfa.DefineAsFinalState("S5");

            Automaton<string> dfa = Conversion.ConvertToDfa(nfa);

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("TS", 'a', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("TS", 'b', "TS")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S0}", 'a', "{S1,S2,S4}")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S0}", 'b', "TS")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S1,S2,S4}", 'a', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S1,S2,S4}", 'b', "{S2,S3,S5}")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S2,S3,S5}", 'a', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S2,S3,S5}", 'b', "{S2,S3,S5}")));

            Assert.AreEqual(new SortedSet<string>() { "{S0}" }, dfa.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "{S2,S3,S5}" }, dfa.GetFinalStates());
        }

        [Test]
        public void ConvertToDfaOrTest()
        {
            Automaton<string> nfa = new Automaton<string>(new SortedSet<char>() { 'a', 'b' });

            nfa.AddTransition(new Transition<string>("S0", 'a', "S1"));
            nfa.AddTransition(new Transition<string>("S2", 'b', "S3"));

            nfa.AddTransition(new Transition<string>("S4", "S0"));
            nfa.AddTransition(new Transition<string>("S4", "S2"));
            nfa.AddTransition(new Transition<string>("S1", "S5"));
            nfa.AddTransition(new Transition<string>("S3", "S5"));

            nfa.DefineAsStartState("S4");
            nfa.DefineAsFinalState("S5");

            Automaton<string> dfa = Conversion.ConvertToDfa(nfa);

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("TS", 'a', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("TS", 'b', "TS")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S4}", 'a', "{S1,S5}")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S4}", 'b', "{S3,S5}")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S3,S5}", 'a', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S3,S5}", 'b', "TS")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S1,S5}", 'a', "TS")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("{S1,S5}", 'b', "TS")));

            Assert.AreEqual(new SortedSet<string>() { "{S4}" }, dfa.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "{S1,S5}", "{S3,S5}" }, dfa.GetFinalStates());
        }

        [Test]
        public void CombineStateTestNull()
        {
            SortedSet<string> states = null;

            Assert.AreEqual("{}", Conversion.CombineStates(states));
        }

        [Test]
        public void CombineStateTestEmpty()
        {
            SortedSet<string> states = new SortedSet<string>();

            Assert.AreEqual("{}", Conversion.CombineStates(states));
        }

        [Test]
        public void CombineStateTestOneState()
        {
            SortedSet<string> states = new SortedSet<string>()
            {
                "S1"
            };

            Assert.AreEqual("{S1}", Conversion.CombineStates(states));
        }

        [Test]
        public void CombineStateTestTwoStates()
        {
            SortedSet<string> states = new SortedSet<string>()
            {
                "S1",
                "S3"
            };

            Assert.AreEqual("{S1,S3}", Conversion.CombineStates(states));
        }

        [Test]
        public void CombineStateTestThree()
        {
            SortedSet<string> states = new SortedSet<string>()
            {
                "S1",
                "S3",
                "S5"
            };

            Assert.AreEqual("{S1,S3,S5}", Conversion.CombineStates(states));
        }

        [Test]
        public void ContainsTestNullItem()
        {
            List<string[]> stringArrays = new List<string[]>()
            {
                new string[] { "S1", "S2" },
                new string[] { "S3", "S2" },
                new string[] { "S4", "S3" },
                new string[] { "S2", "S1" }
            };

            Assert.False(Conversion.Contains(stringArrays, null));
        }

        [Test]
        public void ContainsTestEmptyItem()
        {
            List<string[]> stringArrays = new List<string[]>()
            {
                new string[] { "S1", "S2" },
                new string[] { "S3", "S2" },
                new string[] { "S4", "S3" },
                new string[] { "S2", "S1" }
            };

            Assert.False(Conversion.Contains(stringArrays, new string[] { }));
        }

        [Test]
        public void ContainsTestNullSet()
        {
            List<string[]> stringArrays = null;

            Assert.False(Conversion.Contains(stringArrays, new string[] { "S4", "S3" }));
        }

        [Test]
        public void ContainsTestEmptySet()
        {
            List<string[]> stringArrays = new List<string[]>(){};

            Assert.False(Conversion.Contains(stringArrays, new string[] { "S4", "S3" }));
        }

        [Test]
        public void ContainsTestOnce()
        {
            List<string[]> stringArrays = new List<string[]>()
            {
                new string[] { "S1", "S2" },
                new string[] { "S3", "S2" },
                new string[] { "S4", "S3" },
                new string[] { "S2", "S1" }
            };

            Assert.True(Conversion.Contains(stringArrays, new string[] { "S4", "S3" }));
        }

        [Test]
        public void ContainsTestTwice()
        {
            List<string[]> stringArrays = new List<string[]>()
            {
                new string[] { "S1", "S2" },
                new string[] { "S4", "S3" },
                new string[] { "S4", "S3" },
                new string[] { "S2", "S1" }
            };

            Assert.True(Conversion.Contains(stringArrays, new string[] { "S4", "S3" }));
        }

        [Test]
        public void ContainsTestNull()
        {
            List<string[]> stringArrays = new List<string[]>()
            {
                new string[] { "S1", "S2" },
                new string[] { "S4", "S2" },
                new string[] { "S4", "S5" },
                new string[] { "S2", "S1" }
            };

            Assert.False(Conversion.Contains(stringArrays, new string[] { "S4", "S3" }));
        }
    }
}
