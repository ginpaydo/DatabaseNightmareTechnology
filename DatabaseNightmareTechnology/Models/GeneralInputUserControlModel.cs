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

        #region ボタン
        /// <summary>
        /// 保存ボタン処理
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="value"></param>
        public void Delete(string value)
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
        }
        #endregion
    }
}
