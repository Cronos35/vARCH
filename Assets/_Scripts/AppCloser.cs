using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    public class AppCloser : MonoBehaviour
    {
        public void CloseApplication()
        {
            Application.Quit();
        }
    }
}