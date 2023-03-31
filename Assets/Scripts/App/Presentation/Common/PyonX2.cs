using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace App.Presentation.Common
{
    public class PyonX2 : MonoBehaviour
    {
        [SerializeField]
        private GameObject target;

        public async UniTask StartPyonX2(CancellationToken token)
        {
            var pos = target.transform.localPosition;

            await target.transform
                .DOLocalMoveX(-5f, 0.25f)
                .SetRelative(true)
                .SetEase(Ease.OutQuad)
                .WithCancellation(token);


            await target.transform
                .DOLocalMoveX(10f, 0.5f)
                .SetRelative(true)
                .SetEase(Ease.OutQuad)
                .SetLoops(-1, LoopType.Yoyo)
                .WithCancellation(token);

            await target.transform.DOLocalMove(pos, 0.5f)
                .SetEase(Ease.OutQuad)
                .WithCancellation(target.GetCancellationTokenOnDestroy());
        }
    }
}