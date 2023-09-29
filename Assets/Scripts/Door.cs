using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    int openID;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        openID = Animator.StringToHash("Open");

        GameManager.RegisterDoor(this);
    }

    public void Open()
    {
        anim.SetTrigger(openID); // 触动打开门的动画

        AudioManager.PlayDoorOpenAudio();
    }

}

