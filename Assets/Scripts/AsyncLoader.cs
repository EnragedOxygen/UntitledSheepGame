using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class AsyncLoader : MonoBehaviour {
    // Лоадер отвечающий за загрузку сцен
    public TextMeshProUGUI LoadingProgress;
    public static AsyncLoader instance;
    private const string LoadingScreen = "LoadingScreen";



    public void LoadScene (string sceneName)
    {
        SceneManager.LoadScene(LoadingScreen);
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

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

    private void Start()
    {
        
    }

    IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return null;              
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        Debug.Log("Loading scene: " + sceneName);
        asyncOperation.allowSceneActivation = false;
        Debug.Log("Pro :" + asyncOperation.progress);
       
        while (!asyncOperation.isDone)
        {
            //Показывать текущий процент загрузки
            if(LoadingProgress!=null)
            LoadingProgress.SetText((asyncOperation.progress * 100).ToString("f0") + "%");

            // Проверить завершилась ли загрузка
            if (asyncOperation.progress >= 0.9f)
            {               
                // Плейсхолдер если нужно будет сделать продолжение по нажатию
                //if (Input.GetKeyDown(KeyCode.Space))                   
                    asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
   
}
