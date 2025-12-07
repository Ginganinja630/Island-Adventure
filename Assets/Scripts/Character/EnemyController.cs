using System;
using UnityEngine;
using RPG.Utility;
using RPG.Core;
using System.Collections.Generic;

namespace RPG.Character
{
    public class EnemyController : MonoBehaviour
    {
        [NonSerialized] public float distanceFromPlayer;
        [NonSerialized] public Vector3 originalPosition;
        [NonSerialized] public Movement movementCmp;
        [NonSerialized] public GameObject player;
        [NonSerialized] public Patrol patrolCmp;

        [NonSerialized] public bool hasUIOpened = false;

        private Health healthCmp;
        [NonSerialized] public Combat combatCmp;
        public CharacterStatsSO stats;

        public string enemyID = " ";
        public float chaseRange = 2.5f;
        public float attackRange = 0.75f;

        private AIBaseState currentState;
        public AIReturnState returnState = new AIReturnState();
        public AIChaseState chaseState = new AIChaseState();
        public AIAttackState attackState = new AIAttackState();
        public AIPatrolState patrolState = new AIPatrolState();
        public AIDefeatedState defeatedState = new AIDefeatedState();

        private void Awake()
        {

            if (stats == null)
            {
                Debug.LogWarning($"{name} does not have stats.");
            }

            if (enemyID.Length == 0)
            {
                Debug.LogWarning($"{name} does not have an enemy ID.");
            }

            currentState = returnState;

            player = GameObject.FindWithTag(Constants.PLAYER_TAG);
            movementCmp = GetComponent<Movement>();
            patrolCmp = GetComponent<Patrol>();
            healthCmp = GetComponent<Health>();
            combatCmp = GetComponent<Combat>();

            originalPosition = transform.position;
        }

        private void Start()
        {
            currentState.EnterState(this);

            healthCmp.healthPoints = stats.health;
            combatCmp.damage = stats.damage;

            if (healthCmp.sliderCmp != null)
            {
                healthCmp.sliderCmp.maxValue = stats.health;
                healthCmp.sliderCmp.value = stats.health;
            }

            List<string> enemiesDefeated = PlayerPrefsUtility.GetString("EnemiesDefeated");

            enemiesDefeated.ForEach((ID) =>
            {
                if (ID == enemyID)
                {
                    Destroy(gameObject);
                }
            });
        }

        private void OnEnable()
        {
            healthCmp.OnStartDefeated += HandleStartDefeated;
            EventManager.OnToggleUI += HandleToggleUI;
        }

        private void OnDisable()
        {
            healthCmp.OnStartDefeated -= HandleStartDefeated;
            EventManager.OnToggleUI -= HandleToggleUI;
        }

        private void Update()
        {
            CalculateDistanceFromPlayer();

            currentState.UpdateState(this);
        }

        public void SwitchState(AIBaseState newState)
        {
            currentState = newState;
            currentState.EnterState(this);
        }

        private void CalculateDistanceFromPlayer()
        {
            if (player == null) return;

            Vector3 enemyPosition = transform.position;
            Vector3 playerPosition = player.transform.position;

            distanceFromPlayer = Vector3.Distance(
                enemyPosition, playerPosition
            );
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(
                transform.position,
                chaseRange
            );
        }

        private void HandleStartDefeated()
        {
            SwitchState(defeatedState);
            currentState.EnterState(this);
        }

        private void HandleToggleUI(bool isOpened)
        {
            hasUIOpened = isOpened;
        }
    }
}