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
    /// スクリプトを解析し、システム内で利用する命令形式に変換する
    /// 構文エラーがあればここでチェック
    /// </summary>
    public class ScenarioAnalyzer
    {

        // 解析した結果、行をスキップするのか判断するために利用する
        private bool skipLineFlag = false;

        // エラー情報を格納する
        private string errorInfo = "";

        // 命令
        private string resultOrder = "";

        // 立ち絵
        private StandData standInfo = new StandData();

        // BGM情報
        private BgmData bgmInfo = new BgmData();

        // 選択肢情報
        private SelectData selectInfo = new SelectData();

        // 変数情報
        private VariableData variableData = new VariableData();

        // 入力情報
        private string inputVariable = "";

        /// <summary>
        /// 処理を解析するメソッド
        /// </summary>
        /// <param name="order">命令</param>
        /// <param name="detail">詳細</param>
        /// <returns>処理結果</returns>
        public int Analyze(string order, string detail)
        {
            int result = 0;

            // 大文字変換
            if (order != "") { order = order.ToUpper(); }

            Debug.Log("Analyze "+ order);

            switch (order)
            {
                case "":
                case Order.ORDER_TEXT:
                    skipLineFlag = false;
                    break;
                case Order.ORDER_STAND:
                    result = Stand(detail);
                    skipLineFlag = true;
                    break;
                case Order.ORDER_BG:
                    result = SwitchBg(detail);
                    skipLineFlag = true;
                    break;
                case Order.ORDER_SET:
                    result = Set(detail);
                    skipLineFlag = true;
                    break;
                case Order.ORDER_IF:
                    result = If(detail);
                    skipLineFlag = true;
                    break;
                case Order.ORDER_INPUT:
                    result = Input(detail);
                    skipLineFlag = true;
                    break;
                case Order.ORDER_CALC:
                    skipLineFlag = true;
                    break;
                case Order.ORDER_SELECT:
                    result = Select(detail);
                    skipLineFlag = true;
                    break;
                case Order.ORDER_LABEL:
                    skipLineFlag = true;
                    break;
                case Order.ORDER_GOTO:
                    skipLineFlag = true;
                    break;
                case Order.ORDER_SE:
                    //result = Se(detail);
                    break;
                case Order.ORDER_BGM:
                    result = setBgmInfo("START",detail);
                    skipLineFlag = true;
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
        /// 立ち絵取得
        /// </summary>
        /// <returns></returns>
        public StandData GetStandInfo() { return standInfo; }

        /// <summary>
        /// BGM情報取得
        /// </summary>
        /// <returns></returns>
        public BgmData GetBgmInfo() { return bgmInfo; }

        /// <summary>
        ///  選択肢内容を取得する
        /// </summary>
        /// <returns></returns>
        public string GetSelectText(int selectNo) { return selectInfo.SelectText[selectNo]; }

        /// <summary>
        /// 変数情報を取得する
        /// </summary>
        /// <returns></returns>
        public VariableData GetVariable() { return variableData; }

        /// <summary>
        ///  選択肢の飛び先ラベルを返却する
        /// </summary>
        /// <returns></returns>
        public string GetSelectGoTo(int selectNo) { return selectInfo.SelectGoTo[selectNo]; }

        /// <summary>
        ///  選択肢の個数を返却する
        /// </summary>
        /// <returns></returns>
        public int GetSelectNumber() { return selectInfo.SelectNumber; }

        public string GetInputVariable() { return inputVariable; }

        /// <summary>
        /// スキップフラグ取得
        /// </summary>
        /// <returns></returns>
        public bool GetSkipLineFlag(){ return skipLineFlag; }

        /// <summary>
        /// 命令取得
        /// </summary>
        /// <returns></returns>
        public string GetOrder() { return resultOrder; }


        public string GetErrorInfo() { return errorInfo; }

        /// <summary>
        /// 立ち絵オーダーの時、詳細情報を分解する
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        private int Stand(string detail)
        {
            // TODO:もうちょいスマートに書きたい・・・
            // とりあえず、この段階では立ち絵位置以外の構文チェックはなし
            try
            {
                string[] split = detail.Split(",");
                // 分割した結果、要素数が3、4の時のみ
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
        /// 選択肢情報を読み込む
        /// </summary>
        /// <returns></returns>
        private int Select(string detail)
        {
            NovelSystemCommon nsc = new NovelSystemCommon();

            try
            {
                // 初期化
                selectInfo.SelectNumber = 0;

                List<string> selectText = new List<string>();
                List<string> selectGoto = new List<string>();

                string[] split = detail.Split(",");
                // 分割した結果、要素数が偶数の時かつ、選択肢が５つまで(いったん５つにする)
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


        private int Input(string detail)
        {
            try
            {
                inputVariable = "";
                string[] split = detail.Split(",");

                if (split.Length == 2)
                {
                    inputVariable = split[0];
                    //split[1];
                }
                else
                {
                    errorInfo = "Order:INPUT UnmatchError ElementNum =: " + split.Length;
                    return -1;
                }
            }
            catch (Exception e)
            {
                errorInfo = "Order:INPUT UnknownErorr =: " + e;
                return -1;
            }

            return 0;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        private int Calc(string detail)
        {
            NovelSystemCommon nsc = new NovelSystemCommon();

            try
            {
                // 初期化

                string[] split = detail.Split(",");
                // 分割した結果、要素数が偶数の時かつ、選択肢が５つまで(いったん５つにする)
                if (nsc.CheckEvenNumber(split.Length) && split.Length <= 10)
                {
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

        private int Set(string detail)
        {
            try
            {
                // 初期化
                variableData = new VariableData();

                string[] split = detail.Split(",");
                // 要素数２の時のみ許可する
                if (split.Length == 2)
                {
                    variableData.Name = split[0];
                    variableData.Value = split[1];
                }
                else
                {
                    errorInfo = "Order:Set UnmatchError ElementNum =: " + split.Length;
                    return -1;
                }
            }
            catch (Exception e)
            {
                errorInfo = "Order:Set UnknownErorr =: " + e;
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// IF文解析
        /// </summary>
        /// <returns></returns>
        private int If(string detail)
        {
            return 0;
        }

        private int setBgmInfo(string order , string detail)
        {
            NovelSystemCommon nsc = new NovelSystemCommon();

            try
            {
                bgmInfo.Order = order;

                string[] split = detail.Split(",");
                // 分割した結果、要素数が1、2の時のみ
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
