using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mobs
{
    [RequireComponent(typeof(SphereCollider))]
    public class AudioDistractionCue : MonoBehaviour
    {
        private SphereCollider _collider;

        public float SoundRadius = 1;
        [Min(0)] public float LifeTimeDuration = 1;

        // Start is called before the first frame update
        void Start()
        {
            _collider = GetComponent<SphereCollider>();

            if (LifeTimeDuration > 0)
            {
                StartCoroutine(Lifetime());
            }
        }

        public void SetSoundRadius(float newSoundRadius)
        {
            _collider = GetComponent<SphereCollider>();
            SoundRadius = newSoundRadius;
            _collider.radius = SoundRadius;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out MobBase npc))
            {
                npc.ReactToSuspiciousAudio(this.transform.position);
            }
        }

        private IEnumerator Lifetime()
        {
            yield return new WaitForSeconds(LifeTimeDuration);

            Destroy(this.gameObject);
        }
    }
}
