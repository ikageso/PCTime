using Microsoft.TeamFoundation.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PCTime.ViewModel
{
    class ProgressWindowViewModel : WindowViewModel
    {
        public ProgressWindowViewModel()
        {
            // キャンセルボタン押下時のコマンド登録
            CancelCommand = new RelayCommand(() => 
                {
                    Result = MessageBoxResult.Cancel;
                    this.RequestClose();
                });
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="action"></param>
        /// <param name="message"></param>
        public ProgressWindowViewModel(Action action, string message) :this()
        {
            this.Message = message;

            bool isCompleted = false;

            // 重い処理
            Task.Factory.StartNew(() =>
            {
                action();
                isCompleted = true;
            });

            var cancelTokenSource = new CancellationTokenSource();
            Task task = Task.Factory.StartNew(() =>
            {
                int interval = 100;
                while (true)
                {
                    // 重い処理完了待ち
                    if (isCompleted)
                    {
                        break;
                    }

                    Thread.Sleep(interval);

                    if (cancelTokenSource.IsCancellationRequested)
                    {
                        Debug.WriteLine("キャンセルされました。");
                        return;
                    }
                }

                // 処理完了時ダイアログを閉じる（WindowViewModelのResultに値を設定する）
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.Result = MessageBoxResult.OK;
                    this.RequestClose();
                }));
            });

            if (this.Result.Equals(MessageBoxResult.Cancel))
            {
                cancelTokenSource.Cancel();
            }
        }

        /// <summary>
        /// CancelCommand
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        private string _Message;
        /// <summary>
        /// Message
        /// </summary>
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
                RaisePropertyChanged("Message");
            }
        }

        /// <summary>
        /// Result
        /// </summary>
        public MessageBoxResult Result { get; set; }

    }
}
