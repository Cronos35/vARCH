using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    public class DonoLink : MonoBehaviour
    {
        public void OpenDonoLink()
        {
            System.Diagnostics.Process.Start("https://streamelements.com/franfyrhart/tip");
        }
    }
}