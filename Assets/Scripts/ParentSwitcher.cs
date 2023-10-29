using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentSwitcher : MonoBehaviour
{
    private string player = "Player";

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == player)
        {
            col.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
       if (col.gameObject.tag == player)
       {
            col.transform.SetParent(null);
       }
    }
}
