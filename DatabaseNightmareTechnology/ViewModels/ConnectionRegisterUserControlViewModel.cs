using DatabaseNightmareTechnology.Models;
using Prism.Logging;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;

namespace DatabaseNightmareTechnology.ViewModels
{
    /// <summary>
    /// 接続先登録
    /// </summary>
    class ConnectionRegisterUserControlViewModel : ViewModelBase
    {
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
        /// 接続文字列
        /// </summary>
        public ReadOnlyReactiveProperty<string> ConnectionString { get; }

        #endregion

        #region Command
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
            : base(new ConnectionRegisterUserControlModel(), "接続先登録", loggerFacade)
        {
            var model = Model as ConnectionRegisterUserControlModel;

            #region 値の連動設定
            DatabaseEngine = model.ToReactivePropertyAsSynchronized(
                m => m.DatabaseEngine
                );
            Title = model.ToReactivePropertyAsSynchronized(
                m => m.Title
                );
            Host = model.ToReactivePropertyAsSynchronized(
                m => m.Host
                );
            Port = model.ToReactivePropertyAsSynchronized(
                m => m.Port
                );
            DbName = model.ToReactivePropertyAsSynchronized(
                m => m.DbName
                );
            Account = model.ToReactivePropertyAsSynchronized(
                m => m.Account
                );
            Password = model.ToReactivePropertyAsSynchronized(
                m => m.Password
                );
            InputPrefix = model.ToReactivePropertyAsSynchronized(
                m => m.InputPrefix
                );
            InputCommonColumn = model.ToReactivePropertyAsSynchronized(
                m => m.InputCommonColumn
                );
            ConnectionString = model.ToReactivePropertyAsSynchronized(
                m => m.ConnectionString
                ).ToReadOnlyReactiveProperty();
            #endregion

            #region リストの連動設定
            PrefixList = model.PrefixList.ToReadOnlyReactiveCollection();

            CommonColumnList = model.CommonColumnList.ToReadOnlyReactiveCollection();
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
                    await model.CheckAndSave();
                }
            );

            AddPrefix = new ReactiveCommand(gate);
            AddPrefix.Subscribe(
                d =>
                {
                    model.AddPrefix();
                }
            );

            DeletePrefix = new ReactiveCommand(gate);
            DeletePrefix.Subscribe(
                d =>
                {
                    model.DeletePrefix(d as string);
                }
            );

            AddCommonColumn = new ReactiveCommand(gate);
            AddCommonColumn.Subscribe(
                d =>
                {
                    model.AddCommonColumn();
                }
            );

            DeleteCommonColumn = new ReactiveCommand(gate);
            DeleteCommonColumn.Subscribe(
                d =>
                {
                    model.DeleteCommonColumn(d as string);
                }
            );
            #endregion
        }
    }
}
