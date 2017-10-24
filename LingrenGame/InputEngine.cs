using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
#if ANDROID
using Microsoft.Devices.Sensors;
#endif

namespace Engine.Engines
{
    public class InputEngine : GameComponent
    {
        private static GamePadState previousPadState;
        private static GamePadState currentPadState;

        private static KeyboardState previousKeyState;
        private static KeyboardState currentKeyState;

        private static Vector2 previousMousePos;
        private static Vector2 currentMousePos;


        private static MouseState previousMouseState;
        private static MouseState currentMouseState;


#if ANDROID
        private static Vector2 previousAccelerometerReading;
        private static Accelerometer _acceleromter;
        private static Vector2 currentAcceleromoterReading;
        private static Point touchPoint;
        private static GestureType currentGestureType;

        private void _acceleromter_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            //need to consider orientation here,if support only landscape might be like this
            previousAccelerometerReading = CurrentAcceleromoterReading;
            currentAcceleromoterReading.Y = -(float)e.SensorReading.Acceleration.Y;
            currentAcceleromoterReading.X = -(float)e.SensorReading.Acceleration.X;
        }
#endif

        public InputEngine(Game _game)
            : base(_game)
        {
            currentPadState = GamePad.GetState(PlayerIndex.One);
            currentKeyState = Keyboard.GetState();

#if ANDROID
            _acceleromter = new Accelerometer();
            _acceleromter.CurrentValueChanged += _acceleromter_CurrentValueChanged;
            _acceleromter.Start();
            TouchPanel.EnabledGestures =
                    GestureType.Hold |
                    GestureType.Tap |
                    GestureType.DoubleTap |
                    GestureType.FreeDrag |
                    GestureType.Flick |
                    GestureType.Pinch;
#endif

            _game.Components.Add(this);
        }


        public static void ClearState()
        {
            previousMouseState = Mouse.GetState();
            currentMouseState = Mouse.GetState();
            previousKeyState = Keyboard.GetState();
            currentKeyState = Keyboard.GetState();
#if ANDROID
            currentGestureType = GestureType.None;
#endif
        }

        public override void Update(GameTime gametime)
        {
            previousPadState = currentPadState;
            previousKeyState = currentKeyState;

            currentPadState = GamePad.GetState(PlayerIndex.One);
            currentKeyState = Keyboard.GetState();

#if WINDOWS
            previousMouseState = currentMouseState;
            currentMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            currentMouseState = Mouse.GetState();
#endif

            KeysPressedInLastFrame.Clear();
            CheckForTextInput();
#if ANDROID
            HandleTouchInput();
#endif
            base.Update(gametime);
        }

        public List<string> KeysPressedInLastFrame = new List<string>();

        private void CheckForTextInput()
        {
            foreach (var key in Enum.GetValues(typeof(Keys)) as Keys[])
            {
                if (IsKeyPressed(key))
                {
                    KeysPressedInLastFrame.Add(key.ToString());
                    break;
                }
            }
        }

        public static bool IsButtonPressed(Buttons buttonToCheck)
        {
            if (currentPadState.IsButtonUp(buttonToCheck) && previousPadState.IsButtonDown(buttonToCheck))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsButtonHeld(Buttons buttonToCheck)
        {
            if (currentPadState.IsButtonDown(buttonToCheck))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsKeyHeld(Keys buttonToCheck)
        {
            if (currentKeyState.IsKeyDown(buttonToCheck))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool IsKeyPressed(Keys keyToCheck)
        {
            if (currentKeyState.IsKeyUp(keyToCheck) && previousKeyState.IsKeyDown(keyToCheck))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static GamePadState CurrentPadState
        {
            get { return currentPadState; }
            set { currentPadState = value; }
        }
        public static KeyboardState CurrentKeyState
        {
            get { return currentKeyState; }
        }

        public static MouseState CurrentMouseState
        {
            get { return currentMouseState; }
        }

        public static MouseState PreviousMouseState
        {
            get { return previousMouseState; }
        }

#if ANDROID
        public static Point TouchPoint
        {
            get
            {
                return touchPoint;
            }

            set
            {
                touchPoint = value;
            }
        }

        public static GestureType CurrentGestureType
        {
            get
            {
                GestureType ret = currentGestureType;
                currentGestureType = GestureType.None;
                return ret;
            }

            set
            {
                currentGestureType = value;
            }
        }

        public static Vector2 CurrentAcceleromoterReading
        {
            get
            {
                return currentAcceleromoterReading;
            }

            set
            {
                currentAcceleromoterReading = value;
            }
        }

        private void HandleTouchInput()
        {
            //currentGestureType = GestureType.None;
            TouchCollection touches = TouchPanel.GetState();
            while (TouchPanel.IsGestureAvailable)
            {
                // read the next gesture from the queue
                GestureSample gesture = TouchPanel.ReadGesture();
                if (touches.Count > 0 && touches[0].State == TouchLocationState.Pressed)
                {
                    // convert the touch position into a Point for hit testing
                    touchPoint = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                }
                // we can use the type of gesture to determine our behavior
                switch (gesture.GestureType)
                {
                    case GestureType.Tap:
                        currentGestureType = GestureType.DoubleTap;
                        touchPoint = new Point((int)gesture.Position.X, (int)gesture.Position.Y);
                        break;
                    case GestureType.DoubleTap:
                        touchPoint = new Point((int)gesture.Position.X, (int)gesture.Position.Y);
                        currentGestureType = GestureType.DoubleTap;
                        break;
                    case GestureType.Hold:
                        break;

                    // on drags, we just want to move the selected sprite with the drag
                    case GestureType.FreeDrag:
                        break;

                    // on flicks, we want to update the selected sprite's velocity with
                    // the flick velocity, which is in pixels per second.
                    case GestureType.Flick:
                        break;

                    // on pinches, we want to scale the selected sprite
                    case GestureType.Pinch:
                        // get the current and previous locations of the two fingers
                        Vector2 a = gesture.Position;
                        Vector2 aOld = gesture.Position - gesture.Delta;
                        Vector2 b = gesture.Position2;
                        Vector2 bOld = gesture.Position2 - gesture.Delta2;

                        // figure out the distance between the current and previous locations
                        float d = Vector2.Distance(a, b);
                        float dOld = Vector2.Distance(aOld, bOld);

                        // calculate the difference between the two and use that to alter the scale
                        float scaleChange = (d - dOld) * .01f;
                        break;
                }
            }

        }

#endif

#if WINDOWS

        public static bool IsMouseLeftClick()
        {
            if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else 
                return false;
        }

        public static bool IsMouseRightClick()
        {
            if (currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public static bool IsMouseRightHeld()
        {
            if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public static bool IsMouseLeftHeld()
        {
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public static Vector2 MousePosition
        {
            get { return currentMousePos; }
        }
#endif



    }
}
