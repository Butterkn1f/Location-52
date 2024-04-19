using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Mobs
{
    public class Deer : MobBase
    {
        // Start is called before the first frame update
        void Start()
        {
            InitialiseVariables();

            _characterState.GetObservable().Subscribe(newValue =>
            {
                //Debug.Log(GetCurrentCharacterState());

            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {
            CheckIfReachedWaypoint();
        }

        /// <summary>
        /// What to do when reacting to a suspicious audio
        /// </summary>
        public override void NoticeThreat(Vector3 position)
        {
            // Idk
        }

        /// <summary>
        /// Behaviour that it's supposed to have when looking at something they are scared of 
        /// </summary>
        public override void NoticeReward(Vector3 position)
        {
        }

        public override void ReactToSuspiciousAudio(Vector3 position)
        {
        }

        public override void RemoveThreat(Vector3 position)
        {
            base.RemoveThreat(position);
        }
    }
}
