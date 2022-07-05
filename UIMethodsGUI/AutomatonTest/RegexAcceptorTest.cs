using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Automaton;

namespace AutomatonTest
{
    public class RegexAcceptorTest
    {

        private Automaton<string> regexAcceptor = Automaton.Regex.GetRegexAcceptor();

        [Test]
        [TestCase("abc", true)]
        [TestCase("a|b", true)]
        [TestCase("ab*", true)]
        [TestCase("ab+", true)]
        [TestCase("a(a|b)*b", true)]
        [TestCase("a((a+b)|(c*d)+)+e", true)]
        [TestCase("ad()", false)]
        [TestCase("(ad+", false)]
        [TestCase("a(a|(d)+", false)]
        public void RegexAcceptor(string regex, bool expected)
        {
            Assert.AreEqual(expected, this.regexAcceptor.AcceptDFAOnly(regex));
        }
    }
}
