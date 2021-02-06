using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Recieves and reports player input.
public class InputHandler : MonoBehaviour
{
    /*
    // This actually works, but is not useful for me right now.
    [System.Serializable]
    public class MovementInput
    {
        public float inputX;
        public float inputY;
    }
    
    // Create a component field using your custom type.
    //[SerializeField] private MovementInput movementInput;
         */

    [SerializeField] private Vector2 playerMovementInput = Vector2.zero;

    void Update()
    {
        playerMovementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


    }

    private void FixedUpdate()
    {
        ServicesLocator.playerControl.IntakeInput(playerMovementInput);
    }
}
