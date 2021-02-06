using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServicesLocator
{
    public static GameManager        gameManager        { get; private set; }
    public static AILifecycleManager aILifecycleManager { get; private set; }
    public static Transform          pawnHolder         { get; private set; }
    public static Rigidbody          ball               { get; private set; }
    public static PlayerControl      playerControl      { get; private set; }
    public static InputHandler       inputHandler       { get; private set; }

    public static void InitializeServices(GameManager gm, Rigidbody ballRB)
    {
        gameManager = gm;
        ball = ballRB;
        aILifecycleManager = new AILifecycleManager();
        inputHandler = Object.FindObjectOfType<InputHandler>(); // Is this a good idea? It's possible it won't find InputHandler if it is not initialized before GameObject...
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
            Pawn newPawn = Object.Instantiate(
                ServicesLocator.gameManager.pawnPrefab,
                trans.position,
                trans.rotation,
                ServicesLocator.pawnHolder);

            pawns.Add(newPawn);
        }
    }

    public void FixedUpdate()
    {
        foreach (Pawn pawn in pawns)
        {
            // Move toward ball.
            if (!pawn.isPlayer)
                pawn.Move(CalculateAIMovement(pawn));
        }
    }

    private Vector2 CalculateAIMovement(Pawn pawn)
    {
        // This is meant to find the correct direction on the XZ plane, and maintain Y at 1, but I don't think this does exactly that.
        Vector3 directionVector3D = ServicesLocator.ball.position - pawn.transform.position;
        Vector2 directionVector2D = new Vector2(directionVector3D.x, directionVector3D.z).normalized;
        return directionVector2D;
    }
}

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    public Transform[] pawnStartPositions;
    public Pawn pawnPrefab;
    [SerializeField] private Rigidbody ball;
    //[SerializeField] private InputHandler inputHandler;

    private void Awake()
    {
        // Ensure that there is only one instance of the ServiceLocator.
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        ServicesLocator.InitializeServices(this, ball);
    }

    void Start()
    {
        ServicesLocator.aILifecycleManager.Start();
    }

    void FixedUpdate()
    {
        ServicesLocator.aILifecycleManager.FixedUpdate();
    }
}
