using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML;
using SFML.Graphics;
using SFML.Window;

using System.IO;
using System.Reflection;

namespace TrainBox
{
    static class TextureMan
    {
        static Assembly assembly = Assembly.GetExecutingAssembly();

        static Dictionary<string, Texture> textures;

        public static bool CreateShadows()
        {

            Image img = spriteSheet.Texture.CopyToImage();

            for (uint k = 0; k < img.Size.X; k++)
                for (uint j = 0; j < img.Size.Y; j++)
                {
                    Color c = img.GetPixel(k, j);
                    if (c.A == 255)
                    {
                        Color d = new Color();
                        d.A = 40;
                        d.R = d.G = d.B = 0;
                        img.SetPixel(k, j, d);
                    }
                }

            Texture tex = new Texture(img);
            Sprite tempsprite = new Sprite();
            tempsprite.Texture = tex;

            shadowSpriteSheet = new RenderTexture(textureSize, textureSize);
            shadowSpriteSheet.Draw(tempsprite);
            shadowSpriteSheet.Display();

            shadowSprite.Texture = shadowSpriteSheet.Texture;

            img.Dispose();
            tempsprite.Dispose();
            tex.Dispose();

            return true;
        }

        static RenderTexture spriteSheet;
        static RenderTexture shadowSpriteSheet;
        static Dictionary<string, Vector2i> spriteSheetLocation;

        static Sprite sprite = new Sprite();
        static Sprite shadowSprite = new Sprite();

        static uint textureSize = 512;

        static Font font;

        public static Font Font { get { return font; } }

        public static Sprite GetSprite(string name, bool shadow)
        {
            Vector2i location;
            spriteSheetLocation.TryGetValue(name, out location);

            IntRect rect;

            rect.Left = location.X;
            rect.Top = location.Y;
            rect.Width = 16;
            rect.Height = 16;

            if (!shadow)
            {
                sprite.TextureRect = rect;

                return sprite;
            }
            else
            {
                shadowSprite.TextureRect = rect;

                return shadowSprite;
            }
        }

        public static bool LoadTextures()
        {

            textures = new Dictionary<string, Texture>();

            spriteSheet = new RenderTexture(textureSize, textureSize);
            spriteSheetLocation = new Dictionary<string,Vector2i>();

            spriteSheet.Clear(new Color(0, 0, 0, 0));

            font = new Font(assembly.GetManifestResourceStream("TrainBox.8bit16.ttf"));

            textures.Add("player", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.player.png")));
            textures.Add("playerwalk1", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.playerwalk1.png")));
            textures.Add("playerwalk2", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.playerwalk2.png")));

            textures.Add("block", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.block.png")));
            textures.Add("box", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.box.png")));

            textures.Add("cranebottom", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.crane1.png")));
            textures.Add("cranesegment", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.crane2.png")));
            textures.Add("cranehook", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.crane3.png")));

            textures.Add("ladderbottom", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.ladder1.png")));
            textures.Add("laddersegment", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.ladder2.png")));

            textures.Add("conv1", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.conv1.png")));
            textures.Add("conv2", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.conv2.png")));
            textures.Add("convrail", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.conv3.png")));

            textures.Add("wagon", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.wagon.png")));

            textures.Add("key", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.key.png")));
            textures.Add("keylock", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.keylock.png")));
            textures.Add("keyblock", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.keyblock.png")));

            textures.Add("dynamite", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.dynamite.png")));
            textures.Add("detonator", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.detonatorbox.png")));
            textures.Add("detonatorhandle", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.detonatorhandle.png")));

            textures.Add("infobox", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.infobox.png")));

            textures.Add("star", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.star.png")));
            textures.Add("star2", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.star2.png")));
            textures.Add("star3", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.star3.png")));

            textures.Add("starblock", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.starblock.png")));
            textures.Add("starblock2", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.starblock2.png")));

            textures.Add("exit", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.exit.png")));
            textures.Add("exit1", new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.exit1.png")));

            float x = 0;
            float y = 0;

            Sprite temp = new Sprite();

            foreach (KeyValuePair<string,Texture> a in textures )
            {
                temp.Position = new Vector2f(x, y);
                temp.Texture = a.Value;
                spriteSheet.Draw(temp);

                spriteSheetLocation.Add(a.Key, new Vector2i((int)x,(int)y));

                x += 16;

                if (x > textureSize - 16)
                {
                    x = 0;
                    y += 16;
                }
              
            }

            spriteSheet.Display();
            sprite.Texture = spriteSheet.Texture;

            textures = null;

            return true;
        }
    }
}
