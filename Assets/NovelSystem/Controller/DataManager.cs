using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NovelSystem.Contoroller
{
    public class DataManager
    {
        /// <summary>
        /// �����񒆂̕ϐ���l�ɕϊ�����
        /// </summary>
        /// <returns></returns>
        public string ReplaceVariable(string beforeString)
        {
            string afetrString = "";
            string poolString = "";
            string variableName = "";
            bool chkVariable = false;

            for (int i = 0; i < beforeString.Length; i++)
            {
                switch (beforeString[i])
                {
                    case '\\':
                        if (!chkVariable) { 
                            poolString = poolString + beforeString[i];
                            chkVariable = true;
                        }
                        else
                        {
                            // �z��O�G���[�̂��߁A�ϊ����~
                            afetrString = afetrString + poolString;
                            poolString = "";
                            chkVariable = false;
                        }
                        break;
                    case '[':
                        if (chkVariable) { poolString = poolString + beforeString[i]; }
                        else { afetrString = afetrString + beforeString[i]; }
                        break;
                    case ']':
                        if (chkVariable) {
                            if (!(variableName == "")){ afetrString = afetrString + Get(variableName); }
                            else { afetrString = afetrString + poolString + beforeString[i]; }
                            poolString = "";
                            chkVariable = false;
                        }
                        else { afetrString = afetrString + beforeString[i]; }
                        break;
                    default:
                        if (chkVariable) { variableName = variableName + beforeString[i]; }
                        else { afetrString = afetrString + beforeString[i]; }
                        break;
                }
                
            }
            return afetrString;
        }

        /// <summary>
        /// �ϐ���ݒ肷��
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Set(string key, string value)
        {
            if (MainSystem.Container.UserManager.Instance.isExist(key))
            {
                return Update(key, value);
            } 
            else
            {
                return Add(key, value);
            }
        }

        /// <summary>
        /// �ϐ����擾����
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            return MainSystem.Container.UserManager.Instance.Get(key);
        }



        /// <summary>
        /// �V�K�ǉ�
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private int Add(string key, string value)
        {
            return MainSystem.Container.UserManager.Instance.Add(key, value);
        }

        /// <summary>
        /// �X�V
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newvalue"></param>
        /// <returns></returns>
        private int Update(string key, string newvalue)
        {
            return MainSystem.Container.UserManager.Instance.Update(key, newvalue);
        }

        




    }
}
