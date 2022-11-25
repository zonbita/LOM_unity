namespace com.wao.core
{
    using DG.Tweening;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class AudioManager : MonobehaviourSingleton<AudioManager>
    {
        [SerializeField]
        private float _fadeTime = 0.25f;
        [SerializeField]
        private float _sfxVol;
        [SerializeField]
        private float _bgmVol;

        public float SFXVol
        {
            get { return _sfxVol; }
            set
            {
                for (int i = 0; i < _sfx.Count; i++)
                {
                    _sfx[i].DOFade(value, _fadeTime);
                }
                _sfxVol = value;
                PlayerPrefs.SetFloat("SfxVol", _sfxVol);
            }
        }
        public float BgmVol
        {
            get { return _bgmVol; }
            set
            {
                _bgm.volume = value;
                _bgmVol = value;
                PlayerPrefs.SetFloat("BgmVol", _bgmVol);
            }
        }
        [SerializeField]
        private AudioSource _bgm;
        private List<AudioSource> _sfx = new List<AudioSource>();
        [SerializeField]
        private StringComponentDictionary _loopSfx = new StringComponentDictionary();
        [SerializeField]
        private Transform _sfxRoot;
        [SerializeField]
        private SoundConfigData _soundConfig;
        private void Awake()
        {
            _sfxVol = PlayerPrefs.GetFloat("SfxVol", 1f);
            _bgmVol = PlayerPrefs.GetFloat("BgmVol", 1f);
            //_bgm = GetComponent<AudioSource>();

        }
        public void PlayBgm(string bgmName)
        {
            if (!string.IsNullOrEmpty(bgmName))
            {
                Utility.AssetManager.Instance.Load<AudioClip>("Audio/BGM/" + bgmName, Utility.AssetFrom.Resources).SetOnComplete((bgm) =>
                     {
                         PlayBgm(bgm);
                     }).Excute();
            }

        }
        public void PlayBgm(AudioClip bgm)
        {
            float volConfig = 1f;
            if (_soundConfig.soundDic.ContainsKey(bgm.name))
            {
                volConfig = _soundConfig.soundDic[bgm.name].vol;
            }
            DOTween.Kill(_bgm);
            _bgm.DOFade(0, _fadeTime).onComplete = () =>
             {
                 _bgm.Stop();
                 _bgm.clip = bgm;
                 _bgm.Play();
                 var volto = BgmVol * volConfig;
                 _bgm.DOFade(volto, _fadeTime);
             };

        }
        public void PlaySfx(string nameSfx, float sfxvol = 1f, float bgmDown = 1f, Action<string> oncomPlete = null, bool loop = false)
        {
            if (!string.IsNullOrEmpty(nameSfx))
            {
                Utility.AssetManager.Instance.Load<AudioClip>("Audios/SFX/" + nameSfx, Utility.AssetFrom.Resources).SetOnComplete((sfx) =>
              {
                  if (sfx != null)
                      PlaySfx(sfx, Vector3.zero, sfxvol, bgmDown, oncomPlete, loop);
              }).Excute();
            }
        }
        public void PlaySfx(string nameSfx, Vector3 pos, float sfxvol = 1f, float bgmDown = 1f, Action<string> oncomPlete = null, bool loop = false)
        {
            if (!string.IsNullOrEmpty(nameSfx))
            {
                Utility.AssetManager.Instance.Load<AudioClip>("Audios/SFX/" + nameSfx, Utility.AssetFrom.Resources).SetOnComplete((sfx) =>
                {
                    if (sfx != null)
                        PlaySfx(sfx, pos, sfxvol, bgmDown, oncomPlete, loop);
                }).Excute();
            }
        }
        public void StopSfxLoop(string id)
        {
            if (!string.IsNullOrWhiteSpace(id) && _loopSfx.ContainsKey(id))
            {
                if (_loopSfx[id] != null)
                {
                    (_loopSfx[id] as AudioSource).Stop();
                }
                _loopSfx.Remove(id);
            }
        }
        public void PlaySfx(AudioClip sfx, Vector3 position, float sfxvol = 1f, float bgmDown = 1f, Action<string> oncomPlete = null, bool loop = false)
        {
            string id = string.Empty;
            var freeSfxSource = _sfx.FirstOrDefault(x => x != null && x.isPlaying == false);
            if (freeSfxSource == null)
            {
                var audioObj = new GameObject();
                audioObj.isStatic = true;
                audioObj.transform.SetParent(_sfxRoot);

                freeSfxSource = audioObj.AddComponent<AudioSource>();
                _sfx.Add(freeSfxSource);
            }
            freeSfxSource.transform.position = position;
            float volConfig = 1f;
            if (_soundConfig.soundDic.ContainsKey(sfx.name))
            {
                volConfig = _soundConfig.soundDic[sfx.name].vol;
            }
            freeSfxSource.volume = _sfxVol * sfxvol * volConfig;
            freeSfxSource.clip = sfx;
            freeSfxSource.Play();
            freeSfxSource.loop = loop;
            freeSfxSource.maxDistance = 100;
            if (loop)
            {

                id = Guid.NewGuid().ToString();
                _loopSfx.Add(id, freeSfxSource);
            }
            oncomPlete?.Invoke(id);
        }

        public void StopAllLoopFSX()
        {
            var listID = _loopSfx.Keys.ToList();
            for (int i = 0; i < listID.Count; i++)
            {
                StopSfxLoop(listID[i]);
            }
        }
    }
}