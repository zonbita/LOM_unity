namespace com.wao.core
{
    using UnityEngine;
    [CreateAssetMenu(fileName = "Sound Data", menuName = "Data/Sound", order = 1)]
    public class SoundConfigData : ScriptableObject
    {
        public StringSoundSettingDictionary soundDic = new StringSoundSettingDictionary();
    }
    [System.Serializable]
    public struct SoundSetting
    {
        public string sound;
        public float vol;
    }
}