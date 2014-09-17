using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Audio;
using SFML.Window;

using System.IO;
using System.Reflection;

namespace TrainBox
{
    static class SoundMan
    {
        static Assembly assembly = Assembly.GetExecutingAssembly();

        static Dictionary<string,SoundBuffer> sounds = new  Dictionary<string,SoundBuffer>();
        static Sound[] sound = new Sound[10];
        static Vector2f[] soundPos = new Vector2f[10];

        static Music music;
        static Music endingMusic;

        static public bool LoadSounds()
        {
            for (int i = 0; i < sound.Count(); i++) sound[i] = new Sound();

            sounds.Add("jump", new SoundBuffer(assembly.GetManifestResourceStream("TrainBox.snd.jump.wav")));
            sounds.Add("drop", new SoundBuffer(assembly.GetManifestResourceStream("TrainBox.snd.drop.wav")));
            sounds.Add("pickup", new SoundBuffer(assembly.GetManifestResourceStream("TrainBox.snd.pickup.wav")));
            sounds.Add("open", new SoundBuffer(assembly.GetManifestResourceStream("TrainBox.snd.open.wav")));
            sounds.Add("explode", new SoundBuffer(assembly.GetManifestResourceStream("TrainBox.snd.explode.wav")));
            sounds.Add("win", new SoundBuffer(assembly.GetManifestResourceStream("TrainBox.snd.win.wav")));

            music = new Music(assembly.GetManifestResourceStream("TrainBox.snd.m1.ogg"));
            endingMusic = new Music(assembly.GetManifestResourceStream("TrainBox.snd.m2.ogg"));

            return true;
        }

        static public Vector2f Center { get; set; }

        static public void PlayMusic(int songNum = 1)
        {

            if (songNum == 1)
            {
                music.Play();
                music.Volume = 20f;
                music.Loop = true;
            }
            else
            {
                music.Stop();

                endingMusic.Play();
                endingMusic.Volume = 20f;
                endingMusic.Loop = true;
            }

        }

        static public void PlaySound(string n, Vector2f location)
        {
            SoundBuffer s;
            sounds.TryGetValue(n, out s);


            for (int i = 0; i < sound.Count(); i++)
            {
                if (sound[i].Status == SoundStatus.Stopped)
                {
                    sound[i].SoundBuffer = s;
                    sound[i].Play();
                    sound[i].Position = new Vector3f((location.X - Center.X)/100, (location.Y - Center.Y)/100, 0);
                    break;
                }
            }
        }
    }
}
