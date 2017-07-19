using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockyConsole
{
    public class DatabaseProcessor
    {
        string connString = "Server=localhost;Database=nsedb;Uid=root;Pwd=dattagiri;";

        public bool InsertEOD(DateTime DATE, string MKT,string SERIES, string SYMBOL,string SECURITY,
                              double PREV_CL_PR,double OPEN_PRICE,double HIGH_PRICE,double LOW_PRICE,double CLOSE_PRICE,
                              double NET_TRDVAL,double NET_TRDQTY,string IND_SEC,string CORP_IND,long TRADES,double HI_52_WK,double LO_52_WK)
        {
            string sqlCommand = "insert into EOD (DATE,MKT,SERIES,SYMBOL,SECURITY,PREV_CL_PR,OPEN_PRICE,HIGH_PRICE,LOW_PRICE,CLOSE_PRICE,NET_TRDVAL,NET_TRDQTY,IND_SEC,CORP_IND,TRADES,HI_52_WK,LO_52_WK)"
                       + " values (@DATE,@MKT,@SERIES,@SYMBOL,@SECURITY,@PREV_CL_PR,@OPEN_PRICE,@HIGH_PRICE,@LOW_PRICE,@CLOSE_PRICE,@NET_TRDVAL,@NET_TRDQTY,@IND_SEC,@CORP_IND,@TRADES,@HI_52_WK,@LO_52_WK)";

            MySqlConnection conn = new MySqlConnection(connString);
            try
            {                
                conn.Open();
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = sqlCommand;
                //"INSERT INTO room(person,address) VALUES(@person, @address)";
                comm.Parameters.AddWithValue("@DATE", DATE);
                comm.Parameters.AddWithValue("@MKT", MKT);
                comm.Parameters.AddWithValue("@SERIES", SERIES);
                comm.Parameters.AddWithValue("@SYMBOL", SYMBOL);
                comm.Parameters.AddWithValue("@SECURITY", SECURITY);
                comm.Parameters.AddWithValue("@PREV_CL_PR", PREV_CL_PR);
                comm.Parameters.AddWithValue("@OPEN_PRICE", OPEN_PRICE);
                comm.Parameters.AddWithValue("@HIGH_PRICE", HIGH_PRICE);
                comm.Parameters.AddWithValue("@LOW_PRICE", LOW_PRICE);
                comm.Parameters.AddWithValue("@CLOSE_PRICE", CLOSE_PRICE);
                comm.Parameters.AddWithValue("@NET_TRDVAL", NET_TRDVAL);
                comm.Parameters.AddWithValue("@NET_TRDQTY", NET_TRDQTY);
                comm.Parameters.AddWithValue("@IND_SEC", IND_SEC);
                comm.Parameters.AddWithValue("@CORP_IND", CORP_IND);
                comm.Parameters.AddWithValue("@TRADES", TRADES);
                comm.Parameters.AddWithValue("@HI_52_WK", HI_52_WK);
                comm.Parameters.AddWithValue("@LO_52_WK", LO_52_WK);
                
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if(conn != null && conn.State != System.Data.ConnectionState.Closed)
                    conn.Close();
            }
            return true;
        }

    }
}
