using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILifecycleManager
{
    /*
    [SerializeField] private Transform[] pawnStartPositions;
    [SerializeField] private Pawn pawnPrefab;*/
    private List<Pawn> pawns = new List<Pawn>();
    private Referee _referee = null;

    // Collision details.
    public readonly float maxCollisionSpeed = 50f;
    private readonly float _timeBetweenFouls = 1f;
    private float _timeAtLastFoul = 0f;

    public AILifecycleManager() : base()
    {
        Services.EventManager.Register<StartGameEvent>(Start);
    }

    public void Start(NEvent e)
    {
        SpawnTeam(Services.SceneObjectIndex.pawnStartPositionsRed, Services.SceneObjectIndex.pawnPrefabRed);
        SpawnTeam(Services.SceneObjectIndex.pawnStartPositionsBlue, Services.SceneObjectIndex.pawnPrefabBlue);

        _referee = SpawnPawn(Services.SceneObjectIndex.refereeSpawnTransform, Services.SceneObjectIndex.refereePrefab.GetComponent<Pawn>()).GetComponent<Referee>();

        Services.EventManager.Register<PauseEvent>(OnPause);
        Services.EventManager.Register<UnpauseEvent>(Unpause);
        Services.EventManager.Register<Foul>(OnFoul);
    }

    private void SpawnTeam(Transform[] pawnStartPositions, Pawn pawnPrefab)
    {
        foreach (Transform trans in pawnStartPositions)
        {
            pawns.Add(SpawnPawn(trans, pawnPrefab));
        }
    }

    private Pawn SpawnPawn(Transform trans, Pawn pawnPrefab)
    {
        return Object.Instantiate(
                pawnPrefab,
                trans.position,
                trans.rotation,
                Services.GameManager.transform.parent);
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
        _referee.Pause();
    }

    public void Unpause(NEvent e)
    {
        foreach (Pawn pawn in pawns)
        {
            pawn.Unpause();
        }
        _referee.Unpause();
    }

    public void OnFoul(NEvent e)
    {
        _timeAtLastFoul = Time.time;
    }

    private Vector2 CalculateAIMovement(Pawn pawn)
    {
        // This is meant to find the correct direction on the XZ plane, and maintain Y at 1, but I don't think this does exactly that.
        Vector3 directionVector3D = Services.ball.position - pawn.transform.position;
        Vector2 directionVector2D = new Vector2(directionVector3D.x, directionVector3D.z).normalized;
        return directionVector2D;
    }

    public void RemoveTeam()
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

    public bool CanFoulBeCalled()
    {
        return Time.time - _timeAtLastFoul >= _timeBetweenFouls;
    }

    public void Destroy()
    {
        RemoveTeam();
        Services.EventManager.Unregister<StartGameEvent>(Start);
    }
}