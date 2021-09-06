using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMSTROKE
{
    public static class clsManager
    {
        public static List<clsInputText> getParameters(string path)
        {
            List<clsInputText> lstInput = new List<clsInputText>();
            string strPrefijo = string.Empty;
            using (StreamReader file = new StreamReader(path))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    strPrefijo = line.Substring(0,2);
                    clsInputText tupla = new clsInputText();
                    tupla.field = line.Split('#')[0];
                    tupla.tipe = line.Split('#')[1];
                    tupla.classification = line.Split('#')[2];
                    tupla.bit = int.Parse(line.Split('#')[3]);
                    tupla.comment = line.Split('#')[4];
                    lstInput.Add(tupla);
                }
                file.Close();
            }
            clsInputText tupla_fecha_creacion = new clsInputText();
            tupla_fecha_creacion.field = strPrefijo+"_fecha_creacion";
            tupla_fecha_creacion.tipe = "DATETIME";
            tupla_fecha_creacion.classification = "N";
            tupla_fecha_creacion.bit = 2;
            tupla_fecha_creacion.comment = "FECHA DE CREACION";
            lstInput.Add(tupla_fecha_creacion);
            clsInputText tupla_fecha_modificacion = new clsInputText();
            tupla_fecha_modificacion.field = strPrefijo+"_fecha_modificacion";
            tupla_fecha_modificacion.tipe = "DATETIME";
            tupla_fecha_modificacion.classification = "N";
            tupla_fecha_modificacion.bit = 2;
            tupla_fecha_modificacion.comment = "FECHA DE CREACION";
            lstInput.Add(tupla_fecha_modificacion);
            return lstInput;
        }

        public static string buildParams(List<clsInputText> lstInput, int intBit)
        {
            string rs ="\n";

            foreach (var item in lstInput)
            {
                string prefijo = item.field.Substring(0, 3);

                if ((item.bit & intBit) == intBit)
                {
                    if (item.classification == "P")
                        rs += "\t" + item.field.Replace(prefijo, "@i_") + " " + item.tipe + ",\n";
                    else if (item.classification == "F")
                        rs += "\t" + item.field.Replace(prefijo, "@i_") + " " + item.tipe + ",\n";
                    else
                        rs += "\t" + item.field.Replace(prefijo, "@i_") + " " + item.tipe + ",\n";
                }
            }

            rs=rs.Substring(0, rs.Length - 2);
            rs = rs + "\n";
            return rs;
        }

       

        public static string buildParamsInsert_a(List<clsInputText> lstInput)
        {
            string rs = "\n";


            string prefijo = string.Empty;
            foreach (var item in lstInput)
            {
                prefijo = item.field.Substring(0, 3);
                if (item.classification == "P" || item.classification == "F")
                    rs += "\t\t" + item.field+",\n";
                else if ((item.bit & 0x01) == 0x01)
                {
                    rs += "\t\t" + item.field + ",\n";
                }
            }
            rs += "\t\t" + prefijo+"fecha_creacion" + ",\n";

            rs = rs.Substring(0, rs.Length - 2);
            rs = rs + "\n";
            return rs;
        }


        public static string buildUpdateBody(List<clsInputText> lstInput)
        {
            string rs = "\n";


            string prefijo = string.Empty;
            foreach (var item in lstInput)
            {
                prefijo = item.field.Substring(0, 3);
                if (item.classification == "P" || item.classification == "F")
                    rs += "\t\t\t" + item.field+"="+ item.field.Replace(prefijo, "@i_") + ",\n";
                else if ((item.bit & 0x01) == 0x01)
                {
                    rs += "\t\t\t" + item.field + "=" + item.field.Replace(prefijo, "@i_") + ",\n";
                }
            }
            rs += "\t\t\t" + prefijo + "fecha_modificacion=GETDATE()" + ",\n";

            rs = rs.Substring(0, rs.Length - 2);
            rs = rs + "\n";
            return rs;
        }



        public static string buildCustomSelect(List<clsInputText> lstInput)
        {
            string rs = "\n";


            string prefijo = string.Empty;
            foreach (var item in lstInput)
            {
                prefijo = item.field.Substring(0, 3);
                string eval = string.Empty;
                if (item.tipe.Contains("INT") || item.tipe.Contains("DECIMAL") || item.tipe.Contains("MONEY") || item.tipe.Contains("LONG") || item.tipe.Contains("SHORT"))
                {
                    eval = "0";
                }
                else
                    eval = "''";



                if (item.classification == "P" || item.classification == "F")
                    rs += "\t\t(" + item.field + "="+ item.field.Replace(prefijo, "@i_") + " or "+ item.field.Replace(prefijo, "@i_") +"="+eval+ ")AND\n";
                else if ((item.bit & 0x01) == 0x01)
                {
                    rs += "\t\t(" + item.field + "=" + item.field.Replace(prefijo, "@i_") + " or " + item.field.Replace(prefijo, "@i_") + "=" + eval + ")AND\n";
                }
            }
            rs = rs.Substring(0, rs.Length - 4);
            rs = rs + "\n";
            return rs;
        }


        public static string BuildUpdateBlock(List<clsInputText> lstInput)
        {
            string rs = "\n";


            string prefijo = string.Empty;
            foreach (var item in lstInput)
            {
                prefijo = item.field.Substring(0, 3);
                string eval = string.Empty;
                if (item.tipe.Contains("INT") || item.tipe.Contains("DECIMAL") || item.tipe.Contains("MONEY") || item.tipe.Contains("LONG") || item.tipe.Contains("SHORT"))
                {
                    eval = "0";
                }
                else
                    eval = "''";


                //@i_identificacion=case when isnull(@i_identificacion,'' )='' then cl_identificacion else @i_identificacion end,
                if (item.classification == "P" || item.classification == "F")
                    rs += "\t\t"+ item.field.Replace(prefijo, "@i_")+"=case when is null("+ item.field.Replace(prefijo, "@i_")+","+ eval + ")="+ eval + " then "+ item.field + " else " + item.field.Replace(prefijo, "@i_") + " end,\n";
                else if ((item.bit & 0x01) == 0x01)
                {
                    rs += "\t\t" + item.field.Replace(prefijo, "@i_") + "=case when is null(" + item.field.Replace(prefijo, "@i_") + "," + eval + ")=" + eval + " then " + item.field + " else " + item.field.Replace(prefijo, "@i_") + " end,\n";
                }
            }
            rs = rs.Substring(0, rs.Length - 2);
            rs = rs + "\n";
            return rs;
        }


        public static string buildParamsInsert_b(List<clsInputText> lstInput)
        {
            string rs = "\n";

            rs += "\t\t@w_ha_id_principal,\n";
            string prefijo = string.Empty;
            foreach (var item in lstInput)
            {
                if ((item.bit & 0x01) == 0x01)
                {
                    prefijo = item.field.Substring(0, 3);
                    rs += "\t\t" + item.field.Replace(prefijo, "@i_") + ",\n";   
                }   
            }
            rs += "\t\t@w_fecha_ingreso,\n";
            rs = rs.Substring(0, rs.Length - 2);
            return rs;
        }

        public static string GetGenericWhere(List<clsInputText> lstInput,string strTabs)
        {
            string prefijo = string.Empty;
            string primary = string.Empty;
            foreach (var item in lstInput)
            {
                if ((item.bit & 0x02) == 0x02)
                {
                    if (item.classification == "P")
                        primary = item.field;
                    prefijo = item.field.Substring(0, 3);
                }
            }
            return "\n"+ strTabs + primary + "=@w_ha_id_principal\n";

        }

        public static string buildParamsInsert_select(List<clsInputText> lstInput, string strTable)
        {
            string prefijo = string.Empty;
            string primary = string.Empty;
            string rs = "\tIF @@ROWCOUNT==1\n";
            rs = rs += "\t\tSELECT\n";

            foreach (var item in lstInput)
            {
                if ((item.bit & 0x02) == 0x02)
                {
                    if (item.classification == "P")
                        primary = item.field;
                    prefijo = item.field.Substring(0, 3);
                    rs += "\t\t\t" + item.field + ",\n";
                }
            }
            rs = rs.Substring(0, rs.Length - 2);
            rs = rs + "\n";
            rs += "\t\tFROM\n\t\t\tdbo."+strTable+"\n";
            if(!primary.Equals(string.Empty))
                rs += "\t\tWHERE\n\t\t\t"+primary+ "=@w_ha_id_principal\n";
            return rs;
        }


        public static string generateSpi(List<clsInputText> lstInput, string strTableName, string strTablePureName, string dbname)
        {
            string strIn = clsManager.buildParams(lstInput,0x01);
            #region definitions DB
            string strSpiHeader = "CREATE PROCEDURE spi_{0} ({1})\nAS\n";
            string strSpiAutoincrement = "\tDECLARE\n\t\t@w_fecha_ingreso  datetime=GETDATE(),\n\t\t@_retorno int = 1,\n\t\t@w_ha_id_cuenta INT,\n\t\t@w_retorno INT\n\tEXEC @w_retorno =pr_administracion.dbo.sp_secuencial\n\t@i_base   = '{0}',\n\t@i_tabla        = '{1}',\n\t@o_siguiente    = @w_ha_id_principal out\n\tIF (@w_retorno != 0)\n\t\treturn @w_retorno\n";
            string strSpiInsert = "\tINSERT INTO dbo.{0}(\n{1}\n\t)VALUES(\n{2}\t\n\t)\n";

            #endregion



            string insert_a = clsManager.buildParamsInsert_a(lstInput);
            string insert_b = clsManager.buildParamsInsert_b(lstInput);
            string insert_select = clsManager.buildParamsInsert_select(lstInput, strTableName);

            StringBuilder strStack = new StringBuilder();
            StringBuilder strStackAuto = new StringBuilder();
            StringBuilder strStackInsert = new StringBuilder();
            strStack.AppendFormat(strSpiHeader.ToString(), strTablePureName, strIn);
            strStackAuto.AppendFormat(strSpiAutoincrement.ToString(), dbname, strTableName);
            strStackInsert.AppendFormat(strSpiInsert.ToString(), dbname, insert_a, insert_b);
            return strStack.ToString() + strStackAuto.ToString() + strStackInsert.ToString() + insert_select;
        }

        public static string generateSps(List<clsInputText> lstInput, string strTableName, string strTablePureName, string dbname)
        {
            string strIn = clsManager.buildParams(lstInput, 0x06);
            #region definitions DB
            string strSpiHeader = "CREATE PROCEDURE sps_{0} ({1})\nAS\n";
            
            string strSpiInsert = "\tSELECT{1}\n\tFROM\n\t\t{0}\t\n\tWHERE{2}\n";

            #endregion



            string insert_a = clsManager.buildParamsInsert_a(lstInput);
            string insert_b = clsManager.buildParamsInsert_b(lstInput);
            string parametrizacion = clsManager.buildCustomSelect(lstInput);

            string kk = BuildUpdateBlock(lstInput);
            string ssa = clsManager.buildParamsInsert_select(lstInput, strTableName);

            StringBuilder strStack = new StringBuilder();
            StringBuilder strStackAuto = new StringBuilder();
            StringBuilder strStackInsert = new StringBuilder();
            strStack.AppendFormat(strSpiHeader.ToString(), strTablePureName, strIn);
            
            strStackInsert.AppendFormat(strSpiInsert.ToString(), dbname, insert_a, parametrizacion);
            return strStack.ToString() + strStackInsert.ToString();
        }

        public static string generateSpu(List<clsInputText> lstInput, string strTableName, string strTablePureName, string dbname)
        {
            string strIn = clsManager.buildParams(lstInput, 0x06);
            #region definitions DB
            string strSpuHeader = "CREATE PROCEDURE spu_{0} ({1})\nAS\n";
            string strSelectTemplate = "\tSELECT{1}\n\tFROM\n\t\t{0}\t\n\tWHERE{2}\n";

            string strUpdateTemplate = "\tBEGIN\n\t\tUPDATE\n\t\t\t{1}\n\t\tSET\t\t\t{0}\t\tWHERE\t{2}\n\tEND";
            #endregion

            string insert_a = clsManager.buildParamsInsert_a(lstInput);
            string insert_b = clsManager.buildParamsInsert_b(lstInput);
            string parametrizacion = clsManager.buildCustomSelect(lstInput);

            string strWhere = GetGenericWhere(lstInput,"\t\t\t");

            string update = buildUpdateBody(lstInput);

            StringBuilder strStack = new StringBuilder();
            StringBuilder strStackInsert = new StringBuilder();
            StringBuilder strStackUpdate = new StringBuilder();
            strStack.AppendFormat(strSpuHeader.ToString(), strTablePureName, strIn);
            strStackInsert.AppendFormat(strSelectTemplate.ToString(), dbname, parametrizacion, strWhere);
            strStackUpdate.AppendFormat(strUpdateTemplate.ToString(), update, dbname, strWhere);
            string kk = strStackUpdate.ToString();
            String rs= strStack.ToString() + strStackInsert.ToString();
            return string.Empty;
        }
    }
}
