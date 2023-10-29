using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject aerialFloorPrefab;

    [SerializeField]
    private Transform generateTran;

    [Header("�����܂ł̑ҋ@����")]
    public float waitTime;

    private float timer;

    private GameDirector gameDirector;

    private bool isActivate;        // �����̏�Ԃ�ݒ肵�A�������s�����ǂ����̔���ɗ��p����Btrue�Ȃ琶�����Afalse�Ȃ琶�����Ȃ�

    void Update()
    {
        // ��~���͐������s��Ȃ�
        if (isActivate == false)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= waitTime)
        {
            timer = 0;
            GenerateFloor();
        }
    }

    /// <summary>
    /// �v���t�@�u�����ɃN���[���̃Q�[���I�u�W�F�N�g�𐶐�
    /// </summary>
    
    private void GenerateFloor()
    {
        GameObject obj = Instantiate(aerialFloorPrefab, generateTran);
        float randomPosY = Random.Range(-4.0f, 3.0f);
        obj.transform.position = new Vector2(obj.transform.position.x,
            obj.transform.position.y + randomPosY);

        // ���������J�E���g�A�b�v
        gameDirector.GenerateCount++;
    }

    /// <summary>
    /// FloorGenerator�̏���
    /// </summary>
    
    public void SetUpGenerator(GameDirector gameDirector)
    {
        this.gameDirector = gameDirector;

        // ToDo ���ɂ������ݒ肵������񂪂���ꍇ�ɂ͂����ɏ�����ǉ�����
    }

    /// <summary>
    /// ������Ԃ̃I��/�I�t��؂�ւ�
    /// </summary>
    /// <param name="isSwitch"></param>
    
    public void SwitchActivation(bool isSwitch)
    {
        isActivate = isSwitch;
    }
}
