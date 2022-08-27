using MainSystem.Common;

namespace MainSystem.Container
{
    /// <summary>
    /// �Q�[���Ŏg�����[�U�[�ϐ����Ǘ�����
    /// </summary>
    public class UserContainer : Singleton<UserContainer>
    {
        /// <summary>
        /// ���[�U�[�f�[�^
        /// </summary>
        private UserData UserData { get; set; }

        /// <summary>
        /// ����������
        /// </summary>
        public void Initialize()
        {
            UserData = new UserData();
        }

        /// <summary>
        /// ���[�U�[�ϐ����擾����
        /// </summary>
        /// <returns></returns>
        public UserData GetUserData()
        {
            return UserData;
        }
    }

}
