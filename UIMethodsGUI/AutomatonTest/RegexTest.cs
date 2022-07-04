using Automaton;
using NUnit.Framework;
using System.Collections.Generic;

namespace AutomatonTest
{
    public class RegexTest
    {

        [Test]
        public void StringPostfixTest()
        {
            string regexstring = "woord";
            Regex regex = new Regex(regexstring);

            regex.ProcessConcatenated();
            Assert.AreEqual("w?o?o?r?d", regex.Concatenated, "Concatenation unsuccesful");

            regex.ShuntingYardPostfix();
            Assert.AreEqual("wo?o?r?d?", regex.Postfix, "Shunting-Yard Postfix unsuccesful");
        }

        [Test]
        public void StringNfaTest()
        {
            Regex regex = new Regex("woord");
            regex.Postfix = "wo?o?r?d?";

            Automaton<string> automaton = regex.ConstructThompson();

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S0", 'w', "S1")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S2", 'o', "S3")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S1", "S2")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S4", 'o', "S5")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S3", "S4")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S6", 'r', "S7")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S5", "S6")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S8", 'd', "S9")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S7", "S8")));

            Assert.AreEqual(9, automaton.GetTransitions().Count);

            Assert.AreEqual(new SortedSet<string>() { "S0" }, automaton.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "S9" }, automaton.GetFinalStates());
        }

        [Test]
        public void OrPostfixTest()
        {
            string regexstring = "a|b";
            Regex regex = new Regex(regexstring);

            regex.ProcessConcatenated();
            Assert.AreEqual("a|b", regex.Concatenated, "Concatenation unsuccesful");

            regex.ShuntingYardPostfix();
            Assert.AreEqual("ab|", regex.Postfix, "Shunting-Yard Postfix unsuccesful");
        }

        [Test]
        public void OrNfaTest()
        {
            Regex regex = new Regex("a|b");
            regex.Postfix = "ab|";

            Automaton<string> automaton = regex.ConstructThompson();

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S0", 'a', "S1")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S2", 'b', "S3")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S4", "S0")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S4", "S2")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S1", "S5")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S3", "S5")));

            Assert.AreEqual(6, automaton.GetTransitions().Count);

            Assert.AreEqual(new SortedSet<string>() { "S4" }, automaton.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "S5" }, automaton.GetFinalStates());
        }

        [Test]
        public void StarPostfixTest()
        {
            string regexstring = "a*";
            Regex regex = new Regex(regexstring);

            regex.ProcessConcatenated();
            Assert.AreEqual("a*", regex.Concatenated, "Concatenation unsuccesful");

            regex.ShuntingYardPostfix();
            Assert.AreEqual("a*", regex.Postfix, "Shunting-Yard Postfix unsuccesful");
        }

        [Test]
        public void StarNfaTest()
        {
            Regex regex = new Regex("a*");
            regex.Postfix = "a*";

            Automaton<string> automaton = regex.ConstructThompson();

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S0", 'a', "S1")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S2", "S0")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S1", "S3")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S1", "S0")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S2", "S3")));

            Assert.AreEqual(5, automaton.GetTransitions().Count);

            Assert.AreEqual(new SortedSet<string>() { "S2" }, automaton.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "S3" }, automaton.GetFinalStates());
        }

        [Test]
        public void PlusPostfixTest()
        {
            string regexstring = "a+";
            Regex regex = new Regex(regexstring);

            regex.ProcessConcatenated();
            Assert.AreEqual("a+", regex.Concatenated, "Concatenation unsuccesful");

            regex.ShuntingYardPostfix();
            Assert.AreEqual("a+", regex.Postfix, "Shunting-Yard Postfix unsuccesful");
        }

        [Test]
        public void PlusNfaTest()
        {
            Regex regex = new Regex("a+");
            regex.Postfix = "a+";

            Automaton<string> automaton = regex.ConstructThompson();

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S0", 'a', "S1")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S2", "S0")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S1", "S3")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S1", "S0")));
            
            Assert.AreEqual(4, automaton.GetTransitions().Count);

            Assert.AreEqual(new SortedSet<string>() { "S2" }, automaton.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "S3" }, automaton.GetFinalStates());
        }

        [Test]
        public void CombinationPostfixTest()
        {
            string regexstring = "a|ba+b*b";
            Regex regex = new Regex(regexstring);

            regex.ProcessConcatenated();
            Assert.AreEqual("a|b?a+?b*?b", regex.Concatenated, "Concatenation unsuccesful");

            regex.ShuntingYardPostfix();
            Assert.AreEqual("aba+?b*?b?|", regex.Postfix, "Shunting-Yard Postfix unsuccesful");
        }

        [Test]
        public void CombinationNfaTest()
        {
            Regex regex = new Regex("a|ba+b*b");
            regex.Postfix = "aba+?b*?b?|";

            Automaton<string> automaton = regex.ConstructThompson();

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S0", 'a', "S1")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S2", 'b', "S3")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S4", 'a', "S5")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S6", "S4")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S5", "S7")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S5", "S4")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S3", "S6")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S8", 'b', "S9")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S10", "S8")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S9", "S11")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S9", "S8")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S10", "S11")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S7", "S10")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S12", 'b', "S13")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S11", "S12")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S14", "S0")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S14", "S2")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S1", "S15")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S13", "S15")));

            Assert.AreEqual(19, automaton.GetTransitions().Count);

            Assert.AreEqual(new SortedSet<string>() { "S14" }, automaton.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "S15" }, automaton.GetFinalStates());
        }

        [Test]
        public void ParenthesisnPostfixTest1()
        {
            string regexstring = "a(a|b)*b";
            Regex regex = new Regex(regexstring);

            regex.ProcessConcatenated();
            Assert.AreEqual("a?(a|b)*?b", regex.Concatenated, "Concatenation unsuccesful");

            regex.ShuntingYardPostfix();
            Assert.AreEqual("aab|*?b?", regex.Postfix, "Shunting-Yard Postfix unsuccesful");
        }

        [Test]
        public void ParenthesisNfaTest1()
        {
            Regex regex = new Regex("a(a|b)*b");
            regex.Postfix = "aab|*?b?";

            Automaton<string> automaton = regex.ConstructThompson();

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S0", 'a', "S1")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S2", 'a', "S3")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S4", 'b', "S5")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S6", "S2")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S6", "S4")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S3", "S7")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S5", "S7")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S8", "S6")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S7", "S9")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S7", "S6")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S8", "S6")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S1", "S8")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S10", 'b', "S11")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S9", "S10")));

            Assert.AreEqual(14, automaton.GetTransitions().Count);

            Assert.AreEqual(new SortedSet<string>() { "S0" }, automaton.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "S11" }, automaton.GetFinalStates());
        }

        [Test]
        public void ParenthesisnPostfixTest2()
        {
            string regexstring = "a+(b|c)";
            Regex regex = new Regex(regexstring);

            regex.ProcessConcatenated();
            Assert.AreEqual("a+?(b|c)", regex.Concatenated, "Concatenation unsuccesful");

            regex.ShuntingYardPostfix();
            Assert.AreEqual("a+bc|?", regex.Postfix, "Shunting-Yard Postfix unsuccesful");
        }

        [Test]
        public void ParenthesisNfaTest2()
        {
            Regex regex = new Regex("a+(b|c)");
            regex.Postfix = "a+bc|?";

            Automaton<string> automaton = regex.ConstructThompson();

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S0", 'a', "S1")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S2", "S0")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S1", "S3")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S1", "S0")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S4", 'b', "S5")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S6", 'c', "S7")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S8", "S4")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S8", "S6")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S5", "S9")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S7", "S9")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S3", "S8")));

            Assert.AreEqual(11, automaton.GetTransitions().Count);

            Assert.AreEqual(new SortedSet<string>() { "S2" }, automaton.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "S9" }, automaton.GetFinalStates());
        }

        [Test]
        public void ParenthesisnPostfixTest3()
        {
            string regexstring = "a(a|(bc|cb)+)*";
            Regex regex = new Regex(regexstring);

            regex.ProcessConcatenated();
            Assert.AreEqual("a?(a|(b?c|c?b)+)*", regex.Concatenated, "Concatenation unsuccesful");

            regex.ShuntingYardPostfix();
            Assert.AreEqual("aabc?cb?|+|*?", regex.Postfix, "Shunting-Yard Postfix unsuccesful");
        }

        [Test]
        public void ParenthesisNfaTest3()
        {
            Regex regex = new Regex("a(a|(bc|cb)+)*");
            regex.Postfix = "aabc?cb?|+|*?";

            Automaton<string> automaton = regex.ConstructThompson();

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S0", 'a', "S1")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S2", 'a', "S3")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S4", 'b', "S5")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S6", 'c', "S7")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S5", "S6")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S8", 'c', "S9")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S10", 'b', "S11")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S9", "S10")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S12", "S4")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S12", "S8")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S7", "S13")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S11", "S13")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S14", "S12")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S13", "S15")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S13", "S12")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S16", "S2")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S16", "S14")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S3", "S17")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S15", "S17")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S18", "S16")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S17", "S19")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S17", "S16")));
            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S18", "S19")));

            Assert.IsTrue(automaton.GetTransitions().Contains(new Transition<string>("S1", "S18")));

            Assert.AreEqual(24, automaton.GetTransitions().Count);

            Assert.AreEqual(new SortedSet<string>() { "S0" }, automaton.GetStartStates());
            Assert.AreEqual(new SortedSet<string>() { "S19" }, automaton.GetFinalStates());
        }
    }
}