////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Threading.Tasks;
////using Microsoft.AspNetCore.Hosting;
////using Microsoft.Extensions.Configuration;
////using Microsoft.Extensions.Hosting;
////using Microsoft.Extensions.Logging;
////using Microsoft.EntityFrameworkCore;
////using MongoDB.Driver;
////using Microsoft.EntityFrameworkCore;
////using MongoDB.Bson;
////using MongoDB.Driver;
////using MongoDB.EntityFrameworkCore.Extensions;

//namespace Appblueprint.SharedKernel;
//    //public class Program
//    //{
//    //    public static void Main(string[] args)
//    //    {

//    //        var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
//    //        if (connectionString is null)
//    //        {
//    //            Console.WriteLine("You must set your 'MONGODB_URI' environment variable. To learn how to set it, see https://www.mongodb.com/docs/drivers/csharp/current/quick-start/#set-your-connection-string");
//    //            Environment.Exit(0);
//    //        }
//    //        var client = new MongoClient(connectionString);
//    //        //var db = MflixDbContext.Create(client.GetDatabase("sample_mflix"));
//    //        //var movie = db.Movies.First(m => m.title == "Back to the Future");
//    //        //Console.WriteLine(movie.plot);

//    //        


//    //        //app.AddEfDiagrams<DbContext>();


//    //        //var optionsBuilder = new DbContextOptionsBuilder<SaaSTemplateProjectDbContext>();
//    //        //optionsBuilder("Server=localhost;Database=SaaSTemplateProjectDb;Trusted_Connection=True;MultipleActiveResultSets=true");


//    //        //CreateHostBuilder(args).Build().Run();

//    //        //var optionsBuilder = new DbContextOptionsBuilder<SaaSTemplateProjectDbContext>();
//    //        //optionsBuilder.UseSqlServer("Server=localhost;Database=SaaSTemplateProjectDb;Trusted_Connection=True;MultipleActiveResultSets=true");
//    //        //using (var context = new SaaSTemplateProjectDbContext(optionsBuilder.Options))
//    //        //{
//    //        //    // do stuff
//    //        //    var tenants = context.Tenants.ToList();
//    //        //    Console.WriteLine(tenants.Count);
//    //        //}

//    //        //var optionsBuilder = new DbContextOptionsBuilder<SaaSTemplateProjectDbContext>();
//    //        //optionsBuilder.UseSqlServer("Server=localhost;Database=SaaSTemplateProjectDb;Trusted_Connection=True;MultipleActiveResultSets=true");
//    //        //using (var context = new SaaSTemplateProjectDbContext(optionsBuilder.Options))
//    //        //{
//    //        //    // do stuff
//    //        //    var tenants = context.Tenants.ToList();
//    //        //    Console.WriteLine(tenants.Count);
//    //        //}

//    //        //var optionsBuilder = new DbContextOptionsBuilder<SaaSTemplateProjectDbContext>();
//    //        //optionsBuilder.UseSqlServer("Server=localhost;Database=SaaSTemplateProjectDb;Trusted_Connection=True;MultipleActiveResultSets=true");
//    //        //using (var context = new SaaSTemplateProjectDbContext(optionsBuilder.Options))
//    //        //{
//    //        //    // do stuff
//    //        //    var tenants = context.Tenants.ToList();
//    //        //    Console.WriteLine(tenants.Count);
//    //        //}

//    //        //var optionsBuilder = new DbContextOptionsBuilder<SaaSTemplateProjectDbContext>();
//    //        //optionsBuilder.UseSqlServer("Server=localhost;Database=SaaSTemplateProjectDb;Trusted_Connection=True;MultipleActiveResultSets=true");
//    //        //using (var context = new SaaSTemplateProjectDbContext(optionsBuilder.Options))
//    //        //{
//    //        //    // do stuff
//    //        //    var tenants = context.Tenants.ToList();
//    //        //    Console.WriteLine(tenants.Count);
//    //        //}

//    //        //var optionsBuilder = new DbContextOptionsBuilder<SaaSTemplateProjectDbContext>();


//    //    }
//    //}



