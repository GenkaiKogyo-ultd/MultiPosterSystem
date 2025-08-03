using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Wacky612.MultiPosterSystem
{
    public abstract class Posters : UdonSharpBehaviour
    {
        public abstract int     Count   { get; }
        public abstract bool    IsReady { get; }
        public abstract void    Initialize();
        public abstract bool    IsTextureLoaded(int index);
        public abstract void    LoadTexture(int index);
        public abstract Texture GetTexture(int index);
        public abstract string  GetGroupId(int index);
    }
}
