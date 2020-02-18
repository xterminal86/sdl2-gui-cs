using System;
using System.Drawing;
using System.Collections.Generic;
using SDL2;

public delegate void Callback();
public delegate void CallbackFrom(ItemGUI from);

public static class IdGen
{
  static UInt64 _counter = 1;
  public static UInt64 GetNewId()
  {
    return _counter++;
  }
};

public class Application
{
  public IntPtr Window;
  public IntPtr Renderer;

  readonly int[] WindowSize = { 100, 100, 800, 600 };

  bool _shouldExit = false;

  MyGUI _gui;

  Dictionary<UInt64, int> _countersByButtonId = new Dictionary<UInt64, int>();
  public void Init()
  {
    SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
    Window = SDL.SDL_CreateWindow("SDL2 GUI",
                                   WindowSize[0], WindowSize[1],
                                   WindowSize[2], WindowSize[3],
                                   SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE |
                                   SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN |
                                   SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS);

    SDL.SDL_SetWindowResizable(Window, SDL.SDL_bool.SDL_FALSE);

    int drivers = SDL.SDL_GetNumRenderDrivers();
    for (int i = 0; i < drivers; i++)
    {
      SDL.SDL_RendererInfo info;
      SDL.SDL_GetRenderDriverInfo(i, out info);

      Renderer = SDL.SDL_CreateRenderer(Window, i, (SDL.SDL_RendererFlags)info.flags);
      if (Renderer.ToInt32() != 0)
      {
        Printer.Instance.Init(Renderer);
        break;
      }
    }

    SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG);

    LoadImages();

    SDL.SDL_Rect sliceParams;
    sliceParams.x = 4;
    sliceParams.y = 4;
    sliceParams.w = 27;
    sliceParams.h = 27;

    _images[0].Slice(sliceParams);
    _images[1].Slice(sliceParams);
    _images[2].Slice(sliceParams);
    _images[3].Slice(sliceParams);

    _gui = new MyGUI(Renderer, WindowSize[2], WindowSize[3]);

    Canvas window = new Canvas(_gui);

    for (int i = 0; i < 4; i++)
    {
      Button b = new Button(_images[0], _images[1], _images[2], _images[3], 0, i*25, 100, 25, "Click Me!");
      b.OnMouseClick = ClickHandler;

      if (i == 3)
      {
        b.Disable();
        b.SetText("Disabled");
        _toEnableRef = b;
      }
      else
      {
        _countersByButtonId.Add(b.ObjectId, 0);
      }

      window.AddElement(b);
    }

    Button enabler = new Button(_images[0], _images[1], _images[2], _images[3], 0, 5 * 25, 100, 25, "Enabler");
    enabler.OnMouseClick = EnablerHandler;
    window.AddElement(enabler);

    Button b2 = new Button(_images[0], _images[1], _images[2], _images[3], 775, 0, 25, 25, "X");
    b2.TextScale = 1.5f;
    b2.OnMouseClick = ExitApp;

    window.AddElement(b2);

    _gui.PushCanvas(window);
  }

  public void ExitApp(ItemGUI from)
  {
    _shouldExit = true;
  }

  Button _toEnableRef;
  public void EnablerHandler(ItemGUI from)
  {
    if (_toEnableRef.CurrentState == ItemGUI.State.DISABLED)
    {
      _toEnableRef.SetText("Enabled");
      _toEnableRef.Enable();
    }
    else
    {
      _toEnableRef.SetText("Disabled");
      _toEnableRef.Disable();
    }
  }

  string _textToShow = String.Empty;
  public void ClickHandler(ItemGUI from)
  {
    //_textToShow = string.Format("(objId {0}) [mouse click]", from.ObjectId);
    //Console.WriteLine(_textToShow);

    if (_countersByButtonId.ContainsKey(from.ObjectId))
    {
      _countersByButtonId[from.ObjectId]++;
    }
  }

  List<string> _files = new List<string>()
  {
    "sprites/btn-normal.png",
    "sprites/btn-pressed.png",
    "sprites/btn-disabled.png",
    "sprites/btn-highlighted.png",
    "sprites/slice-test.png",
    "sprites/slice-test2.png"
  };

  List<Image> _images = new List<Image>();
  void LoadImages()
  {
    foreach (var name in _files)
    {
      Image im = new Image(Renderer, name);
      _images.Add(im);
    }
  }

  void Display()
  {
    SDL.SDL_SetRenderDrawColor(Renderer, 128, 128, 128, 255);
    SDL.SDL_RenderClear(Renderer);

    _gui.Draw();

    int offset = 0;
    foreach (var kvp in _countersByButtonId)
    {
      Printer.Instance.Print(120, offset * 25 + 6, kvp.Value.ToString(), Color.Black);
      offset++;
    }

    /*
    Printer.Instance.Print(WindowSize[2] / 2, 
                           WindowSize[3] / 2, 
                           _textToShow, 
                           Color.Black, 
                           3.0f, 
                           Printer.TextAlignment.CENTER);
    */

    SDL.SDL_RenderPresent(Renderer);
  }

  void QueryInput(SDL.SDL_Keycode key)
  {
    switch (key)
    {
      case SDL.SDL_Keycode.SDLK_ESCAPE:
        _shouldExit = true;
        break;

      default:
        //Console.WriteLine("{0}", key);
        break;
    }
  }

  public void Run()
  {
    SDL.SDL_Event evt;

    while (!_shouldExit)
    {
      while (SDL.SDL_PollEvent(out evt) != 0)
      {
        _gui.HandleMouse(evt);

        switch (evt.type)
        {
          case SDL.SDL_EventType.SDL_KEYDOWN:
            QueryInput(evt.key.keysym.sym);
            break;

          case SDL.SDL_EventType.SDL_QUIT:
            _shouldExit = true;
            break;
        }
      }

      Display();
    }

    SDL.SDL_DestroyRenderer(Renderer);
    SDL.SDL_DestroyWindow(Window);
    SDL.SDL_Quit();

    Console.WriteLine("Goodbye!");
  }
}
