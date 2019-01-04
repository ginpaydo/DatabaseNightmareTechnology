using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseNightmareTechnology
{
    /// <summary>
    /// 汎用入力
    /// 処理用
    /// </summary>
    public class GeneralInput
    {
        public const char Separator = '#';
        public Dictionary<string, List<string>> Values { get; set; }
        public Dictionary<string, Dictionary<string, List<string>>> ListedValues { get; set; }

        private void Initialize()
        {
            Values = new Dictionary<string, List<string>>();
            ListedValues = new Dictionary<string, Dictionary<string, List<string>>>();
        }

        public GeneralInput()
        {
            Initialize();
        }

        /// <summary>
        /// 汎用入力
        /// 画面表示中のデータからのコンバート
        /// </summary>
        /// <param name="items">画面表示中のデータ</param>
        public GeneralInput(List<DisplayValueData> items)
        {
            Initialize();

            ReadRawData(items);
        }

        /// <summary>
        /// 汎用入力
        /// 表示用データからのコンバート
        /// </summary>
        /// <param name="raw">CSVから読みこんだデータ</param>
        public GeneralInput(RawGeneralInput raw)
        {
            Initialize();

            ReadRawData(raw.Datas);
        }

        /// <summary>
        /// Rawデータを読む
        /// </summary>
        /// <param name="raws">CSVから読みこんだデータ</param>
        private void ReadRawData(List<DisplayValueData> raws)
        {
            // '#'区切りを処理する
            foreach (var item in raws)
            {
                if (item.Key.Contains(Separator))
                {
                    // リスト化対象パラメータ
                    var names = item.Key.Split(Separator);
                    var key = names[0];
                    var listname = names[1];
                    if (!ListedValues.ContainsKey(key))
                    {
                        ListedValues.Add(key, new Dictionary<string, List<string>>());
                    }
                    ListedValues[key].Add(listname, item.Values.Split(DisplayValueData.Separator).ToList());
                }
                else
                {
                    // 単独パラメータ
                    if (!Values.ContainsKey(item.Key))
                    {
                        // 重複しているものは無視
                        Values.Add(item.Key, item.Values.Split(DisplayValueData.Separator).ToList());
                    }
                }
            }
        }
    }

    /// <summary>
    /// CSVからの入力
    /// 画面表示用
    /// </summary>
    public class RawGeneralInput
    {
        public List<DisplayValueData> Datas { get; set; }

        private void Initialize()
        {
            Datas = new List<DisplayValueData>();
        }

        /// <summary>
        /// 処理用データからのコンバート
        /// </summary>
        /// <param name="generalInput">処理用データ</param>
        public RawGeneralInput(GeneralInput generalInput)
        {
            Initialize();

            // コンバート処理
            foreach (var item in generalInput.ListedValues)
            {
                foreach (var subitem in item.Value)
                {
                    var nameValueData = new DisplayValueData(item.Key + GeneralInput.Separator + subitem.Key, subitem.Value);
                    Datas.Add(nameValueData);
                }
            }

            foreach (var item in generalInput.Values)
            {
                var nameValueData = new DisplayValueData(item.Key, item.Value);
                Datas.Add(nameValueData);
            }

        }

        /// <summary>
        /// CSVからの入力データ
        /// </summary>
        /// <param name="fullpath">CSVファイルフルパス</param>
        public RawGeneralInput(string fullpath)
        {
            Initialize();

            // csvファイルを開く
            using (var sr = new System.IO.StreamReader(fullpath))
            {
                // ストリームの末尾まで繰り返す
                while (!sr.EndOfStream)
                {
                    // ファイルから一行読み込む
                    var line = sr.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        // 読み込んだ一行をカンマ毎に分けて配列に格納する
                        Datas.Add(new DisplayValueData(line));
                    }
                }
            }
        }
    }

    /// <summary>
    /// 1件（行）のデータ
    /// 表示用
    /// </summary>
    public class DisplayValueData
    {
        public static char Separator = ',';

        /// <summary>
        /// 1列目
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 2列目以降
        /// </summary>
        public string Values { get; set; }

        /// <summary>
        /// CSV1行分のデータを格納する
        /// </summary>
        /// <param name="row">CSV1行分のデータ（カンマ区切り）</param>
        public DisplayValueData(string row)
        {
            var all = row.Split(Separator);

            var key = all[0];

            // 2列目以降をカンマでつなぐ
            var values = all.ToList();
            values.RemoveAt(0);
            var strValues = ListToString(values);

            Key = key;
            Values = strValues;
        }

        public DisplayValueData(string key, List<string> values)
        {
            Key = key;
            Values = ListToString(values);
        }

        /// <summary>
        /// リストをカンマ区切りの文字列に変換
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string ListToString(List<string> list)
        {
            var strValues = new StringBuilder();
            foreach (var item in list)
            {
                if (strValues.Length > 0)
                {
                    strValues.Append(Separator);
                }
                var str = item.Trim('\"').Replace("\"\"", "\"");    // ダブルクォーテーション処理
                strValues.Append(str);
            }

            return strValues.ToString().Trim(',');  // 1行ごとにデータ長は変わる
        }
    }

    /// <summary>
    /// 1件（行）のデータ
    /// </summary>
    public class ValueData
    {
        /// <summary>
        /// 1列目
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 2列目以降
        /// </summary>
        public List<string> Values { get; set; }

        /// <summary>
        /// 1件（行）のデータ
        /// </summary>
        public ValueData()
        {
            // なにもなし
        }

        /// <summary>
        /// 1件（行）のデータ
        /// 表示用データからのコンバート
        /// </summary>
        /// <param name="row">表示用データ</param>
        public ValueData(DisplayValueData row)
        {
            Key = row.Key;
            Values = row.Values.Split(DisplayValueData.Separator).ToList();
        }
    }
}
