using Abstraction;
using TableLogic;
using UnityEngine;

public class TableGenerator {
    private IFigureFabric _figureFabric;
    private Table _table;

    public TableGenerator(IFigureFabric figureFabric, Table table) {
        _figureFabric = figureFabric;
        _table = table;
    }

    public (TableMember[,], Vector2Int size) GenerateByScheme(TableScheme scheme) {
        TableMember[,] table = new TableMember[scheme.Size.y, scheme.Size.x];

        for (int y = 0; y < scheme.Size.y; y++) {
            for (int x = 0; x < scheme.Size.x; x++) {
                if (scheme.Map[y, x]) {
                    table[y, x] = _figureFabric.GetFigure(_table, new Vector2Int(x, y));
                }
                else {
                    table[y, x] = _figureFabric.GetVoid();
                }
            }
        }

        return (table, scheme.Size);
    }
}
