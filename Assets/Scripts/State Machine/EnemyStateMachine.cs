using UnityEngine;
using UnityEngine.AI;
using WaveSystem.Enemies;

namespace WaveSystem.StateMachine
{
    public class EnemyStateMachine : BaseStateMachine
    {
        private NavMeshAgent _navMeshAgent;
        private Enemy _enemy;
        private MonoBehaviour _enemyMonoBehaviour;

        public RoamingState RoamingState { get; private set; }
        public IdleState IdleState { get; private set; }

        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        public Enemy Enemy => _enemy;
        public MonoBehaviour MonoBehaviour => _enemyMonoBehaviour;

        public EnemyStateMachine(NavMeshAgent navMeshAgent, MonoBehaviour transform)
        {
            _navMeshAgent = navMeshAgent;
            _enemy = transform.GetComponent<Enemy>();
            _enemyMonoBehaviour = transform;

            RoamingState = new RoamingState(this);
            IdleState = new IdleState(this);
        } 
    }
}