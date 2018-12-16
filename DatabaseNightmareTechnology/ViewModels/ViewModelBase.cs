using Prism.Logging;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Attributes;

namespace DatabaseNightmareTechnology.ViewModels
{
    /// <summary>
    /// ViewModelクラスの共通親クラス
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region PropertyChanged
#pragma warning disable 0067
        /// <summary>
        /// INotifyPropertyChangedを継承してPropertyChangedを実装しないとメモリリークする
        /// 警告が出るので無視設定する
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
        #endregion

        /// <summary>
        /// 画面名
        /// </summary>
        protected string Name;

        /// <summary>
        /// ログ
        /// </summary>
        protected ILoggerFacade Log;

        public ViewModelBase(string name, ILoggerFacade loggerFacade)
        {
            loggerFacade.Log("{name}初期化", Category.Info, Priority.None);
            Name = name;
            Log = loggerFacade;
        }
    }
}
