using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BNB
{
    public class EffectManager : MonoBehaviour
    {
        public Effect getEffect(int index)
        {
            return Effects[index];
        }

        [SerializeField]
        private List<Effect> _Effects;

        public List<Effect> Effects
        {
            get {
                return _Effects;
            }
        }

        [SerializeField] private GameObject _EmptyEffect;

        public Effect EmptyEffect
        {
            get {
                var effect = _EmptyEffect?.GetComponent<Effect>();
                return effect;
            }
        }
    }
}
