using TableLogic;
using UnityEngine;

namespace Abstraction {
    public interface IFigureFabric {
        Figure GetFigure(Table table, Vector2Int position);
    }
}
