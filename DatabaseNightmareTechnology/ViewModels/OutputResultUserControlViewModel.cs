using DatabaseNightmareTechnology.Models;
using Prism.Logging;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;

namespace DatabaseNightmareTechnology.ViewModels
{
    /// <summary>
    /// 出力結果
    /// </summary>
    class OutputResultUserControlViewModel : ViewModelBase
    {
        #region ReactiveProperty

        /// <summary>
        /// ディレクトリ
        /// </summary>
        public ReadOnlyReactiveCollection<string> DirectoryList { get; private set; }

        /// <summary>
        /// ファイル
        /// </summary>
        public ReadOnlyReactiveCollection<string> FileList { get; private set; }

        /// <summary>
        /// ファイル内容
        /// </summary>
        public ReactiveProperty<string> Body { get; }

        #endregion

        #region Command
        /// <summary>
        /// 削除ボタン処理
        /// </summary>
        public ReactiveCommand Delete { get; }
        /// <summary>
        /// ディレクトリ選択したときの処理
        /// </summary>
        public ReactiveCommand SelectDirectory { get; }
        /// <summary>
        /// ファイル選択したときの処理
        /// </summary>
        public ReactiveCommand SelectFile { get; }
        #endregion


        public OutputResultUserControlViewModel(ILoggerFacade loggerFacade)
            : base(new OutputResultUserControlModel(), "出力結果", loggerFacade)
        {
            var model = Model as OutputResultUserControlModel;

            #region 値の連動設定
            Body = model.ToReactivePropertyAsSynchronized(
                m => m.Body
                );
            #endregion

            #region リストの連動設定
            DirectoryList = model.DirectoryList.ToReadOnlyReactiveCollection();
            FileList = model.FileList.ToReadOnlyReactiveCollection();
            #endregion

            #region コマンドの動作設定
            // 実行の許可/不許可を制御するIObservable<bool>
            // このValueがtrueかfalseかで制御される
            ReactiveProperty<bool> gate = new ReactiveProperty<bool>(true);

            Delete = new ReactiveCommand(gate);
            Delete.Subscribe(
                async d =>
                {
                    await model.DeleteAsync();
                }
            );

            SelectDirectory = new ReactiveCommand(gate);
            SelectDirectory.Subscribe(
                async d =>
                {
                    await model.SelectDirectory(d as string);
                }
            );

            SelectFile = new ReactiveCommand(gate);
            SelectFile.Subscribe(
                async d =>
                {
                    await model.SelectFile(d as string);
                }
            );
            #endregion
        }
    }
}
