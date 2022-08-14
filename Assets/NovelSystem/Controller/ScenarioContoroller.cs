using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NovelSystem.Editer;
using NovelSystem.Data;

namespace NovelSystem.Contoroller
{
    public class ScenarioContoroller
    {
        private ScenarioEditer scenarioData = ScriptableObject.CreateInstance("ScenarioEditer") as ScenarioEditer;

        private int currentLine;
        private int maxLine;

        private ScenarioAnalyzer analyzer = new ScenarioAnalyzer();

        /// <summary>
        /// Set ScriptableObjects.
        /// </summary>
        /// <returns></returns>
        public int SetScenarioData(ScenarioEditer input)
        {
            scenarioData = input;
            maxLine = scenarioData.scenarioData.Count;
            currentLine = -1;
            Debug.Log("MaxLine : " + maxLine);
            return 0;
        }

        public int ClearScenarioData()
        {
            return 0;
        }

        /// <summary>
        /// read nextline
        /// </summary>
        /// <returns></returns>
        public bool Next()
        {
            ScenarioMaster scenarioMaster = new ScenarioMaster();
            currentLine++;
            Debug.Log(" ReadLine : "+currentLine);
            if (maxLine <= currentLine)
            {
                return false;
            }

            scenarioMaster = scenarioData.scenarioData[currentLine];
            // Analyze
            int result = analyzer.Analyze(scenarioMaster.Order, scenarioMaster.OrderDetail);
            if (result == -1)
            {
                Debug.Log(analyzer.GetErrorInfo());
                return false;
            }

            return true;
        }

        public string GetText()
        {
            string text = scenarioData.scenarioData[currentLine].Text;
            return text;
        }

        public string GetName()
        {
            string name = scenarioData.scenarioData[currentLine].Name;
            return name;
        }

        public int GetCurrentLine() { return currentLine; }

        public int GetMaxLine() { return maxLine;  }

        public bool CheckOrderStand()
        {
            if (analyzer.GetOrder() == "STAND") { return true; }
            else { return false; }
        }

        public bool CheckOrderBg()
        {
            if (analyzer.GetOrder() == "BG_SWITCH") { return true; }
            else { return false; }
        }

        public bool CheckOrderBgm()
        {
            if (analyzer.GetOrder().StartsWith("Bgm")) { return true; }
            else { return false; }
        }

        public bool CheckOrderSelect()
        {
            if (analyzer.GetOrder() == "SELECT") { return true; }
            else { return false; }
        }

        public bool CheckOrderGoto()
        {
            if (analyzer.GetOrder() == "GOTO") { return true; }
            else { return false; }
        }

        public StandData GetStandInfo()
        {
            return analyzer.GetStandInfo();
        }

        // 選択肢を選択した結果
        public void SelectionSelect(int selectNo)
        {
            Debug.Log("selectNo : " + selectNo);

            string gotoLabel = analyzer.GetSelectGoTo(selectNo);

            // Goto実行
            GotoLabel(gotoLabel);

        }

        public string GetGotoLabel()
        {
            string label = scenarioData.scenarioData[currentLine].OrderDetail;
            return label;
        }

        // GOTO命令実行
        public void GotoLabel(string gotoLabel)
        {
            int line = FindLabel(gotoLabel);

            if (line == -1)
            {
                Debug.Log("Find Error: Nothing To Label");
            }
            else
            {
                currentLine = line;
            }
        }

        // 選択肢内容を取得
        public string GetSelectText(int selectNo)
        {
            return analyzer.GetSelectText(selectNo);
        }

        // 選択肢個数を取得
        public int GetSelectNumber()
        {
            return analyzer.GetSelectNumber();
        }

        // 空白イベント用
        public bool CheckOrderEmpty()
        {
            if (analyzer.GetOrder() == "LABEL") { return true; }
            else { return false; }
        }

        public BgmData GetBgmInfo()
        {
            return analyzer.GetBgmInfo();
        }

        public string GetBackLog(int line)
        {
            if (line >= maxLine) { return ""; }
            if (line < 0) { return ""; }
            return scenarioData.scenarioData[line].Text;
        }

        /// <summary>
        /// ラベル探索（現在のスクリプトのみ、同一ラベル名探査はなし）
        /// </summary>
        /// <param name="findLabel"></param>
        /// <returns></returns>
        private int FindLabel(string findLabel)
        {
            // 現在行よりも後にラベルがあるのか探す
            for(int i= currentLine + 1; i< maxLine; i++)
            {
                ScenarioMaster scenarioMaster = new ScenarioMaster();
                scenarioMaster = scenarioData.scenarioData[i];
                // ラベル
                if (scenarioMaster.Order == "LABEL")
                {
                    if(scenarioMaster.OrderDetail == findLabel){
                        Debug.Log("FindLabel:Find After");
                        return i; 
                    }
                }
            }
            
            // 現在行→最初に向けて探索する
            for(int i= currentLine - 1; i >= 0; i--)
            {
                ScenarioMaster scenarioMaster = new ScenarioMaster();
                scenarioMaster = scenarioData.scenarioData[i];
                // ラベル
                if (scenarioMaster.Order == "LABEL")
                {
                    if (scenarioMaster.OrderDetail == findLabel) {
                        Debug.Log("FindLabel:Find Before");
                        return i; 
                    }
                }
            }

            // 見つからなかった場合
            return -1;
        }
    }
}
