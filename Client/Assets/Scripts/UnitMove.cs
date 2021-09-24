using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    [SerializeField]
    bool isJump = false;
    [SerializeField]
    bool isMove = false;
    [SerializeField]
    bool onGrounded = false;

    [SerializeField]
    int moveSpeed = 6;
    [SerializeField]
    int jumpPower = 12;

    float maxGroundAngle = 25f;

    [SerializeField]
    float floatDelay;

    [SerializeField]
    Vector3 gravity;

    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    Animator animator;

    float minGroundDotProduct;

    CapsuleCollider[ ] capsuleColliders;

    private void Awake()
    {
        minGroundDotProduct = Mathf.Cos( maxGroundAngle * Mathf.Deg2Rad );
        capsuleColliders = GetComponents<CapsuleCollider>();
    }

    public bool OnGrounded
    {
        get
        {
            return onGrounded;
        }
        set
        {
            onGrounded = value;
        }
    }

    public bool IsMove
    {
        get
        {
            return isMove;
        }
        set
        {
            isMove = value;
        }
    }

    public bool IsJump
    {
        get
        {
            return isJump;
        }
        set
        {
            isJump = value;
        }
    }

    public int MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
        set
        {
            moveSpeed = value;
        }
    }

    public int JumpPower
    {
        get
        {
            return jumpPower;
        }
        set
        {
            jumpPower = value;
        }
    }

    public void OnCollisionEnter( Collision collision )
    {
    }

    private void OnCollisionExit( Collision collision )
    {
    }

    void OnCollisionStay( Collision collision )
    {
        EvaluateCollision( collision );
    }

    void OnTriggerEnter( Collider other )
    {
    }

    void OnTriggerStay( Collider other )
    {
        if ( !rb.IsSleeping() )
        {
            EvaluateSubmergence( other );
        }
    }

    void EvaluateSubmergence( Collider other )
    {
        if ( other.transform.position.y < transform.position.y )
        {
            return;
        }

        Vector3 upAxis = -gravity.normalized;

        if ( isJump )
        {
            if ( rb.velocity.y <= 0 )
            {
                isJump = false;
            }
        }
    }

    void EvaluateCollision( Collision collision )
    {
        if ( rb.velocity.y <= 0 )
        {
            int layer = collision.gameObject.layer;

            for ( int i = 0 ; i < collision.contactCount ; i++ )
            {
                Vector3 normal = collision.GetContact( i ).normal;

                if ( normal.y >= minGroundDotProduct )
                {
                    onGrounded = true;
                    isJump = false;

                    animator.SetFloat( "SpeedY" , 0f );

                    break;
                }
            }
        }
    }

    void UpdateGravity()
    {
        gravity = new Vector3( 0f , -20f , 0f );

        rb.AddForce( gravity , ForceMode.Acceleration );
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        float h = -Input.GetAxis( "Horizontal" );
        float v = -Input.GetAxis( "Vertical" );
        bool jump = Input.GetButton( "Jump" );

        capsuleColliders[ 0 ].enabled = onGrounded;

        if ( h != 0 || v != 0 )
        {
            Vector3 targetVelocity = new Vector3( h , 0f , v );
            targetVelocity *= moveSpeed;

            transform.LookAt( new Vector3( h , 0f , v ) + transform.position );

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = ( targetVelocity - velocity );
            velocityChange.y = 0f;

            rb.AddForce( velocityChange , ForceMode.VelocityChange );

            if ( !isMove )
            {
                isMove = true;
            }
        }
        else
        {
            if ( isMove )
            {
                isMove = false;
            }
        }

        if ( onGrounded )
        {
            if ( jump && !isJump )
            {
                isJump = true;
                onGrounded = false;

                rb.AddForce( new Vector3( 0f , jumpPower , 0f ) , ForceMode.Impulse );

                animator.SetTrigger( "Jump" );

//                 Debug.LogWarning( "jump " );
            }
        }

        animator.SetFloat( "SpeedXZ" , Mathf.Abs( rb.velocity.x ) + Mathf.Abs( rb.velocity.z ) );
        animator.SetFloat( "SpeedY" , rb.velocity.y );

//         Debug.Log( "jump " + rb.velocity.y );

        UpdateGravity();
    }

}
