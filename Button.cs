using System;
using System.Drawing;
using System.Collections.Generic;

using SDL2;

public class Button : ItemGUI
{
  Image _imgNormal;
  Image _imgPressed;
  Image _imgDisabled;
  Image _imgHighlighted;

  Dictionary<State, Image> _imageToDraw = new Dictionary<State, Image>();

  float _textScale = 1.0f;
  public float TextScale
  {
    set { _textScale = value; }
    get { return _textScale; }
  }

  string _textToShow;
  public void SetText(string text)
  {
    _textToShow = text;
  }

  public Button(Image imgNormal,
                Image imgPressed,
                Image imgDisabled,
                Image imgHighlighted,
                int x, int y, int w, int h,
                string text = "")
  {
    _imgNormal = imgNormal;
    _imgPressed = imgPressed;
    _imgDisabled = imgDisabled;
    _imgHighlighted = imgHighlighted;

    _dimensions.x = x;
    _dimensions.y = y;
    _dimensions.w = w;
    _dimensions.h = h;

    _textToShow = text;

    _imageToDraw[State.HIGHLIGHTED] = _imgHighlighted;
    _imageToDraw[State.NORMAL] = _imgNormal;
    _imageToDraw[State.PRESSED] = _imgPressed;
    _imageToDraw[State.DISABLED] = _imgDisabled;
  }

  protected override void OnMouseUpInt()
  {
    base.OnMouseUpInt();

    if (_imgHighlighted != _imgNormal)
    {
      _state = State.HIGHLIGHTED;
    }
  }

  protected override void OnMouseClickInt()
  {
    if (OnMouseClick != null)
    {
      OnMouseClick(this);
    }
  }

  public override bool HandleMouse(SDL.SDL_Event evt)
  {
    bool res = base.HandleMouse(evt);

    if (_buttonHold && _mouseLockedFlag)
    {
      if (!_cursorInside)
      {
        _state = State.NORMAL;
      }
      else
      {
        _state = State.PRESSED;
      }
    }

    return res;
  }

  public override void Draw(IntPtr renderer)
  {
    if (_imageToDraw[_state] != null)
    {
      _imageToDraw[_state].DrawSliced(_dimensions.x, _dimensions.y, _dimensions.w, _dimensions.h);

      Printer.Instance.Print(_dimensions.x + _dimensions.w / 2 - (int)((float)(Printer.CharWidth / 2) * _textScale) + 1,
                             _dimensions.y + _dimensions.h / 2 - (int)((float)(Printer.CharHeight / 2) * _textScale) + 1,
                             _textToShow,
                             (_state == State.DISABLED) ? Color.Gray : Color.Black,
                             _textScale,
                             Printer.TextAlignment.CENTER);

    }
  }
}
