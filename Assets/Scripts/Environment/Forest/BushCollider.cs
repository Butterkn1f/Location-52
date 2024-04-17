using Characters.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Forest
{
    public class BushCollider : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnTriggerStay(Collider other)
        {
            if (transform.localScale.x < 50) return;

            if (PlayerManager.Instance.Movement.IsHidden == false)
            {
                PlayerManager.Instance.Movement.HidePlayer(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (transform.localScale.x < 50) return;

            PlayerManager.Instance.Movement.HidePlayer(false);
        }
    }
}
