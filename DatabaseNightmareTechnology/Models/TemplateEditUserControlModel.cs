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

        private string generateFileName;
        /// <summary>
        /// 生成ファイル名(Razor可)
        /// </summary>
        public string GenerateFileName
        {
            get { return generateFileName; }
            set { SetProperty(ref generateFileName, value); }
        }

        private string title;
        /// <summary>
        /// テンプレートファイル名
        /// </summary>
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string discription;
        /// <summary>
        /// 説明
        /// </summary>
        public string Discription
        {
            get { return discription; }
            set { SetProperty(ref discription, value); }
        }

        private string templateBody;
        /// <summary>
        /// テンプレート本体
        /// </summary>
        public string TemplateBody
        {
            get { return templateBody; }
            set { SetProperty(ref templateBody, value); }
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
