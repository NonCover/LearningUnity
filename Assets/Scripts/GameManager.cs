using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    SceneFader fader;
    List<Orb> orbs;
    //public int orbNum;
    public int deathNum;
    float gameTime;
    Door lockedDoor;

    bool gameIsOver;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        orbs = new List<Orb>();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (gameIsOver) // 游戏结束
            return;
        //orbNum = instance.orbs.Count; // 获取宝珠的数量
        gameTime += Time.deltaTime;
        UIManager.UpdateTimeUI(gameTime);
    }

    public static void PlayerDied()
    {
        instance.deathNum++;
        UIManager.UpdateDeathUI(instance.deathNum);
        instance.fader.FadeOut();
        instance.Invoke("RestartScene", 1.5f);
    }


    public static void RegisterSceneFader(SceneFader obj)
    {
        instance.fader = obj;
    }

    public static void RegisterOrb(Orb orb)
    {
        if (instance == null) return;
        if (!instance.orbs.Contains(orb))
        {
            instance.orbs.Add(orb);
        }
        UIManager.UpdateOrbUI(instance.orbs.Count);
    }

    public static void PlayerGrabbedOrb(Orb orb)
    {
        if (!instance.orbs.Contains(orb)) return;
        instance.orbs.Remove(orb);
        if (instance.orbs.Count == 0)
            instance.lockedDoor.Open();
        UIManager.UpdateOrbUI(instance.orbs.Count);
    }

    public static void RegisterDoor(Door door)
    {
        instance.lockedDoor = door;
    }

    public static void PlayerWon()
    {
        instance.gameIsOver = true;
        UIManager.DisplayGameOver();
    }

    public static bool GameOver()
    {
        return instance.gameIsOver;
    }

    void RestartScene()
    {
        instance.orbs.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}


