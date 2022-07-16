using Abstraction;
using PersistentData;
using TableLogic;
using UnityEngine;

namespace Infrastructure {
    public class FigureFabric : IFigureFabric {
        private Dataset _dataset;

        public FigureFabric() {
            _dataset = Resources.FindObjectsOfTypeAll<Dataset>()[0];
        }

        public Void GetVoid() {
            return new Void();
        }

        public Figure GetFigure(Table table, Vector2Int position) {
            return new Figure(table, position, _dataset.GetRandom());
        }
    }
}
