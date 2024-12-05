using api.Data;
using api.Interfaces;
using api.Models;
using api.Repository;
using api.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers(); // Add Controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


builder.Services.AddControllers().AddNewtonsoftJson(options => {
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;  
});

// DB
builder.Services.AddDbContext<ApplicationDBContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Identity 
builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<ApplicationDBContext>(); // Identity의 데이터 저장소로 ApplicationDBContext 사용하도록 지정

// Authentication
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = // 기본 인증 스키마 설정 ex. 사용자가 요청을 보낼때 인증 토큰 검증
    options.DefaultChallengeScheme =  // 인증 실패시 
    options.DefaultForbidScheme = // 인증은 되었지만 권한부족 상태일때 사용할 기본 스키마 설정
    options.DefaultScheme = // 모든 요청에서 사용할 기본 인증 스키마 설정
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;  // 위의 기본 스키마들에 JWT Bearer 인증 사용하도록 지정
}).AddJwtBearer(options => { // JWT 인증방식 추가 구성
    options.TokenValidationParameters = new TokenValidationParameters // JWT 토큰 검증에 필요한 다양한 설정을 정의
    {
        ValidateIssuer = true, // 토큰 발급자 검증
        ValidIssuer = builder.Configuration["JWT:Issuer"], // 유효한 발급자 설정. 발급자는 appsettings.json의 JWT:Issuer 값
        ValidateAudience = true, // 토큰의 대상 검증
        ValidAudience = builder.Configuration["JWT:Audience"], // 유효한 대상 설정. JWT:Audience 값
        ValidateIssuerSigningKey = true, // 토큰 서명이 유효한지 확인
        IssuerSigningKey = new SymmetricSecurityKey( // 서명 검증위해 대칭키를 설정
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]) // 대칭키는 JWT:SigningKey 값.
        )
    };
});

// Interface, Instance
builder.Services.AddScoped<IStockRepository, StockRepository>(); 
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped<IPortfolioRepository,PortfolioRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // 클라이언트가 http로 요청을 보낼경우 https url로 변환하여 안전하게 사용하도록 강제함

// Authentication, Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Add Controllers

app.Run();


