using System.Collections.Generic;

namespace MainSystem.Container
{
    public class UserData
    {
        /// <summary>
        /// ユーザー変数
        /// </summary>
        public Dictionary<string, string> UserVariable { get; set; }

        /// <summary>
        /// 初期化
        /// </summary>
        public UserData()
        {
            UserVariable = new Dictionary<string, string>(){};
        }
    }
}