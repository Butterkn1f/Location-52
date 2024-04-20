using Mobs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Environment.Forest
{
    public class DuckAnomaly : MobBase
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
            SetCurrentCharacterState(MobState.ALERT);
            GoToPoint(position);
            _navmeshAgent.speed *= 2;
        }

        /// <summary>
        /// Behaviour that it's supposed to have when looking at something they are scared of 
        /// </summary>
        public override void NoticeReward(Vector3 position)
        {
            ChangePointOfInterest(position);
            GoToPoint(position);
        }

        public override void ReactToSuspiciousAudio(Vector3 position)
        {
            SetCurrentCharacterState(MobState.ALERT);
            ChangePointOfInterest(position);
        }

        public override void RemoveThreat(Vector3 position)
        {
            base.RemoveThreat(position);
            _navmeshAgent.speed /= 2;
        }
    }

}