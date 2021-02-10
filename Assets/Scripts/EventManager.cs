using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Creator: Jack Schlesinger
 * Date Created: 2/9/2021
 * Description: A non-monobehavior event manager.
 * 
 * Useful for notifying a large number of systems of an event occuring.
 * 
 * Things that care about an event occuring (audiomanager, scoremanager etc want to know if a goal is scored) register to be notified when that event happens.
 * These systems will declare they care about an event, and have code to respond (OnGoalScored()) (The function that responds is whatever function you pass into the Register function)
 * The goal will trigger the event system to notify each registered system that the event has occured (Services.Events.Fire<GoalScored>(other.name) where other is the goal)
 * 
 * Jack explaining this: https://www.youtube.com/watch?v=53pWHP9WR-o&list=PLE3rHjk-SWif4JAzb4FB48FPJIr3xXZUi&index=2 at time = about 1:15:00
 * Github: https://github.com/jackschlesinger/AGP_2021/blob/master/Projects/AGP_SoccerExample/Assets/
 * 
 * Advantages:
 * Super modular, easy to read, easy to conceptualize
 * Warnings:
 * Debugging is harder
 * MUST register and unregister for events, otherwise crashes will occur.
 * So when you get rid of a fan who will chear when there is a goal, you must make sure that fan unregisters from OnGoalScored.
 * Easy to Overuse
 * 
 * In Start in the ScoreManager etc. include Services.EventManager.Register<GoalScored>(IncrementTeamScore)
 * BUT you must write an OnDestroy() method which includes Services.EventManager.Unregister<GoalScored>(IncrementTeamScore) otherwise it will crash
 * public void IncrementTeamScore(NEvent e) is what happens in ScoreManager when a goal is scored.
 * 
 * public void IncrementTeamScore(NEvent e)
 * {
 *      var goalScoredWasBlue = ((GoalScored)e).blueTeamScored;
 * }
 * 
 * When the event occurs, Services.EventManager.Fire(new GoalScored(gameObjectName = "Blue Goal")); will fire the event and notify each registered system.
 * 
 * Example NEvent is include at bottom of file.
 * 
 */

public class EventManager
{
    private Dictionary<Type, NEvent.Handler> _registeredHandlers = new Dictionary<Type, NEvent.Handler>();

    public void Register<T>(NEvent.Handler handler) where T : NEvent
    {
        var type = typeof(T);
        if (_registeredHandlers.ContainsKey(type))
        {
            if (!IsEventHandlerRegistered(type, handler))
                _registeredHandlers[type] += handler;
        }
        else
        {
            _registeredHandlers.Add(type, handler);
        }
    }

    public void Unregister<T>(NEvent.Handler handler) where T : NEvent
    {
        var type = typeof(T);
        if (!_registeredHandlers.TryGetValue(type, out var handlers)) return;

        handlers -= handler;

        if (handlers == null)
        {
            _registeredHandlers.Remove(type);
        }
        else
        {
            _registeredHandlers[type] = handlers;
        }
    }

    public void Fire(NEvent e)
    {
        var type = e.GetType();

        if (_registeredHandlers.TryGetValue(type, out var handlers))
        {
            handlers(e);
        }
    }

    public bool IsEventHandlerRegistered(Type typeIn, Delegate prospectiveHandler)
    {
        return _registeredHandlers[typeIn].GetInvocationList().Any(existingHandler => existingHandler == prospectiveHandler);
    }
}

public abstract class NEvent
{
    public readonly float creationTime;

    public NEvent()
    {
        creationTime = Time.time;
    }

    public delegate void Handler(NEvent e);
}

public class GoalScored : NEvent
{
    public readonly bool blueScored;

    public GoalScored(bool blueScored)
    {
        this.blueScored = blueScored;
    }
}