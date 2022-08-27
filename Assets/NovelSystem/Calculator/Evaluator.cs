using System.Collections.Generic;

namespace NovelSystem.Calculator
{
    public class Evaluator
    {
        // �������v�Z
        public static double Evaluate(string formula)
        {
            List<char> list = new List<char>(formula);

            // �󔒑S����
            list.RemoveAll(x => x == ' ');

            char[] c = list.ToArray();

            // �\�����
            Node node = Parser.Parse(c);

            // �v�Z
            return Lexer.Analyse(node);
        }
    }
}
