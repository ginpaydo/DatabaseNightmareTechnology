﻿namespace DatabaseNightmareTechnology
{
    /// <summary>
    /// アプリケーション定義
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// アプリケーションタイトル
        /// </summary>
        public const string Title = "DatabaseNightmareTechnology";

        /// <summary>
        /// 各画面表示領域名
        /// </summary>
        public const string ContentRegion = "ContentRegion";

        /// <summary>
        /// 設定ファイル名
        /// </summary>
        public const string DataFileName = "config.dat";

        /// <summary>
        /// 設定ファイル保存ディレクトリ
        /// </summary>
        public const string DataDirectory = "./data";

        /// <summary>
        /// アプリケーションディレクトリ（ローカル）
        /// </summary>
        public const string ApplicationDirectoryLocal = "D:" + ApplicationDirectoryDropbox;

        /// <summary>
        /// アプリケーションディレクトリ（共通の相対パス）
        /// </summary>
        public const string ApplicationDirectoryDropbox = "/DNT";

        /// <summary>
        /// 接続先ファイル保存ディレクトリ（共通の相対パス）
        /// </summary>
        public const string ConnectionDirectory = "/Connection";

        /// <summary>
        /// メタデータファイル保存ディレクトリ（共通の相対パス）
        /// </summary>
        public const string MetaDataDirectory = "/MetaData";

        // 画面名
        public const string HomeUserControl = "HomeUserControl";
        public const string DropboxSettingsUserControl = "DropboxSettingsUserControl";
        public const string ConnectionRegisterUserControl = "ConnectionRegisterUserControl";
        public const string GenerateUserControl = "GenerateUserControl";
        public const string TemplateEditUserControl = "TemplateEditUserControl";
        public const string GeneralInputUserControl = "GeneralInputUserControl";
        public const string SourceGenerateUserControl = "SourceGenerateUserControl";
        public const string OutputResultUserControl = "OutputResultUserControl";
    }
}
