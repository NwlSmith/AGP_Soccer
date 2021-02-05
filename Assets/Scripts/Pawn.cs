using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void AIMove(Vector3 moveDirection)
    {
        Vector3 movementVector = moveDirection * movementForce * Time.deltaTime;
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
}
