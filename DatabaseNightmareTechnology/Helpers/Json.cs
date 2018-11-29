using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology
{
    /// <summary>
    /// Jsonにしたり戻したり
    /// </summary>
    public static class Json
    {
        #region json
        /// <summary>
        /// Jsonをオブジェクトにする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<T> ToObjectAsync<T>(string value)
        {
            return await Task.Run(() =>
            {
                return JsonConvert.DeserializeObject<T>(value);
            });
        }

        /// <summary>
        /// オブジェクトをJsonにする
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<string> ToJsonAsync(object value)
        {
            return await Task.Run(() =>
            {
                return JsonConvert.SerializeObject(value);
            });
        }

        /// <summary>
        /// オブジェクトを保存する
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fullpath">フルパス</param>
        /// <returns></returns>
        public static async Task<string> Save(object value, string directory, string filename)
        {
            string fullpath = GetPath(directory, filename);
            var data = await ToJsonAsync(value);

            // ディレクトリがなければ作成する
            SafeCreateDirectory(directory);

            // 出力
            File.WriteAllText(fullpath, data);
            return data;
        }

        /// <summary>
        /// ファイルをオブジェクトにする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directory">ディレクトリ</param>
        /// <param name="filename">ファイル名</param>
        /// <returns></returns>
        public static async Task<T> Load<T>(string directory, string filename)
        {
            string fullpath = GetPath(directory, filename);
            // 存在チェック
            if (!File.Exists(fullpath))
            {
                return default(T);
            }
            var data = File.ReadAllText(fullpath);
            return await ToObjectAsync<T>(data);
        }
        #endregion

        #region private
        /// <summary>
        /// ディレクトリとファイル名から
        /// 入出力用パスを取得する、ディレクトリがなければ作成する
        /// （相対パスでも可）
        /// </summary>
        /// <param name="directory">後ろのスラッシュの有無は自由</param>
        /// <param name="filename">要拡張子</param>
        /// <returns></returns>
        private static string GetPath(string directory, string filename)
        {
            directory = AdjustDirectory(directory);
            SafeCreateDirectory(directory);
            return directory + filename;
        }

        /// <summary>
        /// ディレクトリの後ろをスラッシュにする
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private static string AdjustDirectory(string directory)
        {
            if (!directory.EndsWith("\\") && !directory.EndsWith("/"))
            {
                directory = directory + "/";
            }

            return directory;
        }
        #endregion

        #region ディレクトリ関係
        /// <summary>
        /// 指定したパスにディレクトリが存在しない場合
        /// すべてのディレクトリとサブディレクトリを作成します
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static DirectoryInfo SafeCreateDirectory(string directory)
        {
            directory = AdjustDirectory(directory);
            if (Directory.Exists(directory))
            {
                return null;
            }
            return Directory.CreateDirectory(directory);
        }

        /// <summary>
        /// ディレクトリを削除します
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static void DeleteDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            //ディレクトリ以外の全ファイルを削除
            string[] filePaths = Directory.GetFiles(directory);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            //ディレクトリの中のディレクトリも再帰的に削除
            string[] directoryPaths = Directory.GetDirectories(directory);
            foreach (string directoryPath in directoryPaths)
            {
                DeleteDirectory(directoryPath);
            }

            //中が空になったらディレクトリ自身も削除
            Directory.Delete(directory, false);
        }

        /// <summary>
        /// 指定したディレクトリのファイル一覧を取得する
        /// </summary>
        /// <param name="list">結果を入れるリストの参照</param>
        /// <param name="localDirectory">ディレクトリ（フルパス）</param>
        /// <returns></returns>
        public static ObservableCollection<string> GetFileList(ObservableCollection<string> list, string localDirectory)
        {
            if (list == null)
            {
                list = new ObservableCollection<string>();
            }

            if (localDirectory == null)
            {
                throw new System.ArgumentNullException(nameof(localDirectory));
            }

            var files = Directory.GetFiles(localDirectory);
            foreach (var item in files)
            {
                var file = item.Remove(0, AdjustDirectory(localDirectory).Length);
                list.Add(file);
            }
            return list;
        }
        #endregion
    }
}
