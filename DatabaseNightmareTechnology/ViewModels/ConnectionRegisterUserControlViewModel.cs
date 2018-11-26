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
    class ConnectionRegisterUserControlViewModel : ViewModelBase
    {
        /// <summary>
        /// POCO
        /// </summary>
        private ConnectionRegisterUserControlModel Model;

        #region ReactiveProperty
        /// <summary>
        /// データベースの種類
        /// </summary>
        public ReactiveProperty<DatabaseEngine> DatabaseEngine { get; }

        /// <summary>
        /// タイトル
        /// </summary>
        public ReactiveProperty<string> Title { get; }

        /// <summary>
        /// ホスト
        /// </summary>
        public ReactiveProperty<string> Host { get; }

        /// <summary>
        /// ポート番号
        /// </summary>
        public ReactiveProperty<string> Port { get; }

        /// <summary>
        /// データベース名
        /// </summary>
        public ReactiveProperty<string> DbName { get; }

        /// <summary>
        /// アカウント
        /// </summary>
        public ReactiveProperty<string> Account { get; }

        /// <summary>
        /// パスワード
        /// </summary>
        public ReactiveProperty<string> Password { get; }

        /// <summary>
        /// プレフィクス入力
        /// </summary>
        public ReactiveProperty<string> InputPrefix { get; }

        /// <summary>
        /// 共通項目入力
        /// </summary>
        public ReactiveProperty<string> InputCommonColumn { get; }

        /// <summary>
        /// プレフィクスリスト
        /// </summary>
        public ReadOnlyReactiveCollection<string> PrefixList { get; private set; }

        /// <summary>
        /// 共通項目リスト
        /// </summary>
        public ReadOnlyReactiveCollection<string> CommonColumnList { get; private set; }

        /// <summary>
        /// チェック結果
        /// </summary>
        public ReadOnlyReactiveProperty<string> CheckResult { get; }

        /// <summary>
        /// 接続文字列
        /// </summary>
        public ReadOnlyReactiveProperty<string> ConnectionString { get; }

        #endregion

        #region Command
        /// <summary>
        /// 表示したときの処理
        /// </summary>
        public ReactiveCommand Activate { get; }
        /// <summary>
        /// プレフィクス追加ボタン処理
        /// </summary>
        public ReactiveCommand AddPrefix { get; }
        /// <summary>
        /// プレフィクス削除ボタン処理
        /// </summary>
        public ReactiveCommand DeletePrefix { get; }
        /// <summary>
        /// 共通カラム追加ボタン処理
        /// </summary>
        public ReactiveCommand AddCommonColumn { get; }
        /// <summary>
        /// 共通カラム削除ボタン処理
        /// </summary>
        public ReactiveCommand DeleteCommonColumn { get; }
        /// <summary>
        /// チェック＆保存ボタン処理
        /// </summary>
        public ReactiveCommand CheckAndSave { get; }
        #endregion

        public ConnectionRegisterUserControlViewModel(ILoggerFacade loggerFacade)
            : base("ConnectionRegisterUserControlViewModel", loggerFacade)
        {
            // Modelクラスを初期化
            Model = new ConnectionRegisterUserControlModel();

            #region 値の連動設定
            DatabaseEngine = Model.ToReactivePropertyAsSynchronized(
                m => m.DatabaseEngine
                );
            Title = Model.ToReactivePropertyAsSynchronized(
                m => m.Title
                );
            Host = Model.ToReactivePropertyAsSynchronized(
                m => m.Host
                );
            Port = Model.ToReactivePropertyAsSynchronized(
                m => m.Port
                );
            DbName = Model.ToReactivePropertyAsSynchronized(
                m => m.DbName
                );
            Account = Model.ToReactivePropertyAsSynchronized(
                m => m.Account
                );
            Password = Model.ToReactivePropertyAsSynchronized(
                m => m.Password
                );
            InputPrefix = Model.ToReactivePropertyAsSynchronized(
                m => m.InputPrefix
                );
            InputCommonColumn = Model.ToReactivePropertyAsSynchronized(
                m => m.InputCommonColumn
                );
            CheckResult = Model.ToReactivePropertyAsSynchronized(
                m => m.CheckResult
                ).ToReadOnlyReactiveProperty();
            ConnectionString = Model.ToReactivePropertyAsSynchronized(
                m => m.ConnectionString
                ).ToReadOnlyReactiveProperty();
            #endregion

            #region リストの連動設定
            PrefixList = Model.PrefixList.ToReadOnlyReactiveCollection();

            CommonColumnList = Model.CommonColumnList.ToReadOnlyReactiveCollection();
            #endregion

            #region コマンドの動作設定
            // 実行の許可/不許可を制御するIObservable<bool>
            // このValueがtrueかfalseかで制御される
            ReactiveProperty<bool> gate = new ReactiveProperty<bool>(true);

            CheckAndSave = new ReactiveCommand(gate);
            CheckAndSave.Subscribe(
                async d =>
                {
                    Log.Log($"接続先ファイルを保存", Category.Info, Priority.None);
                    await Model.CheckAndSave();
                }
            );

            AddPrefix = new ReactiveCommand(gate);
            AddPrefix.Subscribe(
                d =>
                {
                    Model.AddPrefix();
                }
            );

            DeletePrefix = new ReactiveCommand(gate);
            DeletePrefix.Subscribe(
                d =>
                {
                    Model.DeletePrefix(d as string);
                }
            );

            AddCommonColumn = new ReactiveCommand(gate);
            AddCommonColumn.Subscribe(
                d =>
                {
                    Model.AddCommonColumn();
                }
            );

            DeleteCommonColumn = new ReactiveCommand(gate);
            DeleteCommonColumn.Subscribe(
                d =>
                {
                    Model.DeleteCommonColumn(d as string);
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
