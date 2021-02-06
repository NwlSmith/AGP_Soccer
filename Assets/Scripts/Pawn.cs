using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Creator: Nate Smith
 * Date: 2/5/2021
 * Description: Class for each soccer player. Can be moved either by the player or by the AI system.
 */
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
    [SerializeField] private float movementForce = 20f;
    [SerializeField] private float kickForceHorizontal = 10f;
    [SerializeField] private float kickForceVertical = 2f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector2 moveDirection)
    {
        // May be a better way than to make a new vector every time
        Vector3 movementVector = new Vector3(moveDirection.x, 0, moveDirection.y) * movementForce * Time.fixedDeltaTime;

        // Actually move
        rb.AddForce(movementVector, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ball"))
            return;

        Kick();
    }

    private void Kick()
    {
        // Similar issue with CalculateAIMovement() in AILifetimeController, I don't think this calculates what I want it to.
        Vector3 forceDirection = ServicesLocator.ball.position - transform.position;
        forceDirection.y = 0f;
        forceDirection.Normalize();
        forceDirection *= kickForceHorizontal;
        forceDirection.y = kickForceVertical;

        ServicesLocator.ball.AddForce(forceDirection, ForceMode.Impulse);
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked " + name);
        //ServicesLocator.playerControl.
    }
}
