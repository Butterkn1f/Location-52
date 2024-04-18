using Characters.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mobs
{
    /// <summary>
    /// This is the collider which is used to simulate the characters vision
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class MobViewCollider : MonoBehaviour
    {
        // Main collider
        private BoxCollider _collider;
        private MobBase _mobBase;

        public void SetMobBase(MobBase mobBase)
        {
            _mobBase = mobBase;
        }

        private void OnTriggerStay(Collider other)
        {
            // Check if the NPC has spotted the player
            if (other.gameObject.CompareTag("Player"))
            {
                Vector3 NPCPos = new Vector3(_mobBase.transform.position.x, _mobBase.transform.position.y + 1.0f, _mobBase.transform.position.z);
                Vector3 ItemPos = new Vector3(other.transform.position.x, PlayerManager.Instance.Camera.gameObject.transform.position.y, other.transform.position.z);
                Vector3 targetDir = ItemPos - NPCPos;

                // do raycast to player to make sure nothing is blocking it
                Debug.DrawRay(NPCPos, targetDir);

                if (Physics.Raycast(NPCPos, targetDir, out RaycastHit hit, 100, _mobBase.NpcViewLayerMask))
                {
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        if (PlayerManager.Instance.Movement.IsHidden)
                        {
                            _mobBase.RemoveThreat(other.transform.position);
                            return;
                        }


                        // As a guard => Register player as a threat if trespassing
                        _mobBase.NoticeThreat(other.transform.position);
                        return;
                    }
                }

                //Debug.Log("Removed player");
                _mobBase.RemoveThreat(other.transform.position);
            }
        }


        private void OnTriggerExit(Collider other)
        {
            // If the player has gone outside of the view box, start the count to become alert
            // Check if the player still has line of sight to the player
            // Also check if the player still has their weapon out
            // If yes, then still be scared
            if (other.gameObject.CompareTag("Player"))
            {
                _mobBase.RemoveThreat(other.transform.position);
            }
        }
    }
}
