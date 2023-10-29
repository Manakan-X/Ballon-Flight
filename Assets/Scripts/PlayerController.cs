using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // �L�[���͗p�̕�����w��(InputManager��Horizontal�̓��͂𔻒肷�邽�߂̕�����)
    private string horizontal = "Horizontal";

    // �L�[���͗p�̕�����w��
    private string jump = "Jump";

    // �R���|�[�l���g�̎擾�p
    private Rigidbody2D rb;

    private Animator anim;

    // �����̐ݒ�ɗ��p����
    private float scale;

    private float limitPosX = 9.5f;
    private float limitPosY = 5.5f;

    private bool isGameOver = false;    // GameOver��Ԃ̔���p�Btrue�Ȃ�Q�[���I�[�o�[�B

    private int ballonCount;

    public bool isFirstGenerateBallon;
    // �ړ����x
    public float moveSpeed;

    // �W�����v�E���V��
    public float JumpPower;

    public bool isGrounded;

    public GameObject[] ballons;

    public int maxBallonCount;      // �o���[���𐶐�����ő吔

    public Transform[] ballonTrans;     // �o���[���̐����ʒu�̔z��

    public GameObject ballonPrefab;     // �o���[���̃v���t�@�u

    public float generateTime;      // �o���[���𐶐����鎞��

    public bool isGenerating;        // �o���[���𐶐������ǂ����𔻒肷��Bfalse �Ȃ琶�����Ă��Ȃ���ԁBtrue �͐������̏��

    public float knockbackPower;    // �G�ƐڐG�����ۂɐ�����΂�����

    public int coinPoint;   // �R�C�����l������Ƒ�����|�C���g�̑���

    public UIManager uiManager;

    [SerializeField, Header("Linecast�p �n�ʔ��背�C���[")]
    private LayerMask groundLayer;

    [SerializeField]
    private StartChecker startChecker;

    [SerializeField]
    private AudioClip knockbackSE;      // �G�ƐڐG�����ۂɖ炷SE�p�̃I�[�f�B�I�t�@�C�����A�T�C������

    [SerializeField]
    private GameObject knockbackEffectPrefab;       // �G�ƐڐG�����ۂɐ�������G�t�F�N�g�p�̃v���t�@�u�̃Q�[���I�u�W�F�N�g���A�T�C������

    [SerializeField]
    private AudioClip coingetSE;       // �R�C���ƐڐG�����ۂɖ炷SE�̃I�[�f�B�I�t�@�C�����A�T�C������

    [SerializeField]
    private GameObject coingetEffectPrefab;     // �G�ƐڐG�����ۂɐ�������G�t�F�N�g�̃v���t�@�u���A�T�C������

    [SerializeField]
    private Joystick joystick;      // FloatingJoyStick�Q�[���I�u�W�F�N�g�ɃA�^�b�`����Ă���FloatingJoystick�X�N���v�g�̃A�T�C���p

    [SerializeField]
    private Button btnJump;     // btnJump�Q�[���I�u�W�F�N�g�ɃA�^�b�`����Ă���Button�R���|�[�l���g�̃A�T�C���p

    [SerializeField]
    private Button btnDetach;       // btnDetachOrGenerate�Q�[���I�u�W�F�N�g�ɃA�^�b�`����Ă���Button�R���|�[�l���g�̃A�T�C���p


    [SerializeField]
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        scale = transform.localScale.x;
        ballons = new GameObject[maxBallonCount];
        btnJump.onClick.AddListener(OnClickJump);
        btnDetach.onClick.AddListener(OnClickDetachOrGenerate);
    }

    void Update()
    {
        isGrounded = Physics2D.Linecast(transform.position + transform.up * 0.4f,
            transform.position - transform.up * 1.2f, groundLayer);

        Debug.DrawLine(transform.position + transform.up * 0.4f,
            transform.position - transform.up * 1.2f, Color.red, 1.0f);

        // Ballons�z��ϐ��̍ő�v�f���� 0 �ȏ�Ȃ� = �C���X�y�N�^�[��Ballons�ϐ��ɏ�񂪓o�^����Ă���Ȃ�
        if (ballons[0] != null)
        {



            // �W�����v
            if (Input.GetButtonDown(jump))
            {
                Jump();
            }

            if (isGrounded == false && rb.velocity.y < 0.15f)   // �󒆂ɂ���ԁA�������̏ꍇ
            {
                anim.SetTrigger("Fall");    // �����A�j�����J��Ԃ�
            }

            // Velocity.y �̒l��5.0f �𒴂���ꍇ(�W�����v��A���ŉ������ꍇ)
            if (rb.velocity.y > 5.0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, 5.0f);     // Velocity.y �̒l�ɐ�����������
            }
        }
        else
        {
            Debug.Log("�o���[�����Ȃ��B�W�����v�s��");
        }

        if (isGrounded == true && isGenerating == false)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(GenerateBallon());
            }
        }
    }

    /// <summary>
    /// �W�����v�Ƌ󒆕��V
    /// </summary>

    private void Jump()
    {
        rb.AddForce(transform.up * JumpPower);  // �L�����̈ʒu��������Ɉړ�������
        anim.SetTrigger("Jump");    //Jump(Up + Mid)�A�j���[�V�������Đ�������
    }

    void FixedUpdate()
    {
        if (isGameOver == true)
        {
            return;
        }

        // �ړ�
        Move();
    }

    /// <summary>
    /// �ړ�
    /// </summary>

    private void Move()
    {
#if UNITY_EDITOR

        // ���������̓��͎�t
        // InputManager �� Horizontal �ɓo�^����Ă���L�[�̓��͂����邩�ǂ����m�F���s��
        float x = Input.GetAxis(horizontal);
        x = joystick.Horizontal;
#else
        float x = joystick.Horizontal;
#endif

        // x�̒l��0�ł͂Ȃ��ꍇ = �L�[���͂�����ꍇ
        if (x != 0)
        {
            // velocity(���x)�ɐV�����l�������Ĉړ�
            rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

            // temp�ϐ��Ɍ��݂�localScale�l����
            Vector3 temp = transform.localScale;

            // ���݂̃L�[���͒lx��temp.x�ɑ��
            temp.x = x;

            // �������ς��Ƃ��ɏ����ɂȂ�ƃL�������k��Ō����Ă��܂��̂Ő����l�ɂ���
            if (temp.x > 0)
            {
                // ������0�����傫����ΑS��1�ɂ���
                temp.x = scale;
            }
            else
            {
                // ������0������������ΑS��-1�ɂ���
                temp.x = -scale;
            }

            // �L�����̌������ړ������ɍ��킹��
            transform.localScale = temp;

            // �ҋ@��Ԃ̃A�j���̍Đ����~�߂āA����A�j���̍Đ��ւ̑J�ڂ��s��
            anim.SetFloat("Run", 0.5f);
        }
        else
        {
            // ���E�̓��͂��Ȃ������牡�ړ��̑��x��0�ɂ��Ă����ɒ�~������
            rb.velocity = new Vector2(0, rb.velocity.y);

            // ����A�j���̍Đ����~�߂āA�ҋ@��Ԃ̃A�j���̍Đ��ւ̑J�ڂ��s��
            anim.SetFloat("Run", 0.0f);
        }

        // ���݂̈ʒu��񂪈ړ��͈͂̐����͈͂𒴂��Ă��Ȃ����m�F����B�����Ă�����A�����͈͓��Ɏ��߂�
        float posX = Mathf.Clamp(transform.position.x, -8.5f, 8.5f);
        float posY = Mathf.Clamp(transform.position.y, -limitPosY, limitPosY);

        // ���݂̈ʒu���X�V(�����͈͂𒴂����ꍇ�A�����ňړ��͈̔͂𐧌�����)
        transform.position = new Vector2(posX, posY);
    }

    /// <summary>
    /// �o���[������
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateBallon()
    {
        if (ballons[1] != null)
        {
            yield break;
        }

        isGenerating = true;

        if (isFirstGenerateBallon == false)
        {
            isFirstGenerateBallon = true;
            Debug.Log("����̃o���[������");
            startChecker.SetInitialSpeed();
        }

        if (ballons[0] == null)
        {
            ballons[0] = Instantiate(ballonPrefab, ballonTrans[0]);

            ballons[0].GetComponent<Ballon>().SetUpBallon(this);
        }
        else
        {
            ballons[1] = Instantiate(ballonPrefab, ballonTrans[1]);

            ballons[1].GetComponent<Ballon>().SetUpBallon(this);
        }

        // �o���[���̐��𑝂₷
        ballonCount++;

        yield return new WaitForSeconds(generateTime);

        isGenerating = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // �ڐG�����R���C�_�[�����Q�[���I�u�W�F�N�g��Tag��Enemy�Ȃ�
        if (col.gameObject.tag == "Enemy")
        {
            // �L�����ƓG�̈ʒu���狗���ƕ������v�Z
            Vector3 direction = (transform.position - col.transform.position).normalized;

            // �G�̔��Α��ɃL�����𐁂���΂�
            transform.position += direction * knockbackPower;

            // �G�Ƃ̐ڐG�p��SE(AudioClip)���Đ�����
            AudioSource.PlayClipAtPoint(knockbackSE, transform.position);

            // �ڐG�����ۂ̃G�t�F�N�g���A�G�̈ʒu�ɃN���[���Ƃ��Đ�������B�������ꂽ�Q�[���I�u�W�F�N�g��ϐ��֑��
            GameObject knockbackEffect = Instantiate(knockbackEffectPrefab, col.transform.position, Quaternion.identity);

            // �G�t�F�N�g��0.5�b��ɔj���B���������^�C�~���O�ŕϐ��ɑ�����Ă���̂ŁA�폜�̖��߂��o����
            Destroy(knockbackEffect, 0.5f);
        }
    }

    // <summary>
    // �o���[���j��
    // </summary>
    public void DestroyBallon()
    {
        // ToDo ����A�o���[�����j�󂳂��ۂɁu���ꂽ�v�悤�Ɍ�����A�j�����o��ǉ�����

        if (ballons[1] != null)
        {
            Destroy(ballons[1]);
        }
        else if (ballons[0] != null)
        {
            Destroy(ballons[0]);
        }

        // �o���[���̐������炷
        ballonCount--;
    }

    // IsTrigger���I���̃R���C�_�[�����Q�[���I�u�W�F�N�g��ʉ߂����ꍇ�ɌĂяo����郁�\�b�h
    private void OnTriggerEnter2D(Collider2D col)
    {
        // �ʉ߂����R���C�_�[�����Q�[���I�u�W�F�N�g��Tag��Coin�̏ꍇ
        if (col.gameObject.tag == "Coin")
        {
            // �ʉ߂����R�C���̃Q�[���I�u�W�F�N�g�̎���Coin�X�N���v�g���擾���Apoint�ϐ��̒l���L�����̎���coinPoint�ϐ��ɉ��Z
            coinPoint += col.gameObject.GetComponent<Coin>().point;

            // �ʉ߂����R�C���̃Q�[���I�u�W�F�N�g��j�󂷂�
            Destroy(col.gameObject);

            uiManager.UpdateDisplayScore(coinPoint);

            // �G�Ƃ̐ڐG�p��SE(AudioClip)���Đ�����
            AudioSource.PlayClipAtPoint(coingetSE, transform.position);

            // �ڐG�����ۂ̃G�t�F�N�g���A�G�̈ʒu�ɃN���[���Ƃ��Đ�������B�������ꂽ�Q�[���I�u�W�F�N�g��ϐ��֑��
            GameObject coingetEffect = Instantiate(coingetEffectPrefab, col.transform.position, Quaternion.identity);

            // �G�t�F�N�g��0.5�b��ɔj���B���������^�C�~���O�ŕϐ��ɑ�����Ă���̂ŁA�폜�̖��߂��o����
            Destroy(coingetEffect, 0.5f);

        }
    }

    /// <summary>
    /// �Q�[���I�[�o�[
    /// </summary>

    public void GameOver()
    {
        isGameOver = true;

        // Console�r���[��isGameOver�ϐ��̒l��\������B���������s������true�ƕ\�������B
        Debug.Log(isGameOver);

        // ��ʂɃQ�[���I�[�o�[�\�����s��
        uiManager.DisplayGameOverInfo();
    }

    /// <summary>
    /// �W�����v�{�^�����������ۂ̏���
    /// </summary>
    private void OnClickJump()
    {
        // �o���[����1�ȏ゠��Ȃ�
        if (ballonCount > 0)
        {
            Jump();
        }
    }

    /// <summary>
    /// �o���[�������{�^�����������ۂ̏���
    /// </summary>
    
    private void OnClickDetachOrGenerate()
    {
        // �n�ʂɐݒu���Ă��āA�o���[����2�ȉ��̏ꍇ
        if (isGrounded == true && ballonCount < maxBallonCount && isGenerating == false)
        {
            // �o���[���̐������łȂ���΁A�o���[����1�쐬����
            StartCoroutine(GenerateBallon());
        }
    }
}
