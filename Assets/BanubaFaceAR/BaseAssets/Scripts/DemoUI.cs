using UnityEngine;
using UnityEngine.UI;

namespace BNB
{
    public class DemoUI : MonoBehaviour
    {
        [SerializeField]
        private Transform _effectsList;
        [SerializeField]
        private Button _effectButton;
        [SerializeField]
        private Text _fps;
        [SerializeField]
        private Text _effectName;
        private Effect _currentEffect;

        private void Start()
        {
            foreach (Effect effect in EffectManager.instance.Effects) {
                if (effect == null) {
                    continue;
                }
                Button button = Instantiate(_effectButton, _effectsList);
                button.image.sprite = effect.Icon;
                button.onClick.AddListener(() => SelectEffect(effect));
            }
            InvokeRepeating(nameof(FPSUpdate), 1, 1);
        }

        private void SelectEffect(Effect effect)
        {
            _effectName.text = effect.name;
            EffectManager.instance.SpawnEffect(effect);
        }
        private void FPSUpdate()
        {
            _fps.text = $"AVG. FPS: {BanubaSDKManager.instance.CurrentFps}";
        }
    }

}
