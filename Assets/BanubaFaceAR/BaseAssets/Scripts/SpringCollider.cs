using UnityEngine;
using System.Collections;

namespace BNB
{
    public class SpringCollider : MonoBehaviour
    {
        public float radius = 0.5f;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }

}