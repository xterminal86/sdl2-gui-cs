using System;
using System.Drawing;
using System.Runtime.InteropServices;
using SDL2;
using System.Collections.Generic;

public class Printer
{
  class CharData
  {
    public int X = -1;
    public int Y = -1;
  };

  public enum TextAlignment
  {
    LEFT = 0,
    CENTER,
    RIGHT
  };

  const int TilesetCharWidth = 16;
  const int TilesetCharHeight = 16;

  public const int CharWidth = 8;
  public const int CharHeight = 16;

  const string Tileset8x16Base64 = "iVBORw0KGgoAAAANSUhEUgAAAIAAAAEAAgMAAAAGl5Y0AAAACVBMVEUAAGcAAAD////NzL25AAAAAXRSTlMAQObYZgAAAAFiS0dEAIgFHUgAAAAJcEhZcwAALiMAAC4jAXilP3YAAAAHdElNRQfjAhQHNi6pOtobAAAHYElEQVRo3uVZy47jOAwUfDL4FcGcCH2l0KeGv4LwSdBXbhUpyXYe3fPa2caOg8SJU6IpPktySndHa/F+eTwB5JrrPWCpYtJyAMsSI5aylCmhqoqo8Ep6/7ZvakvCGJsS3gGABAdQQqtSxHir1sRMZAUgqezbBGSrGSNLzapvb7kEgDLTtkOf2ioBHH1bcQW3I6BmAN7e8JdLyPUALGVK4C2ySVn8Fbcw6JOT5qkD/p7TpBRKwFyzz4KGWnA+TzNTFozhgNemtu/0xe8GPBxQc+1uf6oTNcVkVHMzDZffTQ0OhIFV4fJuiztASwsMRYCUbrQJWFKrqRWpvAWDZtyCjoJqNykEwNGuw3bSAV8tISIKFB8SlhOAg4weMQK6Dgtu0VyDQwKDoM8CYzfekXMgSHDr9cZInXZYtmTNx8s0WGMYhCVDh3mL58cLU//B46VqPFQHQJ6PJYBzEDrJHkCMBjV6QvHnEzuIrTcAVlsVKLmmw+oAGtXgIYYMAXpklHb9lY61rAE4hZ1NAF22WwAQCZXh1+c2AJLM7iXI4VjOdQCoQ0A0TQmGKZilOU1+LsXSScmiVHIYKgDltS/kh5z1Xx+ZqeKeYJqw4p6jO4cPaaQFVoItaS4dflTPQc0F3xTF1bPMwZ6ftAUB1X8wpi6AyFFc2ns+mjW8EBMWHcwtSa829XqCvEw+FkJ1+pGRxYId+hDgd9WQAEnmtcKdegGMYuj+CbHU1gbAjlngTKVonsVNJPwdse33/dI+/sF4SA251LwAwyfw61KrNiaY9nhQHqb95OZFnrFCezzwL9QG1VVRIw4AOEEAvMAgILRosdQB5oCK0155J6/tJQPQPEzAH9K+7QFo9ZDASwMAJYeEDuhBC++Ge8VP4BI6lTwA+NUBlub8KOMAaAdAQZ2GSvJOQ+UCQ9VuqK9+LJ8BWLsRC9n8Df5AfsYXuZS3CfNu45ZE9z8AYIPsp95DJgD0zAG0YGGGjb7RQyl5AnKssWgLe7r1mDQNqhcA1goWhDwleNAxYQnYt+AfkqLvD4CnfAA4np1rsQnYeywTiDjlt0VIjyagDgAVpBacppeUoEkgq1TTWFIJQGzQUOtBND4NivoxN/zlQ34ZwEJHLReLAKm8ojWCJZ1afJT6Xhv1rkJOwK1/v5ZQpn1YEnnm8cL1CZi57r09sWk7SUFdAofwW5DbN432lCMg0NS8ojuA3B4cNQDWAd7UvhfAW1wBUMGjdQD0BMjKTsHPiA3EgfKTgMf6YIO6vTjsM0AOymdfopqgYueU9WVkRe/d7ROAy4XP3e8Ntu71RYPCEyDuF0C8a1uvMLzSE0qDejD/B4BtbaxTknMB57vkwwSwgFDaA4BVyXytJO72BwBGKe8QcWiTYoC4MBYYB4uHK4OafCIMNXLi3nd6/8ueBc1lpXZXNH4rLRk5botrI+GeTuh7DSaLQ/FFrxRPf3FLYp3n9NmdRVfHSlGCxAfTprFc+npda1w18JJuzKsSL40sM/6FvsKm5GnrLdzbeCet4YvR3C2obvYaQ6ozIioAKxPf+qEh87QwdXcfgKC+A+ASxAcOQLDKiw7JKwPe/kpjFv4KgL62w59ahn5Ka2UmP7NLozJreBZUvvolqREFuTivjnAx5umyRWRbRIH5Es7bfdWbejll7vTiN6OeeKGN2WpZdat1X4a//MV0KLoSkGQjsYy13gEg4UT4MDex6rdnAEhoLiHXJwAJCQ7YyyMAs/Dltiu52yOALa0DEI/c7bLeMzA2+/Qa3/LMEZf42MvTvZ9TtLzqGD+5TtBXARPLJlRtsVkwDlVYVUnNsobbZQL6FCOOOkM/AFhIPNgAzbTOW4xlKafFTSWz3ZoeMWpRmJSGCqII+dzo4WLuSH/3e56AvXb6mLjVQOV3yEg2AVbnBH2dIBE5J4B29TjfatyIpQ7PAW/bm/ZMz1xPIBIGoGe7Sv04KS3pxyndPgH8UH1Yvn3b977hF8ZWnvnpat5u7+85fwD4oxKaA7Biw19wOT+fSWC6ivqexlVC44sjQ4J638C1OwkcxU/RmRTXWTwB/E8kzA2VAaEfxvsrAhoAGStaPofAm+dHCb2EyJFDPMYnx8fYKeF4vDJiakRkfPtbACMf+3a0nh458dS+CIA/8Ad8mfzXSwm+4HiU4IHQpFGCxI8S0REYN5PoOIcuXK3+fYDfsEPJR0p365JY9yw2Or36kqs/DuWqwFNXJLooAb7RyB1GdS4a0aG+oZ2d2/seHx97DT7OZ4vVAbuB7PnDkd25W6z1pO7bvpG8BhdMvlvQAY1bo0pASChOLy8SAtDkAuA3shEJpqAnHQJgIBOxEHUlNAjUaRaNT0QT99qlVH9GBqplph+sI7/cE60D0Ole28s9n+20SG3QwRMj5tOfZbsHyHz6F1sB29xQmISyky6yWnA/B0ziS/ZWOiDX1LcDSHwPQAtAbGDHhkI82vBtNDvxurEdQa1zfwiqdlnm+SwAOPZNToAR7gG4rhq7BD5By8d+QzqlzE944t8A/ANF/pjceH3KIwAAAABJRU5ErkJggg==";

  IntPtr _rendererRef;

  Dictionary<char, CharData> _charDataByChar = new Dictionary<char, CharData>();

  public static readonly Printer Instance = new Printer();
  Printer()
  {
  }

  bool _isInitialized = false;

  IntPtr _tileset;
  public void Init(IntPtr rendererRef)
  {
    if (_isInitialized)
    {
      return;
    }

    _rendererRef = rendererRef;

    var bytes = System.Convert.FromBase64String(Tileset8x16Base64);
    IntPtr d = Marshal.AllocHGlobal(bytes.Length);
    Marshal.Copy(bytes, 0, d, bytes.Length);
    IntPtr fromMem = SDL.SDL_RWFromMem(d, bytes.Length);
    var surf = SDL_image.IMG_Load_RW(fromMem, 1);
    _tileset = SDL.SDL_CreateTextureFromSurface(_rendererRef, surf);

    int start = 0;
    for (int i = start; i < 256; i++)
    {
      CharData cd = new CharData();
      cd.X = (i % TilesetCharWidth);
      cd.Y = i / TilesetCharHeight;

      _charDataByChar.Add((char)i, cd);
    }

    _isInitialized = true;
  }

  void PrintChar(int x, int y, char c, float scale = 1.0f)
  {
    char charToPrint = _charDataByChar.ContainsKey(c) ? c : '?';

    if (charToPrint == '\n' || charToPrint == '\r')
    {
      charToPrint = ' ';
    }

    var cd = _charDataByChar[charToPrint];

    SDL.SDL_Rect src;
    src.x = cd.X * CharWidth;
    src.y = cd.Y * CharHeight;
    src.w = CharWidth;
    src.h = CharHeight;

    SDL.SDL_Rect dst;
    dst.x = x;
    dst.y = y;
    dst.w = (int)((float)CharWidth * scale);
    dst.h = (int)((float)CharHeight * scale);

    SDL.SDL_Point point;
    point.x = 0;
    point.y = 0;

    SDL.SDL_RenderCopyEx(_rendererRef, _tileset, ref src, ref dst, 0.0, ref point, SDL.SDL_RendererFlip.SDL_FLIP_NONE);
  }

  int FindMaxNewline(string str)
  {
    int pos = 0;
    int counter = 0;
    for (int i = 0; i < str.Length; i++)
    {
      if ((str[i] == '\n' || (i == (str.Length - 1))) && counter > pos)
      {
        pos = counter;
        counter = 0;
      }

      counter++;
    }

    if (pos == 0)
    {
      pos = str.Length;
    }

    return pos;
  }

  public void Print(int x, int y, string str, Color col, float scale = 1.0f, TextAlignment al = TextAlignment.LEFT)
  {
    if (str == null)
    {
      str = string.Empty;
    }

    int maxNewlineLength = FindMaxNewline(str);

    int newStartX = x;
    int newStartY = y;

    int scaledW = (int)((float)CharWidth * scale);
    int scaledH = (int)((float)CharHeight * scale);

    if (al == TextAlignment.CENTER)
    {
      newStartX -= (scaledW * (str.Length / 2));
    }
    else if (al == TextAlignment.RIGHT)
    {
      newStartX -= (scaledW * maxNewlineLength);
    }

    int startX = newStartX;
    int startY = newStartY;

    foreach (var c in str)
    {
      SDL.SDL_SetTextureColorMod(_tileset, col.R, col.G, col.B);

      PrintChar(startX, startY, c, scale);

      startX += scaledW;

      if (c == '\n')
      {
        startY += scaledH;
        startX = newStartX;
      }
    }
  }
}

