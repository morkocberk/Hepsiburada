using Business.Abstract;
using Business.Concrete;
using Business.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test.Services
{
    public class CommandServiceTest
    {
        private readonly ICommandService _commandService;

        public CommandServiceTest()
        {

            var businessDependencies = BusinessDependencies.CreateHostBuilder(null).Build();
            _commandService = businessDependencies.Services.GetService<ICommandService>();
        }

        [Fact]
        public void ExtractExpectedOutput()
        {
            _commandService.CreateCommandWatcher();
            //Assert.Equal()
        }
    }
}
