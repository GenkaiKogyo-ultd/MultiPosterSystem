using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Wacky612.MultiPosterSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class MultiPosterSystem : UdonSharpBehaviour
    {
        [SerializeField] private PosterType _posterType;
        [SerializeField] private float      _durationSeconds   = 4.0f;
        [SerializeField] private float      _transitionSeconds = 2.0f;

        [UdonSynced] private long _referenceDateTimeBinary;

        private Posters         _posters;
        private PosterImage     _frontPosterImage, _backPosterImage;
        private GroupPageOpener _groupPageOpener;
        private LoadingScreen   _loadingScreen;
        private DateTime        _referenceDateTime;

        private void Start()
        {
            switch (_posterType)
            {
                case PosterType.Static:
                    _posters = (Posters) GetComponentInChildren<StaticPosters>();
                    break;
                case PosterType.Dynamic:
                    _posters = (Posters) GetComponentInChildren<DynamicPosters>();
                    break;
            }
            
            _frontPosterImage  = (PosterImage) GetComponentInChildren<FrontPosterImage>();
            _backPosterImage   = (PosterImage) GetComponentInChildren<BackPosterImage>();
            _groupPageOpener   = GetComponentInChildren<GroupPageOpener>();
            _loadingScreen     = GetComponentInChildren<LoadingScreen>();
            _referenceDateTime = DateTime.MinValue;

            if (_durationSeconds   < 4.0f) _durationSeconds   = 4.0f;
            if (_transitionSeconds < 2.0f) _transitionSeconds = 2.0f;

            _frontPosterImage.Initialize();
            _backPosterImage.Initialize();
            _posters.Initialize();
        }

        private void Update()
        {
            if (_posters.IsReady)
            {
                if (_referenceDateTime == DateTime.MinValue)
                {
                    _referenceDateTime = Networking.GetNetworkDateTime();
                    Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
                    RequestSerialization();
                }
                else
                {
                    int   index   = GetIndex();
                    float seconds = GetSeconds();
                
                    bool isFrontTextureLoaded = _posters.IsTextureLoaded(index);
                    bool isBackTextureLoaded  = _posters.IsTextureLoaded((index + 1) % _posters.Count);

                    if (!isFrontTextureLoaded)
                    {
                        _posters.LoadTexture(index);
                    }
                    else if (!isBackTextureLoaded)
                    {
                        _posters.LoadTexture((index + 1) % _posters.Count);
                    }

                    if (isFrontTextureLoaded &&
                        (isBackTextureLoaded || seconds <= _durationSeconds * 0.75))
                    {
                        _loadingScreen.SetActive(false);
                        UpdatePosterImage(index, seconds);
                        UpdatePosterGroupId(index, seconds);
                    }
                }
            }
        }

        private void UpdatePosterImage(int index, float seconds)
        {
            if (seconds > _durationSeconds)
            {
                var alpha = (_durationSeconds + _transitionSeconds - seconds) / _transitionSeconds;
                    
                _backPosterImage.SetTexture(_posters.GetTexture((index + 1) % _posters.Count));
                _frontPosterImage.SetTexture(_posters.GetTexture(index));
                _frontPosterImage.SetAlpha(alpha);
            }
            else if (seconds > _durationSeconds * 0.75)
            {
                // BackPosterの差し替え
                _backPosterImage.SetTexture(_posters.GetTexture((index + 1) % _posters.Count));
                _frontPosterImage.SetTexture(_posters.GetTexture(index));
                _frontPosterImage.SetAlpha(1);
            }
            else if (seconds > _durationSeconds * 0.50)
            {
                // FrontPosterを見えるように
                _backPosterImage.SetTexture(_posters.GetTexture(index));
                _frontPosterImage.SetTexture(_posters.GetTexture(index));
                _frontPosterImage.SetAlpha(1);
            }
            else if (seconds > _durationSeconds * 0.25)
            {
                // FrontPosterをBackPosterと同じポスターに
                _backPosterImage.SetTexture(_posters.GetTexture(index));
                _frontPosterImage.SetTexture(_posters.GetTexture(index));
                _frontPosterImage.SetAlpha(0);                    
            }
            else
            {
                _backPosterImage.SetTexture(_posters.GetTexture(index));
                _frontPosterImage.SetAlpha(0);                    
            }
        }

        private void UpdatePosterGroupId(int index, float seconds)
        {
            if (seconds > _durationSeconds + (_transitionSeconds / 2))
            {
                _groupPageOpener.GroupId = _posters.GetGroupId((index + 1) % _posters.Count);
            }
            else
            {
                _groupPageOpener.GroupId = _posters.GetGroupId(index);
            }
        }

        private float GetSecondsFromReferenceTime()
        {
            return (float) (Networking.GetNetworkDateTime() - _referenceDateTime).TotalSeconds;
        }

        private float GetSecondsFromPosterStart()
        {
            return GetSecondsFromReferenceTime() % ((_durationSeconds + _transitionSeconds) * _posters.Count);
        }

        private int GetIndex()
        {
            return (int) Math.Floor(GetSecondsFromPosterStart() / (_durationSeconds + _transitionSeconds));
        }

        private float GetSeconds()
        {
            return GetSecondsFromPosterStart() % (_durationSeconds + _transitionSeconds);
        }
        
        public override void OnPreSerialization()
        {
            _referenceDateTimeBinary = _referenceDateTime.ToBinary();
        }

        public override void OnDeserialization()
        {
            _referenceDateTime = DateTime.FromBinary(_referenceDateTimeBinary);
        }
    }

    public enum PosterType
    {
        Static, Dynamic
    }
}
