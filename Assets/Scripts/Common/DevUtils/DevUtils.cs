using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


/// <summary>
/// This class is basically just a bunch of shortcuts that I can use to make my life easier 
/// </summary>
public static class DevUtils
{
    /// <summary>
    /// Basically if the condition is met, then we display the message
    /// Good for finding edge points
    /// </summary>
    public static bool AssertTrue(bool condition, string message, LogType DebugLogType = LogType.Warning)
    {
        if (condition == true)
        {
            switch (DebugLogType)
            {
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Warning:
                default:
                    Debug.LogWarning(message);
                    break;
            }
        }

        return condition;
    }

    /// <summary>
    /// Basically if the condition is not met, then we display the message
    /// </summary>
    public static bool AssertFalse(bool condition, string message, LogType DebugLogType = LogType.Warning)
    {
        if (condition == false)
        {
            switch (DebugLogType)
            {
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Warning:
                default:
                    Debug.LogWarning(message);
                    break;
            }


        }

        return condition == false;
    }
}

[System.Serializable]
public class Wrapper<T>
{
    public T[] array;
}
