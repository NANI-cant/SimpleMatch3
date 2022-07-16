using TableLogic;
using UnityEngine;

namespace Abstraction {
    public abstract class TableMember {
        public abstract bool isMovable { get; }

        public abstract Figure FindAroundById(Vector2Int position, string id, Vector2Int dontLookInDirection);
        public abstract bool TryFallInPosition(Vector2Int position);
    }
}
