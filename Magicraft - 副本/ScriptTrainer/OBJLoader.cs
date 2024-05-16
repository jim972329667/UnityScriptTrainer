using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using static ScriptTrainer.OBJObjectBuilder;
using static System.Net.Mime.MediaTypeNames;

namespace ScriptTrainer
{
    public static class StringExtensions
    {
        public static string Clean(this string str)
        {
            string text = str.Replace('\t', ' ');
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }
            return text.Trim();
        }
    }
    public class MTLLoader
    {
        public Dictionary<string, Stream> Dic = new Dictionary<string, Stream>();
        public virtual Texture2D TextureLoadFunction(string path, bool isNormalMap)
        {
            foreach (string text in this.SearchPaths)
            {
                string text2 = Path.Combine((this._objFileInfo != null) ? text.Replace("%FileName%", Path.GetFileNameWithoutExtension(this._objFileInfo.Name)) : text, path);

                if (Dic.ContainsKey(text2))
                {
                    Texture2D texture2D = ImageLoader.LoadTexture(text2, Dic[text2]);
                    if (isNormalMap)
                    {
                        texture2D = ImageUtils.ConvertToNormalMap(texture2D);
                    }
                    return texture2D;
                }
            }
            return null;
        }

        // Token: 0x06000004 RID: 4 RVA: 0x00002104 File Offset: 0x00000304
        private Texture2D TryLoadTexture(string texturePath, bool normalMap = false)
        {
            texturePath = texturePath.Replace('\\', Path.DirectorySeparatorChar);
            texturePath = texturePath.Replace('/', Path.DirectorySeparatorChar);
            return this.TextureLoadFunction(texturePath, normalMap);
        }

        // Token: 0x06000005 RID: 5 RVA: 0x0000212C File Offset: 0x0000032C
        private int GetArgValueCount(string arg)
        {
            if (arg != null)
            {
                switch (arg.Length)
                {
                    case 2:
                        {
                            char c = arg[1];
                            if (c != 'o')
                            {
                                if (c != 's')
                                {
                                    if (c != 't')
                                    {
                                        return -1;
                                    }
                                    if (!(arg == "-t"))
                                    {
                                        return -1;
                                    }
                                }
                                else if (!(arg == "-s"))
                                {
                                    return -1;
                                }
                            }
                            else if (!(arg == "-o"))
                            {
                                return -1;
                            }
                            return 3;
                        }
                    case 3:
                        {
                            char c = arg[1];
                            if (c != 'b')
                            {
                                if (c != 'm')
                                {
                                    return -1;
                                }
                                if (!(arg == "-mm"))
                                {
                                    return -1;
                                }
                                return 2;
                            }
                            else if (!(arg == "-bm"))
                            {
                                return -1;
                            }
                            break;
                        }
                    case 4:
                    case 5:
                        return -1;
                    case 6:
                        if (!(arg == "-clamp"))
                        {
                            return -1;
                        }
                        break;
                    case 7:
                        switch (arg[6])
                        {
                            case 's':
                                if (!(arg == "-texres"))
                                {
                                    return -1;
                                }
                                break;
                            case 't':
                                return -1;
                            case 'u':
                                if (!(arg == "-blendu"))
                                {
                                    return -1;
                                }
                                break;
                            case 'v':
                                if (!(arg == "-blendv"))
                                {
                                    return -1;
                                }
                                break;
                            default:
                                return -1;
                        }
                        break;
                    case 8:
                        if (!(arg == "-imfchan"))
                        {
                            return -1;
                        }
                        break;
                    default:
                        return -1;
                }
                return 1;
            }
            return -1;
        }

        // Token: 0x06000006 RID: 6 RVA: 0x00002274 File Offset: 0x00000474
        private int GetTexNameIndex(string[] components)
        {
            for (int i = 1; i < components.Length; i++)
            {
                int argValueCount = this.GetArgValueCount(components[i]);
                if (argValueCount < 0)
                {
                    return i;
                }
                i += argValueCount;
            }
            return -1;
        }

        // Token: 0x06000007 RID: 7 RVA: 0x000022A4 File Offset: 0x000004A4
        private float GetArgValue(string[] components, string arg, float fallback = 1f)
        {
            string a = arg.ToLower();
            for (int i = 1; i < components.Length - 1; i++)
            {
                string b = components[i].ToLower();
                if (a == b)
                {
                    return OBJLoaderHelper.FastFloatParse(components[i + 1]);
                }
            }
            return fallback;
        }

        // Token: 0x06000008 RID: 8 RVA: 0x000022E8 File Offset: 0x000004E8
        private string GetTexPathFromMapStatement(string processedLine, string[] splitLine)
        {
            int texNameIndex = this.GetTexNameIndex(splitLine);
            if (texNameIndex < 0)
            {
                Debug.LogError("texNameCmpIdx < 0 on line " + processedLine + ". Texture not loaded.");
                return null;
            }
            int startIndex = processedLine.IndexOf(splitLine[texNameIndex]);
            return processedLine.Substring(startIndex);
        }

        // Token: 0x06000009 RID: 9 RVA: 0x0000232C File Offset: 0x0000052C
        public Dictionary<string, Material> Load(Stream input)
        {
            StringReader stringReader = new StringReader(new StreamReader(input).ReadToEnd());
            Dictionary<string, Material> dictionary = new Dictionary<string, Material>();
            Material material = null;
            for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    string text2 = text.Clean();
                    string[] array = text2.Split(' ');
                    if (array.Length >= 2 && text2[0] != '#')
                    {
                        if (array[0] == "newmtl")
                        {
                            string text3 = text2.Substring(7);
                            Material material2 = new Material(Shader.Find("Standard (Specular setup)"))
                            {
                                name = text3
                            };
                            dictionary[text3] = material2;
                            material = material2;
                        }
                        else if (!(material == null))
                        {
                            if (array[0] == "Kd" || array[0] == "kd")
                            {
                                Color color = material.GetColor("_Color");
                                Color color2 = OBJLoaderHelper.ColorFromStrArray(array, 1f);
                                material.SetColor("_Color", new Color(color2.r, color2.g, color2.b, color.a));
                            }
                            else if (array[0] == "map_Kd" || array[0] == "map_kd")
                            {
                                string texPathFromMapStatement = this.GetTexPathFromMapStatement(text2, array);
                                if (texPathFromMapStatement != null)
                                {
                                    Texture2D texture2D = this.TryLoadTexture(texPathFromMapStatement, false);
                                    material.SetTexture("_MainTex", texture2D);
                                    if (texture2D != null && (texture2D.format == TextureFormat.DXT5 || texture2D.format == TextureFormat.ARGB32))
                                    {
                                        OBJLoaderHelper.EnableMaterialTransparency(material);
                                    }
                                    if (Path.GetExtension(texPathFromMapStatement).ToLower() == ".dds")
                                    {
                                        material.mainTextureScale = new Vector2(1f, -1f);
                                    }
                                }
                            }
                            else if (array[0] == "map_Bump" || array[0] == "map_bump")
                            {
                                string texPathFromMapStatement2 = this.GetTexPathFromMapStatement(text2, array);
                                if (texPathFromMapStatement2 != null)
                                {
                                    Texture2D texture2D2 = this.TryLoadTexture(texPathFromMapStatement2, true);
                                    float argValue = this.GetArgValue(array, "-bm", 1f);
                                    if (texture2D2 != null)
                                    {
                                        material.SetTexture("_BumpMap", texture2D2);
                                        material.SetFloat("_BumpScale", argValue);
                                        material.EnableKeyword("_NORMALMAP");
                                    }
                                }
                            }
                            else if (array[0] == "Ks" || array[0] == "ks")
                            {
                                material.SetColor("_SpecColor", OBJLoaderHelper.ColorFromStrArray(array, 1f));
                            }
                            else if (array[0] == "Ka" || array[0] == "ka")
                            {
                                material.SetColor("_EmissionColor", OBJLoaderHelper.ColorFromStrArray(array, 0.05f));
                                material.EnableKeyword("_EMISSION");
                            }
                            else if (array[0] == "map_Ka" || array[0] == "map_ka")
                            {
                                string texPathFromMapStatement3 = this.GetTexPathFromMapStatement(text2, array);
                                if (texPathFromMapStatement3 != null)
                                {
                                    material.SetTexture("_EmissionMap", this.TryLoadTexture(texPathFromMapStatement3, false));
                                }
                            }
                            else if (array[0] == "d" || array[0] == "Tr")
                            {
                                float num = OBJLoaderHelper.FastFloatParse(array[1]);
                                if (array[0] == "Tr")
                                {
                                    num = 1f - num;
                                }
                                if (num < 1f - Mathf.Epsilon)
                                {
                                    Color color3 = material.GetColor("_Color");
                                    color3.a = num;
                                    material.SetColor("_Color", color3);
                                    OBJLoaderHelper.EnableMaterialTransparency(material);
                                }
                            }
                            else if (array[0] == "Ns" || array[0] == "ns")
                            {
                                float num2 = OBJLoaderHelper.FastFloatParse(array[1]);
                                num2 /= 1000f;
                                material.SetFloat("_Glossiness", num2);
                            }
                        }
                    }
                }
            }
            return dictionary;
        }

        // Token: 0x0600000A RID: 10 RVA: 0x00002724 File Offset: 0x00000924
        public Dictionary<string, Material> Load(string path)
        {
            this._objFileInfo = new FileInfo(path);
            this.SearchPaths.Add(this._objFileInfo.Directory.FullName);
            Dictionary<string, Material> result;
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                result = this.Load(fileStream);
            }
            return result;
        }

        // Token: 0x04000002 RID: 2
        public List<string> SearchPaths = new List<string>
    {
        "%FileName%_Textures",
        string.Empty
    };

        // Token: 0x04000003 RID: 3
        private FileInfo _objFileInfo;
    }
    public class OBJLoader
    {


        // Token: 0x06000056 RID: 86 RVA: 0x00005DDC File Offset: 0x00003FDC
        private void LoadMaterialLibrary(string mtlLibPath)
        {
            if (this._objInfo != null && File.Exists(Path.Combine(this._objInfo.Directory.FullName, mtlLibPath)))
            {
                this.Materials = new MTLLoader().Load(Path.Combine(this._objInfo.Directory.FullName, mtlLibPath));
                return;
            }
            if (File.Exists(mtlLibPath))
            {
                this.Materials = new MTLLoader().Load(mtlLibPath);
                return;
            }
        }

        // Token: 0x06000057 RID: 87 RVA: 0x00005E50 File Offset: 0x00004050
        public GameObject Load(Stream input)
        {
            StreamReader reader = new StreamReader(input);
            Dictionary<string, OBJObjectBuilder> builderDict = new Dictionary<string, OBJObjectBuilder>();
            OBJObjectBuilder currentBuilder = null;
            string material = "default";
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            List<int> list3 = new List<int>();
            Action<string> action = delegate (string objectName)
            {
                if (!builderDict.TryGetValue(objectName, out currentBuilder))
                {
                    currentBuilder = new OBJObjectBuilder(objectName, this);
                    builderDict[objectName] = currentBuilder;
                }
            };
            action("default");
            CharWordReader charWordReader = new CharWordReader(reader, 4096);
            for (; ; )
            {
                charWordReader.SkipWhitespaces();
                if (charWordReader.endReached)
                {
                    break;
                }
                charWordReader.ReadUntilWhiteSpace();
                if (charWordReader.Is("#"))
                {
                    charWordReader.SkipUntilNewLine();
                }
                else if (this.Materials == null && charWordReader.Is("mtllib"))
                {
                    charWordReader.SkipWhitespaces();
                    charWordReader.ReadUntilNewLine();
                    string @string = charWordReader.GetString(0);
                    this.LoadMaterialLibrary(@string);
                }
                else if (charWordReader.Is("v"))
                {
                    this.Vertices.Add(charWordReader.ReadVector());
                }
                else if (charWordReader.Is("vn"))
                {
                    this.Normals.Add(charWordReader.ReadVector());
                }
                else if (charWordReader.Is("vt"))
                {
                    this.UVs.Add(charWordReader.ReadVector());
                }
                else if (charWordReader.Is("usemtl"))
                {
                    charWordReader.SkipWhitespaces();
                    charWordReader.ReadUntilNewLine();
                    string string2 = charWordReader.GetString(0);
                    material = string2;
                    if (this.SplitMode == SplitMode.Material)
                    {
                        action(string2);
                    }
                }
                else if ((charWordReader.Is("o") || charWordReader.Is("g")) && this.SplitMode == SplitMode.Object)
                {
                    charWordReader.ReadUntilNewLine();
                    string string3 = charWordReader.GetString(1);
                    action(string3);
                }
                else if (charWordReader.Is("f"))
                {
                    for (; ; )
                    {
                        bool flag;
                        charWordReader.SkipWhitespaces(out flag);
                        if (flag)
                        {
                            break;
                        }
                        int num = int.MinValue;
                        int num2 = int.MinValue;
                        int num3 = charWordReader.ReadInt();
                        if (charWordReader.currentChar == '/')
                        {
                            charWordReader.MoveNext();
                            if (charWordReader.currentChar != '/')
                            {
                                num2 = charWordReader.ReadInt();
                            }
                            if (charWordReader.currentChar == '/')
                            {
                                charWordReader.MoveNext();
                                num = charWordReader.ReadInt();
                            }
                        }
                        if (num3 > -2147483648)
                        {
                            if (num3 < 0)
                            {
                                num3 = this.Vertices.Count - num3;
                            }
                            num3--;
                        }
                        if (num > -2147483648)
                        {
                            if (num < 0)
                            {
                                num = this.Normals.Count - num;
                            }
                            num--;
                        }
                        if (num2 > -2147483648)
                        {
                            if (num2 < 0)
                            {
                                num2 = this.UVs.Count - num2;
                            }
                            num2--;
                        }
                        list.Add(num3);
                        list2.Add(num);
                        list3.Add(num2);
                    }
                    currentBuilder.PushFace(material, list, list2, list3);
                    list.Clear();
                    list2.Clear();
                    list3.Clear();
                }
                else
                {
                    charWordReader.SkipUntilNewLine();
                }
            }
            GameObject gameObject = new GameObject((this._objInfo != null) ? Path.GetFileNameWithoutExtension(this._objInfo.Name) : "WavefrontObject");
            gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
            foreach (KeyValuePair<string, OBJObjectBuilder> keyValuePair in builderDict)
            {
                if (keyValuePair.Value.PushedFaceCount != 0)
                {
                    keyValuePair.Value.Build().transform.SetParent(gameObject.transform, false);
                }
            }
            return gameObject;
        }

        // Token: 0x06000058 RID: 88 RVA: 0x00006220 File Offset: 0x00004420
        public GameObject Load(Stream input, Stream mtlInput)
        {
            MTLLoader mtlloader = new MTLLoader();
            this.Materials = mtlloader.Load(mtlInput);
            return this.Load(input);
        }

        // Token: 0x06000059 RID: 89 RVA: 0x00006248 File Offset: 0x00004448
        public GameObject Load(string path, string mtlPath)
        {
            this._objInfo = new FileInfo(path);
            if (!string.IsNullOrEmpty(mtlPath) && File.Exists(mtlPath))
            {
                MTLLoader mtlloader = new MTLLoader();
                this.Materials = mtlloader.Load(mtlPath);
                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    return this.Load(fileStream);
                }
            }
            GameObject result;
            using (FileStream fileStream2 = new FileStream(path, FileMode.Open))
            {
                result = this.Load(fileStream2);
            }
            return result;
        }

        // Token: 0x0600005A RID: 90 RVA: 0x000062DC File Offset: 0x000044DC
        public GameObject Load(string path)
        {
            return this.Load(path, null);
        }

        // Token: 0x0400005D RID: 93
        public SplitMode SplitMode = SplitMode.Object;

        // Token: 0x0400005E RID: 94
        internal List<Vector3> Vertices = new List<Vector3>();

        // Token: 0x0400005F RID: 95
        internal List<Vector3> Normals = new List<Vector3>();

        // Token: 0x04000060 RID: 96
        internal List<Vector2> UVs = new List<Vector2>();

        // Token: 0x04000061 RID: 97
        internal Dictionary<string, Material> Materials;

        // Token: 0x04000062 RID: 98
        private FileInfo _objInfo;
    }
    public class ImageLoader
    {
        // Token: 0x06000070 RID: 112 RVA: 0x00006C28 File Offset: 0x00004E28
        public static void SetNormalMap(ref Texture2D tex)
        {
            Color[] pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                Color color = pixels[i];
                color.r = pixels[i].g;
                color.a = pixels[i].r;
                pixels[i] = color;
            }
            tex.SetPixels(pixels);
            tex.Apply(true);
        }

        // Token: 0x06000071 RID: 113 RVA: 0x00006C94 File Offset: 0x00004E94
        public static Texture2D LoadTexture(Stream stream, ImageLoader.TextureFormat format)
        {
            if (format == ImageLoader.TextureFormat.BMP)
            {
                return new BMPLoader().LoadBMP(stream).ToTexture2D();
            }
            if (format == ImageLoader.TextureFormat.DDS)
            {
                return DDSLoader.Load(stream);
            }
            if (format == ImageLoader.TextureFormat.JPG || format == ImageLoader.TextureFormat.PNG)
            {
                byte[] array = new byte[stream.Length];
                stream.Read(array, 0, (int)stream.Length);
                Texture2D texture2D = new Texture2D(1, 1);
                texture2D.LoadImage(array);
                return texture2D;
            }
            if (format == ImageLoader.TextureFormat.TGA)
            {
                return TGALoader.Load(stream);
            }
            return null;
        }

        // Token: 0x06000072 RID: 114 RVA: 0x00006D04 File Offset: 0x00004F04
        public static Texture2D LoadTexture(string fn)
        {
            if (!File.Exists(fn))
            {
                return null;
            }
            byte[] array = File.ReadAllBytes(fn);
            string text = Path.GetExtension(fn).ToLower();
            string fileName = Path.GetFileName(fn);
            Texture2D texture2D = null;
            if (text != null)
            {
                int length = text.Length;
                if (length != 4)
                {
                    if (length != 5)
                    {
                        goto IL_20A;
                    }
                    if (!(text == ".jpeg"))
                    {
                        goto IL_20A;
                    }
                }
                else
                {
                    char c = text[1];
                    if (c <= 'j')
                    {
                        switch (c)
                        {
                            case 'b':
                                if (!(text == ".bmp"))
                                {
                                    goto IL_20A;
                                }
                                texture2D = new BMPLoader().LoadBMP(array).ToTexture2D();
                                goto IL_220;
                            case 'c':
                                {
                                    if (!(text == ".crn"))
                                    {
                                        goto IL_20A;
                                    }
                                    byte[] array2 = array;
                                    ushort width = BitConverter.ToUInt16(new byte[]
                                    {
                                array2[13],
                                array2[12]
                                    }, 0);
                                    ushort height = BitConverter.ToUInt16(new byte[]
                                    {
                                array2[15],
                                array2[14]
                                    }, 0);
                                    byte b = array2[18];
                                    UnityEngine.TextureFormat textureFormat;
                                    if (b == 0)
                                    {
                                        textureFormat = UnityEngine.TextureFormat.DXT1Crunched;
                                    }
                                    else if (b == 2)
                                    {
                                        textureFormat = UnityEngine.TextureFormat.DXT5Crunched;
                                    }
                                    else
                                    {
                                        if (b != 12)
                                        {
                                            Debug.LogError(string.Concat(new string[]
                                            {
                                        "Could not load crunched texture ",
                                        fileName,
                                        " because its format is not supported (",
                                        b.ToString(),
                                        "): ",
                                        fn
                                            }));
                                            goto IL_220;
                                        }
                                        textureFormat = UnityEngine.TextureFormat.ETC2_RGBA8Crunched;
                                    }
                                    texture2D = new Texture2D((int)width, (int)height, textureFormat, true);
                                    texture2D.LoadRawTextureData(array2);
                                    texture2D.Apply(true);
                                    goto IL_220;
                                }
                            case 'd':
                                if (!(text == ".dds"))
                                {
                                    goto IL_20A;
                                }
                                texture2D = DDSLoader.Load(array);
                                goto IL_220;
                            default:
                                if (c != 'j')
                                {
                                    goto IL_20A;
                                }
                                if (!(text == ".jpg"))
                                {
                                    goto IL_20A;
                                }
                                break;
                        }
                    }
                    else if (c != 'p')
                    {
                        if (c != 't')
                        {
                            goto IL_20A;
                        }
                        if (!(text == ".tga"))
                        {
                            goto IL_20A;
                        }
                        texture2D = TGALoader.Load(array);
                        goto IL_220;
                    }
                    else if (!(text == ".png"))
                    {
                        goto IL_20A;
                    }
                }
                texture2D = new Texture2D(1, 1);
                texture2D.LoadImage(array);
                goto IL_220;
            }
        IL_20A:
            Debug.LogError("Could not load texture " + fileName + " because its format is not supported : " + fn);
        IL_220:
            if (texture2D != null)
            {
                texture2D = ImageLoaderHelper.VerifyFormat(texture2D);
                texture2D.name = Path.GetFileNameWithoutExtension(fn);
            }
            return texture2D;
        }
        public static Texture2D LoadTexture(string fn, Stream stream)
        {
            byte[] array = stream.StreamToByteArray();
            string text = Path.GetExtension(fn).ToLower();
            string fileName = Path.GetFileName(fn);
            Texture2D texture2D = null;
            if (text != null)
            {
                int length = text.Length;
                if (length != 4)
                {
                    if (length != 5)
                    {
                        goto IL_20A;
                    }
                    if (!(text == ".jpeg"))
                    {
                        goto IL_20A;
                    }
                }
                else
                {
                    char c = text[1];
                    if (c <= 'j')
                    {
                        switch (c)
                        {
                            case 'b':
                                if (!(text == ".bmp"))
                                {
                                    goto IL_20A;
                                }
                                texture2D = new BMPLoader().LoadBMP(array).ToTexture2D();
                                goto IL_220;
                            case 'c':
                                {
                                    if (!(text == ".crn"))
                                    {
                                        goto IL_20A;
                                    }
                                    byte[] array2 = array;
                                    ushort width = BitConverter.ToUInt16(new byte[]
                                    {
                                array2[13],
                                array2[12]
                                    }, 0);
                                    ushort height = BitConverter.ToUInt16(new byte[]
                                    {
                                array2[15],
                                array2[14]
                                    }, 0);
                                    byte b = array2[18];
                                    UnityEngine.TextureFormat textureFormat;
                                    if (b == 0)
                                    {
                                        textureFormat = UnityEngine.TextureFormat.DXT1Crunched;
                                    }
                                    else if (b == 2)
                                    {
                                        textureFormat = UnityEngine.TextureFormat.DXT5Crunched;
                                    }
                                    else
                                    {
                                        if (b != 12)
                                        {
                                            Debug.LogError(string.Concat(new string[]
                                            {
                                        "Could not load crunched texture ",
                                        fileName,
                                        " because its format is not supported (",
                                        b.ToString(),
                                        "): ",
                                        fn
                                            }));
                                            goto IL_220;
                                        }
                                        textureFormat = UnityEngine.TextureFormat.ETC2_RGBA8Crunched;
                                    }
                                    texture2D = new Texture2D((int)width, (int)height, textureFormat, true);
                                    texture2D.LoadRawTextureData(array2);
                                    texture2D.Apply(true);
                                    goto IL_220;
                                }
                            case 'd':
                                if (!(text == ".dds"))
                                {
                                    goto IL_20A;
                                }
                                texture2D = DDSLoader.Load(array);
                                goto IL_220;
                            default:
                                if (c != 'j')
                                {
                                    goto IL_20A;
                                }
                                if (!(text == ".jpg"))
                                {
                                    goto IL_20A;
                                }
                                break;
                        }
                    }
                    else if (c != 'p')
                    {
                        if (c != 't')
                        {
                            goto IL_20A;
                        }
                        if (!(text == ".tga"))
                        {
                            goto IL_20A;
                        }
                        texture2D = TGALoader.Load(array);
                        goto IL_220;
                    }
                    else if (!(text == ".png"))
                    {
                        goto IL_20A;
                    }
                }
                texture2D = new Texture2D(1, 1);
                texture2D.LoadImage(array);
                goto IL_220;
            }
        IL_20A:
            Debug.LogError("Could not load texture " + fileName + " because its format is not supported : " + fn);
        IL_220:
            if (texture2D != null)
            {
                texture2D = ImageLoaderHelper.VerifyFormat(texture2D);
                texture2D.name = Path.GetFileNameWithoutExtension(fn);
            }
            return texture2D;
        }
        // Token: 0x02000020 RID: 32
        public enum TextureFormat
        {
            // Token: 0x04000079 RID: 121
            DDS,
            // Token: 0x0400007A RID: 122
            TGA,
            // Token: 0x0400007B RID: 123
            BMP,
            // Token: 0x0400007C RID: 124
            PNG,
            // Token: 0x0400007D RID: 125
            JPG,
            // Token: 0x0400007E RID: 126
            CRN
        }
    }
    public static class ImageUtils
    {
        // Token: 0x06000077 RID: 119 RVA: 0x00007140 File Offset: 0x00005340
        public static Texture2D ConvertToNormalMap(Texture2D tex)
        {
            Texture2D texture2D = tex;
            if (tex.format != TextureFormat.RGBA32 && tex.format != TextureFormat.ARGB32)
            {
                texture2D = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, true);
            }
            Color[] pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                Color color = pixels[i];
                color.a = pixels[i].r;
                color.r = 0f;
                color.g = pixels[i].g;
                color.b = 0f;
                pixels[i] = color;
            }
            texture2D.SetPixels(pixels);
            texture2D.Apply(true);
            return texture2D;
        }
    }
    public class ImageLoaderHelper
    {
        // Token: 0x06000074 RID: 116 RVA: 0x00006F58 File Offset: 0x00005158
        public static Texture2D VerifyFormat(Texture2D tex)
        {
            if (tex.format != TextureFormat.ARGB32 && tex.format != TextureFormat.RGBA32 && tex.format != TextureFormat.DXT5)
            {
                return tex;
            }
            Color32[] pixels = tex.GetPixels32();
            bool flag = false;
            Color32[] array = pixels;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].a < 255)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                Texture2D texture2D = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, tex.mipmapCount > 0);
                texture2D.SetPixels32(pixels);
                texture2D.Apply(true);
                return texture2D;
            }
            return tex;
        }

        // Token: 0x06000075 RID: 117 RVA: 0x00006FE4 File Offset: 0x000051E4
        public static void FillPixelArray(Color32[] fillArray, byte[] pixelData, int bytesPerPixel, bool bgra = false)
        {
            if (bgra)
            {
                if (bytesPerPixel == 4)
                {
                    for (int i = 0; i < fillArray.Length; i++)
                    {
                        int num = i * bytesPerPixel;
                        fillArray[i] = new Color32(pixelData[num + 2], pixelData[num + 1], pixelData[num], pixelData[num + 3]);
                    }
                    return;
                }
                for (int j = 0; j < fillArray.Length; j++)
                {
                    fillArray[j].r = pixelData[j * 3 + 2];
                    fillArray[j].g = pixelData[j * 3 + 1];
                    fillArray[j].b = pixelData[j * 3];
                }
                return;
            }
            else
            {
                if (bytesPerPixel == 4)
                {
                    for (int k = 0; k < fillArray.Length; k++)
                    {
                        fillArray[k].r = pixelData[k * 4];
                        fillArray[k].g = pixelData[k * 4 + 1];
                        fillArray[k].b = pixelData[k * 4 + 2];
                        fillArray[k].a = pixelData[k * 4 + 3];
                    }
                    return;
                }
                int num2 = 0;
                for (int l = 0; l < fillArray.Length; l++)
                {
                    fillArray[l].r = pixelData[num2++];
                    fillArray[l].g = pixelData[num2++];
                    fillArray[l].b = pixelData[num2++];
                    fillArray[l].a = byte.MaxValue;
                }
                return;
            }
        }
    }
    public static class DDSLoader
    {
        // Token: 0x0600006D RID: 109 RVA: 0x00006B10 File Offset: 0x00004D10
        public static Texture2D Load(Stream ddsStream)
        {
            byte[] array = new byte[ddsStream.Length];
            ddsStream.Read(array, 0, (int)ddsStream.Length);
            return DDSLoader.Load(array);
        }

        // Token: 0x0600006E RID: 110 RVA: 0x00006B40 File Offset: 0x00004D40
        public static Texture2D Load(string ddsPath)
        {
            return DDSLoader.Load(File.ReadAllBytes(ddsPath));
        }

        // Token: 0x0600006F RID: 111 RVA: 0x00006B50 File Offset: 0x00004D50
        public static Texture2D Load(byte[] ddsBytes)
        {
            Texture2D result;
            try
            {
                if (ddsBytes[4] != 124)
                {
                    throw new Exception("Invalid DDS header. Structure length is incrrrect.");
                }
                byte b = ddsBytes[87];
                if (b != 49 && b != 53)
                {
                    throw new Exception("Cannot load DDS due to an unsupported pixel format. Needs to be DXT1 or DXT5.");
                }
                int height = (int)ddsBytes[13] * 256 + (int)ddsBytes[12];
                int width = (int)ddsBytes[17] * 256 + (int)ddsBytes[16];
                bool mipChain = ddsBytes[28] > 0;
                TextureFormat textureFormat = (b == 49) ? TextureFormat.DXT1 : TextureFormat.DXT5;
                int num = 128;
                byte[] array = new byte[ddsBytes.Length - num];
                Buffer.BlockCopy(ddsBytes, num, array, 0, ddsBytes.Length - num);
                Texture2D texture2D = new Texture2D(width, height, textureFormat, mipChain);
                texture2D.LoadRawTextureData(array);
                texture2D.Apply();
                result = texture2D;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while loading DirectDraw Surface: " + ex.Message);
            }
            return result;
        }
    }
    public class OBJObjectBuilder
    {
        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000062 RID: 98 RVA: 0x00006550 File Offset: 0x00004750
        // (set) Token: 0x06000063 RID: 99 RVA: 0x00006558 File Offset: 0x00004758
        public int PushedFaceCount { get; private set; }

        public static class OBJLoaderHelper
        {
            // Token: 0x0600005C RID: 92 RVA: 0x00006318 File Offset: 0x00004518
            public static void EnableMaterialTransparency(Material mtl)
            {
                mtl.SetFloat("_Mode", 3f);
                mtl.SetInt("_SrcBlend", 5);
                mtl.SetInt("_DstBlend", 10);
                mtl.SetInt("_ZWrite", 0);
                mtl.DisableKeyword("_ALPHATEST_ON");
                mtl.EnableKeyword("_ALPHABLEND_ON");
                mtl.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mtl.renderQueue = 3000;
            }

            // Token: 0x0600005D RID: 93 RVA: 0x00006388 File Offset: 0x00004588
            public static float FastFloatParse(string input)
            {
                if (input.Contains("e") || input.Contains("E"))
                {
                    return float.Parse(input, CultureInfo.InvariantCulture);
                }
                float num = 0f;
                int i = 0;
                int length = input.Length;
                if (length == 0)
                {
                    return float.NaN;
                }
                char c = input[0];
                float num2 = 1f;
                if (c == '-')
                {
                    num2 = -1f;
                    i++;
                    if (i >= length)
                    {
                        return float.NaN;
                    }
                }
                while (i < length)
                {
                    c = input[i++];
                    if (c >= '0' && c <= '9')
                    {
                        num = num * 10f + (float)(c - '0');
                    }
                    else
                    {
                        if (c != '.' && c != ',')
                        {
                            return float.NaN;
                        }
                        float num3 = 0.1f;
                        while (i < length)
                        {
                            c = input[i++];
                            if (c < '0' || c > '9')
                            {
                                return float.NaN;
                            }
                            num += (float)(c - '0') * num3;
                            num3 *= 0.1f;
                        }
                        return num2 * num;
                    }
                }
                return num2 * num;
            }

            // Token: 0x0600005E RID: 94 RVA: 0x00006480 File Offset: 0x00004680
            public static int FastIntParse(string input)
            {
                int num = 0;
                bool flag = input[0] == '-';
                for (int i = flag ? 1 : 0; i < input.Length; i++)
                {
                    num = num * 10 + (int)(input[i] - '0');
                }
                if (!flag)
                {
                    return num;
                }
                return -num;
            }

            // Token: 0x0600005F RID: 95 RVA: 0x000064C6 File Offset: 0x000046C6
            public static Material CreateNullMaterial()
            {
                return new Material(Shader.Find("Standard (Specular setup)"));
            }

            // Token: 0x06000060 RID: 96 RVA: 0x000064D8 File Offset: 0x000046D8
            public static Vector3 VectorFromStrArray(string[] cmps)
            {
                float x = OBJLoaderHelper.FastFloatParse(cmps[1]);
                float y = OBJLoaderHelper.FastFloatParse(cmps[2]);
                if (cmps.Length == 4)
                {
                    float z = OBJLoaderHelper.FastFloatParse(cmps[3]);
                    return new Vector3(x, y, z);
                }
                return new Vector2(x, y);
            }

            // Token: 0x06000061 RID: 97 RVA: 0x0000651C File Offset: 0x0000471C
            public static Color ColorFromStrArray(string[] cmps, float scalar = 1f)
            {
                float r = OBJLoaderHelper.FastFloatParse(cmps[1]) * scalar;
                float g = OBJLoaderHelper.FastFloatParse(cmps[2]) * scalar;
                float b = OBJLoaderHelper.FastFloatParse(cmps[3]) * scalar;
                return new Color(r, g, b);
            }
        }
        // Token: 0x06000064 RID: 100 RVA: 0x00006564 File Offset: 0x00004764
        public GameObject Build()
        {
            GameObject gameObject = new GameObject(this._name);
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            int num = 0;
            Material[] array = new Material[this._materialIndices.Count];
            foreach (KeyValuePair<string, List<int>> keyValuePair in this._materialIndices)
            {
                Material material = null;
                if (this._loader.Materials == null)
                {
                    material = OBJLoaderHelper.CreateNullMaterial();
                    material.name = keyValuePair.Key;
                }
                else if (!this._loader.Materials.TryGetValue(keyValuePair.Key, out material))
                {
                    material = OBJLoaderHelper.CreateNullMaterial();
                    material.name = keyValuePair.Key;
                    this._loader.Materials[keyValuePair.Key] = material;
                }
                array[num] = material;
                num++;
            }
            meshRenderer.sharedMaterials = array;
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            num = 0;
            Mesh mesh = new Mesh
            {
                name = this._name,
                indexFormat = ((this._vertices.Count > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16),
                subMeshCount = this._materialIndices.Count
            };
            mesh.SetVertices(this._vertices);
            mesh.SetNormals(this._normals);
            mesh.SetUVs(0, this._uvs);
            foreach (KeyValuePair<string, List<int>> keyValuePair2 in this._materialIndices)
            {
                mesh.SetTriangles(keyValuePair2.Value, num);
                num++;
            }
            if (this.recalculateNormals)
            {
                mesh.RecalculateNormals();
            }
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
            meshFilter.sharedMesh = mesh;
            return gameObject;
        }

        // Token: 0x06000065 RID: 101 RVA: 0x00006748 File Offset: 0x00004948
        public void SetMaterial(string name)
        {
            if (!this._materialIndices.TryGetValue(name, out this._currentIndexList))
            {
                this._currentIndexList = new List<int>();
                this._materialIndices[name] = this._currentIndexList;
            }
        }

        // Token: 0x06000066 RID: 102 RVA: 0x0000677C File Offset: 0x0000497C
        public void PushFace(string material, List<int> vertexIndices, List<int> normalIndices, List<int> uvIndices)
        {
            if (vertexIndices.Count < 3)
            {
                return;
            }
            if (material != this._lastMaterial)
            {
                this.SetMaterial(material);
                this._lastMaterial = material;
            }
            int[] array = new int[vertexIndices.Count];
            for (int i = 0; i < vertexIndices.Count; i++)
            {
                int num = vertexIndices[i];
                int num2 = normalIndices[i];
                int num3 = uvIndices[i];
                OBJObjectBuilder.ObjLoopHash key = new OBJObjectBuilder.ObjLoopHash
                {
                    vertexIndex = num,
                    normalIndex = num2,
                    uvIndex = num3
                };
                int num4 = -1;
                if (!this._globalIndexRemap.TryGetValue(key, out num4))
                {
                    this._globalIndexRemap.Add(key, this._vertices.Count);
                    num4 = this._vertices.Count;
                    this._vertices.Add((num >= 0 && num < this._loader.Vertices.Count) ? this._loader.Vertices[num] : Vector3.zero);
                    this._normals.Add((num2 >= 0 && num2 < this._loader.Normals.Count) ? this._loader.Normals[num2] : Vector3.zero);
                    this._uvs.Add((num3 >= 0 && num3 < this._loader.UVs.Count) ? this._loader.UVs[num3] : Vector2.zero);
                    if (num2 < 0)
                    {
                        this.recalculateNormals = true;
                    }
                }
                array[i] = num4;
            }
            if (array.Length == 3)
            {
                this._currentIndexList.AddRange(new int[]
                {
                    array[0],
                    array[1],
                    array[2]
                });
            }
            else if (array.Length == 4)
            {
                this._currentIndexList.AddRange(new int[]
                {
                    array[0],
                    array[1],
                    array[2]
                });
                this._currentIndexList.AddRange(new int[]
                {
                    array[2],
                    array[3],
                    array[0]
                });
            }
            else if (array.Length > 4)
            {
                for (int j = array.Length - 1; j >= 2; j--)
                {
                    this._currentIndexList.AddRange(new int[]
                    {
                        array[0],
                        array[j - 1],
                        array[j]
                    });
                }
            }
            int pushedFaceCount = this.PushedFaceCount;
            this.PushedFaceCount = pushedFaceCount + 1;
        }

        // Token: 0x06000067 RID: 103 RVA: 0x000069D8 File Offset: 0x00004BD8
        public OBJObjectBuilder(string name, OBJLoader loader)
        {
            this._name = name;
            this._loader = loader;
        }

        // Token: 0x04000064 RID: 100
        private OBJLoader _loader;

        // Token: 0x04000065 RID: 101
        private string _name;

        // Token: 0x04000066 RID: 102
        private Dictionary<OBJObjectBuilder.ObjLoopHash, int> _globalIndexRemap = new Dictionary<OBJObjectBuilder.ObjLoopHash, int>();

        // Token: 0x04000067 RID: 103
        private Dictionary<string, List<int>> _materialIndices = new Dictionary<string, List<int>>();

        // Token: 0x04000068 RID: 104
        private List<int> _currentIndexList;

        // Token: 0x04000069 RID: 105
        private string _lastMaterial;

        // Token: 0x0400006A RID: 106
        private List<Vector3> _vertices = new List<Vector3>();

        // Token: 0x0400006B RID: 107
        private List<Vector3> _normals = new List<Vector3>();

        // Token: 0x0400006C RID: 108
        private List<Vector2> _uvs = new List<Vector2>();

        // Token: 0x0400006D RID: 109
        private bool recalculateNormals;

        // Token: 0x0200001F RID: 31
        private class ObjLoopHash
        {
            // Token: 0x0600008B RID: 139 RVA: 0x0000756C File Offset: 0x0000576C
            public override bool Equals(object obj)
            {
                if (!(obj is OBJObjectBuilder.ObjLoopHash))
                {
                    return false;
                }
                OBJObjectBuilder.ObjLoopHash objLoopHash = obj as OBJObjectBuilder.ObjLoopHash;
                return objLoopHash.vertexIndex == this.vertexIndex && objLoopHash.uvIndex == this.uvIndex && objLoopHash.normalIndex == this.normalIndex;
            }

            // Token: 0x0600008C RID: 140 RVA: 0x000075B6 File Offset: 0x000057B6
            public override int GetHashCode()
            {
                return ((3 * 314159 + this.vertexIndex) * 314159 + this.normalIndex) * 314159 + this.uvIndex;
            }

            // Token: 0x04000075 RID: 117
            public int vertexIndex;

            // Token: 0x04000076 RID: 118
            public int normalIndex;

            // Token: 0x04000077 RID: 119
            public int uvIndex;
        }
    }
    public enum SplitMode
    {
        None,
        Object,
        Material
    }
    public class CharWordReader
    {
        // Token: 0x06000048 RID: 72 RVA: 0x000059F3 File Offset: 0x00003BF3
        public CharWordReader(StreamReader reader, int bufferSize)
        {
            this.reader = reader;
            this.bufferSize = bufferSize;
            this.buffer = new char[this.bufferSize];
            this.word = new char[this.bufferSize];
            this.MoveNext();
        }

        // Token: 0x06000049 RID: 73 RVA: 0x00005A31 File Offset: 0x00003C31
        public void SkipWhitespaces()
        {
            while (char.IsWhiteSpace(this.currentChar))
            {
                this.MoveNext();
            }
        }

        // Token: 0x0600004A RID: 74 RVA: 0x00005A48 File Offset: 0x00003C48
        public void SkipWhitespaces(out bool newLinePassed)
        {
            newLinePassed = false;
            while (char.IsWhiteSpace(this.currentChar))
            {
                if (this.currentChar == '\r' || this.currentChar == '\n')
                {
                    newLinePassed = true;
                }
                this.MoveNext();
            }
        }

        // Token: 0x0600004B RID: 75 RVA: 0x00005A79 File Offset: 0x00003C79
        public void SkipUntilNewLine()
        {
            while (this.currentChar != '\0' && this.currentChar != '\n' && this.currentChar != '\r')
            {
                this.MoveNext();
            }
            this.SkipNewLineSymbols();
        }

        // Token: 0x0600004C RID: 76 RVA: 0x00005AA8 File Offset: 0x00003CA8
        public void ReadUntilWhiteSpace()
        {
            this.wordSize = 0;
            while (this.currentChar != '\0' && !char.IsWhiteSpace(this.currentChar))
            {
                this.word[this.wordSize] = this.currentChar;
                this.wordSize++;
                this.MoveNext();
            }
        }

        // Token: 0x0600004D RID: 77 RVA: 0x00005AFC File Offset: 0x00003CFC
        public void ReadUntilNewLine()
        {
            this.wordSize = 0;
            while (this.currentChar != '\0' && this.currentChar != '\n' && this.currentChar != '\r')
            {
                this.word[this.wordSize] = this.currentChar;
                this.wordSize++;
                this.MoveNext();
            }
            this.SkipNewLineSymbols();
        }

        // Token: 0x0600004E RID: 78 RVA: 0x00005B5C File Offset: 0x00003D5C
        public bool Is(string other)
        {
            if (other.Length != this.wordSize)
            {
                return false;
            }
            for (int i = 0; i < this.wordSize; i++)
            {
                if (this.word[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        // Token: 0x0600004F RID: 79 RVA: 0x00005B9E File Offset: 0x00003D9E
        public string GetString(int startIndex = 0)
        {
            if (startIndex >= this.wordSize - 1)
            {
                return string.Empty;
            }
            return new string(this.word, startIndex, this.wordSize - startIndex);
        }

        // Token: 0x06000050 RID: 80 RVA: 0x00005BC8 File Offset: 0x00003DC8
        public Vector3 ReadVector()
        {
            this.SkipWhitespaces();
            float x = this.ReadFloat();
            this.SkipWhitespaces();
            float y = this.ReadFloat();
            bool flag;
            this.SkipWhitespaces(out flag);
            float z = 0f;
            if (!flag)
            {
                z = this.ReadFloat();
            }
            return new Vector3(x, y, z);
        }

        // Token: 0x06000051 RID: 81 RVA: 0x00005C10 File Offset: 0x00003E10
        public int ReadInt()
        {
            int num = 0;
            bool flag = this.currentChar == '-';
            if (flag)
            {
                this.MoveNext();
            }
            while (this.currentChar >= '0' && this.currentChar <= '9')
            {
                int num2 = (int)(this.currentChar - '0');
                num = num * 10 + num2;
                this.MoveNext();
            }
            if (!flag)
            {
                return num;
            }
            return -num;
        }

        // Token: 0x06000052 RID: 82 RVA: 0x00005C68 File Offset: 0x00003E68
        public float ReadFloat()
        {
            bool flag = this.currentChar == '-';
            if (flag)
            {
                this.MoveNext();
            }
            float num = (float)this.ReadInt();
            if (this.currentChar == '.' || this.currentChar == ',')
            {
                this.MoveNext();
                num += this.ReadFloatEnd();
                if (this.currentChar == 'e' || this.currentChar == 'E')
                {
                    this.MoveNext();
                    int num2 = this.ReadInt();
                    num *= Mathf.Pow(10f, (float)num2);
                }
            }
            if (flag)
            {
                num = -num;
            }
            return num;
        }

        // Token: 0x06000053 RID: 83 RVA: 0x00005CEC File Offset: 0x00003EEC
        private float ReadFloatEnd()
        {
            float num = 0f;
            float num2 = 0.1f;
            while (this.currentChar >= '0' && this.currentChar <= '9')
            {
                int num3 = (int)(this.currentChar - '0');
                num += (float)num3 * num2;
                num2 *= 0.1f;
                this.MoveNext();
            }
            return num;
        }

        // Token: 0x06000054 RID: 84 RVA: 0x00005D3B File Offset: 0x00003F3B
        private void SkipNewLineSymbols()
        {
            while (this.currentChar == '\n' || this.currentChar == '\r')
            {
                this.MoveNext();
            }
        }

        // Token: 0x06000055 RID: 85 RVA: 0x00005D5C File Offset: 0x00003F5C
        public void MoveNext()
        {
            this.currentPosition++;
            if (this.currentPosition >= this.maxPosition)
            {
                if (this.reader.EndOfStream)
                {
                    this.currentChar = '\0';
                    this.endReached = true;
                    return;
                }
                this.currentPosition = 0;
                this.maxPosition = this.reader.Read(this.buffer, 0, this.bufferSize);
            }
            this.currentChar = this.buffer[this.currentPosition];
        }

        // Token: 0x04000050 RID: 80
        public char[] word;

        // Token: 0x04000051 RID: 81
        public int wordSize;

        // Token: 0x04000052 RID: 82
        public bool endReached;

        // Token: 0x04000053 RID: 83
        private StreamReader reader;

        // Token: 0x04000054 RID: 84
        private int bufferSize;

        // Token: 0x04000055 RID: 85
        private char[] buffer;

        // Token: 0x04000056 RID: 86
        public char currentChar;

        // Token: 0x04000057 RID: 87
        private int currentPosition;

        // Token: 0x04000058 RID: 88
        private int maxPosition;
    }
    public class TGALoader
    {
        // Token: 0x06000078 RID: 120 RVA: 0x000071E7 File Offset: 0x000053E7
        private static int GetBits(byte b, int offset, int count)
        {
            return b >> offset & (1 << count) - 1;
        }

        // Token: 0x06000079 RID: 121 RVA: 0x000071F8 File Offset: 0x000053F8
        private static Color32[] LoadRawTGAData(BinaryReader r, int bitDepth, int width, int height)
        {
            Color32[] array = new Color32[width * height];
            byte[] pixelData = r.ReadBytes(width * height * (bitDepth / 8));
            ImageLoaderHelper.FillPixelArray(array, pixelData, bitDepth / 8, true);
            return array;
        }

        // Token: 0x0600007A RID: 122 RVA: 0x00007228 File Offset: 0x00005428
        private static Color32[] LoadRLETGAData(BinaryReader r, int bitDepth, int width, int height)
        {
            Color32[] array = new Color32[width * height];
            int num;
            for (int i = 0; i < array.Length; i += num)
            {
                byte b = r.ReadByte();
                int bits = TGALoader.GetBits(b, 7, 1);
                num = TGALoader.GetBits(b, 0, 7) + 1;
                if (bits == 0)
                {
                    for (int j = 0; j < num; j++)
                    {
                        Color32 color = (bitDepth == 32) ? r.ReadColor32RGBA().FlipRB() : r.ReadColor32RGB().FlipRB();
                        array[j + i] = color;
                    }
                }
                else
                {
                    Color32 color2 = (bitDepth == 32) ? r.ReadColor32RGBA().FlipRB() : r.ReadColor32RGB().FlipRB();
                    for (int k = 0; k < num; k++)
                    {
                        array[k + i] = color2;
                    }
                }
            }
            return array;
        }

        // Token: 0x0600007B RID: 123 RVA: 0x000072E8 File Offset: 0x000054E8
        public static Texture2D Load(string fileName)
        {
            Texture2D result;
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                result = TGALoader.Load(fileStream);
            }
            return result;
        }

        // Token: 0x0600007C RID: 124 RVA: 0x00007320 File Offset: 0x00005520
        public static Texture2D Load(byte[] bytes)
        {
            Texture2D result;
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                result = TGALoader.Load(memoryStream);
            }
            return result;
        }

        // Token: 0x0600007D RID: 125 RVA: 0x00007358 File Offset: 0x00005558
        public static Texture2D Load(Stream TGAStream)
        {
            Texture2D result;
            using (BinaryReader binaryReader = new BinaryReader(TGAStream))
            {
                binaryReader.BaseStream.Seek(2L, SeekOrigin.Begin);
                byte b = binaryReader.ReadByte();
                if (b != 10 && b != 2)
                {
                    Debug.LogError(string.Format("Unsupported targa image type. ({0})", b));
                    result = null;
                }
                else
                {
                    binaryReader.BaseStream.Seek(12L, SeekOrigin.Begin);
                    short width = binaryReader.ReadInt16();
                    short height = binaryReader.ReadInt16();
                    int num = (int)binaryReader.ReadByte();
                    if (num < 24)
                    {
                        throw new Exception("Tried to load TGA with unsupported bit depth");
                    }
                    binaryReader.BaseStream.Seek(1L, SeekOrigin.Current);
                    Texture2D texture2D = new Texture2D((int)width, (int)height, (num == 24) ? TextureFormat.RGB24 : TextureFormat.ARGB32, true);
                    if (b == 2)
                    {
                        texture2D.SetPixels32(TGALoader.LoadRawTGAData(binaryReader, num, (int)width, (int)height));
                    }
                    else
                    {
                        texture2D.SetPixels32(TGALoader.LoadRLETGAData(binaryReader, num, (int)width, (int)height));
                    }
                    texture2D.Apply();
                    result = texture2D;
                }
            }
            return result;
        }
    }
    public static class BinaryExtensions
    {
        // Token: 0x06000069 RID: 105 RVA: 0x00006A70 File Offset: 0x00004C70
        public static Color32 ReadColor32RGBR(this BinaryReader r)
        {
            byte[] array = r.ReadBytes(4);
            return new Color32(array[0], array[1], array[2], byte.MaxValue);
        }

        // Token: 0x0600006A RID: 106 RVA: 0x00006A98 File Offset: 0x00004C98
        public static Color32 ReadColor32RGBA(this BinaryReader r)
        {
            byte[] array = r.ReadBytes(4);
            return new Color32(array[0], array[1], array[2], array[3]);
        }

        // Token: 0x0600006B RID: 107 RVA: 0x00006AC0 File Offset: 0x00004CC0
        public static Color32 ReadColor32RGB(this BinaryReader r)
        {
            byte[] array = r.ReadBytes(3);
            return new Color32(array[0], array[1], array[2], byte.MaxValue);
        }

        // Token: 0x0600006C RID: 108 RVA: 0x00006AE8 File Offset: 0x00004CE8
        public static Color32 ReadColor32BGR(this BinaryReader r)
        {
            byte[] array = r.ReadBytes(3);
            return new Color32(array[2], array[1], array[0], byte.MaxValue);
        }
    }
    public static class ColorExtensions
    {
        // Token: 0x0600007F RID: 127 RVA: 0x0000745C File Offset: 0x0000565C
        public static Color FlipRB(this Color color)
        {
            return new Color(color.b, color.g, color.r, color.a);
        }

        // Token: 0x06000080 RID: 128 RVA: 0x0000747B File Offset: 0x0000567B
        public static Color32 FlipRB(this Color32 color)
        {
            return new Color32(color.b, color.g, color.r, color.a);
        }
    }
    public class BMPImage
    {
        // Token: 0x06000031 RID: 49 RVA: 0x00004A2A File Offset: 0x00002C2A
        public Texture2D ToTexture2D()
        {
            Texture2D texture2D = new Texture2D(this.info.absWidth, this.info.absHeight);
            texture2D.SetPixels32(this.imageData);
            texture2D.Apply();
            return texture2D;
        }

        // Token: 0x04000042 RID: 66
        public BMPFileHeader header;

        // Token: 0x04000043 RID: 67
        public BitmapInfoHeader info;

        // Token: 0x04000044 RID: 68
        public uint rMask = 16711680U;

        // Token: 0x04000045 RID: 69
        public uint gMask = 65280U;

        // Token: 0x04000046 RID: 70
        public uint bMask = 255U;

        // Token: 0x04000047 RID: 71
        public uint aMask;

        // Token: 0x04000048 RID: 72
        public List<Color32> palette;

        // Token: 0x04000049 RID: 73
        public Color32[] imageData;
    }
    public class BMPLoader
    {
        // Token: 0x06000033 RID: 51 RVA: 0x00004A84 File Offset: 0x00002C84
        public BMPImage LoadBMP(string aFileName)
        {
            BMPImage result;
            using (FileStream fileStream = File.OpenRead(aFileName))
            {
                result = this.LoadBMP(fileStream);
            }
            return result;
        }

        // Token: 0x06000034 RID: 52 RVA: 0x00004AC0 File Offset: 0x00002CC0
        public BMPImage LoadBMP(byte[] aData)
        {
            BMPImage result;
            using (MemoryStream memoryStream = new MemoryStream(aData))
            {
                result = this.LoadBMP(memoryStream);
            }
            return result;
        }

        // Token: 0x06000035 RID: 53 RVA: 0x00004AFC File Offset: 0x00002CFC
        public BMPImage LoadBMP(Stream aData)
        {
            BMPImage result;
            using (BinaryReader binaryReader = new BinaryReader(aData))
            {
                result = this.LoadBMP(binaryReader);
            }
            return result;
        }

        // Token: 0x06000036 RID: 54 RVA: 0x00004B38 File Offset: 0x00002D38
        public BMPImage LoadBMP(BinaryReader aReader)
        {
            BMPImage bmpimage = new BMPImage();
            if (!BMPLoader.ReadFileHeader(aReader, ref bmpimage.header))
            {
                Debug.LogError("Not a BMP file");
                return null;
            }
            if (!BMPLoader.ReadInfoHeader(aReader, ref bmpimage.info))
            {
                Debug.LogError("Unsupported header format");
                return null;
            }
            if (bmpimage.info.compressionMethod != BMPComressionMode.BI_RGB && bmpimage.info.compressionMethod != BMPComressionMode.BI_BITFIELDS && bmpimage.info.compressionMethod != BMPComressionMode.BI_ALPHABITFIELDS && bmpimage.info.compressionMethod != BMPComressionMode.BI_RLE4 && bmpimage.info.compressionMethod != BMPComressionMode.BI_RLE8)
            {
                Debug.LogError("Unsupported image format: " + bmpimage.info.compressionMethod.ToString());
                return null;
            }
            long offset = (long)((ulong)(14U + bmpimage.info.size));
            aReader.BaseStream.Seek(offset, SeekOrigin.Begin);
            if (bmpimage.info.nBitsPerPixel < 24)
            {
                bmpimage.rMask = 31744U;
                bmpimage.gMask = 992U;
                bmpimage.bMask = 31U;
            }
            if (bmpimage.info.compressionMethod == BMPComressionMode.BI_BITFIELDS || bmpimage.info.compressionMethod == BMPComressionMode.BI_ALPHABITFIELDS)
            {
                bmpimage.rMask = aReader.ReadUInt32();
                bmpimage.gMask = aReader.ReadUInt32();
                bmpimage.bMask = aReader.ReadUInt32();
            }
            if (this.ForceAlphaReadWhenPossible)
            {
                bmpimage.aMask = (BMPLoader.GetMask((int)bmpimage.info.nBitsPerPixel) ^ (bmpimage.rMask | bmpimage.gMask | bmpimage.bMask));
            }
            if (bmpimage.info.compressionMethod == BMPComressionMode.BI_ALPHABITFIELDS)
            {
                bmpimage.aMask = aReader.ReadUInt32();
            }
            if (bmpimage.info.nPaletteColors > 0U || bmpimage.info.nBitsPerPixel <= 8)
            {
                bmpimage.palette = BMPLoader.ReadPalette(aReader, bmpimage, this.ReadPaletteAlpha || this.ForceAlphaReadWhenPossible);
            }
            aReader.BaseStream.Seek((long)((ulong)bmpimage.header.offset), SeekOrigin.Begin);
            bool flag = bmpimage.info.compressionMethod == BMPComressionMode.BI_RGB || bmpimage.info.compressionMethod == BMPComressionMode.BI_BITFIELDS || bmpimage.info.compressionMethod == BMPComressionMode.BI_ALPHABITFIELDS;
            if (bmpimage.info.nBitsPerPixel == 32 && flag)
            {
                BMPLoader.Read32BitImage(aReader, bmpimage);
            }
            else if (bmpimage.info.nBitsPerPixel == 24 && flag)
            {
                BMPLoader.Read24BitImage(aReader, bmpimage);
            }
            else if (bmpimage.info.nBitsPerPixel == 16 && flag)
            {
                BMPLoader.Read16BitImage(aReader, bmpimage);
            }
            else if (bmpimage.info.compressionMethod == BMPComressionMode.BI_RLE4 && bmpimage.info.nBitsPerPixel == 4 && bmpimage.palette != null)
            {
                BMPLoader.ReadIndexedImageRLE4(aReader, bmpimage);
            }
            else if (bmpimage.info.compressionMethod == BMPComressionMode.BI_RLE8 && bmpimage.info.nBitsPerPixel == 8 && bmpimage.palette != null)
            {
                BMPLoader.ReadIndexedImageRLE8(aReader, bmpimage);
            }
            else
            {
                if (!flag || bmpimage.info.nBitsPerPixel > 8 || bmpimage.palette == null)
                {
                    Debug.LogError("Unsupported file format: " + bmpimage.info.compressionMethod.ToString() + " BPP: " + bmpimage.info.nBitsPerPixel.ToString());
                    return null;
                }
                BMPLoader.ReadIndexedImage(aReader, bmpimage);
            }
            return bmpimage;
        }

        // Token: 0x06000037 RID: 55 RVA: 0x00004E5C File Offset: 0x0000305C
        private static void Read32BitImage(BinaryReader aReader, BMPImage bmp)
        {
            int num = Mathf.Abs(bmp.info.width);
            int num2 = Mathf.Abs(bmp.info.height);
            Color32[] array = bmp.imageData = new Color32[num * num2];
            if (aReader.BaseStream.Position + (long)(num * num2 * 4) > aReader.BaseStream.Length)
            {
                Debug.LogError("Unexpected end of file.");
                return;
            }
            int shiftCount = BMPLoader.GetShiftCount(bmp.rMask);
            int shiftCount2 = BMPLoader.GetShiftCount(bmp.gMask);
            int shiftCount3 = BMPLoader.GetShiftCount(bmp.bMask);
            int shiftCount4 = BMPLoader.GetShiftCount(bmp.aMask);
            byte a = byte.MaxValue;
            for (int i = 0; i < array.Length; i++)
            {
                uint num3 = aReader.ReadUInt32();
                byte r = (byte)((num3 & bmp.rMask) >> shiftCount);
                byte g = (byte)((num3 & bmp.gMask) >> shiftCount2);
                byte b = (byte)((num3 & bmp.bMask) >> shiftCount3);
                if (bmp.bMask != 0U)
                {
                    a = (byte)((num3 & bmp.aMask) >> shiftCount4);
                }
                array[i] = new Color32(r, g, b, a);
            }
        }

        // Token: 0x06000038 RID: 56 RVA: 0x00004F84 File Offset: 0x00003184
        private static void Read24BitImage(BinaryReader aReader, BMPImage bmp)
        {
            int num = Mathf.Abs(bmp.info.width);
            int num2 = Mathf.Abs(bmp.info.height);
            int num3 = (24 * num + 31) / 32 * 4;
            int num4 = num3 * num2;
            int num5 = num3 - num * 3;
            Color32[] array = bmp.imageData = new Color32[num * num2];
            if (aReader.BaseStream.Position + (long)num4 > aReader.BaseStream.Length)
            {
                Debug.LogError(string.Concat(new string[]
                {
                    "Unexpected end of file. (Have ",
                    (aReader.BaseStream.Position + (long)num4).ToString(),
                    " bytes, expected ",
                    aReader.BaseStream.Length.ToString(),
                    " bytes)"
                }));
                return;
            }
            int shiftCount = BMPLoader.GetShiftCount(bmp.rMask);
            int shiftCount2 = BMPLoader.GetShiftCount(bmp.gMask);
            int shiftCount3 = BMPLoader.GetShiftCount(bmp.bMask);
            for (int i = 0; i < num2; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    int num6 = (int)aReader.ReadByte() | (int)aReader.ReadByte() << 8 | (int)aReader.ReadByte() << 16;
                    byte r = (byte)((uint)(num6 & (int)bmp.rMask) >> shiftCount);
                    byte g = (byte)((uint)(num6 & (int)bmp.gMask) >> shiftCount2);
                    byte b = (byte)((uint)(num6 & (int)bmp.bMask) >> shiftCount3);
                    array[j + i * num] = new Color32(r, g, b, byte.MaxValue);
                }
                for (int k = 0; k < num5; k++)
                {
                    aReader.ReadByte();
                }
            }
        }

        // Token: 0x06000039 RID: 57 RVA: 0x00005120 File Offset: 0x00003320
        private static void Read16BitImage(BinaryReader aReader, BMPImage bmp)
        {
            int num = Mathf.Abs(bmp.info.width);
            int num2 = Mathf.Abs(bmp.info.height);
            int num3 = (16 * num + 31) / 32 * 4;
            int num4 = num3 * num2;
            int num5 = num3 - num * 2;
            Color32[] array = bmp.imageData = new Color32[num * num2];
            if (aReader.BaseStream.Position + (long)num4 > aReader.BaseStream.Length)
            {
                Debug.LogError(string.Concat(new string[]
                {
                    "Unexpected end of file. (Have ",
                    (aReader.BaseStream.Position + (long)num4).ToString(),
                    " bytes, expected ",
                    aReader.BaseStream.Length.ToString(),
                    " bytes)"
                }));
                return;
            }
            int shiftCount = BMPLoader.GetShiftCount(bmp.rMask);
            int shiftCount2 = BMPLoader.GetShiftCount(bmp.gMask);
            int shiftCount3 = BMPLoader.GetShiftCount(bmp.bMask);
            int shiftCount4 = BMPLoader.GetShiftCount(bmp.aMask);
            byte a = byte.MaxValue;
            for (int i = 0; i < num2; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    uint num6 = (uint)((int)aReader.ReadByte() | (int)aReader.ReadByte() << 8);
                    byte r = (byte)((num6 & bmp.rMask) >> shiftCount);
                    byte g = (byte)((num6 & bmp.gMask) >> shiftCount2);
                    byte b = (byte)((num6 & bmp.bMask) >> shiftCount3);
                    if (bmp.aMask != 0U)
                    {
                        a = (byte)((num6 & bmp.aMask) >> shiftCount4);
                    }
                    array[j + i * num] = new Color32(r, g, b, a);
                }
                for (int k = 0; k < num5; k++)
                {
                    aReader.ReadByte();
                }
            }
        }

        // Token: 0x0600003A RID: 58 RVA: 0x000052E8 File Offset: 0x000034E8
        private static void ReadIndexedImage(BinaryReader aReader, BMPImage bmp)
        {
            int num = Mathf.Abs(bmp.info.width);
            int num2 = Mathf.Abs(bmp.info.height);
            int nBitsPerPixel = (int)bmp.info.nBitsPerPixel;
            int num3 = (nBitsPerPixel * num + 31) / 32 * 4;
            int num4 = num3 * num2;
            int num5 = num3 - (num * nBitsPerPixel + 7) / 8;
            Color32[] array = bmp.imageData = new Color32[num * num2];
            if (aReader.BaseStream.Position + (long)num4 > aReader.BaseStream.Length)
            {
                Debug.LogError(string.Concat(new string[]
                {
                    "Unexpected end of file. (Have ",
                    (aReader.BaseStream.Position + (long)num4).ToString(),
                    " bytes, expected ",
                    aReader.BaseStream.Length.ToString(),
                    " bytes)"
                }));
                return;
            }
            BitStreamReader bitStreamReader = new BitStreamReader(aReader);
            for (int i = 0; i < num2; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    int num6 = (int)bitStreamReader.ReadBits(nBitsPerPixel);
                    if (num6 >= bmp.palette.Count)
                    {
                        Debug.LogError("Indexed bitmap has indices greater than it's color palette");
                        return;
                    }
                    array[j + i * num] = bmp.palette[num6];
                }
                bitStreamReader.Flush();
                for (int k = 0; k < num5; k++)
                {
                    aReader.ReadByte();
                }
            }
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00005450 File Offset: 0x00003650
        private static void ReadIndexedImageRLE4(BinaryReader aReader, BMPImage bmp)
        {
            int num = Mathf.Abs(bmp.info.width);
            int num2 = Mathf.Abs(bmp.info.height);
            Color32[] array = bmp.imageData = new Color32[num * num2];
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            while (aReader.BaseStream.Position < aReader.BaseStream.Length - 1L)
            {
                int num6 = (int)aReader.ReadByte();
                byte b = aReader.ReadByte();
                if (num6 > 0)
                {
                    for (int i = num6 / 2; i > 0; i--)
                    {
                        array[num3++ + num5] = bmp.palette[b >> 4 & 15];
                        array[num3++ + num5] = bmp.palette[(int)(b & 15)];
                    }
                    if ((num6 & 1) > 0)
                    {
                        array[num3++ + num5] = bmp.palette[b >> 4 & 15];
                    }
                }
                else if (b == 0)
                {
                    num3 = 0;
                    num4++;
                    num5 = num4 * num;
                }
                else
                {
                    if (b == 1)
                    {
                        break;
                    }
                    if (b == 2)
                    {
                        num3 += (int)aReader.ReadByte();
                        num4 += (int)aReader.ReadByte();
                        num5 = num4 * num;
                    }
                    else
                    {
                        for (int j = (int)(b / 2); j > 0; j--)
                        {
                            byte b2 = aReader.ReadByte();
                            array[num3++ + num5] = bmp.palette[b2 >> 4 & 15];
                            array[num3++ + num5] = bmp.palette[(int)(b2 & 15)];
                        }
                        if ((b & 1) > 0)
                        {
                            array[num3++ + num5] = bmp.palette[aReader.ReadByte() >> 4 & 15];
                        }
                        if (((b - 1) / 2 & 1) == 0)
                        {
                            aReader.ReadByte();
                        }
                    }
                }
            }
        }

        // Token: 0x0600003C RID: 60 RVA: 0x00005630 File Offset: 0x00003830
        private static void ReadIndexedImageRLE8(BinaryReader aReader, BMPImage bmp)
        {
            int num = Mathf.Abs(bmp.info.width);
            int num2 = Mathf.Abs(bmp.info.height);
            Color32[] array = bmp.imageData = new Color32[num * num2];
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            while (aReader.BaseStream.Position < aReader.BaseStream.Length - 1L)
            {
                int num6 = (int)aReader.ReadByte();
                byte b = aReader.ReadByte();
                if (num6 > 0)
                {
                    for (int i = num6; i > 0; i--)
                    {
                        array[num3++ + num5] = bmp.palette[(int)b];
                    }
                }
                else if (b == 0)
                {
                    num3 = 0;
                    num4++;
                    num5 = num4 * num;
                }
                else
                {
                    if (b == 1)
                    {
                        break;
                    }
                    if (b == 2)
                    {
                        num3 += (int)aReader.ReadByte();
                        num4 += (int)aReader.ReadByte();
                        num5 = num4 * num;
                    }
                    else
                    {
                        for (int j = (int)b; j > 0; j--)
                        {
                            array[num3++ + num5] = bmp.palette[(int)aReader.ReadByte()];
                        }
                        if ((b & 1) > 0)
                        {
                            aReader.ReadByte();
                        }
                    }
                }
            }
        }

        // Token: 0x0600003D RID: 61 RVA: 0x0000575C File Offset: 0x0000395C
        private static int GetShiftCount(uint mask)
        {
            for (int i = 0; i < 32; i++)
            {
                if ((mask & 1U) > 0U)
                {
                    return i;
                }
                mask >>= 1;
            }
            return -1;
        }

        // Token: 0x0600003E RID: 62 RVA: 0x00005784 File Offset: 0x00003984
        private static uint GetMask(int bitCount)
        {
            uint num = 0U;
            for (int i = 0; i < bitCount; i++)
            {
                num <<= 1;
                num |= 1U;
            }
            return num;
        }

        // Token: 0x0600003F RID: 63 RVA: 0x000057A8 File Offset: 0x000039A8
        private static bool ReadFileHeader(BinaryReader aReader, ref BMPFileHeader aFileHeader)
        {
            aFileHeader.magic = aReader.ReadUInt16();
            if (aFileHeader.magic != 19778)
            {
                return false;
            }
            aFileHeader.filesize = aReader.ReadUInt32();
            aFileHeader.reserved = aReader.ReadUInt32();
            aFileHeader.offset = aReader.ReadUInt32();
            return true;
        }

        // Token: 0x06000040 RID: 64 RVA: 0x000057F8 File Offset: 0x000039F8
        private static bool ReadInfoHeader(BinaryReader aReader, ref BitmapInfoHeader aHeader)
        {
            aHeader.size = aReader.ReadUInt32();
            if (aHeader.size < 40U)
            {
                return false;
            }
            aHeader.width = aReader.ReadInt32();
            aHeader.height = aReader.ReadInt32();
            aHeader.nColorPlanes = aReader.ReadUInt16();
            aHeader.nBitsPerPixel = aReader.ReadUInt16();
            aHeader.compressionMethod = (BMPComressionMode)aReader.ReadInt32();
            aHeader.rawImageSize = aReader.ReadUInt32();
            aHeader.xPPM = aReader.ReadInt32();
            aHeader.yPPM = aReader.ReadInt32();
            aHeader.nPaletteColors = aReader.ReadUInt32();
            aHeader.nImportantColors = aReader.ReadUInt32();
            int num = (int)(aHeader.size - 40U);
            if (num > 0)
            {
                aReader.ReadBytes(num);
            }
            return true;
        }

        // Token: 0x06000041 RID: 65 RVA: 0x000058AC File Offset: 0x00003AAC
        public static List<Color32> ReadPalette(BinaryReader aReader, BMPImage aBmp, bool aReadAlpha)
        {
            uint num = aBmp.info.nPaletteColors;
            if (num == 0U)
            {
                num = 1U << (int)aBmp.info.nBitsPerPixel;
            }
            List<Color32> list = new List<Color32>((int)num);
            int num2 = 0;
            while ((long)num2 < (long)((ulong)num))
            {
                byte b = aReader.ReadByte();
                byte g = aReader.ReadByte();
                byte r = aReader.ReadByte();
                byte a = aReader.ReadByte();
                if (!aReadAlpha)
                {
                    a = byte.MaxValue;
                }
                list.Add(new Color32(r, g, b, a));
                num2++;
            }
            return list;
        }

        // Token: 0x0400004A RID: 74
        private const ushort MAGIC = 19778;

        // Token: 0x0400004B RID: 75
        public bool ReadPaletteAlpha;

        // Token: 0x0400004C RID: 76
        public bool ForceAlphaReadWhenPossible;
    }
    public struct BMPFileHeader
    {
        // Token: 0x04000033 RID: 51
        public ushort magic;

        // Token: 0x04000034 RID: 52
        public uint filesize;

        // Token: 0x04000035 RID: 53
        public uint reserved;

        // Token: 0x04000036 RID: 54
        public uint offset;
    }
    public enum BMPComressionMode
    {
        // Token: 0x04000029 RID: 41
        BI_RGB,
        // Token: 0x0400002A RID: 42
        BI_RLE8,
        // Token: 0x0400002B RID: 43
        BI_RLE4,
        // Token: 0x0400002C RID: 44
        BI_BITFIELDS,
        // Token: 0x0400002D RID: 45
        BI_JPEG,
        // Token: 0x0400002E RID: 46
        BI_PNG,
        // Token: 0x0400002F RID: 47
        BI_ALPHABITFIELDS,
        // Token: 0x04000030 RID: 48
        BI_CMYK = 11,
        // Token: 0x04000031 RID: 49
        BI_CMYKRLE8,
        // Token: 0x04000032 RID: 50
        BI_CMYKRLE4
    }
    public class BitStreamReader
    {
        // Token: 0x06000043 RID: 67 RVA: 0x00005932 File Offset: 0x00003B32
        public BitStreamReader(BinaryReader aReader)
        {
            this.m_Reader = aReader;
        }

        // Token: 0x06000044 RID: 68 RVA: 0x00005941 File Offset: 0x00003B41
        public BitStreamReader(Stream aStream) : this(new BinaryReader(aStream))
        {
        }

        // Token: 0x06000045 RID: 69 RVA: 0x00005950 File Offset: 0x00003B50
        public byte ReadBit()
        {
            if (this.m_Bits <= 0)
            {
                this.m_Data = this.m_Reader.ReadByte();
                this.m_Bits = 8;
            }
            byte data = this.m_Data;
            int num = this.m_Bits - 1;
            this.m_Bits = num;
            return (byte)(data >> (num & 31) & 1);
        }

        // Token: 0x06000046 RID: 70 RVA: 0x0000599C File Offset: 0x00003B9C
        public ulong ReadBits(int aCount)
        {
            ulong num = 0UL;
            if (aCount <= 0 || aCount > 32)
            {
                throw new ArgumentOutOfRangeException("aCount", "aCount must be between 1 and 32 inclusive");
            }
            for (int i = aCount - 1; i >= 0; i--)
            {
                num |= (ulong)this.ReadBit() << i;
            }
            return num;
        }

        // Token: 0x06000047 RID: 71 RVA: 0x000059E3 File Offset: 0x00003BE3
        public void Flush()
        {
            this.m_Data = 0;
            this.m_Bits = 0;
        }

        // Token: 0x0400004D RID: 77
        private BinaryReader m_Reader;

        // Token: 0x0400004E RID: 78
        private byte m_Data;

        // Token: 0x0400004F RID: 79
        private int m_Bits;
    }
    public struct BitmapInfoHeader
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x0600002F RID: 47 RVA: 0x00004A10 File Offset: 0x00002C10
        public int absWidth
        {
            get
            {
                return Mathf.Abs(this.width);
            }
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000030 RID: 48 RVA: 0x00004A1D File Offset: 0x00002C1D
        public int absHeight
        {
            get
            {
                return Mathf.Abs(this.height);
            }
        }

        // Token: 0x04000037 RID: 55
        public uint size;

        // Token: 0x04000038 RID: 56
        public int width;

        // Token: 0x04000039 RID: 57
        public int height;

        // Token: 0x0400003A RID: 58
        public ushort nColorPlanes;

        // Token: 0x0400003B RID: 59
        public ushort nBitsPerPixel;

        // Token: 0x0400003C RID: 60
        public BMPComressionMode compressionMethod;

        // Token: 0x0400003D RID: 61
        public uint rawImageSize;

        // Token: 0x0400003E RID: 62
        public int xPPM;

        // Token: 0x0400003F RID: 63
        public int yPPM;

        // Token: 0x04000040 RID: 64
        public uint nPaletteColors;

        // Token: 0x04000041 RID: 65
        public uint nImportantColors;
    }
}
