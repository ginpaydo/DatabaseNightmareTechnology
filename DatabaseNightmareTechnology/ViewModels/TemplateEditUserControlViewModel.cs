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
    class TemplateEditUserControlViewModel : ViewModelBase
    {
        /// <summary>
        /// POCO
        /// </summary>
        private TemplateEditUserControlModel Model;

        #region ReactiveProperty
        /// <summary>
        /// 生成ファイル名(Razor可)
        /// </summary>
        public ReactiveProperty<string> GenerateFileName { get; }

        /// <summary>
        /// テンプレートファイル名
        /// </summary>
        public ReactiveProperty<string> Title { get; }

        /// <summary>
        /// 説明
        /// </summary>
        public ReactiveProperty<string> Discription { get; }

        /// <summary>
        /// テンプレート本体
        /// </summary>
        public ReactiveProperty<string> TemplateBody { get; }

        /// <summary>
        /// 保存結果
        /// </summary>
        public ReactiveProperty<string> SaveResult { get; }

        /// <summary>
        /// ファイルリスト
        /// </summary>
        public ReadOnlyReactiveCollection<string> FileList { get; private set; }
        #endregion

        #region Command
        /// <summary>
        /// 表示したときの処理
        /// </summary>
        public ReactiveCommand Activate { get; }
        /// <summary>
        /// 保存ボタン処理
        /// </summary>
        public ReactiveCommand Save { get; }
        /// <summary>
        /// ファイル選択処理
        /// </summary>
        public ReactiveCommand SelectFile { get; }
        #endregion

        public TemplateEditUserControlViewModel(ILoggerFacade loggerFacade)
            : base("TemplateEditUserControlViewModel", loggerFacade)
        {
            // Modelクラスを初期化
            Model = new TemplateEditUserControlModel();

            #region 値の連動設定
            GenerateFileName = Model.ToReactivePropertyAsSynchronized(
                m => m.GenerateFileName
                );
            Title = Model.ToReactivePropertyAsSynchronized(
                m => m.Title
                );
            Discription = Model.ToReactivePropertyAsSynchronized(
                m => m.Discription
                );
            TemplateBody = Model.ToReactivePropertyAsSynchronized(
                m => m.TemplateBody
                );
            SaveResult = Model.ToReactivePropertyAsSynchronized(
                m => m.ActionResult
                );
            #endregion

            #region リストの連動設定
            FileList = Model.FileList.ToReadOnlyReactiveCollection();
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

            SelectFile = new ReactiveCommand(gate);
            SelectFile.Subscribe(
                async d =>
                {
                    if (d != null)
                    {
                        await Model.SelectFile(d as string);
                    }
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
