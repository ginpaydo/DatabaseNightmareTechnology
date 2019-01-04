using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    /// <summary>
    /// MainWindowのModel
    /// </summary>
    class MainWindowModel : ModelBase
    {
        private string _title = Constants.Title;
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        /// <summary>
        /// セーブデータ使用画面
        /// </summary>
        protected override bool UseSaveData
        {
            get { return false; }
        }

        protected override async Task Activate()
        {
            await Task.Delay(1);
        }

        protected override bool CheckRequiredFields()
        {
            return true;
        }
    }
}
