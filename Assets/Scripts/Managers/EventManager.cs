using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Dictionary<EventKeys, Action<object[]>> eventDictionary;

    private static EventManager eventManager;

    private static EventManager Instance
    {
        get
        {
            if (eventManager)
            {
                return eventManager;
            }

            eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

            if (eventManager)
            {
                eventManager.Init();
            }
            else
            {
                Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
            }

            return eventManager;
        }
    }

    private void Init()
    {
        eventDictionary ??= new Dictionary<EventKeys, Action<object[]>>();
    }

    public static void StartListening(EventKeys eventType, Action<object[]> listener)
    {
        if (Instance.eventDictionary.TryGetValue(eventType, out var thisEvent))
        {
            thisEvent += listener;
        }
        else
        {
            thisEvent = listener;
        }

        Instance.eventDictionary[eventType] = thisEvent;
    }

    public static void StopListening(EventKeys eventType, Action<object[]> listener)
    {
        if (eventManager == null)
        {
            return;
        }

        if (Instance.eventDictionary.TryGetValue(eventType, out var thisEvent))
        {
            thisEvent -= listener;
            Instance.eventDictionary[eventType] = thisEvent;
        }
    }

    public static void TriggerEvent(EventKeys eventType, object[] parameters = null)
    {
        if (!Instance.eventDictionary.TryGetValue(eventType, out var thisEvent))
        {
            return;
        }

        foreach (var singleCast in thisEvent.GetInvocationList().Cast<Action<object[]>>())
        {
            try
            {
                singleCast.Invoke(parameters);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}

public enum EventKeys
{
    OnGridCreated,
    OnBlastRequested,
    OnBlastCompleted,
    OnSwipeDownCompleted,
    OnFillColumnsCompleted,
    OnShuffleRequested,
    OnShuffledGridReady,
    OnShuffleCompleted,
    OnGridResetRequested,
    OnGridResetCompeted,
    OnGameStarted,
}