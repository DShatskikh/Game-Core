using System;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public sealed class EnemyMover : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        
        private Animator _animator;
        private NavMeshAgent _agent;
        private bool _isAnimationStopped;
        public bool IsMove => !(!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance);

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();

            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }

        private void Update()
        {
            if (!_isAnimationStopped && IsMove)
            {
                _animator.CrossFade("Walk", 0);
            }
            
            if (!_isAnimationStopped && !IsMove)
            {
                StopMove();
            }

            if (!_isAnimationStopped)
            {
                _spriteRenderer.transform.localScale =
                    _spriteRenderer.transform.localScale.SetX(transform.position.x > _agent.destination.x ? 1 : -1);
            }
        }

        public void MoveToPoint(Vector2 target)
        {
            _agent.isStopped = false;
            _agent.SetDestination(target);
            _animator.CrossFade("Walk", 0);
            _isAnimationStopped = false;
        }

        public void StopMove()
        {
            _agent.isStopped = true;
            _animator.CrossFade("Idle", 0);
            _isAnimationStopped = true;
        }
    }
}