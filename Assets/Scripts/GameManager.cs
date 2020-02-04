using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Инстанс для обращения   
    public static SoundManager soundManager;

    private void Awake()
    {
        // Простой синглтон
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string name)
    {
        AsyncLoader.instance.LoadScene(name);
    }

    private void Start()
    {
        soundManager = SoundManager.instance;
        LoadScene("MainScene");
    }

}
