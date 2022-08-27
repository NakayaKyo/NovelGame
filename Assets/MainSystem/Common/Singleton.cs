using System;

namespace MainSystem.Common
{
    /// <summary>
    /// シングルトンクラス。
    /// このクラスを継承した場合、コンストラクタをprivateで作成してください。
    /// </remarks>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>シングルトンとして使いたい場合はこのクラスを継承する</remarks>
    public abstract class Singleton<T>
    {
        /// <summary>
        /// インスタンス
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
                    // リフレクションでTのprivateコンストラクタを呼び出す
                    instance = (T)Activator.CreateInstance(typeof(T), true);
                }
                return instance;
            }
        }
    }

}

