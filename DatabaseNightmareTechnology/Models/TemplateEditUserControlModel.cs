using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    /// <summary>
    /// テンプレート編集
    /// </summary>
    class TemplateEditUserControlModel : ModelBase
    {
        #region Fields

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

        #region initialize
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TemplateEditUserControlModel()
        {
            DmtTemplate = new DmtTemplate();
        }
        #endregion

        #region ボタン
        /// <summary>
        /// 保存ボタン処理
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
            await SimpleSave(Constants.TemplateDirectory, $"{Title}{Constants.Extension}", DmtTemplate, FileList);
        }

        protected override bool CheckRequiredFields()
        {
            return !string.IsNullOrWhiteSpace(Title);
        }
        #endregion

        #region 選択されたときの処理

        /// <summary>
        /// ファイル選択されたときの処理
        /// </summary>
        /// <param name="value">ファイル名</param>
        /// <returns></returns>
        public async Task SelectFile(string value)
        {
            var data = await SimpleLoad<DmtTemplate>(Constants.TemplateDirectory, value);

            GenerateFileName = data.GenerateFileName;
            Title = RemoveExtension(value);
            Discription = data.Discription;
            TemplateBody = data.TemplateBody;
        }
        #endregion

        #region 画面が表示されたときの処理
        /// <summary>
        /// 画面が表示されたときの処理
        /// </summary>
        /// <returns></returns>
        protected override async Task Activate()
        {
            GenerateFileName = string.Empty;
            Title = string.Empty;
            Discription = string.Empty;
            TemplateBody = string.Empty;

            // 接続先データ読み込み（ディレクトリのファイル一覧を取得）
            await SimpleFileList(Constants.TemplateDirectory, FileList);
        }
        #endregion

    }
}
