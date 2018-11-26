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
    class OutputResultUserControlViewModel : ViewModelBase
    {
        /// <summary>
        /// POCO
        /// </summary>
        private OutputResultUserControlModel model;

        public OutputResultUserControlViewModel(ILoggerFacade loggerFacade)
            : base("OutputResultUserControlViewModel", loggerFacade)
        {
            // Modelクラスを初期化
            model = new OutputResultUserControlModel();
        }
    }
}
