namespace TrainBox
{
    using System;
    using System.Reflection;
    using System.IO;

    public static class ResourceExtractor
    {
        public static void ExtractResourceToFile(string resourceName, string filename)
        {
            if (!System.IO.File.Exists(filename))
                using (System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create))
                {
                    byte[] b = new byte[s.Length];
                    s.Read(b, 0, b.Length);
                    fs.Write(b, 0, b.Length);
                }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            ResourceExtractor.ExtractResourceToFile("TrainBox.sfmlnet-audio-2.dll", "sfmlnet-audio-2.dll");
            ResourceExtractor.ExtractResourceToFile("TrainBox.sfmlnet-graphics-2.dll", "sfmlnet-graphics-2.dll");
            ResourceExtractor.ExtractResourceToFile("TrainBox.sfmlnet-window-2.dll", "sfmlnet-window-2.dll");

            ResourceExtractor.ExtractResourceToFile("TrainBox.libsndfile-1.dll", "libsndfile-1.dll");
            ResourceExtractor.ExtractResourceToFile("TrainBox.openal32.dll", "openal32.dll");
            ResourceExtractor.ExtractResourceToFile("TrainBox.csfml-audio-2.dll", "csfml-audio-2.dll");
            ResourceExtractor.ExtractResourceToFile("TrainBox.csfml-graphics-2.dll", "csfml-graphics-2.dll");
            ResourceExtractor.ExtractResourceToFile("TrainBox.csfml-window-2.dll", "csfml-window-2.dll");

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            StartApp();

        }

        static void StartApp()
        {
            SoundMan.LoadSounds();

            

            TrainBoxApp app = new TrainBoxApp();
            app.StartTrainBoxApp();
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            
            String s = args.Name.Substring(0,args.Name.IndexOf(",")) + ".dll";
            Console.WriteLine(s);
            return Assembly.LoadFile(s);
        }
    }

}