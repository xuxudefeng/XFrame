
using System.Runtime.InteropServices;

public class WebPlayer
{
    [DllImport("__Internal")]
    private static extern void Play(string url);

    public static void PlayVedio(string url)
    {
        Play(url);
    }
}







