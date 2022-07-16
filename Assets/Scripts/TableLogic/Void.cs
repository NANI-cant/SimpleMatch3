using Abstraction;
using UnityEngine;

namespace TableLogic {
    public class Void : TableMember {
        public override bool isMovable => false;

        public override Figure FindAroundById(Vector2Int position, string id, Vector2Int dontLookInDirection) {
            return null;
        }

        public override bool TryFallInPosition(Vector2Int position) {
            return false;
        }
    }
}
