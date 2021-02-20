using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

/*
 * Creator: Nate Smith
 * Date: 2/5/2021
 * Description: Class for each soccer player. Can be moved either by the player or by the AI system.
 */
 
public enum BehaviorEnum { Aggressive, Defensive, Afraid_of_ball, Goalie };
public class Pawn : MonoBehaviour
{
    /* maybe make a struct like this?
    private struct Stats
    {
        [SerializeField] private float movementForce = 20f;
        [SerializeField] private float kickForceHorizontal = 10f;
        [SerializeField] private float kickForceVertical = 2f;
    }*/
    public bool isPlayer = false;
    [SerializeField] protected float movementForce = 20f;
    [SerializeField] private float kickForceHorizontal = 10f;
    [SerializeField] private float kickForceVertical = 2f;
    protected Rigidbody rb;
    protected BehaviorTree.Tree<Pawn> _tree;

    public BehaviorEnum behavior; // Set behavior at spawn.

    #region Lifecycle management.

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void SetBehaviorTree(BehaviorTree.Tree<Pawn> tree)
    {
        _tree = tree;
    }

    #endregion

    #region Utilities


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
            Kick();
        else if (collision.gameObject.CompareTag("Pawn") &&
            collision.relativeVelocity.magnitude >= Services.AILifecycleManager.maxCollisionSpeed &&
            Services.AILifecycleManager.CanFoulBeCalled()
            )
        {
            Services.EventManager.Fire(new Foul(this, collision.gameObject.GetComponent<Pawn>()));
        }

    }

    private void Kick()
    {
        // Similar issue with CalculateAIMovement() in AILifetimeController, I don't think this calculates what I want it to.
        Vector3 forceDirection = Services.ball.position - transform.position;
        forceDirection.y = 0f;
        forceDirection.Normalize();
        forceDirection *= kickForceHorizontal;
        forceDirection.y = kickForceVertical;

        Services.ball.AddForce(forceDirection, ForceMode.Impulse);
    }

    #endregion

    #region Events.

    public virtual void Pause()
    {
        rb.isKinematic = true;
    }

    public virtual void Unpause()
    {
        rb.isKinematic = false;
    }

    #endregion

    #region Behavior Tree actions

    public void Move(Vector2 moveDirection)
    {
        // May be a better way than to make a new vector every time
        Vector3 movementVector = new Vector3(moveDirection.x, 0, moveDirection.y) * movementForce * Time.fixedDeltaTime;

        // Actually move
        rb.AddForce(movementVector, ForceMode.VelocityChange);
    }

    #endregion
}

#region Behavior Tree nodes

public class IsCloseToBall : BehaviorTree.Node<Pawn>
{
    private float _distance;

    public IsCloseToBall(float dist)
    {
        _distance = dist;
    }

    public override bool Update(Pawn context)
    {
        return Vector3.Distance(context.transform.position, Services.ball.position) < _distance;
    }
}

public class HasStraightPathToBall : BehaviorTree.Node<Pawn>
{

    public override bool Update(Pawn context)
    {
        return Physics.SphereCast(
            context.transform.position, 
            1f,
            (Services.ball.position - context.transform.position).normalized, 
            out RaycastHit hit
            ) &&
            hit.collider.CompareTag("Ball");
    }
}

#endregion
