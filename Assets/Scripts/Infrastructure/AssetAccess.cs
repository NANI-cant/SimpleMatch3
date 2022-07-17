using PersistentData;
using UnityEngine;

namespace Infrastructure {
    public class AssetAccess {
        private const string DatasetPath = "Datasets/BaseDataset/BaseDataset";

        public Dataset GetDataset() => Resources.Load<Dataset>(DatasetPath);
        public TextAsset GetTableScheme(string name) => Resources.Load<TextAsset>("Maps/" + name);
    }
}
