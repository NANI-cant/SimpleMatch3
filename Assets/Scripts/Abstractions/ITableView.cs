using System.Collections.Generic;
using System.Threading.Tasks;

namespace TableLogic {
    public interface ITableView {
        Task OnFiguresDestroyedAsync(List<Figure> figures);
        Task OnFiguresReplacedAsync(List<Figure> figures);
        Task OnFiguresArrivedAsync(List<Figure> figures);
    }
}