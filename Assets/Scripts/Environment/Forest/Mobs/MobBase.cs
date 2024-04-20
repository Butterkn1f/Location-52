using Characters.Player;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using Environment.Forest;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace Mobs
{
    /// <summary>
    /// Base class for the mobs to be at
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class MobBase : MonoBehaviour
    {

        protected ReactiveProp<MobState> _characterState = new ReactiveProp<MobState>();

        // Current point of interest
        // This is where they are currently focused on right now
        // Changes based on their environments
        public Vector3 CurrentPointOfInterest;

        // Where the NPCs have said they last saw the player
        public Vector3 LastSeenThreatPosition;

        [Space(5)]
        [Header("Colliders")]
        // View collider
        // Simulates the NPC's vision
        [SerializeField] private MobViewCollider _viewCollider;
        [SerializeField] public LayerMask NpcViewLayerMask;

        #region private variables

        protected bool _isMoving;

        // Whether they are currently investigating
        protected bool _isInvestigating;
        protected bool _isSeeingPlayer;

        // Current player speed
        protected float _speed;

        // private variables
        protected Animator _animator;
        protected NavMeshAgent _navmeshAgent;
        protected CapsuleCollider _capsuleCollider;
        protected Rigidbody _rigidbody;

        private float _animatorSpeed;

        #endregion

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _navmeshAgent = GetComponent<NavMeshAgent>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected void InitialiseVariables()
        {
            _isMoving = false;
            _isSeeingPlayer = false;

            _speed = _navmeshAgent.speed;
            _animatorSpeed = _animator.speed;

            _viewCollider.SetMobBase(this);

            FindNewPosition();
        }

        public void FindNewPosition()
        {
            bool positionFound = false;

            while (!positionFound)
            {
                // Find a new position and walk to it
                // Generate a random point around the player between the given coordinates 
                Vector3 randomPosition = new Vector3(Random.Range(MobSpawnManager.Instance.SpawnArea.bounds.min.x, MobSpawnManager.Instance.SpawnArea.bounds.max.x), 0, Random.Range(MobSpawnManager.Instance.SpawnArea.bounds.min.z, MobSpawnManager.Instance.SpawnArea.bounds.max.z));

            
                // Check that it's not too far from the current point
                float distanceFromPlayer = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), randomPosition);
                if (distanceFromPlayer < 20)
                {
                    // Not too far away, let's walk in this direction
                    GoToPoint(randomPosition);
                    positionFound = true;
                }
            }
        }

        public void GoToPoint(Vector3 destination)
        {
            // Move to the new waypoint
            _navmeshAgent.SetDestination(destination);
            ChangePointOfInterest(destination);

            _animator.CrossFade("Walking", 0.25f);
            _navmeshAgent.isStopped = false;
            _isMoving = true;
        }

        /// <summary>
        /// Check if has reached destination waypoint
        /// </summary>
        protected void CheckIfReachedWaypoint()
        {
            // Check if the player has reached its destination
            if (_navmeshAgent.remainingDistance < 0.5f && _isMoving)
            {
                // Stop moving
                _navmeshAgent.isStopped = true;
                _isMoving = false;

                // Change to idle
                _animator.CrossFade("Idle", 0.15f);

                // Take a little break
                float pauseDuration = Random.Range(0, 20);
                StartCoroutine(Break(pauseDuration));

                ReachDestinationEvent();
            }
        }

        private IEnumerator Break(float breakDuration)
        {
            yield return new WaitForSeconds(breakDuration);

            if (_characterState.GetValue() == MobState.IDLE)
            {
                FindNewPosition();
            }
        }

        /// <summary>
        /// Change their point of interest
        /// </summary>
        public void ChangePointOfInterest(Vector3 newPOI)
        {
            CurrentPointOfInterest = newPOI;
            this.transform.DORotate(new Vector3(0, Quaternion.LookRotation(newPOI - this.transform.position).eulerAngles.y, 0), 0.05f);
        }

        /// <summary>
        /// Setter for the character state
        /// </summary>
        public void SetCurrentCharacterState(MobState newcharacterState)
        {
            _characterState.SetValue(newcharacterState);
        }

        /// <summary>
        /// Getter for the character state
        /// </summary>
        public MobState GetCurrentCharacterState()
        {
            return _characterState.GetValue();
        }

        /// <summary>
        /// Behaviour that it's supposed to have when looking at something they are scared of 
        /// </summary>
        public virtual void NoticeThreat(Vector3 position)
        {
        }

        /// <summary>
        /// Behaviour that it's supposed to have when looking at something they are scared of 
        /// </summary>
        public virtual void NoticeReward(Vector3 position)
        {
        }

        /// <summary>
        /// What to do when reacting to a suspicious audio
        /// </summary>
        public virtual void ReactToSuspiciousAudio(Vector3 position)
        {
        }

        public virtual void ReachDestinationEvent()
        {
        }

        /// <summary>
        /// Behaviour that it's supposed to have when the threat leaves their vicinity
        /// </summary>
        public virtual void RemoveThreat(Vector3 position)
        {

        }
    }

    public enum MobState
    {
        // Default State
        IDLE,
        // Investigating
        SUSPICIOUS,
        // Scared
        ALERT,
        // Prolly unused..  but for when they faint lol
        UNCONSIOUS
    }
}
