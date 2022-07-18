using System.Collections.Generic;
using TableLogic;
using UnityEngine;

namespace Abstraction {
    public abstract class TableMember {
        public abstract Figure FindAroundById(string id, List<Vector2Int> dontLookInDirections);
        public abstract bool TryFallInPosition(Vector2Int position);
    }
}
