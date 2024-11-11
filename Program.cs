using api_coqon.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICameraService, CameraService>();
builder.Services.AddSingleton<pCloudService>(new pCloudService("TOKEN_DE_ACCESO_P_CLOUD"));//Debes crear una cuenta de desarrollador en pCloud y generar un token de acceso OAuth, que permitirá a la aplicación autenticarse y realizar operaciones de carga en la cuenta de pCloud.
builder.Services.AddCors();
builder.Services.AddHostedService<OldFileCleanupService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
;