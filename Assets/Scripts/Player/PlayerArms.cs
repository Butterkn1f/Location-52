using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerArms : MonoBehaviour
    {
        private Vector3 _currentPosition;
        private Vector3 _currentRotation;

        private Vector3 _weaponSwayOffset;
        private Vector3 _weaponSwayRotationOffset;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, _weaponSwayOffset + _currentPosition, Time.deltaTime * 10);
            this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, (Quaternion.Euler(_weaponSwayRotationOffset) * Quaternion.Euler(_currentRotation)), Time.deltaTime * 10);
        }

        public void SetWeaponSway(Vector3 swayOffset, Vector3 swayRotationOffset)
        {
            _weaponSwayOffset = swayOffset;
            _weaponSwayRotationOffset = swayRotationOffset;
        }
    }
}