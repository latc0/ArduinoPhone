using System.Data.SQLite;
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
                string sql = "create table messages(number varchar(50), type varchar(8), message varchar(300), timestamp varchar(50))";
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
            SQLiteCommand command = new SQLiteCommand("insert into messages (number,type,message,timestamp) "
                + "values ($num, $type, $message, $time)", dbConn);
            command.Parameters.AddWithValue("$num", dest);
            command.Parameters.AddWithValue("$type", "sent");
            command.Parameters.AddWithValue("$message", message);
            command.Parameters.AddWithValue("$time", time);
            command.ExecuteNonQuery();
        }

        public void StoreIncomingMessage(string src, string message, string time)
        {
            SQLiteCommand command = new SQLiteCommand(
                    "insert into messages "
                   + "(number,type,message,timestamp) "
                   + "values ($num, $type, $message, $time)", dbConn);
            command.Parameters.AddWithValue("$num", src);
            command.Parameters.AddWithValue("$type", "recv");
            command.Parameters.AddWithValue("$message", message);
            command.Parameters.AddWithValue("$time", time);
            command.ExecuteNonQuery();
        }

        public void ShowMessagesForNumber(MessageControl m, string number)
        {
            SQLiteCommand command = new SQLiteCommand(
                    "select * from messages "
                   + "where number=$num", dbConn);
            command.Parameters.AddWithValue("$num", number);
            command.ExecuteNonQuery();
            SQLiteDataReader read = command.ExecuteReader();
            if (read.HasRows)
            {
                while (read.Read())
                {
                    string mType = read.GetValue(1).ToString();
                    string message = read.GetValue(2).ToString();
                    if (mType == "sent")
                        m.AddSent(message);
                    else
                        m.AddReceived(message);
                }
            }
        }

        public void GetAllDbMessages(Conversation c)
        {
            SQLiteCommand command = new SQLiteCommand(
                "select number,message,timestamp from messages group by number", dbConn);
            command.ExecuteNonQuery();
            SQLiteDataReader read = command.ExecuteReader();
            if (read.HasRows)
            {
                while (read.Read())
                {
                    string number = read.GetString(0);
                    string message = read.GetString(1);
                    string timestamp = read.GetString(2);
                    c.Add(number, message, timestamp);
                }
            }
        }

        public bool NumberInPreviousRecipients(string number)
        {
            SQLiteCommand command = new SQLiteCommand(
                   "select * from messages "
                  + "where number=$num group by number", dbConn);
            command.Parameters.AddWithValue("$num", number);
            command.ExecuteNonQuery();
            SQLiteDataReader read = command.ExecuteReader();
            return read.HasRows;
        }

        public void DeleteConversation(string number)
        {
            SQLiteCommand command = new SQLiteCommand("delete from messages where number=$num", dbConn);
            command.Parameters.AddWithValue("$num", number);
            command.ExecuteNonQuery();
        }

        public void CloseConnection()
        {
            dbConn.Close();
        }
    }
}
