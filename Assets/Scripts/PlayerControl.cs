using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Creator: Nate Smith
 * Date: 2/5/2021
 * Description: Processes player input.
 */
public class PlayerControl : MonoBehaviour
{

    private Pawn targetPawn;

    public void IntakeInput(Vector2 playerMovementInput)
    {
        if (!targetPawn) return;

        targetPawn.Move(playerMovementInput);
    }
}
