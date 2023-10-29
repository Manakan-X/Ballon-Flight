using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Button startButton;

    private void Start()
    {
        // 最初はボタンを無効にする
        startButton.interactable = false;
    }

    // ゲーム開始条件が達成されたら呼び出す関数
    public void EnableStartButton()
    {
        startButton.interactable = true;
    }
}
