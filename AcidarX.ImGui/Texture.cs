﻿using System;
using Silk.NET.OpenGL;

namespace AcidarX.ImGui
{
    public enum TextureCoordinate
    {
        S = TextureParameterName.TextureWrapS,
        T = TextureParameterName.TextureWrapT,
        R = TextureParameterName.TextureWrapR
    }

    internal class Texture : IDisposable
    {
        public const GLEnum Srgb8Alpha8 = GLEnum.Srgb8Alpha8;
        public const GLEnum Rgb32F = GLEnum.Rgb32f;

        public static float? MaxAniso;
        private readonly GL _gl;
        public readonly uint GlTexture;
        public readonly GLEnum InternalFormat;
        public readonly uint MipmapLevels;
        public readonly string Name;
        public readonly uint Width, Height;

        public unsafe Texture
            (GL gl, string name, int width, int height, IntPtr data, bool generateMipmaps = false, bool srgb = false)
        {
            _gl = gl;
            MaxAniso ??= gl.GetFloat(GLEnum.MaxTextureMaxAnisotropy);
            Name = name;
            Width = (uint) width;
            Height = (uint) height;
            InternalFormat = srgb ? Srgb8Alpha8 : GLEnum.Rgba8;
            MipmapLevels =
                (uint) (generateMipmaps == false ? 1 : (int) Math.Floor(Math.Log(Math.Max(Width, Height), 2)));

            _gl.CreateTexture(TextureTarget.Texture2D, Name, out GlTexture);
            _gl.TextureStorage2D(GlTexture, MipmapLevels, InternalFormat, Width, Height);

            _gl.TextureSubImage2D(GlTexture, 0, 0, 0, Width, Height, PixelFormat.Bgra, PixelType.UnsignedByte,
                (void*) data);

            if (generateMipmaps)
            {
                _gl.GenerateTextureMipmap(GlTexture);
            }

            SetWrap(TextureCoordinate.S, TextureWrapMode.Repeat);
            SetWrap(TextureCoordinate.T, TextureWrapMode.Repeat);

            _gl.TextureParameter(GlTexture, TextureParameterName.TextureMaxLevel, MipmapLevels - 1);
        }

        public void Dispose()
        {
            _gl.DeleteTexture(GlTexture);
        }

        public void SetMinFilter(TextureMinFilter filter)
        {
            _gl.TextureParameter(GlTexture, TextureParameterName.TextureMinFilter, (int) filter);
        }

        public void SetMagFilter(TextureMagFilter filter)
        {
            _gl.TextureParameter(GlTexture, TextureParameterName.TextureMagFilter, (int) filter);
        }

        public void SetAnisotropy(float level)
        {
            const TextureParameterName textureMaxAnisotropy = (TextureParameterName) 0x84FE;
            _gl.TextureParameter(GlTexture, textureMaxAnisotropy, Util.Clamp(level, 1, MaxAniso.GetValueOrDefault()));
        }

        public void SetLod(int @base, int min, int max)
        {
            _gl.TextureParameter(GlTexture, TextureParameterName.TextureLodBias, @base);
            _gl.TextureParameter(GlTexture, TextureParameterName.TextureMinLod, min);
            _gl.TextureParameter(GlTexture, TextureParameterName.TextureMaxLod, max);
        }

        public void SetWrap(TextureCoordinate coord, TextureWrapMode mode)
        {
            _gl.TextureParameter(GlTexture, (TextureParameterName) coord, (int) mode);
        }
    }
}