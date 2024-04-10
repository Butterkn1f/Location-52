using ChatSys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom data containers/Chat List")]
public class ChatList : ScriptableObject
{
    // TODO: make this more modular and extendible
    public string ChatFileName;

    // Custom container to store all the chat nodes
    public List<ChatNode> ChatNodeList;

    private void Awake()
    {
        ChatNodeList.Clear();

        if (DevUtils.AssertTrue(ChatFileName != "", "Please enter a filename to read the CSV files"))
        {
            CSVReader.Instance.ReadCSV(this);
        }
    }

    private void OnDestroy()
    {
        ChatNodeList.Clear();
    }
}