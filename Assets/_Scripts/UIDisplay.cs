using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static ControlsMaster;

namespace Assets._Scripts
{
    public class UIDisplay : MonoBehaviour, IPlayerActions
    {

        [SerializeField] private GameObject mainUI;

        private ControlsMaster controlsMaster;
        private Vector2 mouseMovement;
        private void Awake()
        {
            controlsMaster = new ControlsMaster();
            controlsMaster.Player.SetCallbacks(this);
            controlsMaster.Player.Enable();
        }

        public void OnMouseMove(InputAction.CallbackContext context)
        {
            mouseMovement = context.ReadValue<Vector2>();
            if (mouseMovement != Vector2.zero)
            {
                mainUI.SetActive(true);
                return;
            }
            _ = DisableUIDelayed();
        }

        private async UniTask DisableUIDelayed()
        {
            if (mouseMovement != Vector2.zero)
            {
                return;
            }
            await UniTask.Delay(1500);
            mainUI.SetActive(false);
        }
    }
}