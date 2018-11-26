using DatabaseNightmareTechnology.Models;
using Dropbox.Api;
using Dropbox.Api.Files;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology
{
    /// <summary>
    /// Jsonにしたり戻したり
    /// </summary>
    public static class Json
    {
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

        /// <summary>
        /// 入出力用パスを取得する、ディレクトリがなければ作成する
        /// （相対パスでも可）
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filename"></param>
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

        #region Dropboxあり
        // TODO:後でDropboxとローカルのメソッドに分けると良い

        /// <summary>
        /// Dropboxかローカルに保存
        /// </summary>
        /// <typeparam name="T">保存データクラス</typeparam>
        /// <param name="dataOutput">データ出力モード</param>
        /// <param name="filename">ファイル名（拡張子も必要）</param>
        /// <param name="data">保存データ</param>
        /// <param name="dropboxDirectory">Dropbox保存先ディレクトリ（相対パス、スラッシュから開始、最後スラッシュなし）</param>
        /// <param name="localDirectory">ローカル保存先ディレクトリ（フルパス）</param>
        /// <param name="accessToken">Dropboxアクセストークン</param>
        /// <returns></returns>
        public static async Task MultiSaveAsync<T>(DataOutput dataOutput, string filename, T data, string dropboxDirectory = null, string localDirectory = null, string accessToken = null)
        {
            if (dataOutput == DataOutput.Dropbox)
            {
                using (var client = new DropboxClient(accessToken))
                {
                    FolderExists(client);

                    // ファイルを保存
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(await Json.ToJsonAsync(data))))
                    {
                        await client.Files.UploadAsync(
                            $"{dropboxDirectory}/{filename}",
                            WriteMode.Overwrite.Instance,
                            false,
                            body: stream
                            );
                    }
                }
            }
            else if (dataOutput == DataOutput.Local)
            {
                await Json.Save(data, localDirectory, $"{filename}");
            }
        }

        /// <summary>
        /// 指定したディレクトリのファイル一覧を取得する
        /// </summary>
        /// <param name="list">結果を入れるリストの参照</param>
        /// <param name="dataOutput">データ出力モード</param>
        /// <param name="dropboxDirectory">Dropbox保存先ディレクトリ（相対パス、スラッシュから開始、最後スラッシュなし）</param>
        /// <param name="localDirectory">ローカル保存先ディレクトリ（フルパス）</param>
        /// <param name="accessToken">Dropboxアクセストークン</param>
        /// <returns></returns>
        public static async Task GetFileList(ObservableCollection<string> list, DataOutput dataOutput, string dropboxDirectory = null, string localDirectory = null, string accessToken = null)
        {
            if (dataOutput == DataOutput.Dropbox)
            {
                using (var client = new DropboxClient(accessToken))
                {
                    // ファイルを保存
                    var result = await client.Files.ListFolderAsync($"{Constants.ApplicationDirectoryDropbox + Constants.ConnectionDirectory}");
                    foreach (var item in result.Entries)
                    {
                        if (item.IsFile)
                        {
                            list.Add(item.Name);
                        }
                    }
                }
            }
            else if (dataOutput == DataOutput.Local)
            {
                var files = Directory.GetFiles(localDirectory);
                foreach (var item in files)
                {
                    var file = item.Remove(0, AdjustDirectory(localDirectory).Length);
                    list.Add(file);
                }
            }
        }

        /// <summary>
        /// Dropboxかローカルのいずれかでデータを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataOutput"></param>
        /// <param name="filename"></param>
        /// <param name="dropboxDirectory"></param>
        /// <param name="localDirectory"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task<T> LoadMultiData<T>(DataOutput dataOutput, string filename, string dropboxDirectory = null, string localDirectory = null, string accessToken = null)
        {
            T data = default(T);
            if (dataOutput == DataOutput.Dropbox)
            {
                using (var client = new DropboxClient(accessToken))
                {
                    using (var response = await client.Files.DownloadAsync($"{dropboxDirectory}/{filename}"))
                    {
                        data = await ToObjectAsync<T>(await response.GetContentAsStringAsync());
                    }
                }
            }
            else if (dataOutput == DataOutput.Local)
            {
                data = await Load<T>(localDirectory, filename);
            }
            return data;
        }

            #region フォルダチェック
            /// <summary>
            /// アプリケーションに必要なフォルダが
            /// Dropboxに存在することを確認し、なければ作成する
            /// </summary>
            /// <param name="client"></param>
            /// <returns></returns>
            private static async void FolderExists(DropboxClient client)
        {
            await FolderExists(client, Constants.ApplicationDirectoryDropbox);
        }

        private static async Task FolderExists(DropboxClient client, string path)
        {
            try
            {
                // フォルダが無いので作成
                await client.Files.CreateFolderV2Async(path);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #endregion


    }
}
