using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    class GeneralInputUserControlModel : BindableBase
    {
        #region Fields
        /// <summary>
        /// 設定ファイル
        /// </summary>
        private SaveData SaveData { get; set; }

        private string fileName;
        /// <summary>
        /// 生成ファイル名(Razor可)
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set { SetProperty(ref fileName, value); }
        }

        private string saveResult;
        /// <summary>
        /// テンプレートファイル名
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

        /// <summary>
        /// パラメータと値リスト
        /// TODO:独自クラスになるはず
        /// </summary>
        public ObservableCollection<string> Items { get; } = new ObservableCollection<string>();
        #endregion

        #region initialize
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GeneralInputUserControlModel()
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

        #region 選択されたときの処理
        /// <summary>
        /// ファイル選択されたときの処理
        /// </summary>
        /// <returns></returns>
        public async Task SelectFile(string value)
        {
            //Body = await DropboxHelper.MultiLoadStringAsync(SaveData.DataOutput, value, Constants.ApplicationDirectoryDropbox + Directory, SaveData.LocalDirectory + Directory, SaveData.AccessToken);
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
            await DropboxHelper.GetFileListAsync(FileList, SaveData.DataOutput, Constants.ApplicationDirectoryDropbox + Constants.GeneralInputDirectory, SaveData.LocalDirectory + Constants.GeneralInputDirectory, SaveData.AccessToken);

        }
        #endregion
    }
}
