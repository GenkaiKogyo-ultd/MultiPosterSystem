using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components;

namespace Wacky612.MultiPosterSystem
{
    [RequireComponent(typeof(VRCUnityVideoPlayer))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DynamicPosters : Posters
    {
        [SerializeField] private VRCUrl        _posterVideoUrl;
        [SerializeField] private VRCUrl        _groupIdsJsonUrl          = null;
        [SerializeField] private float         _posterLoadDelaySeconds   = 0;
        [SerializeField] private float         _groupIdsLoadDelaySeconds = 0;
        [SerializeField] private RenderTexture _renderTexture;

        private Texture[] _textures;
        private string[]  _groupIds;

        private VRCUnityVideoPlayer _videoPlayer;
        private int                 _processingIndex;
        private bool                _isReady;

        public override int  Count   { get { return _textures.Length; } }
        public override bool IsReady { get { return _isReady; }}
        
        public override void Initialize()
        {
            _videoPlayer = GetComponent<VRCUnityVideoPlayer>();
            _videoPlayer.Loop                  = false;
            _videoPlayer.EnableAutomaticResync = false;
            _processingIndex = -1;
            _isReady         = false;

            SendCustomEventDelayedSeconds(nameof(LoadPosterVideoUrl),  _posterLoadDelaySeconds);
            SendCustomEventDelayedSeconds(nameof(LoadGroupIdsJsonUrl), _groupIdsLoadDelaySeconds);
        }

        public void LoadPosterVideoUrl()
        {
            _videoPlayer.LoadURL(_posterVideoUrl);
        }

        public override void OnVideoReady()
        {
            _textures = new Texture[(int) Math.Floor(_videoPlayer.GetDuration() - 2)];
            _videoPlayer.Pause();
            _isReady = true;
        }

        public override void OnVideoError(VideoError videoError)
        {
            // 
        }

        public override bool IsTextureLoaded(int index)
        {
            return (_textures != null) ? (_textures[index] != null) : false;
        }
        
        public override void LoadTexture(int index)
        {
            if (_videoPlayer.IsReady && _processingIndex == -1)
            {
                _processingIndex = index;
                _UpdateVideoPlayerTime();
            }
        }

        public void _UpdateVideoPlayerTime()
        {
            _videoPlayer.SetTime(_processingIndex + 1);
            _videoPlayer.Play();
            SendCustomEventDelayedFrames(nameof(_CheckVideoPlayerTime), 10);
        }

        public void _CheckVideoPlayerTime()
        {
            _videoPlayer.Pause();
            var time = _videoPlayer.GetTime();

            if (time < _processingIndex + 1 || time >= _processingIndex + 2)
            {
                // まれに、SetTime() しても動画プレイヤーの再生時間が目的の時間にならないことがある
                // 一旦、SetTime(0) した後、_UpdateVideoPlayerTime を再度実行することで対処している

                _videoPlayer.SetTime(0);
                SendCustomEventDelayedFrames(nameof(_UpdateVideoPlayerTime), 10);
            }
            else
            {
                SendCustomEventDelayedFrames(nameof(_CaptureTexture), 10);
            }
        }

        public void _CaptureTexture()
        {
            var texture = new RenderTexture(_renderTexture);
            texture.Create();

            #if UNITY_STANDALONE_WIN
            // var scale  = new Vector2(1, -1);
            // var offset = new Vector2(0,  1);
            var scale  = new Vector2(1,  1);
            var offset = new Vector2(0,  0);
            #else
            var scale  = new Vector2(1,  1);
            var offset = new Vector2(0,  0);
            #endif

            VRCGraphics.Blit(_renderTexture, texture, scale, offset);
            _textures[_processingIndex] = (Texture) texture;
            _processingIndex = -1;
        }

        public override Texture GetTexture(int index)
        {
            return (_textures != null) ? _textures[index] : null;
        }

        public void LoadGroupIdsJsonUrl()
        {
            if (_groupIdsJsonUrl != null)
            {
                VRCStringDownloader.LoadUrl(_groupIdsJsonUrl, (IUdonEventReceiver) this);
            }
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            if (result.Url == _groupIdsJsonUrl)
            {
                if (VRCJson.TryDeserializeFromJson(result.Result, out DataToken data))
                {
                    _groupIds = new string[data.DataList.Count];

                    for (int i = 0; i < data.DataList.Count; i++)
                    {
                        _groupIds[i] = data.DataList[i].String;
                    }
                }
            }
        }

        public override string GetGroupId(int index)
        {
            return (_groupIds != null) ? _groupIds[index] : "";
        }
    }
}
