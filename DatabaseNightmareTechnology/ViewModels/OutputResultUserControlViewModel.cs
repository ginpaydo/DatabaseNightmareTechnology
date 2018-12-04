﻿using DatabaseNightmareTechnology.Models;
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
        /// 保存結果
        /// </summary>
        public ReactiveProperty<string> Body { get; }

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
                d =>
                {
                    Model.Delete(d as string);
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
