using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text txtScore;  // txtScore �Q�[���I�u�W�F�N�g�̎��� Text �R���|�[�l���g���C���X�y�N�^�[����A�T�C������

    [SerializeField]
    private Text txtInfo;

    [SerializeField]
    private CanvasGroup canvasGroupInfo;

    [SerializeField]
    private ResultPopUp resultPopUpPrefab;

    [SerializeField]
    private Transform canvasTran;

    [SerializeField]
    private Button btnInfo;

    [SerializeField]
    private Button btnTitle;

    [SerializeField]
    private Text lblStart;

    [SerializeField]
    private CanvasGroup canvasGroupTitle;

    private Tweener tweener;

    /// <summary>
    /// �X�R�A�\�����X�V
    /// </summary>
    /// <param name="score"></param>

    public void UpdateDisplayScore(int score)   // ���̈����ŃX�R�A�̏����󂯎��
    {
        txtScore.text = score.ToString();
    }

    /// <summary>
    /// �Q�[���I�[�o�[�\��
    /// </summary>
    
    public void DisplayGameOverInfo()
    {
        // InfoBackGround�Q�[���I�u�W�F�N�g�̎���CanvasGroup�R���|�[�l���g��Alpha�̒l���A
        // 1�b������1�ɕύX���āA�w�i�ƕ�������ʂɌ�����悤�ɂ���
        canvasGroupInfo.DOFade(1.0f, 1.0f);

        // ��������A�j���[�V���������ĕ\��
        txtInfo.DOText("Game Over...", 1.0f);

        btnInfo.onClick.AddListener(RestartGame);
    }

    /// <summary>
    /// ResultPopUp�̐���
    /// </summary>
    
    public void GenerateResultPopUp(int score)
    {

        // ResultPopUp�𐶐�
        ResultPopUp resultPopUp = Instantiate(resultPopUpPrefab, canvasTran, false);

        // ResultPopUp�̐ݒ���s��
        resultPopUp.SetUpResultPopUp(score);
    }

    /// <summary>
    /// �^�C�g���֖߂�
    /// </summary>
    
    public void RestartGame()
    {
        // �{�^�����烁�\�b�h���폜(�d���N���b�N�h�~)
        btnInfo.onClick.RemoveAllListeners();

        // ���݂̃V�[���̖��O���擾
        string sceneName = SceneManager.GetActiveScene().name;

        canvasGroupInfo.DOFade(0f, 1.0f)
            .OnComplete( () => {
            Debug.Log("Restart");
            SceneManager.LoadScene(sceneName);
            });
    }

    private void Start()
    {
        // �^�C�g���\��
        SwitchDisplayTitle(true, 1.0f);

        // �{�^����OnClick�C�x���g�Ƀ��\�b�h��o�^
        btnTitle.onClick.AddListener(OnClickTitle);
    }

    /// <summary>
    /// �^�C�g���\��
    /// </summary>
     
    public void SwitchDisplayTitle(bool isSwitch, float alpha)
    {
        // �^�C�g�����\������ĂȂ��Ƃ��A�^�C�g����\��������
        if (isSwitch) canvasGroupTitle.alpha = 0;

        canvasGroupTitle.DOFade(alpha, 1.0f).SetEase(Ease.Linear).OnComplete(() => {
            lblStart.gameObject.SetActive(isSwitch);
        });

        if (tweener == null)
        {
            // Tap Start�̕������������_�ł�����
            tweener = lblStart.gameObject.GetComponent<CanvasGroup>()
                .DOFade(0, 1.0f)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        }
        else tweener.Kill();
    }

    /// <summary>
    /// �^�C�g���\�����ɉ�ʂ��N���b�N�����ۂ̏���
    /// </summary>
     
    private void OnClickTitle()
    {
        // �{�^���̃��\�b�h���폜���ďd���^�b�v�h�~
        btnTitle.onClick.RemoveAllListeners();

        // �^�C�g�������X�ɔ�\��
        SwitchDisplayTitle(false, 0.0f);

        // �^�C�g���\����������̂Ɠ���ւ��ŁA�Q�[���X�^�[�g�̕�����\������
        StartCoroutine(DisplayGameStartInfo());
    }

    /// <summary>
    /// �Q�[���X�^�[�g�\��
    /// </summary>
    /// <returns></returns>
     
    public IEnumerator DisplayGameStartInfo()
    {
        // 0.5�b�ҋ@���Ă��玟�̏����Ɉڂ�
        yield return new WaitForSeconds(0.5f);

        // �w�i�𓧖��ɂ���
        canvasGroupInfo.alpha = 0;
        // �w�i��0.5�b�����ăA�j���[�V���������āA�s������Ԃŕ\��������
        canvasGroupInfo.DOFade(1.0f, 0.5f);
        // UI�e�L�X�g�̃e�L�X�g��"Game Start!"�ɕύX����
        txtInfo.text = "Game Start!";
        
        // 1�b�ҋ@���Ă��玟�̏����Ɉڂ�
        yield return new WaitForSeconds(1.0f);
        // �w�i��0.5�b�ԃA�j���[�V�������Ȃ��瓧���ɂȂ�
        canvasGroupInfo.DOFade(0f, 0.5f);
        // �^�C�g����ʂ�����
        canvasGroupTitle.gameObject.SetActive(false);
    }
}
