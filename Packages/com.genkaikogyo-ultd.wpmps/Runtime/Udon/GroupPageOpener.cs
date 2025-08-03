using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Economy;

namespace Wacky612.MultiPosterSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class GroupPageOpener : UdonSharpBehaviour
    {
        public string GroupId { set; get; }
        
        public void Open()
        {
            Store.OpenGroupPage(GroupId);
        }
    }
}
