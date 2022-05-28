using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts
{
    public class AudioSyncScale : AudioSyncer
    {
        [SerializeField] private Vector3 restScale;

        private MeshRenderer mesh;
        private Material glowMaterial;
        private Color glowColor;

        private void Awake()
        {
            mesh = GetComponent<MeshRenderer>();
            glowMaterial = mesh.materials[1];
        }
        protected override void OnStart()
        {
            base.OnStart();
        }
        public void SetGlowColor(Color color)
        {
            if(glowMaterial == null)
            {
                return;
            }
            glowMaterial.SetColor("_EmissionColor", color * 2);
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            transform.localScale = new Vector3(restScale.x, _spectrumValue / 2, restScale.z);
        }
    }
}