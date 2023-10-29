using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ballon : MonoBehaviour
{
    private PlayerController playerController;

    private Tweener tweener;

    // <summary>
    // バルーンの初期設定
    // </summary>

    public void SetUpBallon(PlayerController playerController)
    {
        this.playerController = playerController;

        // 本来のScaleを保持
        Vector3 scale = transform.localScale;

        // 現在のScaleを0にして画面から一時的に非表示にする
        transform.localScale = Vector3.zero;

        // だんだんバルーンが膨らむアニメ演出
        transform.DOScale(scale, 2.0f).SetEase(Ease.InBounce);

        // 左右にふわふわさせる
        tweener = transform.DOLocalMoveX(0.02f, 0.2f).SetEase(Ease.Flash).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            // 左右にふわふわさせるループアニメを破棄する
            tweener.Kill();

            // PlayerControllerのDestroyBallonメソッドを呼び出し、バルーンの破壊処理を行う
            playerController.DestroyBallon();
        }
    }
}
