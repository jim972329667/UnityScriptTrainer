using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ScriptTrainer
{
    public class ZGCustomBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            // 在这里，你可以根据typeName来决定如何绑定到类型
            // 例如，你可以映射到你自己的assembly中的类型
            if (typeName == "CustomizationMod.PresetData")
            {
                return typeof(PresetData);
            }
            // 对于其他类型，你可以选择返回null，或者映射到其他类型
            return null;
        }
    }
    public class GameSkin
    {
        public string FileName {  get; set; }
        public PresetData PresetData {  get; set; } = new PresetData();
        public Texture2D ClothSkin {  get; set; }
        public Texture2D[] BodyTexture { get; set; } = new Texture2D[4];
        public GameObject CrownModel {  get; set; }
        public bool IsHaveBody {  get; set; } = false;
    }
    
    internal static class StreamConverter
    {
        private static readonly byte[] savedKey = { 0x7A, 0x67, 0x77, 0x64, 0x7A, 0x67, 0x7A, 0x73, 0x7A, 0x67, 0x62, 0x66, 0x61, 0x73, 0x64, 0x66 };
        public static byte[] StreamToByteArray(this Stream input)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                input.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        public static void writeFile()
        {
            FileStream dataStream;
            Aes iAes = Aes.Create();
            dataStream = new FileStream(Paths.PluginPath + "/Mario-Set.eskin", FileMode.Create);
            byte[] inputIV = iAes.IV;
            dataStream.Write(inputIV, 0, inputIV.Length);
            CryptoStream iStream = new CryptoStream(dataStream, iAes.CreateEncryptor(savedKey, iAes.IV), CryptoStreamMode.Write);

            var dataStream2 = new FileStream(Paths.PluginPath + "/Mario-Set.zip", FileMode.Open);
            dataStream2.Position = 0;
            dataStream2.CopyTo(iStream);

            dataStream2.Close();
            iStream.Close();
            dataStream.Close();
        }
        public static PresetData GetPresetData(this Stream stream)
        {
            PresetData presetData = new PresetData();

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Binder = new ZGCustomBinder();
            PresetData presetData2 = (PresetData)binaryFormatter.Deserialize(stream);
            stream.Close();
            if (presetData2.version == 1)
            {
                presetData.hideCloth = presetData2.hideCloth;
                presetData.skinColor = presetData2.skinColor;
            }
            else if (presetData2.version == 2)
            {
                presetData.hideCloth = presetData2.hideCloth;
                presetData.skinColor = presetData2.skinColor;
                presetData.clothSkin = presetData2.clothSkin;
            }
            else if (presetData2.version == 3)
            {
                presetData.presetName = presetData2.presetName;
                presetData.skinColor = presetData2.skinColor;
                presetData.bodyTexture = presetData2.bodyTexture;
                presetData.hideCloth = presetData2.hideCloth;
                presetData.clothSkin = presetData2.clothSkin;
                presetData.crownModel = presetData2.crownModel;
                presetData.crownPositionX = presetData2.crownPositionX;
                presetData.crownPositionY = presetData2.crownPositionY;
                presetData.crownPositionZ = presetData2.crownPositionZ;
                presetData.crownRotationX = presetData2.crownRotationX;
                presetData.crownRotationY = presetData2.crownRotationY;
                presetData.crownRotationZ = presetData2.crownRotationZ;
                presetData.crownScale = presetData2.crownScale;
            }
            else if (presetData2.version == 4)
            {
                presetData.presetName = presetData2.presetName;
                presetData.skinColor = presetData2.skinColor;
                presetData.bodyTexture = presetData2.bodyTexture;
                presetData.hideCloth = presetData2.hideCloth;
                presetData.clothSkin = presetData2.clothSkin;
                presetData.crownModel = presetData2.crownModel;
                presetData.crownPositionX = presetData2.crownPositionX;
                presetData.crownPositionY = presetData2.crownPositionY;
                presetData.crownPositionZ = presetData2.crownPositionZ;
                presetData.crownRotationX = presetData2.crownRotationX;
                presetData.crownRotationY = presetData2.crownRotationY;
                presetData.crownRotationZ = presetData2.crownRotationZ;
                presetData.crownScale = presetData2.crownScale;
                presetData.bodySmoothness = presetData2.bodySmoothness;
            }
            return presetData;
        }
        private static Texture2D LoadTexture(this Stream stream, bool isLinear = false)
        {
            byte[] data = stream.StreamToByteArray();
            Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, -1, isLinear);
            texture2D.LoadImage(data);
            return texture2D;
        }
        public static GameSkin ReadSkinFile(this string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    FileStream dataStream;
                    dataStream = new FileStream(file, FileMode.Open);
                    Aes oAes = Aes.Create();
                    byte[] outputIV = new byte[oAes.IV.Length];
                    dataStream.Read(outputIV, 0, outputIV.Length);
                    CryptoStream oStream = new CryptoStream(dataStream, oAes.CreateDecryptor(savedKey, outputIV), CryptoStreamMode.Read);

                    Stream decryptedStream = new MemoryStream();
                    oStream.CopyTo(decryptedStream);



                    GameSkin skin = new GameSkin
                    {
                        FileName = new FileInfo(file).Name
                    };
                    Stream objstream = null;
                    Stream mtlstream = null;
                    MTLLoader mtlloader = new MTLLoader();

                    using (var zipArchive = new ZipArchive(decryptedStream, ZipArchiveMode.Read, true))
                    {
                        foreach (var entry in zipArchive.Entries)
                        {
                            if (entry.Name.EndsWith(".preset", StringComparison.CurrentCultureIgnoreCase))
                            {
                                skin.PresetData = entry.Open().GetPresetData();
                            }
                            else
                            {
                                string Name = entry.FullName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(" ", "").ToLower();
                                if (Name == "clothskin" || Name == "clothskins")
                                {
                                    skin.ClothSkin = entry.Open().LoadTexture();
                                    skin.ClothSkin.FlipTexture();
                                }
                                if (Name == "bodytexture" || Name == "bodytextures")
                                {
                                    skin.IsHaveBody = true;
                                    if (entry.Name.Contains("_Normal"))
                                    {
                                        skin.BodyTexture[1] = entry.Open().LoadTexture(true);
                                    }
                                    else if (entry.Name.Contains("_Roughness"))
                                    {
                                        skin.BodyTexture[2] = entry.Open().LoadTexture(true);
                                    }
                                    else if (entry.Name.Contains("_Occlusion"))
                                    {
                                        skin.BodyTexture[3] = entry.Open().LoadTexture();
                                    }
                                    else
                                    {
                                        skin.BodyTexture[0] = entry.Open().LoadTexture();
                                    }
                                }

                                if (Name == "crownmodel" || Name == "crownmodels")
                                {
                                    if (entry.Name.EndsWith(".obj", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        objstream = entry.Open();
                                    }
                                    else if (entry.Name.EndsWith(".mtl", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        mtlstream = entry.Open();
                                    }
                                    else
                                    {
                                        mtlloader.Dic.Add(entry.Name, entry.Open());
                                    }
                                }
                            }
                            

                        }

                        if (objstream != null && mtlstream != null)
                        {
                            var tmp = new OBJLoader
                            {
                                Materials = mtlloader.Load(mtlstream)
                            };
                            skin.CrownModel = tmp.Load(objstream);
                        }
                    }
                    decryptedStream.Close();
                    return skin;
                }
                return null;
            }

            catch (Exception ex) 
            {
                ScriptTrainer.Instance.Log(ex, LogType.Error);
                return null; 
            }
        }
        private static void FlipTexture(this Texture2D original)
        {
            Color[] pixels = original.GetPixels();
            Color[] array = new Color[pixels.Length];
            int width = original.width;
            int height = original.height;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int num = height - j - 1;
                    int num2 = width - i - 1;
                    array[num2 + num * width] = pixels[i + j * width];
                }
            }
            original.SetPixels(array);
            original.Apply();
        }
    }
}
