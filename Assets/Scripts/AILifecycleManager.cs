using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

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

    #region Lifecycle Management

    public AILifecycleManager() : base()
    {
        Services.EventManager.Register<StartGameEvent>(Start);
    }

    public void Start(NEvent e)
    {
        SpawnTeam(Services.SceneObjectIndex.pawnStartPositionsRed, Services.SceneObjectIndex.pawnPrefabRed, false);
        SpawnTeam(Services.SceneObjectIndex.pawnStartPositionsBlue, Services.SceneObjectIndex.pawnPrefabBlue, true);

        _referee = SpawnPawn(Services.SceneObjectIndex.refereeSpawnTransform, Services.SceneObjectIndex.refereePrefab.GetComponent<Pawn>()).GetComponent<Referee>();

        Services.EventManager.Register<PauseEvent>(OnPause);
        Services.EventManager.Register<UnpauseEvent>(Unpause);
        Services.EventManager.Register<Foul>(OnFoul);
    }

    public void Update()
    {
        foreach (Pawn pawn in pawns)
        {
            // Move toward ball.
            if (!pawn.isPlayer)
                pawn.BehaviorTreeUpdate();
                //pawn.Move(CalculateAIMovement(pawn));
        }
    }

    public void Destroy()
    {
        RemoveTeam();
        Services.EventManager.Unregister<StartGameEvent>(Start);
    }

    #endregion

    #region Spawning and despawning

    private void SpawnTeam(Transform[] pawnStartPositions, Pawn pawnPrefab, bool isBlue)
    {
        Dictionary<int, BehaviorEnum> behaviorDictionary = new Dictionary<int, BehaviorEnum>();
        behaviorDictionary.Add(0, BehaviorEnum.Aggressive);
        behaviorDictionary.Add(1, BehaviorEnum.Aggressive);
        behaviorDictionary.Add(2, BehaviorEnum.Aggressive);
        behaviorDictionary.Add(3, BehaviorEnum.Defensive);
        behaviorDictionary.Add(4, BehaviorEnum.Afraid_of_ball);
        behaviorDictionary.Add(5, BehaviorEnum.Afraid_of_ball);
        behaviorDictionary.Add(6, BehaviorEnum.Defensive);
        behaviorDictionary.Add(7, BehaviorEnum.Defensive);
        behaviorDictionary.Add(8, BehaviorEnum.Goalie);
        for (int i = 0; i < pawnStartPositions.Length; i++)
        {
            Transform trans = pawnStartPositions[i];
            Pawn pawn = SpawnPawn(trans, pawnPrefab);
            pawn.isBlue = isBlue;
            pawn.SetBehaviorTree(behaviorDictionary[i]);
            pawns.Add(pawn);
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

    public void RemoveTeam()
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            if (pawns[i]) pawns[i].Destroy();
            pawns[i] = null;
        }
        pawns = new List<Pawn>();

        _referee.Destroy();

        Services.EventManager.Unregister<PauseEvent>(OnPause);
        Services.EventManager.Unregister<PauseEvent>(Unpause);
    }

    #endregion

    #region Events

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

    #endregion

    #region Utilities

    private Vector2 CalculateAIMovement(Pawn pawn)
    {
        // This is meant to find the correct direction on the XZ plane, and maintain Y at 1, but I don't think this does exactly that.
        Vector3 directionVector3D = Services.ball.position - pawn.transform.position;
        Vector2 directionVector2D = new Vector2(directionVector3D.x, directionVector3D.z).normalized;
        return directionVector2D;
    }

    public bool CanFoulBeCalled()
    {
        return Time.time - _timeAtLastFoul >= _timeBetweenFouls;
    }

    #endregion
}