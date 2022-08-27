using MainSystem.Common;

namespace MainSystem.Container
{
    /// <summary>
    /// ゲームで使うユーザー変数を管理する
    /// </summary>
    public class UserContainer : Singleton<UserContainer>
    {
        /// <summary>
        /// ユーザーデータ
        /// </summary>
        private UserData UserData { get; set; }

        /// <summary>
        /// 初期化する
        /// </summary>
        public void Initialize()
        {
            UserData = new UserData();
        }

        /// <summary>
        /// ユーザー変数を取得する
        /// </summary>
        /// <returns></returns>
        public UserData GetUserData()
        {
            return UserData;
        }
    }

}
