using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Automaton;

namespace AutomatonTest
{
    public class StateNamerTest
    {

        [Test]
        [TestCase('S')]
        [TestCase('q')]
        [TestCase(char.MinValue)]
        public void StateNamerTestChar(char prefix)
        {
            StateNamer statenamer = new StateNamer(prefix);
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(prefix + "" + i, statenamer.GetNew());
            }
        }
    }
}
