using WaveSystem.Utilities;
using UnityEngine;

namespace WaveSystem.StateMachine
{
    public class IdleState : BaseState
    {
        Timer _idleTimer;
        public IdleState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
            _idleTimer = new Timer(StateMachine.Enemy.EnemyData.idleTime);
        }

        public override void Enter()
        {
            StateMachine.Enemy.Animator.CrossFade("Idle", 0.1f);
            
            StateMachine.NavMeshAgent.isStopped = true;
            StateMachine.NavMeshAgent.ResetPath();
            _idleTimer.Start();
        }

        public override void Update()
        {
            _idleTimer.Update(Time.deltaTime);

            // Check if the idle timer has finished, if so, change to RoamingState

            if (!_idleTimer.IsRunning)
            {
                StateMachine.ChangeState(StateMachine.RoamingState);
            }
        }

        public override void Exit()
        {
            StateMachine.NavMeshAgent.isStopped = false;
            StateMachine.NavMeshAgent.ResetPath();
        }
    }
}
