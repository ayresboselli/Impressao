using Impressao.Models;

namespace Impressao.ViewModel
{
    public class MateriaPrimaViewModel
    {
        public MateriaPrima Materia { get; set; }
        public IEnumerable<MateriaPrimaGrupo> Grupos { get; set; }
        public IEnumerable<UnidadeMedida> UnidadesMedida { get; set; }
        public IEnumerable<SetorEstoque> SetoresEstoque { get; set; }
    }
}
