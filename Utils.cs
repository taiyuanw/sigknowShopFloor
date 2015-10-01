using System;
using System.Collections.Generic;
using System.Windows;
using MySql.Data.MySqlClient;


namespace SigknowShopFloor
{

    public class Global
    {
        public static string gEMPTY = "";
        public static string gUsername = "";
        public static string gStation = "";
        public static string gResult = "";
        public const string gTableName = "shopfloor";
        public const string gUpdateHistoryTableName = "updatehistory";
        public const string gBOXPREFIX = "sx";
        public static string gPCBASN = "";
        public static string gSIGKNOWSN = "";
        public static string gOLDSIGKNOWSN = "";
        public static string gBOXSN;
        public static string gSNAction = ""; // action code for SN association : associate / disassociate / query
        public const string OK = "OK";
        public const string NG = "NG";
        public const int gMaxRecycleCount = 20;
        public static bool gREWORK;  // flag : indicate second (or more) run on the pcba board in the same station
        public static bool gSKIP;    // flag : indicate if REWORK can be skipped (test result is identical to db)
        public static bool gINITIALRUN; // flag : indicate the testresult is input for the first time
        public const string gBOXTIMEINTERVAL = " BOXTIME >= date_sub(now(),interval 20 day) ";  // MySQL : 20 day interval
    }

    public class Barcode
    {
        public const string ASSOCIATE = "S001K";
        public const string DISASSOCIATE = "S002K";
        public const string QUERY = "S003K";
        public const string RESET = "S004K";
        public const string OK = "S005K";
        public const string NG = "S006K";
    }

    public class StationName
    {
        public const string gStationA = "PCBA板功能檢測";
        public const string gStationB = "電池供電測試";
        public const string gStationC = "PCBA板初始化";
        //public const string gStationD = "電極導通";
        public const string gStationE = "上蓋下蓋關連";
        public const string gStationF = "標籤檢測";
        public const string gStationG = "裝箱";

        public const string gStationEassociation = "建立關連";
        public const string gStationEdisassociation = "解除關連";
        //public const string gStationEquery = "查詢關連";

    }

    public class DBColPrefix
    {
        public const string gStationA = "A";
        public const string gStationB = "B";
        public const string gStationC = "C";
        //public const string gStationD = "D";
        public const string gStationE = "E";
        public const string gStationF = "F";
        public const string gStationG = "G";
    }

    public class DBColPostfix
    {
        public const string gColname = "RESULT";
        public const string gTime = "TIME";
        public const string gUser = "USERNAME";
    }

    public class ShopFloorLabel
    {
        public const string OperaterName = "輸入操作員姓名 : ";
        public const string SN = "上蓋或PCBA序號 (擇一) : ";
        public const string PCBASN = "輸入PCBA序號 : ";
        public const string RESULT = "輸入測試結果 : ";
        public const string SIGKNOWSN = "輸入上蓋序號 : ";
        public const string BOXSN = "輸入外箱序號 : ";

        public const string SelectAction = "掃描指令條碼 : ";
        public const string Association = "建立關連";
        public const string Disassociation = "解除關連";
        public const string QueryAssociation = "查詢關連";
    }

    public class SigknowDBServer
    {
        public static string gServer = Global.gEMPTY;
        public static string gUser = "ug01";
        public static string gPassword = "Sigkn0w";
        public static string gDatabase = "sigknow";
    }

    class Utils
    {
        public static void OKBeep()
        {
            Console.Beep(4000, 200);
            //Thread.Sleep(50);
            Console.Beep(5000, 200);
        }
        public static void ErrorBeep()
        {
            Console.Beep(1000, 1000);
            //Console.Beep(1000, 500);
        }
        public static string CurrentTime()
        {
            DateTime theDate = DateTime.Now;
            return theDate.ToString("yyyy-MM-dd H:mm:ss");
        }
        public static void ValidateSN(string pcbasn, params string[] cols)
        {
            List<string> resultlist = new List<string>();
            if (pcbasn.Substring(0, 1).ToUpper().Contains("T") ||
                pcbasn.Substring(0, 1).ToUpper().Contains("C") ||
                pcbasn.Substring(0, 1).ToUpper().Contains("B"))
            {

            }
            else
                throw new InvalidSerialNumberException("PCBA序號 '" + pcbasn + "' 不符合規格.");

            if (Global.gStation == StationName.gStationA)
            {
                if (!Utils.SNPrecheck(pcbasn, "", ref resultlist))
                {
                    throw new PreviousErrorException("前一站檢驗未通過.");
                }

            }
            else if (Global.gStation == StationName.gStationB)
            {
                if (!Utils.SNPrecheck(pcbasn, "", ref resultlist))
                {
                    throw new PreviousErrorException("前一站檢驗未通過.");
                }

            }
            else
            {
                resultlist.Clear();
                for (int i = 0; i < cols.Length; i++)
                {
                    if (!Utils.SNPrecheck(pcbasn, cols[i], ref resultlist))
                    {
                        throw new PreviousErrorException("前一站檢驗未通過.");
                    }
                }
            }
        }
        public static void ValidateResult(string res)
        {
            if ((res == Barcode.OK) || (res == Barcode.NG))
            {

            }
            else
            {
                throw new InvalidSerialNumberException("'" + res + "'是無效的條碼.");
            }
        }
        public static void ValidateResult(string station, string pcbasn, string result)
        {
            var cmd = " select " + station + DBColPostfix.gColname + " ,ARESULT " +
                " from " + Global.gTableName +
                " where PCBASN = '" + pcbasn + "'" +
                " and (" + Global.gBOXTIMEINTERVAL  + " || BOXSN is NULL) order by id desc limit 1;";
            MySQLDB.DBconnect();
            MySqlCommand sqlcmd = MySQLDB.command(cmd);
            MySqlDataReader reader = sqlcmd.ExecuteReader();
            if ((reader.HasRows))
            {
                while (reader.Read())
                {

                    if (station == DBColPrefix.gStationA)
                    {
                        Global.gREWORK = true;
                    }

                    if (station == DBColPrefix.gStationB)
                    {
                        if (!reader.IsDBNull(0) || !reader.IsDBNull(1))
                        {
                            Global.gREWORK = true;
                        }
                    }
                    
                    if (!reader.IsDBNull(0))
                    {
                        var val = reader.GetString(0);
                        if (val.Length > 0)
                        {
                            Global.gSKIP = (((reader.GetString(0) == "1") ? Barcode.OK : Barcode.NG) == result)
                                ? true
                                : false;
                            Global.gINITIALRUN = false;
                        }
                        else
                        {
                            Global.gSKIP = false;
                            Global.gINITIALRUN = true;
                        }
                        //MessageBox.Show("result: " + reader.GetString(0) + "\n\nrework: " + Global.gREWORK + "\nskip: " + Global.gSKIP + "\ninitrun: " + Global.gINITIALRUN);
                    }
                    else
                    {
                        Global.gSKIP = false;
                        if ((station != DBColPrefix.gStationA) && (station != DBColPrefix.gStationB))
                            Global.gINITIALRUN = true;
                    }
                }
            }
            else if ((station == DBColPrefix.gStationA) || (station == DBColPrefix.gStationB))
            {
                Global.gSKIP = false;
                Global.gINITIALRUN = true;
                Global.gREWORK = false;
            }
            else
            {
                MySQLDB.DBdisconnect();
                throw new Exception("序號尚未存在系統.");
            }
            MySQLDB.DBdisconnect();
        }

        public static void ValidateBoxSN(string boxsn)
        {
            if (
                String.Compare(boxsn.Substring(0, Global.gBOXPREFIX.Length).ToUpper(), Global.gBOXPREFIX.ToUpper()) !=
                0)
            {
                throw new InvalidSerialNumberException();
            }
        }

        public static void ValidateResultBoxing(string boxsn, string sn)
        {
            var cmd = " select BOXSN, BOXTIME, BOXUSERNAME " +
                " from " + Global.gTableName +
                " where SIGKNOWSN = '" + sn + "'" +
                " and " + Global.gBOXTIMEINTERVAL + " order by id desc limit 1;";
            MySQLDB.DBconnect();
            MySqlCommand sqlcmd = MySQLDB.command(cmd);
            MySqlDataReader reader = sqlcmd.ExecuteReader();
            if ((reader.HasRows))
            {
                while (reader.Read())
                {
                    var val = reader.GetString(0);
                    if (val.Length > 0)
                    {
                        Global.gSKIP = (reader.GetString(0) == boxsn);
                        Global.gINITIALRUN = false;
                    }
                    else
                    {
                        Global.gSKIP = false;
                        Global.gINITIALRUN = true;
                    }
                }
            }
            else
            {
                Global.gINITIALRUN = true;
                Global.gSKIP = false;
            }
            MySQLDB.DBdisconnect();
        }


        public static string barcode2dbbool(string input)
        {
            /***
             * convert string "OK" to string "true/false" (used by MySQL)
             */
            if (input == Barcode.OK)
            {
                return "true";
            }
            else if (input == Barcode.NG)
            {
                return "false";
            }
            else
            {
                throw new Exception("不正確的條碼");
            }
        }

        public static bool SNPrecheck(string pcbasn, string colname, ref List<string> colvalue)
        {
            var cmd = "";
            var recycled = 99;
            var result = true;
            colvalue.Clear();
            try
            {
                //// 攔下已出貨裝箱 20 次的 (裝箱日期不是 NULL)
                cmd = "select count(*) from " + Global.gTableName + " where pcbasn = '" + pcbasn + "' and boxsn is not NULL;";
                MySQLDB.DBconnect();
                MySqlCommand sqlcmd = MySQLDB.command(cmd);
                MySqlDataReader reader = sqlcmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        recycled = reader.GetInt16(0);
                    }
                }
                MySQLDB.DBdisconnect();
                if (recycled > Global.gMaxRecycleCount)
                {
                    MessageBox.Show("產品回收再利用次數已達上限");
                    return false;
                }
                //////////////////////////////////////////////////////////////////////////////////////
                /// 站別 : 
                /// A : 不論如何一定過
                /// B : 如果 A 有紀錄, 則 update  XOR 如果 A 沒有紀錄, 則 insert
                /// C - F: update
                var collist = "";
                if (Global.gStation == StationName.gStationA)
                {
                    cmd = "select aresult from " + Global.gTableName + 
                          " where pcbasn = '" + pcbasn + "' " +
                          " and ( " + Global.gBOXTIMEINTERVAL + " || BOXTIME is null) " + 
                          " order by id desc limit 1;";  
                }
                else if (Global.gStation == StationName.gStationB)
                {
                    cmd = "select aresult, bresult from " + Global.gTableName + 
                          " where pcbasn = '" + pcbasn + "' " +
                          " and ( " + Global.gBOXTIMEINTERVAL + " || BOXTIME is null) " +
                          " order by id desc limit 1;";

                }
                else // Station C,F,G
                {
                    collist += colname + DBColPostfix.gColname + ", "
                        + colname + DBColPostfix.gTime + ", "
                        + colname + DBColPostfix.gUser;
                    cmd = "select " + collist + 
                        " from " + Global.gTableName + 
                        " where pcbasn = '" + pcbasn + "'" +
                        " and ( " + Global.gBOXTIMEINTERVAL + " || BOXTIME is null) " + 
                        " order by id desc limit 1;";
                }

                MySQLDB.DBconnect();
                sqlcmd = MySQLDB.command(cmd);
                reader = sqlcmd.ExecuteReader();
                if (Global.gStation == StationName.gStationA)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read()) //一次讀一列 (row)
                        {
                            if (!reader.IsDBNull(0))
                            {
                                Global.gREWORK = true;
                            }

                        }
                    }
                }
                else if (Global.gStation == StationName.gStationB)
                {
                    Global.gREWORK = false;
                    if (reader.HasRows)
                    {
                        while (reader.Read()) //一次讀一列 (row)
                        {
                            if (!reader.IsDBNull(0))
                            {
                                if (reader.GetString(0) == "0")
                                {
                                    result = false;
                                    break;
                                }
                            }
                            if (!reader.IsDBNull(1))
                            {
                                Global.gREWORK = true;
                            }
                        }
                    }
                }
                else
                {
                    if (reader.HasRows)
                    {
                        Global.gREWORK = true;
                        while (reader.Read()) //一次讀一列 (row)
                        {
                            if ((reader.IsDBNull(0)) || (reader.IsDBNull(1)) || (reader.IsDBNull(2)))
                            {
                                result = false;
                                break;
                            }
                            if (reader.GetString(0) != "0")
                            {
                                colvalue.Add(reader.GetString(0)); // DBColPostfix.gColname
                                colvalue.Add(reader.GetString(1)); // DBColPostfix.gTime
                                colvalue.Add(reader.GetString(2)); //DBColPostfix.gUser
                            }
                            else
                            {
                                result = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
                MySQLDB.DBdisconnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show("MySQL 指令: \n" + cmd + "\n  執行過程發生錯誤!");
                MessageBox.Show(ex.ToString());
                MySQLDB.DBdisconnect();
                result = false;
            }
            return result;
        }

        public static void dbchangehistory(string pcbasn, string sigknowsn, string station, string changeto)
        {
            var cmd = "insert into " + Global.gUpdateHistoryTableName + " ( " +
                      "PCBASN, SIGKNOWSN, STATION, CHANGETO, TIME, USERNAME " +
                      ") values ( '" +
                      pcbasn + "' , '" + sigknowsn + "', '" + station + "' , " + changeto + " , '" +
                      Utils.CurrentTime() + "' , '" + Global.gUsername + "' );";

            try
            {
                MySQLDB.DBExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("新增 '" + pcbasn + "' 修正紀錄失敗");
                throw;
            }
        }
        public static void dbinsert(string pcbasn, string station, string changeto)
        {
            var cmd = "insert into " + Global.gTableName + " (" +
                "PCBASN, " +
                station + DBColPostfix.gColname + ", " +
                station + DBColPostfix.gTime + ", " +
                station + DBColPostfix.gUser +
                " ) values ( '" +
                pcbasn + "', " +
                changeto + ", " +
                "'" + Utils.CurrentTime() + "', " +
                "'" + Global.gUsername + "' " +
                ");";
            try
            {
                MySQLDB.DBExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("新增 PCBA '" + pcbasn + "' 資料失敗");
                MessageBox.Show(ex.ToString());
                Utils.ErrorBeep();
            }
        }
        public static void dbupdate(string pcbasn, string station, string changeto)
        {
            var cmd = "update " + Global.gTableName + " set " +
                station + DBColPostfix.gColname + " = " +
                changeto + ", " +
                station + DBColPostfix.gTime + " = " + "'" + Utils.CurrentTime() + "', " +
                station + DBColPostfix.gUser + " = " + "'" + Global.gUsername + "' " +
                " where PCBASN = '" + pcbasn + "' " +
                " and boxsn is NULL order by id desc limit 1; ";
            try
            {
                MySQLDB.DBExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改 PCBA '" + pcbasn + "' 資料失敗");
                MessageBox.Show(ex.ToString());
                Utils.ErrorBeep();
                throw;
            }
        }
        public static void changeTextboxLang2Eng(System.Windows.Controls.TextBox t)
        {
            System.Globalization.CultureInfo LangEng = new System.Globalization.CultureInfo("en-us");
            System.Windows.Input.InputLanguageManager.SetInputLanguage(t, LangEng);
        }

        public static List<string> GetPatchSNbyBoxSN(string boxsn)
        {
            var patchsnlist = new List<string>();
            var cmd = "select sigknowsn from " + Global.gTableName
                      + " where boxsn = '" + boxsn + "' order by sigknowsn asc;";
            MySQLDB.DBconnect();
            MySqlCommand sqlcmd = MySQLDB.command(cmd);
            MySqlDataReader reader = sqlcmd.ExecuteReader();
            if (reader.HasRows)
            {
                Global.gREWORK = true;
                while (reader.Read()) //一次讀一列 (row)
                {
                    if (reader.IsDBNull(0))
                    {
                        MySQLDB.DBdisconnect();
                        throw new InvalidSerialNumberException();
                    }

                    patchsnlist.Add(reader.GetString(0));
                }
            }
            MySQLDB.DBdisconnect();
            return patchsnlist;
        }
    }

    class MySQLDB
    {
        /* TIP: to clean up shopfloor table, do the following:
        SET SQL_SAFE_UPDATES = 0;
        delete from shopfloor;
        commit;
        select * from shopfloor;
        */

        //public static string MySQLserver = "Taiyuan-Lenovo";
        //public static string MySQLserver = "192.168.0.189";
        public static string MySQLserver = SigknowDBServer.gServer;
        public static string MySQLuser = SigknowDBServer.gUser;
        public static string MySQLpassword = SigknowDBServer.gPassword;
        public static string MySQLdatabase = SigknowDBServer.gDatabase;
        static MySqlConnection conn = new MySqlConnection();

        private static void connect()
        {
            string connStr = String.Format("server={0};user={1}; password={2}; database={3}; pooling=false",
            MySQLserver, MySQLuser, MySQLpassword, MySQLdatabase);
            conn.ConnectionString = connStr;

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Utils.ErrorBeep();
                MessageBox.Show("連結到資料庫失敗.");
                MessageBox.Show(ex.ToString());
                Application.Current.Shutdown();
            }
        }
        public static void DBconnect()
        {
            connect();
        }
        private static void disconnect()
        {
            try
            {
                conn.Close();
            }
            catch (Exception ex)
            {
                Utils.ErrorBeep();
                MessageBox.Show("無法中斷與資料庫的連結.");
                MessageBox.Show(ex.ToString());
                Application.Current.Shutdown();
            }
        }
        public static void DBdisconnect()
        {
            disconnect();
        }

        public static MySqlCommand command(string cmd)
        {
            MySqlCommand resultcmd = new MySqlCommand(cmd, conn);
            return resultcmd;
        }
        public static void DBExecuteNonQuery(string cmd)
        {
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = cmd;
            try
            {
                connect();
                command.ExecuteNonQuery();
                disconnect();
                Utils.OKBeep();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show("MySQL 指令 \n" + cmd + "\n  執行過程發生錯誤!");
                try
                {
                    disconnect();
                }
                catch
                {
                    Utils.ErrorBeep();
                }
            }
        }
    }

    class SNAssociate
    {
        public static void ValidateActionCode(string barcode)
        {
            if (!barcode.Equals(Barcode.ASSOCIATE) && !barcode.Equals(Barcode.DISASSOCIATE)
                && barcode.Equals(Barcode.QUERY))
            {
                throw new Exception("無效的條碼");
            }
        }

        public static void ValidatePCBASN(string pcbasn)
        {
            // check SN format  OK
            if (!pcbasn.Substring(0, 1).ToUpper().Contains("B")) throw new InvalidSerialNumberException("PCBA序號規格不符合.");

            // check if station B,C,D has been completed.
            //Utils.ValidateSN(pcbasn, DBColPrefix.gStationB, DBColPrefix.gStationC, DBColPrefix.gStationD);
            Utils.ValidateSN(pcbasn, DBColPrefix.gStationB, DBColPrefix.gStationC);

            // skip SIGKNOWSN NULL check, incase user input the same SIGKNOWSN multiple times and cause false negative.
            var cmd = " select sigknowsn " + " from " + Global.gTableName + " where pcbasn = '" + pcbasn + "'"
                      + " and ( " + Global.gBOXTIMEINTERVAL + " || BOXTIME is null)  " + " order by id desc limit 1;";

            MySQLDB.DBconnect();
            MySqlCommand sqlcmd = MySQLDB.command(cmd);
            MySqlDataReader reader = sqlcmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read()) //一次讀一列 (row)
                {
                    if (!reader.IsDBNull(0))
                    {
                        Global.gREWORK = true;
                    }
                }
            }
            else
            {
                Global.gREWORK = false;
            }
            MySQLDB.DBdisconnect();
        }

        public static void ValidateSIGKNOWSN(string sigknowsn)
        {
            // check format
            if ((!sigknowsn.Substring(0, 1).ToUpper().Contains("C")) && (!sigknowsn.Substring(0, 1).ToUpper().Contains("T")))
                throw new Exception("上蓋序號規格不符合.");
            
        }

        public static void PrecheckAssociation(string pcbasn, string sigknowsn)
        {
            if (!Global.gREWORK) return;

            // check if unboxed (i.e. boxsn is null) PCBA has been associated
            var cmd = " select sigknowsn " +
                      " from " + Global.gTableName +
                      " where pcbasn = '" + pcbasn + "'" +
                      " and ( " + Global.gBOXTIMEINTERVAL + " || BOXTIME is null)  " +
                      " order by id desc limit 1;";

            MySQLDB.DBconnect();
            MySqlCommand sqlcmd = MySQLDB.command(cmd);
            MySqlDataReader reader = sqlcmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read()) //一次讀一列 (row)
                {
                    if (!reader.IsDBNull(0))
                    {
                        // pcbasn has been associated.. need to check again when entering sigknowsn
                        Global.gOLDSIGKNOWSN = reader.GetString(0);
                        if (String.Compare(sigknowsn, Global.gOLDSIGKNOWSN) != 0)
                        {
                            Global.gSKIP = false;
                            MySQLDB.DBdisconnect();
                            MessageBox.Show("衝突!! 上蓋序號 '" + sigknowsn + "'已搭配 PCBA '" + Global.gOLDSIGKNOWSN + "'.");
                            throw new SerialNumberNotMatchedException("上蓋序號'" + sigknowsn + "'已搭配 PCBA '" + Global.gOLDSIGKNOWSN + "'.");
                        }
                        else
                        {
                            MySQLDB.DBdisconnect();
                            throw new ResultUnchangedException();
                        }
                    }
                    else
                    {
                        MySQLDB.DBdisconnect();
                        break;
                    }
                }
            }
            else
            {
                Global.gINITIALRUN = true;
                MySQLDB.DBdisconnect();
                throw new Exception("序號不符生產規定.");
            }
            MySQLDB.DBdisconnect();
        }

        // precheck - before disassociate.
        public static void PrecheckSIGKNOWSN(string sigknowsn)
        {
            // check format
            if ((!sigknowsn.Substring(0, 1).ToUpper().Contains("C")) && (!sigknowsn.Substring(0, 1).ToUpper().Contains("T")))
                throw new Exception("上蓋序號規格不符合.");
        }

        // precheck - before disassociate.
        public static void PrecheckPCBASN(string pcbasn)
        {
            // check SN format
            if (!pcbasn.Substring(0, 1).ToUpper().Contains("B")) throw new Exception("PCBA序號規格不符合");

            // check if station B,C,D has been completed.
            //Utils.ValidateSN(pcbasn, DBColPrefix.gStationB, DBColPrefix.gStationC, DBColPrefix.gStationD);
            Utils.ValidateSN(pcbasn, DBColPrefix.gStationB, DBColPrefix.gStationC);

        }

        public static string GetSigknowSNbyPCBA(string pcbasn)
        {
            var cmd = " select sigknowsn " + " from " + Global.gTableName +
                " where pcbasn = '" + pcbasn + "' " +
                " and ( " + Global.gBOXTIMEINTERVAL + " || BOXTIME is null) " +
                " order by id desc limit 1;";
            var sigknowsn = "";

            MySQLDB.DBconnect();
            MySqlCommand sqlcmd = MySQLDB.command(cmd);
            MySqlDataReader reader = sqlcmd.ExecuteReader();
            if (!reader.HasRows)
            {
                // to prevent false negatives when issuing the input multiple times.
            }
            else
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0))
                    {
                    }
                    else
                    {
                        sigknowsn = reader.GetString(0);
                    }
                }
            }
            MySQLDB.DBdisconnect();
            return sigknowsn;
        }
        public static void PrecheckDisassociation(string pcbasn, string sigknowsn)
        {
            // skip SIGKNOWSN NULL check, incase user input the same SIGKNOWSN multiple times and cause false negative.
            var cmd = " select sigknowsn " + " from " + Global.gTableName + 
                " where pcbasn = '" + pcbasn + "' " + 
                //" and sigknowsn = '" + sigknowsn + "' " +
                " and ( " + Global.gBOXTIMEINTERVAL + " || BOXTIME is null) " + 
                " order by id desc limit 1;";

            MySQLDB.DBconnect();
            MySqlCommand sqlcmd = MySQLDB.command(cmd);
            MySqlDataReader reader = sqlcmd.ExecuteReader();
            if (!reader.HasRows)
            {
                // to prevent false negatives when issuing the input multiple times.
                MySQLDB.DBdisconnect();
                throw new ResultUnchangedException();
            }
            else
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0))
                    {
                        MySQLDB.DBdisconnect();
                        throw new ResultUnchangedException();
                    }
                    var tmpsigknowsn = reader.GetString(0);
                    if (String.Compare((sigknowsn), tmpsigknowsn) != 0)
                    {
                        MySQLDB.DBdisconnect();
                        MessageBox.Show("衝突 !! PCBA'" + pcbasn + "' 與上蓋'" + tmpsigknowsn + "'關聯.");
                        throw new SerialNumberNotMatchedException();
                    }
                }
            }
            MySQLDB.DBdisconnect();
        }

        public static void dbassociate(string pcbasn, string sigknowsn)
        {
            var cmd = "update " + Global.gTableName + " set " +
                " sigknowsn = '" + sigknowsn + "', " +
                " E" + DBColPostfix.gTime + " = " + "'" + Utils.CurrentTime() + "', " +
                " E" + DBColPostfix.gUser + " = " + "'" + Global.gUsername + "' " +
                " where PCBASN = '" + pcbasn + "' " +
                " and boxsn is NULL order by id desc limit 1; ";
            try
            {
                MySQLDB.DBExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("關連 '" + pcbasn + "' 與 上蓋 '" + sigknowsn +"' 失敗");
                MessageBox.Show(ex.ToString());
                Utils.ErrorBeep();
                throw;
            }
        }

        public static void dbdisassociate(string pcbasn, string sigknowsn)
        {
            var cmd = "update " + Global.gTableName + " set " +
                " sigknowsn = null, " +
                " E" + DBColPostfix.gTime + " = " + "'" + Utils.CurrentTime() + "', " +
                " E" + DBColPostfix.gUser + " = " + "'" + Global.gUsername + "' " +
                " where PCBASN = '" + pcbasn + "' " +
                " and SIGKNOWSN = '" + sigknowsn + "' " +
                " and boxsn is NULL order by id desc limit 1; ";
            try
            {
                MySQLDB.DBExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("解除關連 '" + pcbasn + "' 與 上蓋 '" + sigknowsn + "' 失敗");
                MessageBox.Show(ex.ToString());
                Utils.ErrorBeep();
                throw;
            }
        }

        public static void dbchangehistory(string pcbasn, string sigknowsn, string station, string changeto)
        {
            var cmd = "insert into " + Global.gUpdateHistoryTableName + " ( " +
                      "PCBASN, SIGKNOWSN, STATION, CHANGETO, TIME, USERNAME " +
                      ") values ( '" +
                      pcbasn + "' , '" + sigknowsn + "', '" + station + "' , '" + changeto + "' , '" +
                      Utils.CurrentTime() + "' , '" + Global.gUsername + "' );";

            try
            {
                MySQLDB.DBExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("新增 '" + pcbasn + "' 修正紀錄失敗");
                throw;
            }
        }

        public static string GetPCBASN(string sigknowsn)
        {
            var pcbasn = "";
            var cmd = " select pcbasn " + " from " + Global.gTableName + " where sigknowsn = '" + sigknowsn + "'"
                      + " and ( " + Global.gBOXTIMEINTERVAL + " || BOXTIME is null) " + " order by id desc limit 1;";

            MySQLDB.DBconnect();
            MySqlCommand sqlcmd = MySQLDB.command(cmd);
            MySqlDataReader reader = sqlcmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read()) //一次讀一列 (row)
                {
                    if (!reader.IsDBNull(0))
                    {
                        pcbasn = reader.GetString(0);
                    }
                }
            }
            else
            {
                MySQLDB.DBdisconnect();
                throw new SerialNumberNotMatchedException();
            }
            MySQLDB.DBdisconnect();

            return pcbasn;
        }
    }

    class Boxing : Utils
    {
        public static void dbinsert(string pcbasn, string changeto)
        {
            var cmd = "insert into " + Global.gTableName + " (" +
                "PCBASN, BOXSN, BOXTIME, BOXUSERNAME" +
                " ) values ( '" +
                pcbasn + "', " +
                changeto + ", " +
                "'" + Utils.CurrentTime() + "', " +
                "'" + Global.gUsername + "' " +
                ");";
            try
            {
                MySQLDB.DBExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PCBA '" + pcbasn + "' 裝箱資料輸入失敗");
                MessageBox.Show(ex.ToString());
                Utils.ErrorBeep();
            }
        }
        public static void dbupdate(string pcbasn, string changeto)
        {
            var cmd = "update " + Global.gTableName + " set " +
                " BOXSN = '" + changeto + "', " +
                " BOXTIME = " + "'" + Utils.CurrentTime() + "', " +
                " BOXUSERNAME = " + "'" + Global.gUsername + "' " +
                " where PCBASN = '" + pcbasn + "' " +
                " and ( " + Global.gBOXTIMEINTERVAL + " || BOXTIME is null) " +
                " order by id desc limit 1; ";
            try
            {
                MySQLDB.DBExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("裝箱資料修改失敗");
                MessageBox.Show(ex.ToString());
                Utils.ErrorBeep();
                throw;
            }
        }
        public static void dbchangehistory(string pcbasn, string sigknowsn, string station, string changeto)
        {
            var cmd = "insert into " + Global.gUpdateHistoryTableName + " ( " +
                      "PCBASN, SIGKNOWSN, STATION, CHANGETO, TIME, USERNAME " +
                      ") values ( '" +
                      pcbasn + "' , '" + sigknowsn + "', '" + station + "' , '" + changeto + "' , '" +
                      Utils.CurrentTime() + "' , '" + Global.gUsername + "' );";

            try
            {
                MySQLDB.DBExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("新增 '" + pcbasn + "' 修正紀錄失敗");
                throw;
            }
        }

    }

    class DatabaseToFile
    {
        public static void ExportShopFloor(string fn)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(fn);
            // note: Ordering matters.
            var cmd = " select pcbasn,sigknowsn," 
                + "aresult,atime,ausername," 
                + "bresult,btime,busername," 
                + "cresult,ctime,cusername,"
                + "etime,eusername,"
                + "fresult,ftime,fusername,"
                + "boxsn,boxtime,boxusername "
                + " from " + Global.gTableName + " order by id ;";
            var ln = "PCBASN,SIGKNOWSN(E),"
                + StationName.gStationA + "(A),輸入時間(A),操作人員(A),"
                + StationName.gStationB + "(B),輸入時間(B),操作人員(B),"
                + StationName.gStationC + "(C),輸入時間(C),操作人員(C),"
                + "輸入時間(E),操作人員(E),"
                + StationName.gStationF + "(F),輸入時間(F),操作人員(F),"
                + "外箱序號,裝箱時間,裝箱人員";
            file.WriteLine(ln);

            MySQLDB.DBconnect();
            MySqlCommand sqlcmd = MySQLDB.command(cmd);
            MySqlDataReader reader = sqlcmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read()) //一次讀一列 (row)
                {
                    ln = (reader.IsDBNull(0)) ?  "<n/a>,"  : reader.GetString(0) + ",";
                    ln += (reader.IsDBNull(1)) ? "<n/a>,"  : reader.GetString(1) + ",";

                    ln += (reader.IsDBNull(2)) ? "<n/a>,"  : reader.GetBoolean(2) + ",";
                    ln += (reader.IsDBNull(3)) ? "<n/a>,"  : reader.GetString(3) + ",";
                    ln += (reader.IsDBNull(4)) ? "<n/a>,"  : reader.GetString(4) + ",";
                    
                    ln += (reader.IsDBNull(5)) ? "<n/a>,"  : reader.GetBoolean(5) + ",";
                    ln += (reader.IsDBNull(6)) ? "<n/a>,"  : reader.GetString(6) + ",";
                    ln += (reader.IsDBNull(7)) ? "<n/a>,"  : reader.GetString(7) + ",";

                    ln += (reader.IsDBNull(8)) ? "<n/a>,"  : reader.GetBoolean(8) + ",";
                    ln += (reader.IsDBNull(9)) ? "<n/a>,"  : reader.GetString(9) + ",";
                    ln += (reader.IsDBNull(10)) ? "<n/a>," : reader.GetString(10) + ",";

                    ln += (reader.IsDBNull(11)) ? "<n/a>," : reader.GetString(11) + ",";
                    ln += (reader.IsDBNull(12)) ? "<n/a>," : reader.GetString(12) + ",";

                    ln += (reader.IsDBNull(13)) ? "<n/a>," : reader.GetBoolean(13) + ",";
                    ln += (reader.IsDBNull(14)) ? "<n/a>," : reader.GetString(14) + ",";
                    ln += (reader.IsDBNull(15)) ? "<n/a>," : reader.GetString(15) + ",";


                    ln += (reader.IsDBNull(16)) ? "<n/a>," : reader.GetString(16) + ",";
                    ln += (reader.IsDBNull(17)) ? "<n/a>," : reader.GetString(17) + ",";
                    ln += (reader.IsDBNull(18)) ? "<n/a>" : reader.GetString(18);
                    file.WriteLine(ln);
                }
            }
            file.Close();
            MySQLDB.DBdisconnect();
        }

    }
}


public class SerialNumberNotMatchedException: Exception
{
    public SerialNumberNotMatchedException()
    {
    }

    public SerialNumberNotMatchedException(string message)
        : base(message + "")
    {
    }

    public SerialNumberNotMatchedException(string pcbasn, string sigknowsn)
        : base("PCBASN: " + (pcbasn.Length > 0 ? pcbasn : "不存在 , \n") + "SIGKNOWSN: " + (sigknowsn.Length > 0 ? sigknowsn : "不存在 "))
    {
    }

    public SerialNumberNotMatchedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class ResultUnchangedException : Exception
{
    public ResultUnchangedException()
    {
    }

    public ResultUnchangedException(string message)
        : base(message + "")
    {
    }

    public ResultUnchangedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class PreviousErrorException : Exception
{
    public PreviousErrorException()
    {
    }

    public PreviousErrorException(string message)
        : base(message + "")
    {
    }

    public PreviousErrorException(string pcbasn, string sigknowsn)
        : base("PCBASN: " + (pcbasn.Length > 0 ? pcbasn : "NotFound, \n") + "SIGKNOWSN: " + (sigknowsn.Length > 0 ? sigknowsn : "NotFound"))
    {
    }

    public PreviousErrorException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class InvalidSerialNumberException : Exception
{
    public InvalidSerialNumberException()
    {
    }

    public InvalidSerialNumberException(string message)
        : base(message + "")
    {
    }

    public InvalidSerialNumberException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class DatabaseException : Exception
{
    public DatabaseException()
    {
    }

    public DatabaseException(string message)
        : base(message + "")
    {
    }

    public DatabaseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
