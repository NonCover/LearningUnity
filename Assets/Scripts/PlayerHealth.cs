using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class PlayerHealth : MonoBehaviour
{

    public GameObject deathVFXPrefab;

    int trapslayer;

    // Start is called before the first frame update
    void Start()
    {
        trapslayer = LayerMask.NameToLayer("Traps");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == trapslayer)
        {
            Instantiate(deathVFXPrefab, transform.position, transform.rotation);
            gameObject.SetActive(false);
            AudioManager.PlayDeathAudio();

            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 场景重置
            GameManager.PlayerDied(); // 角色死亡，重新加载场景
        }
    }
}
