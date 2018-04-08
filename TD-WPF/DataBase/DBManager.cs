using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Web.Script.Serialization;
using TD_WPF.Game.Save;

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
            if (!File.Exists(DbName)) SQLiteConnection.CreateFile(DbName);

            var con = EstablishDbConnection();

            var sql = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{DbTableName}'";
            var cmd = new SQLiteCommand(sql, con);
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
            var saveObjectJson = serializer.Serialize(dbObject.GameData);

            var converter = new ImageConverter();
            var thumbnailByteArray = converter.ConvertTo(dbObject.MetaData.Thumbnail, typeof(byte[]));

            var unixTimeCreated =
                Convert.ToInt32(dbObject.MetaData.CreationDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            var unixTimeModified =
                Convert.ToInt32(dbObject.MetaData.ModifiedDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

            var sql = $"INSERT INTO {DbTableName}({DbFieldSaveObject}, {DbFieldThumbnail}, {DbFieldCreated}, " +
                      $"{DbFieldModified}, {DbFiledGuid}) " +
                      $"VALUES(@{DbFieldSaveObject}, @{DbFieldThumbnail}, " +
                      $"@{DbFieldCreated}, @{DbFieldModified}, @{DbFiledGuid})";

            var cmd = new SQLiteCommand(sql, con);
            cmd.Parameters.Add($"@{DbFieldSaveObject}", DbType.String).Value = saveObjectJson;
            cmd.Parameters.Add($"@{DbFieldThumbnail}", DbType.Binary).Value = thumbnailByteArray;
            cmd.Parameters.Add($"@{DbFieldCreated}", DbType.Int32).Value = unixTimeCreated;
            cmd.Parameters.Add($"@{DbFieldModified}", DbType.Int32).Value = unixTimeModified;
            cmd.Parameters.Add($"@{DbFiledGuid}", DbType.String).Value =
                dbObject.MetaData.Guid.ToString();

            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void SaveModifiedMapToDataBase(DbObject dbObject)
        {
            var con = EstablishDbConnection();

            var serializer = new JavaScriptSerializer();
            var saveObjectJson = serializer.Serialize(dbObject.GameData);

            var converter = new ImageConverter();
            var thumbnailByteArray = converter.ConvertTo(dbObject.MetaData.Thumbnail, typeof(byte[]));

            var unixTimeModified =
                Convert.ToInt32(dbObject.MetaData.ModifiedDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

            var sql = $"UPDATE {DbTableName} SET {DbFieldSaveObject} = @{DbFieldSaveObject}, " +
                      $"{DbFieldThumbnail} = @{DbFieldThumbnail}, " +
                      $"{DbFieldModified} = @{DbFieldModified} " +
                      $"WHERE {DbFiledGuid} = @{DbFiledGuid}";

            var cmd = new SQLiteCommand(sql, con);
            cmd.Parameters.Add($"@{DbFieldSaveObject}", DbType.String).Value = saveObjectJson;
            cmd.Parameters.Add($"@{DbFieldThumbnail}", DbType.Binary).Value = thumbnailByteArray;
            cmd.Parameters.Add($"@{DbFieldModified}", DbType.Int32).Value = unixTimeModified;
            cmd.Parameters.Add($"@{DbFiledGuid}", DbType.String).Value =
                dbObject.MetaData.Guid.ToString();

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
                var saveMetaData = new MetaData();
                var saveObject =
                    javaScriptSerializer.Deserialize<GameData>((string) sqLiteDataReader[DbFieldSaveObject]);
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

                dbObject.GameData = saveObject;
                dbObject.MetaData = saveMetaData;
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
            cmd.Parameters.Add($"{DbFiledGuid}", DbType.String).Value =
                dbObject.MetaData.Guid.ToString();

            cmd.ExecuteNonQuery();
            con.Close();
        }

        private static SQLiteConnection EstablishDbConnection()
        {
            var con = new SQLiteConnection($"Data Source = {DbName}; Version = 3;");
            con.Open();
            return con;
        }
    }
}