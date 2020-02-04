using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameController : MonoBehaviour
{
    public static GameController instance; // Инстанс для обращения
    public static UiController uiController;
    public static bool GameIsOn = true;
    public static int turnNumber = 0;
    public static bool huntIsOn = false;
    // Реализация механики и гейм лупа
    private List<BotSheepBehaviour> botList = new List<BotSheepBehaviour>();
    private List<SheepBehaviour> sheepAliveList = new List<SheepBehaviour>();
    private PlayerSheepBehaviour playerSheep;
    private SafeZoneBehaviour safeZone;
    private GameObject gameField;
    private WolfBehaviour wolfBehaviour;

    private Vector3 safeZoneScale;

    private GameState currentGameState = GameState.roamingStage;
    public enum GameState
    {
        setup = 0,
        roamingStage = 1,
        safeZoneSpawn = 2,
        safeZoneLifts = 3,
        Hunt = 4,
        safeZoneDescends = 5,
        checkGameOver = 6
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
        SetupGame();
        StartCoroutine(GameLoop());
    }

    public void ReloadLevel()
    {
        GameManager.instance.LoadScene("MainScene");
    }

    private void SetupGame()
    {
        foreach (GameObject botSheep in GameObject.FindGameObjectsWithTag("SheepAgent")) // Ищем все игровые объекты и добавляем в менеджер
        {
            botList.Add(botSheep.GetComponent<BotSheepBehaviour>());
        }
        playerSheep = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSheepBehaviour>();
        sheepAliveList.AddRange(botList);
        sheepAliveList.Add(playerSheep);
        safeZone = GameObject.FindGameObjectWithTag("Rescue").GetComponent<SafeZoneBehaviour>();
        gameField = GameObject.FindGameObjectWithTag("GameField");
        wolfBehaviour = GameObject.FindGameObjectWithTag("Wolf").GetComponent<WolfBehaviour>();
        uiController = GameObject.FindGameObjectWithTag("UiController").GetComponent<UiController>();
        safeZoneScale = GameObject.FindGameObjectWithTag("Rescue").transform.localScale;

        // Обновляем статичские переменные
        GameIsOn = true;
        turnNumber = 0;
        huntIsOn = false;

    // На начало игры отключаем волка 
    wolfBehaviour.gameObject.SetActive(false);


        Debug.Log("Game Setup Finished");
    }

    public void StartSheepHunt()
    {
        wolfBehaviour.gameObject.SetActive(true);
        List<Transform> huntList = new List<Transform>();
        huntIsOn = true;
        foreach (SheepBehaviour sheep in sheepAliveList)
        {
            if (!sheep.IsSafe) // Добавляем овец в список охоты если они не на платформе
                huntList.Add(sheep.transform);
        }
        wolfBehaviour.SetSheepHuntList(huntList);
        foreach (Transform sheep in huntList) // Некрасиво, потом переделать
        {
            sheepAliveList.Remove(sheep.GetComponent<SheepBehaviour>());
        }
    }

    public void FinishSheepHunt() // Заканчиваем охоту и отключаем волка
    {
        huntIsOn = false;
        wolfBehaviour.gameObject.SetActive(false);
    }

    private void SpawnSafeZone() // Спавн безопасной зоны/подъемника
    {
        
        Bounds gameFieldBounds = gameField.GetComponent<Collider>().bounds;
        Bounds safeZoneBounds = safeZone.GetComponent<Collider>().bounds;
        safeZone.transform.localScale = new Vector3 (safeZoneScale.x / Mathf.Max(1,turnNumber),safeZoneScale.y, safeZoneScale.z / Mathf.Max(1, turnNumber));
        
       Vector3 spawnPosition;
        do
        {
            spawnPosition = gameField.transform.position + (Random.insideUnitSphere * gameFieldBounds.extents.x); // Проверка что она не вылазит за поле
            spawnPosition.y = 0.1f;
        }
        while (Mathf.Abs(spawnPosition.x) + safeZoneBounds.extents.x - gameFieldBounds.extents.x > 0 || Mathf.Abs(spawnPosition.z) + safeZoneBounds.extents.z - gameFieldBounds.extents.z > 0);

        // Натравливаем ботов на зону
        foreach (BotSheepBehaviour botSheep in botList)
        {
            botSheep.SetCurrentShipBehaviour(BotSheepBehaviour.BotSheepBehaviourMode.runningToSavePoint);
        }

        safeZone.transform.position = spawnPosition;
    }


    public IEnumerator GameLoop()
    {       
        while (GameIsOn)
        {
            switch (currentGameState)
            {
                case GameState.roamingStage:
                    {
                        turnNumber++;
                        Debug.Log("TurnNumber");
                        Debug.Log("Turn:" + turnNumber);
                        Debug.Log("roamingStage");
                        yield return new WaitForSeconds(5f);
                        currentGameState = GameState.safeZoneSpawn;
                        break;
                    }
                case GameState.safeZoneSpawn:
                    {
                        Debug.Log("safeZoneSpawn");
                        SpawnSafeZone();                       
                        yield return new WaitForSeconds(4f);
                        currentGameState = GameState.safeZoneLifts;
                        break;
                    }
                case GameState.safeZoneLifts:
                    {
                        Debug.Log("safeZoneLifts");
                        safeZone.LiftPlatform();
                        while (safeZone.platformMoving)
                            yield return null;
                        currentGameState = GameState.Hunt;
                        break;
                    }
                case GameState.Hunt:
                    {
                        Debug.Log("Hunt");
                        StartSheepHunt();
                        while (huntIsOn)
                            yield return null;
                        currentGameState = GameState.safeZoneDescends;
                        break;
                    }
                case GameState.safeZoneDescends:
                    {
                        Debug.Log("safeZoneDescends");
                        safeZone.DescendPlatform();
                        while (safeZone.platformMoving)
                            yield return null;

                        currentGameState = GameState.checkGameOver;
                        break;
                    }
                case GameState.checkGameOver:
                    {
                        // Проверка условия победы
                        Debug.Log("checkGameOver");
                        if (playerSheep.Alive)
                        {
                            if (botList.FirstOrDefault(one => one.Alive) == null)
                            {
                                Victory();                               
                            }
                            else
                            {
                                currentGameState = GameState.roamingStage;
                            }
                        }
                        else
                        {
                            Defeat();                            
                        }
                        break;
                    }
            }
        }
    }

    private void Victory()
    {
        GameIsOn = false;
        UiController.instance.ShowVictoryScreen();
    }

    public void Defeat()
    {
        GameIsOn = false;
        UiController.instance.ShowDefeatScreen();
    }

}
