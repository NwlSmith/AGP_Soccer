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
            Debug.Log("StartMenu Enter");
            Services.UIManager.StartMenu();
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
            Debug.Log("StartMenu Exit");
            Services.UIManager.StartPlay();
        }
    }

    private class StartGame : GameState
    {
        public override void OnEnter()
        {
            Debug.Log("StartGame Enter");
            // start ai and other things
            Services.AILifecycleManager.Start(); // Doesn't belong here anymore!
            // A loading screen could go here? Would need an update screen to update progress.
            // Initialize score controller?
            Services.ball.position = Services.SceneObjectIndex.ballInitPos;
        }

        public override void Update()
        {
            base.Update();
            TransitionTo<PlayGame>();
        }

        public override void OnExit()
        {
            Debug.Log("StartGame Exit");

        }
    }

    private class PlayGame : GameState
    {
        public override void OnEnter()
        {
            // start ai and other things
            // register timeup
            Services.EventManager.Register<TimeUp>(OnTimeUp);
            Services.EventManager.Register<GoalScored>(OnGoalScored);
            Debug.Log("PlayGame Enter");
        }

        public override void Update()
        {
            base.Update();
            Services.AILifecycleManager.Update();
            Services.ScoreController.UpdateTime();
            
            if (Input.GetKeyDown(KeyCode.P))
            {
                TransitionTo<Pause>();
            }
        }

        public override void OnExit()
        {
            // Pause ai update?
            // unregister timeup event
            // Unregister pause event?

            Services.EventManager.Unregister<TimeUp>(OnTimeUp);
            Services.EventManager.Unregister<GoalScored>(OnGoalScored);
            Debug.Log("PlayGame Exit");
        }

        public void OnTimeUp(NEvent e)
        {
            Debug.Log("OnTimeUp");
            TransitionTo<GameOver>();
        }

        public void OnGoalScored(NEvent e)
        {
            Debug.Log("Goal scored, starting again.");
            Services.AILifecycleManager.Destroy();
            TransitionTo<StartGame>();
        }
    }

    private class Pause : GameState
    {
        private Vector3 ballVelocity;

        public override void OnEnter()
        {
            // start ai and other things
            Debug.Log("Pause Enter");
            Services.UIManager.Pause(); // Turn this into an event!
            Services.AILifecycleManager.Pause();

            ballVelocity = Services.ball.velocity;
            Services.ball.isKinematic = true;
        }

        public override void Update()
        {
            base.Update();
            // Detect if 

            if (Input.GetKeyDown(KeyCode.P))
            {
                TransitionTo<PlayGame>();
            }
        }

        public override void OnExit()
        {
            // Pause ai update?
            Debug.Log("Pause Exit");
            Services.UIManager.Unpause(); // Turn this into an event!
            Services.AILifecycleManager.Unpause();

            Services.ball.isKinematic = false;
            Services.ball.velocity = ballVelocity;
        }
    }

    private class GameOver : GameState
    {
        public override void OnEnter()
        {
            Services.AILifecycleManager.Destroy();
            // end ai and other things
            Debug.Log("GameOver Enter");
            Services.UIManager.GameOver();
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
            Debug.Log("GameOver Exit");
        }
    }

    #endregion
}
