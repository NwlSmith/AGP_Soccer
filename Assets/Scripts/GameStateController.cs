using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController
{
    private FiniteStateMachine<GameStateController> _fsm;

    public void Start()
    {
        _fsm = new FiniteStateMachine<GameStateController>(this);
        _fsm.TransitionTo<StartMenu>();
    }

    public void Update()
    {
        _fsm.Update();
    }

    #region States

    private abstract class GameState : FiniteStateMachine<GameStateController>.State { }

    private class StartMenu : GameState
    {
        public override void OnEnter()
        {
            // display menu
        }

        public override void Update()
        {
            base.Update();
            // Somehow detect button press?

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TransitionTo<StartGame>();
            }
        }

        public override void OnExit()
        {
            // get rid of buttons
            if (Services.SceneObjectIndex.startText != null)
                Services.SceneObjectIndex.startText.enabled = false;
        }
    }

    private class StartGame : GameState
    {
        public override void OnEnter()
        {
            // start ai and other things
            Services.AILifecycleManager.Start(); // Doesn't belong here anymore!
            // A loading screen could go here? Would need an update screen to update progress.
            // Initialize score controller?

            TransitionTo<PlayGame>();
        }

        public override void OnExit()
        {

        }
    }

    private class PlayGame : GameState
    {
        public override void OnEnter()
        {
            // start ai and other things
            // register timeup
            // register pause event?
        }

        public override void Update()
        {
            base.Update();
            Services.AILifecycleManager.Update();
            Services.ScoreController.UpdateTime();
            

            if (false)
            {
                TransitionTo<GameOver>();
            }
            else if (false)
            {
                TransitionTo<Pause>();
            }
        }

        public override void OnExit()
        {
            // Pause ai update?
            // unregister timeup event
            // Unregister pause event?
        }

        public void TimeUp()
        {
            TransitionTo<GameOver>();
        }

        // create pause function?
    }

    private class Pause : GameState
    {
        public override void OnEnter()
        {
            // start ai and other things
        }

        public override void Update()
        {
            base.Update();
            // Detect if 

            if (false)
            {
                TransitionTo<PlayGame>();
            }
        }

        public override void OnExit()
        {
            // Pause ai update?
        }
    }

    private class GameOver : GameState
    {
        public override void OnEnter()
        {
            Services.AILifecycleManager.Destroy();
            // end ai and other things
        }

        public override void Update()
        {
            base.Update();
            // Detect if 

            if (false)
            {
                TransitionTo<StartGame>();
            }
        }

        public override void OnExit()
        {
            // Quit?
        }
    }

    #endregion
}
