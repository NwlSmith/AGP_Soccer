using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    public Transform[] pawnStartPositionsRed;
    public Transform[] pawnStartPositionsBlue;
    public Pawn pawnPrefabRed;
    public Pawn pawnPrefabBlue;
    [SerializeField] private Rigidbody ball;

    //[SerializeField] private InputHandler inputHandler;

    #region Lifecycle Management
    private void Awake()
    {
        // Ensure that there is only one instance of the ServiceLocator.
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        Services.InitializeServices(this);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Services.GameStateController.Start();
    }

    void Update()
    {
        Services.GameStateController.Update();
    }

    // Not sure this is necessary...
    private void OnDestroy()
    {

    }
    #endregion
}
