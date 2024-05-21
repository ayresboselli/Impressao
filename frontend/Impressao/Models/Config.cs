using System.ComponentModel.DataAnnotations;

namespace Impressao.Models
{
    public class Config
    {
        public int Id { get; set; }
        public double DPI { get; set; } // { get { return 300; } set {} }
        public string UnidadeMedida { get; set; } // { get { return "mm"; } set {} }
        //public static string PathTemp { get { return @"C:\Users\Ayres\Documents\Projetos\Piovelli\impressao_pro\Storage\tmp\"; } set {} }
        public string PathFotos { get; set; } // { get { return @"C:\Users\Ayres\Documents\Projetos\Piovelli\impressao_pro\Storage\fotos\"; } set {} }
        public string PathPdf { get; set; } // { get { return @"C:\Users\Ayres\Documents\Projetos\Piovelli\impressao_pro\Storage\PDF\"; } set {} }
        public string WwwPath { get; set; }
        public bool AprovarAutomaticamente { get; set; }

        // Index
        public int MargemMidia { get; set; } // { get { return 10; } set {} }
        public int MargemThumb { get; set; } // { get { return 5; } set { } }
        public int LarguraThumb { get; set; } // { get { return 30; } set { } }
        public int AlturaThumb { get; set; } // { get { return 30; } set { } }
        public int TamanhoFonte { get; set; }

        public double Densidade(double value = 1){
            var densid = UnidadeMedida switch
            {
                "inch" => DPI,
                "mm" => DPI / 25.4,
                _ => 1,
            };
            return densid * value;
        }
    }
}