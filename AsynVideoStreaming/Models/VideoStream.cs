using System.Net;

namespace AsynVideoStreaming.Models;

public class VideoStream
{
    private readonly string _filename;
    private long _contentLength;

    public long FileLength => _contentLength;

    public VideoStream(string videoPath)
    {
        _filename = videoPath;
        //FileShare.Read
        //باعث می‌شود فایل مورد نظر هنگام باز شدن قفل نشود و برای پروسه‌های دیگر قابل دسترسی باشد.
        using (var video = File.Open(videoPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            _contentLength = video.Length;
        }
    }

    public async void WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
    {
        try
        {
            var buffer = new byte[65536];// 64KB
            using (var video = File.Open(_filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var length = (int)video.Length;
                var bytesRead = 1;

                while (length > 0 && bytesRead>0)
                {
                    bytesRead = video.Read(buffer, 0, Math.Min(length, buffer.Length));
                    await outputStream.WriteAsync(buffer, 0, bytesRead);
                    length -= bytesRead;
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            outputStream.Close();
        }
    }
}
