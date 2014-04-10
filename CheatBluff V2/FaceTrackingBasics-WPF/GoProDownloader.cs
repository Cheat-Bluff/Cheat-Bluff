using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GoPro.Hero.Hero3;
using System.Threading;
using GoPro.Hero.Browser.Media;

using System.IO;
using System.Net;

namespace FaceTrackingBasics
{
    class GoProDownloader
    {
        private volatile Hero3Camera camera;
        private String filename;
        public volatile bool successful = true;

        public GoProDownloader(Hero3Camera camera, String filename)
        {
            this.camera = camera;
            this.filename = filename;
        }

        public void Download()
        {
            try
            {
                Video video = camera.Contents().VideosAsync().Result.LastOrDefault();

                if (video == null)
                {
                    Console.WriteLine("no video found");
                    return;
                }

                Thread.Sleep(1000);

                FileStream writeStream = new FileStream(filename + "_gopro.mp4", FileMode.Create, FileAccess.Write);

                Stream response = video.DownloadAsync().Result.GetResponseStream();
                //write to the stream
                ReadWriteStream(response, writeStream);
            }
            catch (Exception ex)
            {
                successful = false;
            }
        }


        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int length = 1048576;
            Byte[] buffer = new Byte[length];

            int bytesRead = readStream.Read(buffer, 0, length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, length);
            }
            readStream.Close();
            writeStream.Close();
        }
    }

}
