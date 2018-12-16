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
    class GeneralInputUserControlModel : ModelBase
    {
        #region Fields

        private string fileName;
        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set { SetProperty(ref fileName, value); }
        }

        /// <summary>
        /// ファイルリスト
        /// </summary>
        public ObservableCollection<string> FileList { get; } = new ObservableCollection<string>();

        /// <summary>
        /// パラメータと値リスト
        /// </summary>
        public ObservableCollection<DisplayValueData> Items { get; } = new ObservableCollection<DisplayValueData>();

        /// <summary>
        /// セーブデータ使用画面
        /// </summary>
        protected override bool UseSaveData
        {
            get { return true; }
        }
        #endregion

        #region initialize
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GeneralInputUserControlModel()
        {
            ActionResult = "チェック結果";
        }
        #endregion

        #region ボタン
        /// <summary>
        /// 保存ボタン処理
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
            await SimpleSave(Constants.GeneralInputDirectory, $"{FileName}{Constants.Extension}", FileList);
        }

        /// <summary>
        /// 実行条件
        /// </summary>
        /// <returns></returns>
        protected override bool CheckRequiredFields()
        {
            return !string.IsNullOrWhiteSpace(FileName);
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
            var general = await SimpleLoad<GeneralInput>(Constants.GeneralInputDirectory, value);
            var raw = new RawGeneralInput(general);
            foreach (var item in raw.Datas)
            {
                Items.Add(item);
            }
            FileName = RemoveExtension(value);
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

        protected override async Task Activate()
        {
            Items.Clear();
            FileName = string.Empty;

            // データ読み込み（ディレクトリのファイル一覧を取得）
            await SimpleFileList(Constants.GeneralInputDirectory, FileList);
        }
        #endregion
    }
}
