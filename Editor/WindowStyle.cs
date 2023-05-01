using System.Collections.Generic;
using UnityEngine;

namespace Editor
{
    public enum Styles
    {
        Block,
        BuildItem,
        BuildItemInfo,
        Lable,
        Image,
        Popup,
        PopupLable
    }
    
    public class WindowStyle
    {
        private readonly Dictionary<Styles, GUIStyle> _styleDict = new ();

        public WindowStyle()
        {
            SetBlockStyle();
            SetBuildItem();
            SetBuildItemInfo();
            SetLable();
            SetPopup();
            SetPopupLable();
        }
        
        public GUIStyle GetStyle(Styles style)
        {
            return _styleDict[style];
        }

        private void SetBlockStyle()
        {
            var style = new GUIStyle
            {
                margin = new RectOffset(3, 3, 5, 5),
                border = new RectOffset(4, 4, 4, 4),
                padding = new RectOffset(4, 4, 4, 4)
            };

            Texture2D backgroundTexture = new Texture2D(1, 1);
            Color backgroundColor = new Color(155f / 255, 164f / 255, 181f / 255);
            backgroundTexture.SetPixel(0, 0, backgroundColor);
            backgroundTexture.Apply();
            style.normal.background = backgroundTexture;
            style.alignment = TextAnchor.MiddleCenter;

            _styleDict[Styles.Block] = style;
        }

        private void SetBuildItem()
        {
            var style = new GUIStyle
            {
                margin = new RectOffset(3, 3, 5, 5),
                border = new RectOffset(2, 2, 2, 2),
                padding = new RectOffset(4, 4, 4, 4)
            };

            // Texture2D backgroundTexture = new Texture2D(1, 1);
            // Color backgroundColor = new Color(155f / 255, 164f / 255, 181f / 255);
            // backgroundTexture.SetPixel(0, 0, backgroundColor);
            // backgroundTexture.Apply();
            // style.normal.background = backgroundTexture;
            // style.alignment = TextAnchor.MiddleCenter;

            _styleDict[Styles.BuildItem] = style;
        }
        
        private void SetBuildItemInfo()
        {
            var style = new GUIStyle
            {
                margin = new RectOffset(3, 3, 5, 5),
                border = new RectOffset(2, 2, 2, 2),
                padding = new RectOffset(4, 4, 4, 4),
            };

            // Texture2D backgroundTexture = new Texture2D(1, 1);
            // Color backgroundColor = new Color(155f / 255, 164f / 255, 181f / 255);
            // backgroundTexture.SetPixel(0, 0, backgroundColor);
            // backgroundTexture.Apply();
            // style.normal.background = backgroundTexture;
            // style.alignment = TextAnchor.MiddleCenter;

            _styleDict[Styles.BuildItemInfo] = style;
        }

        private void SetPopup()
        {
            var style = new GUIStyle
            {
                margin = new RectOffset(0, 0, 10, 10),
            };
            
            _styleDict[Styles.Popup] = style;
        }
        
        private void SetPopupLable()
        {
            var style = new GUIStyle
            {
                margin = new RectOffset(3, 0, 0, 0),
                normal = {textColor = Color.black},
                alignment = TextAnchor.MiddleCenter
            };
            
            _styleDict[Styles.PopupLable] = style;
        }
        
        private void SetLable()
        {
            Texture2D backgroundTexture = new Texture2D(1, 1);
            Color backgroundColor = new Color(57f / 255, 72f / 255, 103f / 255);
            backgroundTexture.SetPixel(0, 0, backgroundColor);
            backgroundTexture.Apply();
            
            var style = new GUIStyle
            {
                normal =
                {
                    background = backgroundTexture,
                    textColor = Color.white,
                },
                alignment = TextAnchor.MiddleCenter,
            };

            // Texture2D backgroundTexture = new Texture2D(1, 1);
            // Color backgroundColor = new Color(155f / 255, 164f / 255, 181f / 255);
            // backgroundTexture.SetPixel(0, 0, backgroundColor);
            // backgroundTexture.Apply();
            // style.normal.background = backgroundTexture;
            // style.alignment = TextAnchor.MiddleCenter;

            _styleDict[Styles.Lable] = style;
        }
    }
}