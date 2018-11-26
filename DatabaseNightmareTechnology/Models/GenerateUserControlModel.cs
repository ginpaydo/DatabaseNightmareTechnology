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
    // TODO:ユニーク制約は未実装
    class GenerateUserControlModel : BindableBase
    {
        #region Fields

        /// <summary>
        /// 設定ファイル
        /// </summary>
        private SaveData SaveData { get; set; }

        /// <summary>
        /// データベースから収集したメタデータ
        /// </summary>
        private MetaData MetaData { get; set; }

        private string fileName;
        /// <summary>
        /// 生成ファイル名
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set { SetProperty(ref fileName, value); }
        }

        private string checkResult;
        /// <summary>
        /// チェック結果
        /// </summary>
        public string CheckResult
        {
            get { return checkResult; }
            set { SetProperty(ref checkResult, value); }
        }

        /// <summary>
        /// 接続先のリスト
        /// </summary>
        public ObservableCollection<string> DataList { get; } = new ObservableCollection<string>();
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GenerateUserControlModel()
        {
            MetaData = new MetaData();
            CheckResult = "チェック結果";
        }

        #region ボタン

        /// <summary>
        /// 生成ボタン
        /// </summary>
        /// <param name="value"></param>
        public async Task Generate(string value)
        {
            #region 入力チェック
            if (string.IsNullOrWhiteSpace(value))
            {
                FileName = "どの接続ファイル使うねん";
                return;
            }

            if (string.IsNullOrWhiteSpace(FileName))
            {
                FileName = "なんか書けや";
                return;
            }
            #endregion

            // 対象のデータを取得
            ConnectionSettingData connectionData = await Json.LoadMultiData<ConnectionSettingData>(
                SaveData.DataOutput, value,
                Constants.ApplicationDirectoryDropbox + Constants.ConnectionDirectory,
                SaveData.LocalDirectory + Constants.ConnectionDirectory,
                SaveData.AccessToken
                );

            // 接続してデータを取得する
            MetaData = new MetaData();
            var tables = new List<Table>();
            var columns = new List<Column>();
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
                            var table = new Table();
                            // テーブル名
                            table.RawName = tableRow[cName].ToString();
                            table.Name = GetTableName(connectionData, table.RawName);
                            table.NameCamel = table.Name.SnakeToLowerCamel();
                            table.NamePascal = table.Name.SnakeToUpperCamel();

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
                                column.Name = commentRow["Field"].ToString();
                                column.NameCamel = column.Name.SnakeToLowerCamel();
                                column.NamePascal = column.Name.SnakeToUpperCamel();
                                column.IsKey = !String.IsNullOrEmpty(commentRow["Key"].ToString()) && commentRow["Key"].ToString() == "PRI";
                                column.Comment = commentRow["Comment"].ToString();
                                column.IsNullable = commentRow["Null"].ToString() == "YES";
                                column.DataTypeRaw = commentRow["Type"].ToString();
                                column.Length = GetLength(column.DataTypeRaw);    // 文字列型のみ

                                // キー情報（最初の1つだけ取得、複合キー非対応）
                                if (column.IsKey && string.IsNullOrEmpty(table.KeyName))
                                {
                                    table.KeyDataTypeRaw = column.DataTypeRaw;
                                    table.KeyName = column.Name;
                                    table.KeyNamePascal = column.NamePascal;
                                    table.KeyNameCamel = column.NameCamel;
                                }

                                columns.Add(column);
                            }
                            table.Columns = columns;
                            tables.Add(table);
                        }
                        MetaData.Tables = tables;
                    }
                }
                catch (Exception e)
                {
                    CheckResult = $"接続に失敗したみたい:{e.Message}";
                }
                #endregion
            }
            else if (connectionData.DatabaseEngine == DatabaseEngine.SqlServer)
            {
                using (var connection = new SqlConnection(connectionData.ConnectionString))
                {
                    connection.Open();
                }
            }

            // Dropboxかローカルに保存
            await Json.MultiSaveAsync(SaveData.DataOutput, $"{FileName}.dat", MetaData, Constants.ApplicationDirectoryDropbox + Constants.MetaDataDirectory, SaveData.LocalDirectory + Constants.MetaDataDirectory, SaveData.AccessToken);
            CheckResult = $"保存できたよー";
        }

        /// <summary>
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

        private string GetLength(string type)
        {
            var length = String.Empty;
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

        #endregion

        #region 画面が表示されたときの処理
        /// <summary>
        /// 画面が表示されたときの処理
        /// </summary>
        /// <returns></returns>
        public async Task ActivateAsync()
        {
            DataList.Clear();

            // データがあるかチェック
            SaveData = await Json.Load<SaveData>(Constants.DataDirectory, Constants.DataFileName);

            if (SaveData == null)
            {
                // セーブデータがない場合
                CheckResult = "接続先の設定ができないぜ。先に設定画面の設定を完了させてくれよな！";
            }

            // 接続先データ読み込み（ディレクトリのファイル一覧を取得）
            await Json.GetFileList(DataList, SaveData.DataOutput, Constants.ApplicationDirectoryDropbox + Constants.ConnectionDirectory, SaveData.LocalDirectory + Constants.ConnectionDirectory, SaveData.AccessToken);

        }
        #endregion
    }
}
