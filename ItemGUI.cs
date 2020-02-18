using System;

using SDL2;

public abstract class ItemGUI
{
  public enum State
  {
    NORMAL = 0,
    PRESSED,
    HIGHLIGHTED,
    DISABLED
  }

  protected SDL.SDL_Rect _dimensions;

  protected State _state = State.NORMAL;
  public State CurrentState
  {
    get { return _state; }
  }

  public CallbackFrom OnMouseDown;
  public CallbackFrom OnMouseUp;
  public CallbackFrom OnMouseEnter;
  public CallbackFrom OnMouseExit;
  public CallbackFrom OnMouseClick;

  public int MouseX, MouseY;

  protected SDL2.SDL.SDL_EventType _lastEvent = SDL.SDL_EventType.SDL_LASTEVENT;

  protected UInt64 _objectId = 0;
  public UInt64 ObjectId
  {
    get { return _objectId; }
  }

  public ItemGUI()
  {
    _objectId = IdGen.GetNewId();
  }

  public void Enable()
  {
    _state = State.NORMAL;
  }

  public void Disable()
  {
    _state = State.DISABLED;
  }

  protected bool CheckCoords(int mx, int my)
  {
    if (mx > _dimensions.x
     && my > _dimensions.y
     && mx < _dimensions.x + _dimensions.w
     && my < _dimensions.y + _dimensions.h)
    {
      return true;
    }

    return false;
  }

  public virtual void Draw(IntPtr renderer) { }

  protected virtual void OnMouseEnterInt() 
  {
    //Console.WriteLine("\t{0} [mouse enter]", DateTime.Now.TimeOfDay);
    _state = State.HIGHLIGHTED; 
  }

  protected virtual void OnMouseExitInt()  
  {
    //Console.WriteLine("\t{0} [mouse exit]", DateTime.Now.TimeOfDay);
    _state = State.NORMAL;      
  }

  protected virtual void OnMouseDownInt()  
  {
    //Console.WriteLine("\t{0} [mouse down]", DateTime.Now.TimeOfDay);
    _state = State.PRESSED;     
  }

  protected virtual void OnMouseUpInt()    
  {
    //Console.WriteLine("\t{0} [mouse up]", DateTime.Now.TimeOfDay);
    _state = State.NORMAL;      
  }

  protected virtual void OnMouseClickInt() { }

  protected bool _mouseLockedFlag = false;
  protected bool _mouseOverFlag = false;
  protected bool _buttonHold = false;
  protected bool _cursorInside = false;

  public virtual bool HandleMouse(SDL.SDL_Event evt)
  {
    if (_state == State.DISABLED)
    {
      return true;
    }

    uint bm = SDL.SDL_GetMouseState(out MouseX, out MouseY);

    if (evt.type != _lastEvent)
    {
      _lastEvent = evt.type;
      //Console.WriteLine("{0} {1}", _lastEvent, bm);
    }

    bool wasHandled = false;
    bool wasClick = false;

    _cursorInside = CheckCoords(evt.button.x, evt.button.y);
    if (evt.type == SDL.SDL_EventType.SDL_MOUSEMOTION)
    {
      if (_cursorInside)
      {
        if (!_mouseOverFlag)
        {
          _mouseOverFlag = true;
          OnMouseEnterInt();
        }
      }
      else
      {
        if (_mouseOverFlag)
        {
          _mouseOverFlag = false;
          OnMouseExitInt();
        }
      }
    }

    if (bm == 1)
    {
      if (!_buttonHold)
      {
        _buttonHold = true;

        if (_cursorInside)
        {
          if (!_mouseLockedFlag)
          {
            _mouseLockedFlag = true;
            wasHandled = true;
            OnMouseDownInt();
          }
        }
        else
        {
          _mouseLockedFlag = false;
          wasHandled = true;
        }
      }
    }
    else if (bm == 0)
    {
      if (_buttonHold)
      {
        _buttonHold = false;

        if (_cursorInside)
        {
          if (_mouseLockedFlag)
          {
            _mouseLockedFlag = false;
            wasClick = true;
            wasHandled = true;
            OnMouseUpInt();
          }
        }
        else
        {
          wasHandled = true;
          _mouseLockedFlag = false;
        }
      }
    }

    if (wasClick)
    {
      OnMouseClickInt();
      wasHandled = true;
    }

    return wasHandled;
  }
}
