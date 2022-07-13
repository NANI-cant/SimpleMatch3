using TableLogic;
using UnityEngine;
using Zenject;

namespace Architecture {
    public class Bootstrapper : MonoInstaller {
        private Table _table;

        public override void InstallBindings() {
            _table = new Table();
            
            Container.BindInstance<Table>(_table).AsSingle().NonLazy();
        }
    }
}
