using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.FileReader.Abstract
{
    public interface IFileStreamReader
    {
        List<string> ReadAllLines(string filePath);
    }
}
