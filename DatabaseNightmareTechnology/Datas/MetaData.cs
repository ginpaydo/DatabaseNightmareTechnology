using System.Collections.Generic;

namespace DatabaseNightmareTechnology
{
    /// <summary>
    /// データベースから収集したメタデータ
    /// </summary>
    public class MetaData
    {
        /// <summary>
        /// テーブル
        /// </summary>
        public List<Table> Tables { get; set; }

        /// <summary>
        /// 汎用入力を適用する
        /// </summary>
        public GeneralInput GeneralInput
        {
            set
            {
                if(Tables != null)
                {
                    foreach (var item in Tables)
                    {
                        item.GeneralInput = value;
                    }
                }
            }
        }
    }

    /// <summary>
    /// テーブルデータ
    /// </summary>
    public class Table
    {
        /// <summary>
        /// 汎用入力
        /// 保存はせず、ソース生成直前で別データから適用して使う
        /// </summary>
        public GeneralInput GeneralInput { get; set; }

        /// <summary>
        /// カラム
        /// </summary>
        public List<Column> Columns { get; set; }

        /// <summary>
        /// プレフィクス付きの名前
        /// </summary>
        public string RawName { get; set; }

        /// <summary>
        /// プレフィクスを取った名前
        /// </summary>
        public Element Name { get; set; }

        /// <summary>
        /// コメント
        /// </summary>
        public string Comment { get; set; }

        // キー項目
        /// <summary>
        /// キー項目のDBの型
        /// DBから取得したカッコつきの値を格納
        /// </summary>
        public string KeyDataTypeRaw { get; set; }

        /// <summary>
        /// キー項目のDBの型
        /// カッコ除外
        /// </summary>
        public string KeyDataType { get; set; }

        /// <summary>
        /// キー項目の名前
        /// </summary>
        public Element KeyName { get; set; }

        // インデックス情報
        /// <summary>
        /// インデックスの種類ごとに項目を列挙
        /// "_id"抜き、Pascal
        /// </summary>
        public Dictionary<string, List<Element>> Indexs { get; set; }

        /// <summary>
        /// 各インデックスに登場する項目を列挙
        /// "_id"抜き、Pascal
        /// </summary>
        public List<Element> IndexColumns { get; set; }
    }

    /// <summary>
    /// カラムデータ
    /// </summary>
    public class Column
    {
        /// <summary>
        /// 元の名前
        /// </summary>
        public Element Name { get; set; }

        /// <summary>
        /// 型（DBの型）カッコ付きなので注意
        /// </summary>
        public string DataTypeRaw { get; set; }

        /// <summary>
        /// 型（DBの型）カッコ除外
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// コメント
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// データの長さ
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// キーならばtrue
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// Null可能ならtrue
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// 他のテーブルのIDだったら、そのクラス名を入れる
        /// PascalCase
        /// 例：user_auth -> UserAuth
        /// </summary>
        public string IndexClass { get; set; }

    }


    /// <summary>
    /// 要素名
    /// 様々な形の名前を持つ
    /// </summary>
    public class Element
    {
        private string raw;
        /// <summary>
        /// 元の名前
        /// SnakeCase
        /// </summary>
        public string Raw
        {
            get { return raw; }
            set
            {
                raw = value;
                Camel = value.SnakeToLowerCamel();
                Pascal = value.SnakeToUpperCamel();
            }
        }
        /// <summary>
        /// CamelCase
        /// </summary>
        public string Camel { get; private set; }

        /// <summary>
        /// PascalCase
        /// </summary>
        public string Pascal { get; private set; }

        public Element()
        {
            // なにもなし
        }

        public Element(string raw)
        {
            Raw = raw;
        }
    }
}
