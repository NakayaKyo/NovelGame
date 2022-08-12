using System.Collections;
using System.Collections.Generic;

namespace NovelSystem.Data
{
    [System.Serializable]
    public class SelectData
    {
        // 選択肢の個数
        public int SelectNumber;

        // 選択肢内容
        public List<string> SelectText = new List<string>();

        // 飛び先のラベル名
        public List<string> SelectGoTo = new List<string>();
    }
}