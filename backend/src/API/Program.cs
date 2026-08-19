using API.Filters;
using Application;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
// ==========================================
// 启用 Npgsql 旧版时间戳行为
// 必须放在数据库上下文注册之前，确保 EF Core 在初始化时能读取到该配置
// ==========================================
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

// ==========================================
// 1. 服务注册 (Services Registration)
// ==========================================

// 1.1 添加控制器支持 + 全局过滤器（统一响应包装、异常处理、操作日志）
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ResultWrapperFilter>();
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<OperationLogFilter>();
})
.AddJsonOptions(options =>
{
    // 配置 JSON 序列化使用 camelCase（与前端 TypeScript 接口保持一致）
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; // 忽略属性名大小写敏感
});

// 1.2 注册数据库上下文 (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

    // 优化：在开发环境下，将 EF Core 生成的 SQL 语句打印到控制台，方便调试
    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
});

// 1.3 【核心修复】：将 IApplicationDbContext 接口映射到具体的 AppDbContext 实例
// ⚠️ 注意：如果没有这一行，UserService 注入 IApplicationDbContext 时会报错，导致后端直接闪退！
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<AppDbContext>());

// 1.4 注册应用层业务服务
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDictService, DictService>();
builder.Services.AddScoped<IOrgService, OrgService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IHealthArchiveService, HealthArchiveService>();
builder.Services.AddScoped<IOperationLogService, OperationLogService>();
builder.Services.AddScoped<ILoginLogService, LoginLogService>();

// 1.4.1 IP 归属地查询（基于 ip2region 离线库）
//   xdb 文件由 Infrastructure.csproj 配置 CopyToOutputDirectory，发布后位于 bin 根目录
var xdbPath = Path.Combine(AppContext.BaseDirectory, "ip2region_v4.xdb");
if (!File.Exists(xdbPath))
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"[ip2region] 警告：未找到 xdb 文件 {xdbPath}，IP 归属地查询将退化为「未知」");
    Console.ResetColor();
}
builder.Services.AddIpLocation(xdbPath);

// 1.5 添加 Swagger/OpenAPI 支持 (强烈建议：方便脱离前端，直接在浏览器测试接口)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1.6 配置跨域 (CORS) - 前后端分离项目必备
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // 开发环境允许所有来源，生产环境建议替换为具体的前端域名
        // 注意：AllowAnyOrigin() 和 AllowCredentials() 互斥（CORS 规范不允许），
        //      启用 credentials 时必须列出具体 origin。
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ==========================================
// 2. 中间件管道配置 (Middleware Pipeline)
// ==========================================

// 2.1 开发环境专属中间件
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // 启动后访问 /swagger 即可查看接口文档
    app.UseDeveloperExceptionPage(); // 显示详细的报错页面
}

// 2.2 启用跨域 (必须放在 UseRouting 和 UseAuthorization 之前)
app.UseCors("AllowFrontend");

// 2.3 启用 wwwroot 静态文件服务（用于访问上传的头像 /uploads/avatars/xxx.png）
app.UseStaticFiles();

// 2.4 路由与鉴权
app.UseRouting();
// app.UseAuthentication(); // 如果后续加了 JWT 登录鉴权，请取消这行的注释
app.UseAuthorization();

// 2.5 映射控制器路由
app.MapControllers();

// ==========================================
// 3. 数据库迁移 (推荐做法：使用 EF Core Migrations)
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // 使用 Migrate() 而非 EnsureCreated()，这样已有的 Migrations/InitDatabase.cs 会真正被执行
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // 如果数据库连接失败，在控制台输出明显提示
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[数据库错误] 无法连接到 PostgreSQL 数据库: {ex.Message}");
        Console.ResetColor();
    }
}

app.Run();
