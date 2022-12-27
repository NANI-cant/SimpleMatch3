using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace PersistentProgress {
    public class PersistentProgressService {
        public PersistentProgress PersistentProgress { get; private set; }
        
        public event Action Changed;
        
        [DllImport("__Internal")]
        private static extern void Log(string log);
        [DllImport("__Internal")]
        private static extern void LoadExtern();
        [DllImport("__Internal")]
        private static extern void SaveExtern(string progress);

        public PersistentProgressService() {
            PersistentProgress = new PersistentProgress();
        }

        public void Save() {
            string serialized = JsonUtility.ToJson(PersistentProgress);
            Debug.Log(serialized);
#if UNITY_EDITOR
#else
        SaveExtern(serialized);
#endif
        }

        public void Load() => LoadExtern();

        public void TryUpdateBestScore(int score) {
            if (score <= PersistentProgress.BestScore) return;

            PersistentProgress.BestScore = score;
            Save();
            Changed?.Invoke();
        }

        public void SetProgress(string progress) {
            Log("Setting " + progress);
            PersistentProgress = JsonUtility.FromJson<PersistentProgress>(progress);
            Log(PersistentProgress.BestScore.ToString());
            Changed?.Invoke();
        }
    }

    [Serializable]
    public class PersistentProgress {
        public int BestScore;
    }
}