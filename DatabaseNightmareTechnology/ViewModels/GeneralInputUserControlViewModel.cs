using DatabaseNightmareTechnology.Models;
using GongSolutions.Wpf.DragDrop;
using Prism.Logging;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Linq;
using System.Windows;

namespace DatabaseNightmareTechnology.ViewModels
{
    /// <summary>
    /// 汎用入力
    /// </summary>
    class GeneralInputUserControlViewModel : ViewModelBase, IDropTarget
    {
        /// <summary>
        /// 受付するファイル
        /// </summary>
        private readonly string Extention = ".csv";

        #region ReactiveProperty

        /// <summary>
        /// データファイル名
        /// </summary>
        public ReactiveProperty<string> FileName { get; }

        /// <summary>
        /// 保存結果
        /// </summary>
        public ReactiveProperty<string> SaveResult { get; }

        /// <summary>
        /// ファイルリスト
        /// </summary>
        public ReadOnlyReactiveCollection<string> FileList { get; private set; }

        /// <summary>
        /// 汎用入力リスト
        /// </summary>
        public ReadOnlyReactiveCollection<DisplayValueData> Items { get; private set; }
        #endregion

        #region Command
        /// <summary>
        /// チェック＆保存ボタン処理
        /// </summary>
        public ReactiveCommand Save { get; }
        /// <summary>
        /// ファイル選択処理
        /// </summary>
        public ReactiveCommand SelectFile { get; }
        #endregion

        public GeneralInputUserControlViewModel(ILoggerFacade loggerFacade)
            : base(new GeneralInputUserControlModel(), "汎用入力", loggerFacade)
        {
            var model = Model as GeneralInputUserControlModel;

            #region 値の連動設定
            FileName = model.ToReactivePropertyAsSynchronized(
                m => m.FileName
                );
            SaveResult = model.ToReactivePropertyAsSynchronized(
                m => m.ActionResult
                );
            #endregion

            #region リストの連動設定
            FileList = model.FileList.ToReadOnlyReactiveCollection();
            Items = model.Items.ToReadOnlyReactiveCollection();
            #endregion

            #region コマンドの動作設定
            // 実行の許可/不許可を制御するIObservable<bool>
            // このValueがtrueかfalseかで制御される
            ReactiveProperty<bool> gate = new ReactiveProperty<bool>(true);

            Save = new ReactiveCommand(gate);
            Save.Subscribe(
                async d =>
                {
                    Log.Log($"ファイルを保存", Category.Info, Priority.None);
                    await model.Save();
                }
            );

            SelectFile = new ReactiveCommand(gate);
            SelectFile.Subscribe(
                async d =>
                {
                    if (d != null)
                    {
                        await model.SelectFile(d as string);
                    }
                }
            );
            #endregion
        }

        #region ドラッグ＆ドロップ
        public void DragOver(IDropInfo dropInfo)
        {
            var files = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = files.Any(fname => fname.EndsWith(Extention))
                ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            var files = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = files.Any(fname => fname.EndsWith(Extention))
                ? DragDropEffects.Copy : DragDropEffects.None;

            foreach (var file in files)
            {
                var model = Model as GeneralInputUserControlModel;
                model.DropFile(file);
            }
        }
        #endregion

    }
}
