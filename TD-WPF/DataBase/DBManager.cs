using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms.VisualStyles;
using TD_WPF.Game.Save;
using System.Web.Script.Serialization;

namespace TD_WPF.DataBase
{
    public static class DbManager
    {
        private const string DbName = "TowerDefense.sqlite";
        private const string DbTableName = "towerdefencemap";
        private const string DbFieldSaveObject = "saveobject";
        private const string DbFieldThumbnail = "thumbnail";
        private const string DbFieldCreated = "created";
        private const string DbFieldModified = "modified";
        private const string DbFiledGuid = "guid";

        public static void CreateDataBase()
        {
            if (!File.Exists(DbName))
            {
                SQLiteConnection.CreateFile(DbName);
            }

            var con = EstablishDbConnection();

            string sql = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{DbTableName}'";
            SQLiteCommand cmd = new SQLiteCommand(sql, con);
            var scalar = cmd.ExecuteScalar();
            if ((long) scalar == 0)
            {
                sql = $"CREATE TABLE {DbTableName}({DbFieldSaveObject} TEXT, {DbFieldThumbnail} BLOB, " +
                      $"{DbFieldCreated} INTEGER, {DbFieldModified} INTEGER, {DbFiledGuid} TEXT PRIMARY KEY NOT NULL)";
                cmd.Reset();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            con.Close();
        }

        public static void SaveMapToDataBase(DbObject dbObject)
        {
            var con = EstablishDbConnection();

            var serializer = new JavaScriptSerializer();
            var saveObjectJson = serializer.Serialize(dbObject.SaveObject);

            var converter = new ImageConverter();
            var thumbnailByteArray = converter.ConvertTo(dbObject.SaveMetaData.Thumbnail, typeof(byte[]));

            var unixTimeCreated =
                Convert.ToInt32(dbObject.SaveMetaData.CreationDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            var unixTimeModified =
                Convert.ToInt32(dbObject.SaveMetaData.ModifiedDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

            string sql = $"INSERT INTO {DbTableName}({DbFieldSaveObject}, {DbFieldThumbnail}, {DbFieldCreated}, " +
                         $"{DbFieldModified}, {DbFiledGuid}) " +
                         $"VALUES(@{DbFieldSaveObject}, @{DbFieldThumbnail}, " +
                         $"@{DbFieldCreated}, @{DbFieldModified}, @{DbFiledGuid})";

            SQLiteCommand cmd = new SQLiteCommand(sql, con);
            cmd.Parameters.Add($"@{DbFieldSaveObject}", System.Data.DbType.String).Value = saveObjectJson;
            cmd.Parameters.Add($"@{DbFieldThumbnail}", System.Data.DbType.Binary).Value = thumbnailByteArray;
            cmd.Parameters.Add($"@{DbFieldCreated}", System.Data.DbType.Int32).Value = unixTimeCreated;
            cmd.Parameters.Add($"@{DbFieldModified}", System.Data.DbType.Int32).Value = unixTimeModified;
            cmd.Parameters.Add($"@{DbFiledGuid}", System.Data.DbType.String).Value =
                dbObject.SaveMetaData.Guid.ToString();

            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void SaveModifiedMapToDataBase(DbObject dbObject)
        {
            var con = EstablishDbConnection();

            var serializer = new JavaScriptSerializer();
            var saveObjectJson = serializer.Serialize(dbObject.SaveObject);

            var converter = new ImageConverter();
            var thumbnailByteArray = converter.ConvertTo(dbObject.SaveMetaData.Thumbnail, typeof(byte[]));

            var unixTimeModified =
                Convert.ToInt32(dbObject.SaveMetaData.ModifiedDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

            string sql = $"UPADTE {DbTableName} SET {DbFieldSaveObject} = @{DbFieldSaveObject}, " +
                         $"{DbFieldThumbnail} = @{DbFieldThumbnail}, " +
                         $"{DbFieldModified} = @{DbFieldModified}, " +
                         $"WHERE {DbFiledGuid} = @{DbFiledGuid}";

            var cmd = new SQLiteCommand(sql, con);
            cmd.Parameters.Add($"@{DbFieldSaveObject}", System.Data.DbType.String).Value = saveObjectJson;
            cmd.Parameters.Add($"@{DbFieldThumbnail}", System.Data.DbType.Binary).Value = thumbnailByteArray;
            cmd.Parameters.Add($"@{DbFieldModified}", System.Data.DbType.String).Value = unixTimeModified;
            cmd.Parameters.Add($"@{DbFiledGuid}", System.Data.DbType.String).Value =
                dbObject.SaveMetaData.Guid.ToString();

            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static List<DbObject> LoadDbObjects()
        {
            var list = new List<DbObject>();
            var javaScriptSerializer = new JavaScriptSerializer();
            var con = EstablishDbConnection();
            var sql = $"SELECT * FROM {DbTableName}";
            var cmd = new SQLiteCommand(sql, con);
            var sqLiteDataReader = cmd.ExecuteReader();

            while (sqLiteDataReader.Read())
            {
                var dbObject = new DbObject();
                var saveMetaData = new SaveMetaData();
                var saveObject =
                    javaScriptSerializer.Deserialize<SaveObject>((string) sqLiteDataReader[DbFieldSaveObject]);
                saveMetaData.CreationDate =
                    new DateTime(1970, 1, 1).AddSeconds(int.Parse(sqLiteDataReader[DbFieldCreated].ToString()));
                saveMetaData.ModifiedDate =
                    new DateTime(1970, 1, 1).AddSeconds(int.Parse(sqLiteDataReader[DbFieldModified].ToString()));
                saveMetaData.Guid = Guid.Parse((string) sqLiteDataReader[DbFiledGuid]);
                var byteArray = (byte[]) sqLiteDataReader[DbFieldThumbnail];
                using (var ms = new MemoryStream(byteArray))
                {
                    saveMetaData.Thumbnail = new Bitmap(ms);
                }

                dbObject.SaveObject = saveObject;
                dbObject.SaveMetaData = saveMetaData;
                list.Add(dbObject);
            }
            
            con.Close();
            
            return list;
        }

        public static void RemoveDbObject(DbObject dbObject)
        {
            var con = EstablishDbConnection();
            var sql = $"DELETE FROM {DbTableName} WHERE {DbFiledGuid} = @{DbFiledGuid}";
            var cmd = new SQLiteCommand(sql, con);
            cmd.Parameters.Add($"{DbFiledGuid}", System.Data.DbType.String).Value =
                dbObject.SaveMetaData.Guid.ToString();

            cmd.ExecuteNonQuery();
            con.Close();
        }
        
        private static SQLiteConnection EstablishDbConnection()
        {
            SQLiteConnection con = new SQLiteConnection($"Data Source = {DbName}; Version = 3;");
            con.Open();
            return con;
        }
    }
}