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
            string dbname = args[0];
            string strTableName = args[1].Split("\\")[1].Split(".")[0];
            string strTablePureName = strTableName.Substring(3, strTableName.Length-3);
            List<clsInputText> lstInput = clsManager.getParameters(@"i:\co_cuenta_contable.txt");
            string rs=clsManager.generateSpi(lstInput, strTableName, strTablePureName, dbname);
            string rss = clsManager.generateSps(lstInput, strTableName, strTablePureName, dbname+".dbo."+ strTableName);
            string rsas = clsManager.generateSpu(lstInput, strTableName, strTablePureName, dbname + ".dbo." + strTableName);
            Console.WriteLine(rs+rss+rsas);
        }
    }
}
