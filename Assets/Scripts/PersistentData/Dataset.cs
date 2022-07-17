using UnityEngine;

namespace PersistentData {
    [CreateAssetMenu(fileName = "Dataset", menuName = "PersistentData/Dataset")]
    public class Dataset : ScriptableObject {
        [SerializeField] private FigureData[] _dataset;

        public int Length => _dataset.Length;
        public string[] Ids {
            get {
                string[] ids = new string[Length];
                for (int i = 0; i < Length; i++) {
                    ids[i] = _dataset[i].Id;
                }
                return ids;
            }
        }

        public FigureData GetRandom() => _dataset[Random.Range(0, _dataset.Length)];
    }
}
