using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCTime.Model
{
    public class EventLogDataModel
    {
        /// <summary>
        /// 日にちごとの開始・終了データのクラス
        /// </summary>
        public class EventDateTimeData
        {
            public string Date { get; set; }                // キー: yyyy/MM/dd
            public DateTime? StartTime { get; set; }        // 日付データから判定した開始時刻
            public DateTime? EndTime { get; set; }          // 日付データから判定した終了時刻
            public bool EndTimeNextDay { get; set; }        // 翌日終了フラグ
            public List<DateData> DateList { get; set; }    // 日付における開始・終了時刻のペアのリスト

            /// <summary>
            /// 日付における開始・終了時刻のペアのクラス
            /// </summary>
            public class DateData
            {
                public DateTime? StartTime { get; set; }
                public DateTime? EndTime { get; set; }
            }

        }

        /// <summary>
        /// イベントログから取り出したEventCode、TimeGeneratedを格納するクラス
        /// </summary>
        public class EventTimeGeneratedData
        {
            public string EventCode { get; set; }
            public DateTime TimeGenerated { get; set; }
        }

        /// <summary>
        /// イベントログを取得
        /// </summary>
        /// <param name="startDate">開始日</param>
        /// <param name="endDate">終了日</param>
        /// <returns>イベントログ取得結果のCSVデータ</returns>
        public static string GetEventLog(DateTime startDate, DateTime endDate)
        {
            string results = string.Empty;

            //Processオブジェクトを作成
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            //出力を読み取れるようにする
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = false;
            //ウィンドウを表示しないようにする
            p.StartInfo.CreateNoWindow = true;

            // 日付を整形する
            string startTimeGenerated = startDate.ToString("yyyyMMdd000000.0+540");
            string endTimeGenerated = endDate.ToString("yyyyMMdd235959.0+540");

            // コマンドライン文字列作成
            string sCmd = string.Format(@"wmic ntevent where ""(logfile='system' and SourceName='EventLog' and TimeGenerated >='{0}' and TimeGenerated <='{1}' and (EventCode='6005' or EventCode='6006'))"" list /format:CSV"
                                        , startTimeGenerated
                                        , endTimeGenerated);

            Debug.WriteLine(sCmd);

            //コマンドラインを指定（"/c"は実行後閉じるために必要）
            p.StartInfo.Arguments = @"/c " + sCmd;

            //開始
            p.Start();

            //出力を読み取る
            results = p.StandardOutput.ReadToEnd();

            //プロセス終了まで待機する
            //WaitForExitはReadToEndの後である必要がある
            //(親プロセス、子プロセスでブロック防止のため)
            p.WaitForExit();
            p.Close();

            return results;
        }

        /// <summary>
        /// イベントログから表示データを作成する
        /// </summary>
        /// <param name="eventData">イベントログ取得結果のCSVデータ</param>
        public static List<EventDateTimeData> MakeData(string eventData)
        {
            // 結果CSV
            // Node,Category,CategoryString,ComputerName,Data,EventCode,EventIdentifier,EventType,InsertionStrings,Logfile,Message,RecordNumber,SourceName,TimeGenerated,TimeWritten,Type,User
            // EventCode=6005 イベント ログ サービス開始
            // EventCode=6006 イベント ログ サービス停止
            // TimeGenerated イベント発生時刻(yyyyMMdd235959.0+540)→20150310062329.000000-000;

            // 改行で区切る
            var splitEventData = System.Text.RegularExpressions.Regex.Split(eventData, Environment.NewLine);

            // csvから、EventCode,TimeGeneratedのデータを作成する
            List<EventTimeGeneratedData> list = new List<EventTimeGeneratedData>();
            foreach (var s in splitEventData)
            {
                if (s != null && s.Length > 0)
                {
                    var values = s.Split(',');
                    if (values.Length >= 14)
                    {
                        DateTime dt;
                        if (DateTime.TryParseExact(values[13],
                                                    "yyyyMMddHHmmss.ffffff-fff",
                                                    System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                                    System.Globalization.DateTimeStyles.None
                                                    , out dt))
                        {
                            var localTime = System.TimeZoneInfo.ConvertTimeFromUtc(dt, System.TimeZoneInfo.Local);

                            list.Add(new EventTimeGeneratedData() { EventCode = values[5], TimeGenerated = localTime });
                        }
                    }

                }
            }

            // 日付でグルーピングする
            var dateTimeData = (from s in list
                                group s by string.Format("{0}/{1:00}/{2:00}", s.TimeGenerated.Year, s.TimeGenerated.Month, s.TimeGenerated.Day) into dtgroup
                                select dtgroup).OrderBy(x => x.Key);

            // 日ごとのデータを作成
            var dayDatasList = new List<EventDateTimeData>();
            if (dateTimeData != null)
            {
                // 日の開始・終了データは複数あるので、開始・終了でペアにしたリストを作成
                foreach (var s in dateTimeData)
                {
                    var dayDatas = s.OrderBy(x => x.TimeGenerated).Select(x => x);

                    EventDateTimeData eventDateTimeData = new EventDateTimeData()
                    {
                        Date = s.Key.ToString(),
                    };

                    // 開始・終了をペアにしたリスト
                    var dateList = new List<EventLogDataModel.EventDateTimeData.DateData>();

                    var dateData = new EventLogDataModel.EventDateTimeData.DateData();
                    // 全体のデータ作成
                    foreach (var d in dayDatas)
                    {
                        // 開始時間
                        if (d.EventCode.Equals("6005"))
                        {
                            // 開始時間
                            if (dateData.StartTime == null && dateData.EndTime == null)
                            {
                                dateData.StartTime = d.TimeGenerated;
                            }
                            else
                            {
                                dateList.Add(dateData);
                                dateData = new EventLogDataModel.EventDateTimeData.DateData();
                                dateData.StartTime = d.TimeGenerated;
                            }
                        }

                        // 終了時間
                        if (d.EventCode.Equals("6006"))
                        {
                            // 開始時間
                            if (dateData.EndTime == null)
                            {
                                dateData.EndTime = d.TimeGenerated;
                            }
                            else
                            {
                                dateList.Add(dateData);
                                dateData = new EventLogDataModel.EventDateTimeData.DateData();
                                dateData.EndTime = d.TimeGenerated;
                            }
                        }
                    }
                    // 最後のデータを追加
                    if (dateData.StartTime != null || dateData.EndTime != null)
                    {
                        dateList.Add(dateData);
                    }

                    // 全体のデータをセット
                    eventDateTimeData.DateList = dateList;

                    // 日ごとのデータリストに追加   
                    dayDatasList.Add(eventDateTimeData);
                }

                // 起床時間のしきい値取得
                var time = (ConfigurationManager.AppSettings["TimeBorder"] ?? "5:0").Split(':');

                // 開始・終了時間を判定してセットする
                foreach (var s in dayDatasList)
                {
                    // 当日
                    var date = DateTime.Parse(s.Date);

                    // 開始時間をセット
                    s.StartTime = s.DateList.Where(x => x.StartTime != null).Select(x => x.StartTime).FirstOrDefault();

                    //　起動時間が複数ある場合は、起床時間以降の最初の起動時間
                    if (s.DateList.Where(x => x.StartTime != null).Count() > 1)
                    {
                        // 起床時間より遅い開始時間を取得する
                        var timeBorder = new DateTime(date.Year, date.Month, date.Day, int.Parse(time[0]), int.Parse(time[1]), 0);

                        s.StartTime = s.DateList.Where(x => x.StartTime >= timeBorder).Select(x => x.StartTime).FirstOrDefault();
                    }

                    // 終了時間をセット
                    s.EndTime = s.DateList.LastOrDefault().EndTime;

                    // 開始時間はあるが、終了時間がない場合は翌日以降に終了したと思われる。
                    if (s.EndTime == null)
                    {
                        // 翌日以降で終了時間の入っている日のデータを取得する
                        var nextDayData = dayDatasList.Where(x => DateTime.Parse(x.Date) > date && x.DateList.Where(y => y.EndTime != null).Any()).FirstOrDefault();

                        if (nextDayData != null)
                        {
                            var datas = nextDayData.DateList;

                            // 日付
                            var nextDate = DateTime.Parse(nextDayData.Date);

                            // 起床時間より早い終了時間を取得する（何度も再起動した場合は、最初の終了時間が寝た時間ではないので・・・）
                            var timeBorder = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, int.Parse(time[0]), int.Parse(time[1]), 0);

                            var endTimeBeforStartTime = datas.Where(x => x.EndTime <= timeBorder).Select(x => x.EndTime).FirstOrDefault();

                            if (endTimeBeforStartTime != null)
                            {
                                s.EndTime = endTimeBeforStartTime;
                                s.EndTimeNextDay = true;
                            }
                        }
                    }

                }
            }

            return dayDatasList;
        }

    }
}
