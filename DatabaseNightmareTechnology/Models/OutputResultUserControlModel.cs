using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    public class OutputResultUserControlModel : ModelBase
    {
        #region Fields
        /// <summary>
        /// 選択中ディレクトリの控え
        /// </summary>
        private string Directory;
        /// <summary>
        /// 選択中ファイルの控え
        /// </summary>
        private string Filename;

        private string body;
        /// <summary>
        /// ファイル内容
        /// </summary>
        public string Body
        {
            get { return body; }
            set { SetProperty(ref body, value); }
        }

        /// <summary>
        /// ディレクトリリスト
        /// </summary>
        public ObservableCollection<string> DirectoryList { get; } = new ObservableCollection<string>();

        /// <summary>
        /// ファイルリスト
        /// </summary>
        public ObservableCollection<string> FileList { get; } = new ObservableCollection<string>();

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
        /// 削除
        /// </summary>
        public async Task DeleteAsync()
        {
            await SimpleDelete(Directory, Filename, FileList);
            Body = string.Empty;
        }

        /// <summary>
        /// 実行条件
        /// </summary>
        /// <returns></returns>
        protected override bool CheckRequiredFields()
        {
            return !string.IsNullOrWhiteSpace(Directory) && !string.IsNullOrWhiteSpace(Filename);
        }
        #endregion

        #region 選択されたときの処理
        /// <summary>
        /// ディレクトリ選択されたときの処理
        /// </summary>
        /// <returns></returns>
        public async Task SelectDirectory(string value)
        {
            ActionResult = MessageConstants.ActionResult;
            Directory = value;

            // 接続先データ読み込み（ディレクトリのファイル一覧を取得）
            await SimpleFileList(Directory, FileList);
        }

        /// <summary>
        /// ファイル選択されたときの処理
        /// </summary>
        /// <returns></returns>
        public async Task SelectFile(string value)
        {
            Filename = value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                Body = await SimpleLoadString(Directory, Filename);
            }
        }
        #endregion

        #region 画面が表示されたときの処理

        /// <summary>
        /// 画面が表示されたときの処理
        /// </summary>
        /// <returns></returns>
        protected override async Task Activate()
        {
            FileList.Clear();
            DirectoryList.Clear();
            Body = string.Empty;
            DirectoryList.Add(Constants.ConnectionDirectory);
            DirectoryList.Add(Constants.MetaDataDirectory);
            DirectoryList.Add(Constants.TemplateDirectory);
            DirectoryList.Add(Constants.GeneralInputDirectory);
            DirectoryList.Add(Constants.OutputSourceDirectory);
            await Task.Delay(1);
        }
        #endregion
    }
}
