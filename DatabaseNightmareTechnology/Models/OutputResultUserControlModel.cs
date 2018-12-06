using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    class OutputResultUserControlModel : BindableBase
    {

        private readonly string Message = "削除結果";

        #region Fields
        /// <summary>
        /// 設定ファイル
        /// </summary>
        private SaveData SaveData { get; set; }

        /// <summary>
        /// 選択中ディレクトリの控え
        /// </summary>
        private string Directory;

        private string body;
        /// <summary>
        /// ファイル内容
        /// </summary>
        public string Body
        {
            get { return body; }
            set { SetProperty(ref body, value); }
        }

        private string deleteResult;
        /// <summary>
        /// 削除結果
        /// </summary>
        public string DeleteResult
        {
            get { return deleteResult; }
            set { SetProperty(ref deleteResult, value); }
        }

        /// <summary>
        /// ディレクトリリスト
        /// </summary>
        public ObservableCollection<string> DirectoryList { get; } = new ObservableCollection<string>();

        /// <summary>
        /// ファイルリスト
        /// </summary>
        public ObservableCollection<string> FileList { get; } = new ObservableCollection<string>();

        #endregion

        #region initialize
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OutputResultUserControlModel()
        {
            DeleteResult = Message;

        }
        #endregion

        #region ボタン

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="value"></param>
        public async Task DeleteAsync(string value)
        {
            await DropboxHelper.MultiDeleteFileAsync(SaveData.DataOutput, value, Constants.TrushDirectory, Directory, Constants.ApplicationDirectoryDropbox, SaveData.LocalDirectory, SaveData.AccessToken);
            DeleteResult = "削除しました";
            Body = string.Empty;
            FileList.Remove(value);
        }
        #endregion

        #region 選択されたときの処理
        /// <summary>
        /// ディレクトリ選択されたときの処理
        /// </summary>
        /// <returns></returns>
        public async Task SelectDirectory(string value)
        {
            DeleteResult = Message;
            FileList.Clear();
            Directory = value;

            // データがあるかチェック
            SaveData = await Json.Load<SaveData>(Constants.DataDirectory, Constants.DataFileName);

            if (SaveData == null)
            {
                // セーブデータがない場合
                DeleteResult = "接続先の設定ができないぜ。先に設定画面の設定を完了させてくれよな！";
            }

            // 接続先データ読み込み（ディレクトリのファイル一覧を取得）
            await DropboxHelper.GetFileListAsync(FileList, SaveData.DataOutput, Constants.ApplicationDirectoryDropbox + value, SaveData.LocalDirectory + value, SaveData.AccessToken);
        }

        /// <summary>
        /// ファイル選択されたときの処理
        /// </summary>
        /// <returns></returns>
        public async Task SelectFile(string value)
        {
            Body = await DropboxHelper.MultiLoadStringAsync(SaveData.DataOutput, value, Constants.ApplicationDirectoryDropbox + Directory, SaveData.LocalDirectory + Directory, SaveData.AccessToken);
        }
        #endregion

        #region 画面が表示されたときの処理
        /// <summary>
        /// 画面が表示されたときの処理
        /// </summary>
        /// <returns></returns>
        public void Activate()
        {
            FileList.Clear();
            DirectoryList.Clear();
            Body = string.Empty;
            DirectoryList.Add(Constants.ConnectionDirectory);
            DirectoryList.Add(Constants.MetaDataDirectory);
            DirectoryList.Add(Constants.TemplateDirectory);
            DirectoryList.Add(Constants.GeneralInputDirectory);
            DirectoryList.Add(Constants.OutputSourceDirectory);
        }
        #endregion
    }
}
