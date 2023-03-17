using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Presentation
{

    public class PageManager
    {
        public const string RootSceneName = "RootScene";
        private static readonly Stack<string> PageStack = new ();

        public static bool IsRootSceneLoaded { get; private set; }
        private static Scene _rootScene = default;
        private static RootView _rootView;
        
        private static void PushActiveIfEmpty()
        {
            if (PageStack.Count != 0)
            {
                return;
            }

            var activeSceneName = SceneManager.GetActiveScene().name;
            if (activeSceneName != RootSceneName)
            {
                PageStack.Push(activeSceneName);
            }
        }

        private static async UniTask LoadRootSceneIfNeeded()
        {
            if (IsRootSceneLoaded)
            {
                return;
            }

            var rootSceneExists = false;
            
            // Check if a root scene has already loaded or not.
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name != RootSceneName) continue;
                _rootScene = scene;
                rootSceneExists = true;
            }

            if (!rootSceneExists)
            {
                await SceneManager.LoadSceneAsync(RootSceneName, LoadSceneMode.Additive);
                _rootScene = SceneManager.GetSceneByName(RootSceneName);
            }
            
            IsRootSceneLoaded = true;
            _rootView = GetComponent<RootView>(_rootScene);
        }
        
        public static async UniTask PushAsync(string sceneName, Action onLoad = null, bool hidePreviousScene = true)
        {
            await LoadRootSceneIfNeeded();
            PushActiveIfEmpty();
            await ShowFadeOut();
            await PushSimplyAsync(sceneName, onLoad, hidePreviousScene);
            await ShowFadeIn();
        }

        public static async UniTask PushSimplyAsync(string sceneName, Action onLoad = null,
            bool hidePreviousScene = true)
        {
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (PageStack.Count != 0 && hidePreviousScene)
            {
                HideScene(PageStack.Peek());
            }
            PageStack.Push(sceneName);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            onLoad?.Invoke();
        }

        public static async UniTask PopAsync(Action onUnload = null)
        {
            await LoadRootSceneIfNeeded();
            PushActiveIfEmpty();
            await ShowFadeOut();
            await SceneManager.UnloadSceneAsync(PageStack.Pop());
            if (PageStack.Count != 0)
            {
                ShowScene(PageStack.Peek());
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(PageStack.Peek()));
            }
            onUnload?.Invoke();
            await ShowFadeIn();
        }

        public static async UniTask ReplaceAsync(string sceneName, Action onLoad = null)
        {
            if (PageStack.Count == 0)
            {
                throw new Exception("There is no elements in the page stack");
            }
            await LoadRootSceneIfNeeded();
            await ShowFadeOut();

            var unLoadSceneName = PageStack.Pop();
            PageStack.Push(sceneName);
            
            var loadTask =  SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask();
            var unloadTask = SceneManager.UnloadSceneAsync(unLoadSceneName).ToUniTask();
            
            await UniTask.WhenAll(loadTask, unloadTask);
            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            onLoad?.Invoke();
            
            await ShowFadeIn();
        }

        public static T GetComponent<T>() where T : Component
        {
            return GetComponent<T>(SceneManager.GetActiveScene());
        }
        public static T GetComponent<T>(Scene scene) where T : Component
        {
            var rootGameObjects = scene.GetRootGameObjects();
            return rootGameObjects
                .Select(go => go.GetComponent<T>())
                .FirstOrDefault(view => view is not null);
        }
        
        private static void HideScene(string sceneName)
        {
            var rootGameObjects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                rootGameObject.SetActive(false);
            }
        }

        private static void ShowScene(string sceneName)
        {
            var rootGameObjects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                rootGameObject.SetActive(true);
            }
        } 
        
        private static async UniTask ShowFadeIn()
        {
            await _rootView.PlayFadeIn(CancellationToken.None);
        }
        
        private static async UniTask ShowFadeOut()
        {
            await _rootView.PlayFadeOut(CancellationToken.None);
        }

    }
}