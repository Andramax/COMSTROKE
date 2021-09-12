using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace COMSTROKE
{
    class Program
    {
        static void Main(string[] args)
        {

            Dictionary<string,string> parametros= new Dictionary<string, string>();


            string dbname = string.Empty;
            string sourceFile = string.Empty;
            string destinationFile = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                if (args[(i * 2)] == "-b")
                    dbname = args[(i * 2) + 1];

                if (args[(i * 2)] == "-o")
                {
                    sourceFile = args[(i * 2) + 1];
                    
                }
                if (args[(i * 2)] == "-d")
                {
                    destinationFile = args[(i * 2) + 1];
                }

            }
            string strTableName = sourceFile.Split("\\")[sourceFile.Split("\\").Length - 1].Split(".")[0];
            string strTablePureName = strTableName.Substring(3, strTableName.Length-3);
            List<clsInputText> lstInput = clsManager.getParameters(sourceFile);
            string script = clsManager.Generate(lstInput, strTableName, strTablePureName, dbname);
            File.WriteAllText(destinationFile, script);
        }
    }
}
