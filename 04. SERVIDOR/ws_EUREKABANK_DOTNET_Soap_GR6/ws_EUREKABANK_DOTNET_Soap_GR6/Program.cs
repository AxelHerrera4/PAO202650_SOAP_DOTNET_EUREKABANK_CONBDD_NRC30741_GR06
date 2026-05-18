using monster.edu.ec.controlador;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

// Agregar SOAP
builder.Services.AddSoapCore();
builder.Services.AddScoped<EurekaBankWSControlador>();

var app = builder.Build();

// Configurar middleware
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.UseSoapEndpoint<EurekaBankWSControlador>(
        "/EurekaBankWS.asmx",
        new SoapEncoderOptions(),
        SoapSerializer.XmlSerializer
    );
});

app.Run();
