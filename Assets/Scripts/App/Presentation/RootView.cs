using System;
using System.IO;
using System.Threading;
using App.Presentation.Common;
using Cysharp.Threading.Tasks;
using Database.Impl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Presentation
{
    public class RootView : MonoBehaviour
    {
        [SerializeField]
        private FadeInOutView fadeInOutView;

        [SerializeField]
        private string githubToken;

        [SerializeField]
        private bool showPrompt;

        [SerializeField]
        private PopupDialogueView popupDialogueView;

        [SerializeField]
        private float scaleRate = 1f;
        
        private async void Start()
        {
            fadeInOutView.SetPanelBlack();
            await Initialize();
            await PlayFadeIn(CancellationToken.None);
        }

        private async UniTask CheckScenarioAccessToken()
        {
            var path = Path.Combine(UnityEngine.Application.dataPath,
                GitHubScenarioFetcher.ScenarioAccessTokenLocation);
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
            if (!File.Exists(path))
            {
                File.Create(path);
                showPrompt = true;
                await UniTask.WaitWhile(() => showPrompt);
                await File.WriteAllTextAsync(path, githubToken);
            }
        }

        private readonly Lazy<Rect> area = new(() =>
            new Rect(Screen.width / 2f - 250, Screen.height / 2f - 250, 500f, 500f));
        
        private void OnGUI()
        {
            var oldMatrix = GUI.matrix;

            if (showPrompt)
            {
                GUILayout.BeginArea(area.Value);

                GUILayout.Label("シナリオ取得用のTOKENを入力してください...");

                githubToken = GUILayout.TextField(githubToken, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Submit", GUILayout.ExpandWidth(true)))
                {
                    showPrompt = false;
                }

                GUILayout.EndArea();
            }

            GUI.matrix = oldMatrix;
        }

        private async UniTask Initialize()
        {
            await CheckScenarioAccessToken();

            // Load TitleScene if not loaded.
            if (SceneManager.sceneCount == 1 && SceneManager.GetActiveScene().name == PageManager.RootSceneName)
            {
                await PageManager.PushAsync("TitleScene");
            }
        }

        public async UniTask PlayFadeIn(CancellationToken ct)
        {
            await fadeInOutView.PlayFadeIn(ct);
        }

        public async UniTask PlayFadeOut(CancellationToken ct)
        {
            await fadeInOutView.PlayFadeOut(ct);
        }

        public async UniTask<bool> ShowExitConfirm()
        {
            return await popupDialogueView.ShowPopup("ゲームを終了しますか？", CancellationToken.None);
        }
    }
}