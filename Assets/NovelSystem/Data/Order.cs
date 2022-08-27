using System.Collections.Generic;

namespace NovelSystem.Data
{
    [System.Serializable]
    public class Order
    {
        // ���ߕ��p��`
        public const string ORDER_TEXT = "TEXT";
        public const string ORDER_STAND = "STAND";
        public const string ORDER_SELECT = "SELECT";
        public const string ORDER_GOTO = "GOTO";
        public const string ORDER_CALC = "CALC";
        public const string ORDER_BGM = "BGM";
        public const string ORDER_SE = "SE";
        public const string ORDER_LABEL = "LABEL";
        public const string ORDER_BG = "BG";
        public const string ORDER_SET = "SET";
        public const string ORDER_IF = "IF";

        /// <summary>
        /// ���ߕ���`
        /// </summary>
        public enum Name
        {
            Empty,
            Stand,
            Bg_Switch,
            Bgm,
            Select,
            Goto,
            Calc,
            Set,
            Text,
        };

        /// <summary>
        /// ���ߕ��p�̃f�B�N�V���i���[�A�V�K���߂͂�����ɒǉ�����
        /// </summary>
        private static Dictionary<string, Name> Map = new Dictionary<string, Name>()
        {
            { ORDER_STAND , Name.Stand },
            { ORDER_BG , Name.Bg_Switch },
            { ORDER_BGM , Name.Bgm },
            { ORDER_SELECT , Name.Select },
            { ORDER_GOTO , Name.Goto },
            { ORDER_LABEL , Name.Empty },
            { ORDER_CALC , Name.Calc},
            { ORDER_SET , Name.Set },
        };

        /// <summary>
        /// Order��Name��ԋp����
        /// </summary>
        /// <param name="orderString"></param>
        /// <returns></returns>
        public static Name GetNameByOrder(string orderString)
        {
            return Map.ContainsKey(orderString) ? Map[orderString] : Name.Text;
        }
    }
}
