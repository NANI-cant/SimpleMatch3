using Abstraction;
using UnityEngine;

namespace PersistentData {
    [CreateAssetMenu(fileName = "FigureData", menuName = "PersistentData/FigureData")]
    public class FigureData : ScriptableObject, IFigureData {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _sprite;

        public string Id => _id;
        public Sprite Sprite => _sprite;
    }
}
