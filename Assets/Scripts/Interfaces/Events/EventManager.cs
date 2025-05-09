using System;
using System.Collections.Generic;

public static class EventManager
{
    private static Dictionary<Type, Delegate> eventTable = new ();

    public static void Subscribe<T>(Action<T> listener) where T : GameEvent
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

    public static void Unsubscribe<T>(Action<T> listener) where T : GameEvent
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

    public static void Publish<T>(T publishedEvent) where T : GameEvent
    {
        if (eventTable.TryGetValue(typeof(T), out var existingDelegate))
        {
            var callback = existingDelegate as Action<T>;
            callback?.Invoke(publishedEvent);
        }
    }
}
