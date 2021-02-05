using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{

    public bool isPlayer = false;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float kickForceHorizontal = 10f;
    [SerializeField] private float kickForceVertical = 2f;

    public void AIMove(Vector3 moveDirection)
    {
        Vector3 movementVector = moveDirection * speed * Time.deltaTime;
        // Actually move
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
