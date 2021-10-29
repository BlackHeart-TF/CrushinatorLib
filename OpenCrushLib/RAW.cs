﻿
using System.Collections;
using System.Collections.Generic;
using System.IO;                            // Added for file read function
using System;                               // Added for array resize, could possibly use a list for this


namespace LibOpenCrush {
    /// <summary>
    /// This is strictly for texture testing only! Creates a spinning cube with the desired texture on it
    /// </summary>
    public class RAW
    {
        Metadata metadata;
        SubStream file;
        POD pod;

        public int textureSize = 256;           // Left public for debugging
        byte[] colorTable = new byte[256 * 3];    // Color table is 768 bytes
        byte[] textureData = new byte[256 * 256];   // Texture data. (Defaulting to 8bpp, 256x256px)
        byte[] opaData = new byte[256 * 256];   // Texture data. (Defaulting to 8bpp, 256x256px)

        // Visible flags in the inspector
        public int textureBytes = 0;            // Total bytes of texture
        public int opaBytes = 0;                // Total bytes of texture
        public bool isAlpha = false;            // Flag for alpha blended
        public bool isLoaded = false;           // Flag is set once texture if loaded from a file
        public bool debugMode = true;
        public Color[] PixelArray { get; private set; }
        public RAW(Metadata metadata, POD pod)
        {
            this.metadata = metadata;
            this.file = metadata.FileStream;
            this.pod = pod;

            MagicSolveRAWFile();
        }
        void MagicSolveRAWFile()
        {
            try                                                     // Attempt to read colortable (768 bytes, Adobe color table used for 256-color raw files)
            {
                StreamReader file = new StreamReader(this.file);
                var name = Path.GetFileNameWithoutExtension(this.metadata.Name);
                var ext = Path.GetExtension(this.metadata.Name);

                if (ext == ".RAW" && pod.FileTree.ContainsKey($"ART\\{name}.RAW"))
                {
                    if (pod.FileTree.ContainsKey($"ART\\{name}.ACT"))
                    {
                        var act = pod.FileTree[$"ART\\{name}.ACT"];
                        LoadAdobeColorTable(act);// Load adobe color table
                    }
                    LoadRAWTexture(this.metadata);
                }

                else if (ext == ".ACT" && pod.FileTree.ContainsKey($"ART\\{name}.ACT"))
                {
                    LoadAdobeColorTable(this.metadata);
                    if (pod.FileTree.ContainsKey($"ART\\{name}.RAW"))
                    {
                        var raw = pod.FileTree[$"ART\\{name}.RAW"];
                        LoadRAWTexture(raw);
                    }
                }
                // Attempt to load OPA Texture file (8bpp) 
                if (pod.FileTree.ContainsKey($"ART\\{name}.OPA"))
                {
                    var opa = pod.FileTree["ART\\4RSR5.OPA"];
                    LoadOPATexture(opa);                     // Load 8bpp RAW file
                }

                // Checking for null texture
                if (isLoaded == true)                                    // Checking if textureData's body is ready
                {
                    PixelArray = CreateRGBATexture(textureSize);            // It is generate a RAW texture!
                }
                else
                {
                    PixelArray = CreateNullTexture2D(256);                 // Failed to load, generate a rainbow!
                }
            }
            catch (IOException e)
            {
                string debugStr = "ERROR: Failed to load " + metadata.Name + "!";
                Console.WriteLine(debugStr);
                Console.WriteLine(e);
            }
        }


        /// <summary>
        /// Loads adobe color table "ACT" file to memory. (8-bit MTM/Evo textures)
        /// </summary>
        /// <param name="filename">Used for deubgging, file name only</param>
        /// <param name="filepath">full file path</param>
        void LoadAdobeColorTable(Metadata meta)
        {
            try                                                     // Attempt to read colortable (768 bytes, Adobe color table used for 256-color raw files)
            {
                if (meta.FileSize != 768)
                {
                    throw new IOException("ERROR: .ACT File should be eaxctly 768 bytes! Loading inhibited.");
                }
                    colorTable = meta.GetBytes();
                if (debugMode == true)
                {
                    string debugStr = "Found " + meta.Name + "! Loaded colortable.";
                        Console.WriteLine(debugStr);
                }
            }
            catch (IOException e)
            {
                string debugStr = "ERROR: Failed to load colortable " + meta.Name + "!";
                    Console.WriteLine(debugStr);
                    Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Loads 8bpp .RAW texture file into an array
        /// </summary>
        /// <param name="filename">Used for deubgging, file name only</param>
        /// <param name="filepath">full file path</param>
        void LoadRAWTexture(Metadata meta)
        {
            try                                                     // Attempt to read RAW file (768 bytes, Adobe color table used for 256-color raw files)
            {
                switch (meta.FileSize)
                {
                    case 256:       // 16 x 16
                        Array.Resize<byte>(ref textureData, 256);
                        textureSize = 16;
                        break;
                    case 1024:      // 32 x 32
                        Array.Resize<byte>(ref textureData, 1024);
                        textureSize = 32;
                        break;
                    case 4096:      // 64 x 64
                        Array.Resize<byte>(ref textureData, 4096);
                        textureSize = 64;
                        break;
                    case 16384:     // 128 x 128
                        Array.Resize<byte>(ref textureData, 16384);
                        textureSize = 128;
                        break;
                    case 65536:     // 256 x 256
                        Array.Resize<byte>(ref textureData, 65535);
                        textureSize = 256;
                        break;
                    case 262144:    // 512 x 512
                        Array.Resize<byte>(ref textureData, 262144);
                        textureSize = 512;
                        break;
                    default:        // Invalid file size
                        string debugstr = "ERROR: Invalid filesize, expected 8bpp texture with power of two. Filesize:" + meta.FileSize;
                        throw new IOException(debugstr);
                }
                textureData = meta.GetBytes();
                isLoaded = true;
                textureBytes = (int)meta.FileSize;                // Store this for OPA check

                if (debugMode == true)
                {
                    string debugStr = "Found " + meta.Name + "! Loaded texture.";
                        Console.WriteLine(debugStr);
                }
            }
            catch (IOException e)
            {
                string debugStr = "ERROR: Failed to load raw texture data " + meta.Name + "!";
                    Console.WriteLine(debugStr);
                    Console.WriteLine( e);
            }
        }

        /// <summary>
        /// Loads 8bpp .OPA texture file into an array
        /// </summary>
        /// <param name="filename">Used for deubgging, file name only</param>
        /// <param name="filepath">full file path</param>
        void LoadOPATexture(Metadata meta)
        {
            if (isLoaded == false) return;                          // No texture, no need for alpha
            try                                                     // Attempt to read OPA file (Same length as RAW, if it's not reject it)
            {
                if (meta.FileSize != textureBytes)                       // OPA should match RAW file
                {
                    string debugStr = "ERROR: OPA file size mismatch: " + meta.Name + "!\nLoad inhibited!";
                        Console.WriteLine(debugStr);
                    return;
                }

                switch (meta.FileSize)
                {
                    case 256:       // 16 x 16
                        Array.Resize<byte>(ref opaData, 256);
                        textureSize = 16;
                        break;
                    case 1024:      // 32 x 32
                        Array.Resize<byte>(ref opaData, 1024);
                        textureSize = 32;
                        break;
                    case 4096:      // 64 x 64
                        Array.Resize<byte>(ref opaData, 4096);
                        textureSize = 64;
                        break;
                    case 16384:     // 128 x 128
                        Array.Resize<byte>(ref opaData, 16384);
                        textureSize = 128;
                        break;
                    case 65536:     // 256 x 256
                        Array.Resize<byte>(ref opaData, 65535);
                        textureSize = 256;
                        break;
                    case 262144:    // 512 x 512
                        Array.Resize<byte>(ref opaData, 262144);
                        textureSize = 512;
                        break;
                    default:        // Invalid file size
                        string debugstr = "ERROR: Invalid filesize, expected 8bpp texture with power of two. Filesize:" + meta.FileSize;
                        throw new IOException(debugstr);
                }
                opaData = meta.GetBytes();
                isAlpha = true;

                if (debugMode == true)
                {
                    string debugStr = "Found " + meta.Name + "! Loaded texture.";
                        Console.WriteLine(debugStr);
                }
            }
            catch (IOException e)
            {
                string debugStr = "ERROR: Failed to load raw texture data " + meta.Name + "!";
                    Console.WriteLine(debugStr);
                    Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Take the data loaded into textureData and translate it to a Texture2D
        /// </summary>
        /// <param name="size">Texture Size</param>
        /// <returns></returns>
        Color[] CreateRGBATexture(int size)
        {
            int arraysize = size * size;
            Color[] colorArray = new Color[arraysize];
                Console.WriteLine(arraysize);
            for (int i = 0; i < arraysize; i++)
            {
                Color c = new Color(
                    colorTable[textureData[i] * 3],
                    colorTable[(textureData[i] * 3) + 1],
                    colorTable[(textureData[i] * 3) + 2]
                    );

                colorArray[i] = c;
            }

                if (debugMode == true) Console.WriteLine("RAW Texture Applied");
            return colorArray;
        }

        /// <summary>
        /// CreateNullTexture2D - Creates generic rainbow texture used in Evo's collisions boxes.
        /// </summary>
        /// <param name="size">Texture Size</param>
        /// <returns></returns>
        Color[] CreateNullTexture2D(int size)
        {
            Color[] colorArray = new Color[size * size * size];
            float r = 0.3f; // (byte)Math.Floor(1.0f / (size - 1.0f));
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        Color c = new Color((byte)(x * r), (byte)(y * r), (byte)(z * r));
                        colorArray[x + (y * size) + (z * size * size)] = c;
                    }
                }
            }

            if (debugMode == true) Console.WriteLine("NULL Texture Applied");
            return colorArray;
        }
    }
}