using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    /// <summary>
    /// 出力結果
    /// </summary>
    class SourceGenerateUserControlModel : ModelBase
    {
        #region Fields
        // Razorのエンジン上に配置するテンプレートのキー
        private readonly string FilenameKeyBase = "FilenameKey";
        private readonly string BodyKeyBase = "BodyKey";

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

        /// <summary>
        /// セーブデータ使用画面
        /// </summary>
        protected override bool UseSaveData
        {
            get { return true; }
        }
        #endregion

        #region ボタン
        /// <summary>
        /// 生成ボタン処理
        /// </summary>
        /// <returns></returns>
        public async Task Generate()
        {
            if (CheckRequiredFields())
            {
                SaveResult = MessageConstants.NotChooseTemplate;
            }
            else
            {
                try
                {
                    // 実行結果
                    var resultStr = new StringBuilder(MessageConstants.ActionSucceed);

                    // キー生成
                    var FilenameKey = FilenameKeyBase + Json.GetUnixTime();
                    var BodyKey = BodyKeyBase + Json.GetUnixTime();

                    // テンプレート読み込み、エンジンに設定
                    var template = await SimpleLoad<DmtTemplate>(Constants.TemplateDirectory, Template);
                    Engine.Razor.AddTemplate(FilenameKey, template.GenerateFileName);
                    Engine.Razor.Compile(FilenameKey);
                    Engine.Razor.AddTemplate(BodyKey, template.TemplateBody);
                    Engine.Razor.Compile(BodyKey);

                    // 汎用入力読み込み
                    GeneralInput general = null;
                    if (!string.IsNullOrWhiteSpace(General))
                    {
                        general = await SimpleLoad<GeneralInput>(Constants.GeneralInputDirectory, General);
                    }
                    if (string.IsNullOrWhiteSpace(Connection))
                    {
                        if (general == null)
                        {
                            await SimpleSaveString(Constants.OutputSourceDirectory, template.GenerateFileName, template.TemplateBody);
                            resultStr.Append("\nファイル名：" + template.GenerateFileName);
                            SaveResult = resultStr.ToString();
                        }
                        else
                        {
                            // 出力ファイル名生成取得
                            var filename = Engine.Razor.Run(FilenameKey, null, general);
                            resultStr.Append("\nファイル名：" + filename);

                            // 単独生成
                            var result = Engine.Razor.Run(BodyKey, null, general);

                            // 保存
                            await SimpleSaveString(Constants.OutputSourceDirectory, filename, result);
                            SaveResult = resultStr.ToString();
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
                            resultStr.Append("\nファイル名：" + filename);

                            // 生成
                            var result = Engine.Razor.Run(BodyKey, null, table);

                            // 保存
                            await SimpleSaveString(Constants.OutputSourceDirectory, filename, result);
                        }
                        SaveResult = resultStr.ToString();
                    }
                }
                catch (Exception e)
                {
                    SaveResult = e.Message;
                }
            }
        }

        /// <summary>
        /// 実行条件
        /// </summary>
        /// <returns></returns>
        protected override bool CheckRequiredFields()
        {
            return !string.IsNullOrWhiteSpace(Template);
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
        protected override async Task Activate()
        {
            Template = string.Empty;
            Connection = string.Empty;
            General = string.Empty;
            ConnectionList.Add(string.Empty);
            GeneralList.Add(string.Empty);

            // データ読み込み（ディレクトリのファイル一覧を取得）
            await SimpleFileList(Constants.TemplateDirectory, TemplateList);
            await SimpleFileList(Constants.MetaDataDirectory, ConnectionList);
            await SimpleFileList(Constants.GeneralInputDirectory, GeneralList);
        }
        #endregion
    }
}
