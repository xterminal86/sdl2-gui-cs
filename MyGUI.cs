using System;
using System.Collections.Generic;

using SDL2;

public class MyGUI
{
  Stack<Canvas> _canvas = new Stack<Canvas>();

  Canvas _currentCanvas;
  public Canvas CurrentCanvas
  {
    get { return _currentCanvas; }
  }

  IntPtr _renderer;
  int _windowWidth, _windowHeight;
  public MyGUI(IntPtr renderer, int w, int h)
  {
    _renderer = renderer;
    _windowWidth = w;
    _windowHeight = h;
  }

  public void PushCanvas(Canvas newCanvas)
  {
    _canvas.Push(newCanvas);

    _currentCanvas = _canvas.Peek();
  }

  public void PopCanvas()
  {
    if (_canvas.Count != 0)
    {
      _canvas.Pop();
      _currentCanvas = null;
    }

    if (_canvas.Count != 0)
    {
      _currentCanvas = _canvas.Peek();
    }
  }

  public void HandleMouse(SDL.SDL_Event evt)
  {
    _currentCanvas.HandleMouse(evt);
  }

  public void Draw()
  {
    if (_currentCanvas != null)
    {
      _currentCanvas.Draw(_renderer);
    }
  }
}
