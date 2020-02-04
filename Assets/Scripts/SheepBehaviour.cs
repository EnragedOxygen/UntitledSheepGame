using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public abstract class SheepBehaviour : MonoBehaviour
{
    protected Rigidbody rigidBody;
    protected Animator animator;
    // таранит ли сейчас овца
    protected bool ramming = false;
    // Можно ли сейчас таранить 
    protected bool rammingPossible = true;
    // кд тарана 
    [SerializeField]
    protected float rammingCoolDownTime = 3;
    // Сила с которой овцы ударяются друг о дружку
    protected float bumpingForce = 10000f;

    protected bool alive;
    private bool isSafe = false;
    // Овцы в безопасности на платформе
    public bool IsSafe
    {
        get
        {
            return isSafe;
        }

        set
        {
            isSafe = value;
        }
    }
    // Жива ли овца
    public virtual bool Alive
    {
        get
        {
            return alive;
        }
        set
        {
            
                alive = value;              
           
        }
    }



    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    // Убить овцу
    public abstract void KillSheep();

    // Убить овцу и отбросить ее труп от трансформа
    public abstract void KillSheep(Transform killerTransform, float knockbackForce);

    // Таран овцы
    public virtual void Ram()
    {
        if (rammingPossible)
        {
            rammingPossible = false; // отключаем возможность тарана и посылаем его на кд
            StartCoroutine(RammingCoolDown(rammingCoolDownTime));
        }
    }

    // Метод который вызывает овца у другой овцы если она ее затаранила
    //public abstract void KnockBack(Transform attackerTransform);

    // Отправить таран на кд
    protected IEnumerator RammingCoolDown(float time)
    {       
            yield return new WaitForSeconds(time);
            rammingPossible = true;        
    }

   
   
   
    // Деактивация овцы
    IEnumerator DeactivateSheepWithDelayRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    public void DeactivateSheep()
    {
        gameObject.SetActive(false);
    }

    public void DeactivateSheepWithDelay(float delay=3f)
    {
        StartCoroutine(DeactivateSheepWithDelayRoutine(delay));
    }

    // Когда овцы сталкиваются с друг другом без тарана
    public virtual void Bump(Transform otherBumperTransform)
    {
        rigidBody.AddForce((transform.position - otherBumperTransform.position) * bumpingForce);
    }

    // Когда овцы сталкиваются с друг другом без тарана
    public virtual void Bump(Transform otherBumperTransform, float bumpForce)
    {
        rigidBody.AddForce((transform.position - otherBumperTransform.position) * bumpForce);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "SheepAgent" || collision.gameObject.tag == "Player")
        {           
            SoundManager.instance.PlaySound("Thump");
            collision.gameObject.GetComponent<SheepBehaviour>().Bump(transform);
            //Bump(collision.transform);
        }      
      
    }

    
}
