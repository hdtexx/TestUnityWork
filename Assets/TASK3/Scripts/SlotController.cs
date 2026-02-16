using AxGrid.Base;
using AxGrid.Model;
using AxGrid.Path;
using AxGrid.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace TASK3.Scripts
{
    public class SlotController : MonoBehaviourExtBind
    {
        [Header("Слоты")]
        [SerializeField] private Image[] _items;

        private int[] _indices;
        private int _totalSprites;
        private bool _stopping;
        private CPath _stopPath;
        private CPath _pulsePath;

        private float _itemHeight;
        private string _collectionName;

        [OnStart]
        private void Initialize()
        {
            _itemHeight = Model.GetFloat("ItemHeight");
            _collectionName = Model.Get<string>("CollectionName");
            _totalSprites = AxSpriteCollection.spriteIndexes[_collectionName].Count;
            _indices = new int[] { 0, 1, 2 };

            for (var i = 0; i < _items.Length; i++)
            {
                SetPositionY(_items[i], _itemHeight - i * _itemHeight);
                ApplySprite(_items[i], _indices[i]);
            }
        }

        [OnUpdate]
        private void Tick()
        {
            _pulsePath?.Update(Time.deltaTime);

            if (_stopping)
            {
                _stopPath?.Update(Time.deltaTime);
                return;
            }

            var speed = Model.GetFloat("Speed");
            if (speed <= 0f)
                return;

            MoveItems(speed * Time.deltaTime);
            RecycleItems();
        }

        [Bind("OnIsStoppingChanged")]
        private void OnIsStoppingChanged()
        {
            if (false == Model.GetBool("IsStopping"))
                return;

            var minDistance = Model.GetFloat("SnapDistance");
            var stopDuration = Model.GetFloat("DecelerationTime");

            var currentSpeed = Model.GetFloat("Speed");
            var speedDistance = currentSpeed * stopDuration * 0.25f;
            var snapDistance = Mathf.Max(minDistance, speedDistance);

            var centerY = GetClosestToCenterItem().rectTransform.localPosition.y;
            var overshoot = (centerY % _itemHeight + _itemHeight) % _itemHeight;
            var fullStops = Mathf.Max(Mathf.CeilToInt((snapDistance - overshoot) / _itemHeight), 0);
            var totalTravel = overshoot + fullStops * _itemHeight;

            StartStopAnimation(totalTravel, stopDuration);
        }

        private void StartStopAnimation(float totalTravel, float duration)
        {
            _stopping = true;
            Model.Set("Speed", 0f);

            var previousMoved = 0f;

            _stopPath = new CPath()
                .EasingCircEaseOut(duration, 0f, totalTravel, moved =>
                {
                    var frameDelta = moved - previousMoved;
                    previousMoved = moved;
                    MoveItems(frameDelta);
                    RecycleItems();
                })
                .Action(() =>
                {
                    SnapToGrid();
                    _stopping = false;
                    _stopPath = null;
                    StartPulse();
                });
        }

        private void StartPulse()
        {
            var centerItem = GetClosestToCenterItem();
            var pulseScale = Model.GetFloat("PulseScale");
            var halfDuration = Model.GetFloat("PulseDuration") * 0.5f;

            _pulsePath = new CPath()
                .EasingQuadEaseInOut(halfDuration, 1f, pulseScale, scale =>
                {
                    centerItem.rectTransform.localScale = new Vector3(scale, scale, 1f);
                })
                .EasingQuadEaseInOut(halfDuration, pulseScale, 1f, scale =>
                {
                    centerItem.rectTransform.localScale = new Vector3(scale, scale, 1f);
                })
                .Action(() =>
                {
                    centerItem.rectTransform.localScale = Vector3.one;
                    _pulsePath = null;
                    Model.Set("AnimationDone", true);
                });
        }

        private void MoveItems(float delta)
        {
            for (var i = 0; i < _items.Length; i++)
            {
                var pos = _items[i].rectTransform.localPosition;
                pos.y -= delta;
                _items[i].rectTransform.localPosition = pos;
            }
        }

        private void RecycleItems()
        {
            for (var i = 0; i < _items.Length; i++)
            {
                if (_items[i].rectTransform.localPosition.y > -_itemHeight * 1.5f)
                    continue;

                var topY = GetTopItemY();
                SetPositionY(_items[i], topY + _itemHeight);

                var topIndex = GetTopItemIndex(i);
                _indices[i] = WrapIndex(topIndex - 1);
                ApplySprite(_items[i], _indices[i]);
            }
        }

        private void SnapToGrid()
        {
            var center = GetClosestToCenterItem();
            var offset = center.rectTransform.localPosition.y;

            for (var i = 0; i < _items.Length; i++)
            {
                var pos = _items[i].rectTransform.localPosition;
                pos.y -= offset;
                _items[i].rectTransform.localPosition = pos;
            }
        }

        private Image GetClosestToCenterItem()
        {
            var closestIndex = 0;
            var minDist = float.MaxValue;

            for (var i = 0; i < _items.Length; i++)
            {
                var dist = Mathf.Abs(_items[i].rectTransform.localPosition.y);
                if (dist >= minDist)
                    continue;

                minDist = dist;
                closestIndex = i;
            }

            return _items[closestIndex];
        }

        private float GetTopItemY()
        {
            var maxY = float.MinValue;
            for (var i = 0; i < _items.Length; i++)
                if (_items[i].rectTransform.localPosition.y > maxY)
                    maxY = _items[i].rectTransform.localPosition.y;
            return maxY;
        }

        private int GetTopItemIndex(int excludeSlot)
        {
            var maxY = float.MinValue;
            var topIndex = 0;

            for (var i = 0; i < _items.Length; i++)
            {
                if (i == excludeSlot)
                    continue;
                if (_items[i].rectTransform.localPosition.y <= maxY)
                    continue;

                maxY = _items[i].rectTransform.localPosition.y;
                topIndex = _indices[i];
            }

            return topIndex;
        }

        private int WrapIndex(int index)
        {
            return (index % _totalSprites + _totalSprites) % _totalSprites;
        }

        private void SetPositionY(Image item, float y)
        {
            var pos = item.rectTransform.localPosition;
            pos.y = y;
            item.rectTransform.localPosition = pos;
        }

        private void ApplySprite(Image item, int index)
        {
            item.sprite = AxSpriteCollection.GetSprite(_collectionName, index);
        }
    }
}