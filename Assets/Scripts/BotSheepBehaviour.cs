using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotSheepBehaviour : SheepBehaviour
{
    // Двигается ли овца 
    private bool moving; 
    // скорость повора овцы в сторону движения   
    [SerializeField]
    private float faceTurnSpeed = 3; 
    // куда смотрит овца
    private Vector3 lookDirection; 
    // скорость овцы   
    [SerializeField]
    private float speed = 3;
    [SerializeField]
    private float maxVelocityChange = 0.1f;
    // радиус в котором овца может выбрать точку для роама
    private float roamRadius = 3;
    // Верхний крайн арены
    private float roamYPlane;
    private Vector3 gameFieldPosition;
    // Точка к которой движемся
    private Vector3 moveDestination;  
    private float stoppingDistance=3;
    private Vector3 MoveVector;
    //как сильно толкает игрока
    [SerializeField]
    int playerKnockBack = 6;

    // текущий мод поведения овцы    
    public BotSheepBehaviourMode currentBehaviour = BotSheepBehaviourMode.roaming; 

    public enum BotSheepBehaviourMode
    {
        // овца бродит бесцельно
        roaming = 0,
        // овца бежит к точке спасения
        runningToSavePoint = 1,
        // навигация отключена
        disabled = 2 
    }    

    public void SetCurrentShipBehaviour(BotSheepBehaviourMode behaviour)
    {
        currentBehaviour = behaviour;
        switch (currentBehaviour)
        {
            case BotSheepBehaviourMode.roaming:
                {
                    moveDestination= SetRoamingPoint();
                    moving = true;
                    break;
                }

            case BotSheepBehaviourMode.runningToSavePoint:
                {
                    moveDestination = GameObject.FindGameObjectWithTag("Rescue").transform.position;
                    moving = true;
                    break;
                }
            case BotSheepBehaviourMode.disabled:
                {
                    moving = false;
                    break;
                }
        }
    }

    private void Start()
    {
        roamRadius = GameObject.FindGameObjectWithTag("GameField").GetComponent<MeshRenderer>().bounds.extents.x;
        gameFieldPosition = GameObject.FindGameObjectWithTag("GameField").transform.position;
        roamYPlane= gameFieldPosition.y + GameObject.FindGameObjectWithTag("GameField").GetComponent<MeshRenderer>().bounds.size.y/2;        
        moveDestination = SetRoamingPoint();
        Alive = true;
    }
       
    // Установка случайной точки движения для овцы
    private Vector3 SetRoamingPoint()
    {        
        Vector3 randomPosition;
        randomPosition =  Random.insideUnitCircle * roamRadius;
        randomPosition += gameFieldPosition;
        Vector3 finalRoamPosition = new Vector3(randomPosition.x, roamYPlane, randomPosition.y);        
        return finalRoamPosition;
    }

    // добежала ли овца до своей цели
    protected bool pathComplete() 
    {
        if (Vector3.Distance(moveDestination,transform.position) <= stoppingDistance)
        {           
                return true;           
        }
        
        return false;
    }

    public override void Ram()
    {
        if (rammingPossible)
        {            
            rammingPossible = false; // отключаем возможность тарана и посылаем его на кд
            StartCoroutine(RammingCoolDown(rammingCoolDownTime));
        }
    }    

    public override void KillSheep()
    {
        Alive = false;
        moving = false;
        DeactivateSheepWithDelay();
        rigidBody.freezeRotation = false;       
    }

    public override void KillSheep(Transform killerTransform, float knockbackForce) 
    {
        KillSheep();
        rigidBody.AddForce(Vector3.up * 1000, ForceMode.Impulse);
        rigidBody.AddForce((transform.position - killerTransform.position) * knockbackForce, ForceMode.Impulse);      
    }

    private void Move()
    {
        MoveVector = (moveDestination - transform.position).normalized;         

        Vector3 targetVelocity = MoveVector;       
        targetVelocity *= speed;

        // Применяем силу не превыщающую макс ускорение
        Vector3 velocity = rigidBody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
        rigidBody.AddForce(velocityChange, ForceMode.VelocityChange);

        //lookDirection = Vector3.RotateTowards(transform.forward, MoveVector, faceTurnSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(moveDestination-transform.position);
      

    }
  
    // Update is called once per frame
    void Update()
    {
        switch (currentBehaviour) {
            case BotSheepBehaviourMode.roaming:
                {
                    Move();
                    if (pathComplete())
                    {
                      moveDestination = SetRoamingPoint();                        
                    }
                    break;
                }
            case BotSheepBehaviourMode.runningToSavePoint:
                {
                    Move();
                    moveDestination = new Vector3 (GameObject.FindGameObjectWithTag("Rescue").transform.position.x,roamYPlane, GameObject.FindGameObjectWithTag("Rescue").transform.position.z);
                    break;
                }
            case BotSheepBehaviourMode.disabled:
                {
                    
                    break;
                }

        }       

        animator.SetBool("Moving", rigidBody.velocity != Vector3.zero);       
       

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "SheepAgent")
        {           
            collision.gameObject.GetComponent<SheepBehaviour>().Bump(transform);
            //Bump(collision.transform);
        }
        else if (collision.gameObject.tag == "Player")
        {           
            collision.gameObject.GetComponent<SheepBehaviour>().Bump(transform, bumpingForce*playerKnockBack);
            //Bump(collision.transform);
        }
    }
}
