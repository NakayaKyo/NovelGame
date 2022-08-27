using MainSystem.Common;

namespace MainSystem.Container
{
    /// <summary>
    /// ���[�U�[�}�l�[�W���[
    /// </summary>
    public sealed class UserManager : Singleton<UserManager>
    {
        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        private UserManager()
        {
        }

        /// <summary>
        /// ������
        /// </summary>
        public void Initialize()
        {
            UserContainer.Instance.Initialize();
        }

        /// <summary>
        /// �ǉ�����
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
        /// �X�V����
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
        /// �폜����i�����炭�A�g�p���Ȃ��j
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
        /// �擾����
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            var user = UserContainer.Instance.GetUserData().UserVariable;
            return user.ContainsKey(key) ? user[key] : "NoData";
        }

        /// <summary>
        /// �ϐ������݂���̂��`�F�b�N����
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
