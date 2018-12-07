using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    /// <summary>
    /// テンプレート編集
    /// </summary>
    class TemplateEditUserControlModel : BindableBase
    {
        #region Fields
        /// <summary>
        /// 設定ファイル
        /// </summary>
        private SaveData SaveData { get; set; }

        #region DmtTemplate
        /// <summary>
        /// テンプレートデータ
        /// </summary>
        private DmtTemplate DmtTemplate { get; set; }

        private string generateFileName;
        /// <summary>
        /// 生成ファイル名(Razor可)
        /// </summary>
        public string GenerateFileName
        {
            get
            {
                generateFileName = DmtTemplate.GenerateFileName;
                return generateFileName;
            }
            set
            {
                DmtTemplate.GenerateFileName = value;
                SetProperty(ref generateFileName, value);
            }
        }

        private string discription;
        /// <summary>
        /// 説明
        /// </summary>
        public string Discription
        {
            get
            {
                discription = DmtTemplate.Discription;
                return discription;
            }
            set
            {
                DmtTemplate.Discription = value;
                SetProperty(ref discription, value);
            }
        }

        private string templateBody;
        /// <summary>
        /// テンプレート本体
        /// </summary>
        public string TemplateBody
        {
            get
            {
                templateBody = DmtTemplate.TemplateBody;
                return templateBody;
            }
            set
            {
                DmtTemplate.TemplateBody = value;
                SetProperty(ref templateBody, value);
            }
        }

        #endregion

        private string title;
        /// <summary>
        /// テンプレートファイル名
        /// </summary>
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string saveResult;
        /// <summary>
        /// 保存結果
        /// </summary>
        public string SaveResult
        {
            get { return saveResult; }
            set { SetProperty(ref saveResult, value); }
        }

        /// <summary>
        /// ファイルリスト
        /// </summary>
        public ObservableCollection<string> FileList { get; } = new ObservableCollection<string>();

        #endregion

        #region initialize
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TemplateEditUserControlModel()
        {
            DmtTemplate = new DmtTemplate();
            SaveResult = "チェック結果";
        }
        #endregion

        #region ボタン
        /// <summary>
        /// 保存ボタン処理
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
            if (!string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Title))
            {
                // OKならDropboxかローカルに保存
                await DropboxHelper.MultiSaveAsync(SaveData.DataOutput, $"{Title}{Constants.Extension}", DmtTemplate, Constants.ApplicationDirectoryDropbox + Constants.TemplateDirectory, SaveData.LocalDirectory + Constants.TemplateDirectory, SaveData.AccessToken);
                SaveResult = "チェックOK、保存したぜ";

                if (!FileList.Contains($"{Title}{Constants.Extension}"))
                {
                    FileList.Add($"{Title}{Constants.Extension}");
                }
            }
            else
            {
                SaveResult = "…おい、タイトルを入力してくれよ保存できねぇだろ？";
            }
        }
        #endregion

        #region 選択されたときの処理
        /// <summary>
        /// ファイル選択されたときの処理
        /// </summary>
        /// <returns></returns>
        public async Task SelectFile(string value)
        {
            var data = await DropboxHelper.MultiLoadAsync<DmtTemplate>(SaveData.DataOutput, value, Constants.ApplicationDirectoryDropbox + Constants.TemplateDirectory, SaveData.LocalDirectory + Constants.TemplateDirectory, SaveData.AccessToken);

            GenerateFileName = data.GenerateFileName;
            Title = value.Substring(0, value.Length - Constants.Extension.Length);
            Discription = data.Discription;
            TemplateBody = data.TemplateBody;
        }
        #endregion

        #region 画面が表示されたときの処理
        /// <summary>
        /// 画面が表示されたときの処理
        /// </summary>
        /// <returns></returns>
        public async Task ActivateAsync()
        {
            FileList.Clear();
            GenerateFileName = string.Empty;
            Title = string.Empty;
            Discription = string.Empty;
            TemplateBody = string.Empty;

            // データがあるかチェック
            SaveData = await Json.Load<SaveData>(Constants.DataDirectory, Constants.DataFileName);

            if (SaveData == null)
            {
                // セーブデータがない場合
                SaveResult = "接続先の設定ができないぜ。先に設定画面の設定を完了させてくれよな！";
            }

            // 接続先データ読み込み（ディレクトリのファイル一覧を取得）
            await DropboxHelper.GetFileListAsync(FileList, SaveData.DataOutput, Constants.ApplicationDirectoryDropbox + Constants.TemplateDirectory, SaveData.LocalDirectory + Constants.TemplateDirectory, SaveData.AccessToken);

        }
        #endregion

    }
}
