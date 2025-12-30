using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace Wacky612.MultiPosterSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ClickText : UdonSharpBehaviour
    {
        public void SetActive(bool b)
        {
            this.gameObject.SetActive(b);
        }
    }
}
