using UnityEngine;

namespace Utility
{
    public class AnimatorUtilities : MonoBehaviour
    {
        //From https://stackoverflow.com/a/50447357
        public static bool IsPlaying(Animator animator, string stateName, int animLayerIndex = 0)
        {
            if (animator.GetCurrentAnimatorStateInfo(animLayerIndex).IsName(stateName) &&
                animator.GetCurrentAnimatorStateInfo(animLayerIndex).normalizedTime < 1.0f)
                return true;
            return false;
        }
    }
}
