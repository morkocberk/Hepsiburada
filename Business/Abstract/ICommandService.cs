using Core.Constant;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface ICommandService
    {
        void DefineCommand();
        void ExecuteCommand(string command, COMMAND_TYPE type);
        COMMAND_TYPE ValidateCommand(string command);
    }
}
