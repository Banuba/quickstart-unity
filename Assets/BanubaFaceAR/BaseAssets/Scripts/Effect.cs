using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace BNB
{
    public class Effect : MonoBehaviour
    {
        [SerializeField]
        private Texture2D _icon;
        [SerializeField]
        private RenderToTexture _renderResult;

        public Texture2D Icon
        {
            get {
                return _icon;
            }
        private
            set {
                _icon = value;
            }
        }

        public RenderToTexture RenderResult
        {
            get {
                return _renderResult;
            }
        }
    }

}
