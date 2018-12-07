using Dropbox.Api;
using Dropbox.Api.Files;
using MySql.Data.MySqlClient;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    class ConnectionRegisterUserControlModel : BindableBase
    {
        #region Fields

        /// <summary>
        /// 設定ファイル
        /// </summary>
        private SaveData SaveData { get; set; }

        #region ConnectionSettingData
        /// <summary>
        /// ファイルとして出力する設定項目
        /// </summary>
        private ConnectionSettingData ConnectionSettingData { get; set; }

        public ObservableCollection<string> PrefixList { get { return ConnectionSettingData.PrefixList; } }

        public ObservableCollection<string> CommonColumnList { get { return ConnectionSettingData.CommonColumnList; } }

        private DatabaseEngine databaseEngine;
        /// <summary>
        /// 対象データベース
        /// </summary>
        public DatabaseEngine DatabaseEngine
        {
            get
            {
                databaseEngine = ConnectionSettingData.DatabaseEngine;
                return databaseEngine;
            }
            set
            {
                ConnectionSettingData.DatabaseEngine = value;
                SetProperty(ref databaseEngine, value);
            }
        }

        private string connectionString;
        /// <summary>
        /// 接続文字列
        /// </summary>
        public string ConnectionString
        {
            get
            {
                connectionString = ConnectionSettingData.ConnectionString;
                return connectionString;
            }
            set
            {
                ConnectionSettingData.ConnectionString = value;
                SetProperty(ref connectionString, value);
            }
        }

        private string host;
        /// <summary>
        /// ホスト
        /// </summary>
        public string Host
        {
            get
            {
                return host;
            }
            set
            {
                SetProperty(ref host, value);
            }
        }

        private string port;
        /// <summary>
        /// ポート番号
        /// </summary>
        public string Port
        {
            get
            {
                return port;
            }
            set
            {
                SetProperty(ref port, value);
            }
        }

        private string dbName;
        /// <summary>
        /// データベース名
        /// </summary>
        public string DbName
        {
            get
            {
                return dbName;
            }
            set
            {
                SetProperty(ref dbName, value);
            }
        }

        private string account;
        /// <summary>
        /// アカウント
        /// </summary>
        public string Account
        {
            get
            {
                return account;
            }
            set
            {
                SetProperty(ref account, value);
            }
        }

        private string password;
        /// <summary>
        /// パスワード
        /// </summary>
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                SetProperty(ref password, value);
            }
        }
        #endregion

        private string inputPrefix;
        /// <summary>
        /// プレフィクス入力
        /// </summary>
        public string InputPrefix
        {
            get { return inputPrefix; }
            set { SetProperty(ref inputPrefix, value); }
        }

        private string inputCommonColumn;
        /// <summary>
        /// 共通項目入力
        /// </summary>
        public string InputCommonColumn
        {
            get { return inputCommonColumn; }
            set { SetProperty(ref inputCommonColumn, value); }
        }

        private string title;
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
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
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConnectionRegisterUserControlModel()
        {
            ConnectionSettingData = new ConnectionSettingData();
            CheckResult = "チェック結果";

            // サンプルデータ
            PrefixList.Add("mt_");
            PrefixList.Add("tt_");
            CommonColumnList.Add("created_at");
            CommonColumnList.Add("created_by");
            CommonColumnList.Add("updated_at");
            CommonColumnList.Add("updated_by");
            CommonColumnList.Add("deleted");
        }

        #region ボタン
        /// <summary>
        /// 確認＆保存ボタン
        /// </summary>
        /// <returns></returns>
        public async Task CheckAndSave()
        {
            try
            {
                if (SaveData != null)
                {
                    var portStr = string.Empty;
                    if (!string.IsNullOrWhiteSpace(Port))
                    {
                        portStr = "," + Port;
                    }

                    var builder = new SqlConnectionStringBuilder()
                    {
                        DataSource = Host + portStr,
                        IntegratedSecurity = false, // Windows認証ではなく、ID,Pass認証
                        UserID = $"{Account}",
                        Password = $"{Password}"
                    };
                    // MariaDBはInitialCatalogできないため
                    ConnectionString = $"{builder.ToString()};database={DbName};";

                    // 繋がるかどうか確認
                    if (ConnectionSettingData.DatabaseEngine == DatabaseEngine.MariaDB)
                    {
                        var connection = new MySqlConnection(ConnectionString);
                        connection.Open();
                    }
                    else if (ConnectionSettingData.DatabaseEngine == DatabaseEngine.SqlServer)
                    {
                        var connection = new SqlConnection(ConnectionString);
                        connection.Open();
                    }

                    if (!string.IsNullOrWhiteSpace(Title))
                    {
                        // OKならDropboxかローカルに保存
                        await DropboxHelper.MultiSaveAsync(SaveData.DataOutput, $"{Title}{Constants.Extension}", ConnectionSettingData, Constants.ApplicationDirectoryDropbox + Constants.ConnectionDirectory, SaveData.LocalDirectory + Constants.ConnectionDirectory, SaveData.AccessToken);

                        CheckResult = "チェックOK、保存したぜ";
                    }
                    else
                    {
                        CheckResult = "…おい、タイトルを入力してくれよ保存できねぇだろ？";
                    }

                }
            }
            catch (Exception e)
            {
                CheckResult = "チェックNG:" + e.Message;
            }
        }

        /// <summary>
        /// 追加
        /// </summary>
        public void AddPrefix()
        {
            if (!string.IsNullOrWhiteSpace(InputPrefix) && !PrefixList.Contains(InputPrefix))
            {
                PrefixList.Add(InputPrefix.Trim());
            }
            InputPrefix = string.Empty;
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="value"></param>
        public void DeletePrefix(string value)
        {
            var obj = PrefixList.FirstOrDefault(e => e == value);
            PrefixList.Remove(obj);
        }

        /// <summary>
        /// 追加
        /// </summary>
        public void AddCommonColumn()
        {
            if (!string.IsNullOrWhiteSpace(InputCommonColumn) && !CommonColumnList.Contains(InputCommonColumn))
            {
                CommonColumnList.Add(InputCommonColumn.Trim());
            }
            InputCommonColumn = string.Empty;
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="value"></param>
        public void DeleteCommonColumn(string value)
        {
            var obj = CommonColumnList.FirstOrDefault(e => e == value);
            CommonColumnList.Remove(obj);
        }
        #endregion

        #region 画面が表示されたときの処理
        /// <summary>
        /// 画面が表示されたときの処理
        /// </summary>
        /// <returns></returns>
        public async Task ActivateAsync()
        {
            // データがあるかチェック
            SaveData = await Json.Load<SaveData>(Constants.DataDirectory, Constants.DataFileName);

            if (SaveData == null)
            {
                // セーブデータがない場合
                CheckResult = "接続先の設定ができないぜ。先に設定画面の設定を完了させてくれよな！";
            }
        }
        #endregion
    }
}

