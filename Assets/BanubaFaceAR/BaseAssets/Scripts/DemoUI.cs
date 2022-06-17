using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BNB
{
    public class DemoUI : MonoBehaviour
    {
        [SerializeField]
        private EffectManager effectManager;

        [SerializeField]
        private Transform effectsList;

        [SerializeField]
        private Button effectButton;

        [SerializeField]
        private Text FPS;
        [SerializeField]
        private Text EffectName;

        private Effect currentEffect;

        private static Button CreateButton(Button buttonPrefab, Transform tr)
        {
            var button = Object.Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity) as Button;
            var rectTransform = button.GetComponent<RectTransform>();
            rectTransform.SetParent(tr);
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(150, 150);
            rectTransform.localScale = Vector2.one;
            rectTransform.localPosition = Vector3.zero;
            return button;
        }

        private void SelectEffect(Effect effect)
        {
            if (currentEffect) {
                GameObject.DestroyImmediate(currentEffect.gameObject);
            }
            var obj = Instantiate(effect.gameObject, effectManager.transform);

            var fx = obj.GetComponent<Effect>();
            var renderFromTex = GetComponent<RenderFromTexture>();
            renderFromTex.setRenderTexture(fx.RenderResult);
            currentEffect = fx;
            obj.SetActive(true);
            EffectName.text = effect.gameObject.name;
        }
        private void FPSUpdate()
        {
            FPS.text = string.Format("AVG. FPS: {0}", BanubaSDKManager.instance.currentFps.ToString());
        }

        // Start is called before the first frame update
        void Start()
        {
            foreach (Effect effect in effectManager.Effects) {
                var button = CreateButton(effectButton, effectsList);
                if (effect.Icon) {
                    button.image.sprite = Sprite.Create(effect.Icon, new Rect(0, 0, effect.Icon.width, effect.Icon.height), new Vector2(0.5f, 0.5f));
                }

                button.transform.GetChild(0).gameObject.GetComponent<Text>().text = null;

                button.onClick.AddListener(() => SelectEffect(effect));
            }
            currentEffect = effectManager.EmptyEffect as Effect;

            InvokeRepeating(nameof(FPSUpdate), 1, 1);
        }
    }

}
