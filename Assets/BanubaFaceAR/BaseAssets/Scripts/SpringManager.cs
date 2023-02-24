using System.Reflection;
using UnityEngine;

namespace BNB
{
    public class SpringManager : MonoBehaviour
    {
        // DynamicRatio is parameter for activated level of dynamic animation
        [Range(0f, 1f)]
        public float dynamicRatio = 1.0f;
        [SerializeField]
        private float dragForce;
        [SerializeField]
        private float stiffnessForce;
        public AnimationCurve dragCurve;
        public AnimationCurve stiffnessCurve;

        private SpringBone[] springBones;

        private void Start()
        {
            springBones = GetComponentsInChildren<SpringBone>();
            UpdateParameters();
        }

        private void Update()
        {
            UpdateParameters();
        }

        private void LateUpdate()
        {
            if (dynamicRatio == 0.0f) {
                return;
            }
            foreach (var springBone in springBones) {
                if (dynamicRatio > springBone.threshold) {
                    springBone.UpdateSpring();
                }
            }
        }

        private void UpdateParameters()
        {
            UpdateParameter("stiffnessForce", stiffnessForce, stiffnessCurve);
            UpdateParameter("dragForce", dragForce, dragCurve);
        }

        private void UpdateParameter(string fieldName, float baseValue, AnimationCurve curve)
        {
            var start = curve.keys[0].time;
            var end = curve.keys[curve.length - 1].time;

            var prop = springBones[0]
                           .GetType()
                           .GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < springBones.Length; i++) {
                if (!springBones[i].isUseEachBoneForceSettings) {
                    var scale = curve.Evaluate(start + (end - start) * i / (springBones.Length - 1));
                    prop.SetValue(springBones[i], baseValue * scale);
                }
            }
        }
    }

}
