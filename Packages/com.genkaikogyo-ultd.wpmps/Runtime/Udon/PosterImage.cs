using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Wacky612.MultiPosterSystem
{
    public abstract class PosterImage : UdonSharpBehaviour
    {
        private RawImage _img;

        public void Initialize()
        {
            _img = GetComponent<RawImage>();
        }
        
        public void SetTexture(Texture texture)
        {
            if (texture != null) _img.texture = texture;
        }

        public void SetAlpha(float alpha)
        {
            _img.color = new Color(1, 1, 1, alpha);
        }
    }
}
