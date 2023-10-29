using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [Header("ˆÚ“®‘¬“x")]
    public float moveSpeed;

    void Update()
    {
        transform.position += new Vector3(-moveSpeed, 0, 0);
        if (transform.position.x <= -14.0f)
        {
            Destroy(gameObject);
        }
    }
}
