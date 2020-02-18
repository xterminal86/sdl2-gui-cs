using System;
using System.Collections.Generic;
using SDL2;

public class Image
{
  public SDL.SDL_Rect Params;
  public IntPtr Texture;
  public uint Format;
  public int Access;
  SDL.SDL_RendererFlip FlipFlag = SDL.SDL_RendererFlip.SDL_FLIP_NONE;

  IntPtr _renderer;

  public Image(IntPtr renderer, string fileName, SDL.SDL_RendererFlip flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_NONE)
  {
    _renderer = renderer;

    IntPtr img = SDL_image.IMG_Load(fileName);

    Texture = SDL.SDL_CreateTextureFromSurface(_renderer, img);
    SDL.SDL_FreeSurface(img);

    SDL.SDL_QueryTexture(Texture, out Format, out Access, out Params.w, out Params.h);

    SetColor(255, 255, 255);

    FlipFlag = flipFlag;
  }

  public void SetColor(byte r, byte g, byte b)
  {
    SDL.SDL_SetTextureColorMod(Texture, r, g, b);
  }

  List<SDL.SDL_Rect> _slices = new List<SDL.SDL_Rect>();
  void AddSlice(int x, int y, int w, int h)
  {
    SDL.SDL_Rect slice;
    slice.x = x;
    slice.y = y;
    slice.w = w;
    slice.h = h;

    _slices.Add(slice);
  }

  SDL.SDL_Rect _sliceParams;
  public void Slice(SDL.SDL_Rect sliceParams)
  {
    _sliceParams = sliceParams;

    // UL
    AddSlice(0, 0, sliceParams.x, sliceParams.y);
    // UM
    AddSlice(sliceParams.x, 0, sliceParams.w - sliceParams.x, sliceParams.y);
    // UR
    AddSlice(sliceParams.w, 0, Params.w - sliceParams.w, sliceParams.y);
    // ML
    AddSlice(0, sliceParams.y, sliceParams.x, sliceParams.h - sliceParams.y);
    // M
    AddSlice(sliceParams.x, sliceParams.y, sliceParams.w - sliceParams.x, sliceParams.h - sliceParams.y);
    // MR
    AddSlice(sliceParams.w, sliceParams.y, Params.w - sliceParams.w, sliceParams.h - sliceParams.y);
    // LL
    AddSlice(0, sliceParams.h, sliceParams.x, Params.h - sliceParams.h);
    // LM
    AddSlice(sliceParams.x, sliceParams.h, sliceParams.w - sliceParams.x, Params.h - sliceParams.h);
    // LR
    AddSlice(sliceParams.w, sliceParams.h, Params.w - sliceParams.w, Params.h - sliceParams.h);

    /*
    for (int i = 0; i < _slices.Count; i++)
    {
      Console.WriteLine("{4}: [{0};{1}] - [{2};{3}]", _slices[i].x, _slices[i].y, _slices[i].w, _slices[i].h, i);
    }
    */
  }

  public void DrawSlice(int sliceIndex, int x, int y, int w, int h)
  {
    SDL.SDL_Rect src;
    src.x = _slices[sliceIndex].x;
    src.y = _slices[sliceIndex].y;
    src.w = _slices[sliceIndex].w;
    src.h = _slices[sliceIndex].h;

    SDL.SDL_Rect dst;
    dst.x = x;
    dst.y = y;
    dst.w = w;
    dst.h = h;

    SDL.SDL_RenderCopy(_renderer, Texture, ref src, ref dst);
  }

  void DrawSlices(int sx, int sy, int size, int spacing)
  {
    int x = sx;
    int y = sy;
    int sliceInd = 0;
    for (int i = 0; i < 3; i++)
    {
      for (int j = 0; j < 3; j++)
      {
        //DrawSlice(sliceInd, x + (Params.w / 3 + spacing) * j, y + (Params.h / 3 + spacing) * i, 32, 32);
        DrawSlice(sliceInd, x + (size + spacing) * j, y + (size + spacing) * i, size, size);
        sliceInd++;
      }
    }
  }

  public void DrawSliced(int x, int y, int w, int h)
  {
    if (_slices.Count == 0)
    {
      Draw(x, y, w, h);
    }
    else
    {
      // Central part
      DrawSlice(4,
                 x + _slices[0].w,
                 y + _slices[0].h,
                 w - (_slices[0].w + _slices[2].w),
                 h - (_slices[0].h + _slices[2].h));

      // Upper and lower middle parts
      DrawSlice(1, x + _slices[0].w, y, w - (_slices[0].w + _slices[2].w), _slices[1].h);
      DrawSlice(7, x + _slices[6].w, y + h - _slices[7].h, w - (_slices[0].w + _slices[2].w), _slices[7].h);

      // Left and right middle parts
      DrawSlice(3, x, y + _slices[0].h, _slices[3].w, h - (_slices[0].h + _slices[6].h));
      DrawSlice(5, x + w - _slices[5].w, y + _slices[2].h, _slices[5].w, h - (_slices[2].h + _slices[8].h));

      // Lastly, corners
      DrawSlice(0, x, y, _slices[0].w, _slices[0].h);
      DrawSlice(2, x + w - _slices[2].w, y, _slices[2].w, _slices[2].h);
      DrawSlice(6, x, y + h - _slices[6].h, _slices[6].w, _slices[6].h);
      DrawSlice(8, x + w - _slices[8].w, y + h - _slices[8].h, _slices[8].w, _slices[8].h);

      // FIXME: debug
      //DrawSlices(400, 0, 64, 5);
    }
  }

  public void Draw(int x, int y)
  {
    Draw(x, y, Params.w, Params.h);
  }

  public void Draw(int x, int y, int w, int h)
  {
    SDL.SDL_Rect src;
    src.x = 0;
    src.y = 0;
    src.w = Params.w;
    src.h = Params.h;

    SDL.SDL_Rect dst;
    dst.x = x;
    dst.y = y;
    dst.w = w;
    dst.h = h;

    SDL.SDL_Point point;
    point.x = w / 2;
    point.y = h / 2;

    SDL.SDL_RenderCopyEx(_renderer, Texture, ref src, ref dst, 0.0, ref point, FlipFlag);
  }

  public void Draw(int x, int y, double angle = 0.0)
  {
    Draw(x, y, Params.w, Params.h, angle);
  }

  public void Draw(int x, int y, int w, int h, double angle)
  {
    SDL.SDL_Rect src;
    src.x = 0;
    src.y = 0;
    src.w = Params.w;
    src.h = Params.h;

    SDL.SDL_Rect dst;
    dst.x = x;
    dst.y = y;
    dst.w = w;
    dst.h = h;

    SDL.SDL_Point point;
    point.x = w / 2;
    point.y = h / 2;

    SDL.SDL_RenderCopyEx(_renderer, Texture, ref src, ref dst, angle, ref point, FlipFlag);
  }
}
