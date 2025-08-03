using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Wacky612.MultiPosterSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Poster : UdonSharpBehaviour
    {
        [SerializeField] private Texture2D _texture;
        [SerializeField] private string    _groupId;

        public Texture Texture { get { return (Texture) _texture; }}
        public string  GroupId { get { return _groupId; }}
    }
}
