using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    public class CameraEmitter : MonoBehaviour
    {

        [SerializeField] private EventSystem events;

        private void Awake()
        {
            events._onEmitCamera.Invoke(GetComponent<Camera>());
        }
    }
}