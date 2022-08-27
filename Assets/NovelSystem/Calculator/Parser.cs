using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelSystem.Calculator
{
    public class Parser
    {
        private const string Replaced = "#";

        // 数式を構文解析して木構造を作る
        public static Node Parse(char[] c)
        {
            Node root = new Node(); // 最上位ノード
            Node target = root; // 現在見ているノード
            string lex = "";
            for (int i = 0; i < c.Length; i++)
            {
                switch (c[i])
                {
                    case '(':
                        {
                            target.formula += Replaced;

                            // 子ノードを追加
                            Node node = new Node();
                            target.Add(node);
                            target = node;
                        }
                        break;
                    case ')':
                        target = target.parent;
                        break;
                    default:
                        // 文字列なら
                        if (c[i] >= 'a' && c[i] <= 'z')
                        {
                            lex += c[i];
                            switch (lex)
                            {
                                case "sin":
                                    lex = "";
                                    target.formula += 's';
                                    break;
                                case "cos":
                                    lex = "";
                                    target.formula += 'c';
                                    break;
                                case "tan":
                                    lex = "";
                                    target.formula += 't';
                                    break;
                                case "rand":
                                    lex = "";
                                    target.formula += 'r';
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            lex = "";
                            target.formula += c[i];
                        }
                        
                        break;
                }
            }

            // Consoleへ木構造の中身を表示
            //root.Log();
            return root;
        }
    }

    // ノード
    public class Node
    {
        // 数式
        public string formula = "";

        // 子ノード
        public List<Node> childs = new List<Node>();

        // 親ノード
        public Node parent { get; private set; }

        public void Add(Node node)
        {
            node.parent = this;
            this.childs.Add(node);
        }

        public void Log()
        {
            foreach (var child in this.childs)
            {
                child.Log();
            }
        }
    }
}