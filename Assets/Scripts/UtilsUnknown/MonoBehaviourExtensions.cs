using System.Collections;
using UnityEngine;

namespace UtilsUnknown.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static Coroutine DelayedCall(this MonoBehaviour mono, System.Action method, float delay)
        {
            return mono.StartCoroutine(DelayedCallCoroutine(method, delay));
        }

        private static IEnumerator DelayedCallCoroutine(System.Action method, float delay)
        {
            yield return new WaitForSeconds(delay);
            method();
        }

    }
}
