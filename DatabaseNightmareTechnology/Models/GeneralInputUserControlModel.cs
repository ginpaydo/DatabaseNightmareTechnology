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
    /// 汎用入力
    /// </summary>
    class GeneralInputUserControlModel : BindableBase
    {
        #region Fields
        /// <summary>
        /// 設定ファイル
        /// </summary>
        private SaveData SaveData { get; set; }

        private string fileName;
        /// <summary>
        /// ファイル名
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
        /// </summary>
        public ObservableCollection<NameValueData> Items { get; } = new ObservableCollection<NameValueData>();
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
            if (string.IsNullOrWhiteSpace(FileName))
            {
                FileName = "なんか書けや";
            }
            else
            {
                // 保存データ
                var data = new GeneralInput(Items.ToList());

                // OKならDropboxかローカルに保存
                await DropboxHelper.MultiSaveAsync(SaveData.DataOutput, $"{FileName}{Constants.Extension}", data, Constants.ApplicationDirectoryDropbox + Constants.GeneralInputDirectory, SaveData.LocalDirectory + Constants.GeneralInputDirectory, SaveData.AccessToken);
                SaveResult = "チェックOK、保存したぜ";

                if (!FileList.Contains($"{FileName}{Constants.Extension}"))
                {
                    FileList.Add($"{FileName}{Constants.Extension}");
                }
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
            Items.Clear();
            var general = await DropboxHelper.MultiLoadAsync<GeneralInput>(SaveData.DataOutput, value, Constants.ApplicationDirectoryDropbox + Constants.GeneralInputDirectory, SaveData.LocalDirectory + Constants.GeneralInputDirectory, SaveData.AccessToken);
            var raw = new RawGeneralInput(general);
            foreach (var item in raw.Datas)
            {
                Items.Add(item);
            }
            FileName = value.Substring(0, value.Length - Constants.Extension.Length);
        }
        #endregion

        #region ドロップされたときの処理
        /// <summary>
        /// ドロップされたときの処理
        /// </summary>
        public void DropFile(string value)
        {
            var raw = new RawGeneralInput(value);
            Items.Clear();
            foreach (var item in raw.Datas)
            {
                Items.Add(item);
            }
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
            Items.Clear();
            FileName = string.Empty;

            // データがあるかチェック
            SaveData = await Json.Load<SaveData>(Constants.DataDirectory, Constants.DataFileName);

            if (SaveData == null)
            {
                // セーブデータがない場合
                SaveResult = "接続先の設定ができないぜ。先に設定画面の設定を完了させてくれよな！";
            }

            // データ読み込み（ディレクトリのファイル一覧を取得）
            await DropboxHelper.GetFileListAsync(FileList, SaveData.DataOutput, Constants.ApplicationDirectoryDropbox + Constants.GeneralInputDirectory, SaveData.LocalDirectory + Constants.GeneralInputDirectory, SaveData.AccessToken);

        }
        #endregion
    }
}
