using Automaton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace UIMethods
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string input = "";

        private static readonly string enterStr = "\n";
        private static readonly string regexRegex = @"^(?:(?:[^?+*{}()[\]\\|]+|\\.|\[(?:\^?\\.|\^[^\\]|[^\\^])(?:[^\]\\]+|\\.)*\]|\((?:\?[:=!]|\?<[=!]|\?>|\?<[^\W\d]\w*>|\?'[^\W\d]\w*')?(?<N>)|\)(?<-N>))(?:(?:[?+*]|\{\d+(?:,\d*)?\})[?+]?)?|\|)*$(?(N)(?!))";

        private Automaton<string> regexAcceptor;

        private Automaton.Regex regex = null;
        private Automaton<string> nfa = null;
        private Automaton<string> dfa = null;

        public MainWindow()
        {
            InitializeComponent();
            this.regexAcceptor = Automaton.Regex.GetRegexAcceptor();
            this.regexAcceptor.AddTransition(new Transition<string>("S0", 'a', "S1"));
        }

        private void CheckBtn_Click(object sender, RoutedEventArgs e)
        {
            this.regex = null;
            this.nfa = null;
            this.dfa = null;

            this.input = InputBox.Text.Split('\n').Last(); ;
            Console.WriteLine(input);

            Match match = System.Text.RegularExpressions.Regex.Match(input, regexRegex);

            string succes = "\" is not a regular expression...";

            if (match.Success) succes = "\" is a regular expression";

            this.Log("\"" + input + succes);


        }

        private void SYBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.input == null) { this.Log("Input is not available"); return; }

            this.regex = new Automaton.Regex(input);

            regex.ProcessConcatenated();
            this.Log("Processed Concatenated: " + this.regex.Concatenated);

            regex.ShuntingYardPostfix();
            this.Log("Processed Shunting-Yard-Postfix: " + this.regex.Postfix);
            
        }

        private void CnvrtNfaBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.regex == null) { this.Log("Processed regex is not available"); return; }

            this.nfa = regex.ConstructThompson();
            this.Log("NFA made from regex");
            this.Log(nfa.ToString());

            foreach (Transition<string> transition in this.nfa.GetTransitions())
            {
                this.Log(transition.ToString());
            }
        }

        private void CnvrtDfaBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.nfa == null) { this.Log("NFA not made yet"); return; }

            this.dfa = Conversion.ConvertToDfa(this.nfa);

            this.dfa = Automaton<string>.RenameAll(this.dfa);

            this.Log(this.dfa.ToString());

            foreach (Transition<string> transition in this.dfa.GetTransitions())
            {
                this.Log(transition.ToString());
            }
        }

        private void Visual_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.dfa == null) { this.Log("DFA not made yet"); return; }

            string testword = TestBox.Text.Split('\n').Last();

            this.Log("Tested '" + testword + "' : " + this.dfa.AcceptDFAOnly(testword));
        }

        private void Log(string text)
        {
            logline.Text = logline.Text + enterStr + text;
        }
    }
}
