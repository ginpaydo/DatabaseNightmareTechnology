using Prism.Mvvm;

namespace DatabaseNightmareTechnology.Models
{
    /// <summary>
    /// MainWindowのModel
    /// </summary>
    class MainWindowModel : BindableBase
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
    }
}
