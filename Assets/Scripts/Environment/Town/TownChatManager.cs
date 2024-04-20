using Characters.Player;
using ChatSys;
using Common.DesignPatterns;
using MainGame;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Environment.Town
{
    /// <summary>
    /// This class manages the towns folk NPC lines
    /// </summary>
    public class TownChatManager : Singleton<TownChatManager>
    {
        [SerializeField] private ChatList _NPCChatList;

        [System.Serializable]
        public class EventChat
        {
            public EventID EventType;
            public string ChatID;
        }

        [SerializeField] private List<EventChat> _eventChatDialogues;
        [SerializeField] private List<string> _defaultDialogueLines;

        // Start is called before the first frame update
        void Start()
        {
            CSVReader.Instance.ReadCSV(_NPCChatList);

            MainGameManager.Instance.RecentGameEvent = EventID.NOBODY_MOVED;
        }

        public string DialogueRequest(NPC_ID NPC)
        {
            List<ChatNode> chats;
            System.Random rand = new System.Random();

            //// Get the dialogue lines based on the latest event
            //if (Random.Range(0, 5) > 2)
            //{
            //    // Display event chat
            //    chats = _NPCChatList.GetChatNodes(_eventChatDialogues.Where(x => x.EventType == MainGameManager.Instance.RecentGameEvent).First().ChatID);
            //}
            //else
            //{
            //    // Get from random list of generic chats
            //}

            chats = _NPCChatList.GetChatNodes(_defaultDialogueLines.OrderBy(_ => rand.Next()).ToList().First());


            // Filter based off NPC name
            string returnText = "";

            if (chats.Where(x => x.BodyText_alt == NPC.ToString()).Count() > 0)
            {
                returnText = chats.Where(x => x.BodyText_alt == NPC.ToString()).OrderBy(_ => rand.Next()).ToList().First().BodyText;
            }

            return returnText;
        }
    }

    public enum NPC_ID
    {
        DEFAULT,
        NPC_EXAMPLE_A,
        NPC_EXAMPLE_B,
    }

    
}
