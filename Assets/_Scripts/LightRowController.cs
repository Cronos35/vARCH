using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts
{
    public class LightRowController : MonoBehaviour
    {
        private Color materialColor;
        private MeshRenderer[] meshRenderers;
        private LightBlinker[] lightBlinkers;

        private void Awake()
        {
            InitializeLights();
        }

        public void InitializeLights()
        {
            meshRenderers = new MeshRenderer[transform.childCount];
            lightBlinkers = new LightBlinker[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                meshRenderers[i] = transform.GetChild(i).GetComponent<MeshRenderer>();
                lightBlinkers[i] = transform.GetChild(i).GetComponent<LightBlinker>();
            }
        }

        public void ToggleLights(float lightMultiplier, Color lightColor)
        {
            if (meshRenderers == null || meshRenderers[0] == null)
            {
                return; 
            }

            if (lightBlinkers[0] == null)
            {
                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    //materialColor = meshRenderer.materials[0].GetColor("_BaseColor");
                    //materialColor.a = 1;
                    meshRenderer.sharedMaterial.SetColor("_EmissionColor", lightColor * lightMultiplier);

                    _ = FadeLight(meshRenderer.sharedMaterial, lightColor, lightMultiplier);
                    if (lightMultiplier == 0)
                    {
                        continue;
                    }

                }
                return;
            }

            foreach (LightBlinker lightBlinker in lightBlinkers)
            {
                lightBlinker.StartBlink(lightMultiplier, lightColor);
            }
        }

        public void SetLightColorEditor(Color lightColor)
        {
            if (meshRenderers == null || meshRenderers.Length == 0 || meshRenderers[0] == null)
            {
                return;
            }
            foreach (MeshRenderer mesh in meshRenderers)
            {
                mesh.sharedMaterial.SetColor("_EmissionColor", lightColor * 1);
            }
        }

        public async UniTask FadeLight(Material material, Color color, float lightMultiplier)
        {
            for (float i = lightMultiplier; i > 0; i -= lightMultiplier / 4)
            {
                UniTask.Delay(100);
                //yield return new WaitForSeconds(100f);
                material.SetColor("_EmissionColor", color * i);
            }
        }
    }
}