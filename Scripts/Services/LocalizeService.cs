namespace com.wao.core
{
    using System;
    using UnityEngine;

    public class LocalizeService : ILocalizeService, IService
    {

        public string GetLocalize(string id, Language language)
        {
            string[] sp = id.Split('|');
            using (var localizeFile = GameObject.Instantiate(Resources.Load<LocalizeData>("Data/Localize/" + language.ToString() + "/" + sp[1])))
            {
                return localizeFile != null && localizeFile.localize.ContainsKey(sp[0]) ? localizeFile.localize[sp[0]] : id;
            }
        }

        public void Init()
        {

        }
    }
    public interface ILocalizeService
    {
        string GetLocalize(string id, Language language);
    }

    public enum Language
    {
        English,
        Vietnamese,
    }
    [System.Serializable]

    [CreateAssetMenu(fileName = "Localize", menuName = "Data/Localize", order = 1)]
    public class LocalizeData : ScriptableObject, IDisposable
    {
        public Language language;
        public StringStringDictionary localize;

        public void Dispose()
        {
            localize = null;
        }
    }


}