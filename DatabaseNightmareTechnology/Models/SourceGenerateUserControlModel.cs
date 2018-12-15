using Prism.Mvvm;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO:できたらModelの親クラスを作ってリファクタリング
// TODO:汎用入力のフォーマットを変える。値を全てList<string>にする。
namespace DatabaseNightmareTechnology.Models
{
    /// <summary>
    /// 出力結果
    /// </summary>
    class SourceGenerateUserControlModel : BindableBase
    {
        #region Fields
        // Razorのエンジン上に配置するテンプレートのキー
        private readonly string FilenameKeyBase = "FilenameKey";
        private readonly string BodyKeyBase = "BodyKey";

        ///// <summary>
        ///// ファイル名の生成の際に一時的に出力するファイル名
        ///// </summary>
        //private readonly string TmpTitle = "TmpTitle";

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

        /// <summary>
        /// 選択中の値
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// 選択中の値
        /// </summary>
        public string Connection { get; set; }

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
            SaveResult = "結果";
        }
        #endregion


        #region ボタン
        /// <summary>
        /// 生成ボタン処理
        /// </summary>
        /// <returns></returns>
        public async Task Generate()
        {
            if (string.IsNullOrWhiteSpace(Template))
            {
                SaveResult = "テンプレートはちゃんと選んでくれよな";
            }
            else
            {
                try
                {
                    // キー生成
                    var FilenameKey = FilenameKeyBase + Json.GetUnixTime();
                    var BodyKey = BodyKeyBase + Json.GetUnixTime();

                    // テンプレート読み込み、エンジンに設定
                    var template = await DropboxHelper.MultiLoadAsync<DmtTemplate>(SaveData.DataOutput, Template, Constants.ApplicationDirectoryDropbox + Constants.TemplateDirectory, SaveData.LocalDirectory + Constants.TemplateDirectory, SaveData.AccessToken);
                    Engine.Razor.AddTemplate(FilenameKey, template.GenerateFileName);
                    Engine.Razor.Compile(FilenameKey);
                    Engine.Razor.AddTemplate(BodyKey, template.TemplateBody);
                    Engine.Razor.Compile(BodyKey);

                    // 汎用入力読み込み
                    GeneralInput general = null;
                    if (!string.IsNullOrWhiteSpace(General))
                    {
                        general = await DropboxHelper.MultiLoadAsync<GeneralInput>(SaveData.DataOutput, General, Constants.ApplicationDirectoryDropbox + Constants.GeneralInputDirectory, SaveData.LocalDirectory + Constants.GeneralInputDirectory, SaveData.AccessToken);
                    }
                    if (string.IsNullOrWhiteSpace(Connection))
                    {
                        if (general == null)
                        {
                            await DropboxHelper.MultiSaveStringAsync(SaveData.DataOutput, template.GenerateFileName, template.TemplateBody, Constants.ApplicationDirectoryDropbox + Constants.OutputSourceDirectory, SaveData.LocalDirectory + Constants.OutputSourceDirectory, SaveData.AccessToken);
                            SaveResult = "成功だ。保存したぜ";
                        }
                        else
                        {
                            // 出力ファイル名生成取得
                            var filename = Engine.Razor.Run(FilenameKey, null, general);
                            Console.WriteLine("ファイル名：" + filename);

                            // 単独生成
                            var result = Engine.Razor.Run(BodyKey, null, general);

                            // 保存
                            await DropboxHelper.MultiSaveStringAsync(SaveData.DataOutput, filename, result, Constants.ApplicationDirectoryDropbox + Constants.OutputSourceDirectory, SaveData.LocalDirectory + Constants.OutputSourceDirectory, SaveData.AccessToken);
                            SaveResult = "成功だ。保存したぜ";
                        }
                    }
                    else
                    {
                        // メタデータ読み込み
                        var metadata = await DropboxHelper.MultiLoadAsync<MetaData>(SaveData.DataOutput, Connection, Constants.ApplicationDirectoryDropbox + Constants.MetaDataDirectory, SaveData.LocalDirectory + Constants.MetaDataDirectory, SaveData.AccessToken);
                        metadata.GeneralInput = general;

                        foreach (Table table in metadata.Tables)
                        {
                            // 出力ファイル名生成取得
                            var filename = Engine.Razor.Run(FilenameKey, null, table);
                            Console.WriteLine("ファイル名：" + filename);

                            // 生成
                            var result = Engine.Razor.Run(BodyKey, null, table);

                            // 保存
                            await DropboxHelper.MultiSaveStringAsync(SaveData.DataOutput, filename, result, Constants.ApplicationDirectoryDropbox + Constants.OutputSourceDirectory, SaveData.LocalDirectory + Constants.OutputSourceDirectory, SaveData.AccessToken);
                        }
                        SaveResult = "成功だ。保存したぜ";
                    }
                }
                catch (Exception e)
                {
                    SaveResult = e.Message;
                }
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
            SaveResult = "結果";
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
