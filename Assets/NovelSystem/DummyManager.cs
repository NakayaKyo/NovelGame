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

            //TODO:�A�Z�b�g�ǂݍ��ݗp�̃��[�f�B���O�@�\��肽��

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


            // �V�i���I�f�[�^�ǂݍ���
            Addressables.LoadAssetAsync<ScenarioEditer>("ScenarioData/TestScenario.asset").Completed += handle =>
            {
                if (handle.Result == null)
                {
                    Debug.Log("ScenarioDataLoad Error");
                    return;
                }

                ScenarioEditer scenario = handle.Result;
                scenarioContoroller.SetScenarioData(scenario);

                // ����ǂݍ���
                NextScript();

            };

            Debug.Log("SETUP-OK");
        }

        // Update is called once per frame
        void Update()
        {
            // �N���b�N��
            if (backLogFlg == false && selectFlg == false && msc.onEnter())
            {
                NextScript();
            }

            // �I����
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

            // ��������A�t���O�Ő��䂷��
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
        /// ���̃X�N���v�g��ǂݍ���
        /// </summary>
        /// <returns></returns>
        private void NextScript()
        {
            token = cts.Token;
            // TODO: �������蒆�ɃN���b�N�ŕ���������X�L�b�v�ł���@�\����肽��
            // ���n�I��������Ŏ�������
            if (endtypeflg)
            {
                cts.Cancel();
                var label1 = novelMain.rootVisualElement.Q<Label>("TextArea");
                label1.text = text;
                endtypeflg = false;

                // �񓯊������L�����Z��������������
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
                        // �����G����̏ꍇ
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

                                //TODO: ��݂��񂾃f�[�^����������
                                stand.style.backgroundImage = standTexture;
                            }
                        }
                        // �w�i����̏ꍇ
                        else if (scenarioContoroller.CheckOrderBg())
                        {
                            var bg = novelBg.rootVisualElement.Q<VisualElement>("bg");
                            bg.RemoveFromClassList("Hide");
                            //TODO: ��݂��񂾃f�[�^����������
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
                            // ����Ȃ�
                        }
                        else if (scenarioContoroller.CheckOrderSelect())
                        {
                            // �I������\������
                            // TODO:�I�����̐��ɂ���ĕ\������ʒu��ύX������
                            // TODO:�I�������Q����s����ƂQ��ڂ̋������s����i�C�x���g�폜�ł��~�j
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

            // �󔒂ŕ�������NAME���擾����
            string[] split = selectString.Split(" ");

            // �I�����N���A
            int selectNumber = scenarioContoroller.GetSelectNumber();
            for (int i = 0; i < selectNumber; i++)
            {
                if (split[1] == selectVeName[i]) { scenarioContoroller.SelectionSelect(i); NextScript(); }

                var selectVe = novelSelect.rootVisualElement.Q<VisualElement>(selectVeName[i]);
                selectVe.AddToClassList("Hide");

                // �o�^�����C�x���g���폜����
                selectVe.UnregisterCallback<ClickEvent>(AddSelectEvent);
            }
        }

        async UniTask TypeingText(Label label ,string text, float derayTime, CancellationToken token = default)
        {
            // ���n�I�����ǁE�E�E��U����Ń`�F�b�N
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
