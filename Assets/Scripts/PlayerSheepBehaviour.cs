using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSheepBehaviour : SheepBehaviour
{
    Joystick joystick;
    // скорость повора овцы в сторону движения
    [SerializeField]
    private float faceTurnSpeed = 3; 
    Vector3 lookDirection;
    [SerializeField]
    float maxVelocityChange = 2f;
    [SerializeField]
    private float speed=3;
    private Vector3 MoveVector;
    public bool controlsAreOn=true;

    public override bool Alive
    {
        get
        {
            return alive;
        }
        set
        {
            alive = value;
            if (!alive)
            {               
                GameController.GameIsOn = false;
                controlsAreOn = false;
                GameController.instance.Defeat();
            }

           
        }
    }

    protected override void Awake()
    {
        base.Awake();
        joystick = FindObjectOfType<FixedJoystick>();
        Alive = true;
    }      

    public override void KillSheep()
    {
        Alive = false;        
        DeactivateSheepWithDelay();
        rigidBody.freezeRotation = false;
    }

    public override void KillSheep(Transform killerTransform, float knockbackForce)
    {
        KillSheep();
        rigidBody.AddForce(Vector3.up * 1000, ForceMode.Impulse);
        rigidBody.AddForce((transform.position - killerTransform.position) * knockbackForce, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.GameIsOn || controlsAreOn || Alive)
        {           
            MoveVector = new Vector3(joystick.Horizontal, 0.0f, joystick.Vertical);
            Vector3 targetVelocity = MoveVector * speed;           
            // Применяем силу не превыщающую макс ускорение
            Vector3 velocity = rigidBody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
            rigidBody.AddForce(velocityChange, ForceMode.VelocityChange);
            lookDirection = Vector3.RotateTowards(transform.forward, MoveVector, faceTurnSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(lookDirection);
            animator.SetBool("Moving", rigidBody.velocity != Vector3.zero);
        }

    }
}
