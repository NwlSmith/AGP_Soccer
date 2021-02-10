using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILifecycleManager
{
    /*
    [SerializeField] private Transform[] pawnStartPositions;
    [SerializeField] private Pawn pawnPrefab;*/
    private List<Pawn> pawns = new List<Pawn>();

    public void Start()
    {
        SpawnTeam(Services.SceneObjectIndex.pawnStartPositionsRed, Services.SceneObjectIndex.pawnPrefabRed);
        SpawnTeam(Services.SceneObjectIndex.pawnStartPositionsBlue, Services.SceneObjectIndex.pawnPrefabBlue);

        Services.EventManager.Register<PauseEvent>(OnPause);
        Services.EventManager.Register<PauseEvent>(Unpause);
    }

    private void SpawnTeam(Transform[] pawnStartPositions, Pawn pawnPrefab)
    {
        foreach (Transform trans in pawnStartPositions)
        {
            Pawn newPawn = Object.Instantiate(
                pawnPrefab,
                trans.position,
                trans.rotation,
                Services.GameManager.transform.parent);

            pawns.Add(newPawn);
        }
    }

    public void Update()
    {
        foreach (Pawn pawn in pawns)
        {
            // Move toward ball.
            if (!pawn.isPlayer)
                pawn.Move(CalculateAIMovement(pawn));
        }
    }

    public void OnPause(NEvent e)
    {
        foreach (Pawn pawn in pawns)
        {
            pawn.Pause();
        }
    }

    public void Unpause(NEvent e)
    {
        foreach (Pawn pawn in pawns)
        {
            pawn.Unpause();
        }
    }

    private Vector2 CalculateAIMovement(Pawn pawn)
    {
        // This is meant to find the correct direction on the XZ plane, and maintain Y at 1, but I don't think this does exactly that.
        Vector3 directionVector3D = Services.ball.position - pawn.transform.position;
        Vector2 directionVector2D = new Vector2(directionVector3D.x, directionVector3D.z).normalized;
        return directionVector2D;
    }

    public void Destroy()
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            if (pawns[i]) pawns[i].Destroy();
            pawns[i] = null;
        }
        pawns = new List<Pawn>();

        Services.EventManager.Unregister<PauseEvent>(OnPause);
        Services.EventManager.Unregister<PauseEvent>(Unpause);
    }
}