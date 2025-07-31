using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace WaveSystem.StateMachine
{
     public class RoamingState : BaseState
    {
        private bool _isMoving;
        private float _lastDestinationTime;
        private const float DestinationUpdateInterval = 3f;
        private bool _needsDestination;

        public RoamingState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            StateMachine.Enemy.Animator.CrossFade("Run", 0.1f);

            RoamingManager.Instance.Register(this);
            _isMoving = false;
            _needsDestination = false;
        }

        public override void Update()
        {
            if (!_isMoving && !_needsDestination)
            {
                _needsDestination = true;
                RoamingManager.Instance.FlagForDestination(this);
            }

            if (_isMoving && !_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= 0.5f)
            {
                // If the agent has reached its destination, stop moving and change to IdleState

                _isMoving = false;
                StateMachine.ChangeState(StateMachine.IdleState);
            }
        }

        public override void Exit()
        {
            RoamingManager.Instance.Unregister(this);
            _isMoving = false;
            _needsDestination = false;
        }

        public bool IsValid => _transform != null && _navMeshAgent != null;

        public Vector3 GetPosition() => _transform.transform.position;

        public void SetDestination(Vector3 destination)
        {
            // Sets a new destination for the NavMeshAgent

            if (!IsValid) return;
            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 15f, NavMesh.AllAreas))
            {
                _navMeshAgent.SetDestination(hit.position);
                _isMoving = true;
            }
            _needsDestination = false;
        }
    }
}
