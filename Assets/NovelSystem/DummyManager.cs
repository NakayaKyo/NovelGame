using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using UnityEngine.InputSystem;

using Cysharp.Threading.Tasks;

using NovelSystem.Contoroller;
using NovelSystem.Editer;
using NovelSystem.Data;
using MainSystem.Common;

namespace NovelSystem
{
    public class DummyManager : MonoBehaviour
    {

        [SerializeField]
        private UIDocument novelMain;

        [SerializeField]
        private UIDocument novelStand;

        [SerializeField]
        private UIDocument novelBg;

        [SerializeField]
        private UIDocument novelSelect;

        [SerializeField]
        private UIDocument novelBackLog;

        [SerializeField]
        private AudioSource source;


        private ScenarioContoroller scenarioContoroller;
        private AudioController audioController;
        private string text;
        private string nameArea;

        private bool selectFlg = false;
        private bool backLogFlg = false;

        private int backLogPoint = 0;
        private int backLogMax = 0;

        private bool endtypeflg = false;

        private MainSystemCommon msc = new MainSystemCommon();

        Texture2D standTexture;
        Texture2D bgTexture;
        AudioClip audioClip;

        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = new CancellationToken();

        private int backlogScrollCurrent = 0;

        // Start is called before the first frame update
        void Start()
        {
            scenarioContoroller = new ScenarioContoroller();

            audioController = new AudioController();

            //TODO:アセット読み込み用のローディング機能作りたい

            // loading images
            Addressables.LoadAssetAsync<Texture2D>("Assets/images/char.png").Completed += handle =>
            {
                if (handle.Result == null)
                {
                    Debug.Log("Image Load Error");
                    return;
                }

                standTexture = handle.Result;
            };

            Addressables.LoadAssetAsync<Texture2D>("Assets/images/bg01.png").Completed += handle =>
            {
                if (handle.Result == null)
                {
                    Debug.Log("Image Load Error");
                    return;
                }

                bgTexture = handle.Result;
            };

            // audio
            Addressables.LoadAssetAsync<AudioClip>("Assets/Audio/natsuyasuminotanken.mp3").Completed += handle =>
            {
                if (handle.Result == null)
                {
                    Debug.Log("Audio Load Error");
                    return;
                }

                audioClip = handle.Result;
            };


            // シナリオデータ読み込み
            Addressables.LoadAssetAsync<ScenarioEditer>("ScenarioData/TestScenario.asset").Completed += handle =>
            {
                if (handle.Result == null)
                {
                    Debug.Log("ScenarioDataLoad Error");
                    return;
                }

                ScenarioEditer scenario = handle.Result;
                scenarioContoroller.SetScenarioData(scenario);

                // 初回読み込み
                NextScript();

            };

            Debug.Log("SETUP-OK");
        }

        // Update is called once per frame
        void Update()
        {
            // クリック時
            if (backLogFlg == false && selectFlg == false && msc.onEnter())
            {
                NextScript();
            }

            // 選択肢
            if (selectFlg && msc.onEnter())
            {
                List<string> selectVeName = new List<string>();
                selectVeName.Add("SELECT01");
                selectVeName.Add("SELECT02");
                selectVeName.Add("SELECT03");
                selectVeName.Add("SELECT04");
                selectVeName.Add("SELECT05");

                int selectNumber = scenarioContoroller.GetSelectNumber();
                for (int i = 0; i < selectNumber; i++)
                {
                    var selectVe = novelSelect.rootVisualElement.Q<VisualElement>(selectVeName[i]);
                    selectVe.RegisterCallback<ClickEvent>(AddSelectEvent);
                }
            }

            var backlog = novelBackLog.rootVisualElement.Q<ScrollView>("BACKLOG");
            backlog.RegisterCallback<WheelEvent>(AddWheelEvent);

            // いったん、フラグで制御する
            if (selectFlg == false && backLogFlg == false && msc.wheelEvent() > 0)
            {
                var backlogArea = novelBackLog.rootVisualElement.Q<VisualElement>("BACKLOGAREA");
                backlogArea.RemoveFromClassList("Hide");

                backlog.Clear();

                int index = 0;

                while (index <= scenarioContoroller.GetCurrentLine() && index <= scenarioContoroller.GetMaxLine())
                {
                    string backlogString = scenarioContoroller.GetBackLog(index);

                    if (backlogString != "")
                    {
                        Label history = new Label();

                        history.text = backlogString;
                        backlog.Add(history);
                    }
                    index++;
                }

                backLogPoint = backlog.childCount - 1;
                backLogMax = backlog.childCount - 1;

                backLogFlg = true;
            }
            else if (backLogFlg == true && msc.wheelEvent() > 0)
            {
                if (backLogPoint >= 1) { backLogPoint -= 1; }
            }
            else if (backLogFlg == true && msc.wheelEvent() < 0)
            {
                if (backLogPoint >= backLogMax)
                {
                    var backlogArea = novelBackLog.rootVisualElement.Q<VisualElement>("BACKLOGAREA");
                    backlogArea.AddToClassList("Hide");
                    backLogFlg = false;
                }
                else
                {
                    backLogPoint += 1;
                }
            }

        }

        /// <summary>
        /// 次のスクリプトを読み込む
        /// </summary>
        /// <returns></returns>
        private void NextScript()
        {
            token = cts.Token;
            // TODO: 文字送り中にクリックで文字送りをスキップできる機能を作りたい
            // 原始的だがこれで実装する
            if (endtypeflg)
            {
                cts.Cancel();
                var label1 = novelMain.rootVisualElement.Q<Label>("TextArea");
                label1.text = text;
                endtypeflg = false;

                // 非同期処理キャンセルを初期化する
                cts = new CancellationTokenSource();
                token = cts.Token;
            }
            else
            {
                bool endFlg = true;
                while (endFlg)
                {
                    if (!scenarioContoroller.Next())
                    {
                        Debug.Log("READ-END");
                        endFlg = false;
                    }
                    else
                    {
                        // 立ち絵制御の場合
                        if (scenarioContoroller.CheckOrderStand())
                        {
                            StandData standData = scenarioContoroller.GetStandInfo();

                            if (standData.Point == "DELETE")
                            {
                                var stand1 = novelStand.rootVisualElement.Q<VisualElement>("RIGHT");
                                stand1.AddToClassList("Hide");
                                var stand2 = novelStand.rootVisualElement.Q<VisualElement>("CENTER");
                                stand2.AddToClassList("Hide");
                                var stand3 = novelStand.rootVisualElement.Q<VisualElement>("LEFT");
                                stand3.AddToClassList("Hide");
                            }
                            else
                            {
                                var stand = novelStand.rootVisualElement.Q<VisualElement>(standData.Point);
                                stand.RemoveFromClassList("Hide");

                                //TODO: よみこんだデータをつかいたい
                                stand.style.backgroundImage = standTexture;
                            }
                        }
                        // 背景制御の場合
                        else if (scenarioContoroller.CheckOrderBg())
                        {
                            var bg = novelBg.rootVisualElement.Q<VisualElement>("bg");
                            bg.RemoveFromClassList("Hide");
                            //TODO: よみこんだデータをつかいたい
                            bg.style.backgroundImage = bgTexture;
                        }
                        else if (scenarioContoroller.CheckOrderBgm())
                        {

                        }
                        else if (scenarioContoroller.CheckOrderGoto())
                        {
                            scenarioContoroller.GotoLabel(scenarioContoroller.GetGotoLabel());
                        }
                        else if (scenarioContoroller.CheckOrderEmpty())
                        {
                            // 操作なし
                        }
                        else if (scenarioContoroller.CheckOrderSelect())
                        {
                            // 選択肢を表示する
                            // TODO:選択肢の数によって表示する位置を変更したい
                            // TODO:選択肢を２回実行すると２回目の挙動が不安定（イベント削除でも×）
                            selectFlg = true;

                            List<string> selectVeName = new List<string>();
                            selectVeName.Add("SELECT01");
                            selectVeName.Add("SELECT02");
                            selectVeName.Add("SELECT03");
                            selectVeName.Add("SELECT04");
                            selectVeName.Add("SELECT05");

                            int selectNumber = scenarioContoroller.GetSelectNumber();
                            for (int i = 0; i < selectNumber; i++)
                            {
                                var selectVe = novelSelect.rootVisualElement.Q<VisualElement>(selectVeName[i]);
                                selectVe.RemoveFromClassList("Hide");

                                var select = novelSelect.rootVisualElement.Q<Label>(selectVeName[i] + "LABEL");
                                select.text = scenarioContoroller.GetSelectText(i);
                            }
                        }
                        else
                        {
                            text = scenarioContoroller.GetText();
                            var label = novelMain.rootVisualElement.Q<Label>("TextArea");
                            label.text = "";

                            nameArea = scenarioContoroller.GetName();
                            if (nameArea.Length == 0)
                            {
                                var visualElement = novelMain.rootVisualElement.Q<VisualElement>("NamePlate");
                                visualElement.AddToClassList("Hide");
                            }
                            else
                            {
                                var label2 = novelMain.rootVisualElement.Q<Label>("NameArea");
                                label2.text = nameArea;

                                var visualElement = novelMain.rootVisualElement.Q<VisualElement>("NamePlate");
                                visualElement.RemoveFromClassList("Hide");
                            }

                            TypeingText(label, text, 0.1f, token);
                            endFlg = false;
                        }
                    }
                }
            }
        }

        public void AddWheelEvent(WheelEvent mue)
        {
            var backlog = novelBackLog.rootVisualElement.Q<ScrollView>("BACKLOG");
            backlog.ScrollTo(backlog.ElementAt(backLogPoint));
        }

        public void AddSelectEvent(ClickEvent env)
        {
            selectFlg = false;

            List<string> selectVeName = new List<string>();
            selectVeName.Add("SELECT01");
            selectVeName.Add("SELECT02");
            selectVeName.Add("SELECT03");
            selectVeName.Add("SELECT04");
            selectVeName.Add("SELECT05");

            string selectString = env.currentTarget.ToString();

            // 空白で分割してNAMEを取得する
            string[] split = selectString.Split(" ");

            // 選択肢クリア
            int selectNumber = scenarioContoroller.GetSelectNumber();
            for (int i = 0; i < selectNumber; i++)
            {
                if (split[1] == selectVeName[i]) { scenarioContoroller.SelectionSelect(i); NextScript(); }

                var selectVe = novelSelect.rootVisualElement.Q<VisualElement>(selectVeName[i]);
                selectVe.AddToClassList("Hide");

                // 登録したイベントを削除する
                selectVe.UnregisterCallback<ClickEvent>(AddSelectEvent);
            }
        }

        async UniTask TypeingText(Label label ,string text, float derayTime, CancellationToken token = default)
        {
            // 原始的だけど・・・一旦これでチェック
            endtypeflg = true;
            for (int i=0; i < text.Length; i++){
                label.text = label.text + text[i];
                if (text[i] != '\\')
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(derayTime), cancellationToken: token);
                }
            }
            endtypeflg = false;
        }

    }
}
