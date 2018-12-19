using Dropbox.Api;
using MySql.Data.MySqlClient;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    // TODO:データベースの所は気が向いたらリファクタリング

    /// <summary>
    /// メタデータ生成
    /// </summary>
    class GenerateUserControlModel : ModelBase
    {
        #region Fields

        /// <summary>
        /// データベースから収集したメタデータ
        /// </summary>
        private MetaData MetaData { get; set; }

        /// <summary>
        /// 選択ファイル名
        /// </summary>
        public string SelectedFile { get; set; }

        private string fileName;
        /// <summary>
        /// 生成ファイル名
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set { SetProperty(ref fileName, value); }
        }

        /// <summary>
        /// 接続先のリスト
        /// </summary>
        public ObservableCollection<string> DataList { get; } = new ObservableCollection<string>();

        /// <summary>
        /// セーブデータ使用画面
        /// </summary>
        protected override bool UseSaveData
        {
            get { return true; }
        }
        #endregion

        #region initialize
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GenerateUserControlModel()
        {
            MetaData = new MetaData();
        }
        #endregion

        /// <summary>
        /// ファイル選択されたときの処理
        /// </summary>
        /// <returns></returns>
        public void SelectFile(string value)
        {
            SelectedFile = value;
        }

        #region ボタン

        /// <summary>
        /// 生成ボタン
        /// </summary>
        public async Task Generate()
        {
            if (CheckRequiredFields())
            {
                // 対象のデータを取得
                ConnectionSettingData connectionData = await DropboxHelper.MultiLoadAsync<ConnectionSettingData>(
                    SaveData.DataOutput, SelectedFile,
                    Constants.ApplicationDirectoryDropbox + Constants.ConnectionDirectory,
                    SaveData.LocalDirectory + Constants.ConnectionDirectory,
                    SaveData.AccessToken
                    );

                // 接続してデータを取得する
                MetaData = new MetaData();
                var tables = new List<Table>();
                if (connectionData.DatabaseEngine == DatabaseEngine.MariaDB)
                {
                    #region MySQL処理（もうクラス作らない）
                    try
                    {
                        using (var connection = new MySqlConnection(connectionData.ConnectionString))
                        {
                            connection.Open();

                            // テーブル一覧取得
                            var tablesData = ExecuteQuery(connection, "SHOW TABLES");

                            // テーブル一覧のカラム名を取得（スキーマ名によって変動するため）
                            var cName = string.Empty;
                            foreach (DataColumn item in tablesData.Columns)
                            {
                                cName = item.ColumnName;
                            }

                            // それぞれのテーブルに対する処理
                            foreach (DataRow tableRow in tablesData.Rows)
                            {
                                var columns = new List<Column>();
                                var table = new Table();
                                var indexList = new List<string>();
                                var indexs = new Dictionary<string, List<Element>>();

                                // テーブル名
                                table.RawName = tableRow[cName].ToString();
                                table.Name = new Element(GetTableName(connectionData, table.RawName));

                                // インデックス情報を取得
                                var indexData = ExecuteQuery(connection, $"SHOW INDEX FROM {table.RawName};");
                                foreach (DataRow row in indexData.Rows)
                                {
                                    var kn = row["Key_name"].ToString();
                                    var cn = row["Column_name"].ToString();
                                    if (!indexs.ContainsKey(kn))
                                    {
                                        indexs.Add(kn, new List<Element>());
                                    }
                                    indexs[kn].Add(new Element(cn));

                                    if (!indexList.Contains(cn))
                                    {
                                        indexList.Add(cn);
                                    }
                                }

                                // 各テーブルコメント
                                var tableCommentData = ExecuteQuery(connection, $"show table status like '{table.RawName}'");
                                foreach (DataRow commentRow in tableCommentData.Rows)
                                {
                                    table.Comment = commentRow["Comment"].ToString();
                                }

                                // 各カラム情報
                                var tableColumnCommentData = ExecuteQuery(connection, $"show full columns from {table.RawName}");

                                // 各カラムごとの処理
                                foreach (DataRow commentRow in tableColumnCommentData.Rows)
                                {
                                    var column = new Column();
                                    column.Name = new Element(commentRow["Field"].ToString());
                                    column.IsKey = !string.IsNullOrEmpty(commentRow["Key"].ToString()) && commentRow["Key"].ToString() == "PRI";
                                    column.Comment = commentRow["Comment"].ToString();
                                    column.IsNullable = commentRow["Null"].ToString() == "YES";
                                    column.DataTypeRaw = commentRow["Type"].ToString();
                                    column.Length = GetLengthMariaDB(column.DataTypeRaw);    // 文字列型のみ
                                    column.DataType = GetDataType(column.DataTypeRaw);

                                    // キー情報（最初の1つだけ取得、複合キー非対応）
                                    if (column.IsKey && table.KeyName == null)
                                    {
                                        table.KeyDataTypeRaw = column.DataTypeRaw;
                                        table.KeyDataType = GetDataType(table.KeyDataTypeRaw);
                                        table.KeyName = new Element(column.Name.Raw);
                                    }

                                    columns.Add(column);
                                }
                                // インデックス情報を設定
                                foreach (var item in columns)
                                {
                                    // インデックスリストにそのカラム名があるか
                                    if (indexList.Contains(item.Name.Raw))
                                    {
                                        item.IndexClass = item.Name.Raw;
                                        if (table.KeyName != null)
                                        {
                                            var end = $"_{table.KeyName.Raw}";
                                            if (item.Name.Raw.EndsWith(end))
                                            {
                                                // "_id"が付いていたら設定する
                                                item.IndexClass = item.IndexClass.Remove(item.IndexClass.Length - end.Length).SnakeToUpperCamel();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        item.IndexClass = null;
                                    }
                                }
                                // インデックスのリストから"_id"を除去して登録
                                var indexColumns = new List<Element> ();
                                table.Columns = columns;
                                foreach (var item in indexList)
                                {
                                    var end = $"_{table.KeyName.Raw}";
                                    if (item.EndsWith(end))
                                    {
                                        item.Remove(item.Length - end.Length).SnakeToUpperCamel();
                                        indexColumns.Add(new Element(item.Remove(item.Length - end.Length)));
                                    }
                                }
                                table.IndexColumns = indexColumns;
                                table.Indexs = indexs;
                                tables.Add(table);
                            }
                            MetaData.Tables = tables;
                        }
                    }
                    catch (Exception e)
                    {
                        ActionResult = $"接続に失敗したみたい:{e.Message}";
                        return;
                    }
                    #endregion
                }
                else if (connectionData.DatabaseEngine == DatabaseEngine.SqlServer)
                {
                    #region SQLServer処理（もうクラス作らない）
                    try
                    {
                        using (var connection = new SqlConnection(connectionData.ConnectionString))
                        {
                            connection.Open();

                            // テーブル一覧取得
                            var tablesData = ExecuteQuery(connection, "SELECT t.name, ep.value FROM sys.tables t, sys.extended_properties ep WHERE t.object_id = ep.major_id AND ep.minor_id = 0;");

                            // キー情報（1テーブル1つのみ、それ以上は無視）
                            var keyList = new Dictionary<string, string>();
                            var tableKeyData = ExecuteQuery(connection, $"SELECT tbls.name AS table_name, cols.name AS col_name FROM sys.tables tbls INNER JOIN sys.key_constraints key_const ON tbls.object_id = key_const.parent_object_id AND key_const.type = 'PK' INNER JOIN sys.index_columns idx_cols ON key_const.parent_object_id = idx_cols.object_id AND key_const.unique_index_id = idx_cols.index_id INNER JOIN sys.columns cols ON idx_cols.object_id = cols.object_id AND idx_cols.column_id = cols.column_id");
                            foreach (DataRow tableRow in tableKeyData.Rows)
                            {
                                keyList.Add(tableRow["table_name"].ToString(), tableRow["col_name"].ToString());
                            }

                            // インデックス情報（キー以外）
                            // 親テーブル名はID名で特定する（user_authority_id -> tt_user_authority）
                            // 生テーブル名と生カラム名を格納
                            var indexList = new List<string>();
                            var indexs = new Dictionary<string, List<Element>>();
                            var tableIndexData = ExecuteQuery(connection, $"SELECT sys.indexes.name AS index_name, sys.index_columns.object_id, sys.index_columns.column_id, sys.objects.name AS table_name, sys.columns.name AS column_name FROM sys.indexes INNER JOIN sys.index_columns ON sys.indexes.index_id = sys.index_columns.index_id AND sys.indexes.object_id = sys.index_columns.object_id INNER JOIN sys.objects ON sys.indexes.object_id = sys.objects.object_id INNER JOIN sys.columns ON sys.index_columns.object_id = sys.columns.object_id AND sys.index_columns.column_id = sys.columns.column_id WHERE sys.objects.type = 'U' ORDER BY sys.indexes.object_id, sys.indexes.name, sys.index_columns.column_id;");
                            foreach (DataRow tableRow in tableIndexData.Rows)
                            {
                                var indexName = tableRow["index_name"].ToString();
                                var tableName = tableRow["table_name"].ToString();
                                var columnName = tableRow["column_name"].ToString();

                                if (!indexs.ContainsKey(indexName))
                                {
                                    indexs.Add(indexName, new List<Element>());
                                }
                                indexs[indexName].Add(new Element(columnName));

                                // 重複は登録しない
                                if (!indexList.Contains(columnName))
                                {
                                    indexList.Add(columnName);
                                }
                            }

                            // それぞれのテーブルに対する処理
                            foreach (DataRow tableRow in tablesData.Rows)
                            {
                                var columns = new List<Column>();
                                var table = new Table();
                                // テーブル名
                                table.RawName = tableRow["name"].ToString();
                                table.Name = new Element(GetTableName(connectionData, table.RawName));

                                // テーブルコメント
                                table.Comment = tableRow["value"].ToString();

                                // 各カラム情報
                                var tableColumnCommentData = ExecuteQuery(connection, $"SELECT b.name as datatype, c.name, c.is_nullable, c.max_length, ep.value FROM sys.tables t, sys.types b, sys.columns c, sys.extended_properties ep WHERE c.system_type_id = b.system_type_id AND t.name = '{table.RawName}' AND t.object_id = c.object_id AND c.object_id = ep.major_id AND c.column_id = ep.minor_id;");

                                // 各カラムごとの処理
                                foreach (DataRow columnRow in tableColumnCommentData.Rows)
                                {
                                    var column = new Column();
                                    column.Name = new Element(columnRow["name"].ToString());

                                    column.Comment = columnRow["value"].ToString();
                                    column.IsNullable = (bool)columnRow["is_nullable"];
                                    column.DataTypeRaw = columnRow["datatype"].ToString();
                                    column.Length = string.Empty;
                                    if (column.DataTypeRaw == "varchar") // 文字列型のみ
                                    {
                                        column.Length = columnRow["max_length"].ToString();
                                    }
                                    column.DataType = GetDataType(column.DataTypeRaw);

                                    // キー情報（最初の1つだけ取得、複合キー非対応）
                                    column.IsKey = false;
                                    if (keyList.ContainsKey(table.RawName) && keyList[table.RawName] == column.Name.Raw)
                                    {
                                        column.IsKey = true;
                                    }
                                    if (column.IsKey && table.KeyName == null)
                                    {
                                        table.KeyDataTypeRaw = column.DataTypeRaw;
                                        table.KeyDataType = GetDataType(table.KeyDataTypeRaw);
                                        table.KeyName = new Element(column.Name.Raw);
                                    }

                                    columns.Add(column);
                                }
                                // インデックス情報を設定
                                foreach (var item in columns)
                                {
                                    // インデックスリストにそのカラム名があるか
                                    if (indexList.Contains(item.Name.Raw))
                                    {
                                        item.IndexClass = item.Name.Raw;
                                        if (table.KeyName != null)
                                        {
                                            var end = $"_{table.KeyName.Raw}";
                                            if (item.Name.Raw.EndsWith(end))
                                            {
                                                item.IndexClass = item.IndexClass.Remove(item.IndexClass.Length - end.Length).SnakeToUpperCamel();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        item.IndexClass = null;
                                    }
                                }
                                table.Columns = columns;
                                // インデックスのリストから"_id"を除去して登録
                                var indexColumns = new List<Element>();
                                table.Columns = columns;
                                foreach (var item in indexList)
                                {
                                    var end = $"_{table.KeyName.Raw}";
                                    if (item.EndsWith(end))
                                    {
                                        item.Remove(item.Length - end.Length).SnakeToUpperCamel();
                                        indexColumns.Add(new Element(item.Remove(item.Length - end.Length)));
                                    }
                                }
                                table.IndexColumns = indexColumns;
                                table.Indexs = indexs;
                                tables.Add(table);
                            }
                            MetaData.Tables = tables;
                        }
                    }
                    catch (Exception e)
                    {
                        ActionResult = $"接続に失敗したみたい:{e.Message}";
                        return;
                    }
                    #endregion
                }

                // Dropboxかローカルに保存
                await SimpleSave(Constants.MetaDataDirectory, $"{FileName}{Constants.Extension}", MetaData);
            }
        }

        /// <summary>
        /// 実行条件
        /// </summary>
        /// <returns></returns>
        protected override bool CheckRequiredFields()
        {
            if (string.IsNullOrWhiteSpace(SelectedFile))
            {
                FileName = "どの接続ファイル使うねん";
                return false;
            }

            if (string.IsNullOrWhiteSpace(FileName))
            {
                FileName = "なんか書けや";
                return false;
            }
            return true;
        }
        #endregion

        #region 画面が表示されたときの処理

        protected override async Task Activate()
        {
            // 接続先データ読み込み（ディレクトリのファイル一覧を取得）
            await SimpleFileList(Constants.ConnectionDirectory, DataList);
        }
        #endregion

        #region private
        /// <summary>
        /// プレフィクスを取り除く
        /// </summary>
        /// <param name="connectionData"></param>
        /// <param name="rawName"></param>
        /// <returns></returns>
        private string GetTableName(ConnectionSettingData connectionData, string rawName)
        {
            foreach (var item in connectionData.PrefixList)
            {
                if (rawName.StartsWith(item))
                {
                    return rawName.Remove(0, item.Length);
                }
            }
            return rawName;
        }

        #region SQL Server
        /// <summary>
        /// SQL Server
        /// クエリ実行
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private DataTable ExecuteQuery(SqlConnection connection, string query)
        {
            SqlCommand com = new SqlCommand(query, connection);

            SqlDataAdapter sda = new SqlDataAdapter
            {
                SelectCommand = com
            };

            DataSet ds = new DataSet();
            sda.Fill(ds);

            return ds.Tables[0];
        }
        #endregion

        #region MySQL, MariaDB
        /// <summary>
        /// MySQL, MariaDB
        /// クエリ実行
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private DataTable ExecuteQuery(MySqlConnection connection, string query)
        {
            var dataTable = new DataTable();
            var mySqlDataAdapter = new MySqlDataAdapter(query, connection);
            mySqlDataAdapter.Fill(dataTable);

            return dataTable;
        }

        /// <summary>
        /// MySQL,MariaDB
        /// varcharの場合、長さを取得する
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetLengthMariaDB(string type)
        {
            var length = string.Empty;
            if (type.Contains("varchar"))
            {
                if (type.Contains("("))
                {
                    length = type.Split('(')[1];
                    length = length.Split(')')[0];
                }
            }
            return length;
        }

        /// <summary>
        /// MySQL,MariaDB
        /// カッコを除去する
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetDataType(string type)
        {
            if (type.Contains("("))
            {
                type = type.Split('(')[0];
            }
            return type;
        }
        #endregion

        #endregion

    }
}
