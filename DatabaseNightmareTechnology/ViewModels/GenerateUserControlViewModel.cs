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
    /// <summary>
    /// メタデータ生成
    /// </summary>
    class GenerateUserControlViewModel : ViewModelBase
    {
        /// <summary>
        /// POCO
        /// </summary>
        private GenerateUserControlModel Model;

        #region ReactiveProperty
        /// <summary>
        /// 接続先リスト
        /// </summary>
        public ReadOnlyReactiveCollection<string> DataList { get; private set; }

        /// <summary>
        /// 生成ファイル名
        /// </summary>
        public ReactiveProperty<string> FileName { get; }

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
        /// 生成ボタン
        /// </summary>
        public ReactiveCommand Generate { get; }
        #endregion

        public GenerateUserControlViewModel(ILoggerFacade loggerFacade)
            : base("GenerateUserControlViewModel", loggerFacade)
        {
            // Modelクラスを初期化
            Model = new GenerateUserControlModel();

            #region 値の連動設定
            FileName = Model.ToReactivePropertyAsSynchronized(
                m => m.FileName
                );
            CheckResult = Model.ToReactivePropertyAsSynchronized(
                m => m.CheckResult
                ).ToReadOnlyReactiveProperty();
            #endregion

            #region リストの連動設定
            DataList = Model.DataList.ToReadOnlyReactiveCollection();
            #endregion

            #region コマンドの動作設定
            // 実行の許可/不許可を制御するIObservable<bool>
            // このValueがtrueかfalseかで制御される
            ReactiveProperty<bool> gate = new ReactiveProperty<bool>(true);

            Generate = new ReactiveCommand(gate);
            Generate.Subscribe(
                async d =>
                {
                    Log.Log($"メタデータファイルを保存：{FileName}{Constants.Extension}", Category.Info, Priority.None);
                    await Model.Generate(d as string);
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
