//using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace StockyConsole
{
    public class DatabaseProcessor
    {
        string connString = "Server=.\\sqlexpress;Database=nsedb;Trusted_Connection=True;";    //mysql: "Server=localhost;Database=nsedb;Uid=root;Pwd=dattagiri;";

        public bool InsertEOD(DateTime DATE, string MKT, string SERIES, string SYMBOL, string SECURITY,
                              double? PREV_CL_PR, double OPEN_PRICE, double HIGH_PRICE, double LOW_PRICE, double CLOSE_PRICE,
                              double? NET_TRDVAL, double NET_TRDQTY, string IND_SEC, string CORP_IND, long? TRADES, double? HI_52_WK, double? LO_52_WK)
        {
            string sqlCommand = "insert into EOD (DATE,MKT,SERIES,SYMBOL,SECURITY,PREV_CL_PR,OPEN_PRICE,HIGH_PRICE,LOW_PRICE,CLOSE_PRICE,NET_TRDVAL,NET_TRDQTY,IND_SEC,CORP_IND,TRADES,HI_52_WK,LO_52_WK)"
                       + " values (@DATE,@MKT,@SERIES,@SYMBOL,@SECURITY,@PREV_CL_PR,@OPEN_PRICE,@HIGH_PRICE,@LOW_PRICE,@CLOSE_PRICE,@NET_TRDVAL,@NET_TRDQTY,@IND_SEC,@CORP_IND,@TRADES,@HI_52_WK,@LO_52_WK)";

            //MySqlConnection conn = new MySqlConnection(connString);
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                conn.Open();
                //MySqlCommand comm = conn.CreateCommand();
                SqlCommand comm = conn.CreateCommand();
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
                if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                    conn.Close();
            }
            return true;
        }
        

        public List<string> GetLatestDates(int numberOfDates)
        {
            //mysql
            //string sqlCommand = "select distinct date from eod order by date desc limit " + numberOfDates.ToString(); // descending
            string sqlCommand = "select distinct top " + numberOfDates.ToString() + " date from eod order by date desc"; // descending

            //MySqlConnection conn = new MySqlConnection(connString);
            //MySqlDataReader reader = null;

            SqlConnection conn = new SqlConnection(connString);
            List<string> result = new List<string>();
            try
            {
                conn.Open();
                //MySqlCommand comm = conn.CreateCommand();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = sqlCommand;

                //using (MySqlDataReader reader = comm.ExecuteReader())
                using (SqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader != null && reader.Read())
                    {
                        result.Add(Convert.ToString(reader["DATE"]));
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                    conn.Close();
            }
            return result;

        }
        
        private List<string> GetResult(string sqlCommand)
        {
            SqlConnection conn = new SqlConnection(connString);

            // MySqlConnection conn = new MySqlConnection(connString);
            //MySqlDataReader reader = null;
            List<string> result = new List<string>();
            try
            {
                conn.Open();
                //MySqlCommand comm = conn.CreateCommand();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = sqlCommand;

                //using (MySqlDataReader reader = comm.ExecuteReader())
                using (SqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader != null && reader.Read())
                    {
                        result.Add(Convert.ToString(reader["SYMBOL"]));
                    }
                }
            }
            catch (Exception ex)
            {
                var error = new List<string>(); 
                error.Add("NO_RECORD_FOUND");
                return error;
            }
            finally
            {
                if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                    conn.Close();
            }

            return result;
        }

        internal List<string> MyChoice2Positive(string date_2, string date_1, string date0, string date1, string date2)
        {
            string sqlCommand = "select distinct eod2.SYMBOL, eod2.CLOSE_PRICE * eod2.NET_TRDQTY from eod eod_2, eod eod_1, eod eod0, eod eod1, eod eod2 "
                                + " where eod_2.SYMBOL = eod_1.SYMBOL and eod_1.SYMBOL = eod0.SYMBOL and eod0.SYMBOL = eod1.SYMBOL and eod1.SYMBOL = eod2.SYMBOL"
                                + " and eod_2.DATE = '" + date_2 + "'"
                                + " and eod_1.DATE = '" + date_1 + "'"
                                + " and eod0.DATE = '" + date0 + "'"
                                + " and eod1.DATE = '" + date1 + "'"
                                + " and eod2.DATE = '" + date2 + "'"
                                + " and eod_2.CLOSE_PRICE < eod_2.OPEN_PRICE"  //negative
                                + " and eod_1.CLOSE_PRICE < eod_1.OPEN_PRICE"  //negative
                                + " and eod0.CLOSE_PRICE < eod0.OPEN_PRICE"  //negative
                                + " and eod1.CLOSE_PRICE > eod1.OPEN_PRICE" // positive
                                + " and eod2.CLOSE_PRICE > eod2.OPEN_PRICE" // positive   
                                + " and (eod0.CLOSE_PRICE > eod1.OPEN_PRICE and eod0.OPEN_PRICE < eod1.CLOSE_PRICE) " // main condition                
                                + " order by (eod2.CLOSE_PRICE * eod2.NET_TRDQTY) desc"; //order by
            return GetResult(sqlCommand);
        }

        internal List<string> InvertedHammer(string date_2, string date_1, string date0, string date1, string date2)
        {
            string sqlCommand = "select distinct eod2.SYMBOL, eod2.CLOSE_PRICE * eod2.NET_TRDQTY from eod eod_2, eod eod_1, eod eod0, eod eod1, eod eod2 "
                                + " where eod_2.SYMBOL = eod_1.SYMBOL and eod_1.SYMBOL = eod0.SYMBOL and eod0.SYMBOL = eod1.SYMBOL and eod1.SYMBOL = eod2.SYMBOL"
                                + " and eod_2.DATE = '"+ date_2 +"'"
                                + " and eod_1.DATE = '" + date_1 + "'"
                                + " and eod0.DATE = '" + date0 + "'"
                                + " and eod1.DATE = '" + date1 + "'"
                                + " and eod2.DATE = '" + date2 + "'"
                                + " and eod_2.CLOSE_PRICE < eod_2.OPEN_PRICE"  //negative
                                + " and eod_1.CLOSE_PRICE < eod_1.OPEN_PRICE"  //negative
                                //+ " and (((eod0.HIGH_PRICE - eod0.LOW_PRICE) > 3 * (eod0.OPEN_PRICE - eod0.CLOSE_PRICE)) AND((eod0.HIGH_PRICE - eod0.CLOSE_PRICE) / (.001 + eod0.HIGH_PRICE - eod0.LOW_PRICE) > 0.6) AND((eod0.HIGH_PRICE - eod0.OPEN_PRICE) / (.001 + eod0.HIGH_PRICE - eod0.LOW_PRICE) > 0.6))"
                                //inverted hammer"
                                + " and eod0.LOW_PRICE = eod0.OPEN_PRICE" // inverted hammer
                                + " and eod1.CLOSE_PRICE > eod1.OPEN_PRICE" // positive
                                + " and eod2.CLOSE_PRICE > eod2.OPEN_PRICE" // positive            
                                + " order by (eod2.CLOSE_PRICE * eod2.NET_TRDQTY) desc"; //order by
            return GetResult(sqlCommand);
        }
        
        internal List<string> Hammer(string date_2, string date_1, string date0, string date1, string date2)
        {
            string sqlCommand = "select distinct eod2.SYMBOL, eod2.CLOSE_PRICE * eod2.NET_TRDQTY from eod eod_2, eod eod_1, eod eod0, eod eod1, eod eod2 "
                                + " where eod_2.SYMBOL = eod_1.SYMBOL and eod_1.SYMBOL = eod0.SYMBOL and eod0.SYMBOL = eod1.SYMBOL and eod1.SYMBOL = eod2.SYMBOL"
                                + " and eod_2.DATE = '" + date_2 + "'"
                                + " and eod_1.DATE = '" + date_1 + "'"
                                + " and eod0.DATE = '" + date0 + "'"
                                + " and eod1.DATE = '" + date1 + "'"
                                + " and eod2.DATE = '" + date2 + "'"
                                + " and eod_2.CLOSE_PRICE < eod_2.OPEN_PRICE"  //negative
                                + " and eod_1.CLOSE_PRICE < eod_1.OPEN_PRICE"  //negative
                                + " and eod0.CLOSE_PRICE = eod0.HIGH_PRICE" // hammer
                                + " and eod1.CLOSE_PRICE > eod1.OPEN_PRICE" // positive
                                + " and eod2.CLOSE_PRICE > eod2.OPEN_PRICE" // positive            
                                + " order by (eod2.CLOSE_PRICE * eod2.NET_TRDQTY) desc"; //order by
            return GetResult(sqlCommand);
        }

        internal List<string> MyChoice3Negative2Postive(string date_2, string date_1, string date0, string date1, string date2)
        {
            string sqlCommand = "select distinct eod2.SYMBOL, eod2.CLOSE_PRICE * eod2.NET_TRDQTY from eod eod_2, eod eod_1, eod eod0, eod eod1, eod eod2 "
                                + " where eod_2.SYMBOL = eod_1.SYMBOL and eod_1.SYMBOL = eod0.SYMBOL and eod0.SYMBOL = eod1.SYMBOL and eod1.SYMBOL = eod2.SYMBOL"
                                + " and eod_2.DATE = '" + date_2 + "'"
                                + " and eod_1.DATE = '" + date_1 + "'"
                                + " and eod0.DATE = '" + date0 + "'"
                                + " and eod1.DATE = '" + date1 + "'"
                                + " and eod2.DATE = '" + date2 + "'"
                                + " and eod_2.CLOSE_PRICE < eod_2.OPEN_PRICE"  //negative
                                + " and eod_1.CLOSE_PRICE < eod_1.OPEN_PRICE"  //negative
                                + " and eod0.CLOSE_PRICE < eod0.OPEN_PRICE"  //negative
                                + " and eod1.CLOSE_PRICE > eod1.OPEN_PRICE" // positive
                                + " and eod2.CLOSE_PRICE > eod2.OPEN_PRICE" // positive   
                                + " and (eod1.OPEN_PRICE = eod1.LOW_PRICE OR eod2.OPEN_PRICE = eod2.LOW_PRICE) " // main condition                
                                + " order by (eod2.CLOSE_PRICE * eod2.NET_TRDQTY) desc"; //order by
            return GetResult(sqlCommand);
        }


        internal List<string> MyChoice2Negative3Postive(string date_2, string date_1, string date0, string date1, string date2)
        {
            string sqlCommand = "select distinct eod2.SYMBOL, eod2.CLOSE_PRICE * eod2.NET_TRDQTY from eod eod_2, eod eod_1, eod eod0, eod eod1, eod eod2 "
                               + " where eod_2.SYMBOL = eod_1.SYMBOL and eod_1.SYMBOL = eod0.SYMBOL and eod0.SYMBOL = eod1.SYMBOL and eod1.SYMBOL = eod2.SYMBOL"
                               + " and eod_2.DATE = '" + date_2 + "'"
                               + " and eod_1.DATE = '" + date_1 + "'"
                               + " and eod0.DATE = '" + date0 + "'"
                               + " and eod1.DATE = '" + date1 + "'"
                               + " and eod2.DATE = '" + date2 + "'"
                               + " and eod_2.CLOSE_PRICE < eod_2.OPEN_PRICE"  //negative
                               + " and eod_1.CLOSE_PRICE < eod_1.OPEN_PRICE"  //negative
                               + " and eod0.CLOSE_PRICE > eod0.OPEN_PRICE"  //positive
                               + " and eod1.CLOSE_PRICE > eod1.OPEN_PRICE" // positive
                               + " and eod2.CLOSE_PRICE > eod2.OPEN_PRICE" // positive   
                               + " and (eod0.OPEN_PRICE = eod0.LOW_PRICE OR eod1.OPEN_PRICE = eod1.LOW_PRICE OR eod2.OPEN_PRICE = eod2.LOW_PRICE) " // main condition                
                               + " order by (eod2.CLOSE_PRICE * eod2.NET_TRDQTY) desc"; //order by
            return GetResult(sqlCommand);
        }


        internal List<string> MyChoice4Negative1Postive(string date_2, string date_1, string date0, string date1, string date2)
        {
            string sqlCommand = "select distinct eod2.SYMBOL, eod2.CLOSE_PRICE * eod2.NET_TRDQTY from eod eod_2, eod eod_1, eod eod0, eod eod1, eod eod2 "
                              + " where eod_2.SYMBOL = eod_1.SYMBOL and eod_1.SYMBOL = eod0.SYMBOL and eod0.SYMBOL = eod1.SYMBOL and eod1.SYMBOL = eod2.SYMBOL"
                              + " and eod_2.DATE = '" + date_2 + "'"
                              + " and eod_1.DATE = '" + date_1 + "'"
                              + " and eod0.DATE = '" + date0 + "'"
                              + " and eod1.DATE = '" + date1 + "'"
                              + " and eod2.DATE = '" + date2 + "'"
                              + " and eod_2.CLOSE_PRICE < eod_2.OPEN_PRICE"  //negative
                              + " and eod_1.CLOSE_PRICE < eod_1.OPEN_PRICE"  //negative
                              + " and eod0.CLOSE_PRICE < eod0.OPEN_PRICE"  //negative
                              + " and eod1.CLOSE_PRICE < eod1.OPEN_PRICE" // negative
                              + " and eod2.CLOSE_PRICE > eod2.OPEN_PRICE" // positive   
                              + " and (eod2.OPEN_PRICE = eod2.LOW_PRICE) " // main condition                
                              + " order by (eod2.CLOSE_PRICE * eod2.NET_TRDQTY) desc"; //order by
            return GetResult(sqlCommand);
        }


        internal List<string> MyChoiceVolumn(string date_2, string date_1, string date0, string date1, string date2)
        {
            string sqlCommand = "select distinct top (20) eod2.SYMBOL, (eod2.CLOSE_PRICE * eod2.NET_TRDQTY) / (eod1.CLOSE_PRICE * eod1.NET_TRDQTY) from eod eod1, eod eod2 "
                              + " where eod1.SYMBOL = eod2.SYMBOL"
                              + " and eod1.DATE = '" + date1 + "'"
                              + " and eod2.DATE = '" + date2 + "'"
                              + " and eod1.CLOSE_PRICE > eod1.OPEN_PRICE" // positive
                              + " and eod2.CLOSE_PRICE > eod2.OPEN_PRICE" // positive  
                              + " and (eod1.CLOSE_PRICE * eod1.NET_TRDQTY) > 0"
                              + " order by (eod2.CLOSE_PRICE * eod2.NET_TRDQTY) / (eod1.CLOSE_PRICE * eod1.NET_TRDQTY) desc"; //order by
            return GetResult(sqlCommand);
        }


        internal List<string> MyChoicePriceVolumn(string date_2, string date_1, string date0, string date1, string date2)
        {
            string sqlCommand = "select distinct top (20) eod2.SYMBOL, (eod2.CLOSE_PRICE * eod2.NET_TRDQTY) from eod eod2 "
                              + " where "
                              + " eod2.DATE = '" + date2 + "'"
                              + " and eod2.CLOSE_PRICE > eod2.OPEN_PRICE" // positive               
                              + " order by (eod2.CLOSE_PRICE * eod2.NET_TRDQTY) desc"; //order by
            return GetResult(sqlCommand);
        }
    }
}
