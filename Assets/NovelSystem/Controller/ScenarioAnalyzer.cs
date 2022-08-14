using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NovelSystem.Data;
using NovelSystem.Common;


namespace NovelSystem.Contoroller
{
    /// <summary>
    /// �X�N���v�g����͂��A�V�X�e�����ŗ��p���閽�ߌ`���ɕϊ�����
    /// �\���G���[������΂����Ń`�F�b�N
    /// </summary>
    public class ScenarioAnalyzer
    {

        // ��͂������ʁA�s���X�L�b�v����̂����f���邽�߂ɗ��p����
        private bool skipLineFlag = false;

        // �G���[�����i�[����
        private string errorInfo = "";

        // ����
        private string resultOrder = "";

        // �����G
        private StandData standInfo = new StandData();

        // BGM���
        private BgmData bgmInfo = new BgmData();

        // �I�������
        private SelectData selectInfo = new SelectData();

        /// <summary>
        /// ��������͂��郁�\�b�h
        /// </summary>
        /// <param name="order">����</param>
        /// <param name="detail">�ڍ�</param>
        /// <returns>��������</returns>
        public int Analyze(string order, string detail)
        {
            int result = 0;
            switch (order)
            {
                case "":
                case "TEXT":
                    skipLineFlag = false;
                    // TODO: font�ύX�Ƃ������Ŕ��菈���H
                    break;
                case "STAND":
                    Debug.Log("Analyze STAND");
                    result = Stand(detail);
                    skipLineFlag = true;
                    break;
                case "BG_SWITCH":
                    Debug.Log("Analyze BG");
                    result = SwitchBg(detail);
                    skipLineFlag = true;
                    break;
                case "FLAG":
                    break;
                case "FLAG_IF":
                    break;
                case "SELECT":
                    Debug.Log("Analyze SELECT");
                    result = Select(detail);
                    skipLineFlag = true;
                    break;
                case "LABEL":
                    skipLineFlag = true;
                    break;
                case "GOTO":
                    skipLineFlag = true;
                    break;
                case "SE":
                    Debug.Log("Analyze SE");
                    //result = Se(detail);
                    break;
                case "BGM_START":
                    Debug.Log("Analyze BGM_START");
                    result = setBgmInfo("START",detail);
                    break;
                case "BGM_STOP":
                    Debug.Log("Analyze BGM_STOP");
                    result = setBgmInfo("STOP", detail);
                    break;
                case "BGM_RESUME":
                    Debug.Log("Analyze BGM_RESUME");
                    result = setBgmInfo("RESUME", detail);
                    break;
                case "BGM_SWITCH":
                    Debug.Log("Analyze BGM_SWITCH");
                    result = setBgmInfo("SWITCH", detail);
                    break;
                default:
                    result = -1;
                    errorInfo = "Nothing Key Word";
                    break;
            }

            resultOrder = order;
            return result;
        }

        /// <summary>
        /// �����G�擾
        /// </summary>
        /// <returns></returns>
        public StandData GetStandInfo() { return standInfo; }

        /// <summary>
        /// BGM���擾
        /// </summary>
        /// <returns></returns>
        public BgmData GetBgmInfo() { return bgmInfo; }

        /// <summary>
        ///  �I�������e���擾����
        /// </summary>
        /// <returns></returns>
        public string GetSelectText(int selectNo) { return selectInfo.SelectText[selectNo]; }

        /// <summary>
        ///  �I�����̔�ѐ惉�x����ԋp����
        /// </summary>
        /// <returns></returns>
        public string GetSelectGoTo(int selectNo) { return selectInfo.SelectGoTo[selectNo]; }

        /// <summary>
        ///  �I�����̌���ԋp����
        /// </summary>
        /// <returns></returns>
        public int GetSelectNumber() { return selectInfo.SelectNumber; }

        /// <summary>
        /// �X�L�b�v�t���O�擾
        /// </summary>
        /// <returns></returns>
        public bool GetSkipLineFlag(){ return skipLineFlag; }

        /// <summary>
        /// ���ߎ擾
        /// </summary>
        /// <returns></returns>
        public string GetOrder() { return resultOrder; }


        public string GetErrorInfo() { return errorInfo; }

        /// <summary>
        /// �����G�I�[�_�[�̎��A�ڍ׏��𕪉�����
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        private int Stand(string detail)
        {
            // TODO:�������傢�X�}�[�g�ɏ��������E�E�E
            // �Ƃ肠�����A���̒i�K�ł͗����G�ʒu�ȊO�̍\���`�F�b�N�͂Ȃ�
            try
            {
                string[] split = detail.Split(",");
                // �����������ʁA�v�f����3�A4�̎��̂�
                if (split.Length == 3 || split.Length == 4)
                {
                    if (split[0] == "RIGHT" || split[0] == "CENTER" || split[0] == "LEFT" || split[0] == "DELETE")
                    {
                        standInfo.Point = split[0];
                    }

                    standInfo.StandId = split[1];
                    standInfo.FaceId = split[2];

                    if (split.Length == 4)
                    {
                        standInfo.TransId = split[3];
                    }
                    else { standInfo.TransId = ""; }
                }
                else
                {
                    errorInfo = "Order:Stand UnmatchError ElementNum =: " + split.Length;
                    return -1;
                }
            }
            catch (Exception e)
            {
                errorInfo = "Order:Stand UnknownErorr =: " + e ;
                return -1;
            }
            return 0;
        }

        private int SwitchBg(string detail)
        {
            return 0;
        }

        /// <summary>
        /// �I��������ǂݍ���
        /// </summary>
        /// <returns></returns>
        private int Select(string detail)
        {
            NovelSystemCommon nsc = new NovelSystemCommon();

            try
            {
                // ������
                selectInfo.SelectNumber = 0;

                List<string> selectText = new List<string>();
                List<string> selectGoto = new List<string>();

                string[] split = detail.Split(",");
                // �����������ʁA�v�f���������̎����A�I�������T�܂�(��������T�ɂ���)
                if (nsc.CheckEvenNumber(split.Length) && split.Length <= 10)
                {
                    int count = 0;
                    for(int i=0;i< split.Length; i = i + 2)
                    {
                        selectText.Add(split[i]);
                        selectGoto.Add(split[i + 1]);

                        Debug.Log("AnalizeSelect[" + i + "] :Text "+ split[i]);

                        count++;
                    }

                    selectInfo.SelectNumber = count;
                    selectInfo.SelectText = selectText;
                    selectInfo.SelectGoTo = selectGoto;

                    Debug.Log("AnalizeSelectCount:" + count);
                }
                else
                {
                    Debug.Log("CEN :" + nsc.CheckEvenNumber(split.Length));
                    errorInfo = "Order:SELECT UnmatchError ElementNum =: " + split.Length;
                    return -1;
                }
            }
            catch (Exception e)
            {
                errorInfo = "Order:SELECT UnknownErorr =: " + e;
                return -1;
            }

            return 0;
        }

        private int setBgmInfo(string order , string detail)
        {
            NovelSystemCommon nsc = new NovelSystemCommon();

            try
            {
                bgmInfo.Order = order;

                string[] split = detail.Split(",");
                // �����������ʁA�v�f����1�A2�̎��̂�
                if (split.Length == 1 || split.Length == 2)
                {
                    bgmInfo.Music = split[0];

                    if (split.Length == 2)
                    {
                        bgmInfo.FeedTime = nsc.StringToFloat(split[1]);
                    }
                    else { bgmInfo.FeedTime = 0.0f; }
                }
                else
                {
                    errorInfo = "Order:Bgm UnmatchError ElementNum =: " + split.Length;
                    return -1;
                }
            }
            catch (Exception e)
            {
                errorInfo = "Order:Bgm UnknownErorr =: " + e;
                return -1;
            }
            return 0;
        }

    }
}
