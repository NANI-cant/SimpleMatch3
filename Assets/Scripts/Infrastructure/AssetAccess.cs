using PersistentData;
using UnityEngine;

namespace Infrastructure {
    public class AssetAccess {
        private const string DATASETPATH = "Datasets/BaseDataset/BaseDataset";
        private const string MAPSFOLDERPATH = "Maps/";

        public Dataset GetDataset() => Resources.Load<Dataset>(DATASETPATH);
        public TextAsset GetTableScheme(string name) => Resources.Load<TextAsset>(MAPSFOLDERPATH + name);
    }
}
