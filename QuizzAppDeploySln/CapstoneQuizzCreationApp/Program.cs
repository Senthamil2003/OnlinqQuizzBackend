using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CapstoneQuizzCreationApp.Context;
using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using CapstoneQuizzCreationApp.Repositories.GeneralRepository;
using CapstoneQuizzCreationApp.Repositories.JoinedRepository;
using CapstoneQuizzCreationApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CapstoneQuizzCreationApp
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
            var sqlSecretName = builder.Configuration["KeyVault:SecretNames:SqlSecret"];
            var blobStringName = builder.Configuration["KeyVault:SecretNames:BlobString"];
            var blobContainerName = builder.Configuration["KeyVault:SecretNames:BlobContainer"];
            var jwtSecretName = builder.Configuration["KeyVault:SecretNames:JwtSecret"];
            var onlinesql = builder.Configuration["KeyVault:SecretNames:OnlineSql"];

            var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

            Console.WriteLine($"Retrieving your secrets from Key Vault.");
            var sqlsecret = await client.GetSecretAsync(sqlSecretName);
            var blobsecret = await client.GetSecretAsync(blobStringName);
            var blobcontainersecret = await client.GetSecretAsync(blobContainerName);
            var jwtSecret = await client.GetSecretAsync(jwtSecretName);
            var sqlonlieSecret = await client.GetSecretAsync(onlinesql);

            #region Log4net
            builder.Services.AddLogging(l => l.AddLog4Net());
            #endregion

            #region JWT-Authentication-Injection
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret.Value.Value)),
                    };
                });
            #endregion

            #region Context
            builder.Services.AddDbContext<QuizzContext>(
              options => options.UseSqlServer(sqlonlieSecret.Value.Value)
            );
            #endregion

            #region Repository
            builder.Services.AddScoped<IRepository<int, User>, UserRepository>();
            builder.Services.AddScoped<IRepository<int, Certificate>, CertificateRepository>();
            builder.Services.AddScoped<IRepository<int, Favourite>, FavouriteRepository>();
            builder.Services.AddScoped<IRepository<int, Option>, OptionRepository>();
            builder.Services.AddScoped<IRepository<int, Question>, QuestionRepository>();
            builder.Services.AddScoped<IRepository<int, TestHistory>, TestHistoryRepository>();
            builder.Services.AddScoped<IRepository<int, Submission>, SubmissionRepository>();
            builder.Services.AddScoped<IRepository<int, CertificationTest>, CertificationTestRepository>();
            builder.Services.AddScoped<IRepository<int, SubmissionAnswer>, SubmissionAnswerRepository>();
            builder.Services.AddScoped<IRepository<string, UserCredential>, UserCredentialRepository>();
            builder.Services.AddScoped<ITransactionService, TransactionRepository>();
            builder.Services.AddScoped<CertificationTestQuestionRepository, CertificationTestQuestionRepository>();
            builder.Services.AddScoped<SubmissionTestQuestionRepository, SubmissionTestQuestionRepository>();
            builder.Services.AddScoped<SubmissionAnswerQuestionOnly, SubmissionAnswerQuestionOnly>();
            builder.Services.AddScoped<HistoryWithUserRepository, HistoryWithUserRepository>();
            builder.Services.AddScoped<UserHistoryFavouriteRepository, UserHistoryFavouriteRepository>();
            builder.Services.AddScoped<UserFavouriteRepository, UserFavouriteRepository>();
            builder.Services.AddScoped<UserHistoryTest, UserHistoryTest>();
            builder.Services.AddScoped<UserTestHIstoryRepository, UserTestHIstoryRepository>();
            builder.Services.AddScoped<UserFavouriteTestRepository, UserFavouriteTestRepository>();
            builder.Services.AddScoped<CertificationTestOnlyQuestionRepo, CertificationTestOnlyQuestionRepo>();
            #endregion

            #region Service
            builder.Services.AddScoped<ITokenService>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<TokenService>>();
                return new TokenService(jwtSecret.Value.Value, logger);
            });
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<ITestService, TestService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBlobService>(provider =>
            {
                return new BlobService(blobcontainersecret.Value.Value, blobsecret.Value.Value);
            });
            #endregion

            #region CORS
            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("MyCors", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
            #endregion

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("MyCors");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
