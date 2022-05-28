using System.Collections;
using UnityEngine;
using static Assets._Scripts.LightArrayController;

namespace Assets._Scripts
{
    public class LightArraySpinner : MonoBehaviour
    {
        public float rotationSpeed = 1;
        public TriggerSequence rotationDirection;
        [SerializeField] private RotationAxis rotationAxis;
        
        float yRotation = 0;
        float xRotation = 0;
        float zRotation = 0;

        Quaternion rotation;
        private void Awake()
        {
            xRotation = transform.localEulerAngles.x;
            yRotation = transform.localEulerAngles.y;
            zRotation = transform.localEulerAngles.z;
        }

        // Update is called once per frame
        void Update()
        {
            switch (rotationAxis)
            {
                case RotationAxis.x:
                    xRotation = rotationDirection == TriggerSequence.Forward ? 
                        xRotation + Time.deltaTime * rotationSpeed :
                        xRotation + Time.deltaTime * rotationSpeed * -1;
                    break;
                case RotationAxis.y:
                    yRotation = rotationDirection == TriggerSequence.Forward ? 
                        yRotation + Time.deltaTime * rotationSpeed : 
                        yRotation + Time.deltaTime * rotationSpeed * -1;
                    break;
                case RotationAxis.z:
                    zRotation = rotationDirection == TriggerSequence.Forward ? 
                        zRotation + Time.deltaTime * rotationSpeed : 
                        zRotation + Time.deltaTime * rotationSpeed * -1;
                    break;
            }

            rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
            transform.localRotation = rotation;
        }
    }

    public enum RotationAxis
    {
        y,
        x, 
        z
    }
}