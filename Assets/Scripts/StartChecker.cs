using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChecker : MonoBehaviour
{
    private MoveObject moveObject;

    void Start()
    {
        moveObject = GetComponent<MoveObject>();
    }

    /// <summary>
    /// 空中床に移動速度を与える
    /// </summary>
    
    public void SetInitialSpeed()
    {
        moveObject.moveSpeed = 0.02f;
    }
}
