using DatabaseNightmareTechnology.Models;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.ViewModels
{
    class HomeUserControlViewModel : ViewModelBase
    {
        /// <summary>
        /// POCO
        /// </summary>
        private HomeUserControlModel model;

        public HomeUserControlViewModel(ILoggerFacade loggerFacade)
            : base("HomeUserControlViewModel", loggerFacade)
        {
            // Modelクラスを初期化
            model = new HomeUserControlModel();
        }
    }
}
