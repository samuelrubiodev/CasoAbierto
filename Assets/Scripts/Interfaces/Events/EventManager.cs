using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    private static EventManager instance;
    private EventManager() { }

    public static EventManager GetInstance()
    {
        instance ??= new EventManager();
        return instance;
    }

    private static Dictionary<Type, Delegate> eventTable = new ();

    public void Subscribe<T>(Action<T> listener) where T : GameEvent
    {
        if (eventTable.TryGetValue(typeof(T), out var existingDelegate))
        {
            eventTable[typeof(T)] = Delegate.Combine(existingDelegate, listener);
        }
        else
        {
            eventTable[typeof(T)] = listener;
        }
    }

    public void Unsubscribe<T>(Action<T> listener) where T : GameEvent
    {
        if (eventTable.TryGetValue(typeof(T), out var existingDelegate))
        {
            var newDelegate = Delegate.Remove(existingDelegate, listener);
            if (newDelegate == null)
            {
                eventTable.Remove(typeof(T));
            }
            else
            {
                eventTable[typeof(T)] = newDelegate;
            }
        }
    }

    public void Publish<T>(T publishedEvent) where T : GameEvent
    {
        if (eventTable.TryGetValue(typeof(T), out var existingDelegate))
        {
            var callback = existingDelegate as Action<T>;
            callback?.Invoke(publishedEvent);
        }
    }
}
