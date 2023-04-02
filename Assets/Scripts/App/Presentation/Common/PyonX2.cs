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
            var scale = target.transform.localScale;
            
            await target.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 3)
                .WithCancellation(token);

            target.transform.localScale = scale;

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

            target.transform.localPosition = pos;
        }
    }
}