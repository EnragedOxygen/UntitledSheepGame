using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{
    public static UiController instance;

    [SerializeField]
    private GameObject defeatScreen;
    [SerializeField]
    private GameObject victoryScreen;


    private void Awake()
    {
        instance = this;
    }

    public void ShowVictoryScreen()
    {
        victoryScreen.SetActive(true);
    }

    public void ShowDefeatScreen()
    {
        defeatScreen.SetActive(true);
    }

    public void ReloadLevel()
    {
        GameController.instance.ReloadLevel();
    }
}
