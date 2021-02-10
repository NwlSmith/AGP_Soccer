using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Creator: Nate Smith
 * Date: 2/6/2021
 * Description: Catches and sends signals for player input.
 */
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

    public void Update()
    {
        playerMovementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Pawn clickedPawn = hit.collider.GetComponent<Pawn>();
                if (!clickedPawn) return;

                
                Services.PlayerControl.SetTargetPawn(clickedPawn);
            }
        }
    }

    private void FixedUpdate()
    {
        Services.PlayerControl.IntakeInput(playerMovementInput);
    }
}
