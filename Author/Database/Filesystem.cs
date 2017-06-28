using Newtonsoft.Json;
using PCLStorage;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Author.Database
{
    public static class Filesystem
    {
        static IFolder _local = null;

        static Filesystem()
        {
            _local = FileSystem.Current.LocalStorage;
        }

        public static async Task<System.IO.Stream> OpenAsync(string path, bool readOnly = true)
        {
            IFile file = null;
            if (readOnly)
                file = await _local.GetFileAsync(path);
            else
                file = await _local.CreateFileAsync(path, CreationCollisionOption.ReplaceExisting);

            if (file == null)
                return null;

            return await file.OpenAsync(readOnly ? FileAccess.Read : FileAccess.ReadAndWrite);
        }

        public static async Task<bool> SaveToPath<T>(string path, T data)
        {
            if (Device.RuntimePlatform == Device.Windows || Device.RuntimePlatform == Device.WinPhone)
                path.Replace('/', '\\');

            using (System.IO.Stream stream = await OpenAsync(path, false))
            {
                if (stream == null)
                    return false;

                using (System.IO.TextWriter writer = new System.IO.StreamWriter(stream))
                {
                    writer.Write(JsonConvert.SerializeObject(data));
                    return true;
                }
            }
        }

        public static async Task<T> LoadFromPath<T>(string path)
        {
            if (Device.RuntimePlatform == Device.Windows || Device.RuntimePlatform == Device.WinPhone)
                path.Replace('/', '\\');

            using (System.IO.Stream stream = await OpenAsync(path))
            {
                if (stream == null)
                    return default(T);

                using (System.IO.TextReader reader = new System.IO.StreamReader(stream))
                    return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            }
        }
    }
}
