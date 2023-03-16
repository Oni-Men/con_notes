using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Presentation
{

    public class PageManager
    {
        private static Stack<string> _pageStack = new ();
        private static GameObject _fadeView = null;

        private static void PushActiveIfEmpty()
        {
            if (_pageStack.Count != 0)
            {
                return;
            }
            
            _pageStack.Push(SceneManager.GetActiveScene().name);
        }
        
        public static async UniTask PushAsync(string sceneName, Action onLoad = null, bool hidePreviousScene = true)
        {
            PushActiveIfEmpty();
            
            await ShowFadeIn();
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (_pageStack.Count != 0 && hidePreviousScene)
            {
                HideScene(_pageStack.Peek());
            }
            _pageStack.Push(sceneName);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            onLoad?.Invoke();
            await ShowFadeOut();
        }

        public static async UniTask PopAsync(Action onUnload = null)
        {
            await ShowFadeIn();
            await SceneManager.UnloadSceneAsync(_pageStack.Pop());
            if (_pageStack.Count != 0)
            {
                ShowScene(_pageStack.Peek());
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_pageStack.Peek()));
            }
            onUnload?.Invoke();
            await ShowFadeOut();
        }

        public static async UniTask ReplaceAsync(string sceneName, Action onLoad = null)
        {
            if (_pageStack.Count == 0)
            {
                throw new Exception("There is no elements in the page stack");
            }
            await ShowFadeIn();

            var unLoadSceneName = _pageStack.Pop();
            _pageStack.Push(sceneName);
            
            var loadTask =  SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask();
            var unloadTask = SceneManager.UnloadSceneAsync(unLoadSceneName).ToUniTask();
            
            await UniTask.WhenAll(loadTask, unloadTask);
            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            onLoad?.Invoke();
            
            await ShowFadeOut();
        }

        public static T GetComponent<T>() where T : Component
        {
            var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
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
            await UniTask.CompletedTask;
        }
        
        private static async UniTask ShowFadeOut()
        {
            await UniTask.CompletedTask;
        }

    }
}