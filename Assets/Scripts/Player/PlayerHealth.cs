using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] protected float _maxEnergy = 100;
        [SerializeField] protected float _energy;

        [SerializeField] protected float _staminaRechargeRate = 0.1f;

        [SerializeField] protected int _maxHealth = 100;
        [SerializeField] protected int _health;

        public int CurrentHealth { get => _health; protected set => _health = value; }

        public int MaxHealth { get => _maxHealth; protected set => _maxHealth = value; }

        public delegate void TakeDamageEvent(int Damage, Vector3 DamagePosition);
        public event TakeDamageEvent OnTakeDamage;
        public delegate void DeathEvent(Vector3 position);
        public event DeathEvent OnDeath;

        // To call to actually take damage
        public void TakeDamage(int Damage, Vector3 damagePosition)
        {
            SetDamageTaken(Damage, damagePosition);

            if (Damage != 0)
            {
                OnTakeDamage?.Invoke(Damage, damagePosition);
            }
        }

        // If you just want it to take damage without invoking the take damage event
        public void SetDamageTaken(int damageTaken, Vector3 damagePosition)
        {
            damageTaken = Mathf.Clamp(damageTaken, 0, CurrentHealth);
            CurrentHealth -= damageTaken;

            if (CurrentHealth == 0 && damageTaken != 0)
            {
                OnDeath?.Invoke(damagePosition);
            }
        }

        protected void SetFullHealth()
        {
            _health = _maxHealth;
        }

        void Start()
        {
            ResetHealth();

            OnDeath += OnDeathEvent;
            OnTakeDamage += UpdateHealthEvent;
        }

        private void ResetHealth()
        {
            SetFullHealth();
            SetFullEnergy();
        }

        private void UpdateHealthEvent(int Damage, Vector3 damagePosition)
        {
            // TODO: update health UI
        }

        public void IncreaseHealth(int healedAmount)
        {
            CurrentHealth += healedAmount;
        }

        private void LateUpdate()
        {
            RechargeEnergy(_staminaRechargeRate);
        }

        public void UseEnergy(float EnergyConsumed)
        {
            float energyConsumed = Mathf.Clamp(EnergyConsumed, 0, _energy);

            _energy -= energyConsumed;
            _energy = Mathf.Max(0, _energy);

            //_healthUI.UpdateEnergy(_energy, _maxEnergy);
        }

        public float GetEnergy()
        {
            return _energy;
        }

        public void RechargeEnergy(float energyRecharged)
        {
            _energy += energyRecharged;

            _energy = Mathf.Clamp(_energy, 0, _maxEnergy);
            //_healthUI.UpdateEnergy(_energy, _maxEnergy, true);
        }

        protected void SetFullEnergy()
        {
            _energy = _maxEnergy;
        }

        private void OnDeathEvent(Vector3 position)
        {
            // TODO: lose game
        }
    }
}
