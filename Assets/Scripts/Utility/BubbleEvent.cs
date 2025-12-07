using UnityEngine;
using UnityEngine.Events;

namespace RPG.Utility
{
    public class BubbleEvent : MonoBehaviour
    {
        public event UnityAction OnBubbleStartAttack = () => { };
        public event UnityAction OnBubbleCompleteAttack = () => { };
        public event UnityAction OnBubbleHit = () => { };
        public event UnityAction OnBubbleCompleteDefeat = () => { };

        private void OnStartAttack()
        {
            OnBubbleStartAttack.Invoke();
        }

        private void OnCompleteAttack()
        {
            OnBubbleCompleteAttack.Invoke();
        }

        private void OnHit()
        {
            OnBubbleHit.Invoke();
        }

        private void OnCompleteDefeat()
        {
            OnBubbleCompleteDefeat.Invoke();
        }
    }
}
