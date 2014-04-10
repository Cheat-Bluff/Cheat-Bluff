using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using GoPro.Hero.Hero3;
using System.Threading;
using GoPro.Hero;
using GoPro.Hero.Browser.Media;

using System.IO;

namespace FaceTrackingBasics
{
    class GoProController
    {

        private Hero3Camera camera;

        public void Initialize()
        {
            //Inititalize GoPro
            Configuration.CommandRequestMode = Configuration.HttpRequestMode.Sync;

            Console.WriteLine("Gopro Video Record Sample");
            camera = Camera.Create<Hero3Camera>("10.5.5.9"); //address of the camera, default is 10.5.5.9

            PowerUp(camera);
        }

        public void StartRecording()
        {
            bool openShutter = false;
            camera.Shutter(openShutter);

            if (!openShutter)
            {

                camera
                    .Mode(Mode.Video)
                    .OpenShutter();

            }
        }

        public void StopRecording(String recordingDate)
        {
            camera.CloseShutter();

            // Create the thread object. This does not start the thread.
            GoProDownloader downloader = new GoProDownloader(camera, recordingDate);
            Thread downloaderThread = new Thread(downloader.Download);

            // Start the worker thread.
            downloaderThread.Start();
            Console.WriteLine("main thread: Starting worker thread...");

            // Loop until worker thread activates. 
            while (!downloaderThread.IsAlive)
            {
                ;
            }


            downloaderThread.Join();

        }

        private void PowerUp(Hero3Camera camera)
        {
            var powerOn = false;
            camera.Power(out powerOn);

            if (!powerOn)
            {
                Console.WriteLine("Powering Up Camera...");
                camera.PowerOn();
                Thread.Sleep(5000); //waiting for camera to get ready
            }
        }

        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 1024;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }

    }
}
