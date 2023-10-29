using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // キー入力用の文字列指定(InputManagerのHorizontalの入力を判定するための文字列)
    private string horizontal = "Horizontal";

    // キー入力用の文字列指定
    private string jump = "Jump";

    // コンポーネントの取得用
    private Rigidbody2D rb;

    private Animator anim;

    // 向きの設定に利用する
    private float scale;

    private float limitPosX = 9.5f;
    private float limitPosY = 5.5f;

    private bool isGameOver = false;    // GameOver状態の判定用。trueならゲームオーバー。

    private int ballonCount;

    public bool isFirstGenerateBallon;
    // 移動速度
    public float moveSpeed;

    // ジャンプ・浮遊力
    public float JumpPower;

    public bool isGrounded;

    public GameObject[] ballons;

    public int maxBallonCount;      // バルーンを生成する最大数

    public Transform[] ballonTrans;     // バルーンの生成位置の配列

    public GameObject ballonPrefab;     // バルーンのプレファブ

    public float generateTime;      // バルーンを生成する時間

    public bool isGenerating;        // バルーンを生成中かどうかを判定する。false なら生成していない状態。true は生成中の状態

    public float knockbackPower;    // 敵と接触した際に吹き飛ばされる力

    public int coinPoint;   // コインを獲得すると増えるポイントの総数

    public UIManager uiManager;

    [SerializeField, Header("Linecast用 地面判定レイヤー")]
    private LayerMask groundLayer;

    [SerializeField]
    private StartChecker startChecker;

    [SerializeField]
    private AudioClip knockbackSE;      // 敵と接触した際に鳴らすSE用のオーディオファイルをアサインする

    [SerializeField]
    private GameObject knockbackEffectPrefab;       // 敵と接触した際に生成するエフェクト用のプレファブのゲームオブジェクトをアサインする

    [SerializeField]
    private AudioClip coingetSE;       // コインと接触した際に鳴らすSEのオーディオファイルをアサインする

    [SerializeField]
    private GameObject coingetEffectPrefab;     // 敵と接触した際に生成するエフェクトのプレファブをアサインする

    [SerializeField]
    private Joystick joystick;      // FloatingJoyStickゲームオブジェクトにアタッチされているFloatingJoystickスクリプトのアサイン用

    [SerializeField]
    private Button btnJump;     // btnJumpゲームオブジェクトにアタッチされているButtonコンポーネントのアサイン用

    [SerializeField]
    private Button btnDetach;       // btnDetachOrGenerateゲームオブジェクトにアタッチされているButtonコンポーネントのアサイン用


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

        // Ballons配列変数の最大要素数が 0 以上なら = インスペクターでBallons変数に情報が登録されているなら
        if (ballons[0] != null)
        {



            // ジャンプ
            if (Input.GetButtonDown(jump))
            {
                Jump();
            }

            if (isGrounded == false && rb.velocity.y < 0.15f)   // 空中にいる間、落下中の場合
            {
                anim.SetTrigger("Fall");    // 落下アニメを繰り返す
            }

            // Velocity.y の値が5.0f を超える場合(ジャンプを連続で押した場合)
            if (rb.velocity.y > 5.0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, 5.0f);     // Velocity.y の値に制限をかける
            }
        }
        else
        {
            Debug.Log("バルーンがない。ジャンプ不可");
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
    /// ジャンプと空中浮遊
    /// </summary>

    private void Jump()
    {
        rb.AddForce(transform.up * JumpPower);  // キャラの位置を上方向に移動させる
        anim.SetTrigger("Jump");    //Jump(Up + Mid)アニメーションを再生させる
    }

    void FixedUpdate()
    {
        if (isGameOver == true)
        {
            return;
        }

        // 移動
        Move();
    }

    /// <summary>
    /// 移動
    /// </summary>

    private void Move()
    {
#if UNITY_EDITOR

        // 水平方向の入力受付
        // InputManager の Horizontal に登録されているキーの入力があるかどうか確認を行う
        float x = Input.GetAxis(horizontal);
        x = joystick.Horizontal;
#else
        float x = joystick.Horizontal;
#endif

        // xの値が0ではない場合 = キー入力がある場合
        if (x != 0)
        {
            // velocity(速度)に新しい値を代入して移動
            rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

            // temp変数に現在のlocalScale値を代入
            Vector3 temp = transform.localScale;

            // 現在のキー入力値xをtemp.xに代入
            temp.x = x;

            // 向きが変わるときに少数になるとキャラが縮んで見えてしまうので整数値にする
            if (temp.x > 0)
            {
                // 数字が0よりも大きければ全て1にする
                temp.x = scale;
            }
            else
            {
                // 数字が0よりも小さければ全て-1にする
                temp.x = -scale;
            }

            // キャラの向きを移動方向に合わせる
            transform.localScale = temp;

            // 待機状態のアニメの再生を止めて、走るアニメの再生への遷移を行う
            anim.SetFloat("Run", 0.5f);
        }
        else
        {
            // 左右の入力がなかったら横移動の速度を0にしてすぐに停止させる
            rb.velocity = new Vector2(0, rb.velocity.y);

            // 走るアニメの再生を止めて、待機状態のアニメの再生への遷移を行う
            anim.SetFloat("Run", 0.0f);
        }

        // 現在の位置情報が移動範囲の制限範囲を超えていないか確認する。超えていたら、制限範囲内に収める
        float posX = Mathf.Clamp(transform.position.x, -8.5f, 8.5f);
        float posY = Mathf.Clamp(transform.position.y, -limitPosY, limitPosY);

        // 現在の位置を更新(制限範囲を超えた場合、ここで移動の範囲を制限する)
        transform.position = new Vector2(posX, posY);
    }

    /// <summary>
    /// バルーン生成
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
            Debug.Log("初回のバルーン生成");
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

        // バルーンの数を増やす
        ballonCount++;

        yield return new WaitForSeconds(generateTime);

        isGenerating = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // 接触したコライダーを持つゲームオブジェクトのTagがEnemyなら
        if (col.gameObject.tag == "Enemy")
        {
            // キャラと敵の位置から距離と方向を計算
            Vector3 direction = (transform.position - col.transform.position).normalized;

            // 敵の反対側にキャラを吹き飛ばす
            transform.position += direction * knockbackPower;

            // 敵との接触用のSE(AudioClip)を再生する
            AudioSource.PlayClipAtPoint(knockbackSE, transform.position);

            // 接触した際のエフェクトを、敵の位置にクローンとして生成する。生成されたゲームオブジェクトを変数へ代入
            GameObject knockbackEffect = Instantiate(knockbackEffectPrefab, col.transform.position, Quaternion.identity);

            // エフェクトを0.5秒後に破棄。生成したタイミングで変数に代入しているので、削除の命令が出せる
            Destroy(knockbackEffect, 0.5f);
        }
    }

    // <summary>
    // バルーン破壊
    // </summary>
    public void DestroyBallon()
    {
        // ToDo 後程、バルーンが破壊される際に「割れた」ように見えるアニメ演出を追加する

        if (ballons[1] != null)
        {
            Destroy(ballons[1]);
        }
        else if (ballons[0] != null)
        {
            Destroy(ballons[0]);
        }

        // バルーンの数を減らす
        ballonCount--;
    }

    // IsTriggerがオンのコライダーを持つゲームオブジェクトを通過した場合に呼び出されるメソッド
    private void OnTriggerEnter2D(Collider2D col)
    {
        // 通過したコライダーを持つゲームオブジェクトのTagがCoinの場合
        if (col.gameObject.tag == "Coin")
        {
            // 通過したコインのゲームオブジェクトの持つCoinスクリプトを取得し、point変数の値をキャラの持つcoinPoint変数に加算
            coinPoint += col.gameObject.GetComponent<Coin>().point;

            // 通過したコインのゲームオブジェクトを破壊する
            Destroy(col.gameObject);

            uiManager.UpdateDisplayScore(coinPoint);

            // 敵との接触用のSE(AudioClip)を再生する
            AudioSource.PlayClipAtPoint(coingetSE, transform.position);

            // 接触した際のエフェクトを、敵の位置にクローンとして生成する。生成されたゲームオブジェクトを変数へ代入
            GameObject coingetEffect = Instantiate(coingetEffectPrefab, col.transform.position, Quaternion.identity);

            // エフェクトを0.5秒後に破棄。生成したタイミングで変数に代入しているので、削除の命令が出せる
            Destroy(coingetEffect, 0.5f);

        }
    }

    /// <summary>
    /// ゲームオーバー
    /// </summary>

    public void GameOver()
    {
        isGameOver = true;

        // ConsoleビューにisGameOver変数の値を表示する。ここが実行されるとtrueと表示される。
        Debug.Log(isGameOver);

        // 画面にゲームオーバー表示を行う
        uiManager.DisplayGameOverInfo();
    }

    /// <summary>
    /// ジャンプボタンを押した際の処理
    /// </summary>
    private void OnClickJump()
    {
        // バルーンが1つ以上あるなら
        if (ballonCount > 0)
        {
            Jump();
        }
    }

    /// <summary>
    /// バルーン生成ボタンを押した際の処理
    /// </summary>
    
    private void OnClickDetachOrGenerate()
    {
        // 地面に設置していて、バルーンが2個以下の場合
        if (isGrounded == true && ballonCount < maxBallonCount && isGenerating == false)
        {
            // バルーンの生成中でなければ、バルーンを1つ作成する
            StartCoroutine(GenerateBallon());
        }
    }
}
