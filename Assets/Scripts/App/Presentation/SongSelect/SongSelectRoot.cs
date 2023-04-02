using System;
using System.Collections.Generic;
using System.Linq;
using App.Database;
using App.Presentation.Common;
using App.Presentation.Ingame.Views;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace App.Presentation.SongSelect
{
    public class SongSelectRoot : MonoBehaviour
    {

        [SerializeField]
        private PopupDialogueView popupDialogueView;

        [SerializeField]
        private List<ListItem> songListItems;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            var database = DatabaseFactory.SongDatabase;
            var songs = database.All().Keys.ToList();
            
            // とりあえず表示できる分だけ表示する
            for (var i = 0; i < songListItems.Count; i++)
            {
                var item = songListItems[i];
                if (i >= songs.Count)
                {
                    item.gameObject.SetActive(false);
                    continue;
                }

                var key = songs[i];
                var title = database.Get(key);
                item.SetText(title);
                
                item.AsObservable().Subscribe(_ =>
                {
                    PlaySong(key).Forget();
                });
            }
        }

        private async UniTask PlaySong(string songPath)
        {
            await PageManager.PushAsyncWithFade("IngameScene", () => OnLoadInGame(songPath));
            var ingameViewRoot = PageManager.GetComponent<InGameViewRoot>();
            ingameViewRoot.StartGame();
        }

        private UniTask OnLoadInGame(string songPath)
        {
            var inGameViewRoot = PageManager.GetComponent<InGameViewRoot>();
            if (inGameViewRoot is null)
            {
                return UniTask.CompletedTask;
            }

            var param = new InGameViewRoot.InGameViewParam
            {
                songDirectoryPath = songPath
                // TODO スカイボックスを指定するパラメータを追加
            };
            inGameViewRoot.Initialize(param);

            return UniTask.CompletedTask;
        }
    }
}