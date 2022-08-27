using UnityEngine;

namespace MainSystem.Common
{
    /// <summary>
    /// MonoBehaviourを継承したシングルトンクラス。
    /// このクラスを継承した場合、MonoBehaviourのAwake()、Start()、Update()などは使用禁止
    /// </summary>
    /// <remarks>
    /// T.Instanceでインスタンスを呼び出したときにGameObjectが自動的に生成されるようになっています。(Hierarchy上には表示されない)
    /// </remarks>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
    {
        /// <summary>
        /// インスタンス。
        /// メンバ変数のgameObjectは使用しません。
        /// </summary>
        private static T instance;

        /// <summary>
        /// インスタンス
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
        /// インスタンス作成時の処理。
        ///  T.Instanceを初めて呼び出したときのインスタンスの生成の際に実行されます。
        /// </summary>
        protected abstract void OnCreateInstance();

        /// <summary>
        /// Update処理
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// インスタンス破棄する前の処理。
        /// コンポーネントが無効になったときに実行されます。
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
