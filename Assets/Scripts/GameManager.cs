using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    

    //[SerializeField] private InputHandler inputHandler;

    #region Lifecycle Management
    private void Awake()
    {

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
