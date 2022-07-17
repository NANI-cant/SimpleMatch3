using TableLogic;
using UnityEngine;

namespace Abstraction {
    public interface IFigureFabric {
        string[] AllIds { get; }

        Figure GetFigure(Table table, Vector2Int position);
        Void GetVoid();
    }
}
