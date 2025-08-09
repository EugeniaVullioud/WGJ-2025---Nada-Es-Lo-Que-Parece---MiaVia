using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class EventBus 
{
    // A dictionary holding lists of event listeners, keyed by event type.
    private static readonly Dictionary<Type, object> listeners = new();

    /// <summary>
    /// Subscribes to an event of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of event to subscribe to. Must be a value type (struct).</typeparam>
    /// <param name="callback">The callback to be invoked when the event is published.</param>
    public static void Subscribe<T>(Action<T> callback) where T : struct
    {
        if (!listeners.TryGetValue(typeof(T), out var list))
        {
            list = new List<Action<T>>();
            listeners[typeof(T)] = list;
        }

        // Add the provided callback to the list of listeners for this event type
        ((List<Action<T>>)list).Add(callback);
    }

    /// <summary>
    /// Unsubscribes from an event of type <typeparamref name="T"/>. 
    /// </summary>
    /// <typeparam name="T">The type of event to unsubscribe from. Must be a value type (struct).</typeparam>
    /// <param name="callback">The callback to remove from the list of listeners for the event.</param>
    public static void Unsubscribe<T>(Action<T> callback) where T : struct
    {
        if (listeners.TryGetValue(typeof(T), out var list))
        {
            // Remove the callback from the list of listeners for this event type
            ((List<Action<T>>)list).Remove(callback);

            // Clean up the list if it's empty
            if (((List<Action<T>>)list).Count == 0)
            {
                listeners.Remove(typeof(T));
            }
        }
    }

    /// <summary>
    /// Publishes an event of type <typeparamref name="T"/> to all subscribed listeners.
    /// </summary>
    /// <typeparam name="T">The type of event to publish. Must be a value type (struct).</typeparam>
    /// <param name="evt">The event to publish.</param>
    public static void Publish<T>(T evt) where T : struct
    {
        if (listeners.TryGetValue(typeof(T), out var list))
        {
            // Invoke each listener callback with the event data
            foreach (var del in (List<Action<T>>)list)  del(evt);
        }
    }
}
