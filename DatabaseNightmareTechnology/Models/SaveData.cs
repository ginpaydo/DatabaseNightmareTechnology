using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    /// <summary>
    /// ファイルとして保存する設定データ
    /// </summary>
    public class SaveData
    {
        /// <summary>
        /// Dropboxトークン
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 使用ディレクトリ
        /// </summary>
        public string LocalDirectory { get; set; }

        /// <summary>
        /// データの出力先
        /// </summary>
        public DataOutput DataOutput { get; set; }
    }
}
