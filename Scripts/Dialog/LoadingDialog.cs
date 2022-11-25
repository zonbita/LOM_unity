namespace com.wao.rpgs
{
    using com.wao.core;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;


    [ExecuteInEditMode]
    public class LoadingDialog : DialogBase
    {
        [SerializeField]
        private Image _loadingProgressOverlay;
        [SerializeField]
        private float _progress;
        private Action _onComplete;
        private Dictionary<string, ProgressTask> _progressTask = new Dictionary<string, ProgressTask>();

        private void OnDisable()
        {
            _progressTask.Clear();
        }
        public void StartLoading(Action oncomplete, params ProgressTask[] progressTasks)
        {
            for (int i = 0; i < progressTasks.Length; i++)
            {
                AddTask(progressTasks[i].taskName, progressTasks[i]);
            }
            _onComplete = oncomplete;
        }
        public void AddTask(string taskName, ProgressTask progress)
        {
            if (!_progressTask.ContainsKey(taskName))
            {
                _progressTask.Add(taskName, progress);
            }
        }
        public void UpdateProgress(ProgressTask progress)
        {
            if (_progressTask.ContainsKey(progress.taskName))
            {

                if (_progressTask[progress.taskName].current >= _progressTask[progress.taskName].max)
                {
                    _progress += _progressTask[progress.taskName].max;
                    _progressTask.Remove(progress.taskName);
                }
            }
        }

        private void Update()
        {
            var progres = 0f;
            foreach (var smallProgress in _progressTask.Values)
            {
                progres += smallProgress.current;
            }
            _progress = Mathf.Clamp(_progress, 0f, 1f);
            var current = _progress + progres;
            _loadingProgressOverlay.GetComponent<RectTransform>().sizeDelta = new Vector2(current * 1920, 100);
            if (current >= 1f || (current > 0 && _progressTask.Count == 0))
            {
                _onComplete?.Invoke();
                Destroy(gameObject);
            }
        }

    }

    public class ProgressTask
    {
        public string taskName;
        public float current;
        public float max;
    }
}