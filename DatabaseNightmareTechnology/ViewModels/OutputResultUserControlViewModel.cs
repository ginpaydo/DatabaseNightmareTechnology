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
    class OutputResultUserControlViewModel : ViewModelBase
    {
        /// <summary>
        /// POCO
        /// </summary>
        private OutputResultUserControlModel Model;

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

        /// <summary>
        /// ファイル内容
        /// </summary>
        public ReactiveProperty<string> DeleteResult { get; }

        #endregion

        #region Command
        /// <summary>
        /// 表示したときの処理
        /// </summary>
        public ReactiveCommand Activate { get; }
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
            : base("OutputResultUserControlViewModel", loggerFacade)
        {
            // Modelクラスを初期化
            Model = new OutputResultUserControlModel();

            #region 値の連動設定
            Body = Model.ToReactivePropertyAsSynchronized(
                m => m.Body
                );
            DeleteResult = Model.ToReactivePropertyAsSynchronized(
                m => m.ActionResult
                );
            #endregion

            #region リストの連動設定
            DirectoryList = Model.DirectoryList.ToReadOnlyReactiveCollection();
            FileList = Model.FileList.ToReadOnlyReactiveCollection();
            #endregion

            #region コマンドの動作設定
            // 実行の許可/不許可を制御するIObservable<bool>
            // このValueがtrueかfalseかで制御される
            ReactiveProperty<bool> gate = new ReactiveProperty<bool>(true);

            Delete = new ReactiveCommand(gate);
            Delete.Subscribe(
                async d =>
                {
                    await Model.DeleteAsync();
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

            SelectDirectory = new ReactiveCommand(gate);
            SelectDirectory.Subscribe(
                async d =>
                {
                    await Model.SelectDirectory(d as string);
                }
            );

            SelectFile = new ReactiveCommand(gate);
            SelectFile.Subscribe(
                async d =>
                {
                    await Model.SelectFile(d as string);
                }
            );
            #endregion
        }
    }
}
