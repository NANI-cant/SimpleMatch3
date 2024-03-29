using Abstraction;
using PersistentData;
using TableLogic;
using UnityEngine;

namespace Infrastructure {
    public class FigureFabric : IFigureFabric {
        private Dataset _dataset;

        public string[] AllIds => _dataset.Ids;

        public FigureFabric() {
            _dataset = new AssetAccess().GetDataset();
        }

        public Void GetVoid() => new Void();
        public Figure GetFigure(Table table, Vector2Int position) => new Figure(table, position, _dataset.GetRandom());
    }
}
