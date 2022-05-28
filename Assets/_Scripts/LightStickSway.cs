using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    public class LightStickSway : MonoBehaviour
    {
        public float lightIntensity = 3f;
        public float swayTime = 0.5f;
        public Color glowColor;
        public float swayLimit;

        private Vector3 initialPosition;
        private MeshRenderer meshRenderer;

        // Use this for initialization
        void Start()
        {
            initialPosition = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
            transform.LeanRotateX(swayLimit, swayTime).setEaseInOutSine().setLoopPingPong();
            meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        }
        private void Update()
        {
            meshRenderer.materials[0].SetColor("_EmissionColor", glowColor * lightIntensity);
        }
    }
}