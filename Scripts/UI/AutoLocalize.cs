namespace com.wao.core
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Text))]
    public class AutoLocalize : MonoBehaviour
    {
        public string id;
        // Start is called before the first frame update
        private void OnEnable()
        {
            _listAutoLocalize.Add(this);
            var localizeService = ServiceManager.Instance.GetService<LocalizeService>();
            var gameSettingController = ControllerManager.Instance.GetController<GameSettingController>();
            GetComponent<Text>().text = ServiceManager.Instance.GetService<LocalizeService>().GetLocalize(id, gameSettingController.Language);
        }

        private void OnDisable()
        {
            _listAutoLocalize.Remove(this);
        }
        private void OnDestroy()
        {
            _listAutoLocalize.Remove(this);
        }
        private static readonly List<AutoLocalize> _listAutoLocalize = new List<AutoLocalize>();
        public static void ChangeLanguage(Language language)
        {
            var localizeService = ServiceManager.Instance.GetService<LocalizeService>();
            for (int i = 0; i < _listAutoLocalize.Count; i++)
            {
                _listAutoLocalize[i].GetComponent<Text>().text = localizeService.GetLocalize(_listAutoLocalize[i].id, language);
            }
        }
    }
}