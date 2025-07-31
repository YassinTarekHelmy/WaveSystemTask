using UnityEngine;
using UnityEngine.AI;

namespace WaveSystem.StateMachine
{
    public class BaseState : IState
    {
        protected NavMeshAgent _navMeshAgent;
        protected MonoBehaviour _transform;
        private NavMeshAgent _agent;

        public EnemyStateMachine StateMachine { get; private set; }

        public BaseState(EnemyStateMachine stateMachine)
        {
            _navMeshAgent = stateMachine.NavMeshAgent;
            _transform = stateMachine.MonoBehaviour;
            _agent = stateMachine.NavMeshAgent;

            StateMachine = stateMachine;
        }

        public virtual void Enter()
        { 

            Debug.Log($"Entering state: {GetType().Name}");
            
            if (_navMeshAgent != null)
            {
                _navMeshAgent.speed = 3.5f;
                _navMeshAgent.angularSpeed = 120f;
            }
            else
            {
                Debug.LogError("NavMeshAgent is not assigned in BaseState.");
            }
        }
        public virtual void Exit()
        {
            if (_navMeshAgent != null)
            {
                _navMeshAgent.ResetPath();
            }
            else
            {
                Debug.LogError("NavMeshAgent is not assigned in BaseState.");
            }
         }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
    }
}
