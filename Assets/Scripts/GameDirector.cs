using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField]
    private GoalChecker goalHousePrefab;        // �S�[���n�_�̃v���t�@�u���A�T�C��

    [SerializeField]
    private PlayerController playerController;      // �q�G�����L�[�ɂ���Cindy_Player�Q�[���I�u�W�F�N�g���A�T�C��

    [SerializeField]
    private FloorGenerator[] floorGenerators;       // floorGenerator�X�N���v�g�̃A�^�b�`����Ă���Q�[���I�u�W�F�N�g���A�T�C��

    [SerializeField]
    private RandomObjectGenerator[] randomObjectGenerators;     // RandomObjectGenerator�X�N���v�g�̃A�^�b�`����Ă���Q�[���I�u�W�F�N�g���A�T�C��

    [SerializeField]
    private AudioManager audioManager;      // �q�G�����L�[�ɂ���AAudioManager�X�N���v�g���A�^�b�`����Ă���Q�[���I�u�W�F�N�g���A�T�C��

    private bool isSetUp;       //�Q�[���̏�������p�Btrue�ɂȂ�ƃQ�[���J�n

    private bool isGameUp;      //�Q�[���I������p�Btrue�ɂȂ�ƃQ�[���I��

    private int generateCount;      //�󒆏��̐�����


    // generateCount�ϐ��̃v���p�e�B
    public int GenerateCount
    {
        set
        {
            generateCount = value;

            Debug.Log("������ / �N���A�ڕW�� ; " + generateCount + " / " + clearCount);

            if (generateCount >= clearCount)
            {
                // �S�[���n�_�𐶐�
                GenerateGoal();

                // �Q�[���I��
                GameUp();
            }
        }
        get
        {
            return generateCount;
        }    
        
    }

    public int clearCount;      // �S�[���n�_�𐶐�����܂łɕK�v�ȋ󒆏��̐�����

    private void Start()
    {
        int targetFPS = 60;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;

        // �^�C�g���ȍĐ�
        StartCoroutine(audioManager.PlayBGM(0));

        // �Q�[���J�n��ԂɃZ�b�g
        isGameUp = false;
        isSetUp = false;

        // FloorGenerator�̏���
        SetUpFloorGenerators();

        // �e�W�F�l���[�^�[���~
        StopGenerators();
    }

    /// <summary>
    /// FloorGenerator�̏���
    /// </summary>
    private void SetUpFloorGenerators()
    {
        for (int i = 0; i < floorGenerators.Length; i++)
        {
            // FloorGenerator�̏����E�����ݒ���s��
            floorGenerators[i].SetUpGenerator(this);
        }
    }

    private void Update()
    {
        // �v���C���[���͂��߂ăo���[���𐶐�������
        if (playerController.isFirstGenerateBallon && isSetUp == false)
        {
            // ��������
            isSetUp = true;

            // �e�W�F�l���[�^�𓮂����n�߂�
            ActivateGenerators();

            // �^�C�g���Ȃ��I�����A���C���Ȃ��Đ�
            StartCoroutine(audioManager.PlayBGM(1));
        }
    }

    /// <summary>
    /// �S�[���n�_�̐���
    /// </summary>
    private void GenerateGoal()
    {
        // �S�[���n�_�𐶐�
        GoalChecker goalHouse = Instantiate(goalHousePrefab);

        // �S�[���n�_�̏����ݒ�
        goalHouse.SetUpGoalHouse(this);

        // ToDo �S�[���n�_�̏����ݒ�
        Debug.Log("�S�[���n�_ ����");
    }

    /// <summary>
    /// �Q�[���I��
    /// </summary>
    public void GameUp()
    {
        // �Q�[���I��
        isGameUp = true;

        // �e�W�F�l���[�^���~
        StopGenerators();
    }

    /// <summary>
    /// �e�W�F�l���[�^���~����
    /// </summary>
    private void StopGenerators()
    {
        for (int i = 0; i < randomObjectGenerators.Length; i++)
        {
            randomObjectGenerators[i].SwitchActivation(false);
        }

        for (int i = 0; i < floorGenerators.Length; i++)
        {
            floorGenerators[i].SwitchActivation(false);
        }
    }

    /// <summary>
    /// �e�W�F�l���[�^�𓮂����n�߂�
    /// </summary>
    private void ActivateGenerators()
    {
        for (int i = 0; i < randomObjectGenerators.Length;i++)
        {
            randomObjectGenerators[i].SwitchActivation(true);
        }

        for (int i = 0; i < floorGenerators.Length;i++)
        {
            floorGenerators[i].SwitchActivation(true);
        }
    }

    /// <summary>
    /// �S�[������
    /// </summary>
    public void GoalClear()
    {
        // �N���A�̋ȍĐ�
        StartCoroutine(audioManager.PlayBGM(2));
    }
}
