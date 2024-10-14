using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TaskManagerStyleApp {
    public partial class MainWindow : Window {
        private ObservableCollection<ProcessInfo> processList = new ObservableCollection<ProcessInfo>();

        public MainWindow() {
            InitializeComponent();
            ProcessesListView.ItemsSource = processList;
        }

        private async void ScanProcessesButton_Click(object sender, RoutedEventArgs e) {
            processList.Clear();
            try {
                await Task.Run(() => {
                    var processes = Process.GetProcesses();
                    foreach (var process in processes) {
                        try {
                            var info = new ProcessInfo {
                                ProcessName = process.ProcessName,
                                Id = process.Id,
                                MemoryUsage = $"{process.WorkingSet64 / 1024 / 1024} MB"
                            };
                            Dispatcher.Invoke(() => processList.Add(info));
                        }
                        catch {
                        }
                    }
                });
            }
            catch (Exception ex) {
                MessageBox.Show($"扫描进程时出错：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EndProcessButton_Click(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Tag is int processId) {
                try {
                    var process = Process.GetProcessById(processId);
                    process.Kill();
                    var item = processList.FirstOrDefault(p => p.Id == processId);
                    if (item != null) {
                        processList.Remove(item);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show($"无法结束进程：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenProgramButton_Click(object sender, RoutedEventArgs e) {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog {
                Filter = "可执行文件 (*.exe)|*.exe",
                Title = "选择可执行文件"
            };

            if (openFileDialog.ShowDialog() == true) {
                try {
                    Process.Start(openFileDialog.FileName);
                }
                catch (Exception ex) {
                    MessageBox.Show($"无法打开程序：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public class ProcessInfo {
        public string ProcessName { get; set; }
        public int Id { get; set; }
        public string MemoryUsage { get; set; }
    }
}