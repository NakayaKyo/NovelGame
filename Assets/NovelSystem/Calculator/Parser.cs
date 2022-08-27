using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelSystem.Calculator
{
    public class Parser
    {
        private const string Replaced = "#";

        // �������\����͂��Ė؍\�������
        public static Node Parse(char[] c)
        {
            Node root = new Node(); // �ŏ�ʃm�[�h
            Node target = root; // ���݌��Ă���m�[�h
            string lex = "";
            for (int i = 0; i < c.Length; i++)
            {
                switch (c[i])
                {
                    case '(':
                        {
                            target.formula += Replaced;

                            // �q�m�[�h��ǉ�
                            Node node = new Node();
                            target.Add(node);
                            target = node;
                        }
                        break;
                    case ')':
                        target = target.parent;
                        break;
                    default:
                        // ������Ȃ�
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

            // Console�֖؍\���̒��g��\��
            //root.Log();
            return root;
        }
    }

    // �m�[�h
    public class Node
    {
        // ����
        public string formula = "";

        // �q�m�[�h
        public List<Node> childs = new List<Node>();

        // �e�m�[�h
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