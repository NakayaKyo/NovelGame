using System.Collections.Generic;

namespace MainSystem.Container
{
    public class UserData
    {
        /// <summary>
        /// ���[�U�[�ϐ�
        /// </summary>
        public Dictionary<string, string> UserVariable { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public UserData()
        {
            UserVariable = new Dictionary<string, string>(){};
        }
    }
}