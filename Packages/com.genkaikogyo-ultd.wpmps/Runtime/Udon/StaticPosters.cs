using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Wacky612.MultiPosterSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class StaticPosters : Posters
    {
        private Poster[] _posters;

        public override int  Count   { get { return _posters.Length; }}
        public override bool IsReady { get { return (_posters != null); }}
        
        public override void    Initialize()               { _posters = GetComponentsInChildren<Poster>(); }
        public override bool    IsTextureLoaded(int index) { return true; }
        public override void    LoadTexture(int index)     {}
        public override Texture GetTexture(int index)      { return _posters[index].Texture; }
        public override string  GetGroupId(int index)      { return _posters[index].GroupId; }
    }
}
