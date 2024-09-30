
using FastEndpoints;
using HealthMed.Hub.Api;
using FastEndpoints.Swagger;
using Infrastructure.PostgreDb;
using HealthMed.Hub.Domain.Appointments.UseCases.RegisterAppointment;
using HealthMed.Hub.Domain.AvailableTimes.UseCases.RegisterAvailableTime;
using HealthMed.Hub.Domain.Doctors.UseCases.RegisterDoctor;
using HealthMed.Hub.Domain.Patients.UseCases.RegisterPatient;
using HealthMed.Hub.Domain.Appointments.UseCases.GetAppointmentsByPatientId;
using HealthMed.Hub.Domain.AvailableTimes.UseCases.GetAvailableTimes;
using HealthMed.Hub.Domain.Doctors.UseCases.GetDoctors;
using HealthMed.Hub.Domain.Doctors.Gateways;
using HealthMed.Hub.Infrastructure.PostgresDb.Gateways;
using HealthMed.Hub.Domain.Patients.Gateways;
using HealthMed.Hub.Domain.AvailableTimes.Gateways;
using HealthMed.Hub.Domain.Appointments.Gateways;
using FluentValidation;
using HealthMed.Hub.Domain.Appointments.UseCases.GetAppointmentByPatient;
using HealthMed.Hub.Domain.AvailableTimes.UseCases.UpdateAvailableTime;

Console.WriteLine("Aguradando 45 segundos para iniciar a aplicação...");
await Task.Delay(TimeSpan.FromSeconds(45));
Console.WriteLine("Iniciando aplicação...");

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "hh:mm:ss ";
});

builder.Services.AddFastEndpoints();
builder.Services.AddHealthChecks();

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.DocumentName = "swagger";
        s.Title = "Health Hub";
        s.Version = "v1";
        s.Description = "Documentation about endpoints";
    };

    o.EnableJWTBearerAuth = false;
    o.ShortSchemaNames = false;
    o.RemoveEmptyRequestSchema = true;
});

builder.Services.AddHttpClient();

// ** CONTEXT POSTGRE**
var postgreDbSettings = builder.Configuration.GetSection("PostgreDbSettings").Get<PostgreDbSettings>();

builder.Services.AddSingleton<Context>
    (
    sp => new Context
                       (
                        postgreDbSettings!.PostgresConnection
                        ));


// ** USECASES **
builder.Services.AddScoped<IRegisterAppointmentUseCase, RegisterAppointmentUseCase>();
builder.Services.AddScoped<IRegisterAvailableTimeUseCase, RegisterAvailableTimeUseCase>();
builder.Services.AddScoped<IRegisterDoctorUseCase, RegisterDoctorUseCase>();
builder.Services.AddScoped<IRegisterPatientUseCase, RegisterPatientUseCase>();
builder.Services.AddScoped<IGetAppointmentsByPatientIdUseCase, GetAppointmentsByPatientIdUseCase>();
builder.Services.AddScoped<IGetAvailableTimesUseCase, GetAvailableTimesUseCase>();
builder.Services.AddScoped<IGetDoctorsUseCase, GetDoctorsUseCase>();
builder.Services.AddScoped<IUpdateAvailableTimeUseCase, UpdateAvailableTimeUseCase>();

// ** GATEWAY **
builder.Services.AddScoped<IDoctorGateway, DoctorGateway>();
builder.Services.AddScoped<IPatientGateway, PatientGateway>();
builder.Services.AddScoped<IAvailableTimeGateway, AvailableGateway>();
builder.Services.AddScoped<IAppointmentsGateway, AppointmentGateway>();

// ** VALIDATORS **
builder.Services.AddScoped<IValidator<RegisterAppointmentInput>, RegisterAppointmentValidator>();
builder.Services.AddScoped<IValidator<RegisterAvailableTimeInput>, RegisterAvailableTimeValidator>();
builder.Services.AddScoped<IValidator<RegisterDoctorInput>, RegisterDoctorValidator>();
builder.Services.AddScoped<IValidator<RegisterPatientInput>, RegisterPatientValidator>();
builder.Services.AddScoped<IValidator<GetAppointmentsByPatientIdInput>, GetAppointmentsByPatientIdUseCaseValidator>();
builder.Services.AddScoped<IValidator<GetAvailableTimesInput>, GetAvailableTimesInputValidator>();
builder.Services.AddScoped<IValidator<GetDoctorsInput>, GetDoctorsValidator>();
builder.Services.AddScoped<IValidator<UpdateAvailableTimeInput>, UpdateAvailableTimeInputValidator>();

var app = builder.Build();
app.MapHealthChecks("/health");

app.UseFastEndpoints(c =>
{
    c.Endpoints.ShortNames = false;

    c.Endpoints.Configurator = ep =>
    {
        ep.Summary(s =>
        {
            s.Response<ErrorResponse>(400);
            s.Response(401);
            s.Response(403);
            s.Responses[200] = "OK";
        });

        ep.PostProcessors(FastEndpoints.Order.After, new GlobalLoggerPostProcces
        (
            LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }).CreateLogger<GlobalLoggerPostProcces>()
        ));
    };
}).UseSwaggerGen();

app.Run();

