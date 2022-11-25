using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace com.wao.core
{
    public class DebugConsole : MonobehaviourSingleton<DebugConsole>
    {

        [SerializeField]
        private Button _close;
        [SerializeField]
        private Text _consoleLog;
        [SerializeField]
        private GameObject _root;
        [SerializeField]
        private Button _showHideButton;
        [SerializeField]
        private InputField _cmd;
        private static StringBuilder _logStringBuilder;
        [SerializeField]
        private string _version;
#if ENABLE_DEBUG
        private void ClearCache()
        {
            PlayerPrefs.DeleteAll();
            Caching.ClearCache();
        }
        private void Awake()
        {
            _consoleLog.text = string.Empty;
            _logStringBuilder = new StringBuilder();
            UnityEngine.Application.logMessageReceived += ApplicationlogMessageReceivedThreaded;
            _close.onClick.AddListener(OnClose);
            _showHideButton.onClick.AddListener(OnShowConsole);
            _cmd.onEndEdit.AddListener(OnSendCMD);

            AppDomain app = AppDomain.CurrentDomain;
            app.UnhandledException += App_UnhandledException;
            SaveLog();
        }

        private void SaveLog()
        {
            if (!Directory.Exists(com.wao.Utility.Utility.GetStorageDirectory() + "Log"))
            {
                Directory.CreateDirectory(com.wao.Utility.Utility.GetStorageDirectory() + Path.DirectorySeparatorChar + "Log");
            }
            var path = com.wao.Utility.Utility.GetStorageDirectory() + "Log" + Path.DirectorySeparatorChar + "log.txt";
            File.WriteAllText(path, _logStringBuilder.ToString());
        }
        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.LogError(e.ExceptionObject.ToString());
        }

        private void OnSendCMD(string cmd)
        {
            if (!string.IsNullOrWhiteSpace(cmd))
            {
                _cmd.text = string.Empty;
                var objectAndMethod = cmd.Split(' ');
                Debug.LogWarning(cmd);
                if (objectAndMethod.Length > 1)
                {
                    GameObject.Find(objectAndMethod[0])?.SendMessage(objectAndMethod[1], objectAndMethod.Length > 2 ? objectAndMethod[2] : string.Empty);
                }
            }
        }

        private void OnShowConsole()
        {
            _root.SetActive(true);
            _consoleLog.text = _logStringBuilder.ToString();
        }

        public void ShowHideDebugConsole(bool en)
        {
            _showHideButton.gameObject.SetActive(en);

        }
        private void OnApplicationQuit()
        {
            SaveLog();
        }
        private void OnClose()
        {
            _root.SetActive(false);
        }

        private void OnClearLog()
        {
            _logStringBuilder.Clear();
            _logStringBuilder.AppendLine(_version);
            _consoleLog.text = _logStringBuilder.ToString();
        }

        private void ApplicationlogMessageReceivedThreaded(string log, string stackTrace, LogType logType)
        {
            switch (logType)
            {
                case LogType.Warning:
                    _logStringBuilder.AppendFormat("<color=yellow>{0}</color>", log);
                    break;
                case LogType.Error:
                    _logStringBuilder.AppendFormat("<color=red>{0}</color>", log);
                    break;
                case LogType.Exception:
                    _logStringBuilder.AppendFormat("<color=magenta>{0}</color>", log + " " + stackTrace);
                    break;
                case LogType.Log:
                    _logStringBuilder.AppendFormat("<color=white>{0}</color>", log);
                    break;
            }
            _logStringBuilder.Append("\n");
        }
#endif
    }

}
