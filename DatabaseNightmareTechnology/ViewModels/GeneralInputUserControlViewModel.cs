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
    class GeneralInputUserControlViewModel : ViewModelBase
    {
        /// <summary>
        /// POCO
        /// </summary>
        private GeneralInputUserControlModel Model;

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
        public ReadOnlyReactiveCollection<string> Items { get; private set; }
        #endregion

        #region Command
        /// <summary>
        /// 表示したときの処理
        /// </summary>
        public ReactiveCommand Activate { get; }
        /// <summary>
        /// チェック＆保存ボタン処理
        /// </summary>
        public ReactiveCommand Save { get; }
        #endregion

        public GeneralInputUserControlViewModel(ILoggerFacade loggerFacade)
            : base("GeneralInputUserControlViewModel", loggerFacade)
        {
            // Modelクラスを初期化
            Model = new GeneralInputUserControlModel();

            #region 値の連動設定
            FileName = Model.ToReactivePropertyAsSynchronized(
                m => m.FileName
                );
            SaveResult = Model.ToReactivePropertyAsSynchronized(
                m => m.SaveResult
                );
            #endregion

            #region リストの連動設定
            FileList = Model.FileList.ToReadOnlyReactiveCollection();
            Items = Model.Items.ToReadOnlyReactiveCollection();
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
                    await Model.Save();
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
