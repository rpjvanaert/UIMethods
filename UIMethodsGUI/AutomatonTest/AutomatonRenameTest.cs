using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Automaton;

namespace AutomatonTest
{
    public class AutomatonRenameTest
    {

        [Test]
        public void RenameNull()
        {
            Automaton<string> automaton = null;
            Assert.AreEqual(null, Automaton<string>.RenameAll(automaton));
        }

        [Test]
        public void RenameEmpty()
        {
            Automaton<string> automaton = new Automaton<string>();

            Assert.AreEqual(automaton, Automaton<string>.RenameAll(automaton));
        }

        [Test]
        public void RenameString ()
        {
            //abc
            Automaton<string> dfa = new Automaton<string>(new SortedSet<char>() { 'a', 'b', 'c' });

            dfa.AddTransition(new Transition<string>("TS", 'a', "TS"));
            dfa.AddTransition(new Transition<string>("TS", 'b', "TS"));
            dfa.AddTransition(new Transition<string>("TS", 'c', "TS"));

            dfa.AddTransition(new Transition<string>("{S0}", 'a', "{S1,S2}"));
            dfa.AddTransition(new Transition<string>("{S0}", 'b', "TS"));
            dfa.AddTransition(new Transition<string>("{S0}", 'c', "TS"));

            dfa.AddTransition(new Transition<string>("{S1,S2}", 'a', "TS"));
            dfa.AddTransition(new Transition<string>("{S1,S2}", 'b', "{S3,S4}"));
            dfa.AddTransition(new Transition<string>("{S1,S2}", 'c', "TS"));

            dfa.AddTransition(new Transition<string>("{S3,S4}", 'a', "TS"));
            dfa.AddTransition(new Transition<string>("{S3,S4}", 'b', "TS"));
            dfa.AddTransition(new Transition<string>("{S3,S4}", 'c', "{S5}"));

            dfa.AddTransition(new Transition<string>("{S5}", 'a', "TS"));
            dfa.AddTransition(new Transition<string>("{S5}", 'b', "TS"));
            dfa.AddTransition(new Transition<string>("{S5}", 'c', "TS"));

            dfa.DefineAsStartState("{S0}");
            dfa.DefineAsFinalState("{S5}");

            dfa = Automaton<string>.RenameAll(dfa);

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S4", 'a', "S4")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S4", 'b', "S4")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S4", 'c', "S4")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S0", 'a', "S1")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S0", 'b', "S4")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S0", 'c', "S4")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S1", 'a', "S4")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S1", 'b', "S2")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S1", 'c', "S4")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S2", 'a', "S4")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S2", 'b', "S4")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S2", 'c', "S3")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S3", 'a', "S4")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S3", 'b', "S4")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S3", 'c', "S4")));

            Assert.AreEqual(new SortedSet<string>() { "S0" }, dfa.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "S3" }, dfa.GetFinalStates());
        }

        [Test]
        public void RenameStar()
        {
            //ab*
            Automaton<string> dfa = new Automaton<string>(new SortedSet<char>() { 'a', 'b' });

            dfa.AddTransition(new Transition<string>("TS", 'a', "TS"));
            dfa.AddTransition(new Transition<string>("TS", 'b', "TS"));

            dfa.AddTransition(new Transition<string>("{S0}", 'a', "{S1,S2,S4,S5}"));
            dfa.AddTransition(new Transition<string>("{S0}", 'b', "TS"));

            dfa.AddTransition(new Transition<string>("{S1,S2,S4,S5}", 'a', "TS"));
            dfa.AddTransition(new Transition<string>("{S1,S2,S4,S5}", 'b', "{S2,S3,S5}"));

            dfa.AddTransition(new Transition<string>("{S2,S3,S5}", 'a', "TS"));
            dfa.AddTransition(new Transition<string>("{S2,S3,S5}", 'b', "{S2,S3,S5}"));

            dfa.DefineAsStartState("{S0}");
            dfa.DefineAsFinalState("{S1,S2,S4,S5}");
            dfa.DefineAsFinalState("{S2,S3,S5}");

            dfa = Automaton<string>.RenameAll(dfa);

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S3", 'a', "S3")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S3", 'b', "S3")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S0", 'a', "S1")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S0", 'b', "S3")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S1", 'a', "S3")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S1", 'b', "S2")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S2", 'a', "S3")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S2", 'b', "S2")));

            Assert.AreEqual(new SortedSet<string>() { "S0" }, dfa.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "S1", "S2" }, dfa.GetFinalStates());
        }

        [Test]
        public void RenamePlus()
        {
            //ab*
            Automaton<string> dfa = new Automaton<string>(new SortedSet<char>() { 'a', 'b' });

            dfa.AddTransition(new Transition<string>("TS", 'a', "TS"));
            dfa.AddTransition(new Transition<string>("TS", 'b', "TS"));

            dfa.AddTransition(new Transition<string>("{S0}", 'a', "{S1,S2,S4}"));
            dfa.AddTransition(new Transition<string>("{S0}", 'b', "TS"));

            dfa.AddTransition(new Transition<string>("{S1,S2,S4}", 'a', "TS"));
            dfa.AddTransition(new Transition<string>("{S1,S2,S4}", 'b', "{S2,S3,S5}"));

            dfa.AddTransition(new Transition<string>("{S2,S3,S5}", 'a', "TS"));
            dfa.AddTransition(new Transition<string>("{S2,S3,S5}", 'b', "{S2,S3,S5}"));

            dfa.DefineAsStartState("{S0}");
            dfa.DefineAsFinalState("{S2,S3,S5}");

            dfa = Automaton<string>.RenameAll(dfa);

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S3", 'a', "S3")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S3", 'b', "S3")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S0", 'a', "S1")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S0", 'b', "S3")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S1", 'a', "S3")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S1", 'b', "S2")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S2", 'a', "S3")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S2", 'b', "S2")));

            Assert.AreEqual(new SortedSet<string>() { "S0" }, dfa.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "S2" }, dfa.GetFinalStates());
        }

        [Test]
        public void RenameOr()
        {
            //ab*
            Automaton<string> dfa = new Automaton<string>(new SortedSet<char>() { 'a', 'b' });

            dfa.AddTransition(new Transition<string>("TS", 'a', "TS"));
            dfa.AddTransition(new Transition<string>("TS", 'b', "TS"));

            dfa.AddTransition(new Transition<string>("{S4}", 'a', "{S1,S5}"));
            dfa.AddTransition(new Transition<string>("{S4}", 'b', "{S3,S5}"));

            dfa.AddTransition(new Transition<string>("{S3,S5}", 'a', "TS"));
            dfa.AddTransition(new Transition<string>("{S3,S5}", 'b', "TS"));

            dfa.AddTransition(new Transition<string>("{S1,S5}", 'a', "TS"));
            dfa.AddTransition(new Transition<string>("{S1,S5}", 'b', "TS"));

            dfa.DefineAsStartState("{S4}");
            dfa.DefineAsFinalState("{S1,S5}");
            dfa.DefineAsFinalState("{S3,S5}");

            dfa = Automaton<string>.RenameAll(dfa);

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S3", 'a', "S3")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S3", 'b', "S3")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S0", 'a', "S1")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S0", 'b', "S2")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S1", 'a', "S3")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S1", 'b', "S3")));

            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S2", 'a', "S3")));
            Assert.True(dfa.GetTransitions().Contains(new Transition<string>("S2", 'b', "S3")));

            Assert.AreEqual(new SortedSet<string>() { "S0" }, dfa.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "S1", "S2" }, dfa.GetFinalStates());
        }
    }
}
