using System;
using System.IO;
using System.Threading;
using App.Presentation.Common;
using Cysharp.Threading.Tasks;
using Database.Impl;
using UnityEditor;
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
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(path);
            }

            if (!File.Exists(path))
            {
                File.Create(path);
                showPrompt = true;
                await UniTask.WaitWhile(() => showPrompt);
                await File.WriteAllTextAsync(path, githubToken);
            }
        }

        private readonly Lazy<Vector2> scaleAmount = new(() =>
        {
            var scale = Screen.height / 300f;
            return new Vector2(scale, scale);
        });
        
        private void OnGUI()
        {
            var oldMatrix = GUI.matrix;
            
            GUI.matrix = Matrix4x4.identity;
            GUI.matrix *= Matrix4x4.Translate(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            GUIUtility.ScaleAroundPivot(scaleAmount.Value, Vector2.zero);
            GUI.matrix *= Matrix4x4.Translate(new Vector3(-125f, 0, 0));

            if (showPrompt)
            {
                GUILayout.BeginVertical(GUILayout.Width(250f));
                
                githubToken = GUILayout.TextField(githubToken, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Submit", GUILayout.ExpandWidth(true)))
                {
                    showPrompt = false;
                }
                
                GUILayout.EndVertical();
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
    }
}