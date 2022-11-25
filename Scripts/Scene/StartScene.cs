namespace com.wao.rpgs
{

    using com.wao.core;
    using UnityEngine.UI;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class StartScene : SceneBase
    {
        // Start is called before the first frame update
        [SerializeField]
        private Button _startBtn;
        private LoadingDialog _loadingDialog;
        private List<ProgressTask> _progressTask = new List<ProgressTask>();
        void Start()
        {
            _startBtn.onClick.AddListener(OnStartBtnClick);
            for (int i = 0; i < 100; i++)
            {
                _progressTask.Add(new ProgressTask { taskName = i.ToString(), current = 0, max = 1 / 100f });
            }
        }

        private void OnStartBtnClick()
        {
            _loadingDialog = DialogManager.Instance.LoadDialog<LoadingDialog>();

            _loadingDialog.StartLoading(() =>
            {
                SceneManager.Instance.LoadScene<GameScene>();
            }, _progressTask.ToArray());

        }

        // Update is called once per frame
        void Update()
        {
            if (_loadingDialog != null)
            {
                for (int i = 0; i < _progressTask.Count; i++)
                {
                    _progressTask[i].current += 0.0001f;
                    _loadingDialog.UpdateProgress(_progressTask[i]);
                }
            }

        }
    }
}