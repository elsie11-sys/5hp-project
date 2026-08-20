using System;
using System.Linq;
using System.Reflection;

var asm = Assembly.LoadFrom(@"E:\5HP_Project\backend\src\Infrastructure\bin\Debug\net8.0\Infrastructure.dll");
foreach (var t in asm.GetExportedTypes())
{
    if (t.Name.Contains("InitCollect") || t.Name.Contains("Migration") || t.Name.Contains("Snapshot") || t.Name == "AppDbContext")
        Console.WriteLine(t.FullName);
}
Console.WriteLine("---all migration types (incl non-exported):---");
foreach (var t in asm.GetTypes())
{
    if (typeof(Microsoft.EntityFrameworkCore.Migrations.Migration).IsAssignableFrom(t))
        Console.WriteLine(t.FullName);
}
