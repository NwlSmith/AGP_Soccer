using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServicesLocator
{
    public static GameManager gameManager { get; private set; }
    public static AILifecycleManager aILifecycleManager { get; private set; }
    public static Transform pawnHolder { get; private set; }
    public static Rigidbody ball { get; private set; }
    public static PlayerControl playerControl { get; private set; }

    public static void InitializeServices(GameManager gm, Rigidbody ballRB)
    {
        gameManager = gm;
        ball = ballRB;
        aILifecycleManager = new AILifecycleManager();
    }
}

public class AILifecycleManager
{
    /*
    [SerializeField] private Transform[] pawnStartPositions;
    [SerializeField] private Pawn pawnPrefab;*/
    private List<Pawn> pawns = new List<Pawn>();

    public void Start()
    {
        foreach (Transform trans in ServicesLocator.gameManager.pawnStartPositions)
        {
            Pawn newPawn = Object.Instantiate(ServicesLocator.gameManager.pawnPrefab, trans.position, trans.rotation, ServicesLocator.pawnHolder);
            pawns.Add(newPawn);
        }
    }

    public void Update()
    {
        foreach (Pawn pawn in pawns)
        {
            // Move toward ball.
            if (!pawn.isPlayer)
                pawn.AIMove(CalculateAIMovement(pawn));
        }
    }

    private Vector3 CalculateAIMovement(Pawn pawn)
    {
        // This is meant to find the correct direction on the XZ plane, and maintain Y at 1, but I don't think this does exactly that.
        Vector3 directionVector = ServicesLocator.ball.position - pawn.transform.position;
        directionVector.Normalize();
        directionVector.y = 0;
        return directionVector;
    }
}

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    public Transform[] pawnStartPositions;
    public Pawn pawnPrefab;
    public Rigidbody ball;

    private void Awake()
    {
        // Ensure that there is only one instance of the ServiceLocator.
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        ServicesLocator.InitializeServices(this, ball);
    }

    // Start is called before the first frame update
    void Start()
    {
        ServicesLocator.aILifecycleManager.Start();
    }

    // Update is called once per frame
    void Update()
    {
        ServicesLocator.aILifecycleManager.Update();
    }
}
