﻿using System.Collections.ObjectModel;

namespace DatabaseNightmareTechnology
{
    /// <summary>
    /// ファイルとして保存する接続先情報
    /// </summary>
    public class ConnectionSettingData
    {
        /// <summary>
        /// 対象データベース
        /// </summary>
        public DatabaseEngine DatabaseEngine { get; set; }

        /// <summary>
        /// 接続文字列
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// プレフィクスのリスト
        /// </summary>
        public ObservableCollection<string> PrefixList { get; } = new ObservableCollection<string>();

        /// <summary>
        /// 共通項目のリスト
        /// </summary>
        public ObservableCollection<string> CommonColumnList { get; } = new ObservableCollection<string>();
    }
}
