using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController
{
    private FiniteStateMachine<GameStateController> _fsm;
    public bool isPaused = false;

    public void Start()
    {
        _fsm = new FiniteStateMachine<GameStateController>(this);
        _fsm.TransitionTo<StartMenu>();
    }

    public void Update()
    {
        _fsm.Update();
    }

    public void Destroy()
    {

    }

    #region States

    private abstract class GameState : FiniteStateMachine<GameStateController>.State { }

    private class StartMenu : GameState
    {
        public override void OnEnter()
        {
            Debug.Log("StartMenu Enter");
            Services.UIManager.StartMenu();
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TransitionTo<StartGame>();
            }
        }

        public override void OnExit()
        {
            // get rid of buttons
            Debug.Log("StartMenu Exit");
        }
    }

    private class StartGame : GameState
    {
        public override void OnEnter()
        {
            Debug.Log("StartGame Enter");
            // start ai and other things
            Services.EventManager.Fire(new StartGameEvent());
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
            Services.AILifecycleManager.RemoveTeam();
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
            Services.EventManager.Fire(new PauseEvent());

            Services.GameStateController.isPaused = true;
            ballVelocity = Services.ball.velocity;
            Services.ball.isKinematic = true;
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.P))
            {
                TransitionTo<PlayGame>();
            }
        }

        public override void OnExit()
        {
            Debug.Log("Pause Exit");
            Services.EventManager.Fire(new UnpauseEvent());

            Services.ball.isKinematic = false;
            Services.ball.velocity = ballVelocity;
            Services.GameStateController.isPaused = false;
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // load scene again.
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
