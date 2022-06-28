using Automaton;
using static UIMethods.Core.Regex;
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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CheckBtn_Click(object sender, RoutedEventArgs e)
        {
            this.input = InputBox.Text.Split('\n').Last(); ;
            Console.WriteLine(input);

            Match match = Regex.Match(input, regexRegex);

            string succes = "\" is not a regular expression...";

            if (match.Success) succes = "\" is a regular expression";

            this.Log("\"" + input + succes);


        }

        private void ProcessBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.Regex regex = new Core.Regex(input);

            regex.ProcessConcatenated();
            this.Log("Processed Concatenated: " + regex.Concatenated);

            regex.ShuntingYardPostfix();
            this.Log("Processed SY-Postfix: " + regex.Postfix);

            Automaton<string> automaton = regex.ConstructThompson();
            this.Log(automaton.ToString());

            foreach (Transition<string> transition in automaton.GetTransitions())
            {
                this.Log(transition.ToString());
            }
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Log(string text)
        {
            logline.Text = logline.Text + enterStr + text;
        }
    }
}
