using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    // [Header("背景画像のスクロール速度 = 強制スクロール速度")]
    public float scrollSpeed = 0.01f;

    // [Header("画像のスクロール終了地点")]
    public float stopPosition = -16f;

    // [Header("画像の再スタート地点")]
    public float restartPosition = 5.8f;

    void Update()
    {
        // 画面の左方向にこのゲームオブジェクトの位置を移動する
        transform.Translate(-scrollSpeed, 0, 0);

        // このゲームオブジェクトの位置がstopPositionに到達したら
        if (transform.position.x < stopPosition)
        {
            // ゲームオブジェクトの位置を再スタート地点へ移動する
            transform.position = new Vector2(restartPosition, 0);
        }
    }
}
