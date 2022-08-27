using System;

namespace MainSystem.Common
{
    /// <summary>
    /// �V���O���g���N���X�B
    /// ���̃N���X���p�������ꍇ�A�R���X�g���N�^��private�ō쐬���Ă��������B
    /// </remarks>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>�V���O���g���Ƃ��Ďg�������ꍇ�͂��̃N���X���p������</remarks>
    public abstract class Singleton<T>
    {
        /// <summary>
        /// �C���X�^���X
        /// </summary>
        private static T instance;

        /// <summary>
        /// �C���X�^���X
        /// </summary>
        public static T Instance
        {
            get
            {
                if (null == instance)
                {
                    // ���t���N�V������T��private�R���X�g���N�^���Ăяo��
                    instance = (T)Activator.CreateInstance(typeof(T), true);
                }
                return instance;
            }
        }
    }

}

