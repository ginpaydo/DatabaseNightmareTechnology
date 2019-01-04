using DatabaseNightmareTechnology.Models;
using Prism.Logging;

namespace DatabaseNightmareTechnology.ViewModels
{
    class HomeUserControlViewModel : ViewModelBase
    {
        public HomeUserControlViewModel(ILoggerFacade loggerFacade)
            : base(new HomeUserControlModel(), "HomeUserControlViewModel", loggerFacade)
        {
            var model = Model as HomeUserControlModel;
        }
    }
}
