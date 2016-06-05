using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;

namespace ArduinoPhone
{
    class SmsDb
    {
        private SQLiteConnection dbConn;
        private string myNum;

        public SmsDb(string ownNumber)
        {
            myNum = ownNumber;
            SQLiteCommand command = null;
            if (!File.Exists("sms.db"))
            {
                SQLiteConnection.CreateFile("sms.db");
                string sql = "create table messages(sender varchar(50), receiver varchar(50), message varchar(300), timestamp varchar(50))";
                command = new SQLiteCommand(sql);
            }
            dbConn = new SQLiteConnection("Data Source=sms.db;Version=3;");
            dbConn.Open();
            if (command != null && dbConn.State == System.Data.ConnectionState.Open)
            {
                command.Connection = dbConn;
                command.ExecuteNonQuery();
            }
        }

        public void StoreOutgoingMessage(string dest, string message, string time)
        {
            SQLiteCommand command = new SQLiteCommand("insert into messages (sender,receiver,message,timestamp) "
                + "values ($myNum, $num, $message, $time)", dbConn);
            command.Parameters.AddWithValue("$myNum", myNum);
            command.Parameters.AddWithValue("$num", dest);
            command.Parameters.AddWithValue("$message", message);
            command.Parameters.AddWithValue("$time", time);
            command.ExecuteNonQuery();
        }

        public void StoreIncomingMessage(string src, string message, string time)
        {
            string newTime = "";
            try
            {
                DateTime dt = DateTime.ParseExact(time, "yy/MM/dd,HH:mm:sszz", CultureInfo.InvariantCulture);
                newTime = dt.ToString("dd/MM/yyyy HH:mm:ss");
            }
            catch (FormatException)
            {
                newTime = time;
            }
            SQLiteCommand command = new SQLiteCommand(
                    "insert into messages "
                   + "(sender,receiver,message,timestamp) "
                   + "values ($srcNum, $myNum, $message, $time)", dbConn);
            command.Parameters.AddWithValue("$srcNum", src);
            command.Parameters.AddWithValue("$myNum", myNum);
            command.Parameters.AddWithValue("$message", message);
            command.Parameters.AddWithValue("$time", newTime);
            command.ExecuteNonQuery();
        }

        public Dictionary<string, string> GetMessagesForNumber(MessageControl m, string number)
        {
            SQLiteCommand command = new SQLiteCommand(
                    "select * from messages "
                   + "where sender=$num or receiver=$num", dbConn);
            command.Parameters.AddWithValue("$num", number);
            command.ExecuteNonQuery();
            SQLiteDataReader read = command.ExecuteReader();
            Dictionary<string, string> messages = new Dictionary<string, string>();
            if (read.HasRows)
            {
                while (read.Read())
                {
                    string sender = read.GetValue(0).ToString();
                    string message = read.GetValue(2).ToString();
                    messages[sender] = message;
                }
            }
            return messages;
        }

        public Dictionary<string, Dictionary<string, string>> GetAllDbMessages()
        {
            Dictionary<string, Dictionary<string, string>> all = new Dictionary<string, Dictionary<string, string>>();
            SQLiteCommand command = new SQLiteCommand(
                "select * from messages", dbConn);
            command.ExecuteNonQuery();
            SQLiteDataReader read = command.ExecuteReader();
            if (read.HasRows)
            {
                while (read.Read())
                {
                    string sender = read.GetString(0);
                    string receiver = read.GetString(1);
                    string message = read.GetString(2);
                    string timestamp = read.GetString(3);
                    Dictionary<string, string> keyDict = new Dictionary<string, string>();
                    keyDict[message] = timestamp;
                    if (sender == myNum)
                        all[receiver] = keyDict;
                    else
                        all[sender] = keyDict;
                }
            }
            return all;
        }

        public bool NumberInPreviousRecipients(string number)
        {
            SQLiteCommand command = new SQLiteCommand(
                   "select * from messages "
                  + "where sender=$num or receiver=$num group by sender", dbConn);
            command.Parameters.AddWithValue("$num", number);
            command.ExecuteNonQuery();
            SQLiteDataReader read = command.ExecuteReader();
            if (read.HasRows)
            {
                return true;
            }
            return false;
        }

        public void DeleteConversation(string number)
        {
            SQLiteCommand command = new SQLiteCommand("delete from messages where sender=$num or receiver=$num", dbConn);
            command.Parameters.AddWithValue("$num", number);
            command.ExecuteNonQuery();
        }

        public void CloseConnection()
        {
            dbConn.Close();
        }
    }
}
