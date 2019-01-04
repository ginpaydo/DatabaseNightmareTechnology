using DatabaseNightmareTechnology.Models;
using Prism.Logging;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;

namespace DatabaseNightmareTechnology.ViewModels
{
    /// <summary>
    /// メタデータ生成
    /// </summary>
    class GenerateUserControlViewModel : ViewModelBase
    {
        #region ReactiveProperty
        /// <summary>
        /// 接続先リスト
        /// </summary>
        public ReadOnlyReactiveCollection<string> DataList { get; private set; }

        /// <summary>
        /// 生成ファイル名
        /// </summary>
        public ReactiveProperty<string> FileName { get; }
        #endregion

        #region Command
        /// <summary>
        /// ファイル選択したときの処理
        /// </summary>
        public ReactiveCommand SelectFile { get; }
        /// <summary>
        /// 生成ボタン
        /// </summary>
        public ReactiveCommand Generate { get; }
        #endregion

        public GenerateUserControlViewModel(ILoggerFacade loggerFacade)
            : base(new GenerateUserControlModel(), "メタデータ生成", loggerFacade)
        {
            var model = Model as GenerateUserControlModel;

            #region 値の連動設定
            FileName = model.ToReactivePropertyAsSynchronized(
                m => m.FileName
                );
            #endregion

            #region リストの連動設定
            DataList = model.DataList.ToReadOnlyReactiveCollection();
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
                    await model.Generate();
                }
            );

            SelectFile = new ReactiveCommand(gate);
            SelectFile.Subscribe(
                d =>
                {
                    model.SelectFile(d as string);
                }
            );
            #endregion

        }
    }
}
