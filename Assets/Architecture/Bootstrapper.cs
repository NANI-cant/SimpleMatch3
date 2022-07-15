using TableLogic;
using UnityEngine;
using Zenject;

namespace Architecture {
    public class Bootstrapper : MonoInstaller {
        [SerializeField] private TableView _tableView;

        private Table _table;

        public override void InstallBindings() {
            _table = new Table(_tableView);

            Container.BindInstance<Table>(_table).AsSingle().NonLazy();
        }
    }
}
