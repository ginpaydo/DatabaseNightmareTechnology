using DatabaseNightmareTechnology.Models;
using Prism.Logging;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.ViewModels
{
    class DropboxSettingsUserControlViewModel : ViewModelBase
    {
        /// <summary>
        /// POCO
        /// </summary>
        private DropboxSettingsUserControlModel Model;

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

        /// <summary>
        /// チェック結果
        /// </summary>
        public ReadOnlyReactiveProperty<string> CheckResult { get; }
        #endregion

        #region Command
        /// <summary>
        /// 表示したときの処理
        /// </summary>
        public ReactiveCommand Activate { get; }
        /// <summary>
        /// ボタン処理
        /// </summary>
        public ReactiveCommand CheckAndSave { get; }
        #endregion

        public DropboxSettingsUserControlViewModel(ILoggerFacade loggerFacade)
            : base("DropboxSettingsUserControlViewModel", loggerFacade)
        {
            // Modelクラスを初期化
            Model = new DropboxSettingsUserControlModel();

            #region 値の連動設定
            AccessToken = Model.ToReactivePropertyAsSynchronized(
                m => m.AccessToken
                );
            LocalDirectory = Model.ToReactivePropertyAsSynchronized(
                m => m.LocalDirectory
                );
            DataOutputValue = Model.ToReactivePropertyAsSynchronized(
                m => m.DataOutput,
                x => x,
                s => s
                );
            UserName = Model.ToReactivePropertyAsSynchronized(
                m => m.UserName
                ).ToReadOnlyReactiveProperty();
            Email = Model.ToReactivePropertyAsSynchronized(
                m => m.Email
                ).ToReadOnlyReactiveProperty();
            CheckResult = Model.ToReactivePropertyAsSynchronized(
                m => m.SaveResult
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
                    await Model.CheckAndSave();
                }
            );

            Activate = new ReactiveCommand(gate);
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
