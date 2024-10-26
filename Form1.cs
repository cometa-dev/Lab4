using System;
using System.ComponentModel;
using System.Windows.Forms;
using EncryptionLibrary;
using System.Diagnostics;
using NLog;
using System.Threading;

namespace FileEncryptorApp
{
    public partial class Form1 : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly BackgroundWorker _worker;
        private FileEncryptor _encryptor;
        private bool _isEncrypting;
        private string _inputFile;
        private string _outputFile;
        private readonly Stopwatch _stopwatch;
        private volatile bool _cancelRequested;

        public Form1()
        {
            Logger.Info("Ініціалізація програми");
            InitializeComponent();
            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            _stopwatch = new Stopwatch();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = ofd.FileName;
                    _inputFile = ofd.FileName;
                    Logger.Info($"Вибрано файл: {_inputFile}");
                }
            }
        }

        private void btnStartEncryption_Click(object sender, EventArgs e)
        {
            Logger.Info("Спроба почати шифрування");
            if (string.IsNullOrEmpty(txtKey.Text))
            {
                Logger.Warn("Спроба шифрування без ключа");
                MessageBox.Show("Введіть ключ шифрування");
                return;
            }

            if (string.IsNullOrEmpty(_inputFile))
            {
                Logger.Warn("Спроба шифрування без вибраного файлу");
                MessageBox.Show("Виберіть файл");
                return;
            }

            _isEncrypting = true;
            _outputFile = _inputFile + ".encrypted";
            _encryptor = new FileEncryptor(txtKey.Text);
            _encryptor.ProgressChanged += Encryptor_ProgressChanged;
            
            Logger.Info($"Початок шифрування файлу: {_inputFile}");
            StartOperation();
        }

        private void btnStartDecryption_Click(object sender, EventArgs e)
        {
            Logger.Info("Спроба почати дешифрування");
            if (string.IsNullOrEmpty(txtKey.Text))
            {
                Logger.Warn("Спроба дешифрування без ключа");
                MessageBox.Show("Введіть ключ шифрування");
                return;
            }

            if (string.IsNullOrEmpty(_inputFile))
            {
                Logger.Warn("Спроба дешифрування без вибраного файлу");
                MessageBox.Show("Виберіть файл");
                return;
            }

            _isEncrypting = false;
            _outputFile = _inputFile.Replace(".encrypted", ".decrypted");
            _encryptor = new FileEncryptor(txtKey.Text);
            _encryptor.ProgressChanged += Encryptor_ProgressChanged;
            
            Logger.Info($"Початок дешифрування файлу: {_inputFile}");
            StartOperation();
        }

        private void StartOperation()
        {
            Logger.Info($"Ініціалізація операції, Потік: {Thread.CurrentThread.ManagedThreadId}");
            progressBar.Value = 0;
            lblProgress.Text = "0%";
            lblTime.Text = "Час: 0:00:00";
            
            btnStartEncryption.Enabled = false;
            btnStartDecryption.Enabled = false;
            btnCancel.Enabled = true;
            
            _cancelRequested = false;
            _stopwatch.Reset();
            _stopwatch.Start();
            _worker.RunWorkerAsync();
        }

        private void Encryptor_ProgressChanged(object sender, ProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                if (!IsDisposed && !_cancelRequested)
                {
                    Logger.Debug($"Оновлення прогресу через Invoke, Потік: {Thread.CurrentThread.ManagedThreadId}");
                    Invoke(new Action(() => UpdateProgress(e.Percentage, e.ElapsedTime)));
                }
                return;
            }
            UpdateProgress(e.Percentage, e.ElapsedTime);
        }

        private void UpdateProgress(int percentage, TimeSpan elapsedTime)
        {
            try
            {
                if (!IsDisposed && !_cancelRequested)
                {
                    progressBar.Value = percentage;
                    lblProgress.Text = $"{percentage}%";
                    lblTime.Text = $"Час: {elapsedTime:hh\\:mm\\:ss}";
                    Logger.Debug($"Прогрес: {percentage}%, Час: {elapsedTime:hh\\:mm\\:ss}");
                }
            }
            catch (InvalidOperationException ex)
            {
                Logger.Error(ex, "Помилка при оновленні UI");
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Logger.Info($"Потік {Thread.CurrentThread.ManagedThreadId}: Початок операції");
            try
            {
                var progress = new Progress<int>(value =>
                {
                    if (!_cancelRequested)
                    {
                        _worker.ReportProgress(value);
                    }
                });

                if (_worker.CancellationPending)
                {
                    Logger.Info("Операцію скасовано через CancellationPending");
                    e.Cancel = true;
                    return;
                }

                if (_isEncrypting)
                {
                    Logger.Info("Початок шифрування файлу");
                    _encryptor.EncryptFile(_inputFile, _outputFile, progress);
                }
                else
                {
                    Logger.Info("Початок дешифрування файлу");
                    _encryptor.DecryptFile(_inputFile, _outputFile, progress);
                }

                if (_cancelRequested)
                {
                    Logger.Info("Операцію скасовано користувачем");
                    e.Cancel = true;
                    if (System.IO.File.Exists(_outputFile))
                    {
                        try
                        {
                            System.IO.File.Delete(_outputFile);
                            Logger.Info("Видалено тимчасовий файл після скасування");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "Помилка при видаленні тимчасового файлу");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Потік {Thread.CurrentThread.ManagedThreadId}: Помилка при обробці файлу");
                e.Result = ex;
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (!IsDisposed && !_cancelRequested)
                {
                    progressBar.Value = e.ProgressPercentage;
                    lblProgress.Text = $"{e.ProgressPercentage}%";
                }
            }
            catch (InvalidOperationException ex)
            {
                Logger.Error(ex, "Помилка при оновленні прогресу");
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Logger.Info("Завершення операції");
            _stopwatch.Stop();
            TimeSpan totalTime = _stopwatch.Elapsed;

            if (_encryptor != null)
            {
                _encryptor.ProgressChanged -= Encryptor_ProgressChanged;
            }

            btnStartEncryption.Enabled = true;
            btnStartDecryption.Enabled = true;
            btnCancel.Enabled = false;

            if (_cancelRequested)
            {
                Logger.Info("Операцію скасовано користувачем");
                MessageBox.Show("Операцію скасовано", "Інформація", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (e.Error != null)
            {
                Logger.Error(e.Error, "Помилка при виконанні");
                MessageBox.Show($"Виникла помилка: {e.Error.Message}", "Помилка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (e.Result is Exception exception)
            {
                Logger.Error(exception, "Виникла помилка під час виконання");
                MessageBox.Show($"Виникла помилка: {exception.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var fileInfo = new System.IO.FileInfo(_outputFile);
            string operation = _isEncrypting ? "Шифрування" : "Розшифрування";
            string fileSize = (fileInfo.Length / 1024.0 / 1024.0).ToString("F2");

            string resultMessage = 
                $"{operation} успішно завершено!\n\n" +
                $"Ім'я файлу: {fileInfo.Name}\n" +
                $"Розмір файлу: {fileSize} МБ\n" +
                $"Час виконання: {totalTime:hh\\:mm\\:ss}";

            Logger.Info($"Успішне завершення: {resultMessage}");
            MessageBox.Show(
                resultMessage,
                "Інформація про результат", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Logger.Info("Запит на скасування операції");
            if (_worker.IsBusy && !_cancelRequested)
            {
                _cancelRequested = true;
                _worker.CancelAsync();
                btnCancel.Enabled = false;
                lblProgress.Text = "Скасування...";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Logger.Info("Закриття програми");
            if (_worker.IsBusy)
            {
                Logger.Info("Скасування операції при закритті програми");
                _cancelRequested = true;
                _worker.CancelAsync();
                while (_worker.IsBusy)
                {
                    Application.DoEvents();
                }
            }
            base.OnFormClosing(e);
        }
    }
}