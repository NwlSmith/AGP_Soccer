using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Creator: Nate Smith
 * Creation Date: 2/6/2021
 * Description: A holder for systems that need to be referenced in many different scripts.
 * It is Static, so it does not need to be in the scene to work.
 * Needs to grab other systems.
 * Superior to a singleton instances because they are clumsy.
 * 
 * Using this form of Getters and Setters prevents crashes from null references.
 */

public static class Services
{
    #region Variables
    // Ensures you don't get a null reference exception.
    private static GameManager _gm;
    public static GameManager GameManager
    {
        get
        {
            Debug.Assert(_gm != null);
            return _gm;
        }
        private set => _gm = value;
    }

    private static SceneObjectIndex _soi;
    public static SceneObjectIndex SceneObjectIndex
    {
        get
        {
            Debug.Assert(_soi != null);
            return _soi;
        }
        private set => _soi = value;
    }

    private static AILifecycleManager _ai;
    public static AILifecycleManager AILifecycleManager
    {
        get
        {
            Debug.Assert(_ai != null);
            return _ai;
        }
        private set => _ai = value;
    }

    private static GameStateController _gsc;
    public static GameStateController GameStateController
    {
        get
        {
            Debug.Assert(_gsc != null);
            return _gsc;
        }
        private set => _gsc = value;
    }

    private static Transform _pawnHolder;
    public static Transform pawnHolder
    {
        get
        {
            Debug.Assert(_pawnHolder != null);
            return _pawnHolder;
        }
        private set => _pawnHolder = value;
    }

    private static Rigidbody _ball;
    public static Rigidbody ball
    {
        get
        {
            Debug.Assert(_ball != null);
            return _ball;
        }
        private set => _ball = value;
    }

    private static PlayerControl _pc;
    public static PlayerControl PlayerControl
    {
        get
        {
            Debug.Assert(_pc != null);
            return _pc;
        }
        private set => _pc = value;
    }

    private static InputHandler _input;
    public static InputHandler InputHandler
    {
        get
        {
            Debug.Assert(_input != null);
            return _input;
        }
        private set => _input = value;
    }

    private static EventManager _em;
    public static EventManager EventManager
    {
        get
        {
            Debug.Assert(_em != null);
            return _em;
        }
        private set => _em = value;
    }

    private static ScoreController _sc;
    public static ScoreController ScoreController
    {
        get
        {
            Debug.Assert(_sc != null);
            return _sc;
        }
        private set => _sc = value;
    }

    private static UIManager _uim;
    public static UIManager UIManager
    {
        get
        {
            Debug.Assert(_uim != null);
            return _uim;
        }
        private set => _uim = value;
    }
    
    #endregion

    #region Functions
    public static void InitializeServices(GameManager gm)
    {
        GameManager = gm;
        EventManager = new EventManager();
        SceneObjectIndex = gm.GetComponent<SceneObjectIndex>();
        ball = SceneObjectIndex.ball;
        AILifecycleManager = new AILifecycleManager();
        GameStateController = new GameStateController();
        PlayerControl = Object.FindObjectOfType<PlayerControl>();
        InputHandler = Object.FindObjectOfType<InputHandler>(); // Is this a good idea? It's possible it won't find InputHandler if it is not initialized before GameObject...
        ScoreController = new ScoreController();
        UIManager = new UIManager();
    }

    public static void DestroyServices()
    {
        AILifecycleManager.RemoveTeam();
        GameStateController.Destroy();
        ScoreController.OnDestroy();
        UIManager.OnDestroy();
    }
    #endregion
}