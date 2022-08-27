using MainSystem.Common;

namespace MainSystem.Container
{
    /// <summary>
    /// ユーザーマネージャー
    /// </summary>
    public sealed class UserManager : Singleton<UserManager>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private UserManager()
        {
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            UserContainer.Instance.Initialize();
        }

        /// <summary>
        /// 追加する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public int Add(string key, string value)
        {
            var user = UserContainer.Instance.GetUserData().UserVariable;
            user.Add(key, value);

            if (!user.ContainsKey(key)) { return -1; }

            return 0;
        }

        /// <summary>
        /// 更新する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newvalue"></param>
        /// <returns></returns>
        public int Update(string key, string newvalue)
        {
            var user = UserContainer.Instance.GetUserData().UserVariable;

            if (user.ContainsKey(key))
            {
                user[key] = newvalue;
            }
            else { return -1; }

            return 0;
        }

        /// <summary>
        /// 削除する（おそらく、使用しない）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int Remove(string key)
        {
            var user = UserContainer.Instance.GetUserData().UserVariable;

            if (user.ContainsKey(key))
            {
                user.Remove(key);
            }
            else { return -1; }

            return 0;
        }

        /// <summary>
        /// 取得する
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            var user = UserContainer.Instance.GetUserData().UserVariable;
            return user.ContainsKey(key) ? user[key] : "NoData";
        }

        /// <summary>
        /// 変数が存在するのかチェックする
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool isExist(string key)
        {
            var user = UserContainer.Instance.GetUserData().UserVariable;
            return user.ContainsKey(key);
        }
    }
}
