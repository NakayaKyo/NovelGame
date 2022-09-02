using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        private AudioSource source;

        private ScenarioContoroller scenarioContoroller;
        private AudioController audioController;
        private string text;
        private string nameArea;

        private bool selectFlg = false;
        private bool backLogFlg = false;
        private bool inputFlg = false;
        private bool waitFlg = false;

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

            // 一時的にこちらで実施する
            MainSystem.Container.UserManager.Instance.Initialize();

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
            if (inputFlg == false &&backLogFlg == false && selectFlg == false && msc.onEnter())
            {
                NextScript();
            }

            // 選択肢
            if (selectFlg && msc.onEnter())
            {
                List<string> selectVeName = new List<string>();
                selectVeName.Add("select01-area");
                selectVeName.Add("select02-area");
                selectVeName.Add("select03-area");
                selectVeName.Add("select04-area");
                selectVeName.Add("select05-area");

                var dialog = novelMain.rootVisualElement.Q<VisualElement>("dialog");

                int selectNumber = scenarioContoroller.GetSelectNumber();
                for (int i = 0; i < selectNumber; i++)
                {
                    var selectVe = dialog.Q<VisualElement>(selectVeName[i]);
                    selectVe.RegisterCallback<ClickEvent>(AddSelectEvent);
                }
            }

            // 入力時
            if(inputFlg && msc.onEnter())
            {
                var inputVe = novelMain.rootVisualElement.Q<Button>("input-button");
                inputVe.RegisterCallback<ClickEvent>(AddInputEvent);
            }

            // バックログ時
            var backlog = novelMain.rootVisualElement.Q<ScrollView>("history-view");
            backlog.RegisterCallback<WheelEvent>(AddWheelEvent);

            if (inputFlg == false && selectFlg == false && backLogFlg == false && msc.wheelEvent() > 0)
            {
                var backlogArea = novelMain.rootVisualElement.Q<VisualElement>("history");
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
                    var backlogArea = novelMain.rootVisualElement.Q<VisualElement>("history");
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
                var system = novelMain.rootVisualElement.Q<VisualElement>("system");
                var label1 = system.Q<Label>("message");
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
                        FadeManager.Instance.LoadScene("test", 1.0f);
                    }
                    else
                    {
                        switch (scenarioContoroller.GetOrderUpper())
                        {
                            // 立ち絵制御の場合
                            case Order.Name.Stand:
                                StandData standData = scenarioContoroller.GetStandInfo();

                                if (standData.Point == "DELETE")
                                {
                                    var stand1 = novelMain.rootVisualElement.Q<VisualElement>("charctor-left");
                                    stand1.AddToClassList("Hide");
                                    var stand2 = novelMain.rootVisualElement.Q<VisualElement>("charctor-center");
                                    stand2.AddToClassList("Hide");
                                    var stand3 = novelMain.rootVisualElement.Q<VisualElement>("charctor-right");
                                    stand3.AddToClassList("Hide");
                                }
                                else
                                {
                                    var stand = novelMain.rootVisualElement.Q<VisualElement>(standData.Point);
                                    stand.RemoveFromClassList("Hide");

                                    //TODO: よみこんだデータをつかいたい
                                    stand.style.backgroundImage = standTexture;
                                }
                                break;
                            // 背景制御の場合
                            case Order.Name.Bg_Switch:
                                var bg = novelMain.rootVisualElement.Q<VisualElement>("background");
                                bg.RemoveFromClassList("Hide");
                                //TODO: よみこんだデータをつかいたい
                                bg.style.backgroundImage = bgTexture;
                                break;
                            // BGM
                            case Order.Name.Bgm:
                                break;
                            // Goto
                            case Order.Name.Goto:
                                scenarioContoroller.GotoLabel(scenarioContoroller.GetGotoLabel());
                                break;
                            // Set
                            case Order.Name.Set:
                                scenarioContoroller.SetVariable();
                                break;
                            // 選択肢
                            case Order.Name.Select:
                                // 選択肢を表示する
                                // TODO:選択肢の数によって表示する位置を変更したい
                                // TODO:選択肢を２回実行すると２回目の挙動が不安定（イベント削除でも×）
                                selectFlg = true;

                                List<string> selectVeName = new List<string>();
                                selectVeName.Add("select01-");
                                selectVeName.Add("select02-");
                                selectVeName.Add("select03-");
                                selectVeName.Add("select04-");
                                selectVeName.Add("select05-");

                                var dialog = novelMain.rootVisualElement.Q<VisualElement>("dialog");
                                dialog.RemoveFromClassList("Hide");

                                int selectNumber = scenarioContoroller.GetSelectNumber();
                                for (int i = 0; i < selectNumber; i++)
                                {
                                    var selectVe = dialog.Q<VisualElement>(selectVeName[i]+ "area");
                                    selectVe.RemoveFromClassList("Hide");

                                    var select = dialog.Q<Label>(selectVeName[i] + "text");
                                    select.text = scenarioContoroller.GetSelectText(i);
                                }
                                break;
                            case Order.Name.Input:
                                inputFlg = true;
                                var inputVe = novelMain.rootVisualElement.Q<VisualElement>("input");
                                Debug.Log("input:" + inputVe);
                                inputVe.RemoveFromClassList("Hide");
                                break;
                            // Calc
                            case Order.Name.Calc:
                                scenarioContoroller.calc(scenarioContoroller.GetCalcDetail());
                                break;
                            // Empty
                            case Order.Name.Empty:
                                break;
                            // Text
                            default:
                                text = scenarioContoroller.GetText();
                                var system = novelMain.rootVisualElement.Q<VisualElement>("system");

                                var label = system.Q<Label>("message");
                                label.text = "";

                                nameArea = scenarioContoroller.GetName();
                                if (nameArea.Length == 0)
                                {
                                    var visualElement = system.Q<VisualElement>("name-area");
                                    visualElement.AddToClassList("Hide");
                                }
                                else
                                {
                                    var label2 = system.Q<Label>("name");
                                    label2.text = nameArea;

                                    var visualElement = system.Q<VisualElement>("name-area");
                                    visualElement.RemoveFromClassList("Hide");
                                }

                                TypeingText(label, text, 0.1f, token);
                                endFlg = false;
                                break;
                        }
                    }
                }
            }
        }

        public void AddWheelEvent(WheelEvent mue)
        {
            var backlog = novelMain.rootVisualElement.Q<ScrollView>("history-view");
            backlog.ScrollTo(backlog.ElementAt(backLogPoint));
        }

        public void AddInputEvent(ClickEvent env)
        {
            inputFlg = false;
            var inputInfo = novelMain.rootVisualElement.Q<TextField>("input-text");
            scenarioContoroller.setInputVariable(inputInfo.text);

            var inputVe = novelMain.rootVisualElement.Q<VisualElement>("input");
            inputVe.AddToClassList("Hide");

            // 次のスクリプトを読み込む
            NextScript();
        }

        public void AddSelectEvent(ClickEvent env)
        {
            selectFlg = false;

            List<string> selectVeName = new List<string>();
            selectVeName.Add("select01-area");
            selectVeName.Add("select02-area");
            selectVeName.Add("select03-area");
            selectVeName.Add("select04-area");
            selectVeName.Add("select05-area");

            string selectString = env.currentTarget.ToString();

            // 空白で分割してNAMEを取得する
            string[] split = selectString.Split(" ");

            // 選択肢クリア
            var dialog = novelMain.rootVisualElement.Q<VisualElement>("dialog");
            dialog.AddToClassList("Hide");

            int selectNumber = scenarioContoroller.GetSelectNumber();
            for (int i = 0; i < selectNumber; i++)
            {
                if (split[1] == selectVeName[i]) { scenarioContoroller.SelectionSelect(i); NextScript(); }

                var selectVe = dialog.Q<VisualElement>(selectVeName[i]);
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
