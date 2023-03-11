using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace App.Presentation.Common
{
    public class PyonX2 : MonoBehaviour
    {
        public async UniTask StartPyonX2(CancellationToken token)
        {
            var pos = transform.localPosition;
            await transform.DOLocalMoveY(15f, 0.5f)
                .SetRelative(true)
                .SetEase(Ease.OutQuad)
                .SetLoops(-1, LoopType.Yoyo)
                .WithCancellation(token);
            await transform.DOLocalMove(pos, 0.5f)
                .SetEase(Ease.OutQuad);
        }
    }
}