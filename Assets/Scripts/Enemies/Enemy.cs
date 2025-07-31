using UnityEngine;
using WaveSystem.Utilities;
using WaveSystem.StateMachine;
using UnityEngine.AI;

namespace WaveSystem.Enemies
{
    public class Enemy : MonoBehaviour
    { 
        [SerializeField] private EnemyData _enemyData;

        private EnemyStateMachine _stateMachine;

        private WaveSystemManager waveManager;

        private Animator _animator;

        public EnemyData EnemyData => _enemyData;
        public Animator Animator => _animator;
        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void Start()
        {
            waveManager = FindAnyObjectByType<WaveSystemManager>();
            _enemyData.Health = _enemyData.MaxHealth;

            _stateMachine = new EnemyStateMachine(transform.GetComponent<NavMeshAgent>(), this);

            _stateMachine.ChangeState(_stateMachine.RoamingState);
        }

        public void TakeDamage(float damage)
        {
            _enemyData.Health -= damage;

            if (_enemyData.Health <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            // Notify wave manager
            if (waveManager != null)
            {
                waveManager.OnEnemyDestroyed(gameObject);
            }

            ObjectPool.Instance.ReturnToPool(gameObject);
        }

        void OnEnable()
        {
            _enemyData.Health = _enemyData.MaxHealth;
        }

        void FixedUpdate()
        {
            _stateMachine?.FixedUpdate();
        }

        void Update()
        {

            _stateMachine?.Update();

            // Test: Press Space to damage enemy
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TakeDamage(25f);
            }
        }
    }

    [System.Serializable]
    public struct EnemyData
    {
        public float Health;
        public float MaxHealth;

        public float movementSpeed;
        public float rotationSpeed;
        public float idleTime;
        public float roamingRadius;


    }
}
