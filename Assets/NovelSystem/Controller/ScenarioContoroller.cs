using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NovelSystem.Editer;
using NovelSystem.Data;
using NovelSystem.Calculator;
using System.Linq;


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
            DataManager dataManager = new DataManager();
            text = dataManager.ReplaceVariable(text);
            return text;
        }

        public string GetName()
        {
            string name = scenarioData.scenarioData[currentLine].Name;
            DataManager dataManager = new DataManager();
            name = dataManager.ReplaceVariable(name);
            return name;
        }

        public int GetCurrentLine() { return currentLine; }

        public int GetMaxLine() { return maxLine;  }

        public Order.Name GetOrderUpper(){
            return Order.GetNameByOrder(analyzer.GetOrder().ToUpper());  
        }

        public StandData GetStandInfo()
        {
            return analyzer.GetStandInfo();
        }

        //
        public void SetVariable()
        {
            DataManager dataManager = new DataManager();
            var variable = analyzer.GetVariable();

            dataManager.Set(variable.Name, variable.Value);
            
        } 

        // �I������I����������
        public void SelectionSelect(int selectNo)
        {
            Debug.Log("selectNo : " + selectNo);

            string gotoLabel = analyzer.GetSelectGoTo(selectNo);

            // Goto���s
            GotoLabel(gotoLabel);

        }

        public string GetGotoLabel()
        {
            string label = scenarioData.scenarioData[currentLine].OrderDetail;
            return label;
        }

        // GOTO���ߎ��s
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

        // 
        public string GetCalcDetail()
        {
            string label = scenarioData.scenarioData[currentLine].OrderDetail;
            return label;
        }

        // �I�������e���擾
        public string GetSelectText(int selectNo)
        {
            return analyzer.GetSelectText(selectNo);
        }

        // �I���������擾
        public int GetSelectNumber()
        {
            return analyzer.GetSelectNumber();
        }

        // �󔒃C�x���g�p
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
        /// ���x���T���i���݂̃X�N���v�g�̂݁A���ꃉ�x�����T���͂Ȃ��j
        /// </summary>
        /// <param name="findLabel"></param>
        /// <returns></returns>
        private int FindLabel(string findLabel)
        {
            // ���ݍs������Ƀ��x��������̂��T��
            for(int i= currentLine + 1; i< maxLine; i++)
            {
                ScenarioMaster scenarioMaster = new ScenarioMaster();
                scenarioMaster = scenarioData.scenarioData[i];
                // ���x��
                if (scenarioMaster.Order == "LABEL")
                {
                    if(scenarioMaster.OrderDetail == findLabel){
                        Debug.Log("FindLabel:Find After");
                        return i; 
                    }
                }
            }
            
            // ���ݍs���ŏ��Ɍ����ĒT������
            for(int i= currentLine - 1; i >= 0; i--)
            {
                ScenarioMaster scenarioMaster = new ScenarioMaster();
                scenarioMaster = scenarioData.scenarioData[i];
                // ���x��
                if (scenarioMaster.Order == "LABEL")
                {
                    if (scenarioMaster.OrderDetail == findLabel) {
                        Debug.Log("FindLabel:Find Before");
                        return i; 
                    }
                }
            }

            // ������Ȃ������ꍇ
            return -1;
        }

        /// <summary>
        /// ���͂��ꂽ�e�L�X�g��ϐ��ɐݒ肷��
        /// </summary>
        /// <param name="setString"></param>
        public void setInputVariable(string setString)
        {
            DataManager dataManager = new DataManager();
            dataManager.Set(analyzer.GetInputVariable(), setString);
        }

        /// <summary>
        /// �v�Z���v�Z���s
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int calc(string str)
        {
            var result = Calculator.Evaluator.Evaluate(str);

            // �v�Z���ʂ�Console�֏o��
            Debug.Log(str + " = " + result);

            return 0;
        }
    }
}
