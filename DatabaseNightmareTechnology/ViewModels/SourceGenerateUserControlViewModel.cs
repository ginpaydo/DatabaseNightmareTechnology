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
    class SourceGenerateUserControlViewModel : ViewModelBase
    {
        /// <summary>
        /// POCO
        /// </summary>
        private SourceGenerateUserControlModel Model;

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

        /// <summary>
        /// 保存結果
        /// </summary>
        public ReactiveProperty<string> SaveResult { get; }
        #endregion

        #region Command
        /// <summary>
        /// 表示したときの処理
        /// </summary>
        public ReactiveCommand Activate { get; }

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
            : base("SourceGenerateUserControlViewModel", loggerFacade)
        {
            // Modelクラスを初期化
            Model = new SourceGenerateUserControlModel();

            #region 値の連動設定
            SaveResult = Model.ToReactivePropertyAsSynchronized(
                m => m.ActionResult
                );
            #endregion

            #region リストの連動設定
            TemplateList = Model.TemplateList.ToReadOnlyReactiveCollection();
            ConnectionList = Model.ConnectionList.ToReadOnlyReactiveCollection();
            GeneralList = Model.GeneralList.ToReadOnlyReactiveCollection();
            #endregion

            #region コマンドの動作設定
            // 実行の許可/不許可を制御するIObservable<bool>
            // このValueがtrueかfalseかで制御される
            ReactiveProperty<bool> gate = new ReactiveProperty<bool>(true);

            SelectTemplate = new ReactiveCommand(gate);
            SelectTemplate.Subscribe(
                d =>
                {
                    Model.SelectTemplate(d as string);
                }
            );

            SelectConnection = new ReactiveCommand(gate);
            SelectConnection.Subscribe(
                d =>
                {
                    Model.SelectConnection(d as string);
                }
            );

            SelectGeneral = new ReactiveCommand(gate);
            SelectGeneral.Subscribe(
                d =>
                {
                    Model.SelectGeneral(d as string);
                }
            );

            Generate = new ReactiveCommand(gate);
            Generate.Subscribe(
                async d =>
                {
                    Log.Log($"ファイルを保存", Category.Info, Priority.None);
                    await Model.Generate();
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
