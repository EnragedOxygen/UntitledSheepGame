using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZoneBehaviour : MonoBehaviour
{
    private List<GameObject> sheepOnPlatformList = new List<GameObject>();
    public bool platformMoving = false;
    [SerializeField]
    float speed = 1;

    public void SecurePlatformSheep()
    {
        foreach(GameObject sheep in sheepOnPlatformList)
        {
            if (sheep.tag == "SheepAgent")
            {
                sheep.GetComponent<BotSheepBehaviour>().SetCurrentShipBehaviour(BotSheepBehaviour.BotSheepBehaviourMode.disabled); // Отключаем овц на платформе чтобы они отвязались от навмеша
                sheep.GetComponent<BotSheepBehaviour>().IsSafe = true;  // Помечаем что овца в безопасности
                sheep.GetComponent<Rigidbody>().isKinematic = true;
                sheep.transform.parent = transform;
            }
            else if (sheep.tag == "Player")
            {
                sheep.GetComponent<PlayerSheepBehaviour>().controlsAreOn=false; // Отключаем игрока пока он на платформе
                sheep.GetComponent<PlayerSheepBehaviour>().IsSafe = true;  // Помечаем что овца в безопасности  
                sheep.GetComponent<Rigidbody>().isKinematic = true;
                sheep.transform.parent = transform;
            }
           
        }
    }

    public void ReleasePlatformSheep()
    {
        foreach (GameObject sheep in sheepOnPlatformList)
        {
            if (sheep.tag == "SheepAgent")
            {
                sheep.GetComponent<BotSheepBehaviour>().SetCurrentShipBehaviour(BotSheepBehaviour.BotSheepBehaviourMode.roaming); // Включаем овец и привязываем их к мешу
                sheep.GetComponent<BotSheepBehaviour>().IsSafe = false;  // Почеаем что овца в опасности     
                sheep.GetComponent<Rigidbody>().isKinematic = false;
                sheep.transform.parent = null; // Открепляем от платформы   
            }
            else if (sheep.tag == "Player")
            {
                sheep.GetComponent<PlayerSheepBehaviour>().controlsAreOn = true; // Включаем игрока 
                sheep.GetComponent<PlayerSheepBehaviour>().IsSafe = false;  // Почеаем что овца в опасности 
                sheep.GetComponent<Rigidbody>().isKinematic = false;
                sheep.transform.parent = null; // Открепляем от платформы   
            }
                   
        }
        sheepOnPlatformList.Clear();
    }

    public void LiftPlatform()
    {
        SecurePlatformSheep();
        platformMoving = true;
        StartCoroutine(LiftPlatformRoutine());
    }

    public void DescendPlatform()
    {
        platformMoving = true;
        StartCoroutine(DescendPlatformRoutine());
    }

    public IEnumerator LiftPlatformRoutine() // Поднимаем платформу пока охотится волк, и опускаем после
    {
        Vector3 initialPosition; 
        initialPosition = transform.position;
        Vector3 newPosition = initialPosition + Vector3.up * 10;
        while (transform.position != newPosition) {
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, newPosition, speed*Time.deltaTime);
          }
        platformMoving = false;       
    }

    public IEnumerator DescendPlatformRoutine() // Опускаем платформу
    {
        Vector3 initialPosition; // Опускаем платформу
        initialPosition = transform.position;
        Vector3 newPosition = initialPosition + Vector3.down * 10;
        while (transform.position != newPosition)
        {
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
        }
        platformMoving = false;
        ReleasePlatformSheep();        
    }

    private void OnTriggerEnter(Collider Collider) // Добавляем/убираем овц в список при их входе/выходе
    {
       
        if(!sheepOnPlatformList.Contains(Collider.gameObject) && Collider.tag!="GameField")
        sheepOnPlatformList.Add(Collider.gameObject);
    }

    private void OnTriggerExit(Collider Collider)
    {
        if (sheepOnPlatformList.Contains(Collider.gameObject))
            sheepOnPlatformList.Remove(Collider.gameObject);
    }
}
