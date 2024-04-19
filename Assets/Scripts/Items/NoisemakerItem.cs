using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

namespace Mobs
{
    public class NoisemakerItem : Item
    {
        public const float honkTime = 1.0f;

        AudioSource honkSfx;
        SphereCollider sphereCollider;

        IEnumerator lifetimeCoroutine;

        protected override void Start()
        {
            base.Start();
            honkSfx = GetComponent<AudioSource>();
            sphereCollider = GetComponent<SphereCollider>();
        }

        protected override void AssignControls()
        {
            base.AssignControls();
            _controls.MainGameplay.UseItem.performed += UseItem;
        }

        protected override void UnassignControls()
        {
            _controls.MainGameplay.UseItem.performed -= UseItem;
        }

        protected override void OnInactive()
        {
            base.OnInactive();
            if (lifetimeCoroutine != null)
                StopCoroutine(lifetimeCoroutine);
            sphereCollider.enabled = false;
        }

        private void UseItem(InputAction.CallbackContext context)
        {
            if (lifetimeCoroutine != null)
                StopCoroutine(lifetimeCoroutine);
            honkSfx.Play(0);
            sphereCollider.enabled = true;

            lifetimeCoroutine = Lifetime();
            StartCoroutine(lifetimeCoroutine);
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
            yield return new WaitForSeconds(honkTime);
            sphereCollider.enabled = false;
        }
    }
}

