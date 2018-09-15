using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HttpBomb.OptionValidators;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;

namespace HttpBomb
{
    class Program
    {
        const int ONE_SEC_MS = 1000;

        static void Main(string[] args)
        {
            var app = new CommandLineApplication(){
                Name = "HttpBomb",
                Description = "Http bombarding tool. Show no mercy."
            };
            app.HelpOption();

            var threadsOption = app.Option<int>("-n|--number <N>", "Required. Number of threads to run", CommandOptionType.SingleValue);
            threadsOption.Validators.Add(new ThreadValidator());

            var durationOption = app.Option<int>("-t|--time <SECONDS>", "Required. Time to run in seconds", CommandOptionType.SingleValue);
            durationOption.Validators.Add(new DurationValidator());

            var urlOption = app.Option("-u|--url <URL>", "Required. Url to hit", CommandOptionType.SingleValue);
            urlOption.Validators.Add(new UrlValidator());
 
            app.OnExecute(Execute(threadsOption.ParsedValue, durationOption.ParsedValue, urlOption.Value()));

            app.Execute(args);
        }

        static Action Execute(int threads, int duration, string url) => () => 
        {
            long successCount = 0;
            long failCount = 0;

            void incrementSuccess() => Interlocked.Increment(ref successCount);
            void incrementFail() => Interlocked.Increment(ref failCount);

            long getSuccessCount() => successCount;
            long getFailCount() => failCount;

            Enumerable.Range(0, threads).ToList().ForEach(_ => Task.Run(() => MainThread(url, incrementSuccess, incrementFail)));
            Task.Run(() => CounterThread(getSuccessCount, getFailCount));
            Thread.Sleep(duration * ONE_SEC_MS);
        };


        static Task MainThread(string url, Action onFail, Action onSuccess)
        {
            var client = new HttpClient{Timeout = TimeSpan.FromSeconds(3)};
            while (true)
                client.GetAsync(url).ContinueWith(t => {
                    if (t.IsCanceled || t.Result.StatusCode != HttpStatusCode.OK)
                        onFail();
                    else
                        onSuccess();
                });
        }

        static void CounterThread(Func<long> getSuccessCount, Func<long> getFailCount)
        {
            long lastSuccessCount = 0;
            double lastSwTime = 0;

            var sw = Stopwatch.StartNew();
          
            while(true){
                Thread.Sleep(ONE_SEC_MS);

                var swTime = ((double)sw.Elapsed.TotalMilliseconds/ONE_SEC_MS);
                var successRate  = (getSuccessCount() - lastSuccessCount) / (swTime - lastSwTime);
                lastSuccessCount = getSuccessCount();
                lastSwTime = swTime;
                
                Console.WriteLine($"Success count: {getSuccessCount()}, Fail count: {getFailCount()}, Success/sec: {successRate} Req/sec");
            }
        }
    }
}
