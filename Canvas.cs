using System;
using System.Collections.Generic;

using SDL2;

public class Canvas
{
  MyGUI _guiRef;

  List<ItemGUI> _guiItems = new List<ItemGUI>();

  public Canvas(MyGUI guiRef)
  {
    _guiRef = guiRef;
  }

  public void AddElement(ItemGUI itemToAdd)
  {
    _guiItems.Add(itemToAdd);
  }

  public void HandleMouse(SDL.SDL_Event evt)
  {
    foreach (var item in _guiItems)
    {
      item.HandleMouse(evt);
    }
  }

  public void Draw(IntPtr renderer)
  {
    foreach (var item in _guiItems)
    {
      item.Draw(renderer);
    }
  }
}
