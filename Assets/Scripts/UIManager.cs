using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static UIManager instance;

    public TextMeshProUGUI orbText, timeText, deathText, gameoverText;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    public static void UpdateOrbUI(int orbNum)
    {
        instance.orbText.text = orbNum.ToString();
    }

    public static void UpdateDeathUI(int deathNum)
    {
        instance.deathText.text = deathNum.ToString();
    }

    public static void UpdateTimeUI(float time)
    {
        int min = (int)time / 60;
        float seconds = time % 60;
        instance.timeText.text = min.ToString("00") + ":" + seconds.ToString("00");
    }

    public static void DisplayGameOver()
    {
        instance.gameoverText.enabled = true;
    }
    
}
