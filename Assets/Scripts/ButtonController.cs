using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Button startButton;

    private void Start()
    {
        // �ŏ��̓{�^���𖳌��ɂ���
        startButton.interactable = false;
    }

    // �Q�[���J�n�������B�����ꂽ��Ăяo���֐�
    public void EnableStartButton()
    {
        startButton.interactable = true;
    }
}
