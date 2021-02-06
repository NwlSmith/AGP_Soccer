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
    private Pawn targetPawn = null;
    [SerializeField] private SelectionSprite selectionSprite;

    public void IntakeInput(Vector2 playerMovementInput)
    {
        if (!targetPawn) return;

        targetPawn.Move(playerMovementInput);
    }

    public void SetTargetPawn(Pawn clickedPawn)
    {
        if (clickedPawn.Equals(targetPawn)) return;

        if (targetPawn) targetPawn.isPlayer = false;

        targetPawn = clickedPawn;

        targetPawn.isPlayer = true;

        selectionSprite.AlignSpriteToPawn(targetPawn);

        // Add code to make it clear to the player which pawn they are controlling.
    }
}
