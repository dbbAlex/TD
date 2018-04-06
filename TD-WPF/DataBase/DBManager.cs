using System;
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

        public static void SaveMapToDataBase(SaveObject saveObject, SaveMetaData saveMetaData)
        {
            var con = EstablishDbConnection();

            var serializer = new JavaScriptSerializer();
            var saveObjectJson = serializer.Serialize(saveObject);
            var converter = new ImageConverter();
            var thumbnailByteArray = converter.ConvertTo(saveMetaData.Thumbnail, typeof(byte[]));
            var unixTimeCreated = saveMetaData.CreationDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var unixTimeModified = saveMetaData.ModifiedDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            string sql = $"INSERT INTO {DbTableName}({DbFieldSaveObject}, {DbFieldThumbnail}, {DbFieldCreated}, " +
                         $"{DbFieldModified}, {DbFiledGuid}) " +
                         $"VALUES(@{DbFieldSaveObject}, @{DbFieldThumbnail}, " +
                         $"@{DbFieldCreated}, @{DbFieldModified}, @{DbFiledGuid})";
            
            SQLiteCommand cmd = new SQLiteCommand(sql, con);
            cmd.Parameters.Add($"@{DbFieldSaveObject}", System.Data.DbType.String).Value = saveObjectJson; 
            cmd.Parameters.Add($"@{DbFieldThumbnail}", System.Data.DbType.Binary).Value = thumbnailByteArray; 
            cmd.Parameters.Add($"@{DbFieldCreated}", System.Data.DbType.Int32).Value = unixTimeCreated;
            cmd.Parameters.Add($"@{DbFieldModified}", System.Data.DbType.String).Value = unixTimeModified;
            cmd.Parameters.Add($"@{DbFiledGuid}", System.Data.DbType.String).Value = saveMetaData.Guid.ToString();

            cmd.ExecuteNonQuery();
        }

        public static void SaveModifiedMapToDataBase(SaveObject saveObject, SaveMetaData saveMetaData)
        {
            var con = EstablishDbConnection();
            
            var serializer = new JavaScriptSerializer();
            var saveObjectJson = serializer.Serialize(saveObject);
            var converter = new ImageConverter();
            var thumbnailByteArray = converter.ConvertTo(saveMetaData.Thumbnail, typeof(byte[]));
            var unixTimeModified = saveMetaData.ModifiedDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            
            string sql = $"UPADTE {DbTableName} SET {DbFieldSaveObject} = @{DbFieldSaveObject}, " +
                         $"{DbFieldThumbnail} = @{DbFieldThumbnail}, " +
                         $"{DbFieldModified} = @{DbFieldModified}, " +
                         $"WHERE {DbFiledGuid} = @{DbFiledGuid}";
            
            var cmd = new SQLiteCommand(sql, con);
            cmd.Parameters.Add($"@{DbFieldSaveObject}", System.Data.DbType.String).Value = saveObjectJson; 
            cmd.Parameters.Add($"@{DbFieldThumbnail}", System.Data.DbType.Binary).Value = thumbnailByteArray; 
            cmd.Parameters.Add($"@{DbFieldModified}", System.Data.DbType.String).Value = unixTimeModified;
            cmd.Parameters.Add($"@{DbFiledGuid}", System.Data.DbType.String).Value = saveMetaData.Guid.ToString();

            cmd.ExecuteNonQuery();
        }

        private static SQLiteConnection EstablishDbConnection()
        {
            SQLiteConnection con = new SQLiteConnection($"Data Source = {DbName}; Version = 3;");
            con.Open();
            return con;
        }
    }
}