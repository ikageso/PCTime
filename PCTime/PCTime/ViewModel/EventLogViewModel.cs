using Microsoft.TeamFoundation.MVVM;
using PCTime.Model;
using PCTime.ViewModel.ValidateionRules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace PCTime.ViewModel
{
    public class EventLogViewModel : WindowViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EventLogViewModel()
        {
            // コマンド登録
            ViewCommand = new RelayCommand(
                (x) =>
                {
                    GetData(StartDate, EndDate);
                }
                , (x) =>
                {
                    // ValidationRulesにより入力エラーがある場合は無効にする
                    return !this.HasErrors();
                });

            // 閉じるコマンド
            CloseCommand = new RelayCommand((x) =>
              {
                  this.RequestClose();
              });

            // ValidationRulesを設定
            ValidationRules.Add(new ValidationRuleDate("StartDate"));
            ValidationRules.Add(new ValidationRuleDate("EndDate"));
        }

        #region プロパティ

        private ObservableCollection<EventLogDataModel.EventDateTimeData> _DateTimeDatas;
        /// <summary>
        /// DateTimeDatas
        /// </summary>
        public ObservableCollection<EventLogDataModel.EventDateTimeData> DateTimeDatas
        {
            get
            {
                return _DateTimeDatas;
            }
            set
            {
                _DateTimeDatas = value;
                RaisePropertyChanged("DateTimeDatas");
            }
        }

        /// <summary>
        /// 表示コマンド
        /// </summary>
        public ICommand ViewCommand { get; private set; }

        /// <summary>
        /// CloseCommand
        /// </summary>
        public ICommand CloseCommand { get; private set; }

        private DateTime _StartDate = DateTime.Now.AddMonths(-1);
        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                var dt = value;
                _StartDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);

                RaisePropertyChanged("StartDate");
                RaisePropertyChanged("EndDate");
            }
        }

        private DateTime _EndDate = DateTime.Now;
        /// <summary>
        /// EndDate
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                var dt = value;
                _EndDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                RaisePropertyChanged("StartDate");
                RaisePropertyChanged("EndDate");
            }
        }
        #endregion

        #region メソッド
        /// <summary>
        /// イベントログのデータを取得する
        /// </summary>
        public void GetData(DateTime startDate, DateTime endDate)
        {
            string results = string.Empty;
            List<EventLogDataModel.EventDateTimeData> list = null;

            bool bRes = ProgressBar(
                () =>
                {
                    results = EventLogDataModel.GetEventLog(startDate, endDate);
                }
                , Properties.Resources.ProgresEventlogLabel);

            // 処理完了時
            if (bRes)
            {
                // 結果処理
                bRes = ProgressBar(
                () =>
                {
                    list = EventLogDataModel.MakeData(results);
                }
                , Properties.Resources.ProgresResultLabel);
            }

            if (bRes)
            {
                this.DateTimeDatas = new ObservableCollection<EventLogDataModel.EventDateTimeData>(list);
            }
            //else
            //{
            //    MessageBoxService.ShowInformation(" キャンセルされました", "結果");
            //    this.RequestClose();
            //}
        }

        /// <summary>
        /// 処理中ダイアログ表示
        /// </summary>
        /// <param name="action"></param>
        /// <param name="message"></param>
        private bool ProgressBar(Action action, string message)
        {
            var vm = new ProgressWindowViewModel(action, message);

            // 処理中ダイアログ表示
            WindowDisplayService.ShowDialog("ProgressWindowKey", vm);

            return vm.Result == MessageBoxResult.OK ? true : false;
        }
        #endregion

    }
    
    
}
