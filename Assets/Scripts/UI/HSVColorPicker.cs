using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DEMO.UI
{
    public class HSVColorPicker : MonoBehaviour, IDragHandler
    {
        public Color color;
        public GameObject panel;
        public Image selectedImg;
        private Texture2D colorText;

        [SerializeField] private Slider HSVSlider;
        [SerializeField] private RectTransform circle;

        [SerializeField] private Image colorImg;
        [SerializeField] private RectTransform colorRect;

        private float h, s, v = 1;
        private float width, height, radius;

        public event Action OnSelectedColor = null;
        public void UpdateSelectedColor()
        {
            color = Color.HSVToRGB(h, s, v);
            selectedImg.color = color;

            OnSelectedColor?.Invoke();
        }

        private void UpdateCirclePosition(Vector2 position)
        {
            float clampedX = Mathf.Clamp(position.x + width / 2 , 0, width);
            float clampedY = Mathf.Clamp(position.y + height / 2, 0, height);

            circle.anchoredPosition = new Vector2(clampedX, clampedY);
        }

        private void UpdateColorPalette()
        {
            for (int x = 0; x < (int)width; x++)
            {
                for (int y = 0; y < (int)height; y++)
                {
                    float s = (float)x / width;
                    float v = (float)y / height;
                    Color color = Color.HSVToRGB(h, s, v);
                    colorText.SetPixel(x, y, color);
                }
            }

            colorText.Apply();
            colorImg.sprite = Sprite.Create(colorText, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 localCursor;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(colorRect, eventData.position, null, out localCursor))
            {
                s = Mathf.Clamp((localCursor.x + width / 2) / width, 0, 1);
                v = Mathf.Clamp((localCursor.y + height / 2) / height, 0, 1);

                UpdateCirclePosition(localCursor);
                UpdateSelectedColor();
            }
        }

        private void OnHSVChanged(float hue)
        {
            h = hue;
            UpdateColorPalette();
            UpdateSelectedColor();
        }

        public void OnCloseBtnClicked()
        {
            panel.SetActive(false);
            OnSelectedColor = null;
            selectedImg = null;
        }

        public void Init(Image image)
        {
            this.selectedImg = image;

            width = colorRect.rect.width;
            height = colorRect.rect.height;
            radius = circle.sizeDelta.x / 2f;

            colorText = new Texture2D((int)width, (int)height);
            colorText.filterMode = FilterMode.Point;

            HSVSlider.value = h;
            HSVSlider.onValueChanged.AddListener(OnHSVChanged);

            Color color = selectedImg.color;
            Color.RGBToHSV(color, out h, out s, out v);
            UpdateColorPalette();

            Vector2 position = new Vector2(s * width + radius, v * height + radius);
            UpdateCirclePosition(position);

            panel.SetActive(true);
        }
    }
}