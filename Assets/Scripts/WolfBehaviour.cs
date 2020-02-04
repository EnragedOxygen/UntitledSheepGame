using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WolfBehaviour : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    public List<Transform> sheepHuntList = new List<Transform>();
    [SerializeField]
    private float wolfKnockbackForce=200; 
    [SerializeField]
    private float speed;
    [SerializeField]
    // скорость поворjnf овцы в сторону движения
    private float faceTurnSpeed = 3; 
    Vector3 lookDirection;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    public void SetSheepHuntList(List<Transform> huntList)
    {
        sheepHuntList = huntList;        
    }

    private void Update()
    {
        if (GameController.huntIsOn)
        {
            if (sheepHuntList.Count > 0)
            {
                // Охотимся на овец по очереди
                Vector3 target = new Vector3 (sheepHuntList[0].position.x, transform.position.y, sheepHuntList[0].position.z);
                navMeshAgent.SetDestination(target);
            }
            else
            {
                // Отправляем сигнал об окончании охоты
                GameController.instance.FinishSheepHunt();
            }
            animator.SetBool("Moving", navMeshAgent.velocity != Vector3.zero);
            lookDirection = Vector3.RotateTowards(transform.forward, navMeshAgent.velocity, faceTurnSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    // При столкновении убиваем овцу
    private void OnCollisionEnter(Collision collision)
    {   
        if (collision.gameObject.GetComponent<SheepBehaviour>() != null)
        {
            collision.gameObject.GetComponent<SheepBehaviour>().KillSheep(transform, wolfKnockbackForce);
            sheepHuntList.Remove(collision.transform);
            SheepKillEffects();
        }
    }    
   
   private void SheepKillEffects()
    {
        SoundManager.instance.PlaySound("Bark");
    }


}
