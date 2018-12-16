using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology
{
    /// <summary>
    /// Dropboxやローカルを保存先に利用する場合の処理をまとめる
    /// </summary>
    public static class DropboxHelper
    {
        #region public

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
        /// ディレクトリがなければ作成する
        /// リストのクリアも行う
        /// </summary>
        /// <param name="dataOutput">データ出力モード</param>
        /// <param name="dropboxDirectory">Dropbox保存先ディレクトリ（相対パス、スラッシュから開始、最後スラッシュなし）</param>
        /// <param name="localDirectory">ローカル保存先ディレクトリ（フルパス）</param>
        /// <param name="accessToken">Dropboxアクセストークン</param>
        /// <param name="list">結果を入れるリストの参照（指定しない場合リスト作成）</param>
        /// <returns></returns>
        public static async Task<ObservableCollection<string>> GetFileListAsync(DataOutput dataOutput, string dropboxDirectory = null, string localDirectory = null, string accessToken = null, ObservableCollection<string> list = null)
        {
            if (list == null)
            {
                list = new ObservableCollection<string>();
            }
            else
            {
                list.Clear();
            }

            if (dataOutput == DataOutput.Dropbox)
            {
                using (var client = new DropboxClient(accessToken))
                {
                    // ディレクトリがなければ作成
                    await FolderExists(client, dropboxDirectory);

                    // ファイルを保存
                    var result = await client.Files.ListFolderAsync($"{dropboxDirectory}");
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
                // ディレクトリがなければ作成
                Json.SafeCreateDirectory(localDirectory);

                // ファイルリスト取得
                Json.GetFileList(list, localDirectory);
            }
            return list;
        }

        /// <summary>
        /// Dropboxかローカルのいずれかでデータを取得する
        /// ディレクトリがなければ作成する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataOutput"></param>
        /// <param name="filename"></param>
        /// <param name="dropboxDirectory"></param>
        /// <param name="localDirectory"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task<T> MultiLoadAsync<T>(DataOutput dataOutput, string filename, string dropboxDirectory = null, string localDirectory = null, string accessToken = null)
        {
            T data = default(T);
            if (dataOutput == DataOutput.Dropbox)
            {
                using (var client = new DropboxClient(accessToken))
                {
                    // ディレクトリがなければ作成
                    await FolderExists(client, dropboxDirectory);

                    using (var response = await client.Files.DownloadAsync($"{dropboxDirectory}/{filename}"))
                    {
                        data = await Json.ToObjectAsync<T>(await response.GetContentAsStringAsync());
                    }
                }
            }
            else if (dataOutput == DataOutput.Local)
            {
                data = await Json.Load<T>(localDirectory, filename);
            }
            return data;
        }

        /// <summary>
        /// Dropboxかローカルのいずれかでデータを文字列で取得する
        /// ディレクトリがなければ作成する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataOutput"></param>
        /// <param name="filename"></param>
        /// <param name="dropboxDirectory"></param>
        /// <param name="localDirectory"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task<string> MultiLoadStringAsync(DataOutput dataOutput, string filename, string dropboxDirectory = null, string localDirectory = null, string accessToken = null)
        {
            var data = string.Empty;
            if (dataOutput == DataOutput.Dropbox)
            {
                using (var client = new DropboxClient(accessToken))
                {
                    // ディレクトリがなければ作成
                    await FolderExists(client, dropboxDirectory);

                    using (var response = await client.Files.DownloadAsync($"{dropboxDirectory}/{filename}"))
                    {
                        data = await response.GetContentAsStringAsync();
                    }
                }
            }
            else if (dataOutput == DataOutput.Local)
            {
                data = Json.LoadString(localDirectory, filename);
            }
            return data;
        }

        /// <summary>
        /// Dropboxかローカルに文字列で保存
        /// </summary>
        /// <typeparam name="T">保存データクラス</typeparam>
        /// <param name="dataOutput">データ出力モード</param>
        /// <param name="filename">ファイル名（拡張子も必要）</param>
        /// <param name="data">保存データ</param>
        /// <param name="dropboxDirectory">Dropbox保存先ディレクトリ（相対パス、スラッシュから開始、最後スラッシュなし）</param>
        /// <param name="localDirectory">ローカル保存先ディレクトリ（フルパス）</param>
        /// <param name="accessToken">Dropboxアクセストークン</param>
        /// <returns></returns>
        public static async Task MultiSaveStringAsync(DataOutput dataOutput, string filename, string data, string dropboxDirectory = null, string localDirectory = null, string accessToken = null)
        {
            if (dataOutput == DataOutput.Dropbox)
            {
                using (var client = new DropboxClient(accessToken))
                {
                    FolderExists(client);

                    // ファイルを保存
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
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
                Json.SaveString(localDirectory, filename, data);
            }
        }

        /// <summary>
        /// Dropboxかローカルのいずれかでゴミ箱に移動する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataOutput"></param>
        /// <param name="filename"></param>
        /// <param name="trushDirectory"></param>
        /// <param name="dropboxDirectory"></param>
        /// <param name="localDirectory"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task<string> MultiDeleteFileAsync(DataOutput dataOutput, string filename, string trushDirectory, string fromDirectory, string dropboxRootDirectory = null, string localRootDirectory = null, string accessToken = null)
        {
            var data = string.Empty;
            if (dataOutput == DataOutput.Dropbox)
            {
                using (var client = new DropboxClient(accessToken))
                {
                    // ディレクトリがなければ作成
                    await FolderExists(client, dropboxRootDirectory);
                    await FolderExists(client, dropboxRootDirectory + fromDirectory);
                    await FolderExists(client, dropboxRootDirectory + trushDirectory);

                    await client.Files.MoveV2Async(dropboxRootDirectory + fromDirectory + "/" + filename, dropboxRootDirectory + trushDirectory + "/" + filename + Json.GetUnixTime());
                }
            }
            else if (dataOutput == DataOutput.Local)
            {
                Json.DeleteFile(localRootDirectory, fromDirectory, filename, trushDirectory);
            }
            return data;
        }

        ///// <summary>
        ///// ファイル出力のWriterを作成する
        ///// </summary>
        ///// <param name="filename"></param>
        ///// <returns></returns>
        //public static StreamWriter GetWriter(string filename)
        //{
        //    return new StreamWriter(GetTempFilePath(filename));
        //}

        /// <summary>
        /// 一時ファイルのパスを取得する
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetTempFilePath(string filename)
        {
            return Json.GetPath(Constants.TrushDirectory, filename);
        }


        #endregion

        #region private
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

    }
}
