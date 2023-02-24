using UnityEngine;

namespace BNB
{
    public class SpringBone : MonoBehaviour
    {
        public bool debug = true;
        public bool isUseEachBoneForceSettings;
        public float threshold = 0.01f;
        public float radius = 0.05f;
        public float dragForce = 0.4f;
        public float stiffnessForce = 0.01f;
        public Vector3 springForce = new Vector3(0.0f, -0.0001f, 0.0f);
        public Vector3 boneAxis = new Vector3(-1.0f, 0.0f, 0.0f);
        public Transform child;
        public SpringCollider[] colliders;

        // Threshold Starting to activate activeRatio
        private float springLength;
        private Quaternion _localRotation;
        private Vector3 _currTipPos;
        private Vector3 _prevTipPos;
        private Transform _org;
        private Transform _transform;
        private SpringManager _managerRef;

        private void Awake()
        {
            _transform = transform;
            _localRotation = _transform.localRotation;
            _managerRef = GetParentSpringManager(_transform);
        }

        private SpringManager GetParentSpringManager(Transform t)
        {
            var springManager = t.GetComponent<SpringManager>();
            if (springManager != null)
                return springManager;
            if (t.parent != null) {
                return GetParentSpringManager(t.parent);
            }
            return null;
        }

        private void Start()
        {
            _currTipPos = child.position;
            springLength = Vector3.Distance(_transform.position, _currTipPos);
            _prevTipPos = _currTipPos;
        }

        public void UpdateSpring()
        {
            _org = _transform;
            _transform.localRotation = Quaternion.identity * _localRotation;
            float sqrDt = Time.deltaTime * Time.deltaTime;

            // stiffness
            Vector3 force = _transform.rotation * (boneAxis * stiffnessForce) / sqrDt;

            // drag
            force += (_prevTipPos - _currTipPos) * dragForce / sqrDt;
            force += springForce / sqrDt;

            Vector3 temp = _currTipPos;
            _currTipPos = _currTipPos - _prevTipPos + _currTipPos + force * sqrDt;
            _currTipPos = (_currTipPos - _transform.position).normalized * springLength + _transform.position;

            foreach (var t in colliders) {
                if (!(Vector3.Distance(_currTipPos, t.transform.position) <= radius + t.radius)) {
                    continue;
                }
                Vector3 normal = (_currTipPos - t.transform.position).normalized;
                _currTipPos = t.transform.position + normal * (radius + t.radius);
                _currTipPos = (_currTipPos - _transform.position).normalized * springLength + _transform.position;
            }

            _prevTipPos = temp;
            Vector3 aimVector = _transform.TransformDirection(boneAxis);
            Quaternion aimRotation = Quaternion.FromToRotation(aimVector, _currTipPos - _transform.position);
            Quaternion secondaryRotation = aimRotation * _transform.rotation;
            _transform.rotation = Quaternion.Lerp(_org.rotation, secondaryRotation, _managerRef.dynamicRatio);
        }

        private void OnDrawGizmos()
        {
            if (!debug) {
                return;
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_currTipPos, radius);
        }
    }

}