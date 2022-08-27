using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelSystem.Calculator
{

    public class Lexer
    {
        // c�������ǂ����̔���
        private static bool IsNumber(char c)
        {
            return char.IsDigit(c) || c == 'x' || c == 'X' || c == '#';
        }

        // �m�[�h�̐������v�Z
        public static double Analyse(Node node)
        {
            List<string> ns; // ��
            List<char> os; // ���Z�q

            // ������
            LexicalAnalysis(node.formula, out ns, out os);

            // ns�����ɐ���������
            var numbers = new List<double>();

            int child = 0;
            for (int i = 0; i < ns.Count; i++)
            {
                double num = 0.0;

                switch (ns[i])
                {
                    case "#":
                        num = Analyse(node.childs[child++]);
                        break;
                    default:
                        double.TryParse(ns[i], out num);
                        break;
                }
                numbers.Add(num);
            }

            // �����Z�E���Z���s�Ȃ�
            for (int i = 0; i < os.Count;)
            {
                switch (os[i])
                {
                    case '*':
                        {
                            double left = numbers[i];
                            double right = numbers[i + 1];
                            numbers[i] = left * right;
                            numbers.RemoveAt(i + 1);
                            os.RemoveAt(i);
                        }
                        break;
                    case '/':
                        {
                            double left = numbers[i];
                            double right = numbers[i + 1];
                            numbers[i] = left / right;
                            numbers.RemoveAt(i + 1);
                            os.RemoveAt(i);
                        }
                        break;
                    default:
                        i++;
                        break;
                }
            }

            // �����Z�E�����Z���s�Ȃ�    
            double total = numbers[0];

            for (int i = 0; i < os.Count; i++)
            {
                switch (os[i])
                {
                    case '+':
                        total += numbers[i + 1];
                        break;
                    case '-':
                        total -= numbers[i + 1];
                        break;
                }
            }

            return total;
        }

        // ������
        private static void LexicalAnalysis(string str, out List<string> ns, out List<char> os)
        {
            ns = new List<string>();
            os = new List<char>();

            string text = "";
            for (int i = 0; i < str.Length; i++) {

                switch (str[i])
                    {
                        case '+':
                        case '-':
                        case '*':
                        case '/':
                            ns.Add(text);
                            os.Add(str[i]);
                            text = "";
                            break;
                        case 's':
                            break;
                        default:
                            if (IsNumber(str[i]))
                            {
                                text += str[i];
                                if (i == str.Length - 1)
                                {
                                    ns.Add(text);
                                    text = "";
                                }
                            }
                            break;
                    
                }
            }
        }
    }
}