using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject aerialFloorPrefab;

    [SerializeField]
    private Transform generateTran;

    [Header("生成までの待機時間")]
    public float waitTime;

    private float timer;

    private GameDirector gameDirector;

    private bool isActivate;        // 生成の状態を設定し、生成を行うかどうかの判定に利用する。trueなら生成し、falseなら生成しない

    void Update()
    {
        // 停止中は生成を行わない
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
    /// プレファブを元にクローンのゲームオブジェクトを生成
    /// </summary>
    
    private void GenerateFloor()
    {
        GameObject obj = Instantiate(aerialFloorPrefab, generateTran);
        float randomPosY = Random.Range(-4.0f, 3.0f);
        obj.transform.position = new Vector2(obj.transform.position.x,
            obj.transform.position.y + randomPosY);

        // 生成数をカウントアップ
        gameDirector.GenerateCount++;
    }

    /// <summary>
    /// FloorGeneratorの準備
    /// </summary>
    
    public void SetUpGenerator(GameDirector gameDirector)
    {
        this.gameDirector = gameDirector;

        // ToDo 他にも初期設定したい情報がある場合にはここに処理を追加する
    }

    /// <summary>
    /// 生成状態のオン/オフを切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    
    public void SwitchActivation(bool isSwitch)
    {
        isActivate = isSwitch;
    }
}
