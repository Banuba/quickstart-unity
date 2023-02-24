using UnityEngine;

namespace BNB
{
    /// <summary>
    /// This class realised trigger on tarantula
    /// </summary>
    public class TarantulaTrigger : TriggerObject
    {
        private readonly int IsWalking = Animator.StringToHash("IsWalking");
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        protected override void OnMouthOpen()
        {
            _animator.SetTrigger(IsWalking);
        }
    }
}