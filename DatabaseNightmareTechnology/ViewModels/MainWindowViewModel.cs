using DatabaseNightmareTechnology.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using Prism.Regions;
using Prism.Logging;

namespace DatabaseNightmareTechnology.ViewModels
{
    /// <summary>
    /// メインウィンドウ
    /// 各ユーザコントロールを表示する
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Private
        /// <summary>
        /// 各画面表示領域
        /// </summary>
        private string DefaultRegion = Constants.ContentRegion;
        #endregion

        #region ReactiveProperty
        /// <summary>
        /// タイトル
        /// </summary>
        public ReadOnlyReactiveProperty<string> Title { get; }
        #endregion

        #region Command
        /// <summary>
        /// Home遷移
        /// </summary>
        public ReactiveCommand ToHome { get; }
        /// <summary>
        /// Dropbox設定遷移
        /// </summary>
        public ReactiveCommand ToDropboxSettings { get; }
        /// <summary>
        /// 接続先登録遷移
        /// </summary>
        public ReactiveCommand ToConnectionRegister { get; }
        /// <summary>
        /// 生成遷移
        /// </summary>
        public ReactiveCommand ToGenerate { get; }
        /// <summary>
        /// テンプレート編集遷移
        /// </summary>
        public ReactiveCommand ToTemplateEdit { get; }
        /// <summary>
        /// ソース生成遷移
        /// </summary>
        public ReactiveCommand ToGenetalInput { get; }
        /// <summary>
        /// 汎用データ遷移
        /// </summary>
        public ReactiveCommand ToSourceGenerate { get; }
        /// <summary>
        /// 出力プレビュー遷移
        /// </summary>
        public ReactiveCommand ToOutputResult { get; }
        #endregion

        #region Initialize
        public MainWindowViewModel(IRegionManager regionManager, ILoggerFacade loggerFacade)
            : base(new MainWindowModel(), "メインウィンドウ", loggerFacade)
        {
            var model = Model as MainWindowModel;

            #region 値の連動設定
            Title = model.ToReactivePropertyAsSynchronized(
                m => m.Title   // モデルとの対応付け
                ).ToReadOnlyReactiveProperty();
            #endregion

            #region ボタンの動作設定
            // 実行の許可/不許可を制御するIObservable<bool>
            // このValueがtrueかfalseかで制御される
            ReactiveProperty<bool> gate = new ReactiveProperty<bool>(true);

            ToHome = new ReactiveCommand(gate);
            ToHome.Subscribe(
                d =>
                {
                    regionManager.RequestNavigate(DefaultRegion, Constants.HomeUserControl);
                }
            );
            ToDropboxSettings = new ReactiveCommand(gate);
            ToDropboxSettings.Subscribe(
                d =>
                {
                    regionManager.RequestNavigate(DefaultRegion, Constants.DropboxSettingsUserControl);
                }
            );
            ToConnectionRegister = new ReactiveCommand(gate);
            ToConnectionRegister.Subscribe(
                d =>
                {
                    regionManager.RequestNavigate(DefaultRegion, Constants.ConnectionRegisterUserControl);
                }
            );
            ToGenerate = new ReactiveCommand(gate);
            ToGenerate.Subscribe(
                d =>
                {
                    regionManager.RequestNavigate(DefaultRegion, Constants.GenerateUserControl);
                }
            );
            ToTemplateEdit = new ReactiveCommand(gate);
            ToTemplateEdit.Subscribe(
                d =>
                {
                    regionManager.RequestNavigate(DefaultRegion, Constants.TemplateEditUserControl);
                }
            );
            ToGenetalInput = new ReactiveCommand(gate);
            ToGenetalInput.Subscribe(
                d =>
                {
                    regionManager.RequestNavigate(DefaultRegion, Constants.GeneralInputUserControl);
                }
            );
            ToSourceGenerate = new ReactiveCommand(gate);
            ToSourceGenerate.Subscribe(
                d =>
                {
                    regionManager.RequestNavigate(DefaultRegion, Constants.SourceGenerateUserControl);
                }
            );
            ToOutputResult = new ReactiveCommand(gate);
            ToOutputResult.Subscribe(
                d =>
                {
                    regionManager.RequestNavigate(DefaultRegion, Constants.OutputResultUserControl);
                }
            );
            #endregion

        }
        #endregion
    }
}
