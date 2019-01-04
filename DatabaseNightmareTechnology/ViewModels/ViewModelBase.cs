using DatabaseNightmareTechnology.Models;
using Prism.Logging;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System;

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
        /// POCO
        /// </summary>
        protected ModelBase Model;

        /// <summary>
        /// 画面名
        /// </summary>
        protected string Name;

        /// <summary>
        /// ログ
        /// </summary>
        protected ILoggerFacade Log;

        /// <summary>
        /// 処理結果
        /// </summary>
        public ReactiveProperty<string> ActionResult { get; }

        #region Command
        /// <summary>
        /// 表示したときの処理
        /// </summary>
        public ReactiveCommand Activate { get; }
        #endregion

        public ViewModelBase(ModelBase model, string name, ILoggerFacade loggerFacade)
        {
            Model = model;
            Name = name;
            Log = loggerFacade;
            Log.Log($"{Model.GetType().Name}初期化", Category.Info, Priority.None);

            #region 値の連動設定
            ActionResult = Model.ToReactivePropertyAsSynchronized(
                m => m.ActionResult
                );
            #endregion

            #region コマンドの動作設定
            Activate = new ReactiveCommand();
            Activate.Subscribe(
                async d =>
                {
                    Log.Log($"{Name}を表示", Category.Info, Priority.None);
                    await Model.ActivateAsync();
                }
            );
            #endregion
        }
    }
}
