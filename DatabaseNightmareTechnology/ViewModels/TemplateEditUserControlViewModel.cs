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
    /// テンプレート編集
    /// </summary>
    class TemplateEditUserControlViewModel : ViewModelBase
    {
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
        /// ファイルリスト
        /// </summary>
        public ReadOnlyReactiveCollection<string> FileList { get; private set; }
        #endregion

        #region Command
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
            : base(new TemplateEditUserControlModel(), "テンプレート編集", loggerFacade)
        {
            var model = Model as TemplateEditUserControlModel;

            #region 値の連動設定
            GenerateFileName = model.ToReactivePropertyAsSynchronized(
                m => m.GenerateFileName
                );
            Title = model.ToReactivePropertyAsSynchronized(
                m => m.Title
                );
            Discription = model.ToReactivePropertyAsSynchronized(
                m => m.Discription
                );
            TemplateBody = model.ToReactivePropertyAsSynchronized(
                m => m.TemplateBody
                );
            #endregion

            #region リストの連動設定
            FileList = model.FileList.ToReadOnlyReactiveCollection();
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
    }
}
