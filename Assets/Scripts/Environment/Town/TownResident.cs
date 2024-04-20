using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters.Player;
using TMPro;

namespace Environment.Town
{
    public class TownResident : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _speechBubble;
        [SerializeField] private NPC_ID _currentNPCID;

        // Start is called before the first frame update
        void Start()
        {
            _speechBubble.text = TownChatManager.Instance.DialogueRequest(_currentNPCID);
        }

        // Update is called once per frame
        void Update()
        {
            this.gameObject.transform.LookAt(PlayerManager.Instance.Movement.transform);
        }

        
    }
}
