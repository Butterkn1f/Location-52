using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An individual chat node
[System.Serializable]
public class ChatNode
{
    // A 6 character code that is used to identify unique chats
    public string ID;
    public int Order;

    // Main body text that is shown
    public string BodyText;

    // Current mood of the chat
    // Trigger animation based on this
    public int Mood;
}