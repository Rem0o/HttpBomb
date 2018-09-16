# HttpBomb

### Usage
```
Usage: HttpBomb [options]

Options:
  -?|-h|--help             Show help information
  -n|--number <N>          [Required] Number of threads to run
  -d|--duration <SECONDS>  [Required] Time to run in seconds
  -u|--url <URL>           [Required] Url to hit
  -t|--timeout <SECONDS>   [Optional] Http client timeout in seconds
```

### Recommendation
Start using a single thread and note the average rate this thread can make.

```
dotnet run -- -n 1 -u "http://url.com/" -d 3

(...)

Result: Total success: 3000, total fail: 0, average success/second 1000
```
In this exemple, we notice 1000 req/sec. Simply increase the number of threads until the rate per thread starts to decrease significantly, or until you get either CPU or IO limited. This number of thread should be your sweet spot to get the maximum rate possible from a single node to the targeted server. Of course, do not run this tool using the same node the targeted server is running on, your results won't reflect the actual maximum server performance.
