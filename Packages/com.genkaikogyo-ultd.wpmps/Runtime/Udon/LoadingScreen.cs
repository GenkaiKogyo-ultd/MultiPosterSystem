using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Wacky612.MultiPosterSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LoadingScreen : UdonSharpBehaviour
    {
        public void SetActive(bool b)
        {
            this.gameObject.SetActive(b);
        }
    }
}
