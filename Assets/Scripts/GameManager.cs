using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServicesLocator
{
    public static GameManager gameManager { get; private set; }
    public static AILifecycleManager aILifecycleManager { get; private set; }
    public static Transform pawnHolder { get; private set; }
    public static GameObject ball { get; private set; }

}

public class AILifecycleManager
{

    [SerializeField] private Transform[] pawnStartPositions;
    [SerializeField] private Pawn pawnPrefab;
    [SerializeField] private Pawn[] pawns;

    public void Start()
    {
        foreach (Transform trans in pawnStartPositions)
        {
            Pawn newPawn = Object.Instantiate(pawnPrefab, trans.position, trans.rotation, ServicesLocator.pawnHolder);
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

    private Vector3 CalculateAIMovement(Pawn pawn)
    {
        Vector3 directionVector = (ServicesLocator.ball.transform.position - pawn.transform.position).normalized;
        return directionVector * pawn.speed * Time.deltaTime;
    }
}

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    private void Awake()
    {
        // Ensure that there is only one instance of the ServiceLocator.
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
