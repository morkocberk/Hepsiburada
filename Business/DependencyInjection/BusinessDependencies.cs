using Business.Abstract;
using Business.Concrete;
using Core.Utilities.FileReader.Abstract;
using Core.Utilities.FileReader.Concrete;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.DependencyInjection
{
    public static class BusinessDependencies
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ICommandService, CommandService>();
                    services.AddSingleton<IFileStreamReader, FileStreamReader>();
                });

        }
    }
}
