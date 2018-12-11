using Prism.Mvvm;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    /// <summary>
    /// 出力結果
    /// </summary>
    class SourceGenerateUserControlModel : BindableBase
    {
        #region Fields
        /// <summary>
        /// 設定ファイル
        /// </summary>
        private SaveData SaveData { get; set; }

        /// <summary>
        /// テンプレートリスト
        /// </summary>
        public ObservableCollection<string> TemplateList { get; } = new ObservableCollection<string>();

        /// <summary>
        /// データベース情報
        /// 選択しない場合、汎用入力による単発生成
        /// </summary>
        public ObservableCollection<string> ConnectionList { get; } = new ObservableCollection<string>();

        /// <summary>
        /// 汎用データ（任意）
        /// リストデータは、名前に共通のプレフィクスを付ける。
        /// リスト名#データ名('data#Field1')
        /// </summary>
        public ObservableCollection<string> GeneralList { get; } = new ObservableCollection<string>();

        private string template;
        /// <summary>
        /// 選択中の値
        /// </summary>
        public string Template { get; set; }

        private string connection;
        /// <summary>
        /// 選択中の値
        /// </summary>
        public string Connection { get; set; }

        private string general;
        /// <summary>
        /// 選択中の値
        /// </summary>
        public string General { get; set; }

        private string saveResult;
        /// <summary>
        /// 保存結果
        /// </summary>
        public string SaveResult
        {
            get { return saveResult; }
            set { SetProperty(ref saveResult, value); }
        }
        #endregion

        #region initialize
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SourceGenerateUserControlModel()
        {
            SaveResult = "チェック結果";
        }
        #endregion

        #region ボタン
        /// <summary>
        /// 生成ボタン処理
        /// </summary>
        /// <returns></returns>
        public async Task Generate()
        {
            // 何するかって言うと・・・
            if (string.IsNullOrWhiteSpace(Template))
            {
                SaveResult = "テンプレートはちゃんと選んでくれよな";
            }
            else
            {
                // テンプレート読み込み
                var template = DropboxHelper.MultiLoadAsync<DmtTemplate>(SaveData.DataOutput, Template, Constants.ApplicationDirectoryDropbox + Constants.TemplateDirectory, SaveData.LocalDirectory + Constants.TemplateDirectory, SaveData.AccessToken);

                if (string.IsNullOrWhiteSpace(Connection))
                {
                    // 単独生成
                    Console.WriteLine("単独生成");
                }
                else
                {
                    // テーブル一括生成
                    Console.WriteLine("テーブル一括生成");

                    //columns.Add(new { ColumnName = columnName, ColumnNameCamel = columnNameCamel, ColumnNamePascal = columnNamePascal, DataType = dataType, IsKey = isKey, Comment = comment, IsNullable = isNullable, Length = length });
                    //var model = new
                    //{
                    //    Columns = columns,
                    //    TableComment = tableComment,
                    //    RawTableName = rawTableName,
                    //    TableName = tableName,
                    //    TableNameCamel = tableNameCamel,
                    //    TableNamePascal = tableNamePascal,
                    //    TableKeyDataType = keyColumnDataType,
                    //    TableKeyColumnNamePascal = keyColumnNamePascal,
                    //    TableKeyColumnNameCamel = keyColumnNameCamel
                    //};

                    //Console.WriteLine("================================================================");
                    //// @に代入する
                    //var result1 = Engine.Razor.RunCompile(templateModel.FileName, "templateFile", null, model);
                    //Console.WriteLine(result1);
                    //Console.WriteLine("----------------------------------------------------------------");
                    //var result2 = Engine.Razor.RunCompile(templateModel.Body, "templateBody", null, model);
                    //Console.WriteLine(result2);
                    //// TODO:複数のテンプレートを連続で出力すると、同じキーがあるってエラーが出る

                    //// 出力処理
                    //using (StreamWriter writer = File.CreateText($"{Path}{result1}"))
                    //{
                    //    writer.WriteLine(result2);
                    //}
                }

                // TODO:保存
                // OutputSourceDirectory

                //    // 保存データ
                //    var data = new GeneralInput(Items.ToList());

                //    // OKならDropboxかローカルに保存
                //    await DropboxHelper.MultiSaveAsync(SaveData.DataOutput, $"{FileName}{Constants.Extension}", data, Constants.ApplicationDirectoryDropbox + Constants.GeneralInputDirectory, SaveData.LocalDirectory + Constants.GeneralInputDirectory, SaveData.AccessToken);
                //    SaveResult = "チェックOK、保存したぜ";
            }


        }
        #endregion

        #region リスト選択

        /// <summary>
        /// 選択
        /// </summary>
        /// <param name="value"></param>
        public void SelectTemplate(string value)
        {
            Template = value;
        }

        /// <summary>
        /// 選択
        /// </summary>
        /// <param name="value"></param>
        public void SelectConnection(string value)
        {
            Connection = value;
        }

        /// <summary>
        /// 選択
        /// </summary>
        /// <param name="value"></param>
        public void SelectGeneral(string value)
        {
            General = value;
        }

        #endregion

        #region 画面が表示されたときの処理
        /// <summary>
        /// 画面が表示されたときの処理
        /// </summary>
        /// <returns></returns>
        public async Task ActivateAsync()
        {
            TemplateList.Clear();
            ConnectionList.Clear();
            GeneralList.Clear();
            Template = string.Empty;
            Connection = string.Empty;
            General = string.Empty;
            ConnectionList.Add(string.Empty);
            GeneralList.Add(string.Empty);

            // データがあるかチェック
            SaveData = await Json.Load<SaveData>(Constants.DataDirectory, Constants.DataFileName);

            if (SaveData == null)
            {
                // セーブデータがない場合
                SaveResult = "接続先の設定ができないぜ。先に設定画面の設定を完了させてくれよな！";
            }

            // データ読み込み（ディレクトリのファイル一覧を取得）
            await DropboxHelper.GetFileListAsync(TemplateList, SaveData.DataOutput, Constants.ApplicationDirectoryDropbox + Constants.TemplateDirectory, SaveData.LocalDirectory + Constants.TemplateDirectory, SaveData.AccessToken);
            await DropboxHelper.GetFileListAsync(ConnectionList, SaveData.DataOutput, Constants.ApplicationDirectoryDropbox + Constants.MetaDataDirectory, SaveData.LocalDirectory + Constants.MetaDataDirectory, SaveData.AccessToken);
            await DropboxHelper.GetFileListAsync(GeneralList, SaveData.DataOutput, Constants.ApplicationDirectoryDropbox + Constants.GeneralInputDirectory, SaveData.LocalDirectory + Constants.GeneralInputDirectory, SaveData.AccessToken);

        }
        #endregion
    }
}
