using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    class HomeUserControlModel : ModelBase
    {
        /// <summary>
        /// セーブデータ使用画面
        /// </summary>
        protected override bool UseSaveData
        {
            get { return false; }
        }

        protected override async Task Activate()
        {
            await Task.Delay(1);
        }

        protected override bool CheckRequiredFields()
        {
            return true;
        }
    }
}
