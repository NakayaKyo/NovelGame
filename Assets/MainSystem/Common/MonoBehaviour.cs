using UnityEngine;

namespace MainSystem.Common
{
    /// <summary>
    /// MonoBehaviour���p�������V���O���g���N���X�B
    /// ���̃N���X���p�������ꍇ�AMonoBehaviour��Awake()�AStart()�AUpdate()�Ȃǂ͎g�p�֎~
    /// </summary>
    /// <remarks>
    /// T.Instance�ŃC���X�^���X���Ăяo�����Ƃ���GameObject�������I�ɐ��������悤�ɂȂ��Ă��܂��B(Hierarchy��ɂ͕\������Ȃ�)
    /// </remarks>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
    {
        /// <summary>
        /// �C���X�^���X�B
        /// �����o�ϐ���gameObject�͎g�p���܂���B
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
                    var go = new GameObject(typeof(T).Name);
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                    instance = go.AddComponent<T>();
                }
                return instance;
            }
        }

        /// <summary>
        /// �C���X�^���X�쐬���̏����B
        ///  T.Instance�����߂ČĂяo�����Ƃ��̃C���X�^���X�̐����̍ۂɎ��s����܂��B
        /// </summary>
        protected abstract void OnCreateInstance();

        /// <summary>
        /// Update����
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// �C���X�^���X�j������O�̏����B
        /// �R���|�[�l���g�������ɂȂ����Ƃ��Ɏ��s����܂��B
        /// </summary>
        protected abstract void BeforeDestroyInstance();

        /// <inheritdoc/>
        private void Awake()
        {
            OnCreateInstance();
        }

        /// <inheritdoc/>
        private void Update()
        {
            OnUpdate();
        }

        /// <inheritdoc/>
        private void OnDisable()
        {
            if (null != instance)
            {
                BeforeDestroyInstance();
                Destroy(instance.gameObject);
            }
        }
    }
}
