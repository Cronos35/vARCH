using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Assets._Scripts
{
    public class LightBlinker : MonoBehaviour
    {
        [SerializeField] private Color emissionColor;
        private MeshRenderer meshRenderer;
        private Material meshMaterial;

        public int glowMaterialIndex;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshMaterial = meshRenderer.materials[glowMaterialIndex];
        }
            
        public void StartBlink(float lightMultiplier, Color lightColor)
        {
            _ = BlinkLights(lightMultiplier, lightColor);
        }

        private async UniTask BlinkLights(float lightMultiplier, Color lightColor)
        {
            for (float i = lightMultiplier; i > 0; i -= lightMultiplier / 4)
            {
                await UniTask.Delay(Mathf.RoundToInt(Time.deltaTime * 100));
                meshMaterial.SetColor("_EmissionColor", lightColor * i);
            }
        }
    }
}