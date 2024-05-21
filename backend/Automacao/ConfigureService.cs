using Topshelf;

namespace Automacao
{
    public class ConfigureService
    {
        internal static void Configure()
        {
            HostFactory.Run(configure =>
            {
                configure.Service<Automacao>(service =>
                {
                    service.ConstructUsing(s => new Automacao());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                //Configure a Conta que o serviço do Windows usa para rodar
                configure.RunAsLocalSystem();
                configure.SetServiceName("Impressão - Automação");
                configure.SetDisplayName("Impressão - Automação");
                configure.SetDescription("Serviço de automação de impressões");
            });
        }
    }
}
