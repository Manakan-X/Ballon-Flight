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
    /// ‹ó’†°‚ÉˆÚ“®‘¬“x‚ğ—^‚¦‚é
    /// </summary>
    
    public void SetInitialSpeed()
    {
        moveObject.moveSpeed = 0.02f;
    }
}
