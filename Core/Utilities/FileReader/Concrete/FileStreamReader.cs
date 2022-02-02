using Core.Utilities.FileReader.Abstract;
using System;
using System.Collections.Generic;
using System.IO;

namespace Core.Utilities.FileReader.Concrete
{
    public class FileStreamReader : IFileStreamReader
    {
        public List<string> ReadAllLines(string filePath)
        {            
            try
            {
                string line;
                var lineList = new List<string>();
                using (StreamReader sr = new StreamReader(filePath))
                {                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        lineList.Add(line);
                    }
                }
                return lineList;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
