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
        private readonly string FilenameKey = "FilenameKey";
        private readonly string BodyKey = "BodyKey";

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
            if (string.IsNullOrWhiteSpace(Template))
            {
                SaveResult = "テンプレートはちゃんと選んでくれよな";
            }
            else
            {
                // テンプレート読み込み、エンジンに設定
                var template = await DropboxHelper.MultiLoadAsync<DmtTemplate>(SaveData.DataOutput, Template, Constants.ApplicationDirectoryDropbox + Constants.TemplateDirectory, SaveData.LocalDirectory + Constants.TemplateDirectory, SaveData.AccessToken);
                Engine.Razor.AddTemplate(FilenameKey, template.GenerateFileName);
                Engine.Razor.AddTemplate(BodyKey, template.TemplateBody);

                // 汎用入力読み込み
                GeneralInput general = null;
                if (!string.IsNullOrWhiteSpace(General))
                {
                    general = await DropboxHelper.MultiLoadAsync<GeneralInput>(SaveData.DataOutput, General, Constants.ApplicationDirectoryDropbox + Constants.GeneralInputDirectory, SaveData.LocalDirectory + Constants.GeneralInputDirectory, SaveData.AccessToken);
                }

                if (string.IsNullOrWhiteSpace(Connection))
                {
                    // 出力ファイル名生成取得
                    var filename = Engine.Razor.RunCompile(FilenameKey, null, general);
                    Console.WriteLine("ファイル名：" + filename);

                    // 単独生成
                    var result = Engine.Razor.RunCompile(BodyKey, null, general);
                    Console.WriteLine(result);

                    // 保存
                    await DropboxHelper.MultiSaveStringAsync(SaveData.DataOutput, result, Constants.ApplicationDirectoryDropbox + Constants.OutputSourceDirectory, SaveData.LocalDirectory + Constants.OutputSourceDirectory, SaveData.AccessToken);
                    SaveResult = "成功だ。保存したぜ";
                }
                else
                {
                    // TODO:あとここだけ

                    // メタデータ読み込み
                    var metadata = await DropboxHelper.MultiLoadAsync<MetaData>(SaveData.DataOutput, Connection, Constants.ApplicationDirectoryDropbox + Constants.MetaDataDirectory, SaveData.LocalDirectory + Constants.MetaDataDirectory, SaveData.AccessToken);
                    metadata.GeneralInput = general;

                    foreach (var table in metadata.Tables)
                    {
                        // 出力ファイル名生成取得
                        var filename = Engine.Razor.RunCompile(FilenameKey, null, table);
                        Console.WriteLine("ファイル名：" + filename);

                        // 生成
                        var result = Engine.Razor.RunCompile(BodyKey, null, table);
                        Console.WriteLine(result);

                        // 保存
                        await DropboxHelper.MultiSaveStringAsync(SaveData.DataOutput, result, Constants.ApplicationDirectoryDropbox + Constants.OutputSourceDirectory, SaveData.LocalDirectory + Constants.OutputSourceDirectory, SaveData.AccessToken);
                    }
                    SaveResult = "成功だ。保存したぜ";
                }
            }

            // これでキーが解放されるはず？
            Engine.Razor.Dispose();
        }

        //private string GetOutFilePath(object model)
        //{
        //    // 生成ソース一時出力パス生成
        //    var result = string.Empty;
        //    //using (var writer = DropboxHelper.GetWriter(TmpTitle))
        //    //{
        //    //    // ファイル名生成
        //    //    Engine.Razor.RunCompile(FilenameKey, writer, null, model);
        //    //    writer.Close();

        //    //    // 生成したファイル名読み込み
        //    //    var name = Json.LoadString(DropboxHelper.GetTempFilePath(TmpTitle));

        //    //    // 生成ソース一時出力パス取得
        //    //    result = DropboxHelper.GetTempFilePath(name);
        //    //}
        //    // 生成ソース一時出力パス取得
        //    var result = Engine.Razor.RunCompile(FilenameKey, null, model);
        //    return result;
        //}

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
