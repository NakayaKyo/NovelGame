using System.Collections;
using System.Collections.Generic;

namespace NovelSystem.Data
{
    [System.Serializable]
    public class SelectData
    {
        // �I�����̌�
        public int SelectNumber;

        // �I�������e
        public List<string> SelectText = new List<string>();

        // ��ѐ�̃��x����
        public List<string> SelectGoTo = new List<string>();
    }
}