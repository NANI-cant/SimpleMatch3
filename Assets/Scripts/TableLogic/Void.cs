using System.Collections.Generic;
using Abstraction;
using UnityEngine;

namespace TableLogic {
    public class Void : TableMember {
        public override bool isMovable => false;

        public override Figure FindAroundById(string id, List<Vector2Int> dontLookInDirections) {
            return null;
        }

        public override bool TryFallInPosition(Vector2Int position) {
            return false;
        }
    }
}
