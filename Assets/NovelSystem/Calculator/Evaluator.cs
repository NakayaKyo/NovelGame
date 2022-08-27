using System.Collections.Generic;

namespace NovelSystem.Calculator
{
    public class Evaluator
    {
        // 数式を計算
        public static double Evaluate(string formula)
        {
            List<char> list = new List<char>(formula);

            // 空白全消去
            list.RemoveAll(x => x == ' ');

            char[] c = list.ToArray();

            // 構文解析
            Node node = Parser.Parse(c);

            // 計算
            return Lexer.Analyse(node);
        }
    }
}
