using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VerticalFloatingObject : MonoBehaviour
{
    public float moveTime;
    public float moveRange;

    Tweener tweener;

    void Start()
    {
        tweener = transform.DOMoveY(transform.position.y - moveRange, moveTime)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }

    private void OnDestroy()
    {
        tweener.Kill();
    }
}
