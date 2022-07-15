using UnityEngine;

namespace PersistentData {
    [CreateAssetMenu(fileName = "Dataset", menuName = "PersistentData/Dataset")]
    public class Dataset : ScriptableObject {
        [SerializeField] private FigureData[] _dataset;

        public FigureData GetRandom() => _dataset[Random.Range(0, _dataset.Length)];
    }
}
