using System;
using System.IO;

// From ServUO: https://github.com/ServUO/ServUO/blob/37ce53049e2aec494d5cc24c7a8e06db9c39a759/Server/Persistence/Persistence.cs#L8

namespace Server
{
    public static class Persistence
    {
        public static void Serialize(string path, Action<GenericWriter> serializer)
        {
            Serialize(new FileInfo(path), serializer);
        }

        public static void Serialize(FileInfo file, Action<GenericWriter> serializer)
        {
            file.Refresh();

            if (file.Directory != null && !file.Directory.Exists)
            {
                file.Directory.Create();
            }

            if (!file.Exists)
            {
                file.Create().Close();
            }

            file.Refresh();

            using (var fs = file.OpenWrite())
            {
                var writer = new BinaryFileWriter(fs, true);

                try
                {
                    serializer(writer);
                }
                finally
                {
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        public static void Deserialize(string path, Action<GenericReader> deserializer)
        {
            Deserialize(path, deserializer, true);
        }

        public static void Deserialize(FileInfo file, Action<GenericReader> deserializer)
        {
            Deserialize(file, deserializer, true);
        }

        public static void Deserialize(string path, Action<GenericReader> deserializer, bool ensure)
        {
            Deserialize(new FileInfo(path), deserializer, ensure);
        }

        public static void Deserialize(FileInfo file, Action<GenericReader> deserializer, bool ensure)
        {
            file.Refresh();

            if (file.Directory != null && !file.Directory.Exists)
            {
                if (!ensure)
                {
                    throw new DirectoryNotFoundException();
                }

                file.Directory.Create();
            }

            if (!file.Exists)
            {
                if (!ensure)
                {
                    throw new FileNotFoundException
                    {
                        Source = file.FullName
                    };
                }

                file.Create().Close();
            }

            file.Refresh();

            using (var fs = file.OpenRead())
            {
                var reader = new BinaryFileReader(new BinaryReader(fs));

                try
                {
                    deserializer(reader);
                }
                catch (EndOfStreamException eos)
                {
                    if (file.Length > 0)
                    {
                        throw new Exception(String.Format("[Persistance]: {0}", eos));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("[Persistance]: An error was encountered while loading a saved object");

                    throw new Exception(String.Format("[Persistance]: {0}", e));
                }
                finally
                {
                    reader.Close();
                }
            }
        }
    }
}