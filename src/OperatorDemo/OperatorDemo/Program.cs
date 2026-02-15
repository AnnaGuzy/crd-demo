using KubeOps.Abstractions.Builder;
using KubeOps.Operator;

namespace OperatorDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.Configure<DemoOperatorOptions>(builder.Configuration);
            builder.Logging.AddSimpleConsole(options => options.IncludeScopes = true);

            var operatorBuilder = builder.Services.AddKubernetesOperator(x => x.Namespace = "crd-demo");
            operatorBuilder.AddController<SuperServiceController, SuperServiceEntity>();

            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
