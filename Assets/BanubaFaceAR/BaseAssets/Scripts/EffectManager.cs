using System.Collections.Generic;
using UnityEngine;

namespace BNB
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager instance;

        [SerializeField]
        private List<Effect> _effects;
        [SerializeField]
        private Effect _emptyEffect;
        private Effect _currentEffect;

        public List<Effect> Effects => _effects;

        private void Awake()
        {
            if (instance == null) {
                instance = this;
            }
            _currentEffect = _emptyEffect;
        }

        public void SpawnEffect(Effect effect)
        {
            if (_emptyEffect != null) {
                Destroy(_emptyEffect.gameObject);
            }
            if (_currentEffect != null) {
                DestroyImmediate(_currentEffect.gameObject);
            }
            Effect newEffect = Instantiate(effect, transform);
            newEffect.gameObject.SetActive(true);
            _currentEffect = newEffect;
        }
    }
}
