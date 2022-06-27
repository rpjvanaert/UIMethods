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

            logline.Text = logline.Text + enterStr + "\"" + input + succes;


        }

        private void ProcessBtn_Click(object sender, RoutedEventArgs e)
        {
            //Automaton<string> automaton = Automaton<string>.GenerateFromRegex(this.input);
            //logline.Text = logline.Text + enterStr + automaton.ToString();
            //foreach(Transition<string> transition in automaton.getTransitions())
            //{
            //    logline.Text = logline.Text + enterStr + transition.ToString();
            //}

            Core.Regex regex = new Core.Regex(input);
            regex.ProcessConcatenated();
            logline.Text = logline.Text + enterStr + regex.Concatenated;
            regex.ShuntingYardPostfix();
            logline.Text = logline.Text + enterStr + regex.Postfix;

        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
