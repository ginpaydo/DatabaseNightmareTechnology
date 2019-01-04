using DatabaseNightmareTechnology.Models;
using Prism.Logging;
using Reactive.Bindings;
using System;

namespace DatabaseNightmareTechnology.ViewModels
{
    /// <summary>
    /// ソース生成
    /// </summary>
    class SourceGenerateUserControlViewModel : ViewModelBase
    {
        #region ReactiveProperty
        /// <summary>
        /// テンプレートリスト
        /// </summary>
        public ReadOnlyReactiveCollection<string> TemplateList { get; private set; }

        /// <summary>
        /// データベース情報
        /// 選択しない場合、汎用入力による単発生成
        /// </summary>
        public ReadOnlyReactiveCollection<string> ConnectionList { get; private set; }

        /// <summary>
        /// 汎用データ（任意）
        /// リストデータは、名前に共通のプレフィクスを付ける。
        /// リスト名#データ名('data#Field1')
        /// </summary>
        public ReadOnlyReactiveCollection<string> GeneralList { get; private set; }
        #endregion

        #region Command
        /// <summary>
        /// 選択処理
        /// </summary>
        public ReactiveCommand SelectTemplate { get; }

        /// <summary>
        /// 選択処理
        /// </summary>
        public ReactiveCommand SelectConnection { get; }

        /// <summary>
        /// 選択処理
        /// </summary>
        public ReactiveCommand SelectGeneral { get; }

        /// <summary>
        /// 生成ボタン処理
        /// </summary>
        public ReactiveCommand Generate { get; }
        #endregion

        public SourceGenerateUserControlViewModel(ILoggerFacade loggerFacade)
            : base(new SourceGenerateUserControlModel(), "ソース生成", loggerFacade)
        {
            var model = Model as SourceGenerateUserControlModel;

            #region リストの連動設定
            TemplateList = model.TemplateList.ToReadOnlyReactiveCollection();
            ConnectionList = model.ConnectionList.ToReadOnlyReactiveCollection();
            GeneralList = model.GeneralList.ToReadOnlyReactiveCollection();
            #endregion

            #region コマンドの動作設定
            // 実行の許可/不許可を制御するIObservable<bool>
            // このValueがtrueかfalseかで制御される
            ReactiveProperty<bool> gate = new ReactiveProperty<bool>(true);

            SelectTemplate = new ReactiveCommand(gate);
            SelectTemplate.Subscribe(
                d =>
                {
                    model.SelectTemplate(d as string);
                }
            );

            SelectConnection = new ReactiveCommand(gate);
            SelectConnection.Subscribe(
                d =>
                {
                    model.SelectConnection(d as string);
                }
            );

            SelectGeneral = new ReactiveCommand(gate);
            SelectGeneral.Subscribe(
                d =>
                {
                    model.SelectGeneral(d as string);
                }
            );

            Generate = new ReactiveCommand(gate);
            Generate.Subscribe(
                async d =>
                {
                    Log.Log($"ファイルを保存", Category.Info, Priority.None);
                    await model.Generate();
                }
            );
            #endregion
        }
    }
}
