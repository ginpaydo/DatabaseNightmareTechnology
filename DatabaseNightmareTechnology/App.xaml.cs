using DatabaseNightmareTechnology.Views;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using System.Windows;

namespace DatabaseNightmareTechnology
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// メインウィンドウの生成
        /// 普通はBootstrapperに書くが、このテンプレートではそうなっていない
        /// </summary>
        /// <returns></returns>
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        /// <summary>
        /// DIコンテナにクラスを登録する
        /// Navigation用のぺージクラス登録などもここで行う
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 画面の登録
            containerRegistry.RegisterForNavigation<HomeUserControl>();
            containerRegistry.RegisterForNavigation<DropboxSettingsUserControl>();
            containerRegistry.RegisterForNavigation<ConnectionRegisterUserControl>();
            containerRegistry.RegisterForNavigation<GenerateUserControl>();
            containerRegistry.RegisterForNavigation<TemplateEditUserControl>();
            containerRegistry.RegisterForNavigation<GeneralInputUserControl>();
            containerRegistry.RegisterForNavigation<SourceGenerateUserControl>();
            containerRegistry.RegisterForNavigation<OutputResultUserControl>();

            // ログを出力する
            containerRegistry.RegisterSingleton<ILoggerFacade, NLogLogger>();

        }


    }
}
