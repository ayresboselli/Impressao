using System;
using System.Diagnostics;
using System.Timers;
using System.Management;
using static Microsoft.FSharp.Core.ByRefKinds;
using System.Text;
using System.Security.Cryptography;

namespace Automacao
{
    public class Automacao
    {
        System.Timers.Timer timer = new();
        bool processing = true;
        int tempo = 10000;

        public void Start()
        {
            timer.Interval = 10000;
            timer.Elapsed += new ElapsedEventHandler(Processar);
            timer.Enabled = true;
            processing = true;
        }

        private async void Processar(object sender, ElapsedEventArgs e)
        {
            timer.Enabled = false;

            ManagementObjectSearcher baseboardSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
            ManagementObjectSearcher motherboardSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_MotherboardDevice");

            string dispositivo = string.Empty;
            foreach (ManagementObject queryObj in baseboardSearcher.Get())
            {
                dispositivo = queryObj["SerialNumber"].ToString();
            }

            string licenca = File.ReadAllText("C:\\Program Files\\Piovelli\\backend\\licenca.txt");

            using var sha1 = SHA1.Create();
            var hash = Convert.ToHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(dispositivo + "3f81b01465864f49d7d61bb1e11c4160")));

            Console.WriteLine("Dispositivo: " + dispositivo);

            if (licenca == hash)
            {
                Task.Run(() => ProcessaIndex());
                Task.Run(() => ProcessaImportacao());
                Task.Run(() => ProcessaImposicao());
            }
        }

        private async Task ProcessaIndex()
        {
            while (processing)
            {
                var processIndex = new Process();
                processIndex.StartInfo.FileName = "C:\\Program Files\\Piovelli\\backend\\Index.exe";
                processIndex.StartInfo.Arguments = "48966ad7c3a6f9f05b8164fda9ca39fa";
                processIndex.Start();
                processIndex.WaitForExit();

                Thread.Sleep(tempo);
            }
        }

        private async Task ProcessaImportacao()
        {
            while (processing)
            {
                var processImportacao = new Process();
                processImportacao.StartInfo.FileName = "C:\\Program Files\\Piovelli\\backend\\Importacao.exe";
                processImportacao.StartInfo.Arguments = "48966ad7c3a6f9f05b8164fda9ca39fa";
                processImportacao.Start();
                processImportacao.WaitForExit();

                Thread.Sleep(tempo);
            }
        }

        private async Task ProcessaImposicao()
        {
            while (processing)
            {
                var processImposicao = new Process();
                processImposicao.StartInfo.FileName = "C:\\Program Files\\Piovelli\\backend\\Imposicao.exe";
                processImposicao.StartInfo.Arguments = "48966ad7c3a6f9f05b8164fda9ca39fa";
                processImposicao.Start();
                processImposicao.WaitForExit();

                Thread.Sleep(tempo);
            }
        }

        public void Stop()
        {
            processing = false;
        }
    }
}
