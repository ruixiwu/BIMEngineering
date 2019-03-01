namespace BIM.Lmv.Revit.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.IO;
    using System.Runtime.InteropServices;

    internal class SqliteOpr
    {
        private DbTransaction _trans;
        private SQLiteConnection m_dbConnection;

        public void commitData()
        {
            this._trans.Commit();
            this.m_dbConnection.Close();
            this.m_dbConnection.Dispose();
        }

        public void connectToDatabase(string strDir)
        {
            string connectionString = "Data Source = " + strDir + "; Version = 3;";
            this.m_dbConnection = new SQLiteConnection(connectionString);
            this.m_dbConnection.Open();
            this._trans = this.m_dbConnection.BeginTransaction();
        }

        public void createNewDatabase(string strDir)
        {
            SQLiteConnection.CreateFile("Data Source = " + strDir);
        }

        public void createTable(string sPreFix)
        {
            string commandText = "CREATE TABLE COORDINATE_SYSTEM (OBJECTID int(11) NOT NULL,NAME varchar(128) DEFAULT NULL,SEMIMAJORAXIS real DEFAULT NULL,INVERSEFLATTENING real DEFAULT NULL,SCALEFACTOR real DEFAULT NULL,FALSE_EASTING real DEFAULT NULL,FALSE_NORTHING real DEFAULT NULL,CENTRALMERIDIAN real DEFAULT NULL,PRIMARY KEY (OBJECTID))";
            new SQLiteCommand(commandText, this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE AFFIXTABLE" + sPreFix + " (OBJECTID int(11) NOT NULL,NAME varchar(128) DEFAULT NULL,TYPE varchar(128) DEFAULT NULL,AFFIXFILEID varchar(128) DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE AFFIXFILE" + sPreFix + " (OBJECTID int(11) NOT NULL,OBJGUID varchar(128) DEFAULT NULL,CONTENT blob DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
            commandText = "CREATE TABLE PROJECT (OBJECTID int(11) NOT NULL,NAME varchar(128) DEFAULT NULL,PREFIXION integer DEFAULT NULL,SOURCEID int(11) DEFAULT -1,ZIPKEY varchar(128) DEFAULT NULL,WORKSPACEID int DEFAULT NULL,FOREIGNID varchar(128) DEFAULT NULL,DESCRIBE varchar(128) DEFAULT NULL,PRIMARY KEY (OBJECTID))";
            new SQLiteCommand(commandText, this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE TEXTURES" + sPreFix + " (OBJECTID int(11) NOT NULL,NAME varchar(128) DEFAULT NULL,FORMAT integer DEFAULT NULL,MAGFILTER integer DEFAULT NULL,MINFILTER integer DEFAULT NULL,WRAPS integer DEFAULT NULL,WRAPT integer DEFAULT NULL,IMAGENAME varchar(128) DEFAULT NULL,TEXIMGID varchar(128) DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE TEXIMG" + sPreFix + " (OBJECTID int(11) NOT NULL,OBJGUID varchar(128) DEFAULT NULL,CONTENT blob DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE TILESETTABLE" + sPreFix + " (OBJECTID int(11) NOT NULL,PARENT int(11) DEFAULT NULL,MINX real DEFAULT NULL,MINY real DEFAULT NULL,MINZ real DEFAULT NULL,MAXX real DEFAULT NULL,MAXY real DEFAULT NULL,MAXZ real DEFAULT NULL,CENTER varchar(128) DEFAULT NULL,LASTUPDATETIME varchar(128) DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE GEOTABLE" + sPreFix + " (OBJECTID int(11) NOT NULL,NAME varchar(128) DEFAULT NULL,TILESETID int(11) DEFAULT -1,MATRIX varchar(512) DEFAULT NULL,MESHIDS text DEFAULT NULL,BLOCKID int(11) DEFAULT NULL,BOX varchar(256) DEFAULT NULL,FAMILYID int(11) DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE GEOBLOCK" + sPreFix + " (OBJECTID int(11) NOT NULL,NAME varchar(128) DEFAULT NULL,DESCRIBE varchar(128) DEFAULT NULL,MESHIDS text DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE FAMILYTABLE" + sPreFix + " (OBJECTID int(11) NOT NULL,NAME varchar(128) DEFAULT NULL,PARENTID int(11) NOT NULL,DESCRIBE varchar(128) DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE FMYITEMTABLE" + sPreFix + " (OBJECTID int(11) NOT NULL,SECTION varchar(128) DEFAULT NULL,NAME varchar(128) DEFAULT NULL,VALUE varchar(256) DEFAULT NULL,UNIT varchar(128) DEFAULT NULL,TYPE int(11) DEFAULT NULL,FAMILYID int(11) DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE PROTABLE" + sPreFix + " (OBJECTID int(11) NOT NULL,SECTION varchar(128) DEFAULT NULL,NAME varchar(128) DEFAULT NULL,VALUE varchar(256) DEFAULT NULL,UNIT varchar(128) DEFAULT NULL,TYPE int(11) DEFAULT NULL,GEOID int(11) DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE MESHTABLE" + sPreFix + " (OBJECTID int(11) NOT NULL,NAME varchar(128) DEFAULT NULL,MATERIALID int(11) DEFAULT NULL,ACCESSOR_POSTION int(11) DEFAULT NULL,ACCESSOR_INDEX int(11) DEFAULT NULL,ACCESSOR_NORMAL int(11) DEFAULT NULL,ACCESSOR_TEXCOORD_0 int(11) DEFAULT NULL,ACCESSOR_TEXCOORD_1 int(11) DEFAULT NULL,ACCESSOR_TEXCOORD_2 int(11) DEFAULT NULL,ACCESSOR_TEXCOORD_3 int(11) DEFAULT NULL,ACCESSOR_TEXCOORD_4 int(11) DEFAULT NULL,ACCESSOR_TEXCOORD_5 int(11) DEFAULT NULL,ACCESSOR_TEXCOORD_6 int(11) DEFAULT NULL,ACCESSOR_TEXCOORD_7 int(11) DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE ACCESSORTABLE" + sPreFix + " (OBJECTID int(11) NOT NULL,TYPE varchar(128) DEFAULT NULL,BYTEOFFSET integer DEFAULT NULL,BYTESTRIDE integer DEFAULT NULL,COMPONENTTYPE integer DEFAULT NULL,NCOUNT integer DEFAULT NULL,BFILEID varchar(128) DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE BFILETABLE" + sPreFix + " (OBJECTID int(11) NOT NULL,OBJGUID varchar(128) DEFAULT NULL,CONTENT blob DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
            new SQLiteCommand("CREATE TABLE MATERIALTABLE" + sPreFix + " (OBJECTID int(11) NOT NULL,NAME varchar(128) DEFAULT NULL,TECHDES varchar(128) DEFAULT NULL,AMBIENT varchar(128) DEFAULT NULL,EMISSION varchar(128) DEFAULT NULL,SHININESS varchar(128) DEFAULT NULL,SPECULAR varchar(128) DEFAULT NULL,DIFFUSE varchar(128) DEFAULT NULL,TEXTURE_1 int(11) DEFAULT NULL,TEXTURE_2 int(11) DEFAULT NULL,TEXTURE_3 int(11) DEFAULT NULL,TEXTURE_4 int(11) DEFAULT NULL,PRIMARY KEY (OBJECTID))", this.m_dbConnection).ExecuteNonQuery();
        }

        public string fillTable(string sTable, string sColume, string sValue, string sID = "")
        {//sID即是自增值ObjectID
            try
            {
                if (string.IsNullOrEmpty(sID))
                {
                    sID = (this.getTableMaxId(sTable) + 1).ToString();
                }
                new SQLiteCommand("insert into " + sTable + "(OBJECTID," + sColume + ") values (" + sID + "," + sValue + ")", this.m_dbConnection).ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                exception.ToString();
            }
            return sID;
        }

        public void fillTableblob(string sTable, string sObjectId, string sColume, byte[] bArray)
        {
            string str = "update " + sTable + " set " + sColume + "=(:k) where OBJECTID = " + sObjectId;
            SQLiteCommand command = this.m_dbConnection.CreateCommand();
            command.CommandText = str;
            command.Parameters.Add("k", DbType.Binary).Value = bArray;
            command.ExecuteNonQuery();
        }

        public bool GetTableblob(string sTable, string sfilter, string sColume, ref byte[] bArray)
        {
            bool flag = false;
            SQLiteDataReader reader = new SQLiteCommand("select " + sColume + " from " + sTable + " where " + sfilter, this.m_dbConnection).ExecuteReader();
            if (reader.Read() && !reader.IsDBNull(0))
            {
                bArray = (byte[]) reader[sColume];
                flag = true;
            }
            reader.Close();
            return flag;
        }

        public string getTableField(string sTable, string sField, string sfilter)
        {
            string str = "";
            SQLiteDataReader reader = new SQLiteCommand("select " + sField + " from " + sTable + " where " + sfilter, this.m_dbConnection).ExecuteReader();
            if (reader.Read() && !reader.IsDBNull(0))
            {
                str = reader.GetValue(0).ToString();
            }
            reader.Close();
            return str;
        }

        public int getTableMaxId(string sTable)
        {
            int num = 0;
            SQLiteDataReader reader = new SQLiteCommand("select max(OBJECTID) as MAXID from " + sTable, this.m_dbConnection).ExecuteReader();
            if (reader.Read() && !reader.IsDBNull(0))
            {
                num = reader.GetInt32(0);
            }
            reader.Close();
            return num;
        }

        public bool getTableRecord(string sTable, string sfilter, ref Dictionary<string, string> retDic)
        {
            string commandText = "select * from " + sTable + " where " + sfilter;
            bool flag = false;
            SQLiteDataReader reader = new SQLiteCommand(commandText, this.m_dbConnection).ExecuteReader();
            if (reader.Read())
            {
                int fieldCount = reader.FieldCount;
                for (int i = 0; i < fieldCount; i++)
                {
                    string str2 = "";
                    string name = reader.GetName(i);
                    if (!reader.IsDBNull(i))
                    {
                        str2 = reader.GetValue(i).ToString();
                        flag = true;
                    }
                    retDic.Add(name, str2);
                }
            }
            reader.Close();
            return flag;
        }

        public bool getTableRecords(string sTable, string sfilter, ref List<Dictionary<string, string>> retListDic)
        {
            string commandText = "select * from " + sTable + " where " + sfilter;
            bool flag = false;
            SQLiteDataReader reader = new SQLiteCommand(commandText, this.m_dbConnection).ExecuteReader();
            int fieldCount = reader.FieldCount;
            while (reader.Read())
            {
                Dictionary<string, string> item = new Dictionary<string, string>();
                for (int i = 0; i < fieldCount; i++)
                {
                    string str2 = "";
                    string name = reader.GetName(i);
                    if (!reader.IsDBNull(i))
                    {
                        str2 = reader.GetValue(i).ToString();
                        flag = true;
                    }
                    item.Add(name, str2);
                }
                retListDic.Add(item);
            }
            reader.Close();
            return flag;
        }

        public void printHighscores()
        {
            string commandText = "select * from highscores order by score desc";
            SQLiteDataReader reader = new SQLiteCommand(commandText, this.m_dbConnection).ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(string.Concat(new object[] { "Name: ", reader["name"], "\tScore: ", reader["score"] }));
            }
            Console.ReadLine();
        }

        public bool TakeBlobToFile(string sTable, string sfilter, string sColume, string strPathName)
        {
            bool flag = false;
            SQLiteDataReader reader = new SQLiteCommand("select " + sColume + " from " + sTable + " where " + sfilter, this.m_dbConnection).ExecuteReader();
            if (reader.Read() && !reader.IsDBNull(0))
            {
                byte[] buffer = (byte[]) reader[sColume];
                BinaryWriter writer = new BinaryWriter(File.Open(strPathName, FileMode.OpenOrCreate));
                writer.Write(buffer);
                writer.Close();
                flag = true;
            }
            reader.Close();
            return flag;
        }
    }
}

