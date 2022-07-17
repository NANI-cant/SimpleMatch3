using System;
using System.Collections.Generic;
using Infrastructure;
using TableLogic;
using UnityEngine;

namespace Architecture {
    public class Bootstrapper : MonoBehaviour {
        [SerializeField] private string _mapPath;
        [SerializeField] private TableView.TableView _tableView;
        [SerializeField][Min(0)] private int _scoreForFigure;

        private static Dictionary<Type, object> _container;

        public string MapPath => _mapPath;
        public TableView.TableView TableView => _tableView;
        public int ScoreForFigure => _scoreForFigure;

        public static bool TryGetInstance<T>(out T instance) {
            if (_container.ContainsKey(typeof(T))) {
                instance = (T)_container[typeof(T)];
                return true;
            }
            instance = default(T);
            return false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Bootstrap() {
            _container = new Dictionary<Type, object>();
            var instance = FindObjectOfType<Bootstrapper>();

            TableScheme scheme;
            try {
                scheme = new FileParcer(Application.dataPath + instance.MapPath).GenerateScheme();
            }
            catch (System.Exception ex) {
                Debug.LogException(ex);
                return;
            }

            Table table = new Table(instance.TableView, new FigureFabric(), scheme);
            Score score = new Score(instance.ScoreForFigure, table);

            _container[typeof(Table)] = table;
            _container[typeof(Score)] = score;
        }
    }
}
