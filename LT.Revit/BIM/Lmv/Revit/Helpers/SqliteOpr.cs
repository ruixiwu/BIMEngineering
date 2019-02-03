using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace BIM.Lmv.Revit.Helpers
{
    internal class SqliteOpr
    {
        private DbTransaction _trans;
        private SQLiteConnection m_dbConnection;

        public void commitData()
        {
            _trans.Commit();
            m_dbConnection.Close();
            m_dbConnection.Dispose();
        }

        public void connectToDatabase(string strDir)
        {
            var connectionString = "Data Source = " + strDir + "; Version = 3;";
            m_dbConnection = new SQLiteConnection(connectionString);
            m_dbConnection.Open();
            _trans = m_dbConnection.BeginTransaction();
        }

        public void createNewDatabase(string connString)
        {
            SQLiteConnection.CreateFile(connString);
        }

        public void createTable(string sql)
        {
            new SQLiteCommand(sql, m_dbConnection).ExecuteNonQuery();
        }

        public string fillTable(string sTable, string sColume, string sValue, string sID = "")
        {
            if (string.IsNullOrEmpty(sID))
            {
                sID = (getTableMaxId(sTable) + 1).ToString();
            }
            new SQLiteCommand(
                "insert into " + sTable + "(OBJECTID," + sColume + ") values (" + sID + "," + sValue + ")",
                m_dbConnection).ExecuteNonQuery();
            return sID;
        }

        public void fillTableblob(string sTable, string sObjectId, string sColume, byte[] bArray)
        {
            var str = "update " + sTable + " set " + sColume + "=(:k) where OBJECTID = " + sObjectId;
            var command = m_dbConnection.CreateCommand();
            command.CommandText = str;
            command.Parameters.Add("k", DbType.Binary).Value = bArray;
            command.ExecuteNonQuery();
        }

        public string getTableField(string sTable, string sField, string sfilter)
        {
            var str = "";
            var reader =
                new SQLiteCommand("select " + sField + " from " + sTable + " where " + sfilter, m_dbConnection)
                    .ExecuteReader();
            if (reader.Read() && !reader.IsDBNull(0))
            {
                str = reader.GetValue(0).ToString();
            }
            return str;
        }

        public int getTableMaxId(string sTable)
        {
            var num = 0;
            var reader =
                new SQLiteCommand("select max(OBJECTID) as MAXID from " + sTable, m_dbConnection).ExecuteReader();
            if (reader.Read() && !reader.IsDBNull(0))
            {
                num = reader.GetInt32(0);
            }
            reader.Close();
            return num;
        }

        public void printHighscores()
        {
            var commandText = "select * from highscores order by score desc";
            var reader = new SQLiteCommand(commandText, m_dbConnection).ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(string.Concat("Name: ", reader["name"], "\tScore: ", reader["score"]));
            }
            Console.ReadLine();
        }
    }
}