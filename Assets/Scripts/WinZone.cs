using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZone : MonoBehaviour
{

    int PlayerLayer;

    void Start()
    {
        PlayerLayer = LayerMask.NameToLayer("Player");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == PlayerLayer)
        {
            Debug.Log("win");
            GameManager.PlayerWon();
        }
    }

}
