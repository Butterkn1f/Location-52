using ChatSys;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChatSys
{
    [CreateAssetMenu(menuName = "Custom data containers/Chat List")]
    public class ChatList : ScriptableObject
    {
        // TODO: make this more modular and extendible
        public string ChatFileName;

        // Custom container to store all the chat nodes
        public List<ChatNode> ChatNodeList;

        void Awake()
        {
            ChatNodeList.Clear();
        }

        private void OnDestroy()
        {
            ChatNodeList.Clear();
        }

        public List<ChatNode> GetChatNodes(string check_ID)
        {
            // Count how many chat nodes with the ID exist
            int count = GetNumChats(check_ID);
            if (count == 0)
            {
                // if there are no results; display a warning 
                Debug.LogWarning("Requested ID of " + check_ID + " returned no results. Are you sure the ID is correct?");
                return null;
            }

            // get a list of chatnodes, ordered by their order
            var result = ChatNodeList.Where(s => s.ID == check_ID).ToList().OrderBy(s => s.Order);
            return result.ToList();
        }
        

        private int GetNumChats(string ID)
        {
            return ChatNodeList.Count(p => p.ID == ID);
        }
    }
}