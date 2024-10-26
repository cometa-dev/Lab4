using System;
using System.IO;
using System.Security.Cryptography;

namespace EncryptionLibrary
{
    public class FileEncryptor
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;
        public event EventHandler<ProgressEventArgs> ProgressChanged;
        public event EventHandler<EncryptionCompletedEventArgs> EncryptionCompleted;

        public FileEncryptor(string key)
        {
            using (var md5 = MD5.Create())
            {
                _key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
                _iv = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key + "IV"));
            }
        }

        public void EncryptFile(string inputFile, string outputFile, IProgress<int> progress)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;

                    using (FileStream inputStream = File.OpenRead(inputFile))
                    using (FileStream outputStream = File.Create(outputFile))
                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    using (CryptoStream cryptoStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;
                        long totalBytes = inputStream.Length;
                        long currentBytes = 0;

                        DateTime startTime = DateTime.Now;

                        while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cryptoStream.Write(buffer, 0, bytesRead);
                            currentBytes += bytesRead;
                        
                            int percentComplete = (int)((double)currentBytes / totalBytes * 100);
                            progress?.Report(percentComplete);
                            OnProgressChanged(percentComplete, DateTime.Now - startTime);
                        }

                        cryptoStream.FlushFinalBlock();
                        OnEncryptionCompleted(new FileInfo(outputFile).Length, DateTime.Now - startTime);
                    }
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(outputFile))
                {
                    try
                    {
                        File.Delete(outputFile);
                    }
                    catch { }
                }
                throw new EncryptionException("Помилка при шифруванні файлу", ex);
            }
        }

    public void DecryptFile(string inputFile, string outputFile, IProgress<int> progress)
    {
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;

                using (FileStream inputStream = File.OpenRead(inputFile))
                using (FileStream outputStream = File.Create(outputFile))
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    long totalBytes = inputStream.Length;
                    long currentBytes = 0;
                    DateTime startTime = DateTime.Now;

                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    using (CryptoStream cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
                    {
                        while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, bytesRead);
                            currentBytes += bytesRead;

                            int percentComplete = (int)((double)currentBytes / (totalBytes / 1.3) * 100);
                            if (percentComplete > 100) percentComplete = 100;
                            
                            progress?.Report(percentComplete);
                            OnProgressChanged(percentComplete, DateTime.Now - startTime);
                        }
                    }

                    outputStream.Flush();
                    progress?.Report(100);
                    OnProgressChanged(100, DateTime.Now - startTime);
                    OnEncryptionCompleted(new FileInfo(outputFile).Length, DateTime.Now - startTime);
                }
            }
        }
        catch (CryptographicException ex)
        {
            if (File.Exists(outputFile))
            {
                try
                {
                    File.Delete(outputFile);
                }
                catch { }
            }
            throw new EncryptionException("Помилка при розшифруванні файлу. Можливо, невірний ключ.", ex);
        }
        catch (Exception ex)
        {
            if (File.Exists(outputFile))
            {
                try
                {
                    File.Delete(outputFile);
                }
                catch { }
            }
            throw new EncryptionException("Помилка при розшифруванні файлу", ex);
        }
    }

        protected virtual void OnProgressChanged(int percentage, TimeSpan elapsed)
        {
            ProgressChanged?.Invoke(this, new ProgressEventArgs(percentage, elapsed));
        }

        protected virtual void OnEncryptionCompleted(long fileSize, TimeSpan totalTime)
        {
            EncryptionCompleted?.Invoke(this, new EncryptionCompletedEventArgs(fileSize, totalTime));
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public int Percentage { get; }
        public TimeSpan ElapsedTime { get; }

        public ProgressEventArgs(int percentage, TimeSpan elapsedTime)
        {
            Percentage = percentage;
            ElapsedTime = elapsedTime;
        }
    }

    public class EncryptionCompletedEventArgs : EventArgs
    {
        public long FileSize { get; }
        public TimeSpan TotalTime { get; }

        public EncryptionCompletedEventArgs(long fileSize, TimeSpan totalTime)
        {
            FileSize = fileSize;
            TotalTime = totalTime;
        }
    }

    public class EncryptionException : Exception
    {
        public EncryptionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}