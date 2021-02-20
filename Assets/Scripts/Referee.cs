using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : Pawn // TURN THIS INTO A PAWN!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
{

    private FiniteStateMachine<Referee> _fsm;
    private readonly float _distFromBall = 5f;

    #region Lifecycle Management

    protected override void Awake()
    {
        base.Awake();

        transform.position = Services.SceneObjectIndex.refereeSpawnTransform.position;

        _fsm = new FiniteStateMachine<Referee>(this);
        _fsm.TransitionTo<GoNearBall>();


        Services.EventManager.Register<TimeUp>(OnTimeUp);
        Services.EventManager.Register<GoalScored>(OnGoalScored);

        Services.EventManager.Register<Foul>(OnFoul);
        //Services.EventManager.Register<PauseEvent>(Pause);
    }

    public void Update()
    {
        _fsm.Update();
    }

    public void OnDestroy()
    {
        Services.EventManager.Unregister<TimeUp>(OnTimeUp);
        Services.EventManager.Unregister<GoalScored>(OnGoalScored);
        //Services.EventManager.Unregister<PauseEvent>(Pause);
        //_fsm.Destroy();
    }

    #endregion

    public void OnTimeUp(NEvent e)
    {
        _fsm.TransitionTo<EndState>();
    }

    public void OnGoalScored(NEvent e)
    {
        _fsm.TransitionTo<StartState>();
    }

    //public override void Pause(NEvent e)
    public override void Pause()
    {
        _fsm.TransitionTo<PauseState>();
    }
    
    //public override void Unpause(NEvent e)
    public override void Unpause()
    {
        _fsm.TransitionTo<GoNearBall>();
    }

    public void OnFoul(NEvent e)
    {
        Pawn p1 = ((Foul)e).player1;
        Pawn p2 = ((Foul)e).player2;
        Debug.Log($"Foul between {p1.name} and {p2.name}.");
    }

    #region States

    private abstract class RefState : FiniteStateMachine<Referee>.State { }

    private class StartState : RefState
    {
        public override void OnEnter()
        {
            // Go to start pos
            Context.gameObject.transform.position = Services.SceneObjectIndex.refereeSpawnTransform.position;
            TransitionTo<GoNearBall>();
        }
    }

    private class EndState : RefState
    {
        public override void OnEnter()
        {
            Context.gameObject.transform.position = Services.SceneObjectIndex.refereeSpawnTransform.position;
        }
    }

    private class GoNearBall : RefState
    {

        public override void Update()
        {
            base.Update();
            
            Context.Move(CalculateMoveTowardBall());

            if (Vector3.Distance(Context.transform.position, Services.ball.position) < Context._distFromBall)
            {
                TransitionTo<GoAwayFromBall>();
            }
        }

        private Vector2 CalculateMoveTowardBall()
        {
            Vector3 directionVector3D = Services.ball.position - Context.transform.position;
            Vector2 directionVector2D = new Vector2(directionVector3D.x, directionVector3D.z).normalized;
            return directionVector2D;
        }
    }

    private class GoAwayFromBall : RefState
    {

        public override void Update()
        {
            base.Update();

            Context.Move(CalculateMoveAwayFromBall());

            if (Vector3.Distance(Context.transform.position, Services.ball.position) >= Context._distFromBall + 1f)
            {
                TransitionTo<GoNearBall>();
            }
        }

        private Vector2 CalculateMoveAwayFromBall()
        {
            Vector3 directionVector3D = Context.transform.position - Services.ball.position;
            Vector2 directionVector2D = new Vector2(directionVector3D.x, directionVector3D.z).normalized;
            return directionVector2D;
        }
    }

    private class PauseState : RefState
    {

        public override void OnEnter()
        {
            Context.rb.isKinematic = true;
        }

        public override void OnExit()
        {
            // Pause ai update?
            Context.rb.isKinematic = false;
        }
    }

    #endregion
}
