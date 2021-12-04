namespace Extensions
{
    public static class FileExtensions
    {
        public static string ToFileSize(this long fileLength)
        {
            string[] sizes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            int order = 0;
            while (fileLength >= 1024 && order < sizes.Length - 1)
            {
                order++;
                fileLength = fileLength / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return $"{fileLength:0.##} {sizes[order]}";
        }
    }
}
