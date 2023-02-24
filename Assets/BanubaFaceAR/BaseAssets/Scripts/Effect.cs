using UnityEngine;

namespace BNB
{
    public class Effect : MonoBehaviour
    {
        [SerializeField]
        private Sprite _icon;
        public Sprite Icon => _icon;
    }
}
