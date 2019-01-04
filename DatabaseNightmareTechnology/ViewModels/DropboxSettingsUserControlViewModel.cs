using DatabaseNightmareTechnology.Models;
using Prism.Logging;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;

namespace DatabaseNightmareTechnology.ViewModels
{
    /// <summary>
    /// Dropbox設定
    /// </summary>
    class DropboxSettingsUserControlViewModel : ViewModelBase
    {
        #region ReactiveProperty
        /// <summary>
        /// トークン
        /// </summary>
        public ReactiveProperty<string> AccessToken { get; }

        /// <summary>
        /// ローカル保存先
        /// </summary>
        public ReactiveProperty<string> LocalDirectory { get; }

        /// <summary>
        /// ラジオボタンの値
        /// </summary>
        public ReactiveProperty<DataOutput> DataOutputValue { get; }

        /// <summary>
        /// ユーザ名
        /// </summary>
        public ReadOnlyReactiveProperty<string> UserName { get; }

        /// <summary>
        /// メールアドレス
        /// </summary>
        public ReadOnlyReactiveProperty<string> Email { get; }

        #endregion

        #region Command
        /// <summary>
        /// ボタン処理
        /// </summary>
        public ReactiveCommand CheckAndSave { get; }
        #endregion

        public DropboxSettingsUserControlViewModel(ILoggerFacade loggerFacade)
            : base(new DropboxSettingsUserControlModel(), "Dropbox設定", loggerFacade)
        {
            var model = Model as DropboxSettingsUserControlModel;

            #region 値の連動設定
            AccessToken = model.ToReactivePropertyAsSynchronized(
                m => m.AccessToken
                );
            LocalDirectory = model.ToReactivePropertyAsSynchronized(
                m => m.LocalDirectory
                );
            DataOutputValue = model.ToReactivePropertyAsSynchronized(
                m => m.DataOutput,
                x => x,
                s => s
                );
            UserName = model.ToReactivePropertyAsSynchronized(
                m => m.UserName
                ).ToReadOnlyReactiveProperty();
            Email = model.ToReactivePropertyAsSynchronized(
                m => m.Email
                ).ToReadOnlyReactiveProperty();
            #endregion

            #region コマンドの動作設定
            // 実行の許可/不許可を制御するIObservable<bool>
            // このValueがtrueかfalseかで制御される
            ReactiveProperty<bool> gate = new ReactiveProperty<bool>(true);

            CheckAndSave = new ReactiveCommand(gate);
            CheckAndSave.Subscribe(
                async d =>
                {
                    Log.Log($"設定ファイルを保存", Category.Info, Priority.None);
                    await model.CheckAndSave();
                }
            );
            #endregion
        }
    }
}
