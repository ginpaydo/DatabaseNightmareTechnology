namespace DatabaseNightmareTechnology.Models
{
    /// <summary>
    /// テンプレートデータ
    /// </summary>
    class DmtTemplate
    {
        /// <summary>
        /// 生成ファイル名
        /// Razor含む
        /// </summary>
        public string GenerateFileName { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Discription { get; set; }

        /// <summary>
        /// テンプレート本体
        /// Razor含む
        /// </summary>
        public string TemplateBody { get; set; }
    }
}
