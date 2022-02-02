using Business.Abstract;
using Business.Concrete;
using Business.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace HepsiBurada
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var businessDependencies = BusinessDependencies.CreateHostBuilder(args).Build();
                var commandService = businessDependencies.Services.GetService<ICommandService>();
                commandService.DefineCommand();
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
