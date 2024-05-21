// See https://aka.ms/new-console-template for more information

using System.Text;

namespace Automacao
{
    class Agrupado
    {
        public int Count { get; set; }
        public int Largura { get; set; }
        public int Altura { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ConfigureService.Configure();

            //int coreCount = 0;
            //foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            //{
            //    coreCount += int.Parse(item["NumberOfCores"].ToString());
            //}

            //Importacao.Processar(coreCount);
            //Imposicao.Imposicionar();

            //Thread.Sleep(60000);
        }

    }
}