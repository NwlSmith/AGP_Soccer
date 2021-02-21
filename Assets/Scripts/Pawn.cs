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
// Aggressive always goes toward ball
// Defensive tries to get between ball and goal
// Afraid of ball will run away from the ball if it gets too close
// Goalie is like a defensive player, but must stay within certain distance from goal
// Irrational goes perpendicular to the vector of the ball to the goal?
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
    protected Tree<Pawn> _tree;

    private Vector3 _initPos;

    public bool isBlue;

    public BehaviorEnum behavior; // Set behavior at spawn.

    #region Lifecycle management.

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _initPos = transform.position;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void SetBehaviorTree(Tree<Pawn> tree)
    {
        _tree = tree;

        // Defensive
        Tree<Pawn> defensiveTree = new Tree<Pawn>
        (
            new Selector<Pawn>
            (
                new Sequence<Pawn>
                (
                    new BallIsOnMySideOfField(),
                    new IAmOnCorrectSideOfBall(),
                    new MoveTowardBall()
                ),
                new Sequence<Pawn>
                (
                    new BallIsOnMySideOfField(),
                    new Not<Pawn>(new IAmOnCorrectSideOfBall()),
                    new MoveToMySideOfField()
                ),
                new MoveToInitPos()
            )
        );
    }

    public void BehaviorTreeUpdate()
    {
        _tree.Update(this);
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

    private Vector2 TowardBall()
    {
        // This is meant to find the correct direction on the XZ plane, and maintain Y at 1, but I don't think this does exactly that.
        Vector3 directionVector3D = Services.ball.position - transform.position;
        Vector2 directionVector2D = new Vector2(directionVector3D.x, directionVector3D.z).normalized;
        return directionVector2D;
    }

    private Vector2 TowardInitPos()
    {
        // This is meant to find the correct direction on the XZ plane, and maintain Y at 1, but I don't think this does exactly that.
        Vector3 directionVector3D = _initPos - transform.position;
        Vector2 directionVector2D = new Vector2(directionVector3D.x, directionVector3D.z).normalized;
        return directionVector2D;
    }

    public void Move(Vector2 moveDirection)
    {
        // May be a better way than to make a new vector every time
        Vector3 movementVector = new Vector3(moveDirection.x, 0, moveDirection.y) * movementForce * Time.fixedDeltaTime;

        // Actually move
        rb.AddForce(movementVector, ForceMode.VelocityChange);
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

    public void MoveTowardBall() => Move(TowardBall());

    public void MoveToMySideOfField() => Move(isBlue ? new Vector2(-1, 0) : new Vector2(1, 0));

    public void MoveToOtherSideOfField() => Move(isBlue ? new Vector2(1, 0) : new Vector2(-1, 0));

    public void MoveToInitPos() => Move(TowardInitPos());

    #endregion
}

#region Behavior Tree actions

public class MoveTowardBall : Node<Pawn>
{
    public override bool Update(Pawn context)
    {
        context.MoveTowardBall();
        return true;
    }
}

public class MoveToMySideOfField : Node<Pawn>
{
    public override bool Update(Pawn context)
    {
        context.MoveToMySideOfField();
        return true;
    }
}

public class MoveToOtherSideOfField : Node<Pawn>
{
    public override bool Update(Pawn context)
    {
        context.MoveToOtherSideOfField();
        return true;
    }
}

public class MoveToInitPos : Node<Pawn>
{
    public override bool Update(Pawn context)
    {
        context.MoveToInitPos();
        return true;
    }
}

#endregion

#region Behavior Tree conditions

public class IsCloseToBall : Node<Pawn>
{
    private float _distance;
    private float _precision;

    public IsCloseToBall(float dist, float precision)
    {
        _distance = dist;
        _precision = precision;
    }

    public override bool Update(Pawn context)
    {
        float dist = Vector3.Distance(context.transform.position, Services.ball.position);
        return dist < _distance + _precision && dist > _distance - _precision;
    }
}

public class HasStraightPathToBall : Node<Pawn>
{

    public override bool Update(Pawn context) => Physics.SphereCast(
            context.transform.position,
            1f,
            (Services.ball.position - context.transform.position).normalized,
            out RaycastHit hit
            ) &&
            hit.collider.CompareTag("Ball");
}

public class BallHasStraightPathToGoal : Node<Pawn>
{
    public override bool Update(Pawn context) => Physics.SphereCast(
            Services.ball.transform.position,
            1f,
            (((context.isBlue) ? Services.SceneObjectIndex.blueGoal.position : Services.SceneObjectIndex.redGoal.position) - context.transform.position).normalized,
            out RaycastHit hit
            ) &&
            hit.collider.CompareTag("Goal");
}

public class BallIsInMyZone : Node<Pawn>
{
    private Vector3 _upperRight;
    private Vector3 _lowerLeft;

    public BallIsInMyZone(Vector3 ur, Vector3 ll)
    {
        _upperRight = ur;
        _lowerLeft = ll;
    }

    public override bool Update(Pawn context)
    {
        Vector3 ballPos = Services.ball.position;
        return ballPos.x <= _upperRight.x && ballPos.x >= _lowerLeft.x && ballPos.z <= _upperRight.z && ballPos.z >= _lowerLeft.z;
    }
}

// Meaning, if I am on the blue team, I am to the left of the ball.
public class IAmOnCorrectSideOfBall : Node<Pawn>
{
    public override bool Update(Pawn context) => context.isBlue ? Services.ball.position.x > context.transform.position.x : Services.ball.position.x < context.transform.position.x;
}

public class BallIsBetweenMeAndEnemyGoal : Node<Pawn>
{
    public override bool Update(Pawn context)
    {
        Vector3 dir = (Services.ball.position - context.transform.position).normalized;
        if (Physics.SphereCast(context.transform.position, 1f, dir, out RaycastHit hit1) && hit1.collider.CompareTag("Ball"))
        {
            if (Physics.SphereCast(Services.ball.position, 1f, dir, out RaycastHit hit2) && hit2.collider.CompareTag("Goal"))
            {
                if ((hit2.collider.GetComponent<Goal>().isBlue && context.isBlue) || (!hit2.collider.GetComponent<Goal>().isBlue && !context.isBlue))
                    return true;
            }
        }
        return false;
    }
}

public class BallIsOnMySideOfField : Node<Pawn>
{
    public override bool Update(Pawn context) => context.isBlue ? Services.ball.position.x < 0 : Services.ball.position.x > 0;
}


#endregion
